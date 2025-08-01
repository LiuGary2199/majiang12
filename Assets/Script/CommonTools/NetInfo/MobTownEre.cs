/***
 * 
 * 
 * 网络信息控制
 * 
 * **/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Runtime.InteropServices;
using com.adjust.sdk;
//using AdjustSdk;
//using MoreMountains.NiceVibrations;

public class MobTownEre : MonoBehaviour
{
[HideInInspector] [UnityEngine.Serialization.FormerlySerializedAs("DataFrom")]public string LoomEach; //数据来源 打点用
    public static MobTownEre instance;
    //请求超时时间
    private static float TIMEOUT= 3f;
[UnityEngine.Serialization.FormerlySerializedAs("ComeCow")]    //base
    public string ComeCow;
[UnityEngine.Serialization.FormerlySerializedAs("BaseLoginUrl")]    //登录url
    public string ComeHumidCow;
[UnityEngine.Serialization.FormerlySerializedAs("BaseConfigUrl")]    //配置url
    public string ComeFellowCow;
[UnityEngine.Serialization.FormerlySerializedAs("BaseTimeUrl")]    //时间戳url
    public string ComeRichCow;
[UnityEngine.Serialization.FormerlySerializedAs("BaseAdjustUrl")]    //更新AdjustId url
    public string ComeCensusCow;
[UnityEngine.Serialization.FormerlySerializedAs("GameCode")]    //后台gamecode
    public string FadeCash= "20000";
[UnityEngine.Serialization.FormerlySerializedAs("Channel")]
    //channel渠道平台
#if UNITY_IOS
    public string Nonself= "IOS";
#elif UNITY_ANDROID
    public string Channel = "GooglePlay";
#else
    public string Channel = "Other";
#endif
    //工程包名
    private string ReserveWhig{ get { return Application.identifier; } }
    //登录url
    private string HumidCow= "";
    //配置url
    private string FellowCow= "";
    //更新AdjustId url
    private string CensusCow= "";
[UnityEngine.Serialization.FormerlySerializedAs("country")]    //国家
    public string Artiste= "";
[UnityEngine.Serialization.FormerlySerializedAs("ConfigData")]    //服务器Config数据
    public OxygenLoom FellowLoom;
[UnityEngine.Serialization.FormerlySerializedAs("GameData")]    public GameData FadeLoom;
[UnityEngine.Serialization.FormerlySerializedAs("InitData")]    //游戏内数据
    public Init VoltLoom;
[UnityEngine.Serialization.FormerlySerializedAs("adManager")]    //ADAnalyze
    public GameObject AnAnalyze;
    [HideInInspector]
[UnityEngine.Serialization.FormerlySerializedAs("gaid")]    public string Anti;
    [HideInInspector]
[UnityEngine.Serialization.FormerlySerializedAs("aid")]    public string Lug;
    [HideInInspector]
[UnityEngine.Serialization.FormerlySerializedAs("idfa")]    public string Jade;
    int Proof_Hardy= 0;
[UnityEngine.Serialization.FormerlySerializedAs("ready")]    public bool Proof= false;
[UnityEngine.Serialization.FormerlySerializedAs("BlockRule")]    public BlockRuleData LightThin;
   // ios 获取idfa函数声明
#if UNITY_IOS
        [DllImport("__Internal")]
        internal extern static void getIDFA();
#endif
    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 240;
        HumidCow = ComeHumidCow + FadeCash + "&channel=" + Nonself + "&version=" + Application.version;
        FellowCow = ComeFellowCow + FadeCash + "&channel=" + Nonself + "&version=" + Application.version;
        CensusCow = ComeCensusCow + FadeCash;
    }
    private void Start()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass aj = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject p = aj.GetStatic<AndroidJavaObject>("currentActivity");
            p.Call("getGaid");
            p.Call("getAid");

        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS
            getIDFA();
            string idfv = UnityEngine.iOS.Device.vendorIdentifier;
            SameLoomAnalyze.FadChange("idfv", idfv);
