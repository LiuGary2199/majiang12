using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace Mkey
{
    /// <summary>
    /// 触摸管理器，负责处理拖拽、点击、跟随、触摸事件等逻辑，支持移动端和PC端
    /// </summary>
    public class TouchManager : TouchPadMessageTarget, IPointerExitHandler
    {
        public bool dlog = false; // 是否输出调试日志
        public static TouchManager Instance; // 单例
        public Transform PointerUpObject; // 抬起时的对象
        public Transform FirstObject; // 当前可拖拽对象
        //{
        //    get; private set;
        //}

        public bool CanDrag = false; // 是否允许拖拽
        public bool MinDragReached = false; // 是否达到最小拖拽距离

        #region temp vars
        private Vector3 dragPos; // 当前拖拽位置
        private Vector3 pointerDownPos; // 按下时位置
        private Vector3 draggableStartPos; // 拖拽对象起始位置
        private TouchPadEventArgs tPEA; // 当前触摸事件参数
        private bool followStarted = false; // 是否正在跟随
        private Vector3 dragDirection; // 拖拽方向
        private float dragMagnitude; // 拖拽距离
        private float dragPathLength; // 拖拽路径长度
        private Action<Action> ResetDragEvent; // 拖拽重置回调
        #endregion temp vars

        #region regular
        /// <summary>
        /// 初始化单例，注册TouchPad事件
        /// </summary>
        private IEnumerator Start()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
            }

            while (!TouchPad.Instance) yield return new WaitForEndOfFrame();
            if (GameBoard.GMode == GameMode.Play)
            {
                TouchPad.Instance.ScreenDragEvent += LastScreenDragHandler;
                TouchPad.Instance.ScreenPointerDownEvent += LastScreenPointerDownEventHandler;
                TouchPad.Instance.ScreenPointerUpEvent += LastScreePointerUpEventHandler;
            }
            dragPathLength = 0;
        }
        #endregion regular

        /// <summary>
        /// 判断当前是否为移动设备
        /// </summary>
        public static bool IsMobileDevice()
        {
            //check if our current system info equals a desktop
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                //we are on a desktop device, so don't use touch
                return false;
            }
            //if it isn't a desktop, lets see if our device is a handheld device aka a mobile device
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                //we are on a mobile device, so lets use touch input
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启用或禁用触摸回调处理
        /// </summary>
        internal static void SetTouchActivity(bool activity)
        {
            TouchPad.Instance.SetTouchActivity(activity);
        }

        #region touchpad handlers
        /// <summary>
        /// 拖拽事件处理
        /// </summary>
        private void LastScreenDragHandler(TouchPadEventArgs tpea)
        {
            if (!CanDrag) return;
            tPEA = tpea;
            dragPathLength += (dragPos - pointerDownPos).magnitude;
            dragPos = tpea.WorldPos;
            dragDirection = dragPos - pointerDownPos;
            dragMagnitude = dragDirection.magnitude;
            MinDragReached = (dragPathLength > 0.1f);
#if UNITY_EDITOR
            if (dlog) Debug.Log("drag: " + gameObject.name + " ; Draggable: " + FirstObject + " ; distance:" + dragMagnitude);
#endif
            if (FirstObject )
            {
                // Debug.Log("follow _ 0");
                if (!followStarted) StartCoroutine(SlowFollowC()); // &&  !criticalDrag
            }
        }
        /// <summary>
        /// 拖拽跟随协程
        /// </summary>
        private IEnumerator SlowFollowC() // slow motion
        {
           // Debug.Log("follow_1");
            followStarted = true;
            if(FirstObject && CanDrag) FirstObject.position = draggableStartPos + dragDirection;  // show drag
            yield return new WaitForEndOfFrame();
            followStarted = false;
            if (dlog) Debug.Log("end follow cor");
        }

        /// <summary>
        /// 按下事件处理
        /// </summary>
        private void LastScreenPointerDownEventHandler(TouchPadEventArgs tpea)
        {
            pointerDownPos = tpea.WorldPos;
            dragPos = pointerDownPos;
            dragMagnitude = 0;
            dragPathLength = 0;
            MinDragReached = false;
        }

        /// <summary>
        /// 抬起事件处理
        /// </summary>
        private void LastScreePointerUpEventHandler(TouchPadEventArgs tpea)
        {
            // Debug.Log("LastScreenPointerUpEventHandler");
            CanDrag = false;
            if (FirstObject && PointerUpObject) return;
            if (FirstObject)
            {
                ResetDragEventRaise(null);
            }
        }
        #endregion touchpad handlers

        #region interface implement
        /// <summary>
        /// 指针离开事件处理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                CanDrag = false;
                if (FirstObject)
                {
                    // FirstObject.GetComponentInParent<TileTouchBehavior>().SetInitialposition();
                    ResetDragEventRaise(null);
                }
            }
        }
        #endregion interface implement

        /// <summary>
        /// 设置第一个选中的对象和拖拽重置回调
        /// </summary>
        public void SetFirstObject(Transform firstObject, Action<Action> resetDrag)
        {
            if (firstObject)
            {
                FirstObject = firstObject;
                PointerUpObject = null;
                draggableStartPos = firstObject.transform.position;
            }
            else
            {
                FirstObject = null;
                PointerUpObject = null;
            }
            ResetDragEvent = resetDrag;
        }

        /// <summary>
        /// 触发拖拽重置事件
        /// </summary>
        public void ResetDragEventRaise(Action completeCallBack)
        {
            if (dlog) Debug.Log("Reset drag");
            ResetDragEvent?.Invoke(completeCallBack);
        }
    }
}