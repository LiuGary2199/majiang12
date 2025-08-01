using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using AdjustSdk;
using com.adjust.sdk;
using LitJson;

public class ADAnalyze : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("MAX_SDK_KEY")]    public string MAX_SDK_KEY= "";
[UnityEngine.Serialization.FormerlySerializedAs("MAX_REWARD_ID")]    public string MAX_REWARD_ID= "";
[UnityEngine.Serialization.FormerlySerializedAs("MAX_INTER_ID")]    public string MAX_INTER_ID= "";
[UnityEngine.Serialization.FormerlySerializedAs("isTest")]
    public bool UpGrid= false;
    public static ADAnalyze Overtone{ get; private set; }

    private int SightFateful;   // 广告加载失败后，重新加载广告次数
    private bool UpAnimateTo;     // 是否正在播放广告，用于判断切换前后台时是否增加计数

    public int PikeInchRichFertile{ get; private set; }   // 距离上次广告的时间间隔
    public int Luckily101{ get; private set; }     // 定时插屏(101)计数器
    public int Luckily102{ get; private set; }     // NoThanks插屏(102)计数器
    public int Luckily103{ get; private set; }     // 后台回前台插屏(103)计数器

    private string CellarMagicalWhig;
    private Action<bool> CellarTentPermEocene;    // 激励视频回调
    private bool CellarBedroom;     // 激励视频是否成功收到奖励
    private string CellarNomad;     // 激励视频的打点

    private string PreparednessMagicalWhig;
    private int PreparednessRate;      // 当前播放的插屏类型，101/102/103
    private string PreparednessNomad;     // 插屏广告的的打点
    public bool BouleRichProficiently{ get; private set; } // 定时插屏暂停播放

    private List<Action<ADType>> adVarietyEnjoyable;    // 广告播放完成回调列表，用于其他系统广告计数（例如商店看广告任务）

    private long InefficientHingeRationale;     // 切后台时的时间戳
    private Ad_CustomData RadiumToCustomLoom; //激励视频自定义数据
    private Ad_CustomData ProficientlyToVacantLoom; //插屏自定义数据

    private void Awake()
    {
        Overtone = this;
    }

    private void OnEnable()
    {
        BouleRichProficiently = false;
        UpAnimateTo = false;
        PikeInchRichFertile = 999;  // 初始时设置一个较大的值，不阻塞插屏广告
        CellarBedroom = false;

        // Android平台将Adjust的adid传给Max；iOS将randomKey传给Max
#if UNITY_ANDROID
        MaxSdk.SetSdkKey(OwnRegretLoom.DecryptDES(MAX_SDK_KEY));
        // 将adjust id 传给Max
        string adjustId = SameLoomAnalyze.GetString(CFellow.sv_AdjustAdid);
        if (string.IsNullOrEmpty(adjustId))
        {
            adjustId = Adjust.getAdid();
        }
        if (!string.IsNullOrEmpty(adjustId))
        {
            MaxSdk.SetUserId(adjustId);
            MaxSdk.InitializeSdk();
            SameLoomAnalyze.SetString(CFellow.sv_AdjustAdid, adjustId);
        }
        else
        {
            StartCoroutine(setAdjustAdid());
        }
#else
        MaxSdk.SetSdkKey(OwnRegretLoom.MixtureDES(MAX_SDK_KEY));
        MaxSdk.SetUserId(SameLoomAnalyze.OwnChange(CFellow.Dy_SullyTiltAt));
        MaxSdk.InitializeSdk();
#endif

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // 打开调试模式
            //MaxSdk.ShowMediationDebugger();

            IlluminateAcademicHem();
            MaxSdk.SetCreativeDebuggerEnabled(true);

            // 每秒执行一次计数
            InvokeRepeating(nameof(BorrowChitin), 1, 1);
        };
    }

    IEnumerator MapCensusUpon()
    {
        int i = 0;
      //  string adjustId = "";
        while (i < 5)
        {
            yield return new WaitForSeconds(1);
            if (FactorDram.UpDegree())
            {
                MaxSdk.SetUserId(SameLoomAnalyze.OwnChange(CFellow.Dy_SullyTiltAt));
                MaxSdk.InitializeSdk();
                yield break;
            }
            else
            {
                //Adjust.GetAdid((adid) =>
                //{
                //    adjustId = adid;
                //});
                string adjustId = Adjust.getAdid();
                if (!string.IsNullOrEmpty(adjustId))
                {
                    MaxSdk.SetUserId(adjustId);
                    MaxSdk.InitializeSdk();
                    SameLoomAnalyze.FadChange(CFellow.Dy_CensusUpon, adjustId);
                    yield break;
                }
            }
            i++;
        }
        if (i == 5)
        {
            MaxSdk.SetUserId(SameLoomAnalyze.OwnChange(CFellow.Dy_SullyTiltAt));
            MaxSdk.InitializeSdk();
        }
    }

    public void IlluminateAcademicHem()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first rewarded ad
        TallAcademicTo();

        // Load the first interstitial
        TallProficiently();
    }

    private void TallAcademicTo()
    {
        MaxSdk.LoadRewardedAd(MAX_REWARD_ID);
    }

    private void TallProficiently()
    {
        MaxSdk.LoadInterstitial(MAX_INTER_ID);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        SightFateful = 0;
        CellarMagicalWhig = adInfo.NetworkName;

        RadiumToCustomLoom = new Ad_CustomData();
        RadiumToCustomLoom.user_id = CashOutManager.GetInstance().Data.UserID;
        RadiumToCustomLoom.version = Application.version;
        RadiumToCustomLoom.request_id = CashOutManager.GetInstance().EcpmRequestID();
        RadiumToCustomLoom.vendor = adInfo.NetworkName;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        SightFateful++;
        double retryDelay = Math.Pow(2, Math.Min(6, SightFateful));

        Invoke(nameof(TallAcademicTo), (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if UNITY_IOS
        PanelEre.GetInstance().OxPanelDesire = !PanelEre.GetInstance().OxPanelDesire;
        Time.timeScale = 0;
#endif
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        TallAcademicTo();
        UpAnimateTo = false;
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
#if UNITY_IOS
        Time.timeScale = 1;
        PanelEre.GetInstance().OxPanelDesire = !PanelEre.GetInstance().OxPanelDesire;
#endif

        UpAnimateTo = false;
        TallAcademicTo();
        if (CellarBedroom)
        {
            CellarBedroom = false;
            CellarTentPermEocene?.Invoke(true);

            AfterToInchBedroom(ADType.Rewarded);
            BaskTrialGerman.GetInstance().ArabTrial("9007", CellarNomad);
        }
        else
        {
            CellarTentPermEocene?.Invoke(false);
        }

        // 上报ecpm
        CashOutManager.GetInstance().ReportEcpm(adInfo, RadiumToCustomLoom.request_id, "REWARD");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        CellarBedroom = true;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo info)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        //从MAX获取收入数据
        //var adRevenue = new AdjustAdRevenue("applovin_max_sdk");
        //adRevenue.SetRevenue(info.Revenue, "USD");
        //adRevenue.AdRevenueNetwork = info.NetworkName;
        //adRevenue.AdRevenueUnit = info.AdUnitIdentifier;
        //adRevenue.AdRevenuePlacement = info.Placement;
        var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        adRevenue.setRevenue(info.Revenue, "USD");
        adRevenue.setAdRevenueNetwork(info.NetworkName);
        adRevenue.setAdRevenueUnit(info.AdUnitIdentifier);
        adRevenue.setAdRevenuePlacement(info.Placement);
        //发回收入数据给自己后台
        string countryCodeByMAX = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"
        BaskTrialGerman.GetInstance().ArabTrial("9008", info.Revenue.ToString(), countryCodeByMAX);

        //带广告收入的漏传策略
       // CensusVoltAnalyze.Instance.AddAdCount(countryCodeByMAX, info.Revenue);

        string adjustAdid = CensusVoltAnalyze.Instance.OwnCensusUpon();
        //发回收入数据给Adjust
        if (!string.IsNullOrEmpty(adjustAdid))
        {
            //Adjust.TrackAdRevenue(adRevenue);
            Adjust.trackAdRevenue(adRevenue);
            UnityEngine.Debug.Log("Max to Adjust (rewarded), adUnitId:" + adUnitId + ", revenue:" + info.Revenue + ", network:" + info.NetworkName + ", unit:" + info.AdUnitIdentifier + ", placement:" + info.Placement);
        }

        // 发回收入数据给Mintegral
        if (!string.IsNullOrEmpty(adjustAdid))
        {
#if UNITY_ANDROID || UNITY_IOS
            MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_ADJUST, adjustAdid);
            ///MaxSdkBase.AdInfo类型的adInfo
            mBridgeRevenueParamsEntity.SetMaxAdInfo(info);
            MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
            UnityEngine.Debug.Log(nameof(MBridgeRevenueManager) + "~Rewarded revenue:" + info.Revenue);
#endif
        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        SightFateful = 0;
        PreparednessMagicalWhig = adInfo.NetworkName;

        ProficientlyToVacantLoom = new Ad_CustomData();
        ProficientlyToVacantLoom.user_id = CashOutManager.GetInstance().Data.UserID;
        ProficientlyToVacantLoom.version = Application.version;
        ProficientlyToVacantLoom.request_id = CashOutManager.GetInstance().EcpmRequestID();
        ProficientlyToVacantLoom.vendor = adInfo.NetworkName;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        SightFateful++;
        double retryDelay = Math.Pow(2, Math.Min(6, SightFateful));

        Invoke(nameof(TallProficiently), (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if UNITY_IOS
        PanelEre.GetInstance().OxPanelDesire = !PanelEre.GetInstance().OxPanelDesire;
        Time.timeScale = 0;
#endif
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        TallProficiently();
        UpAnimateTo = false;
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo info)
    {
        //从MAX获取收入数据
        var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        adRevenue.setRevenue(info.Revenue, "USD");
        adRevenue.setAdRevenueNetwork(info.NetworkName);
        adRevenue.setAdRevenueUnit(info.AdUnitIdentifier);
        adRevenue.setAdRevenuePlacement(info.Placement);
        //var adRevenue = new AdjustAdRevenue("applovin_max_sdk");
        //adRevenue.SetRevenue(info.Revenue, "USD");
        //adRevenue.AdRevenueNetwork = info.NetworkName;
        //adRevenue.AdRevenueUnit = info.AdUnitIdentifier;
        //adRevenue.AdRevenuePlacement = info.Placement;

        //发回收入数据给自己后台
        string countryCodeByMAX = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"
        BaskTrialGerman.GetInstance().ArabTrial("9108", info.Revenue.ToString(), countryCodeByMAX);

        //带广告收入的漏传策略
    //    CensusVoltAnalyze.Instance.AddAdCount(countryCodeByMAX, info.Revenue);

        //发回收入数据给Adjust
        if (!string.IsNullOrEmpty(CensusVoltAnalyze.Instance.OwnCensusUpon()))
        {
            //Adjust.TrackAdRevenue(adRevenue);
            Adjust.trackAdRevenue(adRevenue);
            UnityEngine.Debug.Log("Max to Adjust (interstitial), adUnitId:" + adUnitId + ", revenue:" + info.Revenue + ", network:" + info.NetworkName + ", unit:" + info.AdUnitIdentifier + ", placement:" + info.Placement);
        }

        // 发回收入数据给Mintegral
        string adjustAdid = CensusVoltAnalyze.Instance.OwnCensusUpon();
        if (!string.IsNullOrEmpty(adjustAdid))
        {
#if UNITY_ANDROID || UNITY_IOS
            MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_ADJUST, adjustAdid);
            ///MaxSdkBase.AdInfo类型的adInfo
            mBridgeRevenueParamsEntity.SetMaxAdInfo(info);
            MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
            UnityEngine.Debug.Log(nameof(MBridgeRevenueManager) + "~Interstitial revenue:" + info.Revenue);
#endif
        }
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
#if UNITY_IOS
        Time.timeScale = 1;
        PanelEre.GetInstance().OxPanelDesire = !PanelEre.GetInstance().OxPanelDesire;
#endif
        TallProficiently();

        AfterToInchBedroom(ADType.Interstitial);
        BaskTrialGerman.GetInstance().ArabTrial("9107", PreparednessNomad);
        // 上报ecpm
        CashOutManager.GetInstance().ReportEcpm(adInfo, ProficientlyToVacantLoom.request_id, "INTER");
    }


    /// <summary>
    /// 播放激励视频广告
    /// </summary>
    /// <param name="callBack"></param>
    /// <param name="index"></param>
    public void DikeRadiumUnder(Action<bool> callBack, string index)
    {
        if (UpGrid)
        {
            callBack(true);
            AfterToInchBedroom(ADType.Rewarded);
            return;
        }

        bool rewardVideoReady = MaxSdk.IsRewardedAdReady(MAX_REWARD_ID);
        CellarTentPermEocene = callBack;
        if (rewardVideoReady)
        {
            // 打点
            CellarNomad = index;
            BaskTrialGerman.GetInstance().ArabTrial("9002", index);
            UpAnimateTo = true;
            CellarBedroom = false;
            string placement = index + "_" + CellarMagicalWhig;
            RadiumToCustomLoom.placement_id = placement;
            MaxSdk.ShowRewardedAd(MAX_REWARD_ID, placement, JsonMapper.ToJson(RadiumToCustomLoom));
        }
        else
        {
            OughtAnalyze.GetInstance().BeadOught("No ads right now, please try it later.");
            CellarTentPermEocene(false);
        }
    }

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="index"></param>
    public void DikeProficientlyTo(int index)
    {
        if (index == 101 || index == 102 || index == 103)
        {
            UnityEngine.Debug.LogError("广告点位不允许为101、102、103");
            return;
        }

        DikeProficiently(index);
    }

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="index">101/102/103</param>
    /// <param name="customIndex">用户自定义点位</param>
    private void DikeProficiently(int index, int customIndex = 0)
    {
        PreparednessRate = index;

        if (UpAnimateTo)
        {
            return;
        }

        //这个参数很少有游戏会用 需要的时候自己再打开
         //当用户过关数 < trial_MaxNum时，不弹插屏广告
         int sv_trialNum = SameLoomAnalyze.OwnNor(CFellow.Dy_An_Spark_Peg);
        int trial_MaxNum = int.Parse(MobTownEre.instance.FellowLoom.trial_MaxNum);
        if (sv_trialNum < trial_MaxNum)
        {
            return;
        }

        // 时间间隔低于阈值，不播放广告
        if (PikeInchRichFertile < int.Parse(MobTownEre.instance.FellowLoom.inter_freq))
        {
            return;
        }

        if (UpGrid)
        {
            AfterToInchBedroom(ADType.Interstitial);
            return;
        }

        bool interstitialVideoReady = MaxSdk.IsInterstitialReady(MAX_INTER_ID);
        if (interstitialVideoReady)
        {
            UpAnimateTo = true;
            // 打点
            string point = index.ToString();
            if (customIndex > 0)
            {
                point += customIndex.ToString().PadLeft(2, '0');
            }
            PreparednessNomad = point;
            BaskTrialGerman.GetInstance().ArabTrial("9102", point);
            string placement = point + "_" + PreparednessMagicalWhig;
            ProficientlyToVacantLoom.placement_id = placement;
            MaxSdk.ShowInterstitial(MAX_INTER_ID, placement, JsonMapper.ToJson(ProficientlyToVacantLoom));
        }
    }

    /// <summary>
    /// 每秒更新一次计数器 - 101计数器 和 时间间隔计数器
    /// </summary>
    private void BorrowChitin()
    {
        PikeInchRichFertile++;

        int relax_interval = int.Parse(MobTownEre.instance.FellowLoom.relax_interval);
        // 计时器阈值设置为0或负数时，关闭广告101；
        // 播放广告期间不计数；
        if (relax_interval <= 0 || UpAnimateTo)
        {
            return;
        }
        else
        {
            Luckily101++;
            if (Luckily101 >= relax_interval && !BouleRichProficiently)
            {
                DikeProficiently(101);
            }
        }
    }

    /// <summary>
    /// NoThanks插屏 - 102
    /// </summary>
    public void WeIgniteAxeImage(int customIndex = 0)
    {
        // 用户行为累计次数计数器阈值设置为0或负数时，关闭广告102
        int nextlevel_interval = int.Parse(MobTownEre.instance.FellowLoom.nextlevel_interval);
        if (nextlevel_interval <= 0)
        {
            return;
        }
        else
        {
            Luckily102 = SameLoomAnalyze.OwnNor("NoThanksCount") + 1;
            SameLoomAnalyze.FadNor("NoThanksCount", Luckily102);
            if (Luckily102 >= nextlevel_interval)
            {
                DikeProficiently(102, customIndex);
            }
        }
    }

    /// <summary>
    /// 前后台切换计数器 - 103
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            // 切回前台
            if (!UpAnimateTo)
            {
                // 前后台切换时，播放间隔计数器需要累加切到后台的时间
                if (InefficientHingeRationale > 0)
                {
                    PikeInchRichFertile += (int)(FoodDram.Seminar() - InefficientHingeRationale);
                    InefficientHingeRationale = 0;
                }
                // 后台切回前台累计次数，后台配置为0或负数，关闭该广告
                int inter_b2f_count = int.Parse(MobTownEre.instance.FellowLoom.inter_b2f_count);
                if (inter_b2f_count <= 0)
                {
                    return;
                }
                else
                {
                    Luckily103++;
                    if (Luckily103 >= inter_b2f_count)
                    {
                        DikeProficiently(103);
                    }
                }
            }
        }
        else
        {
            // 切到后台
            InefficientHingeRationale = FoodDram.Seminar();
        }
    }

    /// <summary>
    /// 暂停定时插屏播放 - 101
    /// </summary>
    public void HingeRichProficiently()
    {
        BouleRichProficiently = true;
    }

    /// <summary>
    /// 恢复定时插屏播放 - 101
    /// </summary>
    public void CowboyRichProficiently()
    {
        BouleRichProficiently = false;
    }

    /// <summary>
    /// 更新游戏的TrialNum
    /// </summary>
    /// <param name="num"></param>
    public void GalaxyVagueBow(int num)
    {
        SameLoomAnalyze.FadNor(CFellow.Dy_An_Spark_Peg, num);
    }

    /// <summary>
    /// 注册看广告的回调事件
    /// </summary>
    /// <param name="callback"></param>
    public void BlowholeInchDominant(Action<ADType> callback)
    {
        if (adVarietyEnjoyable == null)
        {
            adVarietyEnjoyable = new List<Action<ADType>>();
        }

        if (!adVarietyEnjoyable.Contains(callback))
        {
            adVarietyEnjoyable.Add(callback);
        }
    }

    /// <summary>
    /// 广告播放成功后，执行看广告回调事件
    /// </summary>
    private void AfterToInchBedroom(ADType adType)
    {
        UpAnimateTo = false;
        // 播放间隔计数器清零
        PikeInchRichFertile = 0;
        // 插屏计数器清零
        if (adType == ADType.Interstitial)
        {
            // 计数器清零
            if (PreparednessRate == 101)
            {
                Luckily101 = 0;
            }
            else if (PreparednessRate == 102)
            {
                Luckily102 = 0;
                SameLoomAnalyze.FadNor("NoThanksCount", 0);
            }
            else if (PreparednessRate == 103)
            {
                Luckily103 = 0;
            }
        }

        // 看广告总数+1
        SameLoomAnalyze.FadNor(CFellow.Dy_Compo_An_Peg + adType.ToString(), SameLoomAnalyze.OwnNor(CFellow.Dy_Compo_An_Peg + adType.ToString()) + 1);

        // 回调
        if (adVarietyEnjoyable != null && adVarietyEnjoyable.Count > 0)
        {
            foreach (Action<ADType> callback in adVarietyEnjoyable)
            {
                callback?.Invoke(adType);
            }
        }
    }

    /// <summary>
    /// 获取总的看广告次数
    /// </summary>
    /// <returns></returns>
    public int OwnGenreToBow(ADType adType)
    {
        return SameLoomAnalyze.OwnNor(CFellow.Dy_Compo_An_Peg + adType.ToString());
    }
}

public enum ADType { Interstitial, Rewarded }

[System.Serializable]
public class Ad_CustomData //广告自定义数据
{
    public string user_id; //用户id
    public string version; //版本号
    public string request_id; //请求id
    public string vendor; //渠道
    public string placement_id; //广告位id
}