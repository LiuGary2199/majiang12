using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Lofelt.NiceVibrations;
using DG.Tweening;
using System.Runtime.CompilerServices;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mkey
{
    /// <summary>
    /// 游戏主面板控制类，负责管理游戏主流程、网格、分数、事件等
    /// </summary>
    public class GameBoard : MonoBehaviour
    {
        #region settings 
        [Space(8)]
        [Header("Game settings")]
        public bool showScore; // 是否显示分数
        public bool enableContinuousMatch = true; // 是否启用连续消除
        public bool skipAnimation = false; // 是否跳过动画
        #endregion settings

        [Header("Collect properties")]
        public float speed = 6; // 收集动画速度
        public EaseAnim ease_0;     // outsine
        public EaseAnim ease_1;      // outbounce
        public AudioClip collectSound; // 收集音效

        #region references
        [Header("Main references")]
        [Space(8)]
        public Transform GridContainer; // 网格容器
        public SpriteRenderer backGround; // 背景渲染器
        public GameConstructor gConstructor; // 关卡构造器
        [SerializeField]
        private ScoreController scoreController; // 分数控制器
        //[SerializeField]
        //private PopUpsController winPrefab; // 胜利弹窗预制体
        [SerializeField]
        private PopUpsController noMatchesPrefab; // 无可消除弹窗预制体
        [SerializeField]
        private GameObject collectPrefab; // 收集物体预制体
        [SerializeField]
        private GUIFlyer scoreFlyerPrefab; // 分数飞行动画预制体
        #endregion references

        #region grid
        public MatchGrid MainGrid { get; private set; } // 主网格对象
        #endregion grid

        #region states
        public static GameMode GMode = GameMode.Play; // 当前游戏模式：Play或Edit
        #endregion states

        #region properties
        public Sprite BackGround
        {
            get { return backGround.sprite; }
            set { if (backGround) backGround.sprite = value; }
        } // 背景图片属性

        private SoundMaster MSound { get { return SoundMaster.Instance; } } // 声音管理器单例

        public GuiController MGui => GuiController.Instance; // GUI控制器单例
        #endregion properties

        #region sets
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } } // 游戏构造配置单例
        private LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } } // 当前关卡配置
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } } // 游戏对象集合
        private GameLevelHolder MGLevel => GameLevelHolder.Instance; // 关卡进度管理器
        #endregion sets

        #region events
        public Action WinAction; // 胜利事件
        public Action NoMatchesAction; // 无可消除事件
        public Action<int> ChangePossibleMatchesAction; // 可消除对数变化事件
        public Action<GridCell, GridCell, MahjongTile, MahjongTile> BeforeCollectAction; // 收集前事件
        public Action<Sprite, Sprite> CollectAction; // 收集事件
        public Action EndCollectAnimatioAction; // 收集动画结束事件
        public Action FailedMatchAction; // 匹配失败事件
        public Action ShuffleGridEndAction; // 洗牌结束事件
        public Action ShuffleGridBeginAction; // 洗牌开始事件
        public Action UndoEndAction; // 撤销结束事件
        public Action<bool> ChangeFreeHiglightModeAction; // 自由高亮模式变化事件
        #endregion events

        public static GameBoard Instance { get; private set; } // 单例实例
        // 预加载系统相关
        private class PreloadedLevelData
        {
            public MatchGrid grid;                    // 预创建的网格
            public List<Sprite> spriteAssignments;    // 预计算的图片分配
            public int level;                         // 关卡号
            public bool isComplete;                   // 是否完成预加载
        }
        private PreloadedLevelData preloadedData;
        private bool hasClickedMahjong = false;       // 是否已经点击过麻将
        private Transform preloadContainer;            // 预加载容器


        // 防止WinAction重复触发
        private bool hasWinActionInvoked = false;
        private bool popGold = false;


        #region regular
        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
#if UNITY_EDITOR
            if (GCSet && GCSet.testMode) GameLevelHolder.CurrentLevel = Mathf.Abs(GCSet.testLevel);
