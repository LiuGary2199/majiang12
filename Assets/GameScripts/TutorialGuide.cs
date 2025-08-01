using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

namespace Mkey
{
    /// <summary>
    /// 新手引导系统，实现强引导功能
    /// </summary>
    public class TutorialGuide : MonoBehaviour
    {
        public static TutorialGuide Instance { get; private set; }

        [Header("新手引导设置")]
        public bool enableTutorialGuide = true; // 是否启用新手引导

        // 新手引导配对字典
        private Dictionary<int, (MahjongTile, MahjongTile)> tutorialPairs = new Dictionary<int, (MahjongTile, MahjongTile)>();
        private int currentPairIndex = 0; // 当前引导的配对索引
        private bool isTutorialActive = false; // 是否正在引导中

        // 点击状态跟踪
        private MahjongTile firstClickedTile = null; // 第一次点击的麻将牌
        private MahjongTile secondClickedTile = null; // 第二次点击的麻将牌
        public GameObject Mask;
        // 引导步骤状态
        private enum TutorialStep
        {
            Step1_1_3,      // 第1步：引导点击1-3牌
            Step2_4_5_Blocked, // 第2步：引导点击4-5牌（无法消除）
            Step3_2_6,      // 第3步：引导点击2-6牌
            Step4_4_5_Unlocked // 第4步：引导点击4-5牌（已解锁）
        }
        private TutorialStep currentStep = TutorialStep.Step1_1_3;

        // ===================== 第二关专用引导 =====================
        private enum TutorialStepLevel2
        {
            Step1_Button,         // 引导点击按钮
            Step2_SpecialPair,   // 引导点击第1、3张麻将
            Step3_BlockPair,     // 引导点击7、9张（无法配对）
            Step4_UnlockPair,    // 引导点击4、10张
            Step5_FinalPair,     // 引导点击7、9张
            End
        }
        private TutorialStepLevel2 currentStepLevel2;
        private bool isTutorialLevel2Active = false;
        // 第二关配对字典和索引（完全隔离）
        private Dictionary<int, (MahjongTile, MahjongTile)> level2Pairs = new Dictionary<int, (MahjongTile, MahjongTile)>();
        public int currentLevel2PairIndex = 0;
        private MahjongTile level2FirstClickedTile = null;
        private MahjongTile level2SecondClickedTile = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 监听关卡加载完成事件
            GameEvents.LevelLoadCompleteAction += OnLevelLoadComplete;
        }

        private void OnDestroy()
        {
            GameEvents.LevelLoadCompleteAction -= OnLevelLoadComplete;
        }

        /// <summary>
        /// 关卡加载完成时的处理
        /// </summary>
        private void OnLevelLoadComplete()
        {
            // 只在第一关启用新手引导
            if (GameLevelHolder.CurrentLevel == 0 && enableTutorialGuide)
            {
                StartTutorialGuide();
            }
            else if (GameLevelHolder.CurrentLevel == 1 && enableTutorialGuide)
            {
                StartTutorialGuideForLevel2();
            }
            else
            {
                isTutorialActive = false;
            }
        }

        /// <summary>
        /// 开始新手引导
        /// </summary>
        public void StartTutorialGuide()
        {
            Debug.Log("开始新手引导");
            isTutorialActive = true;
            currentPairIndex = 0;
            currentStep = TutorialStep.Step1_1_3;
            // 新增：广播引导开始事件
            GameEvents.TutorialGuideStartedAction?.Invoke();

            // 重置点击状态
            ResetClickState();

            // 获取所有麻将牌
            MahjongTile[] allTiles = GameBoard.Instance.MainGrid.GetTiles();

            // 重置所有麻将牌的高亮状态
            ResetAllHighlights(allTiles);

            // 清理GameBoard的提示高亮
            GameBoard.Instance.RemoveHint();

            // 构建配对字典（根据新的引导顺序）
            BuildTutorialPairs(allTiles);

            // 开始引导第一对
            StartGuideCurrentPair();
        }

        /// <summary>
        /// 重置点击状态
        /// </summary>
        private void ResetClickState()
        {
            firstClickedTile = null;
            secondClickedTile = null;
        }

        /// <summary>
        /// 重置所有麻将牌的高亮状态
        /// </summary>
        private void ResetAllHighlights(MahjongTile[] allTiles)
        {
            foreach (var tile in allTiles)
            {
                if (tile != null)
                {
                    tile.HighlightHint(false);
                }
            }
            Debug.Log("已重置所有麻将牌的高亮状态");
        }

