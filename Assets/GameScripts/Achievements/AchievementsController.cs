using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	/// <summary>
	/// 成就系统主控制器，负责成就的加载、状态检测与事件分发
	/// </summary>
	public class AchievementsController : MonoBehaviour
	{
        public List<Achievement> achievements; // 成就列表
        public bool HaveTargetAchieved { get; private set; } // 是否有可领奖成就
        public Action<bool> HaveTargetAchievedEvent; // 可领奖成就事件
        #region temp vars
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } } // 配置集
        private LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } } // 当前关卡配置
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } } // 对象集
        #endregion temp vars
        public static AchievementsController Instance; // 单例
		
		#region regular
		private void Start()
		{
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            foreach (var item in achievements)
            {
                item.Load();
                item.ChangeCurrentCountEvent += (c, t) => { CheckState(); };
                item.RewardReceivedEvent += () => { CheckState(); };
            }
            CheckState();
        }
		#endregion regular

        /// <summary>
        /// 检查所有成就状态，触发事件
        /// </summary>
        private void CheckState()
        {
            bool temp = HaveTargetAchieved;
            HaveTargetAchieved = false;
            foreach (var item in achievements)
            {
                if (item.TargetAchieved && !item.RewardReceived) 
                {
                    HaveTargetAchieved = true;
                    break;
                }
            }

           // if (temp != HaveTargetAchieved)
                HaveTargetAchievedEvent?.Invoke(HaveTargetAchieved);
        }
	}
}
