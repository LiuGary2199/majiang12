using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using Spine.Unity;
using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// 麻将牌对象，继承自GridObject，包含匹配、阻挡、渲染等逻辑
    /// </summary>
    public class MahjongTile : GridObject
    {
        public Vector3 layerOffset; // 层偏移量
        public Sprite MSprite => SRenderer.sprite; // 当前精灵

        public GameObject selectPrefab; // 选中高亮预制体
        public GameObject hintPrefab; // 提示高亮预制体
        public SpriteRenderer leftBorder; // 左边框
        public SpriteRenderer shadow; // 阴影
        public SpriteRenderer constructLineHor; // 编辑器横线
        public SpriteRenderer constructLineVert; // 编辑器竖线
        public TextMesh constructLayerText; // 层数文本
        public bool IsgoldTile= false; // 是否为金牌

        internal List<GridCell> rawOverBlockers; // 上方阻挡格子
        internal List<GridCell> rawLeftBlockers; // 左侧阻挡格子
        internal List<GridCell> rawRightBlockers; // 右侧阻挡格子

        public Sprite goldSprite;

        #region temp vars
        private GameObject hintObject; // 提示对象
        private GameObject selectObject; // 选中对象
        private Vector3 spriteTransformPosition; // 精灵初始位置
        private bool highlightedHint = false; // 是否高亮提示
        private bool highlightedSel = false; // 是否高亮选中
        #endregion temp vars

        #region properties
        public int OccupiedCols => 2; // 占用列数
        public int OccupiedRows => 2; // 占用行数
        private TouchManager TouchM { get { return TouchManager.Instance; } } // 触摸管理器
        public Transform spriteTransform => SRenderer.transform; // 精灵变换

        public Transform insGoldTrans;
        public GameObject qian_Skeleton;

        #endregion properties

        #region override
        /// <summary>
        /// 设置渲染顺序，前置/还原
        /// </summary>
        public override void SetToFront(bool toFront)
        {
            SRenderer.sortingOrder = GetRenderOrder(toFront);
            if (shadow) shadow.sortingOrder = (toFront) ? 20000 - 1:  SortingOrder.Base + Layer * 2000;
            // set border
            if (toFront)
            {
                leftBorder.enabled = false;
            }
            else
            {
                EnableLeftBorder();
            }
        }

        public override string ToString()
        {
            return "MahjongTile: " + Layer;
        }

        public override GridObject Create(GridCell parent, int layer)
        {
            if (!parent) return null;
            Layer = layer;
            DestroyHierCompetitor(parent, true, true);
           
            MahjongTile gridObject = Instantiate(this, parent.transform);
            if (!gridObject) return null;
            gridObject.SetLayer(layer);
            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero + layerOffset * layer;
            gridObject.ParentCell = parent;
#if UNITY_EDITOR
            gridObject.name = "MahjongTile " + parent.ToString();
#endif
            // Debug.Log("create gameobject: " + parent + "; " + gridObject);

            return gridObject;
        }

        /// <summary>
        /// 关联到指定格子
        /// </summary>
        public void LinkToCell(GridCell gridCell, bool setPosition)
        {
            transform.parent = gridCell.transform;
            if(setPosition) transform.localPosition = Vector3.zero + layerOffset * Layer;
            ParentCell = gridCell;
#if UNITY_EDITOR
            name = "MahjongTile " + ParentCell.ToString();
#endif
        }

        public override bool CanSetBySize(GridCell gCell)
        {
            List<GridCell> cells = GetOccupiedCells(gCell);
            return (cells.Count == OccupiedCols * OccupiedRows);
        }

        /// <summary>
        /// 检查该对象能否按层放置在格子上
        /// </summary>
        /// <param name="gCell"></param>
        /// <returns></returns>
        public override bool CanSetByLayer(GridCell gCell, int layer)
        {
            if (layer == 0) return true;
            if (layer > 0)
            {
                int underLayer = layer - 1;
                List<GridCell> cells = GetOccupiedCells(gCell);
                foreach (var item in cells)
                {
                    GridObject gO = item.GetLayerObject(underLayer, true, true);
                    if (!gO) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取占用尺寸（行，列）
        /// </summary>
        /// <returns></returns>
        public override Vector2Int GetSize()
        {
            return new Vector2Int(OccupiedRows, OccupiedCols);
        }

        public override List<GridCell> GetOccupiedCells(GridCell gCell)
        {
            List<GridCell> res = new List<GridCell>();
            int cRow = gCell.Row;
            int cCol = gCell.Column;
            MatchGrid mGrid = gCell.MGrid;
            GridCell _gCell;
            for (int r = cRow; r > cRow - OccupiedRows; r--)
            {
                for (int c = cCol; c < cCol + OccupiedCols; c++)
                {
                    _gCell = mGrid[r, c];
                    if (_gCell) res.Add(_gCell);
                }
            }
            return res;
        }
        #endregion override

        /// <summary>
        /// 判断该格子是否可被填充
        /// </summary>
        public bool IsFreeToFill()
        {
            NeighBors neighBors = ParentCell.Neighbors;
            // over
            GridObject o1 = (neighBors.Main_1) ? neighBors.Main_1.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o2 = (neighBors.Main_2) ? neighBors.Main_2.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o3 = (neighBors.Main_3) ? neighBors.Main_3.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o4 = (neighBors.Main_4) ? neighBors.Main_4.GetLayerObject(Layer + 1, true, true) : null;
            bool overBlocked1 = (o1 != null && !o1.Excluded);
            bool overBlocked2 = (o2 != null && !o2.Excluded);
            bool overBlocked3 = (o3 != null && !o3.Excluded);
            bool overBlocked4 = (o4 != null && !o4.Excluded);
            bool overBlocked = (overBlocked1 || overBlocked2 || overBlocked3 || overBlocked4);
            if (overBlocked) return false;

            // left
            GridObject l1 = (neighBors.Left_1) ? neighBors.Left_1.GetLayerObject(Layer, true, true) : null;
            GridObject l2  = (neighBors.Left_2) ? neighBors.Left_2.GetLayerObject(Layer, true, true) : null;
            bool leftBlocked1 = (l1 != null && !l1.Excluded);
            bool leftBlocked2 = (l2 != null && !l2.Excluded);
            bool leftBlocked = leftBlocked1 || leftBlocked2;
            if (!leftBlocked) return true;

            // right
            GridObject r1 = (neighBors.Right_1) ? neighBors.Right_1.GetLayerObject(Layer, true, true) : null;
            GridObject r2 = (neighBors.Right_2) ? neighBors.Right_2.GetLayerObject(Layer, true, true) : null;
            bool rightBlocked1 = (r1 != null && !r1.Excluded);
            bool rightBlocked2 = (r2 != null && !r2.Excluded);
            bool rightBlocked = rightBlocked1 || rightBlocked2;
            if (!rightBlocked) return true;

            return false;
        }

        /// <summary>
        /// 判断该格子是否可被匹配（旧逻辑）
        /// </summary>
        public bool IsFreeToMatch_old()
        {
            NeighBors neighBors = base.ParentCell.Neighbors;
            // over
            GridObject o1 = (neighBors.Main_1) ? neighBors.Main_1.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o2 = (neighBors.Main_2) ? neighBors.Main_2.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o3 = (neighBors.Main_3) ? neighBors.Main_3.GetLayerObject(Layer + 1, true, true) : null;
            GridObject o4 = (neighBors.Main_4) ? neighBors.Main_4.GetLayerObject(Layer + 1, true, true) : null;
            bool overBlocked1 = (o1 != null);
            bool overBlocked2 = (o2 != null);
            bool overBlocked3 = (o3 != null);
            bool overBlocked4 = (o4 != null);
            bool overBlocked = (overBlocked1 || overBlocked2 || overBlocked3 || overBlocked4);
            if (overBlocked) return false;

            // left
            GridObject l1 = (neighBors.Left_1) ? neighBors.Left_1.GetLayerObject(Layer, true, true) : null;
            GridObject l2 = (neighBors.Left_2) ? neighBors.Left_2.GetLayerObject(Layer, true, true) : null;
            bool leftBlocked1 = (l1 != null);
            bool leftBlocked2 = (l2 != null);
            bool leftBlocked = leftBlocked1 || leftBlocked2;
            if (!leftBlocked) return true;

            // right
            GridObject r1 = (neighBors.Right_1) ? neighBors.Right_1.GetLayerObject(Layer, true, true) : null;
            GridObject r2 = (neighBors.Right_2) ? neighBors.Right_2.GetLayerObject(Layer, true, true) : null;
            bool rightBlocked1 = (r1 != null);
            bool rightBlocked2 = (r2 != null);
            bool rightBlocked = rightBlocked1 || rightBlocked2;
            if (!rightBlocked) return true;

            return false;
        }

        /// <summary>
        /// 判断该格子是否可被匹配（新逻辑，使用缓存阻挡）
        /// </summary>
        public bool IsFreeToMatch()
        {
            // use cached raw blockers
            GridObject bl = null;
            //check over blocked
            foreach (var gCell in rawOverBlockers)
            {
                bl = gCell.GetLayerObject(Layer + 1, false, true);
                if (bl) return false;
            }

            // check left blocked
            bl = null;
            foreach (var gCell in rawLeftBlockers)
            {
                bl = gCell.GetLayerObject(Layer, false, true);
                if (bl) break;
            }
            if (!bl) return true;

            // check right blocked
            GridObject blr = null;
            foreach (var gCell in rawRightBlockers)
            {
                blr = gCell.GetLayerObject(Layer, false, true);
                if (blr) break;
            }
            if (!blr) return true;

            return false;
        }

        #region cache raw blockers
        /// <summary>
        /// 获取上方阻挡格子列表
        /// </summary>
        private List<GridCell> GetOverBlockers()
        {
            NeighBors neighBors = ParentCell.Neighbors;
            List<GridCell> blockers = new List<GridCell>();
            GridObject bl = (neighBors.Main_1) ? neighBors.Main_1.GetLayerObject(Layer + 1, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            bl = (neighBors.Main_2) ? neighBors.Main_2.GetLayerObject(Layer + 1, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            bl = (neighBors.Main_3) ? neighBors.Main_3.GetLayerObject(Layer + 1, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            bl = (neighBors.Main_4) ? neighBors.Main_4.GetLayerObject(Layer + 1, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            return blockers;
        }

        /// <summary>
        /// 获取左侧阻挡格子列表
        /// </summary>
        private List<GridCell> GetLeftBlockers()
        {
            NeighBors neighBors = ParentCell.Neighbors;
            List<GridCell> blockers = new List<GridCell>();
            GridObject bl = (neighBors.Left_1) ? neighBors.Left_1.GetLayerObject(Layer, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            bl = (neighBors.Left_2) ? neighBors.Left_2.GetLayerObject(Layer, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            return blockers;
        }

        /// <summary>
        /// 获取右侧阻挡格子列表
        /// </summary>
        private List<GridCell> GetRightBlockers()
        {
            NeighBors neighBors = ParentCell.Neighbors;
            List<GridCell> blockers = new List<GridCell>();
            GridObject bl = (neighBors.Right_1) ? neighBors.Right_1.GetLayerObject(Layer, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            bl = (neighBors.Right_2) ? neighBors.Right_2.GetLayerObject(Layer, true, true) : null;
            if (bl) blockers.Add(bl.ParentCell);
            return blockers;
        }

        /// <summary>
        /// 缓存阻挡格子
        /// </summary>
        public void CacheRawBlockers()
        {
            rawOverBlockers = GetOverBlockers();
            rawLeftBlockers = GetLeftBlockers();
            rawRightBlockers = GetRightBlockers();
        }
        #endregion cache raw blockers

        /// <summary>
        /// 获取左下角阻挡的麻将牌
        /// </summary>
        public MahjongTile GetBottomLeftBlocker()
        {
            NeighBors neighBors = ParentCell.Neighbors;
            GridObject l1 = (neighBors.Left_1) ? neighBors.Left_1.GetLayerObject(Layer, true, true) : null;
            GridObject l2 = (neighBors.Left_2) ? neighBors.Left_2.GetLayerObject(Layer, true, true) : null;
            if (l2 && (l1 != l2)) return l2.GetComponent<MahjongTile>();
            return null;
        }

        /// <summary>
        /// 获取指定格子上占用的麻将牌
        /// </summary>
        /// <param name="matchGrid"></param>
        /// <param name="gridCell"></param>
        /// <returns></returns>
        public static MahjongTile GetOccupied(MatchGrid matchGrid, GridCell gridCell)
        {
            MahjongTile[] source = matchGrid.Parent.GetComponentsInChildren<MahjongTile>();

            foreach (var item in source)
            {
                List<GridCell> occupiedCells = item.GetOccupiedCells();
                if (occupiedCells.Contains(gridCell)) return item;
            }
            return null;
        }

        /// <summary>
        /// 设置排除状态
        /// </summary>
        /// <param name="exclude"></param>
        public void SetExcluded(bool exclude)
        {
            if(exclude != Excluded)
            {
                Excluded = exclude;
            }
        }

        /// <summary>
        /// 高亮提示
        /// </summary>
        public void HighlightHint(bool highlight)
        {
            if (highlightedHint == highlight) return;
            bool useColor = (hintPrefab== null);
            if (useColor) 
            {
                if (highlight)
                {
                  //  SRenderer.color = new Color(1f, 0.856f, 0.504f);
                   // leftBorder.color = new Color(1f, 0.856f, 0.504f);
                     SRenderer.color = new Color(1f, 1f, 1f);
                    leftBorder.color = new Color(1f, 1f, 1f);
                }
                else
                {
                    SRenderer.color = new Color(1f, 1f, 1f);
                    leftBorder.color = new Color(1f, 1f, 1f);
                }
            }
            else // use prefab
            {
                if (highlight)
                {
                    if (!hintObject)
                    {
                        hintObject = Instantiate(hintPrefab, SRenderer.transform);
                        hintObject.GetComponent<SpriteRenderer>().sortingOrder = GetRenderOrder(true) + 2;
                    }
                }
                else
                {
                    GameObject old = hintObject;
                    if(old) Destroy(old);
                }
            }
            highlightedHint = highlight;
        }

        /// <summary>
        /// 高亮选中
        /// </summary>
        public void HighlightSelected(bool highlight)
        {
            if (highlightedSel == highlight) return;
            if (!SRenderer) return;
            bool useColor = (hintPrefab == null);
            if (useColor)
            {
                if (highlight)
                {
                    //SRenderer.color = new Color(1f, 0.856f, 0.504f);
                  //  leftBorder.color = new Color(1f, 0.856f, 0.504f);


                    SRenderer.color = new Color(1f, 1f, 1f);
                    leftBorder.color = new Color(1f, 1f, 1f);
                }
                else
                {
                    SRenderer.color = new Color(1f, 1f, 1f);
                    leftBorder.color = new Color(1f, 1f, 1f);
                }
            }
            else // use prefab
            {
                if (highlight)
                {
                    if (!selectObject)
                    {
                        selectObject = Instantiate(selectPrefab, SRenderer.transform);
                        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                        selectObject.GetComponent<SpriteRenderer>().sortingOrder = GetRenderOrder(true) + 1;
                    }
                }
                else
                {
                    GameObject old = selectObject;
                    if (old) Destroy(old);
                }
            }
            highlightedSel = highlight;
        }

        /// <summary>
        /// 设置自由高亮颜色
        /// </summary>
        internal void SetFreeHiglightColor(bool highLight)
        {
            //SRenderer.color = highLight ? Color.white : new Color(0.88f, 0.88f, 0.88f, 1);
            //if (leftBorder) leftBorder.color = highLight ? Color.white : new Color(0.88f, 0.88f, 0.88f, 1);

            SRenderer.color = highLight ? Color.white : Color.white;
            if (leftBorder) leftBorder.color = highLight ? Color.white : Color.white;
        }
        // #endregion highlight

        /// <summary>
        /// 判断精灵是否可匹配
        /// </summary>
        public bool SpriteCanMatchhWith(Sprite other)
        {
            return MSprite == other || GOSet.IsOneGroup(MSprite, other);
        }

        /// <summary>
        /// 获取渲染顺序
        /// </summary>
        private int GetRenderOrder(bool onFront)
        {
            int layerOrder =(onFront)? 20000 : Layer * 2000;

            int addOrder = (ParentCell) ? ParentCell.AddRenderOrder : 0;

            if (onFront)
                return SortingOrder.MahjongTileToFront + addOrder + layerOrder;
            else
               return SortingOrder.MahjongTile + addOrder + layerOrder;
        }

        /// <summary>
        /// 启用左边框
        /// </summary>
        private void EnableLeftBorder()
        {
            MahjongTile bL = GetBottomLeftBlocker();
            if(bL)
            {
                int rO = bL.GetRenderOrder(false);
                leftBorder.sortingOrder = rO + 1;
                leftBorder.enabled = true;
            }
            else
            {
                leftBorder.enabled = false;
            }
        }

        /// <summary>
        /// 混合跳跃动画
        /// </summary>
        internal void MixJump(Vector3 toPosition, Action completeCallBack)
        {
            if (hintObject) HighlightHint(false);
            if (selectObject) HighlightSelected(false);
            spriteTransformPosition = spriteTransform.position;
            SimpleTween.Move(SRenderer.gameObject, SRenderer.transform.position, toPosition, 0.5f).AddCompleteCallBack(() =>
            {
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseInSine);
        }

        /// <summary>
        /// 反向混合跳跃动画
        /// </summary>
        internal void ReversMixJump(Action completeCallBack)
        {
            SimpleTween.Move(SRenderer.gameObject, SRenderer.transform.position, spriteTransformPosition, 0.5f).AddCompleteCallBack(() =>
            {
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseInSine);
        }

        /// <summary>
        /// 显示编辑器辅助线
        /// </summary>
        public void ShowConstructLines(bool show, float alpha)
        {
            int renderOrder = GetRenderOrder(false);
            int renderOrder_L = leftBorder.enabled ? leftBorder.sortingOrder : renderOrder;
            renderOrder = (renderOrder_L > renderOrder) ? renderOrder_L + 2 : renderOrder + 2;

            if (constructLineHor) 
            { 
                constructLineHor.gameObject.SetActive(show);
                constructLineHor.sortingOrder = renderOrder;
                constructLineHor.color = new Color(constructLineHor.color.r, constructLineHor.color.g, constructLineHor.color.b, alpha);
            }
            if (constructLineVert) 
            { 
                constructLineVert.gameObject.SetActive(show);
                constructLineVert.sortingOrder = renderOrder;
                constructLineVert.color = new Color(constructLineHor.color.r, constructLineHor.color.g, constructLineHor.color.b, alpha);
            }

            if (constructLayerText)
            {
                constructLayerText.gameObject.SetActive(show);
                constructLayerText.text = (Layer + 1).ToString();
                constructLayerText.color = new Color(constructLayerText.color.r, constructLayerText.color.g, constructLayerText.color.b, alpha);
                SpriteText rend = constructLayerText.GetComponent<SpriteText>();
                if (rend) 
                { 
                    rend.SortingOrder = renderOrder;
                }
            }
        }

        /// <summary>
        /// 设置编辑器辅助线颜色
        /// </summary>
        public void SetConstructColor(Color color)
        {
            SRenderer.color = color;
            leftBorder.color = color;
        }

        /// <summary>
        /// 抖动动画反馈（先快后慢，幅度逐渐减小，更自然）
        /// </summary>
        public void Shake(float duration = 0.5f, float strength = 0.16f, int vibrato = 10)
        {
            // 第一次Shake时记录原点
            if (shakeOriginPos == Vector3.zero)
                shakeOriginPos = spriteTransform.localPosition;
            // 每次抖动前都归位
            spriteTransform.localPosition = shakeOriginPos;
            Vector3 originalPos = shakeOriginPos;
            int totalSteps = vibrato;
            float totalTime = duration;
            float timePassed = 0f;
            float[] stepTimes = new float[totalSteps];
            float[] stepStrengths = new float[totalSteps];
            float baseTime = totalTime * 0.5f / totalSteps; // 前半段快
            float slowTime = totalTime * 0.5f / totalSteps; // 后半段慢
            float baseStrength = strength;

            // 生成每步的时间和幅度（前快后慢，幅度递减）
            for (int i = 0; i < totalSteps; i++)
            {
                if (i < totalSteps / 2)
                {
                    stepTimes[i] = baseTime;
                    stepStrengths[i] = baseStrength;
                }
                else
                {
                    stepTimes[i] = slowTime * (1.2f + (i - totalSteps / 2) * 0.2f); // 越来越慢
                    stepStrengths[i] = baseStrength * (1f - 0.7f * (i - totalSteps / 2) / (totalSteps / 2)); // 幅度递减
                }
            }
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
            // 启动协程实现分步抖动
            StartCoroutine(ShakeCoroutine(spriteTransform, originalPos, stepTimes, stepStrengths));
        }

        private IEnumerator ShakeCoroutine(Transform target, Vector3 originalPos, float[] stepTimes, float[] stepStrengths)
        {
            for (int i = 0; i < stepTimes.Length; i++)
            {
                float offset = (i % 2 == 0 ? 1 : -1) * stepStrengths[i];
                float t = 0f;
                Vector3 start = shakeOriginPos;
                Vector3 end = shakeOriginPos + new Vector3(offset, 0, 0);
                while (t < stepTimes[i])
                {
                    t += Time.deltaTime;
                    float lerp = Mathf.Clamp01(t / stepTimes[i]);
                    target.localPosition = Vector3.Lerp(start, end, lerp);
                    yield return null;
                }
                // 回到原位
                target.localPosition = shakeOriginPos;
            }
        }

        /// <summary>
        /// 设置金麻将状态，并切换图片，关闭leftBorder
        /// </summary>
        public void SetGoldState(bool isGold)
        {
            IsgoldTile = isGold;
            if (isGold)
            {
                if (goldSprite) SRenderer.sprite = goldSprite;
                if (leftBorder) leftBorder.enabled = false;
                qian_Skeleton.gameObject.SetActive(true);
                GameObject hou_Skeleton = Instantiate(qian_Skeleton, insGoldTrans);
                SkeletonAnimation skeletonAnimation = qian_Skeleton.GetComponent<SkeletonAnimation>();

                skeletonAnimation.gameObject.transform.localPosition = new Vector3(-0.3f, -1.68f, 0);
                skeletonAnimation.gameObject.transform.localScale = new Vector3(1f, 0.85f, 0);
                MeshRenderer m_Mr = skeletonAnimation.GetComponent<MeshRenderer>();
                m_Mr.sortingOrder = SRenderer.sortingOrder + 1;

                SkeletonAnimation skeletonAnimation1 = hou_Skeleton.GetComponent<SkeletonAnimation>();
                skeletonAnimation1.gameObject.transform.localPosition = new Vector3(0, -1.73f, 0);
                skeletonAnimation1.gameObject.transform.localScale = new Vector3(0.8f, 0.75f, 0);
                skeletonAnimation1.state.SetAnimation(0, "2", true);
                MeshRenderer m_Mr1 = skeletonAnimation1.GetComponent<MeshRenderer>();
                m_Mr1.sortingOrder = SRenderer.sortingOrder - 1;
            }
            // 如需恢复普通麻将，可在此处加else逻辑
        }

        /// <summary>
        /// 设置麻将牌变灰或恢复
        /// </summary>
        public void SetGray(bool isGray)
        {
            Color gray = new Color(0.6f, 0.6f, 0.6f, 1f);
            if (isGray)
            {
                if (SRenderer) SRenderer.color = gray;
                if (leftBorder) leftBorder.color = gray;
            }
            else
            {
                if (SRenderer) SRenderer.color = Color.white;
                if (leftBorder) leftBorder.color = Color.white;
            }
        }

        // 记录抖动原点，防止多次Shake累计偏移
        private Vector3 shakeOriginPos = Vector3.zero;
        public void PlayNbroke(Action finish)
        {
            Lie.sortingOrder = SRenderer.sortingOrder + 1;
            if (hintObject)
            {
                Destroy(hintObject);
            } // 提示对象
            if (selectObject)
            {
                Destroy(selectObject);
            }
            m_TileAnimController.PlayNbroke(() =>
            {
                finish?.Invoke();
            });
        }

        public void PlayGbroke(Action finish)
        {
            Lie.sortingOrder = SRenderer.sortingOrder + 1;
            if (hintObject)
            {
                Destroy(hintObject);
            } // 提示对象
            if (selectObject)
            {
                Destroy(selectObject);
            }
            m_TileAnimController.PlayNbroke(() =>
            {
                finish?.Invoke();
            });
        }
        public void PlayShak(Action finish)
        {
            m_TileAnimController.PlayShak(() =>
            {
                finish?.Invoke();
            });
        }
    }
}
