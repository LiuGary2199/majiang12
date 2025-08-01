using UnityEngine;
using UnityEngine.Events;

namespace Mkey
{
	/// <summary>
	/// 处理"重排"道具（Shuffle）相关逻辑的帮助类
	/// 它连接了ShuffleHolder（数据层）和游戏UI/逻辑层
	/// </summary>
	public class ShuffleHelper : MonoBehaviour
	{
		[Tooltip("当玩家没有重排道具时，点击按钮弹出的'免费获取'窗口")]
		public PopUpsController getFreePU;
		private ShuffleHolder MShuffles => ShuffleHolder.Instance;
		private GuiController MGui => GuiController.Instance;

		#region 事件
		[Header("Events")]
		[Tooltip("当成功使用重排道具时触发")]
		public UnityEvent ApplyShuffleEvent;
		[Tooltip("当重排道具数量发生任何变化时触发，参数为当前总数")]
		public UnityEvent<int> ChangeEvent;
		[Tooltip("当获得新的重排道具时触发，参数为获得的数量")]
		public UnityEvent<int> GetShufflesEvent;
		[Tooltip("当消耗重排道具时触发，参数为消耗的数量")]
		public UnityEvent<int> ConsumptionShufflesEvent;
		[Tooltip("当加载重排道具数据时触发，参数为加载后的总数")]
		public UnityEvent<int> LoadEvent;
		[Tooltip("在Start方法开始时触发")]
		public UnityEvent BeginStartEvent;
		[Tooltip("在Start方法结束时触发")]
		public UnityEvent EndStartEvent;
		public ShuffleHolder ShuffleHolder;
		#endregion 事件

		#region 临时变量
		private int shuffles; // 用于缓存当前的重排道具数量，以检测变化
		#endregion 临时变量

		#region Unity生命周期方法
		private void Start()
		{
			BeginStartEvent?.Invoke();
			// 监听 ShuffleHolder 中的事件
			MShuffles.ChangeEvent.AddListener(ChangeEventHandler);
			MShuffles.LoadEvent.AddListener(LoadEventHandler);
			// 手动调用一次Load，以初始化显示
			LoadEventHandler(ShuffleHolder.Count);
			EndStartEvent?.Invoke();
		}

		private void OnDestroy()
		{
			// 移除事件监听，防止内存泄漏
			MShuffles.ChangeEvent.RemoveListener(ChangeEventHandler);
			MShuffles.LoadEvent.RemoveListener(LoadEventHandler);
		}
		#endregion Unity生命周期方法

		/// <summary>
		/// 处理来自 ShuffleHolder 的数量变化事件
		/// </summary>
		/// <param name="count">变化后的新数量</param>
		private void ChangeEventHandler(int count)
		{
			ChangeEvent?.Invoke(count);

			if (shuffles < ShuffleHolder.Count)
			{
				// 数量增加，触发"获得"事件
				GetShufflesEvent?.Invoke(ShuffleHolder.Count - shuffles);
			}
			else if (shuffles > ShuffleHolder.Count)
			{
				// 数量减少，触发"消耗"事件
				ConsumptionShufflesEvent?.Invoke(shuffles - ShuffleHolder.Count);
			}
			shuffles = ShuffleHolder.Count; // 更新缓存的数量
		}

		/// <summary>
		/// 处理来自 ShuffleHolder 的数据加载事件
		/// </summary>
		/// <param name="count">加载后的数量</param>
		private void LoadEventHandler(int count)
		{
			LoadEvent?.Invoke(count);
			shuffles = ShuffleHolder.Count; // 更新缓存的数量
		}

		/// <summary>
		/// 当玩家点击"重排"按钮时调用此方法
		/// </summary>
		public void Shuffle_click()
		{/*
            if (shuffles > 0)
			{
				// 如果还有重排道具
				GameBoard.Instance.ShuffleGrid(null); // 执行棋盘重排逻辑
				ShuffleHolder.Add(-1); // 减少一个重排道具
				ApplyShuffleEvent?.Invoke(); // 触发"成功使用"事件
				GameEvents.ApplyShuffleAction?.Invoke();
			}
            else
            {
				ADAnalyze.Instance.playRewardVideo((success) =>
				{
					if (success)
					 {
						ShuffleHolder.Add(1);
                    }
                }, "101");
				
				// 如果没有重排道具，显示"免费获取"弹窗
				//if (getFreePU) MGui.ShowPopUp(getFreePU);
			}
			*/
			ADAnalyze.Overtone.DikeRadiumUnder((success) =>
				{
					if (success)
					{
						BaskTrialGerman.GetInstance().ArabTrial("1006");
						GameBoard.Instance.ShuffleGrid(null); // 执行棋盘重排逻辑
						ApplyShuffleEvent?.Invoke(); // 触发"成功使用"事件
						GameEvents.ApplyShuffleAction?.Invoke();
					}
				}, "3");
		}
	}
}