#endif
        }
        else
        {
            Humid();           //编辑器登录
        }
        //获取config数据
        OwnFellowLoom();
    }

    /// <summary>
    /// 获取gaid回调
    /// </summary>
    /// <param name="gaid_str"></param>
    public void AntiEocene(string gaid_str)
    {
        Debug.Log("unity收到gaid：" + gaid_str);
        Anti = gaid_str; 
        if (Anti == null || Anti == "")
        {
            Anti = SameLoomAnalyze.OwnChange("gaid");
        }
        else
        {
            SameLoomAnalyze.FadChange("gaid", Anti);
        }
        Proof_Hardy++;
        if (Proof_Hardy == 2)
        {
            Humid();
        }
    }
    /// <summary>
    /// 获取aid回调
    /// </summary>
    /// <param name="aid_str"></param>
    public void LugEocene(string aid_str)
    {
        Debug.Log("unity收到aid：" + aid_str);
        Lug = aid_str;
        if (Lug == null || Lug == "")
        {
            Lug = SameLoomAnalyze.OwnChange("aid");
        }
        else
        {
            SameLoomAnalyze.FadChange("aid", Lug);
        }
        Proof_Hardy++;
        if (Proof_Hardy == 2)
        {
            Humid();
        }
    }
    /// <summary>
    /// 获取idfa成功
    /// </summary>
    /// <param name="message"></param>
    public void JadeBedroom(string message)
    {
        Debug.Log("idfa success:" + message);
        Jade = message;
        SameLoomAnalyze.FadChange("idfa", Jade);
        Humid();
    }
    /// <summary>
    /// 获取idfa失败
    /// </summary>
    /// <param name="message"></param>
    public void JadeHusk(string message)
    {
        Debug.Log("idfa fail");
        Jade = SameLoomAnalyze.OwnChange("idfa");
        Humid();
    }
    /// <summary>
    /// 登录
    /// </summary>
    public void Humid()
    {
        CashOutManager.GetInstance().Login();
        //获取本地缓存的Local用户ID
        string localId = SameLoomAnalyze.OwnChange(CFellow.Dy_SullyTiltAt);

        //没有用户ID，视为新用户，生成用户ID
        if (localId == "" || localId.Length == 0)
        {
            //生成用户随机id
            TimeSpan st = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            string timeStr = Convert.ToInt64(st.TotalSeconds).ToString() + UnityEngine.Random.Range(0, 10).ToString() + UnityEngine.Random.Range(1, 10).ToString() + UnityEngine.Random.Range(1, 10).ToString() + UnityEngine.Random.Range(1, 10).ToString();
            localId = timeStr;
            SameLoomAnalyze.FadChange(CFellow.Dy_SullyTiltAt, localId);
        }

        //拼接登录接口参数
        string url = "";
        if (Application.platform == RuntimePlatform.IPhonePlayer)       //一个参数 - iOS
        {
            url = HumidCow + "&" + "randomKey" + "=" + localId + "&idfa=" + Jade + "&packageName=" + ReserveWhig;
        }
        else if (Application.platform == RuntimePlatform.Android)  //两个参数 - Android
        {
            url = HumidCow + "&" + "randomKey" + "=" + localId + "&gaid=" + Anti + "&androidId=" + Lug + "&packageName=" + ReserveWhig;
        }
        else //编辑器
        {
            url = HumidCow + "&" + "randomKey" + "=" + localId + "&packageName=" + ReserveWhig;
        }

        //获取国家信息
        MixMermaid(() => {
            url += "&country=" + Artiste;
            //登录请求
            MobAutoAnalyze.GetInstance().BondOwn(url,
                (data) => {
                    Debug.Log("Login 成功" + data.downloadHandler.text);
                    SameLoomAnalyze.FadChange("init_time", DateTime.Now.ToString());
                    ServerUserData serverUserData = JsonMapper.ToObject<ServerUserData>(data.downloadHandler.text);
                    SameLoomAnalyze.FadChange(CFellow.Dy_SullyOxygenAt, serverUserData.data.ToString());

                    ArabCensusUpon();
                    if (PlayerPrefs.GetInt("SendedEvent") != 1 && !String.IsNullOrEmpty(FactorDram.DareMat))
                        FactorDram.ArabTrial();
                },
                () => {
                    Debug.Log("Login 失败");
                });
        });
    }
    /// <summary>
    /// 获取国家
    /// </summary>
    /// <param name="cb"></param>
    private void MixMermaid(Action cb)
    {
        bool callBackReady = false;
        if (String.IsNullOrEmpty(Artiste))
        {
            //获取国家超时返回
            StartCoroutine(MobAutoRichBut(() =>
            {
                if (!callBackReady)
                {
                    Artiste = "";
                    callBackReady = true;
                    cb?.Invoke();
                }
            }));
            MobAutoAnalyze.GetInstance().BondOwn("https://a.mafiagameglobal.com/event/country/", (data) =>
            {
                Artiste = JsonMapper.ToObject<Dictionary<string, string>>(data.downloadHandler.text)["country"];
                Debug.Log("获取国家 成功:" + Artiste);
                if (!callBackReady)
                {
                    callBackReady = true;
                    cb?.Invoke();
                }
            },
            () => {
                Debug.Log("获取国家 失败");
                if (!callBackReady)
                {
                    Artiste = "";
                    callBackReady = true;
                    cb?.Invoke();
                }
            });
        }
        else
        {
            if (!callBackReady)
            {
                callBackReady = true;
                cb?.Invoke();
            }
        }
    }

    /// <summary>
    /// 获取服务器Config数据
    /// </summary>
    private void OwnFellowLoom()
    {
        Debug.Log("GetConfigData:" + FellowCow);
        StartCoroutine(MobAutoRichBut(() =>
        {
            OwnGathererLoom();
        }));

        //获取并存入Config
        MobAutoAnalyze.GetInstance().BondOwn(FellowCow,
        (data) => {
            LoomEach = "OnlineData";
            Debug.Log("ConfigData 成功" + data.downloadHandler.text);
            SameLoomAnalyze.FadChange("OnlineData", data.downloadHandler.text);
            FadFellowLoom(data.downloadHandler.text);
        },
        () => {
            OwnGathererLoom();
            Debug.Log("ConfigData 失败");
        });
    }

    /// <summary>
    /// 获取本地Config数据
    /// </summary>
    private void OwnGathererLoom()
    {
        //是否有缓存
        if (SameLoomAnalyze.OwnChange("OnlineData") == "" || SameLoomAnalyze.OwnChange("OnlineData").Length == 0)
        {
            Debug.Log("本地数据");
            LoomEach = "LocalData_Updated"; //已联网更新过的数据
            TextAsset json = Resources.Load<TextAsset>("LocationJson/LocationData");
            FadFellowLoom(json.text);
        }
        else
        {
            LoomEach = "LocalData_Original"; //原始数据
            Debug.Log("服务器缓存数据");
            FadFellowLoom(SameLoomAnalyze.OwnChange("OnlineData"));
        }
    }


    /// <summary>
    /// 解析config数据
    /// </summary>
    /// <param name="configJson"></param>
    void FadFellowLoom(string configJson)
    {
        //如果已经获得了数据则不再处理
        if (FellowLoom == null)
        {
            RootData rootData = JsonMapper.ToObject<RootData>(configJson);
            FellowLoom = rootData.data;
            VoltLoom = JsonMapper.ToObject<Init>(FellowLoom.init);
            FadeLoom = JsonMapper.ToObject<GameData>(FellowLoom.game_data);
            if (!string.IsNullOrEmpty(FellowLoom.BlockRule))
                LightThin = JsonMapper.ToObject<BlockRuleData>(FellowLoom.BlockRule);
            OwnTiltTown();
        }
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    void FadeRound()
    {
        ClanAnalyze.instance.Legitimately();
        //打开admanager
        AnAnalyze.SetActive(true);
        //进度条可以继续
        Proof = true;
    }



    /// <summary>
    /// 超时处理
    /// </summary>
    /// <param name="finish"></param>
    /// <returns></returns>
    IEnumerator MobAutoRichBut(Action finish)
    {
        yield return new WaitForSeconds(TIMEOUT);
        finish();
    }

    /// <summary>
    /// 向后台发送adjustId
    /// </summary>
    public void ArabCensusUpon()
    {
        string serverId = SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt);
        string adjustId = CensusVoltAnalyze.Instance.OwnCensusUpon();
        if (string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(adjustId))
        {
            return;
        }

        string url = CensusCow + "&serverId=" + serverId + "&adid=" + adjustId;
        MobAutoAnalyze.GetInstance().BondOwn(url,
            (data) => {
                Debug.Log("服务器更新adjust adid 成功" + data.downloadHandler.text);
            },
            () => {
                Debug.Log("服务器更新adjust adid 失败");
            });
        CashOutManager.GetInstance().ReportAdjustID();
    }
