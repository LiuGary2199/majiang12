using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// 匹配网格，负责网格的创建、重建、对象分布、层管理等
    /// </summary>
    public class MatchGrid
    {
        private GameObjectsSet goSet; // 游戏对象集
        public List<Column<GridCell>> Columns { get; private set; } // 列集合
        public List<GridCell> Cells { get; private set; } // 所有格子
        public List<Row<GridCell>> Rows { get; private set; } // 行集合
        public Transform Parent { get; private set; } // 父节点
        private GameMode gMode; // 当前模式
        private int vertSize; // 行数
        private int horSize; // 列数
        private GameObject prefab; // 格子预制体
        private float yStart; // Y起始坐标
        private float yStep; // Y步进
        private float xStep; // X步进
        private int yOffset; // Y偏移
        private Vector2 cellSize; // 格子尺寸
        private float cOffset; // X轴偏移

        public LevelConstructSet LcSet { get; private set; } // 当前关卡配置
        private List<MahjongTile> tiles; // 当前所有麻将牌

        #region ctor, create
        /// <summary>
        /// 构造函数，创建网格并初始化格子
        /// </summary>
        public MatchGrid(LevelConstructSet lcSet, GameObjectsSet goSet, Transform parent, GameMode gMode)
        {
            this.LcSet = lcSet;
            this.Parent = parent;
            this.gMode = gMode;
            Debug.Log("new grid " + lcSet.name);

            vertSize = lcSet.VertSize;
            horSize = lcSet.HorSize;
            this.goSet = goSet;
            prefab = goSet.gridCellPrefab;
            cellSize = prefab.GetComponent<BoxCollider2D>().size;

            float deltaX = lcSet.DistX;
            float deltaY = lcSet.DistY;
            SetScale(lcSet.Scale);

            Cells = new List<GridCell>(vertSize * horSize);
            Rows = new List<Row<GridCell>>(vertSize);

            yOffset = 0;
            xStep = (cellSize.x + deltaX);
            yStep = (cellSize.y + deltaY);

            cOffset = (1 - horSize) * xStep / 2.0f; // offset from center by x-axe
            yStart = (vertSize - 1) * yStep / 2.0f;

            //instantiate cells
            for (int i = 0; i < vertSize; i++)
            {
                AddRow();
            }
            InitCells();
            Debug.Log("Created Grid Cells: " + Cells.Count);
            SetObjectsData(lcSet, gMode, -1);   // -1 set all tiles
        }

        /// <summary>
        /// 重建网格，重新分配格子和对象
        /// </summary>
        public void Rebuild(GameObjectsSet mSet, GameMode gMode)
        {
            // 修复：重建前销毁所有旧的麻将牌对象，防止层级错乱
            var oldTiles = Parent.GetComponentsInChildren<MahjongTile>(true);
            foreach (var tile in oldTiles)
            {
                UnityEngine.Object.DestroyImmediate(tile.gameObject);
            }
            Debug.Log("rebuild ");

            this.LcSet = LcSet;
            vertSize = LcSet.VertSize;
            horSize = LcSet.HorSize;

            float deltaX = LcSet.DistX;
            float deltaY = LcSet.DistY;
            SetScale(LcSet.Scale);

            this.goSet = mSet;
            Cells.ForEach((c) => { c.DestroyGridObjects(); });

            List<GridCell> tempCells = Cells;
            Cells = new List<GridCell>(vertSize * horSize + horSize);
            Rows = new List<Row<GridCell>>(vertSize);

            xStep = (cellSize.x + deltaX);
            yStep = (cellSize.y + deltaY);

            cOffset = (1 - horSize) * xStep / 2.0f; // offset from center by x-axe
            yStart = (vertSize - 1) * yStep / 2.0f;

            // create rows 
            GridCell cell;
            Row<GridCell> row;
            int cellCounter = 0;
            int ri = 0;

            for (int i = 0; i < vertSize; i++)
            {
                row = new Row<GridCell>(horSize);

                for (int j = 0; j < row.Length; j++)
                {
                    Vector3 pos = new (j * xStep + cOffset, yStart - i * yStep, 0);

                    if (tempCells != null && cellCounter < tempCells.Count)
                    {
                        cell = tempCells[cellCounter];
                        cell.gameObject.SetActive(true);
                        cell.transform.localPosition = pos;
                        cellCounter++;
                        SpriteRenderer sR = cell.GetComponent<SpriteRenderer>();
                        if (sR)
                        {
                            sR.enabled = true;
                        }
                    }
                    else
                    {
                        cell = UnityEngine.Object.Instantiate(mSet.gridCellPrefab).GetComponent<GridCell>();
                        cell.transform.parent = Parent;
                        cell.transform.localPosition = pos;
                        cell.transform.localScale = Vector3.one;
                    }


                    Cells.Add(cell);
                    row[j] = cell;
                }
                Rows.Add(row);
                ri++;
            }

            // destroy not used cells
            if (cellCounter < tempCells.Count)
            {
                for (int i = cellCounter; i < tempCells.Count; i++)
                {
                    UnityEngine.Object.Destroy(tempCells[i].gameObject);
                }
            }

            // cache columns
            Columns = new List<Column<GridCell>>(horSize);
            Column<GridCell> column;
            for (int c = 0; c < horSize; c++)
            {
                column = new Column<GridCell>(Rows.Count);
                for (int r = 0; r < Rows.Count; r++)
                {
                    column[r] = Rows[r][c];
                }
                Columns.Add(column);
            }

            Debug.Log("objects count: " + Parent.GetComponentsInChildren<MahjongTile>(true).Length);
            InitCells();
            SetObjectsData(LcSet, gMode, -1);   // -1 set all tiles

            Debug.Log("rebuild cells: " + Cells.Count);
            // 修复：重建后重置所有麻将牌的渲染顺序
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
            {
                tile.SetToFront(false);
            }
        }


        /// <summary>
        /// 从关卡配置设置对象数据
        /// </summary>
        internal void SetObjectsData(LevelConstructSet lcSet, GameMode gMode, int countLimit)
        {
            tiles = new List<MahjongTile>();
            if (countLimit < 0) countLimit = int.MaxValue;
            if (lcSet.cells != null)
            {
                bool canSetNextLayer = true;
                for (int layer = 0; layer < GameConstructSet.MaxLayersCount; layer++)
                {
                    if (canSetNextLayer)
                    {
                        canSetNextLayer = false;
                        // Debug.Log("Fill layer #" + layer);
                        int objectsCount = 0;
                        foreach (var c in lcSet.cells)
                        {
                            if (c != null && c.gridObjects != null)
                            {
                                foreach (var o in c.gridObjects)
                                {
                                    if (c.row >= 0 && c.column >= 0 && c.row < Rows.Count && c.column < Rows[c.row].Length && o.layer == layer && tiles.Count < countLimit)
                                    {
                                        GridObject gO = Rows[c.row][c.column].SetObject(goSet.mahjongTilePrefab, layer);
                                        MahjongTile _tile = gO ? (MahjongTile)gO : null;

                                        if (_tile)
                                        {
                                            tiles.Add(_tile);
                                            objectsCount++;
                                            canSetNextLayer = true;
                                        }
                                    }
                                }
                            }
                        }
                        Debug.Log("Layer #" + layer + "; objects count: " + objectsCount);
                    }
                }

                // remove the last odd tile
                if (gMode == GameMode.Play && tiles.Count % 2 != 0)
                {
                    MahjongTile mahjongTile = tiles[tiles.Count - 1];
                    Debug.Log("remove object: " + mahjongTile.ParentCell + "; layer: " + mahjongTile.Layer);
                    mahjongTile.ParentCell.RemoveObject(mahjongTile.Layer, false);
                    tiles.RemoveAt(tiles.Count - 1);
                }

                // cache raw blockers
                if (gMode == GameMode.Play)
                {
                    foreach (var item in tiles)
                    {
                        item.CacheRawBlockers();
                    }
                }
            }
        }

        /// <summary>
        /// 缓存所有麻将牌的阻挡信息
        /// </summary>
        public void CacheBlockers()
        {
            MahjongTile[] mahjongTiles = Parent.GetComponentsInChildren<MahjongTile>(true);
            foreach (var item in mahjongTiles)
            {
                item.CacheRawBlockers();
            }
        }

        /// <summary>
        /// 添加一行格子到网格
        /// </summary>
        private void AddRow()
        {
            GridCell cell;
            Row<GridCell> row = new Row<GridCell>(horSize);
            for (int j = 0; j < row.Length; j++)
            {
                Vector3 pos = new Vector3(j * xStep + cOffset, yStart + yOffset * yStep, 0);
                cell = UnityEngine.Object.Instantiate(goSet.gridCellPrefab).GetComponent<GridCell>();
                cell.transform.parent = Parent;
                cell.transform.localPosition = pos;
                cell.transform.localScale = Vector3.one;
                Cells.Add(cell);
                row[j] = cell;
            }

            Rows.Add(row);

            // cache columns
            Columns = new List<Column<GridCell>>(horSize);
            Column<GridCell> column;
            for (int c = 0; c < horSize; c++)
            {
                column = new Column<GridCell>(Rows.Count);
                for (int r = 0; r < Rows.Count; r++)
                {
                    column[r] = Rows[r][c];
                }
                Columns.Add(column);
            }
            yOffset--;
        }

        public GridCell this[int index0, int index1]
        {
            get {if (ok(index0, index1)) { return Rows[index0][index1]; } else { return null; } }
            set { if (ok(index0, index1)) { Rows[index0][index1] = value; } else { } }
        }

        private bool ok(int index0, int index1)
        {
            return (index0 >= 0 && index0 < vertSize && index1 >= 0 && index1 < horSize);
        }

        private void InitCells()
        {
            // init layer 0
            int layer = 0;
            for (int r = 0; r < Rows.Count; r++)
            {
                for (int c = 0; c < horSize; c++)
                {
                    Rows[r][c].Init(r, c, layer, Columns[c], Rows[r], this, gMode);
                }
            }
        }

        public void SetTofrontAll(bool setTofront)
        {
            MahjongTile[] mahjongTiles = Parent.GetComponentsInChildren<MahjongTile>(true);
            for (int i = 0; i < mahjongTiles.Length; i++)
            {
                mahjongTiles[i].SetToFront(setTofront);
            }
        }
        #endregion ctor, create

        #region  get data from grid
        public void CalcObjects()
        {
            GridObject[] bds = Parent.GetComponentsInChildren<GridObject>(true);
            Debug.Log("Objects count: " + bds.Length);
        }

        public Row<GridCell> GetRow(int row)
        {
            return (row >= 0 && row < Rows.Count) ? Rows[row] : null;
        }

        public Column<GridCell> GetColumn(int col)
        {
            return (col >= 0 && col < Columns.Count) ? Columns[col] : null;
        }

        public List<MahjongTile> GetFreeToMatchTiles()
        {
            List<MahjongTile> result = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            result.RemoveAll((t) => { return !t.IsFreeToMatch(); });
            return result;
        }

        public List<MahjongTile> GetLayerObjects(int layer)
        {
            List<MahjongTile> result = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            result.RemoveAll((t) => { return t.Layer != layer; });
            return result;
        }

        public int GetMaxLayer()
        {
            int maxLayer = 0;
            List<MahjongTile> result = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));

            foreach (var item in result)
            {
                if (item.Layer > maxLayer) maxLayer = item.Layer;
            }
            return maxLayer;

        }

        public MahjongTile [] GetTiles()
        {
            return Parent.GetComponentsInChildren<MahjongTile>(true);
        }
        #endregion  get data from grid

        #region fill sprites
        public void SetMahjongSprites(System.Action onComplete = null)
        {
            #if UNITY_EDITOR
            Debug.Log("SetMahjongSprites 方法被调用，tiles数量: " + (tiles != null ? tiles.Count : -1));
            #endif
            if (tiles == null || tiles.Count == 0) {
                #if UNITY_EDITOR
                Debug.LogError("SetMahjongSprites tiles为空，直接return，回调不会被调用！");
                #endif
                onComplete?.Invoke();
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 已调用（tiles为空分支）");
                #endif
                return;
            }

            // set majong sprites
            var sprites = goSet.GetRandomPairs(tiles.Count / 2, LcSet.fillType);
            var tT = tiles; // 复用tiles本身，避免多次new List
            #if UNITY_EDITOR
            Debug.Log("set sprites, tiles count: " + tiles.Count + "; sprites pairs count: " + sprites.Count + "; " + LcSet.fillType);
            #endif
            // 第一关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 0)
            {
                SetTutorialSprites(onComplete);
                return;
            }
            // 第二关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 1)
            {
                SetMahjongSpritesForLevel2(onComplete);
                return;
            }
            // 1 type - get random from free to fill tiles
            bool failed = false;
            for (int i = 0; i < tT.Count; i += 2)
            {
                List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, false);      // not sorted by layer
                if(freeTiles.Count < 5) freeTiles = GetFreeToFillTiles(tiles, true, true); // avoid last error (tile 0 over tile)
                if(freeTiles.Count == 1)
                {
                    failed = true;
                    #if UNITY_EDITOR
                    Debug.Log("SetMahjongSprites onComplete 即将被调用（freeTiles.Count==1分支）");
                    #endif
                    onComplete?.Invoke();
                    #if UNITY_EDITOR
                    Debug.Log("SetMahjongSprites onComplete 已调用（freeTiles.Count==1分支）");
                    #endif
                    return;
                }
                MahjongTile t1 = freeTiles[0];
                MahjongTile t2 = freeTiles[1];
                freeTiles[0].SetExcluded(true);
                freeTiles[1].SetExcluded(true);
                SpritesPair s = sprites[i / 2];
                t1.SetSprite(s.sprite_1);
                t2.SetSprite(s.sprite_2);
            }
            if (!failed) {
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 即将被调用（1type分支）");
                #endif
                onComplete?.Invoke();
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 已调用（1type分支）");
                #endif
                return;
            }

            // 2 type -  get first from most top layer, second from most bottom layer
            if (failed)
            {
                tiles.ForEach((t) => { t.SetExcluded(false); });
                for (int i = 0; i < tT.Count; i += 2)
                {
                    List<MahjongTile>  freeTiles = GetFreeToFillTiles(tiles, true, true); // reverse sorted
                    if (freeTiles.Count == 1)
                    {
                        failed = true;
                        #if UNITY_EDITOR
                        Debug.Log("SetMahjongSprites onComplete 即将被调用（2type freeTiles.Count==1分支）");
                        #endif
                        onComplete?.Invoke();
                        #if UNITY_EDITOR
                        Debug.Log("SetMahjongSprites onComplete 已调用（2type freeTiles.Count==1分支）");
                        #endif
                        return;
                    }
                    MahjongTile t1 = freeTiles[0];
                    MahjongTile t2 = freeTiles[freeTiles.Count - 1];
                    freeTiles[0].SetExcluded(true);
                    freeTiles[1].SetExcluded(true);
                    SpritesPair s = sprites[i/2];
                    t1.SetSprite(s.sprite_1);
                    t2.SetSprite(s.sprite_2);
                }
            }
            if (!failed) {
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 即将被调用（2type分支）");
                #endif
                onComplete?.Invoke();
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 已调用（2type分支）");
                #endif
                return;
            }

            // 3 type - get 2 tiles from most top layers
            if (failed)
            {
                tiles.ForEach((t)=> { t.SetExcluded(false); });
                failed = false;
                for (int i = 0; i < tT.Count; i += 2)
                {
                    List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, true); // reverse sorted
                    if (freeTiles.Count == 1)
                    {
                        failed = true;
                        #if UNITY_EDITOR
                        Debug.Log("SetMahjongSprites onComplete 即将被调用（3type freeTiles.Count==1分支）");
                        #endif
                        onComplete?.Invoke();
                        #if UNITY_EDITOR
                        Debug.Log("SetMahjongSprites onComplete 已调用（3type freeTiles.Count==1分支）");
                        #endif
                        return;
                    }
                    MahjongTile t1 = freeTiles[0];
                    MahjongTile t2 = freeTiles[1];
                    freeTiles[0].SetExcluded(true);
                    freeTiles[1].SetExcluded(true);
                    SpritesPair s = sprites[i / 2];
                    t1.SetSprite(s.sprite_1);
                    t2.SetSprite(s.sprite_2);
                }
            }
            if (!failed) {
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 即将被调用（3type分支）");
                #endif
                onComplete?.Invoke();
                #if UNITY_EDITOR
                Debug.Log("SetMahjongSprites onComplete 已调用（3type分支）");
                #endif
                return;
            }
            else Debug.LogError("Fill failed, make changes in game board.");
            // 在所有麻将牌设置完毕后调用回调
            #if UNITY_EDITOR
            Debug.Log("SetMahjongSprites onComplete 即将被调用（结尾分支）");
            #endif
            onComplete?.Invoke();
            #if UNITY_EDITOR
            Debug.Log("SetMahjongSprites onComplete 已调用（结尾分支）");
            #endif
            // 修复：设置完麻将牌后重置所有麻将牌的渲染顺序
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
            {
                tile.SetToFront(false);
            }
        }
