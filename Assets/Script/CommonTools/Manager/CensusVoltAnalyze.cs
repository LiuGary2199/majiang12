using System;
using System.Collections;
using com.adjust.sdk;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CensusVoltAnalyze : MonoBehaviour
{
    public static CensusVoltAnalyze Instance;
[UnityEngine.Serialization.FormerlySerializedAs("adjustID")]
    public string MotionID;     // 由遇总的打包工具统一修改，无需手动配置

    //用户adjust 状态KEY
    private string Dy_ADLumpVoltRate= "sv_ADJustInitType";

    //adjust 时间戳
    private string Dy_ADLumpRich= "sv_ADJustTime";

    //adjust行为计数器
    public int _AcreageImage{ get; private set; }

    public double _AcreageAmplify{ get; private set; }

    double MotionVoltToAmplify= 0;


    private void Awake()
    {
        Instance = this;
        SameLoomAnalyze.FadChange(Dy_ADLumpRich, FoodDram.Seminar().ToString());

#if UNITY_IOS
        SameLoomAnalyze.FadChange(Dy_ADLumpVoltRate, AdjustStatus.OpenAsAct.ToString());
        CensusVolt();
#endif
    }

    private void Start()
    {
        _AcreageImage = 0;
    }


    void CensusVolt()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustConfig adjustConfig = new AdjustConfig(MotionID, AdjustEnvironment.Production, false);
        adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
        adjustConfig.setSendInBackground(false);
        adjustConfig.setEventBufferingEnabled(false);//SDK v5 中已删除该设置。
        adjustConfig.setLaunchDeferredDeeplink(true);
        Adjust.start(adjustConfig);
        //adjustConfig.LogLevel = AdjustLogLevel.Verbose;
        //adjustConfig.IsSendingInBackgroundEnabled = adjustConfig.IsSendingInBackgroundEnabled = true;
        //adjustConfig.IsDeferredDeeplinkOpeningEnabled = true;
        //Adjust.InitSdk(adjustConfig);
        StartCoroutine(SameCensusUpon());
    }

    private IEnumerator SameCensusUpon()
    {
        while (true)
        {
            string adjustAdid = Adjust.getAdid();
           // string adjustAdid = "";
            if (string.IsNullOrEmpty(adjustAdid))
            {
                //Adjust.GetAdid((adid) =>
                //{
                //    adjustAdid = adid;
                //});
                yield return new WaitForSeconds(5);
            }
            else
            {
                SameLoomAnalyze.FadChange(CFellow.Dy_CensusUpon, adjustAdid);
                MobTownEre.instance.ArabCensusUpon();
                yield break;
            }
        }
    }

    public string OwnCensusUpon()
    {
        return SameLoomAnalyze.OwnChange(CFellow.Dy_CensusUpon);
    }

    /// <summary>
    /// 获取adjust初始化状态
    /// </summary>
    /// <returns></returns>
    public string OwnCensusBoomer()
    {
        return SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate);
    }

    /*
     *  API
     *  Adjust 初始化
     */
    public void VoltCensusLoom(bool isOldUser = false)
    {
#if UNITY_IOS
        return;
#endif
        if (SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate) == "" && isOldUser)
        {
            EelTiltFad();
        }
        // 如果后台配置的adjust_init_act_position <= 0，直接初始化
        if (string.IsNullOrEmpty(MobTownEre.instance.FellowLoom.adjust_init_act_position) || int.Parse(MobTownEre.instance.FellowLoom.adjust_init_act_position) <= 0)
        {
            SameLoomAnalyze.FadChange(Dy_ADLumpVoltRate, AdjustStatus.OpenAsAct.ToString());
        }
        print(" user init adjust by status :" + SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate));
        //用户二次登录 根据标签初始化
        if (SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate) == AdjustStatus.OldUser.ToString() || SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate) == AdjustStatus.OpenAsAct.ToString())
        {
            print("second login  and  init adjust");
            CensusVolt();
        }
    }

    /*
    *  API
    *  标记老用户
    */
    public void EelTiltFad()
    {
        print("old user add adjust status");
        SameLoomAnalyze.FadChange(Dy_ADLumpVoltRate, AdjustStatus.OldUser.ToString());
        BaskTrialGerman.GetInstance().ArabTrial("1093", OwnCensusRich());
    }

    /*
     * API
     *  记录行为累计次数
     *  @param2 打点参数
     */
    public void AxeSeeImage(string param2 = "")
    {
#if UNITY_IOS
        return;
#endif
        if (SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate) != "") return;
        _AcreageImage++;
        print(" add up to :" + _AcreageImage);
        if (string.IsNullOrEmpty(MobTownEre.instance.FellowLoom.adjust_init_act_position) || _AcreageImage == int.Parse(MobTownEre.instance.FellowLoom.adjust_init_act_position))
        {
            TallCensusOfSee(param2);
        }
    }

    /// <summary>
    /// 记录广告行为累计次数，带广告收入
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="revenue"></param>
    public void AxeToImage(string countryCode, double revenue)
    {
#if UNITY_IOS
        return;
#endif
        //if (SameLoomAnalyze.GetString(sv_ADJustInitType) != "") return;

        _AcreageImage++;
        _AcreageAmplify += revenue;
        print(" Ads count: " + _AcreageImage + ", Revenue sum: " + _AcreageAmplify);

        //如果后台有adjust_init_adrevenue数据 且 能找到匹配的countryCode，初始化adjustInitAdRevenue
        if (!string.IsNullOrEmpty(MobTownEre.instance.FellowLoom.adjust_init_adrevenue))
        {
            JsonData jd = JsonMapper.ToObject(MobTownEre.instance.FellowLoom.adjust_init_adrevenue);
            if (jd.ContainsKey(countryCode))
            {
                MotionVoltToAmplify = double.Parse(jd[countryCode].ToString(), new System.Globalization.CultureInfo("en-US"));
            }
        }

        if (
            string.IsNullOrEmpty(MobTownEre.instance.FellowLoom.adjust_init_act_position)                   //后台没有配置限制条件，直接走LoadAdjust
            || (_AcreageImage == int.Parse(MobTownEre.instance.FellowLoom.adjust_init_act_position)         //累计广告次数满足adjust_init_act_position条件，且累计广告收入满足adjust_init_adrevenue条件，走LoadAdjust
                && _AcreageAmplify >= MotionVoltToAmplify)
        )
        {
            TallCensusOfSee();
        }
    }

    /*
     * API
     * 根据行为 初始化 adjust
     *  @param2 打点参数 
     */
    public void TallCensusOfSee(string param2 = "")
    {
        if (SameLoomAnalyze.OwnChange(Dy_ADLumpVoltRate) != "") return;

        // 根据比例分流   adjust_init_rate_act  行为比例
        if (string.IsNullOrEmpty(MobTownEre.instance.FellowLoom.adjust_init_rate_act) || int.Parse(MobTownEre.instance.FellowLoom.adjust_init_rate_act) > Random.Range(0, 100))
        {
            print("user finish  act  and  init adjust");
            SameLoomAnalyze.FadChange(Dy_ADLumpVoltRate, AdjustStatus.OpenAsAct.ToString());
            CensusVolt();

            // 上报点位 新用户达成 且 初始化
            BaskTrialGerman.GetInstance().ArabTrial("1091", OwnCensusRich(), param2);
        }
        else
        {
            print("user finish  act  and  not init adjust");
            SameLoomAnalyze.FadChange(Dy_ADLumpVoltRate, AdjustStatus.CloseAsAct.ToString());
            // 上报点位 新用户达成 且  不初始化
            BaskTrialGerman.GetInstance().ArabTrial("1092", OwnCensusRich(), param2);
        }
    }


    /*
     * API
     *  重置当前次数
     */
    public void FiftySeeImage()
    {
        print("clear current ");
        _AcreageImage = 0;
    }


    // 获取启动时间
    private string OwnCensusRich()
    {
        return FoodDram.Seminar() - long.Parse(SameLoomAnalyze.OwnChange(Dy_ADLumpRich)) + "";
    }
}


/*
 *@param
 *  OldUser     老用户
 *  OpenAsAct   行为触发且初始化
 *  CloseAsAct  行为触发不初始化
 */
public enum AdjustStatus
{
    OldUser,
    OpenAsAct,
    CloseAsAct
}