using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Mkey
{
    /// <summary>
    /// 滚动面板控制器，负责面板的开关动画与交互
    /// </summary>
    public class ScrollPanelController : MonoBehaviour
    {
        public Text textCaption; // 标题文本
        public RectTransform scrollContent; // 滚动内容

        private void OnDestroy()
        {
            // 销毁时取消所有Tween动画
            SimpleTween.Cancel(gameObject, true);
        }

        /// <summary>
        /// 打开滚动面板
        /// </summary>
        /// <param name="panel"></param>
        public void OpenScrollPanel(Action completeCallBack )
        {
            RectTransform panel = GetComponent<RectTransform>();
            transform.localScale = new Vector3(0, 1, 1);
            if (!panel) return;
            SetControlActivity( false);
            float startX = 0;
            float endX = 1f;
            SimpleTween.Cancel(gameObject, true);

            SimpleTween.Value(gameObject, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
                               SetOnUpdate((float val) =>
                               {
                                   transform.localScale = new Vector3(val, 1, 1);
                               }).AddCompleteCallBack(() =>
                               {
                                   SetControlActivity(true);
                                   completeCallBack?.Invoke();
                               });
        }

        /// <summary>
        /// 关闭滚动面板
        /// </summary>
        /// <param name="panel"></param>
        public void CloseScrollPanel(bool destroy, Action completeCallBack)
        {
            RectTransform panel = GetComponent<RectTransform>();
            if (!panel) return;
            SetControlActivity(false);
            float startX = 1;
            float endX = 0f;
            SimpleTween.Cancel(gameObject, true);
            SimpleTween.Value(gameObject, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
                               SetOnUpdate((float val) =>
                               {
                                   transform.localScale = new Vector3(val, 1, 1);
                               }).AddCompleteCallBack(() =>
                               {
                                   if (destroy && this) Destroy(gameObject);
                                   completeCallBack?.Invoke();
                               });
        }

        /// <summary>
        /// 设置面板内按钮交互状态
        /// </summary>
        private void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

    }
}