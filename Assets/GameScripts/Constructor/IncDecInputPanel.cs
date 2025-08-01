using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mkey
{
    /// <summary>
    /// 增减输入面板，支持按钮、输入框、切换、图片等多种UI交互
    /// </summary>
    public class IncDecInputPanel : MonoBehaviour
    {
        public Button incButton; // 增加按钮
        public Button decButton; // 减少按钮
        public InputField inputField; // 输入框
        public Text textField; // 文本显示
        public Image image; // 图片
        public Toggle toggle; // 开关

        public Button objectButton; // 对象按钮
        public Text objectButtonText; // 对象按钮文本

        /// <summary>
        /// 创建带增减、输入、切换等功能的面板
        /// </summary>
        public static IncDecInputPanel Create(RectTransform parent, IncDecInputPanel prefab, string text, string inputFielText, bool toggleOn, UnityAction incDelegate, UnityAction decDelegate, UnityAction<string> inputChangedDelegate, UnityAction <bool> toogleChange, Func<string> refreshDelegate, Sprite image)
        {
            if (!prefab) return null;
            IncDecInputPanel panel = Instantiate(prefab).GetComponent<IncDecInputPanel>();
            if (!panel) return null;
            if (parent) panel.GetComponent<RectTransform>().SetParent(parent);
            if (panel.textField) panel.textField.text = text;

            if (panel.inputField)
            {
                panel.inputField.text = inputFielText;
                panel.inputField.onValueChanged.AddListener(inputChangedDelegate);
                panel.inputField.onValueChanged.AddListener((val) => { if (refreshDelegate != null) panel.inputField.text = refreshDelegate(); });
            }

            if (panel.incButton)
            {
                if (incDelegate != null)
                {
                    panel.incButton.onClick.RemoveAllListeners();
                    panel.incButton.onClick.AddListener(incDelegate);
                    panel.incButton.onClick.AddListener(() => { if (refreshDelegate != null) panel.inputField.text = refreshDelegate(); });
                }
                else
                {
                    panel.incButton.gameObject.SetActive(false);
                }
            }

            if (panel.decButton)
            {
                if (decDelegate != null)
                {
                    panel.decButton.onClick.RemoveAllListeners();
                    panel.decButton.onClick.AddListener(decDelegate);
                    panel.decButton.onClick.AddListener(() => { if (refreshDelegate != null) panel.inputField.text = refreshDelegate(); });
                }
                else
                {
                    panel.decButton.gameObject.SetActive(false);
                }
            }

            if (panel.image)
            {
                panel.image.enabled = image;
                panel.image.sprite = image;
            }
            if (panel.toggle)
            {
                panel.toggle.isOn = toggleOn;
                if (toogleChange != null)
                {
                    panel.toggle.onValueChanged.RemoveAllListeners();
                    panel.toggle.onValueChanged.AddListener(toogleChange);
                    panel.toggle.onValueChanged.AddListener((val) => { if (refreshDelegate != null) panel.inputField.text = refreshDelegate();});
                }
                else
                {
                    panel.toggle.gameObject.SetActive(false);
                }
            }
            return panel;
        }

        /// <summary>
        /// 创建带增减、输入功能的面板（无切换）
        /// </summary>
        public static IncDecInputPanel Create(RectTransform parent, IncDecInputPanel prefab, string text, string inputFielText, UnityAction incDelegate, UnityAction decDelegate, UnityAction<string> inputChangedDelegate, Func<string> refreshDelegate, Sprite image)
        {
            return Create(parent, prefab, text, inputFielText, false, incDelegate, decDelegate, inputChangedDelegate, null, refreshDelegate, image);
        }

        /// <summary>
        /// 创建带输入功能的面板（无增减、切换）
        /// </summary>
        public static IncDecInputPanel Create(RectTransform parent, IncDecInputPanel prefab, string text, string inputFielText, UnityAction<string> inputChangedDelegate, Func<string> refreshDelegate, Sprite image)
        {
            return Create(parent, prefab, text, inputFielText, null, null, inputChangedDelegate, refreshDelegate, image);
        }
    
        /// <summary>
        /// 设置对象按钮文本
        /// </summary>
        public void SetObjectButtonText(string text)
        {
            if (objectButtonText)
            {
                objectButtonText.text = string.IsNullOrEmpty(objectButtonText.text) ? text : ""; 
            }
        }
    }
}