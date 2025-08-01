using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// 实心线渲染器，用于在编辑器中绘制辅助线
    /// </summary>
    public class SolidLineRenderer : MonoBehaviour
    {
        [SerializeField]
        private float width = 0.15f; // 线宽
        [SerializeField]
        private Material material; // 材质
        [SerializeField]
        private int sortingOrder = 0; // 渲染顺序

        #region temp vars
        private LineRenderer lineRenderer; // 线渲染器
        private Vector3 offset; // 偏移量
        private Vector3 sourcePos_1; // 起点
        private Vector3 sourcePos_2; // 终点
        #endregion temp vars

        /// <summary>
        /// 创建一条实心线
        /// </summary>
        public SolidLineRenderer Create (Transform parent, Vector3 pos1, Vector3 pos2)
        {
            Material mat = (!material) ? new Material(Shader.Find("Sprites/Default")) : material;
            
            SolidLineRenderer sLR = Instantiate(this, parent.transform);
            if (!sLR) return null;
            sLR.sourcePos_1 = pos1;
            sLR.sourcePos_2 = pos2;
            sLR.lineRenderer = sLR.GetComponent<LineRenderer>();
            sLR.lineRenderer.material = mat;
            sLR.lineRenderer.startWidth = width;
            sLR.lineRenderer.endWidth = width;
            sLR.lineRenderer.startColor = new Color(1,0,0,0.3f);
            sLR.lineRenderer.endColor = new Color(1, 0, 0, 0.3f);
            sLR.lineRenderer.sortingOrder = sortingOrder ;

            Vector3 [] positions = new Vector3 [] {pos1, pos2 }; // world pos
            sLR.lineRenderer.positionCount = positions.Length;
            sLR.lineRenderer.SetPositions(positions);
            return sLR;
        }

        /// <summary>
        /// 设置线是否可见
        /// </summary>
        internal void SetLineVisible(bool visible)
        {
            if (lineRenderer) lineRenderer.enabled = visible;
        }

        /// <summary>
        /// 设置线的偏移
        /// </summary>
        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
            Vector3[] positions = new Vector3[] { sourcePos_1 + offset, sourcePos_2 + offset }; // world pos
            lineRenderer.SetPositions(positions);
        }

        #region private
        /// <summary>
        /// 设置线的渲染顺序
        /// </summary>
        private void SetLineRenderOrder(int order)
        {
            if (lineRenderer) lineRenderer.sortingOrder = order;
        }
        #endregion private
    }
}