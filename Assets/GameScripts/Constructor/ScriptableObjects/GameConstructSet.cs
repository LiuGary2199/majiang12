using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Mkey
{
    public class GameConstructSet : BaseScriptable
    {
        [SerializeField]
        private GameObjectsSet gOSet;
        [Space(8, order = 0)]
        [Header("Constructed Levels", order = 1)]
        public List<LevelConstructSet> levelSets;

        [Space(8, order = 2)]
        [Header("Skip Levels Configuration", order = 3)]
        [SerializeField]
        public int[] skipLevels = new int[0]; // 需要跳过的关卡列表

        public bool testMode = false;
        public int testLevel = 0;

        public static int MaxLayersCount = 5;

        #region properties
        public GameObjectsSet GOSet { get { return gOSet; } }

        public int LevelCount
        {
            get { if (levelSets != null) return levelSets.Count; else return 0; }
        }
        #endregion properties

        static GameConstructSet _instance = null;
        public static GameConstructSet Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.FindObjectsOfTypeAll<GameConstructSet>().FirstOrDefault();
                }

#if UNITY_EDITOR
                if (!_instance)
                {
                    string[] guids2 = UnityEditor.AssetDatabase.FindAssets("GameConstructSet", null);
                    foreach (var item in guids2)
                    {
                        Debug.Log(item);
                    }
                    if (guids2 != null && guids2.Length > 0)
                    {
                        _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<GameConstructSet>(guids2[0]); // UnityEditor.AssetDatabase.LoadAssetAtPath<GameConstructSet>("Assets/Resources/GameConstaructSet/GameConstructSet_1.asset");
                    }
                }
#endif
                return _instance;
            }
        }

        /// <summary>
        /// Return LevelConstructSet for levelNumber. If levelNumber out of range - return LevelConstruct for 1 levelNumber.
        /// </summary>
        /// <param name="displayLevel">显示的关卡号（用户看到的关卡号）</param>
        /// <returns></returns>
        public LevelConstructSet GetLevelConstructSet(int displayLevel)
        {
            skipLevels = MobTownEre.instance.FellowLoom.skipLevel;
            if (levelSets == null || levelSets.Count == 0)
                return null;

            // 将显示关卡号转换为实际关卡号
            int actualLevel = GetActualLevel(displayLevel);

            // 处理新手关卡（0和1）
            if (actualLevel >= 0 && actualLevel <= 1 && actualLevel < levelSets.Count)
                return levelSets[actualLevel];

            // 处理正常关卡范围
            if (InRange(actualLevel))
                return levelSets[actualLevel];

            // 处理超过最大关卡的情况
            int maxValidLevel = levelSets.Count - 1;
            int normalLevelCount = maxValidLevel - 2 + 1; // 非新手关卡数量（2到maxValidLevel）

            if (normalLevelCount <= 0) // 确保有非新手关卡
                return levelSets[maxValidLevel];

            // 修正：将哈希计算的常量改为int兼容，并显式转换结果
            int primeOffset = 509;
            // 使用 unchecked 避免溢出检查，并用 (int) 显式转换
            unchecked
            {
                int hash = (int)((actualLevel + primeOffset) * 2654435761u); // 添加u后缀表示uint常量
                int mappedIndex = (hash ^ (hash >> 16)) % normalLevelCount;

                if (mappedIndex < 0)
                    mappedIndex += normalLevelCount;

                int mappedLevel = 2 + mappedIndex;
                mappedLevel = Mathf.Clamp(mappedLevel, 2, maxValidLevel);

                return levelSets[mappedLevel];
            }
        }

        /// <summary>
        /// 将显示关卡号转换为实际关卡号
        /// </summary>
        /// <param name="displayLevel">显示的关卡号</param>
        /// <returns>实际关卡号</returns>
        private int GetActualLevel(int displayLevel)
        {
            if (skipLevels == null || skipLevels.Length == 0)
                return displayLevel;

            int actualLevel = displayLevel;
            
            // 遍历跳过关卡列表，调整实际关卡号
            foreach (int skipLevel in skipLevels)
            {
                // 将后台配置的关卡号转换为数组索引（减1）
                int skipLevelIndex = skipLevel - 1;
                if (displayLevel >= skipLevelIndex)
                {
                    actualLevel++;
                }
            }
            
            return actualLevel;
        }

        /// <summary>
        /// 将实际关卡号转换为显示关卡号
        /// </summary>
        /// <param name="actualLevel">实际关卡号</param>
        /// <returns>显示关卡号</returns>
        public int GetDisplayLevel(int actualLevel)
        {
            if (skipLevels == null || skipLevels.Length == 0)
                return actualLevel;

            int displayLevel = actualLevel;
            
            // 遍历跳过关卡列表，调整显示关卡号
            foreach (int skipLevel in skipLevels)
            {
                // 将后台配置的关卡号转换为数组索引（减1）
                int skipLevelIndex = skipLevel - 1;
                if (actualLevel > skipLevelIndex)
                {
                    displayLevel--;
                }
            }
            
            return displayLevel;
        }

        #region regular
        private void OnEnable()
        {

        }

        #endregion regular

        public void Clean()
        {
            bool needClean = false;
            if (levelSets == null) { levelSets = new List<LevelConstructSet>(); needClean = true; }
            ;

            if (!needClean)
                foreach (var item in levelSets)
                {
                    if (item == null)
                    {
                        needClean = true;
                        break;
                    }
                }

            if (needClean)
            {
                levelSets = levelSets.Where(item => item != null).ToList();
                SetAsDirty();
            }
            Debug.Log("levels count " + levelSets.Count);
        }

        public void AddLevel(LevelConstructSet lc)
        {
            if (levelSets == null) { levelSets = new List<LevelConstructSet>(); }
            levelSets.Add(lc);
            SetAsDirty();
        }

        public void InsertBeforeLevel(int levelIndex, LevelConstructSet lcs)
        {
            if (!InRange(levelIndex)) return;
            levelSets.Insert(levelIndex, lcs);
            SetAsDirty();
        }

        public void InsertAfterLevel(int levelIndex, LevelConstructSet lcs)
        {
            Debug.Log("insert level after: " + levelIndex);
            if (!InRange(levelIndex)) return;
            if (levelIndex + 1 == levelSets.Count)
            {
                levelSets.Add(lcs);
                Debug.Log("add after : " + levelIndex);
            }
            else
            {
                levelSets.Insert(levelIndex + 1, lcs);
                Debug.Log("insert after : " + levelIndex);
            }
            SetAsDirty();
        }

        public void RemoveLevel(int levelIndex)
        {
            if (!InRange(levelIndex)) return;
#if UNITY_EDITOR
            ScriptableObjectUtility.DeleteResourceAsset(levelSets[levelIndex]);
#endif
            levelSets.RemoveAt(levelIndex);
            SetAsDirty();
        }

        private bool InRange(int level)
        {
            return (levelSets != null && levelSets.Count > level && level >= 0);
        }

        #region Skip Levels Management
        /// <summary>
        /// 添加跳过关卡
        /// </summary>
        /// <param name="level">要跳过的关卡号</param>
        public void AddSkipLevel(int level)
        {
            if (skipLevels == null)
                skipLevels = new int[0];
            
            if (!skipLevels.Contains(level))
            {
                skipLevels = skipLevels.Concat(new[] { level }).ToArray();
                skipLevels = skipLevels.OrderBy(l => l).ToArray(); // 保持有序
                SetAsDirty();
            }
        }

        /// <summary>
        /// 移除跳过关卡
        /// </summary>
        /// <param name="level">要移除的跳过关卡号</param>
        public void RemoveSkipLevel(int level)
        {
            if (skipLevels != null && skipLevels.Contains(level))
            {
                skipLevels = skipLevels.Where(l => l != level).ToArray();
                SetAsDirty();
            }
        }

        /// <summary>
        /// 清空所有跳过关卡
        /// </summary>
        public void ClearSkipLevels()
        {
            if (skipLevels != null)
            {
                skipLevels = new int[0];
                SetAsDirty();
            }
        }

        /// <summary>
        /// 检查关卡是否被跳过
        /// </summary>
        /// <param name="level">关卡号</param>
        /// <returns>是否被跳过</returns>
        public bool IsLevelSkipped(int level)
        {
            if (skipLevels == null || skipLevels.Length == 0)
                return false;
            
            // 将关卡号转换为数组索引（减1）
            int levelIndex = level - 1;
            return skipLevels.Contains(levelIndex);
        }

        /// <summary>
        /// 获取跳过关卡列表
        /// </summary>
        /// <returns>跳过关卡列表</returns>
        public int[] GetSkipLevels()
        {
            return skipLevels != null ? (int[])skipLevels.Clone() : new int[0];
        }
        #endregion
    }
}

