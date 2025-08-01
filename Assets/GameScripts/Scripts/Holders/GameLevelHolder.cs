using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

/*
  game level holder
  17.06.2021
 */

namespace Mkey
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameLevelHolder")]
    public class GameLevelHolder : SingletonScriptableObject<GameLevelHolder>
    {
        #region keys
        [SerializeField]
        private string saveKey = "mk_gamelevel";
        #endregion keys

        #region temp vars
        private static bool loaded = false;
        private static int _count;
        private static List<int> levelsStars;  // temporary array for store levels stars
        #endregion temp vars

        public static int TopPassedLevel
        {
            get
            { if (!loaded) Instance.Load(); return _count; }
            private set { _count = value; }
        }

        // 当前关卡号，始终从PlayerPrefs读取和保存，保证持久化
        private static int _currentLevelCache = -1;
        public static int CurrentLevel
        {
            get
            {
                if (_currentLevelCache < 0)
                {
                    if (PlayerPrefs.HasKey("current_level"))
                        _currentLevelCache = PlayerPrefs.GetInt("current_level");
                    else
                        _currentLevelCache = 0;
                }
                return _currentLevelCache;
            }
            set
            {
                _currentLevelCache = value;
                PlayerPrefs.SetInt("current_level", value);
                PlayerPrefs.Save();
            }
        }

        public UnityEvent <int> ChangePassedEvent;
        public UnityEvent <int> LoadEvent;
        public UnityEvent <int> PassLevelEvent;
        public UnityEvent <int> StartLevelEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ;top passed level: " + TopPassedLevel);
            // CurrentLevel属性已自动处理恢复，无需手动赋值
        }

        public void Load()
        {
            loaded = true;
            TopPassedLevel = PlayerPrefs.GetInt(saveKey, -1);
            LoadEvent?.Invoke(TopPassedLevel);
        }

        public void Save()
        {
            // save top passed level
            PlayerPrefs.SetInt(saveKey, TopPassedLevel);
        }

        public void SetDefaultData()
        {
            TopPassedLevel = -1;
            PlayerPrefs.DeleteKey(saveKey);
            ChangePassedEvent?.Invoke(TopPassedLevel);
        }

        public void PassLevel()
        {
            if (CurrentLevel > TopPassedLevel)
            {
                TopPassedLevel = CurrentLevel; 
                ChangePassedEvent?.Invoke(TopPassedLevel);
            }

            Save();
            PassLevelEvent?.Invoke(CurrentLevel);
        }

        public static void StartLevel()
        {
          if(Instance) Instance.StartLevelEvent?.Invoke(CurrentLevel);
        }

        /// <summary>
        /// 保存当前关卡号到本地（可选，实际CurrentLevel属性已自动保存）
        /// </summary>
        public static void SaveCurrentLevel()
        {
            PlayerPrefs.SetInt("current_level", CurrentLevel);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置当前关卡并保存
        /// </summary>
        public static void SetCurrentLevelAndSave(int level)
        {
            CurrentLevel = level;
            // CurrentLevel属性已自动保存

            ADAnalyze.Overtone.GalaxyVagueBow(CurrentLevel);
        }

        /// <summary>
        /// 设置当前关卡并触发UI更新事件
        /// </summary>
        public static void SetCurrentLevelAndUpdateUI(int level)
        {
            CurrentLevel = level;
            // 触发LoadEvent来更新UI
            if (Instance)
            {
                Instance.LoadEvent?.Invoke(CurrentLevel);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameLevelHolder))]
    public class GameLevelHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GameLevelHolder tH = (GameLevelHolder)target;
            EditorGUILayout.LabelField("Top passed level: " + GameLevelHolder.TopPassedLevel);

            #region test
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Reset to default"))
                {
                    tH.SetDefaultData();
                }

                if (GUILayout.Button("Log"))
                {
                    Debug.Log("Top passed level: " + GameLevelHolder.TopPassedLevel);

                }

                if (GUILayout.Button("Load data"))
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