        /// <summary>
        /// 构建新手引导配对字典
        /// </summary>
        private void BuildTutorialPairs(MahjongTile[] allTiles)
        {
            tutorialPairs.Clear();

            if (allTiles.Length >= 6)
            {
                // 第1步：第1块(索引0)和第3块(索引2)配对
                tutorialPairs[0] = (allTiles[0], allTiles[2]);

                // 第2步：第4块(索引3)和第5块(索引4)配对（无法消除）
                tutorialPairs[1] = (allTiles[3], allTiles[4]);

                // 第3步：第2块(索引1)和第6块(索引5)配对
                tutorialPairs[2] = (allTiles[1], allTiles[5]);

                // 第4步：第4块(索引3)和第5块(索引4)配对（已解锁）
                tutorialPairs[3] = (allTiles[3], allTiles[4]);

                Debug.Log($"新手引导配对构建完成，共{tutorialPairs.Count}对");
                Debug.Log($"第1步: 索引0和索引2 (1-3牌)");
                Debug.Log($"第2步: 索引3和索引4 (4-5牌，无法消除)");
                Debug.Log($"第3步: 索引1和索引5 (2-6牌)");
                Debug.Log($"第4步: 索引3和索引4 (4-5牌，已解锁)");
            }
        }

        /// <summary>
        /// 开始引导当前配对
        /// </summary>
        private void StartGuideCurrentPair()
        {
            if (!isTutorialActive || !tutorialPairs.ContainsKey(currentPairIndex))
            {
                Debug.Log("新手引导完成或配对不存在");
                EndTutorialGuide();
                return;
            }
            var currentPair = tutorialPairs[currentPairIndex];
            MahjongTile tile1 = currentPair.Item1;
            MahjongTile tile2 = currentPair.Item2;
            Debug.Log($"开始高亮第{currentPairIndex + 1}步配对");
            // 高亮当前配对
            tile1.HighlightHint(true);
            tile2.HighlightHint(true);

            Debug.Log($"开始引导第{currentPairIndex + 1}步配对");
            // 重置点击状态
            ResetClickState();
            // 根据当前步骤显示相应提示
            ShowStepTip();
        }

        /// <summary>
        /// 显示当前步骤的提示
        /// </summary>
        private void ShowStepTip()
        {
            switch (currentStep)
            {
                case TutorialStep.Step1_1_3:
                    ShowTutorialTipManual("Match the same tiles to get rewards!");
                    break;
                case TutorialStep.Step2_4_5_Blocked:
                    break;
                case TutorialStep.Step3_2_6:

                    break;
                case TutorialStep.Step4_4_5_Unlocked:
                    // 先全部恢复正常色
                    MahjongTile[] allTiles = GameBoard.Instance.MainGrid.GetTiles();
                    foreach (var t in allTiles) t?.SetGray(false);
                    ShowTutorialTip("Match the same tiles to get rewards!", 3f);
                    break;
            }
        }

        /// <summary>
        /// 检查点击的麻将牌是否为当前引导的配对
        /// </summary>
        public bool IsValidTutorialClick(MahjongTile clickedTile)
        {
            if (!isTutorialActive || !tutorialPairs.ContainsKey(currentPairIndex))
                return true; // 不在引导中，允许正常点击

            var currentPair = tutorialPairs[currentPairIndex];
            MahjongTile tile1 = currentPair.Item1;
            MahjongTile tile2 = currentPair.Item2;

            // 检查点击的是否为当前引导的配对
            return clickedTile == tile1 || clickedTile == tile2;
        }