[UnityEngine.Serialization.FormerlySerializedAs("UserDataStr")]    //暂时去掉，屏蔽规则不再处理归因信息
    //轮询检查Adjust归因信息
    // int CheckCount = 0;
    // [HideInInspector] public string Event_TrackerName; //打点用参数
    // bool _CheckOk = false;
    // [HideInInspector]
    // public bool AdjustTracker_Ready //是否成功获取到归因信息 
    // {
    //     get
    //     {
    //         if (Application.isEditor) //编译器跳过检查
    //             return true;
    //         return _CheckOk;
    //     }
    // }
    // public void CheckAdjustNetwork() //检查Adjust归因信息
    // {
    //     if (Application.isEditor) //编译器跳过检查
    //         return;
    //     if (!string.IsNullOrEmpty(Event_TrackerName)) //已经拿到归因信息
    //         return;

    //     CancelInvoke(nameof(CheckAdjustNetwork));
    //     if (!string.IsNullOrEmpty(BlockRule.fall_down) && BlockRule.fall_down == "fall")
    //     {
    //         print("Adjust 无归因相关配置或未联网 跳过检查");
    //         _CheckOk = true;
    //     }
    //     try
    //     {
    //         AdjustAttribution Info = Adjust.getAttribution();
    //         print("Adjust 获取信息成功 归因渠道：" + Info.trackerName);
    //         Event_TrackerName = "TrackerName: " + Info.trackerName;
    //         FactorDram.Adjust_TrackerName = Info.trackerName;
    //         _CheckOk = true;
    //     }
    //     catch (System.Exception e)
    //     {
    //         CheckCount++;
    //         Debug.Log("Adjust 获取信息失败：" + e.Message + " 重试次数：" + CheckCount);
    //         if (CheckCount >= 10)
    //             _CheckOk = true;
    //         Invoke(nameof(CheckAdjustNetwork), 1);
    //     }
    // }


    //获取用户信息
    public string TiltLoomOwe= "";
