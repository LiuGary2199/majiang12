using System.Collections;
using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// 分数控制器，负责分数计算、连击、最大分数等逻辑
    /// </summary>
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        private int baseMatchScore = 240; // 基础匹配分数
        [SerializeField]
        private int increaseComboScore = 40; // 连击加分
        [SerializeField]
        private int maxMatchScore = 40; // 最大匹配分数

        private int combo = 0; // 当前连击数

        public int BaseMatchScore { get { return baseMatchScore; } } // 只读属性：基础分数


        private IEnumerator Start()
        {
            yield return null;
            while (!GameBoard.Instance) yield return null;
            GameBoard.Instance.CollectAction += CollectMatcEventHandler;
            GameBoard.Instance.FailedMatchAction += FailedMatcEventHandler;
        }

        /// <summary>
        /// 获取当前连击下的分数
        /// </summary>
        public int GetMatchScore()
        {
            return GetMatchScore(combo);
        }

        private int GetMatchScore(int _combo)
        {
            int score = baseMatchScore + 40 * _combo;
            if (score > 600) score = 600;
            return score;
        }

        private void CollectMatcEventHandler(Sprite s1, Sprite s2)
        {
            combo++;
        }

        private void FailedMatcEventHandler()
        {
            combo = 0;
        }

        /// <summary>
        /// 获取关卡最大分数
        /// </summary>
        public int GetMaxLevelScore(int matchesCount)
        {
            int score = 0;
            
            for (int _combo = 0; _combo < matchesCount; _combo++)
            {
                score += GetMatchScore(_combo);
            }
            return score;
        }
    }
}