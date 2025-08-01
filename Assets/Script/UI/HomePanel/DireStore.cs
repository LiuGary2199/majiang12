using DG.Tweening;
using LitJson;
using Mkey;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class DireStore : ComeUIFlank
{
    public static DireStore Instance;
[UnityEngine.Serialization.FormerlySerializedAs("GoldBar")]
    public GameObject BoldHim;
[UnityEngine.Serialization.FormerlySerializedAs("BackBtn")]
    public Button PermDew;
[UnityEngine.Serialization.FormerlySerializedAs("SettingBtn")]    public Button IngoingDew;
[UnityEngine.Serialization.FormerlySerializedAs("SelectBtn")]    public Button CanadaDew;
[UnityEngine.Serialization.FormerlySerializedAs("SelectText")]    public Text CanadaSago;
[UnityEngine.Serialization.FormerlySerializedAs("SelectLevelObj")]    public GameObject CanadaTenthSun;
[UnityEngine.Serialization.FormerlySerializedAs("cashImg")]
    public Image LienHat;
[UnityEngine.Serialization.FormerlySerializedAs("cashNumText")]    public Text LienBowSago;
[UnityEngine.Serialization.FormerlySerializedAs("LevelText")]    public Text TenthSago;
[UnityEngine.Serialization.FormerlySerializedAs("instrans")]    public Transform Organism;

    // 提示相关UI组件
    [Header("提示面板")]
[UnityEngine.Serialization.FormerlySerializedAs("tipPanel")]    public GameObject SewStore; // 提示面板
[UnityEngine.Serialization.FormerlySerializedAs("tipText")]    public Text SewSago; // 提示文本
[UnityEngine.Serialization.FormerlySerializedAs("tipBackground")]    public Image SewConspiracy; // 提示背景
[UnityEngine.Serialization.FormerlySerializedAs("tipCloseButton")]    public Button SewMediaPiston; // 提示关闭按钮
[UnityEngine.Serialization.FormerlySerializedAs("ComboanimPB")]
    public Household HouseholdPB;
[UnityEngine.Serialization.FormerlySerializedAs("ComboParent")]    public Transform ClumpTomato;
[UnityEngine.Serialization.FormerlySerializedAs("DayTimeBtn")]    public Button YewRichDew;
[UnityEngine.Serialization.FormerlySerializedAs("DayTimeRedPoint")]    public GameObject YewRichSexStage;
[UnityEngine.Serialization.FormerlySerializedAs("CashoutBtn")]    public RectTransform MicrobeDew;
[UnityEngine.Serialization.FormerlySerializedAs("TimeDown")]    public int RichLust= 0;

    // 保存CashoutBtn的原始位置
    private Vector2 IdentityMicrobeDewTurnpike;
[UnityEngine.Serialization.FormerlySerializedAs("m_SkBG")]
    public SkeletonGraphic m_ByBG;
[UnityEngine.Serialization.FormerlySerializedAs("m_SkSetting")]    public SkeletonGraphic m_ByIngoing;
[UnityEngine.Serialization.FormerlySerializedAs("m_Bottomobj")]    public GameObject m_Unlimited;
[UnityEngine.Serialization.FormerlySerializedAs("GoldImg")]    public Image BoldHat;
[UnityEngine.Serialization.FormerlySerializedAs("GoldFly")]    public GameObject BoldGut;
[UnityEngine.Serialization.FormerlySerializedAs("Logo")]    public GameObject Duke;
[UnityEngine.Serialization.FormerlySerializedAs("settingbtn1")]
    public Button Imperative1; //
[UnityEngine.Serialization.FormerlySerializedAs("Coin")]
    public GameObject Dumb;
[UnityEngine.Serialization.FormerlySerializedAs("Coinimage")]    public Image Looseness;
[UnityEngine.Serialization.FormerlySerializedAs("CoinStr")]    public Text DumbOwe;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (FactorDram.UpHonor())
        {
            Dumb.SetActive(true);
            MicrobeDew.gameObject.SetActive(false);

        }
        else
        {
            Dumb.SetActive(false);
            MicrobeDew.gameObject.SetActive(true);

        }

        // 监听动画结束事件
        m_ByBG.AnimationState.Complete += OnBGComplete;
        m_ByIngoing.AnimationState.Complete += OnSetComplete;
        GameEvents.LevelLoadCompleteAction += OnLevelComplete;
        GameEvents.GoldProgress += BoldMandible;


        // 监听提示事件
        GameEvents.ShowTipAction += OnShowTip;
        GameEvents.ShowTipManualAction += OnShowTipManual;
        GameEvents.HideTipAction += OnHideTip;

        PermDew.onClick.AddListener(() =>
        {
            IdentityMicrobeDewTurnpike = new Vector2(0, -146);
            CanadaTenthSun.gameObject.SetActive(true);
            m_ByBG.AnimationState.ClearTracks();
            m_ByBG.AnimationState.SetAnimation(0, "3close", false);
            m_ByIngoing.AnimationState.ClearTracks();
            m_ByIngoing.AnimationState.SetAnimation(0, "1open", false);
            SalineAttemptTenthSago();
            BravelyMicrobeDewAnyway();
        });
        YewRichDew.onClick.AddListener(() =>
        {
            YelpUIDeer(nameof(RichRadium));
        });

        CanadaDew.onClick.AddListener(() =>
        {
            IdentityMicrobeDewTurnpike = new Vector2(0, -277);
            CanadaDew.gameObject.SetActive(false);
            m_ByBG.AnimationState.ClearTracks();
            m_ByBG.AnimationState.SetAnimation(0, "1open", false);
            m_ByIngoing.AnimationState.ClearTracks();
            m_ByIngoing.AnimationState.SetAnimation(0, "3close", false);
            Duke.SetActive(false);
            BravelyMicrobeDewAnyway();

            // 新增：点击SelectBtn时让新手引导开始检测
            if (Mkey.TutorialGuide.Instance != null)
            {
                if (Mkey.GameLevelHolder.CurrentLevel == 0)
                {
                    Mkey.TutorialGuide.Instance.StartTutorialGuide();
                }
                else if (Mkey.GameLevelHolder.CurrentLevel == 1)
                {
                    Mkey.TutorialGuide.Instance.StartTutorialGuideForLevel2();
                }
            }
        });
        Imperative1.onClick.AddListener(() =>
        {
            YelpUIDeer(nameof(IngoingStore));
        });
        IngoingDew.onClick.AddListener(() =>
        {
            YelpUIDeer(nameof(IngoingStore));
        });
        SharplyResort.AxeCarIncoming(CFellow.Me_OfClumpBead, OnComboUpdate);
        SharplyResort.AxeCarIncoming(CFellow.Me_OfAxeFavor, OnCashUpdate);
        SharplyResort.AxeCarIncoming(CFellow.Me_CheeseYewRadium, OnUpdateDayReward);

        FieldRadiumWoody();
        AttemptTenthSago(); // 初始化时刷新关卡文本

        // 保存CashoutBtn的原始位置

        FadFamineCrane();

        // 初始化提示面板
        VoltRoeStore();

        // 新增：监听新手引导开始/结束事件，控制按钮可用性
        GameEvents.TutorialGuideStartedAction += OnTutorialGuideStarted;
        GameEvents.TutorialGuideEndedAction += OnTutorialGuideEnded;

        m_ByIngoing.AnimationState.SetAnimation(0, "1open", false);
        MicrobeDew.anchoredPosition = new Vector2(0, -146);
    }

    /// <summary>
    /// 初始化提示面板
    /// </summary>
    private void VoltRoeStore()
    {
        if (SewStore != null)
        {
            // 初始时隐藏
            SewStore.SetActive(false);

            // 设置关闭按钮事件
            if (SewMediaPiston != null)
            {
                SewMediaPiston.onClick.AddListener(OnHideTip);
            }
        }
    }

    /// <summary>
    /// 显示提示事件处理
    /// </summary>
    private void OnShowTip(string message, float duration)
    {
        BeadRoe(message, duration);
    }

    /// <summary>
    /// 显示提示（手动关闭）事件处理
    /// </summary>
    private void OnShowTipManual(string message)
    {
        BeadRoe(message, 0f); // 传入0表示不自动关闭
    }

    /// <summary>
    /// 隐藏提示事件处理
    /// </summary>
    private void OnHideTip()
    {
        WarmRoe();
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="message">提示信息</param>
    /// <param name="duration">显示时长（秒），0表示不自动隐藏，需要手动调用HideTip()关闭</param>
    public void BeadRoe(string message, float duration = 0f)
    {
        if (SewSago != null)
        {
            SewSago.text = message;
        }

        if (SewStore != null)
        {
            SewStore.SetActive(true);

            // 淡入动画
            CanvasGroup canvasGroup = SewStore.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = SewStore.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f);

            // 如果设置了自动隐藏时间（大于0）
            if (duration > 0)
            {
                DOVirtual.DelayedCall(duration, () =>
                {
                    WarmRoe();
                });
            }
        }
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    public void WarmRoe()
    {
        if (SewStore != null)
        {
            CanvasGroup canvasGroup = SewStore.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
                {
                    SewStore.SetActive(false);
                });
            }
            else
            {
                SewStore.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 静态方法，供外部调用显示提示（自动关闭）
    /// </summary>
    /// <param name="message">提示信息</param>
    /// <param name="duration">显示时长（秒），0表示不自动隐藏</param>
    public static void BeadRoeSharply(string message, float duration = 3f)
    {
        if (Instance != null)
        {
            Instance.BeadRoe(message, duration);
        }
    }

    /// <summary>
    /// 静态方法，供外部调用显示提示（手动关闭）
    /// </summary>
    /// <param name="message">提示信息</param>
    public static void BeadRoeSharplySymbol(string message)
    {
        if (Instance != null)
        {
            Instance.BeadRoe(message, 0f); // 传入0表示不自动关闭
        }
    }

    /// <summary>
    /// 静态方法，供外部调用隐藏提示
    /// </summary>
    public static void WarmRoeSharply()
    {
        if (Instance != null)
        {
            Instance.WarmRoe();
        }
    }

    private void OnLevelComplete()
    {
        Debug.Log("DireStore 监听到过关事件");
        FadFamineCrane();
        // 进度条归零
        if (BoldHat != null && BoldHat.gameObject.activeInHierarchy)
        {
            DG.Tweening.DOTween.Kill(BoldHat, "GoldFill");
            DG.Tweening.DOTween.To(() => BoldHat.fillAmount, x => BoldHat.fillAmount = x, 0f, 0.3f)
                .SetId("GoldFill");
        }
    }
    // goldMul是总进度，count是当前进度，GoldImg是进度图片，GoldFly是金币预制体
    // posTrans为目标位置列表，callBack为飞行完成回调
    public void BoldMandible(int count, bool flystate, List<Transform> posTrans, Action callBack)
    {
        int goldMul = MobTownEre.instance.FadeLoom.combogold;
        float targetFill = Mathf.Clamp01((float)count / goldMul);
        // DOTween动画进度条
        if (BoldHat != null && BoldHat.gameObject.activeInHierarchy)
        {
            if (!Mathf.Approximately(BoldHat.fillAmount, targetFill))
            {
                DG.Tweening.DOTween.Kill(BoldHat, "GoldFill");
                DG.Tweening.DOTween.To(() => BoldHat.fillAmount, x => BoldHat.fillAmount = x, targetFill, 0.3f)
                    .SetId("GoldFill");
            }
        }
        // 满了且需要飞行
        if (count >= goldMul && flystate && posTrans != null && posTrans.Count >= 2)
        {
            // 生成两个金币飞行
            for (int i = 0; i < 2; i++)
            {
                GameObject fly = Instantiate(BoldGut, BoldHat.transform.position, Quaternion.identity, BoldHat.transform.parent);
                Vector3 targetPos = posTrans[i].position;
                fly.transform.SetAsLastSibling();
                fly.transform.DOMove(targetPos, 0.6f).SetEase(DG.Tweening.Ease.InQuad).OnComplete(() =>
                {
                    Destroy(fly);
                    // 两个都完成后回调
                    CuteGutAshcanImage++;
                    if (CuteGutAshcanImage == 2)
                    {
                        CuteGutAshcanImage = 0;
                        callBack?.Invoke();
                    }
                });
            }
        }
    }
    private int CuteGutAshcanImage= 0;

    private void FadFamineCrane()
    {
        if (Mkey.GameLevelHolder.CurrentLevel <= 1)
        {
            m_Unlimited.SetActive(false);
        }
        else
        {
            m_Unlimited.SetActive(true);
        }
    }

    private void OnBGComplete(TrackEntry trackEntry)
    {
        if (trackEntry != null)
        {
            if (trackEntry.Animation.Name == "1open")
            {
                CanadaTenthSun.gameObject.SetActive(false);
            }

            if (trackEntry.Animation.Name == "3close")
            {
                CanadaDew.gameObject.SetActive(true);
                Duke.SetActive(true);
                m_ByBG.AnimationState.ClearTracks();
                m_ByBG.AnimationState.SetAnimation(0, "2idle", true);
            }
        }

    }
    private void OnSetComplete(TrackEntry trackEntry)
    {
        if (trackEntry != null)
        {
            if (trackEntry.Animation.Name == "1open" || trackEntry.Animation.Name == "3close")
            {
                m_ByIngoing.AnimationState.ClearTracks();
                m_ByIngoing.AnimationState.SetAnimation(0, "2idle", true);
            }
        }
    }
    private void OnUpdateDayReward(KeyValuesUpdate kv)
    {
        FieldRadiumWoody();
    }

    private void OnCashUpdate(KeyValuesUpdate kv)
    {
        addScoreData sendData = (addScoreData)kv.Tongue;
        Organism.position = sendData.Demise3Not;
        AxeArab(sendData.ClumpImage, Organism);
    }

    private void OnComboUpdate(KeyValuesUpdate kv)
    {
        ArabLoom sendData = (ArabLoom)kv.Tongue;
        //Canvas canvas = UIAnalyze.GetInstance().MainCanvas.GetComponent<Canvas>();
        //Vector2  pos = WorldToCanvasPos(canvas, sendData.Vector3pos);
        //Household combo =  Instantiate(ComboanimPB, ComboParent);
        //RectTransform rect = combo.GetComponent<RectTransform>();
        //rect.anchoredPosition = pos;
        HouseholdPB.InchDebt(sendData.ClumpImage);


        //Debug.Log(kv.Key);
    }
   


    /// <summary>
    /// 世界坐标转换为画布坐标
    /// </summary>
    /// <param name="canvas">画布</param>
    /// <param name="world">世界坐标</param>
    /// <returns></returns>
    private Vector2 ShadeBeEnergyOak(Canvas canvas, Vector3 world)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
        return position;
    }
    public void AxeArab(double cash, Transform objTrans = null)
    {
        FadeLoomAnalyze.GetInstance().LopFlood(cash);
        ArabAxeStoreroom(objTrans, 5);
    }
    private void ArabAxeStoreroom(Transform startTransform, double num)
    {
      
            if(FactorDram.UpHonor())
            {
  AxeStoreroom(startTransform, Looseness.transform, Looseness.gameObject, DumbOwe,
            FadeLoomAnalyze.GetInstance().MixFlood(), num);
            }
            else{
  AxeStoreroom(startTransform, LienHat.transform, LienHat.gameObject, LienBowSago,
            FadeLoomAnalyze.GetInstance().MixFlood(), num);
            }
    }
    private void AxeStoreroom(Transform startTransform, Transform endTransform, GameObject icon, Text text,
       double textValue, double num)
    {
        if (startTransform != null)
        {
            StoreroomSuccessful.BoldRoamPoet(icon, Mathf.Max((int)num, 1), startTransform, endTransform,
                () =>
                {
                    ///PanelEre.GetInstance().PlayEffect(PanelRate.SceneMusic.sound_getcoin);
                    StoreroomSuccessful.PillowVacant(double.Parse(text.text), textValue, 0.1f, text,
                        () => { text.text = VacantDram.ShovelBeOwe(textValue); });
                });
        }
        else
        {
            StoreroomSuccessful.PillowVacant(double.Parse(text.text), textValue, 0.1f, text,
                () => { text.text = VacantDram.ShovelBeOwe(textValue); });
        }
    }

    private void FieldRadiumWoody()
    {
        List<DayRewardData> EarRadiumArrow= new List<DayRewardData>();
        string[] datas = new string[4];
        datas = SameLoomAnalyze.OwnChangeUrban(CFellow.Dy_FieldRadium);
        long nowtime = GameUtil.GetNowTime();
        bool redState = false;
        for (int i = 0; i < datas.Length; i++)
        {
            string data = datas[i];
            DayRewardData dayData = JsonMapper.ToObject<DayRewardData>(data);
            EarRadiumArrow.Add(dayData);
            DayRewardData deforRewardData = EarRadiumArrow.Find(x => x.dataIndex == (i - 1));
            bool beforGet = true;
            if (deforRewardData != null)
            {
                beforGet = deforRewardData.getState == 1;
            }
            if (nowtime > dayData.look_time)
            {
                if (dayData.getState == 0)
                {
                    redState = true;
                    break;
                }
            }
            else
            {
                if (beforGet == true)
                {
                    if (RichLust != 0)
                    {
                        StoneAnalyze.Overtone.PaneStone(RichLust);
                    }
                    RichLust = StoneAnalyze.Overtone.WoodyStone(1, () => //启动计时器
                    {
                        int times = dayData.look_time - (int)GameUtil.GetNowTime();
                        if (times <= 0)
                        {
                            Debug.Log("计时完成");
                            StoneAnalyze.Overtone.PaneStone(RichLust);
                            YewRichSexStage.SetActive(true);
                        }
                    }, true);
                    break;
                }
            }
        }
        YewRichSexStage.SetActive(redState);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIAnalyze.GetInstance().BeadUIFlank(nameof(BegBound));
        }
        // 测试用：按下B键切换到下一关
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 关卡号自增并保存
            Mkey.GameLevelHolder.SetCurrentLevelAndSave(Mkey.GameLevelHolder.CurrentLevel + 1);
            // 先清理旧麻将牌对象
            if (GameBoard.Instance != null)
            {
                GameBoard.Instance.DestroyGrid(); // 清理旧麻将
                GameBoard.Instance.CreateGameBoard(); // 用新关卡号重建关卡
            }
            // 触发关卡开始事件，刷新UI（包括GameLevelHelper等）
            Mkey.GameLevelHolder.StartLevel();
            // 刷新主界面关卡号显示

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIAnalyze.GetInstance().BeadUIFlank(nameof(TenthTimidityStore));
        }

    }

    /// <summary>
    /// 刷新关卡文本，显示当前关卡号
    /// </summary>
    public void AttemptTenthSago()
    {
        if (TenthSago != null)
        {
            int level = Mkey.GameLevelHolder.CurrentLevel; // 获取当前关卡号
            TenthSago.text = $"LEVEL {level + 1}"; // 设置关卡文本
        }
    }

    /// <summary>
    /// 静态方法，供外部调用刷新关卡文本
    /// </summary>
    public static void SalineAttemptTenthSago()
    {
        if (Instance != null)
        {
            Instance.AttemptTenthSago();
        }
    }

    /// <summary>
    /// 让CashoutBtn移出屏幕显示范围，然后等待指定时间后移回原位置
    /// </summary>
    /// <param name="waitTime">等待时间（秒）</param>
    /// <param name="moveOutDuration">移出动画时长（秒）</param>
    /// <param name="moveInDuration">移入动画时长（秒）</param>
    /// <param name="onComplete">动画完成回调</param>
    public void BravelyMicrobeDew(float waitTime = 2f, float moveOutDuration = 0.7f, float moveInDuration = 0.5f, Action onComplete = null)
    {
        if (MicrobeDew == null)
        {
            Debug.LogError("CashoutBtn is null!");
            onComplete?.Invoke();
            return;
        }

        // 停止之前的动画
        MicrobeDew.DOKill();

        // 第一步：移出屏幕（Y轴移动到300）
        Vector2 moveOutPosition = new Vector2(MicrobeDew.anchoredPosition.x, 300f);

        MicrobeDew.DOAnchorPos(moveOutPosition, moveOutDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 第二步：等待指定时间
                DOVirtual.DelayedCall(waitTime, () =>
                {
                    // 第三步：移回原位置
                    MicrobeDew.DOAnchorPos(IdentityMicrobeDewTurnpike, moveInDuration)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() =>
                        {
                            onComplete?.Invoke();
                        });
                });
            });
    }

    /// <summary>
    /// 让CashoutBtn移出屏幕显示范围，然后等待指定时间后移回原位置（简化版本）
    /// </summary>
    /// <param name="waitTime">等待时间（秒）</param>
    /// <param name="onComplete">动画完成回调</param>
    public void BravelyMicrobeDewAnyway(float waitTime = 0.1f, Action onComplete = null)
    {
        BravelyMicrobeDew(waitTime, 0.5f, 0.5f, onComplete);
    }
    private void OnDestroy()
    {
        // 记得移除监听，避免内存泄漏
        GameEvents.WinLevelAction -= OnLevelComplete;
        GameEvents.ShowTipAction -= OnShowTip;
        GameEvents.ShowTipManualAction -= OnShowTipManual;
        GameEvents.HideTipAction -= OnHideTip;
        // 新增：移除新手引导事件监听
        GameEvents.TutorialGuideStartedAction -= OnTutorialGuideStarted;
        GameEvents.TutorialGuideEndedAction -= OnTutorialGuideEnded;
    }

    // 新增：新手引导开始时禁用按钮
    private void OnTutorialGuideStarted()
    {
        if (PermDew != null) PermDew.interactable = false;
        if (IngoingDew != null) IngoingDew.interactable = false;
    }
    // 新增：新手引导结束时恢复按钮
    private void OnTutorialGuideEnded()
    {
        if (PermDew != null) PermDew.interactable = true;
        if (IngoingDew != null) IngoingDew.interactable = true;
    }
}
