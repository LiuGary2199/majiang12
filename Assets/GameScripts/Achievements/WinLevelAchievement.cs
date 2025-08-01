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
	/// 通关成就，监听通关事件并计数
	/// </summary>
	public class WinLevelAchievement : Achievement
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

            GameEvents.WinLevelAction += WinLevelEventHandler;
            //RewardReceivedEvent +=(r)=> 
            //{
            //    CoinsHolder.Add(r);
            //};

            ChangeCurrentCountEvent += (cc, tc)=>{  };
        }

        private void OnDestroy()
        {
            GameEvents.WinLevelAction -= WinLevelEventHandler;
        }
        #endregion regular

        public override string GetUniqueName()
        {
            return "winlevels";
        }

        private void WinLevelEventHandler()
        {
            IncCurrentCount();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WinLevelAchievement))]
    public class WinLevelAchievementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WinLevelAchievement t = (WinLevelAchievement)target;
            t.DrawInspector();
        }
    }
#endif
}
