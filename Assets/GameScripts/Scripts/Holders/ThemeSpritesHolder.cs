using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    /// <summary>
    /// 代表一套完整的游戏主题（皮肤）的ScriptableObject。
    /// 它包含了该主题所使用的所有麻将牌图片资源，并将其分为"普通牌"和"特殊组牌"。
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ThemeSpritesHolder")]
    public class ThemeSpritesHolder : BaseScriptable
    {
        [Tooltip("普通麻将牌的精灵列表。这些牌必须与自身完全相同的牌匹配。")]
        public List<Sprite> simpleSprites;
        [Tooltip("特殊麻将牌的组列表。同一组内的牌可以互相匹配。")]
        public List<MahjongSpritesGroup> groups;
        [Tooltip("主题的名称")]
        public string themeName;

        /// <summary>
        /// 获取该主题中所有精灵的总数量。
        /// </summary>
        /// <returns>精灵总数</returns>
        public int GetSpritesCount()
        {
            int count = 0;
            if (groups != null)
            {
                foreach (var item in groups)
                {
                    if (item != null) count += item.Count;
                }
            }
            if (simpleSprites != null) count = simpleSprites.Count + count;
            return count;
        }

        /// <summary>
        ///【调试用】检查是否有未设置（为null）的精灵，并返回它们的索引信息。
        /// </summary>
        /// <returns>包含错误信息的字符串</returns>
        public string FailedSpritesIndexes()
        {
            string res = "";
            if (simpleSprites != null)
            {
                for (int i = 0; i < simpleSprites.Count; i++)
                {
                    if (simpleSprites[i] == null) res += "failed simple collection index: " + i + '\n';
                }
            }

            if (groups != null)
            {
                for (int gri = 0; gri < groups.Count; gri++)
                {
                    MahjongSpritesGroup mSGR = groups[gri];
                    if (mSGR != null)
                    {
                        for (int i = 0; i < mSGR.Count; i++)
                        {
                            if (mSGR.collection[i] == null) res += "group " + gri + "; failed collection index: " + i + '\n';
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 检查两个精灵是否属于同一个特殊组。
        /// </summary>
        /// <param name="sprite_1">精灵1</param>
        /// <param name="sprite_2">精灵2</param>
        /// <returns>如果同组则返回true</returns>
        public bool IsOneGroup(Sprite sprite_1, Sprite sprite_2)
        {
            foreach (var group_i in groups)
            {
                if (group_i.Contains(sprite_1) && group_i.Contains(sprite_2)) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个按固定顺序排列的所有精灵的列表（先是所有普通牌，然后是所有特殊组牌）。
        /// 这个固定顺序对于 GameThemesHolder 中的主题映射至关重要。
        /// </summary>
        /// <returns>包含所有精灵的有序列表</returns>
        public List<Sprite> GetSequencedSprites()
        {
            List<Sprite> res = new List<Sprite>(simpleSprites);
            foreach (var item in groups)
            {
                res.AddRange(item.collection);
            }
            return res;
        }

        /// <summary>
        /// 检查本主题是否包含指定的精灵。
        /// </summary>
        /// <param name="sprite">要检查的精灵</param>
        /// <returns>如果包含则返回true</returns>
        public bool ContainSrite(Sprite sprite)
        {
            if (simpleSprites.Contains(sprite)) return true;
            for (int gri = 0; gri < groups.Count; gri++)
            {
                MahjongSpritesGroup mSGR = groups[gri];
                if (mSGR.Contains(sprite)) return true;
            }
            return false;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为ThemeSpritesHolder提供在Unity Inspector中的自定义编辑器界面。
    /// </summary>
    [CustomEditor(typeof(ThemeSpritesHolder))]
    public class ThemeSpritesHolderEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ThemeSpritesHolder tH = (ThemeSpritesHolder)target;
            EditorGUILayout.LabelField("精灵总数: " + tH.GetSpritesCount());

            #region test
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("打印日志"))
                {
                    Debug.Log("主题名称 - " + tH.themeName );
                    string failedData = tH.FailedSpritesIndexes();
                    if(!string.IsNullOrEmpty(failedData)) Debug.Log(failedData);
                    else
                    {
                        Debug.Log(tH.themeName + " - 未发现配置错误的精灵");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion test
        }
    }
#endif

    /// <summary>
    /// 定义一个可序列化的特殊麻将牌组。
    /// 组内的精灵可以互相匹配。
    /// </summary>
    [Serializable]
    public class MahjongSpritesGroup
    {
        [Tooltip("属于该组的精灵列表")]
        public List<Sprite> collection;
        /// <summary>
        /// 组内的精灵数量
        /// </summary>
        public int Count => (collection == null) ? 0 : collection.Count;

        /// <summary>
        /// 检查指定的精灵是否存在于该组中。
        /// </summary>
        /// <param name="sprite">要检查的精灵</param>
        /// <returns>如果存在则返回true</returns>
        public bool Contains(Sprite sprite)
        {
            return (collection != null && collection.Contains(sprite));
        }
        
        /// <summary>
        /// 获取按顺序配对的精灵对列表。例如 [A,B,C,D] -> [(A,B), (C,D)]
        /// </summary>
        public virtual List<SpritesPair> GetSequencedSpritesPairs()
        {
            int count = (collection.Count % 2 == 0) ? collection.Count : collection.Count - 1;
            List<SpritesPair> res = new List<SpritesPair>();
            for (int i = 0; i < count; i+=2)
            {
                res.Add(new SpritesPair(collection[i], collection[i + 1]));
            }
            return res;
        }
    }

    /// <summary>
    /// 继承自 MahjongSpritesGroup，但提供了不同的配对逻辑。
    /// 在这个类中，每个精灵都与自身配对。
    /// </summary>
    [Serializable]
    public class MahjongSpritesGroupSimple : MahjongSpritesGroup
    {
        /// <summary>
        /// 获取按顺序与自身配对的精灵对列表。例如 [A,B,C] -> [(A,A), (B,B), (C,C)]
        /// </summary>
        public override List<SpritesPair> GetSequencedSpritesPairs()
        {
            int count = (collection.Count % 2 == 0) ? collection.Count : collection.Count - 1;
            List<SpritesPair> res = new List<SpritesPair>();
            for (int i = 0; i < count; i ++)
            {
                res.Add(new SpritesPair(collection[i], collection[i]));
            }
            return res;
        }
    }
 }