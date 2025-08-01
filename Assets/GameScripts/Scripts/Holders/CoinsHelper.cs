using UnityEngine;
using UnityEngine.Events;

namespace Mkey
{
	/// <summary>
	/// 作为 CoinsHolder 的帮助/连接类，核心功能是为UI提供一个带动画效果的金币数量显示。
	/// 当金币数量变化时，它能驱动UI上的数字平滑地滚动到新数值，而不是瞬间变化。
	/// </summary>
	public class CoinsHelper : MonoBehaviour
	{
		private CoinsHolder MCoins => CoinsHolder.Instance;

		[Tooltip("启动数值滚动动画之前的延迟时间（秒），可用于等待金币飞行等其他动画")]
		public float animDelay;

		#region 事件
		[Tooltip("当金币数量发生任何变化时触发，参数为当前总数")]
		public UnityEvent<int> ChangeEvent;
		[Tooltip("当获得金币时触发，参数为获得的数量")]
		public UnityEvent<int> GetMoneyEvent;
		[Tooltip("当花费金币时触发，参数为花费的数量")]
		public UnityEvent<int> PayOutEvent;
		[Tooltip("当加载金币数据时触发，参数为加载后的总数")]
		public UnityEvent<int> LoadEvent;
		[Tooltip("在数值滚动动画的每一帧触发，参数为当前帧的数值。UI文本应监听此事件来更新显示。")]
		public UnityEvent<int> AnimatedUpdateEvent;
		[Tooltip("在数值滚动动画（增加时）开始的瞬间触发，可用于播放音效等。")]
		public UnityEvent StartAnimatedUpdateEvent;
		[Tooltip("在Start方法开始时触发")]
		public UnityEvent BeginStartEvent;
		[Tooltip("在Start方法结束时触发")]
		public UnityEvent EndStartEvent;
		#endregion 事件

		#region 临时变量
		private int coins; // 用于缓存当前的金币数量，以检测变化
		private TweenIntValue amountTween; // 数值缓动动画的实例
		#endregion 临时变量

		#region Unity生命周期方法
		private void Start()
		{
			BeginStartEvent?.Invoke();
			// 监听 CoinsHolder 中的事件
			MCoins.ChangeEvent.AddListener(ChangeEventHandler);
			MCoins.LoadEvent.AddListener(LoadEventHandler);
			LoadEventHandler(CoinsHolder.Count);
			// 初始化数值缓动动画，设置回调，在动画每一帧更新时触发 AnimatedUpdateEvent
			amountTween = new TweenIntValue(gameObject, CoinsHolder.Count, 1, 3, true, (b) => { if (this) AnimatedUpdateEvent?.Invoke(b); });
			EndStartEvent?.Invoke();
		}
		
		private void OnDestroy()
        {
			// 移除事件监听，防止内存泄漏
			MCoins.ChangeEvent.RemoveListener(ChangeEventHandler);
			MCoins.LoadEvent.RemoveListener(LoadEventHandler);
		}
		#endregion Unity生命周期方法

		/// <summary>
		/// 处理来自 CoinsHolder 的数量变化事件
		/// </summary>
		/// <param name="count">变化后的新数量</param>
		private void ChangeEventHandler(int count)
		{
			ChangeEvent?.Invoke(count);
			StartAnimatedTween(count); // 启动带动画的更新

			if (coins < CoinsHolder.Count)
            {
				// 数量增加，触发“获得金币”事件
				GetMoneyEvent?.Invoke(CoinsHolder.Count - coins);
			}
			else if (coins > CoinsHolder.Count)
            {
				// 数量减少，触发“花费金币”事件
				PayOutEvent?.Invoke(coins - CoinsHolder.Count);
			}
			coins = CoinsHolder.Count; // 更新缓存的数量
		}

		/// <summary>
		/// 启动数值缓动动画
		/// </summary>
		/// <param name="count">目标数值</param>
		private void StartAnimatedTween(int count)
        {
			if (coins < CoinsHolder.Count)
            {
				// 如果是增加金币，立即触发一个开始事件
				StartAnimatedUpdateEvent?.Invoke();
			}
			// 延迟 animDelay 秒后，开始真正的数值滚动动画
			TweenExt.DelayAction(gameObject, animDelay, () => { if (amountTween != null) amountTween.Tween(count, 100); });
		}

		/// <summary>
		/// 处理来自 CoinsHolder 的数据加载事件
		/// </summary>
		/// <param name="count">加载后的数量</param>
		private void LoadEventHandler(int count)
		{
			LoadEvent?.Invoke(count);
			coins = CoinsHolder.Count; // 更新缓存的数量
		}
	}
}
