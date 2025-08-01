using UnityEngine;
using UnityEngine.Events;

namespace Mkey
{
	/// <summary>
	/// 作为 GameLevelHolder 的帮助/连接类。
	/// 它监听来自 GameLevelHolder 的C#事件，并将其转发为可在Unity编辑器中配置的 UnityEvents。
	/// 这使得UI元素等其他MonoBehaviour可以方便地响应关卡数据的变化，而无需直接与GameLevelHolder耦合。
	/// </summary>
	public class GameLevelHelper : MonoBehaviour
	{
		private GameLevelHolder MGLevel => GameLevelHolder.Instance;

		#region 事件
		[Header("Events")]
		[Tooltip("当最高通关关卡数发生变化时触发，参数为新的最高关卡数")]
		public UnityEvent<int> ChangePassedEvent;
		[Tooltip("当关卡数据加载时触发，参数为加载的关卡数")]
		public UnityEvent<int> LoadEvent;
		[Tooltip("当成功通关时触发，参数为通过的关卡数")]
		public UnityEvent<int> PassLevelEvent;
		[Tooltip("当开始一个新关卡时触发，参数为开始的关卡数")]
		public UnityEvent<int> StartLevelEvent;

		[Tooltip("在Start方法开始时触发")]
		public UnityEvent BeginStartEvent;
		[Tooltip("在Start方法结束时触发")]
		public UnityEvent EndStartEvent;

		[Tooltip("当需要更新界面显示的关卡编号时触发。参数通常是 number + 1，因为关卡编号在代码中从0开始，在UI中从1开始。")]
		public UnityEvent<int> UpdateGuiLevelNumberEvent;
		#endregion 事件

		#region 临时变量
		private int currentLevel; // 用于缓存当前关卡编号（当前代码中仅赋值，未被读取）
		#endregion 临时变量

		#region Unity生命周期方法
		private void Start()
		{
			BeginStartEvent?.Invoke();

			// 监听来自GameLevelHolder的事件
			MGLevel.ChangePassedEvent.AddListener(ChangePassedEventHandler);
			MGLevel.LoadEvent.AddListener(LoadEventHandler);
			MGLevel.StartLevelEvent.AddListener(StartLevelHandler);
			MGLevel.PassLevelEvent.AddListener(PassLevelHandler);

			// 手动调用一次LoadEventHandler来初始化UI
			LoadEventHandler(GameLevelHolder.CurrentLevel);
			EndStartEvent?.Invoke();
		}
		
		private void OnDestroy()
        {
			// 移除事件监听，防止内存泄漏
			MGLevel.ChangePassedEvent.RemoveListener(ChangePassedEventHandler);
			MGLevel.LoadEvent.RemoveListener(LoadEventHandler);
			MGLevel.StartLevelEvent.RemoveListener(StartLevelHandler);
			MGLevel.PassLevelEvent.RemoveListener(PassLevelHandler);
		}
		#endregion Unity生命周期方法

		/// <summary>
		/// 处理"最高通关关卡变化"事件，并转发
		/// </summary>
		private void ChangePassedEventHandler(int number)
		{
			ChangePassedEvent?.Invoke(number);
		}

		/// <summary>
		/// 处理"数据加载"事件，并转发
		/// </summary>
		private void LoadEventHandler(int number)
		{
			LoadEvent?.Invoke(number);
			// 触发UI更新事件，通常用于显示 "关卡 X"
			UpdateGuiLevelNumberEvent?.Invoke(number + 1);
			currentLevel = GameLevelHolder.CurrentLevel;
		}

		/// <summary>
		/// 处理"开始关卡"事件，并转发
		/// </summary>
		private void StartLevelHandler(int number)
		{
			StartLevelEvent?.Invoke(number);
		}

		/// <summary>
		/// 处理"通关"事件，并转发
		/// </summary>
		private void PassLevelHandler(int number)
		{
			PassLevelEvent?.Invoke(number);
		}
	}
}