        /// <summary>
        /// 处理麻将牌点击事件
        /// </summary>
        public void OnMahjongTileClicked(MahjongTile clickedTile)
        {
            if (!isTutorialActive)
                return;

            if (!IsValidTutorialClick(clickedTile))
            {
                // 显示错误提示
                ShowTutorialTip("请按照提示点击正确的麻将牌！", 2f);
                return;
            }
            // 检查是否完成了当前配对
            var currentPair = tutorialPairs[currentPairIndex];
            MahjongTile tile1 = currentPair.Item1;
            MahjongTile tile2 = currentPair.Item2;

            // 如果点击的是当前配对中的一个
            if (clickedTile == tile1 || clickedTile == tile2)
            {
                // 第一次点击
                if (firstClickedTile == null)
                {
                    firstClickedTile = clickedTile;
                    Debug.Log($"第一次点击麻将牌: {clickedTile}");
                    //ShowTutorialTip("请点击配对的另一张麻将牌！", 2f);
                }
                // 第二次点击
                else if (secondClickedTile == null && clickedTile != firstClickedTile)
                {
                    secondClickedTile = clickedTile;
                    Debug.Log($"第二次点击麻将牌: {clickedTile}");

                    // 检查是否匹配
                    if (firstClickedTile.SpriteCanMatchhWith(secondClickedTile.MSprite))
                    {
                        Debug.Log("两张麻将牌匹配成功！");
                        // 配对成功，进入下一步
                        OnPairCompleted();
                    }
                    else
                    {

                    }
                }
                // 重复点击同一张牌
                else if (clickedTile == firstClickedTile)
                {
                    ShowTutorialTip("请点击不同的麻将牌！", 2f);
                }
                // 其他情况
                else
                {
                    ShowTutorialTip("请按照高亮提示点击正确的麻将牌！", 2f);
                }
            }
            else
            {
                // 点击了其他麻将牌，显示提示
                ShowTutorialTip("请按照高亮提示点击正确的麻将牌！", 2f);
            }
        }

        /// <summary>
        /// 配对完成处理
        /// </summary>
        private void OnPairCompleted()
        {
            // 兼容旧接口，直接调用协程
            StartCoroutine(OnPairCompletedCoroutine());
        }

