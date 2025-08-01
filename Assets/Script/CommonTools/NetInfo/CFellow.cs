/**
 * 
 * 常量配置
 * 
 * 
 * **/
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFellow
{
    #region 常量字段
    //登录url
    public const string HumidCow= "/api/client/user/getId?gameCode=";
    //配置url
    public const string FellowCow= "/api/client/config?gameCode=";
    //时间戳url
    public const string RichCow= "/api/client/common/current_timestamp?gameCode=";
    //更新AdjustId url
    public const string CensusCow= "/api/client/user/setAdjustId?gameCode=";
    #endregion

    #region 本地存储的字符串
    /// <summary>
    /// 本地用户id (string)
    /// </summary>
    public const string Dy_SullyTiltAt= "sv_LocalUserId";
    /// <summary>
    /// 本地服务器id (string)
    /// </summary>
    public const string Dy_SullyOxygenAt= "sv_LocalServerId";
    /// <summary>
    /// 是否是新用户玩家 (bool)
    /// </summary>
    public const string Dy_UpFoxPlayer= "sv_IsNewPlayer";
    public const string Dy_Wold_Gold_Tram_us= "sv_user_show_rate_us";


    /// <summary>
    /// 签到次数 (int)
    /// </summary>
    public const string Dy_FieldHoverOwnImage= "sv_DailyBounsGetCount";
    /// <summary>
    /// 签到最后日期 (int)
    /// </summary>
    public const string Dy_FieldHoverFood= "sv_DailyBounsDate";
    /// <summary>
    /// 新手引导完成的步数
    /// </summary>
    public const string Dy_FoxTiltDare= "sv_NewUserStep";
    /// <summary>
    /// 金币余额
    /// </summary>
    public const string Dy_BoldDumb= "sv_GoldCoin";
    /// <summary>
    /// 累计金币总数
    /// </summary>
    public const string Dy_AggressionBoldDumb= "sv_CumulativeGoldCoin";
    /// <summary>
    /// 钻石/现金余额
    /// </summary>
    public const string Dy_Flood= "sv_Token";
    /// <summary>
    /// 累计钻石/现金总数
    /// </summary>
    public const string Dy_AggressionFlood= "sv_CumulativeToken";
    /// <summary>
    /// 打点当前现金
    /// </summary>
    public const string Dy_StageArabFlood= "sv_PointCashToken";
    /// <summary>
    /// 钻石Amazon
    /// </summary>
    public const string Dy_Amazon= "sv_Amazon";
    /// <summary>
    /// 累计Amazon总数
    /// </summary>
    public const string Dy_AggressionMotion= "sv_CumulativeAmazon";
    /// <summary>
    /// 游戏总时长
    /// </summary>
    public const string Dy_GenreFadeRich= "sv_TotalGameTime";
    /// <summary>
    /// 第一次获得钻石奖励
    /// </summary>
    public const string Dy_YoungOwnFlood= "sv_FirstGetToken";
    /// <summary>
    /// 是否已显示评级弹框
    /// </summary>
    public const string Dy_RimBeadMoodStore= "sv_HasShowRatePanel";
    /// <summary>
    /// 累计Roblox奖券总数
    /// </summary>
    public const string Dy_AggressionCoaming= "sv_CumulativeLottery";
    /// <summary>
    /// 已经通过一次的关卡(int array)
    /// </summary>
    public const string Dy_NaturalEmitUranus= "sv_AlreadyPassLevels";
    /// <summary>
    /// 新手引导
    /// </summary>
    public const string Dy_FoxTiltDareAshcan= "sv_NewUserStepFinish";
    public const string Dy_Dual_Bring_Hardy= "sv_task_level_count";
    // 是否第一次使用过slot
    public const string Dy_YoungHold= "sv_FirstSlot";

        public const string Dy_YoungLysRadium= "sv_FirstLowReward";
    /// <summary>
    /// adjust adid
    /// </summary>
    public const string Dy_CensusUpon= "sv_AdjustAdid";

    /// <summary>
    /// 广告相关 - trial_num
    /// </summary>
    public const string Dy_An_Spark_Peg= "sv_ad_trial_num";
    /// <summary>
    /// 看广告总次数
    /// </summary>
    public const string Dy_Compo_An_Peg= "sv_total_ad_num";

    /// <summary>
    /// 存储天
    /// </summary>
    public static string Dy_SlaySpoutFoodGun= "sv_LastCheckDateKey";
    /// <summary>
    /// 天奖励
    /// </summary>
    public static string Dy_FieldRadium= "sv_DailyReward";

    #endregion

    #region 监听发送的消息

    /// <summary>
    /// 有窗口打开
    /// </summary>
    public static string Me_PointeYelp= "mg_WindowOpen";
    /// <summary>
    /// 窗口关闭
    /// </summary>
    public static string Me_PointeMedia= "mg_WindowClose";
    /// <summary>
    /// 关卡结算时传值
    /// </summary>
    public static string Me_So_Intentionally= "mg_ui_levelcomplete";
    /// <summary>
    /// 增加金币
    /// </summary>
    public static string Me_So_Arrange= "mg_ui_addgold";
    /// <summary>
    /// 增加钻石/现金
    /// </summary>
    public static string Me_So_Ordnance= "mg_ui_addtoken";
    /// <summary>
    /// 增加amazon
    /// </summary>
    public static string Me_So_Retentive= "mg_ui_addamazon";

    /// <summary>
    /// 游戏暂停/继续
    /// </summary>
    public static string Me_FadeReaumur= "mg_GameSuspend";

    /// <summary>
    /// 游戏资源数量变化
    /// </summary>
    public static string Me_CultPillow_= "mg_ItemChange_";

    /// <summary>
    /// 活动状态变更
    /// </summary>
    public static string Me_ChicopeeCranePillow_= "mg_ActivityStateChange_";

    /// <summary>
    /// 关卡最大等级变更
    /// </summary>
    public static string Me_TenthMudTenthPillow= "mg_LevelMaxLevelChange";

    /// <summary>
    /// combo
    /// </summary>
    public static string Me_OfClumpCheese= "mg_OnComboUpdatas";
    /// <summary>
    /// combo show
    /// </summary>
    public static string Me_OfClumpBead= "mg_OnComboShow";
    /// <summary>
    /// combo show
    /// </summary>
    public static string Me_OfAxeFavor= "mg_OnAddScore";
    /// <summary>
    /// combo show
    /// </summary>
    public static string Me_CheeseYewRadium= "mg_UpdataDayReward";

    #endregion

    #region 动态加载资源的路径

    // 金币图片
    public static string View_BoldDumb_Nitric= "Art/Tex/UI/jiangli1";
    // 钻石图片
    public static string View_Flood_Nitric_Tundra= "Art/Tex/UI/jiangli4";

    #endregion
}

