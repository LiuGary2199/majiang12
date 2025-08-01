using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.Events;

namespace Mkey
{
    /// <summary>
    /// 网格对象基类，包含渲染、碰撞、层、父格子等通用属性和方法
    /// </summary>
    public class GridObject : MonoBehaviour
    {
        public SpriteRenderer SRenderer; // 渲染器
         public SpriteRenderer SRMohu; // 渲染器
        public SpriteRenderer Lie; // 渲染器

        public TileAnimController m_TileAnimController;
        public BoxCollider2D boxCollider; // 碰撞体

        #region properties
        public int Layer { get; protected set; } // 层号
        public GridCell ParentCell { get; protected set; } // 父格子
        public bool Excluded { get; protected set; }       // 临时变量，布局时用

        public string Name { get { return name; } } // 名称
        #endregion properties

        #region protected temp vars
        protected GameConstructSet GCSet { get { return GameConstructSet.Instance; } } // 配置集
        protected LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } } // 当前关卡配置
        protected GameObjectsSet GOSet { get { return GCSet.GOSet; } } // 对象集
        protected SoundMaster MSound { get { return SoundMaster.Instance; } } // 声音管理器
        protected GameBoard MBoard { get { return GameBoard.Instance; } } // 游戏主面板
        protected MatchGrid MGrid { get { return MBoard.MainGrid; } } // 主网格


        #endregion protected temp vars

        #region common
        /// <summary>
        /// 设置精灵图片
        /// </summary>
        public void SetSprite(Sprite nSprite)
        {
            m_TileAnimController.stopLoad();
            SRenderer.sprite = nSprite;
            //if (SRenderer)
            // {
            //PlayLoad(()=>{
            //   SRenderer.sprite = nSprite;
            //  });
            // }

        }
           public void PlayLoad()
        {
            SRMohu.sortingOrder =  SRenderer.sortingOrder + 1;
            m_TileAnimController.PlayLoad();
        }

        /// <summary>
        /// 销毁同层竞争对象
        /// </summary>
        public void DestroyHierCompetitor(GridCell gCell, bool andProxy, bool cleanTopLayers)
        {
            if (!gCell) return;
            if (GetSize() == Vector2.one)   // simple object
            {
                GridObject gO = gCell.GetLayerObject(Layer, andProxy, true);
                if (gO) gCell = gO.ParentCell;
                if (gO && gCell)
                {
                    gCell.RemoveObject(gO.Layer, cleanTopLayers);
                }
            }
            else                            // multicells object
            {
                List<GridCell> gridCells = GetOccupiedCells(gCell);
                gridCells.ApplyAction((gC) =>
                {
                    GridObject gOH = gC.GetLayerObject(Layer, andProxy, true);
                    if (gOH)
                    {
                        GridCell cell = gOH.ParentCell;
                        if (cell)
                        {
                            cell.RemoveObject(gOH.Layer, cleanTopLayers);
                        }
                    }
                });
            }
        }
        #endregion common

        #region virtual
        /// <summary>
        /// 虚方法：设置渲染顺序
        /// </summary>
        public virtual void SetToFront(bool set)
        {

        }

        /// <summary>
        /// 虚方法：创建对象
        /// </summary>
        public virtual GridObject Create(GridCell parent, int layer)
        {
            return parent ? Instantiate(this, parent.transform) : Instantiate(this);
        }

        /// <summary>
        /// 虚方法：检查能否按尺寸放置
        /// </summary>
        public virtual bool CanSetBySize(GridCell gCell)
        {
            return true;
        }

        /// <summary>
        /// 虚方法：检查能否按层放置
        /// </summary>
        public virtual bool CanSetByLayer(GridCell gCell, int layer)
        {
            return true;
        }

        /// <summary>
        /// 虚方法：获取占用尺寸
        /// </summary>
        public virtual Vector2Int GetSize()
        {
            return Vector2Int.one;
        }

        /// <summary>
        /// 虚方法：获取占用格子列表（带参数）
        /// </summary>
        public virtual List<GridCell> GetOccupiedCells(GridCell gCell)
        {
            List<GridCell> res = new List<GridCell>();
            res.Add(gCell);
            return res;
        }

        /// <summary>
        /// 虚方法：获取占用格子列表
        /// </summary>
        public virtual List<GridCell> GetOccupiedCells()
        {
            return GetOccupiedCells(ParentCell);
        }
        #endregion virtual

        /// <summary>
        /// 设置层号
        /// </summary>
        public void SetLayer(int layer)
        {
            Layer = layer;
        }
    }

    /// <summary>
    /// 网格对象状态，支持撤销功能
    /// </summary>
    [Serializable]
    public class GridObjectState
    {
        public int layer; // 层号
       
        public GridObjectState(int layer)
        {
            this.layer = layer;
        }

        #region undo
        private Sprite sprite; // 精灵
        /// <summary>
        /// 设置精灵（用于撤销）
        /// </summary>
        public void SetSpite(Sprite sprite) // for undo
        {
            this.sprite = sprite;
        }

        /// <summary>
        /// 获取精灵
        /// </summary>
        public Sprite GetSprite()
        {
            return sprite;
        }

        /// <summary>
        /// 判断状态是否相等
        /// </summary>
        public bool IsEqualTo(GridObjectState other)
        {
            if (other==null) return false;
            if (layer != other.layer) return false;
            if (other.GetSprite() != sprite) return false;
            return true;
        }
        #endregion undo

       
    }
}

