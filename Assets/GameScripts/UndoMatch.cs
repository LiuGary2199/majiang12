using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	/// <summary>
	/// 撤销匹配操作，支持保存和恢复棋盘状态
	/// </summary>
	public class UndoMatch : MonoBehaviour
	{
		[SerializeField]
		private GameObject undoGroup; // gui父节点
		[SerializeField]
		private Text undoCounterText; // 撤销次数文本

		#region temp vars
		private int maxCount = 10000;  // 最大保存步数
		private List<GridCell> checkedCells; // 检查过的格子
		private MatchGrid matchGrid; // 当前网格
		private GameBoard MBoard { get { return GameBoard.Instance; } } // 游戏主面板
		private GameConstructSet GCSet { get { return GameConstructSet.Instance; } } // 配置集
		private GameObjectsSet GOSet { get { return GCSet.GOSet; } } // 对象集
		private List<UndoState> undoStates; // 撤销状态列表
		#endregion temp vars

		#region regular
		private IEnumerator Start()
		{
			if (GameBoard.GMode == GameMode.Play)
			{
				undoStates = new List<UndoState>();
				while (!MBoard) yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				matchGrid = MBoard.MainGrid;
				checkedCells = matchGrid.Cells;
				checkedCells.RemoveAll((c)=> {return c.GetGridObjects(true).Length == 0; }); // remove empty cells
				MBoard.BeforeCollectAction += SaveBeforeMatch;
				MBoard.ShuffleGridBeginAction += CleanUndoStates;
				GameEvents.RestartAction += CleanUndoStates;
				RefreshGUI();
			}
#if UNITY_EDITOR
			else // edit mode
			{
				if (undoGroup) undoGroup.SetActive(false);
				if (undoCounterText) undoCounterText.text = "";
			}
#endif
		}

		private void OnDestroy()
		{
			if (GameBoard.GMode == GameMode.Play)
			{
				if (MBoard) MBoard.BeforeCollectAction -= SaveBeforeMatch;
				if (MBoard) MBoard.ShuffleGridBeginAction -= CleanUndoStates;
				GameEvents.RestartAction -= CleanUndoStates;
			}
		}
        #endregion regular

        #region handlers
        private void SaveBeforeMatch(GridCell gridCell_1 , GridCell gridCell_2, MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
		{
			// 匹配前保存状态
			SaveUndoState(new List<GridCell>() { gridCell_1, gridCell_2 });
		}
		#endregion handlers

		/// <summary>
		/// 保存当前棋盘撤销状态
		/// </summary>
		private void SaveUndoState(List<GridCell> cells)
		{
            if (undoStates.Count > maxCount)
            {
                undoStates.RemoveAt(0);
            }
            UndoState ds = new UndoState(ScoreHolder.Count, cells);
            undoStates.Add(ds);
            // Debug.Log("save undo state " + undoStates.Count);
			RefreshGUI();
        }

		/// <summary>
		/// 恢复最近一次撤销状态
		/// </summary>
		public void RestoreUndoState()
		{
			if (GameBoard.GMode == GameMode.Edit) return;
			if (undoStates == null || undoStates.Count == 0) return;
			UndoState ds = undoStates[undoStates.Count - 1];
			ds.Restore(MBoard.MainGrid, GOSet.mahjongTilePrefab);
			undoStates.RemoveAt(undoStates.Count - 1);
			MBoard.RaiseUndoEvents();
			// Debug.Log("restore undo state " + undoStates.Count);
			RefreshGUI();
			GameEvents.UndoAction?.Invoke();
		}

		private void CleanUndoStates()
        {
			undoStates = new List<UndoState>();
			RefreshGUI();
		}

		private void RefreshGUI()
        {
			if (undoCounterText) undoCounterText.text = (undoStates.Count).ToString();
		}
	}

	/// <summary>
	/// 棋盘撤销状态，保存分数和格子对象信息
	/// </summary>
	public class UndoState
	{
		public int score; // 分数
		public List<GCellObects> cells; // 棋盘格子对象集合
		private GridCell gridCell;

		public UndoState(int score, List<GridCell> checkedCells)
        {
			this.score = score;
			cells = new List<GCellObects>();
			for (int i = 0; i < checkedCells.Count; i++)
			{
				gridCell = checkedCells[i];
				List<GridObjectState> gOSs = GetGridObjectsStates(gridCell);
				cells.Add(new GCellObects(gridCell.Row, gridCell.Column, gOSs));
			}
			// Debug.Log("saved cells: " + cells.Count);
		}

		private List<GridObjectState> GetGridObjectsStates(GridCell gC)
		{
			GridObject[] gridObjects = gC.GetGridObjects(true);
			List<GridObjectState> res = new List<GridObjectState>();
			foreach (var item in gridObjects)
			{
				var objectState = new GridObjectState(item.Layer);
				objectState.SetSpite(item.SRenderer.sprite);
				res.Add(objectState);
			}
			return res;
		}

		public void Restore(MatchGrid matchGrid, GridObject prefab)
        {
			ScoreHolder.Instance.SetCount(score);
            foreach (var objects in cells)
            {
				Debug.Log("restore cell: " + objects.row + " : " + objects.column);
				GridCell gridCell = matchGrid[objects.row, objects.column];
				if(gridCell) RestoreCellState(gridCell, objects, prefab);
            }
		}

		private void RestoreCellState(GridCell gridCell, GCellObects cellObects, GridObject prefab)
        {
            foreach (var item in cellObects.gridObjects)
            {
				GridObject gO = gridCell.GetLayerObject(item.layer, false, true);
				Debug.Log("layer object: " + gO + " ; layer: " + item.layer);
                if (!gO)
                {
					GridObject gridObject = gridCell.SetObject(prefab, item.layer);
					gridObject.SetSprite(item.GetSprite());
					gridObject.SetToFront(false);

				}
			}
        }
	}
}