        /// <summary>
        /// 配对完成处理协程，等待0.2秒后推进
        /// </summary>
        private IEnumerator OnPairCompletedCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            // 清理GameBoard的提示高亮
            GameBoard.Instance.RemoveHint();
            // 取消当前配对的高亮
            var currentPair = tutorialPairs[currentPairIndex];
            TileTouchBehavior.ClearCurrentSelectionAndUnselectAllTiles();
            currentPairIndex++;
            // 更新当前步骤
            UpdateCurrentStep();
            // 检查是否还有下一步
            if (tutorialPairs.ContainsKey(currentPairIndex))
            {
                // 如果是从第二步进入第三步，延迟1秒
                if (currentPairIndex == 2)
                {
                    FadeAnalyze.Instance.SequoiaTrial();
                    ShowTutorialTipManual("Clear left or right tiles to unlock inner tiles");
                    yield return new WaitForSeconds(1f);
                    FadeAnalyze.Instance.WoodyTrial();
                }

                StartGuideCurrentPair();
            }
            else
            {
                // 引导完成
                EndTutorialGuide();
            }
        }

        /// <summary>
        /// 延迟第二步配对完成后的处理
        /// </summary>
        private IEnumerator DelayedOnPairCompletedForStep2()
        {
            // 等待一帧，确保所有状态更新完成
            yield return new WaitForSeconds(1f);

            // 再次确保所有高亮都被清理
            var pair = tutorialPairs[currentPairIndex];
            pair.Item1.HighlightHint(false);
            pair.Item2.HighlightHint(false);
            pair.Item1.HighlightSelected(false);
            pair.Item2.HighlightSelected(false);
            Debug.Log("延迟处理中再次清理高亮状态");
            // 清理GameBoard提示
            GameBoard.Instance.RemoveHint();
            // 进入下一步
            OnPairCompleted();
        }

        /// <summary>
        /// 更新当前引导步骤
        /// </summary>
        private void UpdateCurrentStep()
        {
            switch (currentPairIndex)
            {
                case 1:
                    currentStep = TutorialStep.Step2_4_5_Blocked;
                    break;
                case 2:
                    currentStep = TutorialStep.Step3_2_6;
                    break;
                case 3:
                    currentStep = TutorialStep.Step4_4_5_Unlocked;
                    break;
                default:
                    currentStep = TutorialStep.Step1_1_3;
                    break;
            }
        }

        /// <summary>
        /// 结束新手引导
        /// </summary>
        private void EndTutorialGuide()
        {
            Debug.Log("新手引导完成");
            isTutorialActive = false;
            // 新增：广播引导结束事件
            GameEvents.TutorialGuideEndedAction?.Invoke();

            // 隐藏当前提示
            HideTutorialTip();

            // 取消所有高亮
            foreach (var pair in tutorialPairs.Values)
            {
                pair.Item1.HighlightHint(false);
                pair.Item2.HighlightHint(false);
            }

            // 清理GameBoard的提示高亮
            GameBoard.Instance.RemoveHint();
            tutorialPairs.Clear();
        }

        /// <summary>
        /// 显示新手引导提示（自动关闭）
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="duration">显示时长（秒），0表示手动关闭</param>
        private void ShowTutorialTip(string message, float duration = 2f)
        {
            Debug.Log($"新手引导提示: {message}");

            // 使用事件通知HomePanel显示提示
            GameEvents.ShowTipAction?.Invoke(message, duration);
        }

        /// <summary>
        /// 显示新手引导提示（手动关闭）
        /// </summary>
        /// <param name="message">提示信息</param>
        private void ShowTutorialTipManual(string message)
        {
            Debug.Log($"新手引导提示（手动关闭）: {message}");

            // 使用事件通知HomePanel显示提示（手动关闭）
            GameEvents.ShowTipManualAction?.Invoke(message);
        }

        /// <summary>
        /// 隐藏新手引导提示
        /// </summary>
        private void HideTutorialTip()
        {
            Debug.Log("隐藏新手引导提示");

            // 使用事件通知HomePanel隐藏提示
            GameEvents.HideTipAction?.Invoke();
        }

        /// <summary>
        /// 检查是否正在新手引导中
        /// </summary>
        public bool IsInTutorial()
        {
            return isTutorialActive;
        }

        // 外部调用：开始第二关引导
        public void StartTutorialGuideForLevel2()
        {
            if (GameLevelHolder.CurrentLevel != 1) return;
            isTutorialLevel2Active = true;
            if(FactorDram.UpHonor()){
 currentStepLevel2 = TutorialStepLevel2.Step2_SpecialPair;
            }else{
 currentStepLevel2 = TutorialStepLevel2.Step1_Button;
            }
           
            // 新增：广播引导开始事件
            GameEvents.TutorialGuideStartedAction?.Invoke();
            // 只在引导开始时构建一次配对字典
            MahjongTile[] allTiles = GameBoard.Instance.MainGrid.GetTiles();
            level2Pairs.Clear();
            if (allTiles.Length > 9)
            {
                level2Pairs[0] = (allTiles[0], allTiles[2]); // 1、3
                level2Pairs[1] = (allTiles[6], allTiles[9]); // 7、10
                level2Pairs[2] = (allTiles[3], allTiles[8]); // 4、9
                level2Pairs[3] = (allTiles[6], allTiles[9]); // 7、10
            }
            else
            {
                Debug.LogError($"[引导] 当前麻将牌数量不足10张，实际数量：{allTiles.Length}，请检查关卡配置！");
                isTutorialLevel2Active = false;
                return;
            }
            ShowLevel2Step();
        }
        // 主流程推进
        private void ShowLevel2Step()
        {
            MahjongTile[] allTiles = GameBoard.Instance.MainGrid.GetTiles();
            // 按钮引导
            if (currentStepLevel2 == TutorialStepLevel2.Step1_Button)
            {
                SetRectMaskPanelActive(true);
                Mask.SetActive(true);
                // ShowTutorialTipManual("点击按钮领取奖励");
                GameEvents.CashOutPanelClosedAction += OnLevel2Step1BtnDone;
                return;
            }
            // 进入麻将牌配对引导（不再刷新level2Pairs）
            // 清除所有高亮
            foreach (var t in allTiles) t?.HighlightHint(false);
            // 清空点击状态
            level2FirstClickedTile = null;
            level2SecondClickedTile = null;
            SetRectMaskPanelActive(false);
            Mask.SetActive(false);
            // 高亮当前配对
            if (level2Pairs.ContainsKey(currentLevel2PairIndex))
            {
                var pair = level2Pairs[currentLevel2PairIndex];
                pair.Item1.HighlightHint(true);
                pair.Item2.HighlightHint(true);
                // 不同提示文本
                switch (currentLevel2PairIndex)
                {
                    case 0:
                        ShowTutorialTipManual("Special tiles can be matched with each other");
                        break;
                    case 1:
                        var thirdPair = level2Pairs[2];
                        if (thirdPair.Item2 != null && thirdPair.Item2.SRenderer != null)
                        {
                            thirdPair.Item2.SRenderer.sortingOrder += 20000;
                            Debug.Log("++++++++++++++++++++++" + thirdPair.Item2.SRenderer.sortingOrder);
                        }
                        ShowTutorialTipManual("Try to match these tiles");
                        break;
                    case 2:
                        var thirdPairs = level2Pairs[2];
                        if (thirdPairs.Item2 != null && thirdPairs.Item2.SRenderer != null)
                        {
                            thirdPairs.Item2.SRenderer.sortingOrder -= 20000;

                        }
                        ShowTutorialTipManual("Match the top tiles to unlock other tiles!");
                        break;
                    case 3:
                                        MahjongTile[] allTilesss = GameBoard.Instance.MainGrid.GetTiles();
                    foreach (var t in allTilesss) t?.SetGray(false);
                        ShowTutorialTipManual("Match the golden tiles to get huge rewards!");
                        break;
                }
            }
            else
            {
                // 引导结束
                HideTutorialTip();

                isTutorialLevel2Active = false;
                // 新增：广播引导结束事件
                GameEvents.TutorialGuideEndedAction?.Invoke();
            }
        }
        // 按钮引导完成回调
        private void OnLevel2Step1BtnDone()
        {
            GameEvents.CashOutPanelClosedAction -= OnLevel2Step1BtnDone;
            currentStepLevel2 = TutorialStepLevel2.Step2_SpecialPair;
            currentLevel2PairIndex = 0;
            ShowLevel2Step();
            BaskTrialGerman.GetInstance().ArabTrial("1002");
        }
        // 第二关麻将牌点击处理（完全隔离）
        public void OnMahjongTileClickedLevel2(MahjongTile clickedTile)
        {
            if (!isTutorialLevel2Active || !level2Pairs.ContainsKey(currentLevel2PairIndex)) return;
            var pair = level2Pairs[currentLevel2PairIndex];
            if (clickedTile != pair.Item1 && clickedTile != pair.Item2)
            {
                ShowTutorialTip("请点击高亮的麻将牌", 2f);
                return;
            }
            // 记录点击状态
            if (level2FirstClickedTile == null)
            {
                level2FirstClickedTile = clickedTile;
                return;
            }
            if (level2SecondClickedTile == null && clickedTile != level2FirstClickedTile)
            {
                level2SecondClickedTile = clickedTile;
                // 判断是否配对
                if (level2FirstClickedTile.SpriteCanMatchhWith(level2SecondClickedTile.MSprite))
                {
                    // 配对成功，推进下一步
                    currentLevel2PairIndex++;
                    ShowLevel2Step();
                }
                else
                {
                    ShowTutorialTip("请配对正确的麻将牌", 2f);
                }
            }
        }
        // 第二关：判断是否为当前配对的高亮麻将
        public bool IsValidTutorialClickLevel2(MahjongTile clickedTile)
        {
            if (!isTutorialLevel2Active || !level2Pairs.ContainsKey(currentLevel2PairIndex)) return false;
            var pair = level2Pairs[currentLevel2PairIndex];
            return clickedTile == pair.Item1 || clickedTile == pair.Item2;
        }
        // 第一关：只允许当前配对的两张牌配对
        public bool IsValidTutorialPair(MahjongTile tileA, MahjongTile tileB)
        {
            if (!isTutorialActive || !tutorialPairs.ContainsKey(currentPairIndex)) return true;
            var pair = tutorialPairs[currentPairIndex];
            return (tileA == pair.Item1 && tileB == pair.Item2) || (tileA == pair.Item2 && tileB == pair.Item1);
        }
        // 第二关：只允许当前配对的两张牌配对
        public bool IsValidTutorialPairLevel2(MahjongTile tileA, MahjongTile tileB)
        {
            if (!isTutorialLevel2Active || !level2Pairs.ContainsKey(currentLevel2PairIndex)) return true;
            var pair = level2Pairs[currentLevel2PairIndex];
            return (tileA == pair.Item1 && tileB == pair.Item2) || (tileA == pair.Item2 && tileB == pair.Item1);
        }
        // 设置遮黑面板显示/隐藏
        private void SetRectMaskPanelActive(bool active)
        {
            var maskPanel = GetComponentInChildren<FleeMiteStore>(true);
            if (maskPanel != null)
            {
                maskPanel.gameObject.SetActive(active);
            }
        }
        // ===================== 第二关辅助方法 =====================
        public bool IsTutorialLevel2Active() { return isTutorialLevel2Active; }

        // 新增：获取当前第二关配对索引
        public int GetCurrentLevel2PairIndex()
        {
            return currentLevel2PairIndex;
        }
        // 新增：获取第二关指定配对
        public (MahjongTile, MahjongTile) GetLevel2Pair(int idx)
        {
            return level2Pairs[idx];
        }
        // 新增：获取第一关当前pairIndex
        public int GetCurrentPairIndex => currentPairIndex;
    }
}