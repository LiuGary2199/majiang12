using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;
#endif

/*
  player game coins holder
  04.05.2021
  27.05.2021
 */
namespace Mkey
{
    /// <summary>
    /// 用于管理玩家游戏金币的ScriptableObject
    /// 采用单例模式，方便全局访问
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/CoinsHolder")]
    public class CoinsHolder : SingletonScriptableObject<CoinsHolder>
    {
        #region 默认数据
        [Space(10, order = 0)]
        [Header("默认数据", order = 1)]
        [Tooltip("游戏开始时玩家默认拥有的金币数量")]
        [SerializeField]
        private int defCount = 500;

        [Tooltip("玩家首次关联Facebook时奖励的金币数量")]
        [SerializeField]
        private int defFBCoinsCount = 100;
        #endregion 默认数据

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_match_coins"; // 用于PlayerPrefs存储当前金币数量的键
        [SerializeField]
        private string saveFbCoinsKey = "mk_fbcoins"; // 用于PlayerPrefs存储是否已领取Facebook奖励的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        private static int _count; // 存储当前金币数量
        #endregion 临时变量

        /// <summary>
        /// 当前的金币数量
        /// 在首次访问时会自动从PlayerPrefs加载
        /// </summary>
        public static int Count
        {
            get { if (!loaded) Instance.Load(); return _count; }
            private set { _count = value; }
        }

        /// <summary>
        /// 默认金币数量
        /// </summary>
        public int DefaultCount => defCount;

        /// <summary>
        /// 金币数量变更时触发的事件
        /// </summary>
        public UnityEvent<int> ChangeEvent;
        /// <summary>
        /// 金币数据加载完成时触发的事件
        /// </summary>
        public UnityEvent<int> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ;count: " + Count);
        }

        /// <summary>
        /// 增加指定数量的金币
        /// </summary>
        /// <param name="count">要增加的金币数量</param>
        public static void Add(int count)
        {
            if (Instance)
            {
                Instance.SetCount(Count + count);
            }
        }

        /// <summary>
        /// 设置金币数量并保存结果
        /// </summary>
        /// <param name="count">要设置的金币数量</param>
        public void SetCount(int count)
        {
            count = Mathf.Max(0, count); // 保证金币数量不为负
            bool changed = (Count != count);
            Count = count;
            if (changed)
            {
                PlayerPrefs.SetInt(saveKey, Count); // 如果数量有变，则存入PlayerPrefs
            }
            if (changed) ChangeEvent?.Invoke(Count); // 触发变更事件
        }

        /// <summary>
        /// 增加Facebook奖励金币（仅限一次），并保存标记
        /// </summary>
        public void AddFbCoins()
        {
            if (!PlayerPrefs.HasKey(saveFbCoinsKey) || PlayerPrefs.GetInt(saveFbCoinsKey) == 0)
            {
                PlayerPrefs.SetInt(saveFbCoinsKey, 1); // 标记为已领取
                Add(defFBCoinsCount);
            }
        }

        /// <summary>
        /// 从PlayerPrefs加载已序列化的金币数量，如果不存在则设置为默认值
        /// </summary>
        public void Load()
        {
            loaded = true;
            Count = PlayerPrefs.GetInt(saveKey, defCount);
            LoadEvent?.Invoke(Count); // 触发加载完成事件
        } 

        /// <summary>
        /// 重置为默认数据
        /// </summary>
        public void SetDefaultData()
        {
            SetCount(defCount);
            PlayerPrefs.SetInt(saveFbCoinsKey, 0); // 重置Facebook奖励领取状态
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为CoinsHolder提供在Unity Inspector中的自定义编辑器界面
    /// </summary>
    [CustomEditor(typeof(CoinsHolder))]
    public class CoinsHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            CoinsHolder cH = (CoinsHolder)target;
            EditorGUILayout.LabelField("当前金币: " + CoinsHolder.Count);

            // 折叠测试菜单
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("增加 500 金币"))
                {
                    CoinsHolder.Add(500);
                }
                if (GUILayout.Button("设置为 500 金币"))
                {
                    cH.SetCount(500);
                }
                if (GUILayout.Button("清空金币"))
                {
                    cH.SetCount(0);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("重置为默认值"))
                {
                    cH.SetDefaultData();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("打印金币日志"))
                {
                    Debug.Log("金币: " + CoinsHolder.Count);

                }
                if (GUILayout.Button("加载已保存的金币"))
                {
                    cH.Load();
                }
            }
        }
    }
#endif
}