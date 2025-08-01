using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorDram
{
    [HideInInspector] public static string Census_FlattenWhig; //归因渠道名称 由MobTownEre的CheckAdjustNetwork方法赋值
    static string Same_AP; //ApplePie的本地存档 存储第一次进入状态 未来不再受ApplePie开关影响
    static string FollowModeWhig= "pie"; //正常模式名称
    static string Generator; //距离黑名单位置的距离 打点用
    static string Injure; //进审理由 打点用
    [HideInInspector] public static string DareMat= ""; //判断流程 打点用

    public static bool UpHonor()
    {
        //测试
        // return true;

        if (PlayerPrefs.HasKey("Save_AP"))  //优先使用本地存档
            Same_AP = PlayerPrefs.GetString("Save_AP");
        if (string.IsNullOrEmpty(Same_AP)) //无本地存档 读取网络数据
            SpoutPotterLoom();

        if (Same_AP != "P")
            return true;
        else
            return false;
    }
    public static void SpoutPotterLoom() //读取网络数据 判断进入哪种游戏模式
    {
        string OtherChance = "NO"; //进审之后 是否还有可能变正常
        Same_AP = "P";
        if (MobTownEre.instance.FellowLoom.apple_pie != FollowModeWhig) //审模式 
        {
            OtherChance = "YES";
            Same_AP = "A";
            if (string.IsNullOrEmpty(Injure))
                Injure = "ApplePie";
        }
        DareMat = "0:" + Same_AP;
        //判断运营商信息
        if (MobTownEre.instance.TiltLoom != null && MobTownEre.instance.TiltLoom.IsHaveApple)
        {
            Same_AP = "A";
            if (string.IsNullOrEmpty(Injure))
                Injure = "HaveApple";
            DareMat += "1:" + Same_AP;
        }
        if (MobTownEre.instance.LightThin != null)
        {
            //判断经纬度
            LocationData[] LocationDatas = MobTownEre.instance.LightThin.LocationList;
            if (LocationDatas != null && LocationDatas.Length > 0 && MobTownEre.instance.TiltLoom != null && MobTownEre.instance.TiltLoom.lat != 0 && MobTownEre.instance.TiltLoom.lon != 0)
            {
                for (int i = 0; i < LocationDatas.Length; i++)
                {
                    float Distance = OwnMonument((float)LocationDatas[i].X, (float)LocationDatas[i].Y,
                    (float)MobTownEre.instance.TiltLoom.lat, (float)MobTownEre.instance.TiltLoom.lon);
                    Generator += Distance.ToString() + ",";
                    if (Distance <= LocationDatas[i].Radius)
                    {
                        Same_AP = "A";
                        if (string.IsNullOrEmpty(Injure))
                            Injure = "Location";
                        break;
                    }
                }
            }
            DareMat += "2:" + Same_AP;
            //判断城市
            string[] HeiCityList = MobTownEre.instance.LightThin.CityList;
            if (!string.IsNullOrEmpty(MobTownEre.instance.TiltLoom.regionName) && HeiCityList != null && HeiCityList.Length > 0)
            {
                for (int i = 0; i < HeiCityList.Length; i++)
                {
                    if (HeiCityList[i] == MobTownEre.instance.TiltLoom.regionName
                    || HeiCityList[i] == MobTownEre.instance.TiltLoom.city)
                    {
                        Same_AP = "A";
                        if (string.IsNullOrEmpty(Injure))
                            Injure = "City";
                        break;
                    }
                }
            }
            DareMat += "3:" + Same_AP;
            //判断黑名单
            string[] HeiIPs = MobTownEre.instance.LightThin.IPList;
            if (HeiIPs != null && HeiIPs.Length > 0 && !string.IsNullOrEmpty(MobTownEre.instance.TiltLoom.query))
            {
                string[] IpNums = MobTownEre.instance.TiltLoom.query.Split('.');
                for (int i = 0; i < HeiIPs.Length; i++)
                {
                    string[] HeiIpNums = HeiIPs[i].Split('.');
                    bool isMatch = true;
                    for (int j = 0; j < HeiIpNums.Length; j++) //黑名单IP格式可能是任意位数 根据位数逐个比对
                    {
                        if (HeiIpNums[j] != IpNums[j])
                            isMatch = false;
                    }
                    if (isMatch)
                    {
                        Same_AP = "A";
                        if (string.IsNullOrEmpty(Injure))
                            Injure = "IP";
                        break;
                    }
                }
            }
            DareMat += "4:" + Same_AP;
        }
        //判断自然量
        if (!string.IsNullOrEmpty(MobTownEre.instance.LightThin.fall_down))
        {
            // if (MobTownEre.instance.BlockRule.fall_down == "bottom") //仅判断Organic
            // {
            //     if (Adjust_TrackerName == "Organic") //打开自然量 且 归因渠道是Organic 审模式
            //     {
            //         Save_AP = "A";
            //         if (string.IsNullOrEmpty(Reason))
            //             Reason = "FallDown";
            //     }
            // }
            // else if (MobTownEre.instance.BlockRule.fall_down == "down") //判断Organic + NoUserConsent
            // {
            //     if (Adjust_TrackerName == "Organic" || Adjust_TrackerName == "No User Consent") //打开自然量 且 归因渠道是Organic或NoUserConsent 审模式
            //     {
            //         Save_AP = "A";
            //         if (string.IsNullOrEmpty(Reason))
            //             Reason = "FallDown";
            //     }
            // }
        }
        DareMat += "5:" + Same_AP;

        //安卓平台特殊屏蔽策略
        if (Application.platform == RuntimePlatform.Android && MobTownEre.instance.LightThin != null)
        {
            AndroidJavaClass aj = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject p = aj.GetStatic<AndroidJavaObject>("currentActivity");

            //判断是否使用VPN
            if (MobTownEre.instance.LightThin.BlockVPN)
            {
                bool isVpnConnected = p.CallStatic<bool>("isVpn");
                if (isVpnConnected)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "VPN";
                }
            }
            DareMat += "6:" + Same_AP;

            //是否使用模拟器
            if (MobTownEre.instance.LightThin.BlockSimulator)
            {
                bool isSimulator = p.CallStatic<bool>("isSimulator");
                if (isSimulator)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "Simulator";
                }
            }
            DareMat += "7:" + Same_AP;
            //是否root
            if (MobTownEre.instance.LightThin.BlockRoot)
            {
                bool isRoot = p.CallStatic<bool>("isRoot");
                if (isRoot)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "Root";
                }
            }
            DareMat += "8:" + Same_AP;
            //是否使用开发者模式
            if (MobTownEre.instance.LightThin.BlockDeveloper)
            {
                bool isDeveloper = p.CallStatic<bool>("isDeveloper");
                if (isDeveloper)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "Developer";
                }
            }
            DareMat += "9:" + Same_AP;

            //是否使用USB调试
            if (MobTownEre.instance.LightThin.BlockUsb)
            {
                bool isUsb = p.CallStatic<bool>("isUsb");
                if (isUsb)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "UsbDebug";
                }
            }
            DareMat += "10:" + Same_AP;

            //是否使用sim卡
            if (MobTownEre.instance.LightThin.BlockSimCard)
            {
                bool isSimCard = p.CallStatic<bool>("isSimcard");
                if (!isSimCard)
                {
                    Same_AP = "A";
                    if (string.IsNullOrEmpty(Injure))
                        Injure = "SimCard";
                }
            }
            DareMat += "11:" + Same_AP;
        }
        PlayerPrefs.SetString("Save_AP", Same_AP);
        PlayerPrefs.SetString("OtherChance", OtherChance);

        //打点
        if (!string.IsNullOrEmpty(SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt)))
            ArabTrial();
    }
    static float OwnMonument(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371f; // 地球半径，单位：公里
        float latDistance = Mathf.Deg2Rad * (lat2 - lat1);
        float lonDistance = Mathf.Deg2Rad * (lon2 - lon1);
        float a = Mathf.Sin(latDistance / 2) * Mathf.Sin(latDistance / 2)
               + Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2)
               * Mathf.Sin(lonDistance / 2) * Mathf.Sin(lonDistance / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c * 1000; // 距离，单位：米
    }

    public static void ArabTrial()
    {
        //打点
        if (MobTownEre.instance.TiltLoom != null)
        {
            string Info1 = "[" + (Same_AP == "A" ? "审" : "正常") + "] [" + Injure + "]";
            string Info2 = "[" + MobTownEre.instance.TiltLoom.lat + "," + MobTownEre.instance.TiltLoom.lon + "] [" + MobTownEre.instance.TiltLoom.regionName + "] [" + Generator + "]";
            string Info3 = "[" + MobTownEre.instance.TiltLoom.query + "] [Null]";  // [" + Adjust_TrackerName + "]";
            BaskTrialGerman.GetInstance().ArabTrial("3000", Info1, Info2, Info3);
        }
        else
            BaskTrialGerman.GetInstance().ArabTrial("3000", "No UserData");
        BaskTrialGerman.GetInstance().ArabTrial("3001", (Same_AP == "A" ? "审" : "正常"), DareMat, MobTownEre.instance.LoomEach);
        PlayerPrefs.SetInt("SendedEvent", 1);
    }



    public static bool UpDegree()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 是否为竖屏
    /// </summary>
    /// <returns></returns>
    public static bool UpIllusory()
    {
        return Screen.height > Screen.width;
    }

    /// <summary>
    /// UI的本地坐标转为屏幕坐标
    /// </summary>
    /// <param name="tf"></param>
    /// <returns></returns>
    public static Vector2 SullyStage2RejoinStage(RectTransform tf)
    {
        if (tf == null)
        {
            return Vector2.zero;
        }

        Vector2 fromPivotDerivedOffset = new Vector2(tf.rect.width * 0.5f + tf.rect.xMin, tf.rect.height * 0.5f + tf.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, tf.position);
        screenP += fromPivotDerivedOffset;
        return screenP;
    }


    /// <summary>
    /// UI的屏幕坐标，转为本地坐标
    /// </summary>
    /// <param name="tf"></param>
    /// <param name="startPos"></param>
    /// <returns></returns>
    public static Vector2 RejoinStage2SullyStage(RectTransform tf, Vector2 startPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(tf, startPos, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(tf.rect.width * 0.5f + tf.rect.xMin, tf.rect.height * 0.5f + tf.rect.yMin);
        return tf.anchoredPosition + localPoint - pivotDerivedOffset;
    }

    public static Vector2 OwnShadeTurnpikeOfFleeMeteoroid(RectTransform rectTransform)
    {
        // 从RectTransform开始，逐级向上遍历父级
        Vector2 worldPosition = rectTransform.anchoredPosition;
        for (RectTransform rt = rectTransform; rt != null; rt = rt.parent as RectTransform)
        {
            worldPosition += new Vector2(rt.localPosition.x, rt.localPosition.y);
            worldPosition += rt.pivot * rt.sizeDelta;

            // 考虑到UI元素的缩放
            worldPosition *= rt.localScale;

            // 如果父级不是Canvas，则停止遍历
            if (rt.parent != null && rt.parent.GetComponent<Canvas>() == null)
                break;
        }

        // 将结果从本地坐标系转换为世界坐标系
        return rectTransform.root.TransformPoint(worldPosition);
    }
}
