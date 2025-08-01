using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	/// <summary>
	/// 游戏全局事件管理类，提供关卡、提示、撤销等事件的注册与触发
	/// </summary>
	public static class GameEvents 
	{
        #region comon events
        public static Action ApplyShuffleAction { get; set; } // 洗牌事件
        public static Action ApplyHintAction { get; set; } // 提示事件
        public static Action UndoAction { get; set; } // 撤销事件
        public static Action RestartAction { get; set; } // 重新开始事件
        public static Action WinLevelAction { get; set; } // 通关事件
        public static Action BreakLevelAction { get; set; } // 关卡中断事件
        public static Action LevelLoadCompleteAction { get; set; } // 关卡加载结束事件
        public static Action <int,bool,List<Transform>,Action> GoldProgress; // 关卡加载结束事件

        public static Action <Sprite, Sprite> MatchSpritesEvent { get; set; } // 匹配精灵事件
        
        // 提示相关事件
        public static Action<string, float> ShowTipAction { get; set; } // 显示提示事件 (消息, 持续时间) - 0表示手动关闭
        public static Action<string> ShowTipManualAction { get; set; } // 显示提示事件（手动关闭）(消息)
        public static Action HideTipAction { get; set; } // 隐藏提示事件
        public static Action CashOutPanelClosedAction { get; set; } // CashOutPanel关闭事件

        // 新增：新手引导开始/结束事件
        public static Action TutorialGuideStartedAction { get; set; }
        public static Action TutorialGuideEndedAction { get; set; }

        private static Dictionary<string, List <Action<string>>> CommonEventHandlersDict; // 通用事件字典
        #endregion comon events
		
		#region common
		/// <summary>
		/// 添加通用事件处理器
		/// </summary>
		public static void AddCommonEventHandler(string id , Action<string> CommonEventHandler)
        {
            if (CommonEventHandler == null) return;

            if (CommonEventHandlersDict == null) CommonEventHandlersDict = new Dictionary<string, List< Action<string>>>();

            if (CommonEventHandlersDict.ContainsKey(id))
            {
                if (CommonEventHandlersDict[id] == null) CommonEventHandlersDict[id] = new List<Action<string>>();
                CommonEventHandlersDict[id].Add(CommonEventHandler);
            }
            else
            {
                CommonEventHandlersDict.Add(id, new List<Action<string>>());
                CommonEventHandlersDict[id].Add(CommonEventHandler);
            }
        }

        /// <summary>
        /// 移除通用事件处理器
        /// </summary>
        public static void RemoveCommonEventHandler(string id, Action<string> CommonEventHandler)
        {
            if (CommonEventHandler == null) return;
            if (CommonEventHandlersDict == null) CommonEventHandlersDict = new Dictionary<string, List<Action<string>>>();
            if (CommonEventHandlersDict.ContainsKey(id))
            {
                if (CommonEventHandlersDict[id] != null && CommonEventHandlersDict[id].Contains(CommonEventHandler))
                {
                    CommonEventHandlersDict[id].Remove(CommonEventHandler);
                }
            }
        }

        /// <summary>
        /// 触发通用事件
        /// </summary>
        public static void OnCommonEvent(string id, string jsonParam)
        {
            if (CommonEventHandlersDict == null) CommonEventHandlersDict = new Dictionary<string,List<Action<string>>>();
            if (CommonEventHandlersDict.ContainsKey(id))
            {
                if (CommonEventHandlersDict[id] != null)
                {
                    foreach (var item in CommonEventHandlersDict[id])
                    {
                        item?.Invoke(jsonParam);
                    }
                }
            }
        }
        #endregion common
    }
}
