using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    /// <summary>
    /// 用于管理和存储玩家"重排"道具数量的ScriptableObject
    /// 采用单例模式，方便全局访问
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ShuffleHolder")]
    public class ShuffleHolder : SingletonScriptableObject<ShuffleHolder>
    {
        #region 默认数据
        [Space(10, order = 0)]
        [Header("默认数据", order = 1)]
        [Tooltip("游戏开始时玩家默认拥有的重排次数")]
        [SerializeField]
        private int defCount = 5;
        #endregion 默认数据

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_mahjong_shuffle"; // 用于PlayerPrefs存储当前重排次数的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        private static int _count; // 存储当前重排次数
        #endregion 临时变量

        /// <summary>
        /// 当前的重排次数
        /// 在首次访问时会自动从PlayerPrefs加载
        /// </summary>
        public static int Count
        {
            get { if (!loaded) Instance.Load(); return _count; }
            private set { _count = value; }
        }

        /// <summary>
        /// 默认重排次数
        /// </summary>
        public int DefaultCount => defCount;

        /// <summary>
        /// 重排次数变更时触发的事件
        /// </summary>
        public UnityEvent<int> ChangeEvent;
        /// <summary>
        /// 重排次数数据加载完成时触发的事件
        /// </summary>
        public UnityEvent<int> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ;count: " + Count);
        }

        /// <summary>
        /// 增加指定数量的重排次数
        /// </summary>
        /// <param name="count">要增加的数量</param>
        public static void Add(int count)
        {
            if (Instance)
            {
                Instance.SetCount(Count + count);
            }
        }

        /// <summary>
        /// 设置重排次数并保存结果
        /// </summary>
        /// <param name="count">要设置的数量</param>
        public void SetCount(int count)
        {
            count = Mathf.Max(0, count); // 保证数量不为负
            bool changed = (Count != count);
            Count = count;
            if (changed)
            {
                PlayerPrefs.SetInt(saveKey, Count); // 如果数量有变，则存入PlayerPrefs
            }
            if (changed) ChangeEvent?.Invoke(Count); // 触发变更事件
        }

        /// <summary>
        /// 从PlayerPrefs加载已序列化的重排次数，如果不存在则设置为默认值
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
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为ShuffleHolder提供在Unity Inspector中的自定义编辑器界面
    /// </summary>
    [CustomEditor(typeof(ShuffleHolder))]
    public class ShuffleHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ShuffleHolder cH = (ShuffleHolder)target;
            EditorGUILayout.LabelField("当前数量: " + ShuffleHolder.Count);

            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("增加 5次"))
                {
                    ShuffleHolder.Add(5);
                }
                if (GUILayout.Button("设置为 5次"))
                {
                    cH.SetCount(5);
                }
                if (GUILayout.Button("清空次数"))
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
                if (GUILayout.Button("打印日志"))
                {
                    Debug.Log("Shuffles: " + ShuffleHolder.Count);

                }
                if (GUILayout.Button("加载已保存的数据"))
                {
                    cH.Load();
                }
            }
        }
    }
#endif
}