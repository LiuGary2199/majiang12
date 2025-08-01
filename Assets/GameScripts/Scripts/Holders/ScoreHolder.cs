using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    /// <summary>
    /// 管理和存储玩家分数的ScriptableObject。
    /// 它同时管理当前游戏会话的实时分数和每个关卡的历史最高分。
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ScoreHolder")]
    public class ScoreHolder : SingletonScriptableObject<ScoreHolder>
    {
        #region 默认数据
        [Space(10, order = 0)]
        [Header("默认数据", order = 1)]

        [Tooltip("如果为true，则只保存最好成绩；否则，保存最近一次的成绩。")]
        [SerializeField]
        private bool saveBestResult = true;
        #endregion 默认数据

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_score"; // 用于PlayerPrefs存储分数列表的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        private static int _count; // 存储当前游戏会话的实时分数
        private static List<int> levelsScore;  // 存储每个关卡历史最高分的列表
        #endregion 临时变量

        /// <summary>
        /// 平均分，可能用于计算星级等。可通过 SetAverageScore 方法设置。
        /// </summary>
        public static int AverageScore { get; private set; }

        /// <summary>
        /// 当前游戏会话的实时分数。
        /// </summary>
        public static int Count
        {
            get { if (!loaded) Instance.Load(); return _count; }
            private set { _count = value; }
        }
        /// <summary>
        /// 提供对每个关卡历史最高分列表的只读访问。
        /// </summary>
        public static IList<int> LevelsScoreStore => levelsScore.AsReadOnly(); 

        /// <summary>
        /// 实时分数变化时触发的事件。
        /// </summary>
        public UnityEvent<int> ChangeEvent;
        /// <summary>
        /// 历史最高分列表加载完成时触发的事件。
        /// </summary>
        public UnityEvent<List<int>> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ;count: " + Count);
        }

        /// <summary>
        /// 增加指定数量到当前实时分数。
        /// </summary>
        /// <param name="count">要增加的分数，可为负数。</param>
        public static void Add(int count)
        {
            if (Instance)
            {
                Instance.SetCount(Count + count);
            }
        }

        /// <summary>
        /// 设置当前实时分数。注意：此方法不会自动保存到PlayerPrefs。
        /// </summary>
        /// <param name="count">要设置的分数。</param>
        public void SetCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (Count != count);
            Count = count;
            if (changed)
            {
                ChangeEvent?.Invoke(Count);
            }
        }

        /// <summary>
        /// 从PlayerPrefs加载序列化的历史最高分列表。
        /// 使用 PlayerPrefsExtension.GetObject 将存储的字符串反序列化为列表。
        /// </summary>
        public void Load()
        {
            levelsScore = new List<int>();
            loaded = true;
            Debug.Log("scoreholder:" + PlayerPrefs.GetString(saveKey, "none"));
            // PlayerPrefs不能直接存List，所以用一个包装类(ListWrapperStruct)和扩展方法来序列化/反序列化
            ListWrapperStruct<int> lW = PlayerPrefsExtension.GetObject<ListWrapperStruct<int>>(saveKey, new ListWrapperStruct<int>(levelsScore));
            levelsScore = lW.list;
            LoadEvent?.Invoke(lW.list);
        }

        /// <summary>
        /// 在通过一个关卡后，保存该关卡的分数。
        /// </summary>
        /// <param name="passedLevel">已通过的关卡编号（从0开始）。</param>
        public void Save(int passedLevel)
        {
            if (levelsScore == null) levelsScore = new List<int>();
            int count = levelsScore.Count;
            // 如果列表长度不够，则扩展列表以容纳新关卡的分数
            if (count <= passedLevel)
            {
                levelsScore.AddRange(new int[passedLevel - count + 1]);
            }
            
            // 根据 saveBestResult 标志决定是保存最高分还是当前分数
            int score = (saveBestResult) ? Mathf.Max(Count, levelsScore[passedLevel]) : Count;
            levelsScore[passedLevel] = score;
           
            // 将更新后的列表包装并序列化，存入PlayerPrefs
            ListWrapperStruct <int> lW = new ListWrapperStruct <int> (levelsScore);
            PlayerPrefsExtension.SetObject<ListWrapperStruct<int>>(saveKey, lW);
        }

        /// <summary>
        /// 重置分数数据。
        /// </summary>
        public void SetDefaultData()
        {
            PlayerPrefs.DeleteKey(saveKey);
            SetCount(0);
            levelsScore = new List<int>();
        }

        /// <summary>
        /// 设置平均分。
        /// </summary>
        /// <param name="averageScore">平均分数值</param>
        public void SetAverageScore(int averageScore)
        {
            AverageScore = Mathf.Max(1, averageScore); // 确保平均分不为0
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为ScoreHolder提供在Unity Inspector中的自定义编辑器界面。
    /// </summary>
    [CustomEditor(typeof(ScoreHolder))]
    public class ScoreHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ScoreHolder tH = (ScoreHolder)target;
            EditorGUILayout.LabelField("当前实时分数: " + ScoreHolder.Count);

            #region test
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("增加 100 分"))
                {
                    ScoreHolder.Add(100);
                }

                if (GUILayout.Button("减少 100 分"))
                {
                    ScoreHolder.Add(-100);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");

                if (GUILayout.Button("重置为默认值"))
                {
                    tH.SetDefaultData();
                }

                if (GUILayout.Button("打印日志"))
                {
                    Debug.Log("Score: " + ScoreHolder.Count);

                }

                if (GUILayout.Button("加载数据"))
                {
                    tH.Load();
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion test
        }
    }
#endif
}