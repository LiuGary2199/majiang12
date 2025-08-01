using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mkey
{
    /// <summary>
    /// 面板容器控制器，负责面板的实例化与切换
    /// </summary>
    public class PanelContainerController : MonoBehaviour
    {
        public Button OpenCloseButton; // 开关按钮
        public Button BrushSelectButton; // 画刷选择按钮
        public Image selector; // 选择器图片
        public Image brushImage; // 画刷图片
        public Text BrushName; // 画刷名称
        public string capital; // 标题
        public List<GridObject> gridObjects; // 对象列表

        [SerializeField]
        private ScrollPanelController ScrollPanelPrefab; // 滚动面板预制体
        [SerializeField]
        private ScrollPanelController ScrollPanelPrefabSmall; // 小滚动面板预制体
        internal ScrollPanelController ScrollPanel; // 当前滚动面板

        /// <summary>
        /// 实例化标准滚动面板
        /// </summary>
        public ScrollPanelController InstantiateScrollPanel()
        {
            return InstantiateScrollPanel(ScrollPanelPrefab);
        }

        /// <summary>
        /// 实例化小滚动面板
        /// </summary>
        public ScrollPanelController InstantiateScrollPanelSmall()
        {
            return InstantiateScrollPanel(ScrollPanelPrefabSmall);
        }

        /// <summary>
        /// 内部方法：实例化指定滚动面板
        /// </summary>
        private ScrollPanelController InstantiateScrollPanel(ScrollPanelController scrollPanelPrefab)
        {
            if (!scrollPanelPrefab) return null;

            if (ScrollPanel) DestroyImmediate(ScrollPanel.gameObject);

            RectTransform panel = Instantiate(scrollPanelPrefab).GetComponent<RectTransform>();
            panel.SetParent(GetComponent<RectTransform>());
            panel.anchoredPosition = new Vector2(0, 0);
            ScrollPanel = panel.GetComponent<ScrollPanelController>();
            return ScrollPanel;
        }
    }
}