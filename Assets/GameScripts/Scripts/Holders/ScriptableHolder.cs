using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	/// <summary>
	/// 一个用于在游戏启动时预加载 ScriptableObject 资源的 MonoBehaviour。
	/// 通过将此脚本附加到场景中的一个GameObject上，并将其引用的ScriptableObject资源拖入下面的数组，
	/// 可以确保这些资源在其他脚本访问它们之前被加载到内存中。
	/// </summary>
	[DefaultExecutionOrder (-100)] // 设置脚本的执行顺序为-100，使其非常靠前执行，确保资源被优先加载
	public class ScriptableHolder : MonoBehaviour
	{
		[Tooltip("需要预加载的 ScriptableObject 资源列表")]
		public ScriptableObject[] scriptable;
		
		
		#region Unity生命周期方法
		private void Awake()
        {
			// 此处可添加在所有其他对象唤醒之前的初始化代码
        }
		
		private void Start()
		{	
			// 此处可添加在所有其他对象首次更新之前的初始化代码
		}
		
		private void OnDestroy()
        {
			// 此处可添加对象销毁时的清理代码
        }
		#endregion Unity生命周期方法
	}
}
