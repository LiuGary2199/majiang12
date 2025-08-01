using UnityEngine;

namespace Mkey
{
	/// <summary>
	/// 麻将牌触摸行为，处理点击、拖拽、匹配等交互
	/// </summary>
	public class TileTouchBehavior : TouchPadMessageTarget
	{
        public MahjongTile mahjongTile; // 当前关联的麻将牌对象

        #region temp vars
        private TouchManager TouchM { get { return TouchManager.Instance; } } // 触摸管理器单例
        private Transform spriteTransform; // 精灵变换组件
        private Vector2 spritetransformPosition; // 精灵初始位置
        private bool MayBeDeselect = false; // 是否可能取消选中
        private GameBoard MBoard { get { return GameBoard.Instance; } } // 游戏主面板单例
        #endregion temp vars

        private void Start()
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                PointerDownEvent = PointerDownEventHandler;
                PointerUpEvent = PointerUpEventHandler;
                DragEvent = DragEventHandler;
                mahjongTile = GetComponent<MahjongTile>();
                spriteTransform = mahjongTile.SRenderer.transform;
                spritetransformPosition = spriteTransform.position;
            }
            else
            {
                PointerDownEvent = PointerDownEventHandler;
            }
        }

        #region touchbehavior
        private void PointerDownEventHandler(TouchPadEventArgs tpea)
        {
            // 新增：第二关第一步点击限制，只允许字典第0组的两张牌
            if (Mkey.TutorialGuide.Instance != null && Mkey.TutorialGuide.Instance.IsTutorialLevel2Active())
            {
                if (Mkey.TutorialGuide.Instance.GetCurrentLevel2PairIndex() == 0)
                {
                    var pair = Mkey.TutorialGuide.Instance.GetLevel2Pair(0);
                    if (mahjongTile != pair.Item1 && mahjongTile != pair.Item2)
                    {
                        return;
                    }
                }
            }
            if (GameBoard.GMode == GameMode.Play)
            {
                // 新增：点击麻将牌时播放音效
                PanelEre.GetInstance().InchBellow(PanelRate.SceneMusic.Sound_Clickmj);
                Debug.Log("pointer down: " + this);
                Debug.Log($"[配对调试] 当前点击: {mahjongTile}, IsFirstObject: {IsFirstObject()}, TouchM.FirstObject: {TouchM.FirstObject}");
                MayBeDeselect = false;
                wasDragged = false;

                // 新手引导点击检测
                if (Mkey.TutorialGuide.Instance != null)
                {
                    // 第一关强引导
                    if (Mkey.TutorialGuide.Instance.IsInTutorial())
                    {
                        if (!Mkey.TutorialGuide.Instance.IsValidTutorialClick(mahjongTile))
                        {
                            Debug.Log("[新手引导] 非当前步骤高亮麻将，点击无效");
                            return;
                        }
                        Mkey.TutorialGuide.Instance.OnMahjongTileClicked(mahjongTile);
                        // 不return，允许后续配对消除逻辑继续执行
                    }
                    // 第二关强引导
                    else if (Mkey.TutorialGuide.Instance.IsTutorialLevel2Active())
                    {
                        if (!Mkey.TutorialGuide.Instance.IsValidTutorialClickLevel2(mahjongTile))
                        {
                            Debug.Log("[新手引导-第二关] 非当前步骤高亮麻将，点击无效");
                            return;
                        }
                        Mkey.TutorialGuide.Instance.OnMahjongTileClickedLevel2(mahjongTile);
                        // 不return，允许后续配对消除逻辑继续执行
                    }
                }
                if (mahjongTile.IsFreeToMatch())
                {
                    if (IsFirstObject())
                    {
                        MayBeDeselect = true;
                        TouchM.CanDrag = true;
                        return;
                    }
                    else if (TouchM.FirstObject && TouchM.FirstObject != spriteTransform) // is probably second object
                    {
                        if (CanMatchWith(TouchM.FirstObject))
                        {
                            HighlightSelected(true);
                            MatchWith(TouchM.FirstObject);
                            TouchM.CanDrag = false;
                            return;
                        }
                        else
                        {
                            HighlightBothSelected(false);
                            SetAsFirstObject();
                            HighlightSelected(true);
                            TouchM.CanDrag = false;
                            MBoard.FailedMatchEventRaise();
                        }
                    }

                    SetAsFirstObject();
                }
                else // failed match select
                {
                    HighlightBothSelected(false);
                    TouchM.SetFirstObject(null, null);
                    TouchM.CanDrag = false;
                    mahjongTile.Shake(); // 抖动反馈
                    // 新增：引导阶段特殊处理——不可消除麻将抖动后变灰
                    if (Mkey.TutorialGuide.Instance != null)
                    {
                        // 第一关第二步
                        if (Mkey.TutorialGuide.Instance.IsInTutorial() && Mkey.TutorialGuide.Instance.GetCurrentPairIndex == 1)
                        {
                            mahjongTile.SetGray(true);
                        }
                        // 第二关麻将部分第二对
                        if (Mkey.TutorialGuide.Instance.IsTutorialLevel2Active() && Mkey.TutorialGuide.Instance.currentLevel2PairIndex == 2)
                        {
                            mahjongTile.SetGray(true);
                        }
                    }
                    MBoard.FailedMatchEventRaise();
                }
            }
            else    // edit mode
            {
#if UNITY_EDITOR
                GameConstructor gameConstructor = FindAnyObjectByType<GameConstructor>();
                Vector3 tPos =  tpea.WorldPos;
                BoxCollider2D collider2D = mahjongTile.boxCollider;
                Bounds bounds = collider2D.bounds;
                // Vector3 min = bounds.min;
                // Vector3 max = bounds.max;
                int quadrant = GetQuadrant(bounds, tPos);
                // Debug.Log("collider bound min: " + min + "; max: " + max + "; quadrant: " + quadrant);
                if (quadrant == 1) gameConstructor.Cell_Click(mahjongTile.ParentCell);
                else if (quadrant == 2) gameConstructor.Cell_Click(mahjongTile.ParentCell.Neighbors.Main_2);
                else if (quadrant == 3) gameConstructor.Cell_Click(mahjongTile.ParentCell.Neighbors.Main_3);
                else if (quadrant == 4) gameConstructor.Cell_Click(mahjongTile.ParentCell.Neighbors.Main_4);
#endif
            }
        }

        bool wasDragged = false;
        private void DragEventHandler(TouchPadEventArgs tpea)
        {
            wasDragged = TouchM.MinDragReached;
        }

        private void PointerUpEventHandler(TouchPadEventArgs tpea)
        {
            // 新增：第二关第一步配对限制
            if (Mkey.TutorialGuide.Instance != null && Mkey.TutorialGuide.Instance.IsTutorialLevel2Active())
            {
                if (Mkey.TutorialGuide.Instance.GetCurrentLevel2PairIndex() == 0)
                {
                    var pair = Mkey.TutorialGuide.Instance.GetLevel2Pair(0);
                    MahjongTile otherTile = null;
                    if (TouchM.FirstObject)
                        otherTile = TouchM.FirstObject.GetComponentInParent<MahjongTile>();
                    // 只允许配对字典第0组的两张牌
                    if (!((mahjongTile == pair.Item1 || mahjongTile == pair.Item2) &&
                          (otherTile == pair.Item1 || otherTile == pair.Item2)))
                    {
                        return;
                    }
                }
            }
            if (!TouchM.FirstObject) return;
            TouchM.PointerUpObject = null;
            if (IsFirstObject()&& !wasDragged) 
            { 
                if (MayBeDeselect) 
                {
                    TouchM.SetFirstObject(null, null);
                    HighlightSelected(false);
                    return; 
                } 
            }
            else if (IsFirstObject() && wasDragged)
            {
                TouchM.ResetDragEventRaise(null);
                return;
            }
           
            // Debug.Log("pointer up: " + this + " ;TouchM.Draggable: " + TouchM.Draggable + ";CanCombine(TouchM.Draggable)" + CanCombine(TouchM.Draggable));
            if (CanMatchWith(TouchM.FirstObject))
            {
                TouchM.PointerUpObject = spriteTransform;
                TouchM.CanDrag = false;
                HighlightSelected(true);
                MatchWith(TouchM.FirstObject);
            }
            else
            {
                TouchM.CanDrag = false;
                TouchM.ResetDragEventRaise(null);
                if (!IsFirstObject())
                {
                    MBoard.FailedMatchEventRaise();
                }
            }
        }
        #endregion touchbehavior

        private bool CanMatchWith(Transform other)
        {
            // 新手引导强制配对限制
            if (Mkey.TutorialGuide.Instance != null)
            {
                if (Mkey.TutorialGuide.Instance.IsTutorialLevel2Active())
                {
                    // 只在第二关第一步做特殊限制
                    if (Mkey.TutorialGuide.Instance.GetCurrentLevel2PairIndex() == 0)
                    {
                        var pair = Mkey.TutorialGuide.Instance.GetLevel2Pair(0);
                        MahjongTile otherTile = other.GetComponentInParent<MahjongTile>();
                        if (mahjongTile == pair.Item1 || mahjongTile == pair.Item2 ||
                            otherTile == pair.Item1 || otherTile == pair.Item2)
                        {
                            // 允许配对
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            // 原有逻辑
            if (other == spriteTransform || other == null) return false;
            if (!mahjongTile.IsFreeToMatch()) return false;
            MahjongTile otherTile2 = other.GetComponentInParent<MahjongTile>();
            if (!otherTile2) return false;
            if (MBoard.IsTileBeingProcessed(mahjongTile) || MBoard.IsTileBeingProcessed(otherTile2))
            {
                return false;
            }
            bool canMatch = mahjongTile.SpriteCanMatchhWith(otherTile2.MSprite);
            Debug.Log($"[配对调试] {mahjongTile} 和 {otherTile2} SpriteCanMatchhWith结果: {canMatch}");
            return canMatch;
        }

        private void MatchWith(Transform draggable)
        {
            MahjongTile currentTile = GetComponent<MahjongTile>();
            MahjongTile other = draggable.GetComponentInParent<MahjongTile>();
            Debug.Log($"[配对调试] 执行配对: {currentTile} 和 {other}");
            
            // 检查两个麻将牌是否都有效
            if (currentTile == null || other == null)
            {
                Debug.LogError("One or both MahjongTiles are null in MatchWith");
                return;
            }
            
            // 检查麻将牌是否仍然在网格中
            if (currentTile.GetComponentInParent<GridCell>() == null || other.GetComponentInParent<GridCell>() == null)
            {
                Debug.LogError("One or both MahjongTiles are not in GridCell in MatchWith");
                return;
            }
            
            MBoard.CollectMatch(currentTile, other);
        }

        private void HighlightSelected(bool highlight) 
        {
            if (mahjongTile) 
            {
                mahjongTile.SetToFront(highlight);
                mahjongTile.HighlightSelected(highlight);
                Debug.Log("系统正常高亮");
            }
        }

        private void HighlightBothSelected(bool highlight)
        {
            HighlightSelected(highlight);
            if (TouchM.FirstObject)
            {
                MahjongTile mTile = TouchM.FirstObject.GetComponentInParent<MahjongTile>();
                if (mTile)
                {
                    mTile.HighlightSelected(highlight);
                    mTile.SetToFront(highlight);
                }
            }
        }

        private void SetInitialposition()
        {
            if(spriteTransform) spriteTransform.position = spritetransformPosition;
        }

        public bool IsFirstObject()
        {
            return TouchM.FirstObject && TouchM.FirstObject == spriteTransform;
        }

        public void SetAsFirstObject()
        {
            TouchM.SetFirstObject(spriteTransform, (cBack) =>
            {
                SetInitialposition();
                TouchM.CanDrag = false;
                // Debug.Log("reset drag");
            });
            HighlightSelected(true);
            TouchM.CanDrag = true;
        }

        #region constructor
        // 2 3
        // 1 4
        private int GetQuadrant(Bounds bounds, Vector3 touchPos)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector2 size = max - min;
            Vector2 sizeH = size * 0.5f;

            if (touchPos.x < min.x + sizeH.x && touchPos.y < min.y + sizeH.y) return 1;
            else if (touchPos.x < min.x + sizeH.x) return 2;
            else if (touchPos.y < min.y + sizeH.y) return 4;
            else return 3;
        }
        #endregion constructor

        /// <summary>
        /// 取消当前选择并恢复所有麻将未选中状态
        /// </summary>
        public static void ClearCurrentSelectionAndUnselectAllTiles()
        {
            // 1. 清除TouchManager的当前选择
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.SetFirstObject(null, null);
            }
            // 2. 恢复所有麻将未选中
            if (GameBoard.Instance != null && GameBoard.Instance.MainGrid != null)
            {
                var allTiles = GameBoard.Instance.MainGrid.GetTiles();
                if (allTiles != null)
                {
                    foreach (var tile in allTiles)
                    {
                        if (tile != null)
                        {
                            tile.HighlightHint(false);
                            tile.HighlightSelected(false);
                            tile.SetToFront(false);
                        }
                    }
                }
            }
        }
    }
}
