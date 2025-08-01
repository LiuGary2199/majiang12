using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

/*
  player data holder 
  26.06.2021
 */
namespace Mkey
{
    /// <summary>
    /// 用于管理和存储玩家个人数据（如此处的玩家昵称）的ScriptableObject。
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerDataHolder")]
    public class PlayerDataHolder : SingletonScriptableObject<PlayerDataHolder>
    {
        #region 默认数据
        [Space(10, order = 0)]
        [Header("默认数据", order = 1)]

        [Tooltip("默认的玩家昵称")]
        [SerializeField]
        private string defFullName = "Good Player";
        #endregion 默认数据

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_fullname"; // 用于PlayerPrefs存储玩家昵称的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        #endregion 临时变量
        private static string _fullName; // 静态变量，存储当前玩家昵称

        /// <summary>
        /// 当前的玩家昵称。首次访问时会自动加载。
        /// </summary>
        public static string FullName
        {
            get { if (!loaded) Instance.Load(); return _fullName; }
            private set { _fullName = value; }
        }

        [Tooltip("玩家昵称变更时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<string> ChangeUnityEvent;
        [Tooltip("玩家昵称数据加载时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<string> LoadUnityEvent;
        /// <summary>
        /// 玩家昵称变更时触发的C#事件（Action）
        /// </summary>
        public Action<string> ChangeEvent;
        /// <summary>
        /// 玩家昵称数据加载时触发的C#事件（Action）
        /// </summary>
        public Action<string> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ; full name: " + FullName);
        }

        /// <summary>
        /// 设置玩家的昵称
        /// </summary>
        /// <param name="fName">新的昵称</param>
        public void SetFullName(string fName)
        {
            // 如果传入的名称为空或null，则保留现有名称，防止意外清空
            fName = string.IsNullOrEmpty(fName) ? FullName : fName;
            bool changed = (FullName != fName);
            FullName = fName;
            if (changed)
            {
                PlayerPrefs.SetString(saveKey, FullName);
                ChangeEvent?.Invoke(FullName);
                ChangeUnityEvent?.Invoke(FullName);
            }
        }

        /// <summary>
        /// 从PlayerPrefs加载序列化的玩家昵称，如果不存在则设为默认值
        /// </summary>
        public void Load()
        {
            loaded = true;
            _fullName = PlayerPrefs.GetString(saveKey, defFullName);
            LoadEvent?.Invoke(FullName);
            LoadUnityEvent?.Invoke(FullName);
        }

        /// <summary>
        /// 重置为默认的玩家昵称
        /// </summary>
        public void SetDefaultData()
        {
            PlayerPrefs.DeleteKey(saveKey);
            SetFullName(defFullName);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为PlayerDataHolder提供在Unity Inspector中的自定义编辑器界面
    /// </summary>
    [CustomEditor(typeof(PlayerDataHolder))]
    public class PlayerDataHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            PlayerDataHolder tH = (PlayerDataHolder)target;
            EditorGUILayout.LabelField("玩家昵称: " + PlayerDataHolder.FullName);

            #region test
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");

                if (GUILayout.Button("重置为默认值"))
                {
                    tH.SetDefaultData();
                }

                if (GUILayout.Button("打印日志"))
                {
                    Debug.Log("Player data: " );

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