#endif
            //      ScoreHolder.Instance.SetCount(0);
        }

        private void Start()
        {
            GameThemesHolder.Instance.SetIndex(0); // 主题选择

            Debug.Log("GameBoard Start 被调用");

                 
            // 初始化预加载容器
            InitializePreloadContainer();
            
            // 监听麻将点击事件
            SharplyResort.AxeCarIncoming(CFellow.mg_OnMahjongClick, OnMahjongClick);
            #region game sets 
            if (!GCSet)
            {
                Debug.Log("Game construct set not found!!!");
                return;
            }

            if (!LCSet)
            {
                Debug.Log("Level construct set not found!!! - " + GameLevelHolder.CurrentLevel);
                return;
            }

            if (!GOSet)
            {
                Debug.Log("MatcSet not found!!! - " + GameLevelHolder.CurrentLevel);
                return;
            }
            #endregion game sets 

            DestroyGrid();
            CreateGameBoard();
            GameLevelHolder.StartLevel();

            if (GMode == GameMode.Edit)
            {
#if UNITY_EDITOR
                Debug.Log("start edit mode");
                if (gConstructor)
                {
                    gConstructor.gameObject.SetActive(true);
                    gConstructor.InitStart();
                }
#endif
            }

            else if (GMode == GameMode.Play)
            {
                Debug.Log("start play mode");
                if (gConstructor) DestroyImmediate(gConstructor.gameObject);
                ScoreHolder.Instance.SetAverageScore(scoreController.GetMaxLevelScore(MainGrid.GetTiles().Length / 2));
                Debug.Log("max level score: " + ScoreHolder.AverageScore);
                #region set board eventhandlers
                UndoEndAction += () =>
                {
                    MainGrid.CacheBlockers();
                    UpdatePossibleMatches();
                    if (!CheckExistingHint()) RemoveHint();
                    if (IsHihglightFreeMode)
                    {
                        HighlihtFree(true);
                    }
                };

                ShuffleGridEndAction += () =>
                {
                    UpdatePossibleMatches();
                };

                BeforeCollectAction += (c1, c2, m1, m2) =>
                {
                    if (hintPair != null && hintPair.ContaiAny(m1, m2)) RemoveHint(); // remove hint
                };

                CollectAction += (s1, s2) =>
                {
                    ScoreHolder.Add(scoreController.GetMatchScore());
                    if (MainGrid.GetTiles().Length == 0)
                    {
                        if (!hasWinActionInvoked)
                        {
                            hasWinActionInvoked = true;
                            WinAction?.Invoke();
                        }
                        return;
                    }
                    UpdatePossibleMatches();
                    if (possibleMatches.Count == 0) NoMatchesAction?.Invoke();
                    GameEvents.MatchSpritesEvent?.Invoke(s1, s2);
                    if (IsHihglightFreeMode)
                    {
                        HighlihtFree(true);
                    }
                    TryAutoCollect(); // 自动收集检测
                };

                NoMatchesAction += () =>
                {
                    if (!isInAutoCollect)
                    {
                        UIAnalyze.GetInstance().BeadUIFlank(nameof(BegBound));
                    }

                    //   MGui.ShowPopUp(noMatchesPrefab);    // show no matches popup
                };

                WinAction += () =>
                {
                    Debug.Log("完成关卡");
                    UIAnalyze.GetInstance().BeadUIFlank(nameof(TenthTimidityStore));
                    //  MGui.ShowPopUp(winPrefab);  // show win message
                    MGLevel.PassLevel();        // pass level
                    GameEvents.WinLevelAction?.Invoke();
                };

                ChangeFreeHiglightModeAction += (highlight) =>
                {
                    HighlihtFree(highlight);
                };
                HighlihtFree(true);
                #endregion set board eventhandlers
                MainGrid.CalcObjects();

                UpdatePossibleMatches();

                LoadHighlightMode();
            }
        }
        #endregion regular

        #region grid construct restart
        public void CreateGameBoard()
        {
            // 关卡重建时重置自动收集和通关标志
            isAutoCollecting = false;
            hasTriggeredAutoCollect = false;
            isInAutoCollect = false;
            hasWinActionInvoked = false;
            goldComboCount = 0;
            hasClickedMahjong = false; // 重置点击麻将标志
            MGLevel.Load();
            Debug.Log("Create gameboard ");
            Debug.Log("level set: " + LCSet.name);
            Debug.Log("current level: " + GameLevelHolder.CurrentLevel);

             // 检查是否有预加载数据可用
            Debug.Log($"=== 检查预加载数据 ===");
            Debug.Log($"preloadedData: {(preloadedData != null ? "存在" : "null")}");
            if (preloadedData != null)
            {
                Debug.Log($"preloadedData.isComplete: {preloadedData.isComplete}");
                Debug.Log($"preloadedData.level: {preloadedData.level}");
                Debug.Log($"GameLevelHolder.CurrentLevel: {GameLevelHolder.CurrentLevel}");
                Debug.Log($"preloadedData.spriteAssignments.Count: {preloadedData.spriteAssignments?.Count ?? 0}");
            }
            
            bool hasPreloadedData = preloadedData != null && preloadedData.isComplete && preloadedData.level == GameLevelHolder.CurrentLevel;
            Debug.Log($"hasPreloadedData: {hasPreloadedData}");
            
            if (hasPreloadedData)
            {
                Debug.Log("使用预加载数据快速切换关卡");
                // 先正常重建网格结构
                BackGround = GOSet.GetBackGround(LCSet.BackGround);
                
                if (GMode == GameMode.Play)
                {
                    Func<LevelConstructSet, Transform, MatchGrid> create = (lC, container) =>
                    {
                        MatchGrid g = new MatchGrid(lC, GOSet, container, GMode);
                        g.Cells.ForEach((c) =>
                        {
#if UNITY_EDITOR
                            c.name = c.ToString();
#endif
                        });
                        return g;
                    };

                    MainGrid = create(LCSet, GridContainer);
                }
                
                MainGrid.SetTofrontAll(false);
                
                // 然后应用预加载的图片分配
                ApplyPreloadedSpriteAssignments();
                TestCallback();
                

                 // 从第三关开始，在图片分配完成后播放入场动画
                if (GameLevelHolder.CurrentLevel >= 2)
                {
                    StartCoroutine(PlayLevelLoadAnimation());
                }
                // 清理预加载数据
                CleanupPreloadedData();
                return; // 使用预加载数据后直接返回，不执行后续的创建逻辑
            }

            // 触发关卡加载事件来更新UI
            MGLevel.LoadEvent?.Invoke(GameLevelHolder.CurrentLevel);

            BackGround = GOSet.GetBackGround(LCSet.BackGround);

            if (GMode == GameMode.Play)
            {
                Func<LevelConstructSet, Transform, MatchGrid> create = (lC, container) =>
                {
                    MatchGrid g = new MatchGrid(lC, GOSet, container, GMode);
                    g.Cells.ForEach((c) =>
                    {
#if UNITY_EDITOR
                        c.name = c.ToString();
#endif
                    });
                    return g;
                };

                MainGrid = create(LCSet, GridContainer);
            }
            else // edit mode
            {
#if UNITY_EDITOR

                if (MainGrid != null && MainGrid.LcSet == LCSet)
                {
                    MainGrid.Rebuild(GOSet, GMode);
                }
                else
                {
                    DestroyGrid();
                    MainGrid = new MatchGrid(LCSet, GOSet, GridContainer, GMode);
                }

                // set cells delegates for constructor
                for (int i = 0; i < MainGrid.Cells.Count; i++)
                {
                    MainGrid.Cells[i].GetComponent<Collider2D>().enabled = true;
                    MainGrid.Cells[i].GCPointerDownEvent = (c) =>
                     {
                         gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                     };
                }
#endif
            }

            MainGrid.SetTofrontAll(false);
            var swSprites = System.Diagnostics.Stopwatch.StartNew();
            StartCoroutine(MainGrid.SetMahjongSpritesAsync(() =>
            {
                swSprites.Stop();
                Debug.Log($"[耗时] SetMahjongSprites: {swSprites.ElapsedMilliseconds} ms");
                TestCallback();
            }, 1)); // 使用yieldStep=1，每分配一对麻将牌就暂停一帧，最大平滑度，激进分帧
            var sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Stop();
            Debug.Log($"[耗时] CreateGameBoard: {sw.ElapsedMilliseconds} ms");
        }

        public void RestartLevel()
        {
            // 关卡重建时重置自动收集和通关标志
            isAutoCollecting = false;
            hasTriggeredAutoCollect = false;
            isInAutoCollect = false;
            hasWinActionInvoked = false;
            goldComboCount = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            MainGrid.Rebuild(GOSet, GMode);
            sw.Stop();
            Debug.Log($"[耗时] MainGrid.Rebuild: {sw.ElapsedMilliseconds} ms");
            var swSprites = System.Diagnostics.Stopwatch.StartNew();
            StartCoroutine(MainGrid.SetMahjongSpritesAsync(() =>
            {
                swSprites.Stop();
                Debug.Log($"[耗时] SetMahjongSprites: {swSprites.ElapsedMilliseconds} ms");
                TestCallback();
            }, 1)); // 使用yieldStep=1，每分配一对麻将牌就暂停一帧
        }

        private static void TestCallback()
        {
            Debug.Log("TestCallback 被调用，准备刷新遮灰");
            if (GameBoard.Instance != null)
            {
                if (GameBoard.Instance.MainGrid != null)
                {
                    GameBoard.Instance.MainGrid.CacheBlockers();
                    Debug.Log($"TestCallback: CacheBlockers 已执行，Tiles数量: {GameBoard.Instance.MainGrid.GetTiles().Length}");
                    GameBoard.Instance.HighlihtFree(true);
                    Debug.Log("TestCallback: HighlihtFree(true) 已执行，遮灰刷新已完成");

                    // 触发关卡加载完成事件
                    GameEvents.LevelLoadCompleteAction?.Invoke();
                    Debug.Log("关卡加载完成事件已触发");
                }
                else
                {
                    Debug.LogError("TestCallback: MainGrid 为空！");
                }
            }
            else
            {
                Debug.LogError("TestCallback: GameBoard.Instance 为空！");
            }
        }

        /// <summary>
        /// destroy default main grid cells
        /// </summary>
        public void DestroyGrid()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            GridCell[] gcs = gameObject.GetComponentsInChildren<GridCell>();
            for (int i = 0; i < gcs.Length; i++)
            {
                DestroyImmediate(gcs[i].gameObject);
            }
            sw.Stop();
            Debug.Log($"[耗时] DestroyGrid: {sw.ElapsedMilliseconds} ms");
        }
        #endregion grid construct restart

        #region states
        public void ShuffleGrid(Action completeCallBack)
        {
            if (!MainGrid.CanShuffle())
            {
                ShuffleGridBeginAction?.Invoke();
                MainGrid.HardShuffle();
                MainGrid.SetTofrontAll(false);
                hintPair = null;
                possibleMatches = null;
                if (IsHihglightFreeMode)
                {
                    HighlihtFree(true);
                }
                ShuffleGridEndAction?.Invoke();
                completeCallBack?.Invoke();
                return;
            }
            // standart shuffle action
            SetControlActivity(false, false);
            ShuffleGridBeginAction?.Invoke();
            ParallelTween pT0 = new();
            ParallelTween pT1 = new();
            hintPair = null;
            possibleMatches = null;

            TweenSeq tweenSeq = new();
            List<MahjongTile> mahjongTiles = GetComponentsInChildren<MahjongTile>(true).ToList();

            mahjongTiles.ForEach((mT) => { pT0.Add((callBack) => { mT.MixJump(transform.position, callBack); }); });

            mahjongTiles.ForEach((mT) => { pT1.Add((callBack) => { mT.ReversMixJump(callBack); }); });

            tweenSeq.Add((callBack) =>
            {
                pT0.Start(callBack);
            });

            tweenSeq.Add((callBack) =>
            {
                MainGrid.ShuffleGridSprites();
                pT1.Start(() =>
                {
                    SetControlActivity(true, true);
                    ShuffleGridEndAction?.Invoke();
                    completeCallBack?.Invoke();
                    callBack();
                });
            });

            tweenSeq.Start();
        }

        internal void SetControlActivity(bool activityGrid, bool activityMenu)
        {
            TouchManager.SetTouchActivity(activityGrid);
            //  HeaderGUIController.Instance.SetControlActivity(activityMenu);
            FooterGUIController.Instance.SetControlActivity(activityMenu);
        }
        #endregion states

        #region collect match
        PossibleMatches possibleMatches;
        private int pairNumber = 0;
        private Canvas parentCanvas;
        private Queue<MatchPair> matchQueue = new Queue<MatchPair>(); // 连续消除队列
        private bool isProcessingMatch = false; // 是否正在处理消除
        public HashSet<MahjongTile> processingTiles = new HashSet<MahjongTile>(); // 正在处理的麻将牌
        public int GetPossibleMatchesCount()
        {
            return (possibleMatches != null) ? possibleMatches.Count : 0;
        }

        /// <summary>
        /// 检查麻将牌是否正在被处理
        /// </summary>
        public bool IsTileBeingProcessed(MahjongTile tile)
        {
            return processingTiles.Contains(tile);
        }

        public void CollectMatch(MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
        {
            // 严格的空值检查
            if (mahjongTile_1 == null || mahjongTile_2 == null)
            {
                Debug.LogError("MahjongTile is null in CollectMatch");
                return;
            }

            // 检查麻将牌是否仍然有效（没有被销毁）
            if (mahjongTile_1.gameObject == null || mahjongTile_2.gameObject == null)
            {
                Debug.LogError("MahjongTile GameObject is null in CollectMatch");
                return;
            }

            // 检查麻将牌是否正在被处理
            if (processingTiles.Contains(mahjongTile_1) || processingTiles.Contains(mahjongTile_2))
            {
                Debug.LogWarning("MahjongTile is already being processed");
                return;
            }

            // 检查麻将牌是否仍然在网格中
            GridCell gridCell_1 = mahjongTile_1.GetComponentInParent<GridCell>();
            GridCell gridCell_2 = mahjongTile_2.GetComponentInParent<GridCell>();

            if (gridCell_1 == null || gridCell_2 == null)
            {
                Debug.LogError("GridCell is null in CollectMatch - tiles may have been destroyed or moved");
                return;
            }

            // 添加到处理中集合
            processingTiles.Add(mahjongTile_1);
            processingTiles.Add(mahjongTile_2);

            if (skipAnimation)
            {
                FastCollect(mahjongTile_1, mahjongTile_2);
            }
            else
            {
                StartCoroutine(CollectMatchC(mahjongTile_1, mahjongTile_2));
            }
        }

        private bool hasTriggeredGoldRewardInAutoCollect = false;

        private IEnumerator AutoCollectAllCoroutine()
        {
            float originalSpeed = speed;
            speed = originalSpeed / 2f; // 动画加快一倍
            hasTriggeredGoldRewardInAutoCollect = false; // 自动收集前重置
            while (MainGrid.GetTiles().Length > 0)
            {
                UpdatePossibleMatches();
                if (possibleMatches.Count > 0)
                {
                    // 优先选择层级最高的一对（模拟玩家手动点选）
                    MatchPair topPair = null;
                    int maxLayer = int.MinValue;
                    for (int i = 0; i < possibleMatches.Count; i++)
                    {
                        var pair = possibleMatches.GetMatchPair(i);
                        if (pair != null && pair.mahjongTile_1 && pair.mahjongTile_2)
                        {
                            int layer = Mathf.Max(pair.mahjongTile_1.Layer, pair.mahjongTile_2.Layer);
                            if (layer > maxLayer)
                            {
                                maxLayer = layer;
                                topPair = pair;
                            }
                        }
                    }
                    if (topPair != null)
                    {
                        CollectMatch(topPair.mahjongTile_1, topPair.mahjongTile_2);
                        yield return new WaitUntil(() => !isProcessingMatch);
                        yield return new WaitForSeconds(0.05f);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // 没有可消除对，强制找一对Sprite相同的牌
                    var allTiles = MainGrid.GetTiles();
                    bool found = false;
                    for (int i = 0; i < allTiles.Length; i++)
                    {
                        for (int j = i + 1; j < allTiles.Length; j++)
                        {
                            if (allTiles[i] && allTiles[j] && allTiles[i].SpriteCanMatchhWith(allTiles[j].MSprite))
                            {
                                CollectMatch(allTiles[i], allTiles[j]);
                                found = true;
                                yield return new WaitUntil(() => !isProcessingMatch);
                                yield return new WaitForSeconds(0.05f);
                                break;
                            }
                        }
                        if (found) break;
                    }
                    if (!found)
                    {
                        // 真的无解，弹出提示
                        break;
                    }
                }
            }
            speed = originalSpeed;
            isAutoCollecting = false;
            isInAutoCollect = false;
        }

        private IEnumerator CollectMatchC(MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
        {
            // 空值检查
            if (mahjongTile_1 == null || mahjongTile_2 == null)
            {
                Debug.LogError("MahjongTile is null in CollectMatchC");
                yield break;
            }

            // 只禁用菜单控制，保持网格可交互
            SetControlActivity(true, false);
            GridCell gridCell_1 = mahjongTile_1.GetComponentInParent<GridCell>();
            GridCell gridCell_2 = mahjongTile_2.GetComponentInParent<GridCell>();

            // 检查GridCell是否为空
            if (gridCell_1 == null || gridCell_2 == null)
            {
                Debug.LogError("GridCell is null in CollectMatchC");
                SetControlActivity(true, true);
                yield break;
            }

            BeforeCollectAction?.Invoke(gridCell_1, gridCell_2, mahjongTile_1, mahjongTile_2);
            Sprite sprite_1 = mahjongTile_1.MSprite;
            Sprite sprite_2 = mahjongTile_2.MSprite;

            // 安全地调用UnLinkObject
            try
            {
                if (mahjongTile_1.Layer != null)
                    gridCell_1.UnLinkObject(mahjongTile_1.Layer);
                if (mahjongTile_2.Layer != null)
                    gridCell_2.UnLinkObject(mahjongTile_2.Layer);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in UnLinkObject: {e.Message}");
            }

            yield return CollectAnimationC(mahjongTile_1, mahjongTile_2);
            EndCollectAnimatioAction?.Invoke();

            // 安全地销毁对象
            if (mahjongTile_1 != null && mahjongTile_1.gameObject != null)
                Destroy(mahjongTile_1.gameObject);
            if (mahjongTile_2 != null && mahjongTile_2.gameObject != null)
                Destroy(mahjongTile_2.gameObject);

            yield return new WaitForEndOfFrame();
            CollectAction?.Invoke(sprite_1, sprite_2);

            // 从处理中集合移除
            processingTiles.Remove(mahjongTile_1);
            processingTiles.Remove(mahjongTile_2);

            SetControlActivity(true, true);
        }

        private IEnumerator CollectAnimationC(MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
        {
            MahjongTile leftTile = (mahjongTile_1.spriteTransform.position.x < mahjongTile_2.spriteTransform.position.x) ? mahjongTile_1 : mahjongTile_2;
            MahjongTile rightTile = (leftTile == mahjongTile_1) ? mahjongTile_2 : mahjongTile_1;
            Bounds bounds_1 = leftTile.boxCollider.bounds;
            Vector3 min = bounds_1.min;
            Vector3 max = bounds_1.max;
            Vector2 size = max - min;
            Vector2 size05 = size * 0.5f;
            Vector2 size15 = size * 1.5f;
            Vector2 size01 = size * 0.1f;
            Vector3 wPos_10 = leftTile.spriteTransform.position;
            Vector3 wPos_11 = rightTile.spriteTransform.position;
            Vector3 wPos_center = (wPos_10 + wPos_11) * 0.5f;
            if (Mathf.Abs(wPos_center.x) > 1.5f * size.x)
            {
                wPos_center = new Vector3(wPos_center.x > 0 ? 1.5f : -1.5f, wPos_center.y, wPos_center.z); // offset to center
            }
            Vector3 wPos_20 = wPos_center - new Vector3(size.x, 0, 0);
            Vector3 wPos_21 = wPos_center + new Vector3(size.x, 0, 0);

            Vector3 wPos_30 = wPos_20 - new Vector3(size15.x, 0, 0);
            Vector3 wPos_31 = wPos_21 + new Vector3(size15.x, 0, 0);

            Vector3 wPos_40 = wPos_center - new Vector3(size05.x + size01.x, 0, 0);
            Vector3 wPos_41 = wPos_center + new Vector3(size05.x + size01.x, 0, 0);
            bool moveComplete = false;
            bool finishanim = true;

            float time = (wPos_30 - wPos_10).magnitude / speed;
            if (time < 0.2f) time = 0.2f;
            if (time > 0.4f) time = 0.4f;
            moveComplete = false;
            SimpleTween.Move(leftTile.spriteTransform.gameObject, wPos_10, wPos_30, time).SetEase(ease_0);
            SimpleTween.Move(rightTile.spriteTransform.gameObject, wPos_11, wPos_31, time).SetEase(ease_0).AddCompleteCallBack(() => {
                moveComplete = true;
                if (leftTile.IsgoldTile)
                {
                    leftTile.PlayGbroke(() => { });
                    rightTile.PlayGbroke(() =>
                    {
                       
                    });
                    DOVirtual.DelayedCall(0.3f, () =>  //停顿
                    {
                        finishanim = false;
                        leftTile.gameObject.SetActive(false);
                        rightTile.gameObject.SetActive(false);
                    });
                }
                else
                {
                    leftTile.PlayNbroke(() => { });
                    rightTile.PlayNbroke(() =>
                    {
                      
                    });
                    DOVirtual.DelayedCall(0.3f, () =>  //停顿
                    {
                        finishanim = false;
                         leftTile.gameObject.SetActive(false);
                        rightTile.gameObject.SetActive(false);
                    });
                }
            });

            yield return new WaitWhile(() => !moveComplete);

            time = (wPos_40 - wPos_30).magnitude / speed;
            moveComplete = false;
            SimpleTween.Move(leftTile.spriteTransform.gameObject, wPos_30, wPos_40, time).SetEase(ease_1);
            SimpleTween.Move(rightTile.spriteTransform.gameObject, wPos_31, wPos_41, time).SetEase(ease_1).AddCompleteCallBack(() => {
                moveComplete = true;
            });
            popGold = false;
            TweenExt.DelayAction(rightTile.spriteTransform.gameObject, time * 0.9f, () =>
            {
                if (GameLevelHolder.CurrentLevel >= 2)
                {
                    TryGoldRewardOnCombo();
                }
                KeyValuesUpdate keyfly = new KeyValuesUpdate(CFellow.Me_OfClumpCheese, wPos_center);
                SharplyResort.ArabSharply(CFellow.Me_OfClumpCheese, keyfly);
                double rewardnum = 0;
                if (leftTile.IsgoldTile)
                {
                    if (isInAutoCollect)
                    {
                        if (!hasTriggeredGoldRewardInAutoCollect)
                        {
                            double Goldreward = GameUtil.GetGoldMatch();
                            UIAnalyze.GetInstance().BeadUIFlank(nameof(RadiumLysStore), Goldreward);
                            hasTriggeredGoldRewardInAutoCollect = true;
                        }
                    }
                    else
                    {
                        popGold = true;
                        // 手动消除，每对都弹
                        double Goldreward = GameUtil.GetGoldMatch();
                        DOVirtual.DelayedCall(0.5f, () =>  //停顿
                        {
                            UIAnalyze.GetInstance().BeadUIFlank(nameof(RadiumLysStore), Goldreward);
                        });
                    }
                }
                else
                {
                    rewardnum = GameUtil.GetNormalMatch();
                    if (FadeAnalyze.Instance.m_UpArctic)
                    {
                        rewardnum = rewardnum * MobTownEre.instance.FadeLoom.combommul;
                    }
                    addScoreData scordData = new addScoreData();
                    scordData.ClumpImage = rewardnum;
                    scordData.Demise3Not = wPos_center;
                    KeyValuesUpdate addScore = new KeyValuesUpdate(CFellow.Me_OfAxeFavor, scordData);
                    SharplyResort.ArabSharply(CFellow.Me_OfAxeFavor, addScore);
                }

                if (collectPrefab) Instantiate(collectPrefab, wPos_center, Quaternion.identity, transform);
                if (showScore) InstantiateScoreFlyer(wPos_center + new Vector3(0, size.y, 0), "+" + scoreController.GetMatchScore().ToString());

            });
            if (collectSound) MSound.PlayClip(time * 0.6f, collectSound);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
            PanelEre.GetInstance().InchBellow(PanelRate.SceneMusic.Sound_matchmj);
            //等待动画完成
           yield return new WaitWhile(() =>
           finishanim
           );

            //yield return new WaitWhile(() =>
            //   !moveComplete
            //   );
            yield return new WaitForEndOfFrame();
        }

        private void UpdatePossibleMatches()
        {
            possibleMatches = new PossibleMatches(MainGrid.GetFreeToMatchTiles());
            ChangePossibleMatchesAction?.Invoke(possibleMatches.Count);
            pairNumber = 0;
        }

        private void InstantiateScoreFlyer(Vector3 wPosition, string score)
        {
            if (!scoreFlyerPrefab) return;
            if (!parentCanvas)
            {
                GameObject gC = GameObject.Find("CanvasMain");
                if (gC) parentCanvas = gC.GetComponent<Canvas>();
                if (!parentCanvas) parentCanvas = FindFirstObjectByType<Canvas>();
            }

            GUIFlyer flyer = scoreFlyerPrefab.CreateFlyer(parentCanvas, score);
            if (flyer)
            {
                flyer.transform.localScale = transform.lossyScale;
                flyer.transform.position = wPosition; //  transform.position;
            }
        }

        public void FastCollect(MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
        {
            // 空值检查
            if (mahjongTile_1 == null || mahjongTile_2 == null)
            {
                Debug.LogError("MahjongTile is null in FastCollect");
                return;
            }

            SetControlActivity(false, false);
            GridCell gridCell_1 = mahjongTile_1.GetComponentInParent<GridCell>();
            GridCell gridCell_2 = mahjongTile_2.GetComponentInParent<GridCell>();

            // 检查GridCell是否为空
            if (gridCell_1 == null || gridCell_2 == null)
            {
                Debug.LogError("GridCell is null in FastCollect");
                SetControlActivity(true, true);
                return;
            }

            BeforeCollectAction?.Invoke(gridCell_1, gridCell_2, mahjongTile_1, mahjongTile_2);
            Sprite sprite_1 = mahjongTile_1.MSprite;
            Sprite sprite_2 = mahjongTile_2.MSprite;

            // 安全地设置父对象为null
            if (mahjongTile_1.transform != null)
                mahjongTile_1.transform.parent = null;
            if (mahjongTile_2.transform != null)
                mahjongTile_2.transform.parent = null;

            // 安全地销毁对象
            if (mahjongTile_1 != null && mahjongTile_1.gameObject != null)
                Destroy(mahjongTile_1.gameObject);
            if (mahjongTile_2 != null && mahjongTile_2.gameObject != null)
                Destroy(mahjongTile_2.gameObject);

            CollectAction?.Invoke(sprite_1, sprite_2);

            // 从处理中集合移除
            processingTiles.Remove(mahjongTile_1);
            processingTiles.Remove(mahjongTile_2);

            SetControlActivity(true, true);
        }

        /// <summary>
        /// 连续消除方法，支持在动画播放期间继续选择其他匹配
        /// </summary>
        public void ContinuousCollect(MahjongTile mahjongTile_1, MahjongTile mahjongTile_2)
        {
            // 空值检查
            if (mahjongTile_1 == null || mahjongTile_2 == null)
            {
                Debug.LogError("MahjongTile is null in ContinuousCollect");
                return;
            }

            if (!enableContinuousMatch)
            {
                CollectMatch(mahjongTile_1, mahjongTile_2);
                return;
            }

            MatchPair newPair = new MatchPair(mahjongTile_1, mahjongTile_2);
            matchQueue.Enqueue(newPair);

            if (!isProcessingMatch)
            {
                StartCoroutine(ProcessMatchQueue());
            }
        }

        private IEnumerator ProcessMatchQueue()
        {
            isProcessingMatch = true;

            while (matchQueue.Count > 0)
            {
                MatchPair currentPair = matchQueue.Dequeue();

                // 检查MatchPair是否有效
                if (currentPair != null && currentPair.mahjongTile_1 != null && currentPair.mahjongTile_2 != null)
                {
                    yield return StartCoroutine(CollectMatchC(currentPair.mahjongTile_1, currentPair.mahjongTile_2));
                }
                else
                {
                    Debug.LogWarning("Invalid MatchPair in queue");
                }
            }

            isProcessingMatch = false;
        }
        #endregion collect match

        #region hint
        MatchPair hintPair;

        private bool isInAutoCollect = false;

        private void TryAutoCollect()
        {
            // 前两关禁用自动收集
            if (GameLevelHolder.CurrentLevel <= 1)
            {
                Debug.Log("前两关禁用自动收集功能");
                return;
            }
            if (popGold)
            {
                return;
            }
            //int threshold = 30; 
            int threshold = MobTownEre.instance.FadeLoom.automatch;
            int remainPairs = MainGrid.GetTiles().Length / 2;
            if (isAutoCollecting || hasTriggeredAutoCollect) return;
            if (remainPairs <= threshold && remainPairs > 0)
            {
                isAutoCollecting = true;
                hasTriggeredAutoCollect = true;
                isInAutoCollect = true;
                StartCoroutine(AutoCollectAllCoroutine());
            }
            if (remainPairs > threshold)
            {
                hasTriggeredAutoCollect = false;
            }
        }

        public void ChangeGold()
        {
            if (isInAutoCollect) return; // 自动收集时不产生金麻将
            // 获取所有未消除且未在处理中的麻将牌
            var allTiles = MainGrid.GetTiles();
            if (allTiles == null || allTiles.Length < 2) return;

            // 收集所有可配对且都不是金色的组合
            List<(MahjongTile, MahjongTile)> validPairs = new List<(MahjongTile, MahjongTile)>();
            for (int i = 0; i < allTiles.Length; i++)
            {
                var tile1 = allTiles[i];
                if (tile1 == null || tile1.IsgoldTile || processingTiles.Contains(tile1)) continue;
                for (int j = i + 1; j < allTiles.Length; j++)
                {
                    var tile2 = allTiles[j];
                    if (tile2 == null || tile2.IsgoldTile || processingTiles.Contains(tile2)) continue;
                    if (tile1.SpriteCanMatchhWith(tile2.MSprite))
                    {
                        validPairs.Add((tile1, tile2));
                    }
                }
            }

            if (validPairs.Count > 0)
            {
                var rand = UnityEngine.Random.Range(0, validPairs.Count);
                var (tile1, tile2) = validPairs[rand];
                tile1.IsgoldTile = true;
                tile2.IsgoldTile = true;
                if (tile1.goldSprite) tile1.SRenderer.sprite = tile1.goldSprite;
                if (tile2.goldSprite) tile2.SRenderer.sprite = tile2.goldSprite;
            }
        }

        public void TrySelectHintMatch(Action<bool> selectCallBack)
        {
            if (possibleMatches == null)
            {
                UpdatePossibleMatches();
            }
            RemoveHint();

            if (possibleMatches.Count > pairNumber)
            {
                hintPair = possibleMatches.GetMatchPair(pairNumber);
                hintPair.mahjongTile_1.HighlightHint(true);
                hintPair.mahjongTile_2.HighlightHint(true);
                // paarNumber++;
                selectCallBack?.Invoke(true);
            }
            else
            {
                pairNumber = 0;
                selectCallBack?.Invoke(false);
            }
        }

        public bool IsAlreadyHint()
        {
            return hintPair != null && hintPair.mahjongTile_1 && hintPair.mahjongTile_2;
        }

        public void RemoveHint()
        {
            if (hintPair != null)
            {
                if (hintPair.mahjongTile_1) hintPair.mahjongTile_1.HighlightHint(false);
                if (hintPair.mahjongTile_2) hintPair.mahjongTile_2.HighlightHint(false);
            }
            hintPair = null;
        }

        public bool CheckExistingHint()
        {
            if (hintPair == null || !hintPair.mahjongTile_1 || !hintPair.mahjongTile_2) return false;
            if (possibleMatches.ContainMatchPair(hintPair)) return true;
            return false;
        }
        #endregion hint

        #region undo
        public void RaiseUndoEvents()
        {
            UndoEndAction?.Invoke();
        }
        #endregion undo

        #region free highlight
        public bool IsHihglightFreeMode
        {
            get
                ; set;
        }

        public void SetHiglightFreeMode(bool highlight)
        {
            if (IsHihglightFreeMode == highlight) return;
            PlayerPrefsExtension.SetBool("free_highlight", false);
            IsHihglightFreeMode = false;
            ChangeFreeHiglightModeAction?.Invoke(true);
        }

        private void LoadHighlightMode()
        {
            bool isFreeHihglighted = PlayerPrefsExtension.GetBool("free_highlight", false);
            SetHiglightFreeMode(false);
        }

        private void HighlihtFree(bool highlight)
        {
            List<MahjongTile> freeTiles = MainGrid.GetFreeToMatchTiles();
            List<MahjongTile> allTiles = MainGrid.GetTiles().ToList();

            if (highlight)
            {
                foreach (var item in allTiles)
                {
                    item.SetFreeHiglightColor(freeTiles.Contains(item) ? true : false);
                }
            }
            else
            {
                foreach (var item in allTiles)
                {
                    item.SetFreeHiglightColor(true);
                }
            }
        }
        #endregion free highlight

        /// <summary>
        /// starts when the game is interrupted
        /// </summary>
        public void BreakLevelEventRaise()
        {
            GameEvents.BreakLevelAction?.Invoke();
        }

        public void FailedMatchEventRaise()
        {
            FailedMatchAction?.Invoke();
        }

       
        // 延迟一帧并输出日志后遮灰
        private IEnumerator DelayHighlightFreeWithLog()
        {
            yield return null;
            Debug.Log($"DelayHighlightFreeWithLog 协程开始, MainGrid: {MainGrid}, Tiles数量: {(MainGrid != null ? MainGrid.GetTiles().Length : -1)}");
            MainGrid.CacheBlockers();
            Debug.Log($"==== 遮灰刷新前麻将牌数量: {MainGrid.GetTiles().Length}");
            foreach (var tile in MainGrid.GetTiles())
            {
                Debug.Log($"Tile: {tile.name}, Active: {tile.gameObject.activeSelf}, Layer: {tile.Layer}, ParentCell: {(tile.ParentCell != null ? tile.ParentCell.ToString() : "null")}");
            }
            HighlihtFree(true);
            Debug.Log($"==== 遮灰刷新后麻将牌状态:");
            foreach (var tile in MainGrid.GetTiles())
            {
                Debug.Log($"Tile: {tile.name}, Color: {tile.SRenderer.color}, Active: {tile.gameObject.activeSelf}");
            }
        }

        private void OnDestroy()
        {
            hasWinActionInvoked = false; // 场景销毁时重置
            Debug.Log("GameBoard OnDestroy 被调用");
                        // 移除消息监听
           
            SharplyResort.AwhileCarIncoming(CFellow.mg_OnMahjongClick, OnMahjongClick);
            
            // 清理预加载数据
            CleanupPreloadedData();
        }

        private bool isAutoCollecting = false;
        private bool hasTriggeredAutoCollect = false;
        private int goldComboCount = 0;
        /// <summary>
        /// 连续消除计数，满值后尝试触发金麻将奖励
        /// </summary>
        private void TryGoldRewardOnCombo()
        {
            var allTiles = MainGrid.GetTiles();
            bool hasGold = allTiles.Any(t => t != null && t.IsgoldTile);
            if (hasGold)
            {
                return;
            }
            goldComboCount++;
            int goldComboTarget = MobTownEre.instance.FadeLoom.combogold;
            bool isFull = goldComboCount >= goldComboTarget;
            if (isFull && !hasGold)
            {
                MahjongTile t1 = null, t2 = null;
                // 1. 先从possibleMatches里找一对可消除且都不是processingTiles的牌
                if (possibleMatches != null && possibleMatches.Count > 0)
                {
                    for (int i = 0; i < possibleMatches.Count; i++)
                    {
                        var pair = possibleMatches.GetMatchPair(i);
                        if (pair != null && pair.mahjongTile_1 != null && pair.mahjongTile_2 != null
                            && !processingTiles.Contains(pair.mahjongTile_1)
                            && !processingTiles.Contains(pair.mahjongTile_2))
                        {
                            t1 = pair.mahjongTile_1;
                            t2 = pair.mahjongTile_2;
                            break;
                        }
                    }
                }
                // 2. 如果possibleMatches里没有合适的对，再从全部牌里找一对能配对的
                if (t1 == null || t2 == null)
                {
                    bool found = false;
                    for (int i = 0; i < allTiles.Length; i++)
                    {
                        var tile1 = allTiles[i];
                        if (tile1 == null || processingTiles.Contains(tile1)) continue;
                        for (int j = i + 1; j < allTiles.Length; j++)
                        {
                            var tile2 = allTiles[j];
                            if (tile2 == null || processingTiles.Contains(tile2)) continue;
                            if (tile1.SpriteCanMatchhWith(tile2.MSprite))
                            {
                                t1 = tile1;
                                t2 = tile2;
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                }
                // 3. 只有t1和t2都不为null，才可以变成金麻将
                if (t1 != null && t2 != null)
                {
                    var transList = new List<Transform> { t1.transform, t2.transform };
                    GameEvents.GoldProgress?.Invoke(goldComboCount, true, transList, () =>
                    {
                        t1.SetGoldState(true);
                        t2.SetGoldState(true);
                        goldComboCount = 0;
                        // 通知HomePanel进度归零
                        GameEvents.GoldProgress?.Invoke(0, false, new List<Transform>(), null);
                    });
                }
                // 没有可用对，不生成金麻将，直接return
            }
            else
            {
                // 只做进度动画，无飞行动画和回调
                GameEvents.GoldProgress?.Invoke(goldComboCount, isFull, new List<Transform>(), null);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                isAutoCollecting = true;
                hasTriggeredAutoCollect = true;
                isInAutoCollect = true;
                StartCoroutine(AutoCollectAllCoroutine());
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                TryGoldRewardOnCombo();
            }
        }

         #region 预加载系统
        /// <summary>
        /// 初始化预加载容器
        /// </summary>
        private void InitializePreloadContainer()
        {
            // 创建一个隐藏的容器用于预加载
            GameObject preloadGO = new GameObject("PreloadContainer");
            preloadContainer = preloadGO.transform;
            preloadContainer.SetParent(transform);
            preloadContainer.localPosition = new Vector3(100, 0, 0); // 移出屏幕100像素
            Debug.Log("预加载容器已初始化");
        }

        /// <summary>
        /// 麻将点击事件处理
        /// </summary>
        private void OnMahjongClick(object data)
        {
            Debug.Log($"OnMahjongClick被调用，当前关卡: {GameLevelHolder.CurrentLevel}, hasClickedMahjong: {hasClickedMahjong}");
            
            if (preloadContainer == null)
            {
                Debug.Log("预加载容器未初始化");
                return;
            }
            
            if (!hasClickedMahjong)
            {
                hasClickedMahjong = true;
                Debug.Log("玩家第一次点击麻将，延迟启动预加载下一关");
                // 延迟启动预加载，避免点击时的卡顿
                StartCoroutine(DelayedStartPreload());
            }
        }
 /// <summary>
        /// 延迟启动预加载，避免点击时的卡顿
        /// </summary>
        private IEnumerator DelayedStartPreload()
        {
            // 等待当前帧结束，确保点击响应完成
            yield return null;
            
            // 再等待一帧，确保UI响应完成
            yield return null;
            
            // 检查游戏状态，如果正在处理匹配，再等待一下
            if (isProcessingMatch || matchQueue.Count > 0)
            {
                Debug.Log("游戏正在处理匹配，等待处理完成后再开始预加载");
                yield return new WaitForSeconds(1.0f); // 等待1秒
            }
            
            // 额外等待多帧，确保所有游戏逻辑都完成
            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }
            
            // 再等待一段时间，确保游戏完全稳定
            yield return new WaitForSeconds(0.5f);
            
            // 检查游戏是否空闲
            int idleFrames = 0;
            while (idleFrames < 30) // 等待30帧的空闲时间
            {
                if (isProcessingMatch || matchQueue.Count > 0 || isInAutoCollect)
                {
                    idleFrames = 0; // 重置空闲计数
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    idleFrames++;
                    yield return null;
                }
            }
            
            Debug.Log("游戏空闲，开始预加载");
            
            // 现在开始预加载
            StartPreloadNextLevel();
        }

        /// <summary>
        /// 开始预加载下一关
        /// </summary>
        public void StartPreloadNextLevel()
        {
            int nextLevel = GameLevelHolder.CurrentLevel + 1;
            Debug.Log($"开始预加载下一关: {nextLevel}");
            
            // 第一关是引导关，不预加载
            if (GameLevelHolder.CurrentLevel == 0)
            {
                Debug.Log($"关卡 {GameLevelHolder.CurrentLevel} 是引导关，跳过预加载");
                return;
            }
            
            // 检查是否已经预加载过
            if (preloadedData != null && preloadedData.level == nextLevel && preloadedData.isComplete)
            {
                Debug.Log($"关卡 {nextLevel} 已经预加载过了");
                return;
            }
               // 检查游戏状态，如果游戏繁忙，延迟启动
            if (isProcessingMatch || matchQueue.Count > 0 || isInAutoCollect)
            {
                Debug.Log("游戏状态繁忙，延迟启动预加载");
                StartCoroutine(DelayedStartPreload());
                return;
            }
            
            // 检查当前帧率，如果帧率太低，延迟启动
            if (Time.deltaTime > 0.033f) // 如果帧率低于30FPS
            {
                Debug.Log("当前帧率较低，延迟启动预加载");
                StartCoroutine(DelayedStartPreload());
                return;
            }
            
            // 使用低优先级启动预加载，避免影响游戏性能
            StartCoroutine(PreloadNextLevelCoroutine(nextLevel));
        }

        /// <summary>
        /// 预加载下一关的协程
        /// </summary>
        private IEnumerator PreloadNextLevelCoroutine(int levelToPreload)
        {
            float startTime = Time.realtimeSinceStartup;
            Debug.Log($"[预加载性能] 开始预加载关卡 {levelToPreload}");
            
            // 获取下一关的配置
            var nextLevelConfig = GCSet.GetLevelConstructSet(levelToPreload);
            if (nextLevelConfig == null)
            {
                Debug.LogWarning($"关卡 {levelToPreload} 配置不存在，跳过预加载");
                yield break;
            }

            // 创建预加载数据
            preloadedData = new PreloadedLevelData();
            preloadedData.level = levelToPreload;
            preloadedData.isComplete = false;
   // 在预加载容器中创建网格
            float gridStartTime = Time.realtimeSinceStartup;
            Debug.Log($"[预加载性能] 开始创建预加载网格，配置: {nextLevelConfig.name}");
            
            // 分帧创建网格
            yield return StartCoroutine(CreateGridWithFrameSplit(nextLevelConfig));
            
            float gridTime = Time.realtimeSinceStartup - gridStartTime;
            int tileCount = preloadedData.grid.GetTiles().Length;
            Debug.Log($"[预加载性能] 网格创建完成，耗时: {gridTime:F3}秒，麻将牌数量: {tileCount}");
            
            // 预计算图片分配
            float spriteStartTime = Time.realtimeSinceStartup;
            Debug.Log("[预加载性能] 开始预计算图片分配");
            yield return StartCoroutine(PrecalculateSpriteAssignmentsOptimized());
            
            float spriteTime = Time.realtimeSinceStartup - spriteStartTime;
            Debug.Log($"[预加载性能] 图片分配完成，耗时: {spriteTime:F3}秒");
            
            preloadedData.isComplete = true;
            float totalTime = Time.realtimeSinceStartup - startTime;
            Debug.Log($"[预加载性能] 关卡 {levelToPreload} 预加载完成，总耗时: {totalTime:F3}秒");
            
            // 记录性能数据
            PreloadPerformanceMonitor.RecordPreloadPerformance(levelToPreload, tileCount, totalTime, gridTime, spriteTime);
        }

        /// <summary>
        /// 分帧创建网格，避免卡顿
        /// </summary>
        private IEnumerator CreateGridWithFrameSplit(LevelConstructSet levelConfig)
        {
            // 创建网格对象（使用预加载专用构造函数，只创建网格结构）
            preloadedData.grid = new MatchGrid(levelConfig, GOSet, preloadContainer, GMode, true);
            
            // 等待多帧，让网格初始化完成
            for (int i = 0; i < 3; i++)
            {
                yield return null;
            }
            
            // 异步创建麻将牌
            yield return StartCoroutine(preloadedData.grid.CreateMahjongTilesAsync(levelConfig, GMode));
            
            // 获取麻将牌并分帧处理
            var tiles = preloadedData.grid.GetTiles();
            if (tiles != null && tiles.Length > 0)
            {
                int tilesPerFrame = 1; // 每帧处理1个麻将牌，最大程度减少单帧负载
                int processedCount = 0;
                
                for (int i = 0; i < tiles.Length; i += tilesPerFrame)
                {
                    // 处理这一批麻将牌
                    int endIndex = Mathf.Min(i + tilesPerFrame, tiles.Length);
                    for (int j = i; j < endIndex; j++)
                    {
                        if (tiles[j] != null)
                        {
                            // 这里可以添加一些初始化逻辑
                            tiles[j].SetToFront(false);
                            processedCount++;
                        }
                    }
                    
                    // 每处理一批就暂停多帧
                    for (int k = 0; k < 2; k++)
                    {
                        yield return null;
                    }
                    
                    // 每处理3批后，额外等待多帧，确保游戏流畅
                    if (processedCount % (tilesPerFrame * 3) == 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            yield return null;
                        }
                    }
                    
                    // 每处理10批后，额外等待更多帧，确保游戏非常流畅
                    if (processedCount % (tilesPerFrame * 10) == 0)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            yield return null;
                        }
                    }
                }
                
                Debug.Log($"[预加载性能] 网格初始化完成，处理了 {processedCount} 个麻将牌");
            }
        }

        /// <summary>
        /// 优化的预计算图片分配
        /// </summary>
        private IEnumerator PrecalculateSpriteAssignmentsOptimized()
        {
            var tiles = preloadedData.grid.GetTiles();
            preloadedData.spriteAssignments = new List<Sprite>();
            
            if (tiles == null || tiles.Length == 0)
            {
                Debug.LogWarning("[预加载性能] 没有麻将牌需要分配图片");
                yield break;
            }
            
            Debug.Log($"[预加载性能] 开始分配图片，麻将牌数量: {tiles.Length}");
            
            // 使用预加载专用的图片分配算法，传入目标关卡号
            // 使用更大的yieldStep加速预加载，但保持分帧
            yield return StartCoroutine(preloadedData.grid.SetMahjongSpritesForPreloadAsync(preloadedData.level, () => {
                Debug.Log("[预加载性能] 预加载网格图片分配完成");
            }, 1)); // 每1个操作暂停一帧，最大程度减少单帧负载
            
            // 分帧保存分配结果
            Debug.Log("[预加载性能] 开始保存图片分配结果");
            int savePerFrame = 1; // 每帧保存1个结果，最大程度减少单帧负载
            int savedCount = 0;
            
            for (int i = 0; i < tiles.Length; i += savePerFrame)
            {
                int endIndex = Mathf.Min(i + savePerFrame, tiles.Length);
                for (int j = i; j < endIndex; j++)
                {
                    if (tiles[j] != null && tiles[j].MSprite != null)
                    {
                        preloadedData.spriteAssignments.Add(tiles[j].MSprite);
                    }
                    else
                    {
                        preloadedData.spriteAssignments.Add(null);
                    }
                    savedCount++;
                }
                
                // 每保存一批就暂停多帧
                for (int k = 0; k < 2; k++)
                {
                    yield return null;
                }
                
                // 每保存3批后，额外等待多帧，确保游戏流畅
                if (savedCount % (savePerFrame * 3) == 0)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        yield return null;
                    }
                }
                
                // 每保存10批后，额外等待更多帧，确保游戏非常流畅
                if (savedCount % (savePerFrame * 10) == 0)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        yield return null;
                    }
                }
            }
            
            Debug.Log($"[预加载性能] 预加载图片分配保存完成，共保存 {preloadedData.spriteAssignments.Count} 个图片分配");
        }

        /// <summary>
        /// 应用预加载的图片分配
        /// </summary>
        private void ApplyPreloadedSpriteAssignments()
        {
            if (preloadedData == null || !preloadedData.isComplete)
            {
                Debug.LogWarning("没有可用的预加载数据");
                return;
            }
            
            var tiles = MainGrid.GetTiles();
            Debug.Log($"应用预加载图片分配: {tiles.Length}个麻将, {preloadedData.spriteAssignments.Count}个图片");
            
            // 应用图片分配
            for (int i = 0; i < tiles.Length && i < preloadedData.spriteAssignments.Count; i++)
            {
                if (preloadedData.spriteAssignments[i] != null)
                {
                    tiles[i].SetSprite(preloadedData.spriteAssignments[i]);
                }
            }
            
            Debug.Log("预加载图片分配应用完成");
        }

        /// <summary>
        /// 清理预加载数据
        /// </summary>
        private void CleanupPreloadedData()
        {
            if (preloadedData != null && preloadedData.grid != null)
            {
                // 只销毁预加载的网格内容，不销毁preloadContainer本身
                var tiles = preloadedData.grid.GetTiles();
                foreach (var tile in tiles)
                {
                    if (tile != null && tile.gameObject != null)
                    {
                        UnityEngine.Object.DestroyImmediate(tile.gameObject);
                    }
                }
                
                // 清理预加载数据
                preloadedData = null;
                Debug.Log("预加载数据已清理");
            }
        }

        /// <summary>
        /// 测试预加载性能（开发调试用）
        /// </summary>
        [ContextMenu("测试预加载性能")]
        public void TestPreloadPerformance()
        {
            if (preloadContainer == null)
            {
                InitializePreloadContainer();
            }
            
            Debug.Log("[预加载性能测试] 开始测试预加载性能");
            StartCoroutine(TestPreloadPerformanceCoroutine());
        }
        
        private IEnumerator TestPreloadPerformanceCoroutine()
        {
            // 测试预加载下一关
            int testLevel = GameLevelHolder.CurrentLevel + 1;
            Debug.Log($"[预加载性能测试] 测试预加载关卡 {testLevel}");
            
            yield return StartCoroutine(PreloadNextLevelCoroutine(testLevel));
            
            // 显示性能摘要
            PreloadPerformanceMonitor.LogPerformanceSummary();
            
            // 清理测试数据
            CleanupPreloadedData();
            
            Debug.Log("[预加载性能测试] 测试完成");
        }


        #endregion 预加载系统

        /// <summary>
        /// 播放入场动画：将所有麻将牌移动到很大位置，然后按层级一层一层移动回原位
        /// </summary>
        private IEnumerator PlayLevelLoadAnimation()
        {
            Debug.Log("开始播放入场动画");
            
            var tiles = MainGrid.GetTiles();
            if (tiles == null || tiles.Length == 0)
            {
                Debug.LogWarning("没有麻将牌可以播放动画");
                yield break;
            }
            
            // 1. 将所有麻将牌移动到很大位置
            Vector3 startPosition = new Vector3(100, 0, 0); // 起始位置Y轴100
            foreach (var tile in tiles)
            {
                if (tile != null && tile.transform != null)
                {
                    // 直接动画麻将牌的主Transform，而不是Ani子节点
                    tile.transform.localPosition = startPosition;
                }
            }
            
            // 2. 按层级分组
            var tilesByLayer = new Dictionary<int, List<MahjongTile>>();
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    int layer = tile.Layer;
                    if (!tilesByLayer.ContainsKey(layer))
                    {
                        tilesByLayer[layer] = new List<MahjongTile>();
                    }
                    tilesByLayer[layer].Add(tile);
                }
            }
            
            // 3. 按层级顺序移动回原位（layer 0先移动，但间隔很短）
            var sortedLayers = new List<int>(tilesByLayer.Keys);
            sortedLayers.Sort(); // 从低层到高层，layer 0先移动
            
            // 创建所有层的动画序列
            var allSequences = DOTween.Sequence();
            
            foreach (int layer in sortedLayers)
            {
                var layerTiles = tilesByLayer[layer];
                Debug.Log($"准备第{layer}层动画，麻将牌数量: {layerTiles.Count}");
                
                // 为这一层创建动画序列
                var layerSequence = DOTween.Sequence();
                
                foreach (var tile in layerTiles)
                {
                    if (tile != null && tile.transform != null)
                    {
                        // 直接动画麻将牌的主Transform
                        layerSequence.Join(tile.transform.DOLocalMove(Vector3.zero, 0.7f)
                            .SetEase(Ease.Linear));
                    }
                }
                
                // 将这一层的动画添加到总序列中，延迟很短
                float delay = layer * 0.2f; // 每层延迟0.05秒
                allSequences.Insert(delay, layerSequence);
            }
            
            // 播放所有动画
            yield return allSequences.WaitForCompletion();
            
            // 动画完成后，恢复正确的Z轴位置
            Debug.Log("恢复麻将牌的正确Z轴位置");
            foreach (var tile in tiles)
            {
                if (tile != null && tile.transform != null)
                {
                    // 恢复层级偏移的Z轴位置
                    Vector3 currentPos = tile.transform.localPosition;
                    Vector3 layerOffset = tile.layerOffset * tile.Layer;
                    tile.transform.localPosition = new Vector3(currentPos.x, currentPos.y, layerOffset.z);
                    
                    Debug.Log($"恢复麻将牌 {tile.name} 的Z轴位置: {layerOffset.z} (层级: {tile.Layer})");
                }
            }
            
            // 动画完成后，更新所有麻将牌的spritetransformPosition为当前位置
            Debug.Log("更新所有麻将牌的spritetransformPosition");
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    var tileTouchBehavior = tile.GetComponent<TileTouchBehavior>();
                    if (tileTouchBehavior != null)
                    {
                        // 使用反射更新spritetransformPosition字段
                        var field = typeof(TileTouchBehavior).GetField("spritetransformPosition", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            // 将Vector3转换为Vector2
                            Vector3 position3D = tile.SRenderer.transform.position;
                            Vector2 position2D = new Vector2(position3D.x, position3D.y);
                            field.SetValue(tileTouchBehavior, position2D);
                        }
                    }
                }
            }
            
            Debug.Log("入场动画播放完成");
        }
    }

    #region 预加载性能监控
    public static class PreloadPerformanceMonitor
    {
        public static float LastPreloadTotalTime { get; private set; }
        public static float LastGridCreationTime { get; private set; }
        public static float LastSpriteAssignmentTime { get; private set; }
        public static int LastPreloadLevel { get; private set; }
        public static int LastTileCount { get; private set; }
        
        public static void RecordPreloadPerformance(int level, int tileCount, float totalTime, float gridTime, float spriteTime)
        {
            LastPreloadLevel = level;
            LastTileCount = tileCount;
            LastPreloadTotalTime = totalTime;
            LastGridCreationTime = gridTime;
            LastSpriteAssignmentTime = spriteTime;
            
            Debug.Log($"[预加载性能监控] 关卡{level}预加载完成 - 总耗时:{totalTime:F3}s, 网格创建:{gridTime:F3}s, 图片分配:{spriteTime:F3}s, 麻将牌数量:{tileCount}");
        }
        
        public static void LogPerformanceSummary()
        {
            Debug.Log($"[预加载性能监控] 最近预加载 - 关卡:{LastPreloadLevel}, 麻将牌:{LastTileCount}, 总耗时:{LastPreloadTotalTime:F3}s");
        }
    }
    #endregion 预加载性能监控
}
