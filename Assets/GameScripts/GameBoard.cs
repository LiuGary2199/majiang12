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
            Debug.Log("Create gameboard ");
            Debug.Log("level set: " + LCSet.name);
            Debug.Log("current level: " + GameLevelHolder.CurrentLevel);

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
                    DOVirtual.DelayedCall(1.8f, () =>  //停顿
                    {
                        finishanim = false;
                    });
                }
                else
                {
                    leftTile.PlayNbroke(() => { });
                    rightTile.PlayNbroke(() =>
                    {
                      
                    });
                    DOVirtual.DelayedCall(1.8f, () =>  //停顿
                    {
                        finishanim = false;
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

        /// <summary>
        /// 通关后加载下一关（仅用UI和数据刷新，不重载场景）
        /// </summary>
        public void CompleteAndLoadNextLevel()
        {
            // 关卡切换时重置自动收集和通关标志
            isAutoCollecting = false;
            hasTriggeredAutoCollect = false;
            isInAutoCollect = false;
            hasWinActionInvoked = false;
            goldComboCount = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            // 更新当前关卡编号到下一关并触发UI更新
            GameLevelHolder.SetCurrentLevelAndUpdateUI(GameLevelHolder.CurrentLevel + 1);

            // 刷新关卡数据和UI
            MainGrid.Rebuild(GOSet, GMode);
            sw.Stop();
            Debug.Log($"[耗时] MainGrid.Rebuild: {sw.ElapsedMilliseconds} ms");
            var swSprites = System.Diagnostics.Stopwatch.StartNew();
            // 用静态测试回调
            MainGrid.SetMahjongSprites(() =>
            {
                swSprites.Stop();
                Debug.Log($"[耗时] SetMahjongSprites: {swSprites.ElapsedMilliseconds} ms");
                TestCallback();
            });
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
    }
}