[UnityEngine.Serialization.FormerlySerializedAs("UserData")]    public UserInfoData TiltLoom;
    int OwnTiltTownImage= 0;
    void OwnTiltTown()
    {
        //还有进入正常模式的可能
        if (PlayerPrefs.HasKey("OtherChance") && PlayerPrefs.GetString("OtherChance") == "YES")
            PlayerPrefs.DeleteKey("Save_AP");
        //已经记录过用户信息 跳过检查
        if (PlayerPrefs.HasKey("OtherChance") && PlayerPrefs.GetString("OtherChance") == "NO")
        {
            FadeRound();
            return;
        }


        //检查归因渠道信息
        //CheckAdjustNetwork();
        //获取用户信息
        string CheckUrl = ComeCow + "/api/client/user/checkUser";
        MobAutoAnalyze.GetInstance().BondOwn(CheckUrl,
        (data) =>
        {
            TiltLoomOwe = data.downloadHandler.text;
            print("+++++ 获取用户数据 成功" + TiltLoomOwe);
            UserRootData rootData = JsonMapper.ToObject<UserRootData>(TiltLoomOwe);
            TiltLoom = JsonMapper.ToObject<UserInfoData>(rootData.data);
            if (TiltLoomOwe.Contains("apple")
            || TiltLoomOwe.Contains("Apple")
            || TiltLoomOwe.Contains("APPLE"))
                TiltLoom.IsHaveApple = true;
            FadeRound();
        }, () => { });
        Invoke(nameof(AxOwnTiltTown), 1);
    }
    void AxOwnTiltTown()
    {
        if (!Proof)
        {
            OwnTiltTownImage++;
            if (OwnTiltTownImage < 10)
            {
                print("+++++ 获取用户数据失败 重试： " + OwnTiltTownImage);
                OwnTiltTown();
            }
            else
            {
                print("+++++ 获取用户数据 失败次数过多，放弃");
                FadeRound();
            }
        }
    }
}
