using UnityEngine;
using UnityEngine.Events;

namespace Mkey
{
	/// <summary>
	/// 处理"提示"道具（Hint）相关逻辑的帮助类。
	/// 它连接了HintHolder（数据层）和游戏UI/逻辑层。
	/// </summary>
	public class HintHelper : MonoBehaviour
	{
		[Tooltip("当玩家没有提示道具时，点击按钮弹出的'免费获取'窗口")]
		public PopUpsController getFreePU;

		private HintHolder MHints => HintHolder.Instance;
		private GuiController MGui => GuiController.Instance;

		#region 事件
		[Header("Events")]
		[Tooltip("当成功使用提示道具时触发")]
		public UnityEvent ApplyHintEvent;
		[Tooltip("当提示道具数量发生任何变化时触发，参数为当前总数")]
		public UnityEvent<int> ChangeEvent;
		[Tooltip("当获得新的提示道具时触发，参数为获得的数量")]
		public UnityEvent<int> GetHintsEvent;
		[Tooltip("当消耗提示道具时触发，参数为消耗的数量")]
		public UnityEvent<int> ConsumptionHintsEvent;
		[Tooltip("当加载提示道具数据时触发，参数为加载后的总数")]
		public UnityEvent<int> LoadEvent;
		[Tooltip("在Start方法开始时触发")]
		public UnityEvent BeginStartEvent;
		[Tooltip("在Start方法结束时触发")]
		public UnityEvent EndStartEvent;


		public HintHolder HintHolder;
		#endregion 事件

		#region 临时变量
		private int hints; // 用于缓存当前的提示道具数量，以检测变化
		#endregion 临时变量

		#region Unity生命周期方法
		private void Start()
		{
			BeginStartEvent?.Invoke();
			// 监听 HintHolder 中的事件
			MHints.ChangeEvent.AddListener(ChangeEventHandler);
			MHints.LoadEvent.AddListener(LoadEventHandler);
			// 手动调用一次Load，以初始化显示
			LoadEventHandler(HintHolder.Count);
			EndStartEvent?.Invoke();
		}

		private void OnDestroy()
		{
			// 移除事件监听，防止内存泄漏
			MHints.ChangeEvent.RemoveListener(ChangeEventHandler);
			MHints.LoadEvent.RemoveListener(LoadEventHandler);
		}
		#endregion Unity生命周期方法

		/// <summary>
		/// 处理来自 HintHolder 的数量变化事件
		/// </summary>
		/// <param name="count">变化后的新数量</param>
		private void ChangeEventHandler(int count)
		{
			ChangeEvent?.Invoke(count);

			if (hints < HintHolder.Count)
			{
				// 数量增加，触发"获得"事件
				GetHintsEvent?.Invoke(HintHolder.Count - hints);
			}
			else if (hints > HintHolder.Count)
			{
				// 数量减少，触发"消耗"事件
				ConsumptionHintsEvent?.Invoke(hints - HintHolder.Count);
			}
			hints = HintHolder.Count; // 更新缓存的数量
		}

		/// <summary>
		/// 处理来自 HintHolder 的数据加载事件
		/// </summary>
		/// <param name="count">加载后的数量</param>
		private void LoadEventHandler(int count)
		{
			LoadEvent?.Invoke(count);
			hints = HintHolder.Count; // 更新缓存的数量
		}

		/// <summary>
		/// 当玩家点击"提示"按钮时调用此方法
		/// </summary>
		public void Select_Hint()
		{
			// 检查棋盘上是否已经显示了提示
			if (GameBoard.Instance.IsAlreadyHint())
			{
				UIAnalyze.GetInstance().BeadUIFlank(nameof(Ought), "already been selected");
				//GuiController.Instance.ShowMessage("", "已经有匹配的牌被选中了", 2, null);
				return;
			}
			ADAnalyze.Overtone.DikeRadiumUnder((success) =>
			{
				if (success)
				{
					GameBoard.Instance.TrySelectHintMatch((good) =>
					{
						//if (good) HintHolder.Add(-1);
					});
					BaskTrialGerman.GetInstance().ArabTrial("1005");
					ApplyHintEvent?.Invoke(); // 触发"成功使用"事件
					GameEvents.ApplyHintAction?.Invoke();
				}
			}, "4");

			/*
			if (hints > 0)
			{
				// 尝试在棋盘上选择并高亮一对可消除的牌
				// 只有在成功找到并显示提示后（回调函数中的good为true），才消耗一个提示道具
				GameBoard.Instance.TrySelectHintMatch((good) => { if (good) HintHolder.Add(-1); });
				ApplyHintEvent?.Invoke(); // 触发"成功使用"事件
				GameEvents.ApplyHintAction?.Invoke();
			}
			else
			{
				ADAnalyze.Instance.playRewardVideo((success) =>
				{
					if (success)
					{
						HintHolder.Add(1);
					}
				}, "101");
				// 如果没有提示道具，显示"免费获取"弹窗 新逻辑  直接加

				//if (getFreePU) MGui.ShowPopUp(getFreePU);
			}*/
		}
	}
}
