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
    /// 用于管理和存储所有玩家头像以及玩家当前选择的ScriptableObject。
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/AvatarsHolder")]
    public class AvatarsHolder : SingletonScriptableObject<AvatarsHolder>
    {
        [Tooltip("包含所有可选玩家头像的数组")]
        [SerializeField]
        public Sprite [] avatars;

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_avatar"; // 用于PlayerPrefs存储当前头像索引的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        #endregion 临时变量

        private static int _avatarIndex; // 静态变量，存储当前选择的头像索引

        /// <summary>
        /// 当前选择的头像索引。首次访问时会自动加载。
        /// </summary>
        public static int AvatarIndex
        {
            get { if (!loaded) Instance.Load(); return _avatarIndex; }
            private set { _avatarIndex = value; }
        }

        [Tooltip("头像变更时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<int> ChangeUnityEvent;
        [Tooltip("头像数据加载时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<int> LoadUnityEvent;
        /// <summary>
        /// 头像变更时触发的C#事件（Action）
        /// </summary>
        public Action<int> ChangeEvent;
        /// <summary>
        /// 头像数据加载时触发的C#事件（Action）
        /// </summary>
        public Action<int> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ; avatar index: " + AvatarIndex);
        }

        /// <summary>
        /// 设置新的头像索引
        /// </summary>
        /// <param name="index">新的头像索引</param>
        public void SetIndex(int index)
        {
            if (index < 0) index = 0;
            bool changed = (AvatarIndex != index);
            AvatarIndex = index;
            if (changed)
            {
                PlayerPrefs.SetInt(saveKey, AvatarIndex);
                ChangeEvent?.Invoke(AvatarIndex);
                ChangeUnityEvent?.Invoke(AvatarIndex);
            }
        }

        /// <summary>
        /// 从PlayerPrefs加载序列化的头像数据，如果不存在则设为默认值
        /// </summary>
        public void Load()
        {
            loaded = true;
            _avatarIndex = PlayerPrefs.GetInt(saveKey, 0);
            LoadEvent?.Invoke(AvatarIndex);
            LoadUnityEvent?.Invoke(AvatarIndex);
        }

        /// <summary>
        /// 重置为默认头像数据
        /// </summary>
        public void SetDefaultData()
        {
            PlayerPrefs.DeleteKey(saveKey);
            SetIndex(0); // 设置为第一个头像
        }

        /// <summary>
        /// 获取当前选中的头像精灵 (Sprite)
        /// </summary>
        /// <returns>当前头像的精灵对象</returns>
        public Sprite GetAvatarSprite()
        {
            if (avatars.Length == 0) return null;
            if (avatars.Length > 0 && AvatarIndex >= 0 && AvatarIndex < avatars.Length) return avatars[AvatarIndex];
            return avatars[avatars.Length - 1]; // 作为备用，返回最后一个
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为AvatarsHolder提供在Unity Inspector中的自定义编辑器界面
    /// </summary>
    [CustomEditor(typeof(AvatarsHolder))]
    public class AvatarsHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            AvatarsHolder tH = (AvatarsHolder)target;
            EditorGUILayout.LabelField("当前头像索引: " + AvatarsHolder.AvatarIndex);

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