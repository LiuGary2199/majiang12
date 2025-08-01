using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
	/// <summary>
	/// 使用洗牌成就，监听洗牌事件并计数
	/// </summary>
	public class UseShuffleAchievement : Achievement
	{
        #region events
        #endregion events

        #region regular
        /// <summary>
        /// 加载成就，绑定事件
        /// </summary>
        public override void Load()
        {
            LoadRewardReceived();
            LoadCurrentCount();
            LoadCurrentStage();

            GameEvents.ApplyShuffleAction += UseShuffleEventHandler;
            ChangeCurrentCountEvent += (cc, tc)=>{  };
        }

        private void OnDestroy()
        {
            GameEvents.ApplyShuffleAction -= UseShuffleEventHandler;
        }
        #endregion regular

        /// <summary>
        /// 获取唯一成就名
        /// </summary>
        public override string GetUniqueName()
        {
            return "useshuffle";
        }

        private void UseShuffleEventHandler()
        {
            IncCurrentCount();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UseShuffleAchievement))]
    public class UseShuffleAchievementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            UseShuffleAchievement t = (UseShuffleAchievement)target;
            t.DrawInspector();
        }
    }
#endif
}