/*
        public void SetMahjongSprites(System.Action onComplete = null)
        {
            Debug.Log("SetMahjongSprites 优化版被调用，tiles数量: " + (tiles != null ? tiles.Count : -1));
            if (tiles == null || tiles.Count == 0) {
                Debug.LogError("SetMahjongSprites tiles为空，直接return，回调不会被调用！");
                onComplete?.Invoke();
                Debug.Log("SetMahjongSprites onComplete 已调用（tiles为空分支）");
                return;
            }
            // tiles数量必须为偶数
            if (tiles.Count % 2 != 0)
            {
                Debug.LogError($"tiles数量为奇数({tiles.Count})，分配必然失败！");
                onComplete?.Invoke();
                return;
            }
            // 第一关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 0)
            {
                SetTutorialSprites(onComplete);
                return;
            }
            // 第二关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 1)
            {
                SetMahjongSpritesForLevel2(onComplete);
                return;
            }
            // 其它关卡采用高效分配及智能兜底
            List<SpritesPair> spritesOpt = goSet.GetRandomPairs(tiles.Count / 2, LcSet.fillType);
            bool success = false;
            int tryCount = 0;
            while (!success && tryCount < 10)
            {
                tryCount++;
                // 分配前彻底重置
                tiles.ForEach(t => { t.SetExcluded(false); t.SetSprite(null); });
                List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, false);
                bool failed = false;
                for (int i = 0; i < spritesOpt.Count; i++)
                {
                    if (freeTiles.Count < 2) { failed = true; break; }
                    MahjongTile t1 = freeTiles[0];
                    MahjongTile t2 = freeTiles[1];
                    t1.SetSprite(spritesOpt[i].sprite_1);
                    t2.SetSprite(spritesOpt[i].sprite_2);
                    t1.SetExcluded(true);
                    t2.SetExcluded(true);
                    freeTiles.Remove(t1);
                    freeTiles.Remove(t2);
                }
                if (!failed) success = true;
                else
                {
                    // 失败就重排sprites
                    System.Random rng = new System.Random();
                    int n = spritesOpt.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        var value = spritesOpt[k];
                        spritesOpt[k] = spritesOpt[n];
                        spritesOpt[n] = value;
                    }
                }
            }
            if (!success)
            {
                Debug.LogWarning($"分配失败，采用智能兜底分配，tiles.Count={tiles.Count}, spritesOpt.Count={spritesOpt.Count}");
                // 智能兜底分配算法
                int bestMatchCount = -1;
                List<Sprite> bestSprites = null;
                for (int tryIdx = 0; tryIdx < 20; tryIdx++)
                {
                    // 1. 生成所有成对Sprite并打乱
                    List<Sprite> allSprites = new List<Sprite>();
                    for (int i = 0; i < spritesOpt.Count; i++)
                    {
                        allSprites.Add(spritesOpt[i].sprite_1);
                        allSprites.Add(spritesOpt[i].sprite_2);
                    }
                    // 洗牌
                    System.Random rng = new System.Random();
                    int n = allSprites.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        var value = allSprites[k];
                        allSprites[k] = allSprites[n];
                        allSprites[n] = value;
                    }
                    // 2. 依次分配到tiles
                    tiles.ForEach(t => { t.SetExcluded(false); t.SetSprite(null); });
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].SetSprite(allSprites[i]);
                    }
                    // 3. 统计当前分配下的可消对数
                    int matchCount = 0;
                    try
                    {
                        var possibleMatches = new PossibleMatches(tiles.FindAll(t => t != null && t.IsFreeToMatch()));
                        matchCount = possibleMatches.Count;
                    }
                    catch { matchCount = 0; }
                    // 4. 记录最优分配
                    if (matchCount > bestMatchCount)
                    {
                        bestMatchCount = matchCount;
                        bestSprites = new List<Sprite>(allSprites);
                    }
                }
                // 5. 用最优分配方案赋值
                if (bestSprites != null)
                {
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].SetSprite(bestSprites[i]);
                    }
                    Debug.Log($"智能兜底分配完成，最优可消对数：{bestMatchCount}");
                }
                else
                {
                    Debug.LogError("智能兜底分配失败，仍有白图风险！");
                }
            }
            // 分配后检查所有tile
            foreach (var tile in tiles)
            {
                if (tile.SRenderer.sprite == null)
                    Debug.LogError($"分配后有白图，tile: {tile.name}");
            }
            Debug.Log("SetMahjongSprites onComplete 即将被调用（优化分配分支）");
            onComplete?.Invoke();
            Debug.Log("SetMahjongSprites onComplete 已调用（优化分配分支）");
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
                tile.SetToFront(false);
        }*/

        // 协程分帧分配麻将图片，保持高可玩性
        public System.Collections.IEnumerator SetMahjongSpritesAsync(System.Action onComplete, int yieldStep = 1)
        {
            // 第一关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 0)
            {
                SetTutorialSprites(onComplete);
                yield break;
            }
            // 第二关新手引导特殊排列
            if (GameLevelHolder.CurrentLevel == 1)
            {
                SetMahjongSpritesForLevel2(onComplete);
                yield break;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].PlayLoad();
            }
            // 获取图片对，分帧处理
            var sprites = goSet.GetRandomPairs(tiles.Count / 2, LcSet.fillType);
            yield return null; // 分帧：获取图片对后暂停
            yield return null; // 额外分帧：确保完全平滑
            
            var tT = tiles;
            bool failed = false;
            int yieldCounter = 0;
            
            // 1 type - get random from free to fill tiles
            for (int i = 0; i < tT.Count; i += 2)
            {
                // 分帧：每次循环都暂停
                yield return null; // 每次循环都暂停
                
                List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, false);      // not sorted by layer
                if(freeTiles.Count < 5) freeTiles = GetFreeToFillTiles(tiles, true, true); // avoid last error (tile 0 over tile)
                if(freeTiles.Count == 1)
                {
                    failed = true;
                    onComplete?.Invoke();
                    yield break;
                }
                MahjongTile t1 = freeTiles[0];
                MahjongTile t2 = freeTiles[1];
                freeTiles[0].SetExcluded(true);
                yield return null; // 设置排除状态后暂停
                freeTiles[1].SetExcluded(true);
                yield return null; // 设置排除状态后暂停
                SpritesPair s = sprites[i / 2];
                t1.SetSprite(s.sprite_1);
                yield return null; // 设置第一个图片后暂停
                t2.SetSprite(s.sprite_2);
                
                // 平滑优化：设置图片后立即暂停一帧
                yield return null;
                
                yieldCounter++;
                // 每分配一对麻将牌就暂停一帧，减少卡顿
                if (yieldCounter % yieldStep == 0) yield return null;
            }
            if (!failed) {
                onComplete?.Invoke();
                foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
                {
                    tile.SetToFront(false);
                }
                yield break;
            }
            
            // 分帧：策略1失败后暂停
            yield return null;
            yield return null; // 额外分帧
            
            // 2 type -  get first from most top layer, second from most bottom layer
            if (failed)
            {
                tiles.ForEach((t) => { t.SetExcluded(false); });
                for (int i = 0; i < tT.Count; i += 2)
                {
                    // 分帧：每次循环都暂停
                    yield return null; // 每次循环都暂停
                    
                    List<MahjongTile>  freeTiles = GetFreeToFillTiles(tiles, true, true); // reverse sorted
                    if (freeTiles.Count == 1)
                    {
                        failed = true;
                        onComplete?.Invoke();
                        yield break;
                    }
                    MahjongTile t1 = freeTiles[0];
                    MahjongTile t2 = freeTiles[freeTiles.Count - 1];
                    freeTiles[0].SetExcluded(true);
                    yield return null; // 设置排除状态后暂停
                    freeTiles[1].SetExcluded(true);
                    yield return null; // 设置排除状态后暂停
                    SpritesPair s = sprites[i/2];
                    t1.SetSprite(s.sprite_1);
                    yield return null; // 设置第一个图片后暂停
                    t2.SetSprite(s.sprite_2);
                    
                    // 平滑优化：设置图片后立即暂停一帧
                    yield return null;
                    
                    yieldCounter++;
                    // 每分配一对麻将牌就暂停一帧，减少卡顿
                    if (yieldCounter % yieldStep == 0) yield return null;
                }
            }
            if (!failed) {
                onComplete?.Invoke();
                foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
                {
                    tile.SetToFront(false);
                }
                yield break;
            }
            
            // 分帧：策略2失败后暂停
            yield return null;
            yield return null; // 额外分帧
            
            // 3 type - get 2 tiles from most top layers
            if (failed)
            {
                tiles.ForEach((t)=> { t.SetExcluded(false); });
                failed = false;
                for (int i = 0; i < tT.Count; i += 2)
                {
                    // 分帧：每次循环都暂停
                    yield return null; // 每次循环都暂停
                    
                    List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, true); // reverse sorted
                    if (freeTiles.Count == 1)
                    {
                        failed = true;
                        onComplete?.Invoke();
                        yield break;
                    }
                    MahjongTile t1 = freeTiles[0];
                    MahjongTile t2 = freeTiles[1];
                    freeTiles[0].SetExcluded(true);
                    yield return null; // 设置排除状态后暂停
                    freeTiles[1].SetExcluded(true);
                    yield return null; // 设置排除状态后暂停
                    SpritesPair s = sprites[i / 2];
                    t1.SetSprite(s.sprite_1);
                    yield return null; // 设置第一个图片后暂停
                    t2.SetSprite(s.sprite_2);
                    
                    // 平滑优化：设置图片后立即暂停一帧
                    yield return null;
                    
                    yieldCounter++;
                    // 每分配一对麻将牌就暂停一帧，减少卡顿
                    if (yieldCounter % yieldStep == 0) yield return null;
                }
            }
            if (!failed) {
                onComplete?.Invoke();
                foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
                {
                    tile.SetToFront(false);
                }
                yield break;
            }
            
            // 分帧：所有策略失败，开始兜底分配前暂停
            yield return null;
            yield return null; // 额外分帧
            
            // 兜底：分配失败，智能兜底分配
            int bestMatchCount = -1;
            List<Sprite> bestSprites = null;
            for (int tryIdx = 0; tryIdx < 20; tryIdx++)
            {
                // 分帧：每次尝试前暂停
                yield return null; // 每次尝试都暂停
                
                List<Sprite> allSprites = new List<Sprite>();
                for (int i = 0; i < sprites.Count; i++)
                {
                    allSprites.Add(sprites[i].sprite_1);
                    allSprites.Add(sprites[i].sprite_2);
                }
                allSprites.Shuffle();
                tiles.ForEach(t => { t.SetExcluded(false); t.SetSprite(null); });
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].SetSprite(allSprites[i]);
                    // 每分配一个麻将牌就暂停一帧，减少卡顿
                    if (i % 1 == 0) yield return null; // 每个麻将牌都暂停
                }
                int matchCount = 0;
                try
                {
                    var possibleMatches = new PossibleMatches(tiles.FindAll(t => t != null && t.IsFreeToMatch()));
                    matchCount = possibleMatches.Count;
                }
                catch { matchCount = 0; }
                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestSprites = new List<Sprite>(allSprites);
                }
                yield return null;
            }
            if (bestSprites != null)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].SetSprite(bestSprites[i]);
                    // 每分配一个麻将牌就暂停一帧，减少卡顿
                    if (i % 1 == 0) yield return null; // 每个麻将牌都暂停
                }
            }
            foreach (var tile in tiles)
            {
                if (tile.SRenderer.sprite == null)
                    Debug.LogError($"分配后有白图，tile: {tile.name}");
            }
            onComplete?.Invoke();
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
            {
                tile.SetToFront(false);
            }
        }

        /// <summary>
        /// 新手关卡固定图片分配方法
        /// </summary>
        /// <param name="onComplete">完成回调</param>
        private void SetTutorialSprites(System.Action onComplete = null)
        {
            Debug.Log("SetTutorialSprites 开始执行，麻将牌数量: " + tiles.Count);
            
            // 新手引导第一关的特定图片分配
            List<MahjongTile> allTiles = new List<MahjongTile>(tiles);  
            ThemeSpritesHolder themeSpritesHolder = GameThemesHolder.Instance.GetTheme();
            List<Sprite> simpleSprites = new List<Sprite>(themeSpritesHolder.simpleSprites);
            while (simpleSprites.Count < 3) // 只需要3种不同的图片
            {
                simpleSprites.AddRange(themeSpritesHolder.simpleSprites);
            }          
            if (allTiles.Count >= 6)
            {
                // 第1块麻将 (索引0)
                allTiles[0].SetSprite(simpleSprites[0]);
                // 第2块麻将 (索引1)
                allTiles[1].SetSprite(simpleSprites[1]);
                // 第3块麻将 (索引2) - 与第1块同图
                allTiles[2].SetSprite(simpleSprites[0]);
                // 第4块麻将 (索引3) - 与第3块同图
                allTiles[3].SetSprite(simpleSprites[2]);
                // 第5块麻将 (索引4) - 可以自由分配
                if (allTiles.Count > 4)
                {
                    allTiles[4].SetSprite(simpleSprites[2]);
                }
                // 第6块麻将 (索引5) - 与第2块同图
                if (allTiles.Count > 5)
                {
                    allTiles[5].SetSprite(simpleSprites[1]);
                }
                Debug.Log("新手引导第一关特定分配完成");
                Debug.Log($"第1块(索引0)和第3块(索引2)使用图片: {simpleSprites[0].name}");
                Debug.Log($"第2块(索引1)和第6块(索引5)使用图片: {simpleSprites[1].name}");
                Debug.Log($"第3块(索引2)和第4块(索引3)使用图片: {simpleSprites[0].name} 和 {simpleSprites[2].name}");
            }
            else
            {
                // 如果麻将牌数量不足6个，使用简单配对
                int pairNum = allTiles.Count / 2;
                for (int i = 0; i < pairNum; i++)
                {
                    MahjongTile t1 = allTiles[i * 2];
                    MahjongTile t2 = allTiles[i * 2 + 1];
                    Sprite pairSprite = simpleSprites[i % simpleSprites.Count];
                    
                    t1.SetSprite(pairSprite);
                    t2.SetSprite(pairSprite);
                }
                Debug.Log("新手关麻将牌数量不足6个，使用简单配对");
            }
            
            // 奇数张时，最后一张置空
            if (allTiles.Count % 2 != 0)
            {
                var last = allTiles[allTiles.Count - 1];
                last.SetSprite(null);
                Debug.LogWarning("新手关有未配对的麻将牌，已置空最后一张");
            }
            
            // 重置渲染顺序
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
            {
                tile.SetToFront(false);
            }
            
            Debug.Log("SetTutorialSprites 完成");
            onComplete?.Invoke();
        }

        /// <summary>
        /// 第二关特殊图片分配：第1/3/4/10张用groups[0]的四张花色牌，第2/8张用sprites[1]，第5/6张用sprites[2]，第7/9张设为金麻将
        /// </summary>
        private void SetMahjongSpritesForLevel2(System.Action onComplete = null)
        {
            Debug.Log("SetMahjongSpritesForLevel2 开始执行，麻将牌数量: " + tiles.Count);
            List<MahjongTile> allTiles = new List<MahjongTile>(tiles);
            ThemeSpritesHolder themeSpritesHolder = GameThemesHolder.Instance.GetTheme();
            // 获取groups[0]的四张花色牌
            List<Sprite> groupSprites = (themeSpritesHolder.groups != null && themeSpritesHolder.groups.Count > 0 && themeSpritesHolder.groups[0].collection.Count >= 4)
                ? themeSpritesHolder.groups[0].collection
                : null;
            // 获取普通牌
            List<Sprite> simpleSprites = themeSpritesHolder.simpleSprites;
            // 1、3、4、10号（下标0、2、3、9）用花色牌
            int[] groupIdx = { 0, 2, 3, 9 };
            for (int i = 0; i < groupIdx.Length; i++)
            {
                int idx = groupIdx[i];
                if (idx < allTiles.Count && groupSprites != null)
                {
                    allTiles[idx].SetSprite(groupSprites[i]);
                }
            }
            // 重点：第10张麻将（索引9）前置渲染顺序
            // 2、8号（下标1、7）用sprites[1]
            int[] idx2 = { 1, 7 };
            foreach (int idx in idx2)
            {
                if (idx < allTiles.Count && simpleSprites.Count > 1)
                {
                    allTiles[idx].SetSprite(simpleSprites[0]);
                }
            }
            // 5、6号（下标4、5）用sprites[2]
            int[] idx3 = { 4, 5 };
            foreach (int idx in idx3)
            {
                if (idx < allTiles.Count && simpleSprites.Count > 2)
                {
                    allTiles[idx].SetSprite(simpleSprites[1]);
                }
            }
            // 7、9号（下标6、8）设为金麻将
            int[] goldIdx = { 6, 8 };
            foreach (int idx in goldIdx)
            {
                if (idx < allTiles.Count)
                {
                    allTiles[idx].SetGoldState(true);
                }
            }
            // 其他未分配的牌可置空或随机
            for (int i = 0; i < allTiles.Count; i++)
            {
                if (allTiles[i].MSprite == null && !allTiles[i].IsgoldTile)
                {
                    allTiles[i].SetSprite(simpleSprites[0]);
                }
            }
            // 重置渲染顺序
            foreach (var tile in Parent.GetComponentsInChildren<MahjongTile>(true))
            {
                tile.SetToFront(false);
            }
            Debug.Log("SetMahjongSpritesForLevel2 完成");
            onComplete?.Invoke();
        }

        private List<MahjongTile> GetFreeToFillTiles(List<MahjongTile>fromTiles, bool shuffle, bool sortByLayerReverse)
        {
            List<MahjongTile> result = new List<MahjongTile>(fromTiles);
            result.RemoveAll((t) => { return t == null; });
            result.RemoveAll((t) => { return t.Excluded; });
            result.RemoveAll((t) => { return !t.IsFreeToFill(); });
            if (shuffle) result.Shuffle();
            if (sortByLayerReverse) result.Sort((a,b) =>{return b.Layer.CompareTo(a.Layer);});
            return result;
        }
        #endregion fill sprites

        public void DebugDrawGrid()
        {
            Color color = Color.red;
            foreach (var item in Rows)
            {
                Vector3 pos_1 = item[0].transform.position;
                Vector3 pos_2 = item[item.Length-1].transform.position;
                Debug.DrawLine(pos_1, pos_2, color);
            }
        }

        /// <summary>
        /// shuffle tiles in current positions
        /// </summary>
        public void ShuffleGridSprites()
        {
            List<GridCell> gridCells = new List<GridCell>();
            List<MahjongTile> mahjongTiles = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            mahjongTiles.ForEach((t)=> { t.SetExcluded(false);});
            List<SpritesPair> spritesPairs = new List<SpritesPair>();

            while (mahjongTiles.Count > 0)   // get the list of match pairs
            {
                var mTile = mahjongTiles[0];
                mahjongTiles.RemoveAt(0);
                MahjongTile pairTile = null;
                foreach (var item in mahjongTiles)
                {
                    if (mTile.SpriteCanMatchhWith(item.MSprite))
                    {
                        pairTile = item;
                        SpritesPair freePaar = new SpritesPair(mTile.MSprite, item.MSprite);
                        spritesPairs.Add(freePaar);
                        break;
                    }
                }
                mahjongTiles.Remove(pairTile);
            }

            mahjongTiles = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            bool failed = false;
            foreach (var item in spritesPairs)
            {
                List<MahjongTile> freeTiles = GetFreeToFillTiles(mahjongTiles, true, true); // reverse sorted
                if (freeTiles.Count == 1)
                {
                    failed = true;
                    break;
                }
                MahjongTile t1 = freeTiles[0];
                MahjongTile t2 = freeTiles[1];
                freeTiles[0].SetExcluded(true);
                freeTiles[1].SetExcluded(true);
                t1.SetSprite(item.sprite_1);
                t2.SetSprite(item.sprite_2);
            }
            if(failed) Debug.LogError("!!! MIX FAILED !!!");
        }

        public void ReplaceMahjongSprites(ThemeSpritesHolder oldTheme, ThemeSpritesHolder newTheme)
        {
            Dictionary<Sprite, Sprite> res = GameThemesHolder.Instance.GetSpritesDictionary(oldTheme, newTheme);
            MahjongTile [] tT = Parent.GetComponentsInChildren<MahjongTile>();
            foreach (var item in tT)
            {
                item.SetSprite(res[item.MSprite]);
            }
        }

        public void SetScale(float scale)
        {
            Parent.localScale = new Vector3(scale, scale, scale);
        }


        /// <summary>
        /// Is it possible to shuffle tiles in current positions?
        /// </summary>
        /// <returns></returns>
        public bool CanShuffle()
        {
            List<MahjongTile> mahjongTiles = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            mahjongTiles.ForEach((t) => { t.SetExcluded(false); });
            List<MahjongTile> freeTiles;
            for (int i = 0; i < mahjongTiles.Count; i += 2)
            {
                freeTiles = GetFreeToFillTiles(mahjongTiles, false, false);
                if (freeTiles.Count == 1)
                {
                    return false;
                }
                freeTiles[0].SetExcluded(true);
                freeTiles[1].SetExcluded(true);
            }
            return true;
        }

        /// <summary>
        /// shuffle tiles into new positions
        /// </summary>
        public void HardShuffle()
        {
            List<MahjongTile> mahjongTiles = new List<MahjongTile>(Parent.GetComponentsInChildren<MahjongTile>(true));
            mahjongTiles.ForEach((t) => { t.SetExcluded(false); });
            List<SpritesPair> spritesPairs = new List<SpritesPair>();

            while (mahjongTiles.Count > 0)   // get the list of match pairs
            {
                var mTile = mahjongTiles[0];
                mahjongTiles.RemoveAt(0);
                MahjongTile pairTile = null;
                foreach (var item in mahjongTiles)
                {
                    if (mTile.SpriteCanMatchhWith(item.MSprite))
                    {
                        pairTile = item;
                        SpritesPair freePaar = new SpritesPair(mTile.MSprite, item.MSprite);
                        spritesPairs.Add(freePaar);
                        break;
                    }
                }
                mahjongTiles.Remove(pairTile);
            }
            Cells.ForEach((c) => { c.DestroyGridObjects(); });
            int tilesCount = spritesPairs.Count * 2;
            SetObjectsData(LcSet, GameMode.Play, tilesCount);
            tiles.ForEach((t) => { t.SetExcluded(false); });
            List<MahjongTile> tT = new List<MahjongTile>(tiles);

            bool failed = false;
            for (int i = 0; i < tT.Count; i += 2)
            {
                List<MahjongTile> freeTiles = GetFreeToFillTiles(tiles, true, true); // reverse sorted
                if (freeTiles.Count == 1)
                {
                    failed = true;
                    break;
                }
                MahjongTile t1 = freeTiles[0];
                MahjongTile t2 = freeTiles[1];
                freeTiles[0].SetExcluded(true);
                freeTiles[1].SetExcluded(true);
                SpritesPair s = spritesPairs[i / 2];
                t1.SetSprite(s.sprite_1);
                t2.SetSprite(s.sprite_2);
            }
            if (failed) Debug.LogError("!!!HARD MIX FAILED!!!");
        }
    }
}