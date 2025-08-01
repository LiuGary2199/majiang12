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
    /// 定义麻将牌的填充方式（该枚举在此脚本中未被使用，可能由其他脚本使用）
    /// </summary>
    public enum FillType {OnlySimple, GroupsAndSimple, SimpleAndGroups, RandomGroupAndSimple}

    /// <summary>
    /// 管理游戏中所有视觉主题（皮肤）的ScriptableObject。
    /// 它持有所有主题的引用，并负责处理玩家的主题选择、数据持久化以及不同主题间精灵的映射。
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/GameThemesHolder")]
    public class GameThemesHolder : SingletonScriptableObject<GameThemesHolder>
    {
        [Tooltip("包含游戏中所有主题的数组")]
        [SerializeField]
        public ThemeSpritesHolder [] themes;

        #region 存储键
        [SerializeField]
        private string saveKey = "mk_theme"; // 用于PlayerPrefs存储当前主题索引的键
        #endregion 存储键

        #region 临时变量
        private static bool loaded = false; // 标记是否已从PlayerPrefs加载过数据
        #endregion 临时变量

        private static int _themeIndex; // 静态变量，存储当前选择的主题索引

        /// <summary>
        /// 当前选择的主题索引。首次访问时会自动加载。
        /// </summary>
        public static int ThemeIndex
        {
            get { if (!loaded) Instance.Load(); return _themeIndex; }
            private set { _themeIndex = value; }
        }

        [Tooltip("主题变更时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<int> ChangeUnityEvent;
        [Tooltip("主题数据加载时触发的UnityEvent事件，可在Inspector中配置")]
        public UnityEvent<int> LoadUnityEvent;
        
        /// <summary>
        /// 主题变更时触发的C#事件（Action），参数为 (旧索引, 新索引)
        /// </summary>
        public Action<int, int> ChangeEvent;
        /// <summary>
        /// 主题数据加载时触发的C#事件（Action）
        /// </summary>
        public Action<int> LoadEvent;

        private void Awake()
        {
            Load();
            Debug.Log("Awake: " + this + " ; theme index: " + ThemeIndex);
        }

        /// <summary>
        /// 设置新的主题索引
        /// </summary>
        /// <param name="index">新的主题索引</param>
        public void SetIndex(int index)
        {
            int oldIndex = ThemeIndex;
            if (index < 0) index = 0;
            bool changed = (ThemeIndex != index);
            ThemeIndex = index;
            if (changed)
            {
                PlayerPrefs.SetInt(saveKey, ThemeIndex);
                ChangeEvent?.Invoke(oldIndex, ThemeIndex); // 触发C#事件
                ChangeUnityEvent?.Invoke(ThemeIndex); // 触发UnityEvent事件
            }
        }

        /// <summary>
        /// 从PlayerPrefs加载序列化的主题数据，如果不存在则设为默认值
        /// </summary>
        public void Load()
        {
            loaded = true;
            _themeIndex = PlayerPrefs.GetInt(saveKey, 0);
            LoadEvent?.Invoke(ThemeIndex);
            LoadUnityEvent?.Invoke(ThemeIndex);
        }

        /// <summary>
        /// 重置为默认主题数据
        /// </summary>
        public void SetDefaultData()
        {
            PlayerPrefs.DeleteKey(saveKey);
            SetIndex(0); // 设置为第一个主题
        }

        /// <summary>
        /// 获取当前选中的主题对象 (ThemeSpritesHolder)
        /// </summary>
        /// <returns>当前的主题对象</returns>
        public ThemeSpritesHolder GetTheme()
        {
            if (themes.Length == 0) return null;
            if (themes.Length > 0 && ThemeIndex >= 0 && ThemeIndex < themes.Length) return themes[ThemeIndex];
            return themes[themes.Length - 1]; // 作为备用，返回最后一个
        }
        
        /// <summary>
        /// 创建两个主题之间精灵的映射字典。
        /// 对于主题切换等功能非常有用。
        /// </summary>
        /// <param name="theme_1">主题1</param>
        /// <param name="theme_2">主题2</param>
        /// <returns>一个从主题1的精灵映射到主题2精灵的字典</returns>
        public Dictionary <Sprite, Sprite> GetSpritesDictionary(ThemeSpritesHolder theme_1, ThemeSpritesHolder theme_2)
        {
            Dictionary<Sprite, Sprite> res = new Dictionary<Sprite, Sprite>();
            List<Sprite> sprites_1 = theme_1.GetSequencedSprites();
            List<Sprite> sprites_2 = theme_2.GetSequencedSprites();
            if (sprites_1.Count != sprites_2.Count) return null; // 如果两个主题的精灵数量不一致，则无法映射

            for (int i = 0; i < sprites_1.Count; i++)
            {
                res.Add(sprites_1[i], sprites_2 [i]);
            }
            return res;
        }

        /// <summary>
        /// 根据给定的精灵，查找它属于哪个主题
        /// </summary>
        /// <param name="sprite">要查找的精灵</param>
        /// <returns>包含该精灵的主题对象</returns>
        public ThemeSpritesHolder GetSpriteTheme(Sprite sprite)
        {
            foreach (var item in themes)
            {
                if (item.ContainSrite(sprite)) return item;
            }
            return null;
        }

        /// <summary>
        /// 获取一个精灵在所有其他主题中的"等效"精灵列表。
        /// 例如，获取A主题的"一万"，此方法会返回B、C、D等主题中的"一万"精灵。
        /// </summary>
        /// <param name="sourceSprite">源精灵</param>
        /// <param name="includeSourceSprite">结果中是否包含源精灵自己</param>
        /// <returns>等效精灵的列表</returns>
        public List<Sprite> GetSpriteAliases(Sprite sourceSprite, bool includeSourceSprite)
        {
            if (sourceSprite == null) return null;
            List<Sprite> result = new List<Sprite>();
            ThemeSpritesHolder tH_0 = GetSpriteTheme(sourceSprite); // 找到源精灵所在的主题
            int index = tH_0.GetSequencedSprites().IndexOf(sourceSprite); // 找到源精灵在其主题序列中的索引

            // 遍历所有主题，根据索引找到对应的精灵
            foreach (var item in themes)
            {
                if (item == tH_0 && !includeSourceSprite) continue; // 根据参数决定是否跳过源主题
                result.Add(item.GetSequencedSprites()[index]);
            }
            return result;
        }

        /// <summary>
        /// 获取一个精灵在当前选定主题中的"等效"精灵。
        /// </summary>
        /// <param name="sourceSprite">源精灵</param>
        /// <returns>在当前主题中对应的精灵</returns>
        public Sprite GetSpriteAlias(Sprite sourceSprite)
        {
            if (sourceSprite == null) return null;
            ThemeSpritesHolder th_current = GetTheme(); // 获取当前主题
            ThemeSpritesHolder th_s = GetSpriteTheme(sourceSprite); // 获取源精灵所在的主题
            if (th_s == th_current) return sourceSprite; // 如果源精灵就在当前主题中，直接返回
            int index = th_s.GetSequencedSprites().IndexOf(sourceSprite); // 找到源精灵在其主题中的索引
            return th_current.GetSequencedSprites()[index]; // 返回当前主题中相同索引位置的精灵
        }
    }


/*
    // 一个被注释掉的类，可能用于更复杂的精灵ID系统
    [Serializable]
    public class MahjongSpriteID
    {
        public int indexInCollection;
        public int groupIndex;
        public int themeIndex;
    }
*/

#if UNITY_EDITOR
    /// <summary>
    /// 为GameThemesHolder提供在Unity Inspector中的自定义编辑器界面
    /// </summary>
    [CustomEditor(typeof(GameThemesHolder))]
    public class GameThemesHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GameThemesHolder tH = (GameThemesHolder)target;
            EditorGUILayout.LabelField("当前主题索引: " + GameThemesHolder.ThemeIndex);

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