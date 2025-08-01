using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//登录服务器返回数据
public class RootData 
{
    public int code { get; set; }
    public string msg { get; set; }
    public OxygenLoom data { get; set; }
}
//用户登录信息
public class ServerUserData
{
    public int code { get; set; }
    public string msg { get; set; }
    public int data { get; set; }
}
//服务器的数据
public class OxygenLoom
{
    public string init { get; set; }
    public string version { get; set; }
    public string CashOut_MoneyName { get; set; } //货币名称
    public string CashOut_Description { get; set; } //玩法描述
    public string convert_goal { get; set; } //兑换目标
    public string apple_pie { get; set; }
    public string inter_b2f_count { get; set; }
    public string inter_freq { get; set; }
    public string relax_interval { get; set; }
    public string trial_MaxNum { get; set; }
    public string nextlevel_interval { get; set; }
    public string adjust_init_rate_act { get; set; }
    public string adjust_init_act_position { get; set; }
    public string adjust_init_adrevenue { get; set; }
    public string game_data { get; set; }
    public string fall_down { get; set; }
    public string BlockRule { get; set; } //屏蔽规则
    public string skipLevels { get; set; } // 服务器返回的原始JSON字符串
    public int[] skipLevel
    { 
        get 
        {
            if (string.IsNullOrEmpty(skipLevels))
                return new int[0];
            
            try
            {
                // 尝试解析JSON数组
                return LitJson.JsonMapper.ToObject<int[]>(skipLevels);
            }
            catch
            {
                // 如果解析失败，返回空数组
                return new int[0];
            }
        }
    }

}
public class UserRootData
{
    public int code { get; set; }
    public string msg { get; set; }
    public string data { get; set; }
}

public class LocationData
{
    public double X;
    public double Y;
    public double Radius;
}

public class UserInfoData
{
    public double lat;
    public double lon;
    public string query; //ip地址
    public string regionName; //地区名称
    public string city; //城市名称
    public bool IsHaveApple; //是否有苹果
}

public class BlockRuleData //屏蔽规则
{
    public LocationData[] LocationList; //屏蔽位置列表
    public string[] CityList; //屏蔽城市列表
    public string[] IPList; //屏蔽IP列表
    public string fall_down; //自然量
    public bool BlockVPN; //屏蔽VPN
    public bool BlockSimulator; //屏蔽模拟器
    public bool BlockRoot; //屏蔽root
    public bool BlockDeveloper; //屏蔽开发者模式
    public bool BlockUsb; //屏蔽USB调试
    public bool BlockSimCard; //屏蔽SIM卡
}



public class GameData
{
    
    public int bubble_cd; // 气泡冷却时间（秒）
    public List<RewardData> bubbledatalist; // 气泡奖励列表
    public List<RewardData> mahjongdatalist; // 麻将奖励列表
    public List<RewardData> leveldatalist; // 等级奖励列表
    public List<RewardData> matchdatalist; // 比赛奖励列表
    public List<TimeRewardData> timeDataList; // 时长/广告奖励列表
    public List<RewardData> addatalist; // 广告奖励列表
    public int combommul; // 组合倍数
    public int combogold; // 组合金币数
    public int automatch; // 剩余自动收集

}

[Serializable] // 用于JsonUtility序列化/反序列化
public class RewardData
{
    public string type; // 奖励类型（如"Cash"）
    public int reward_num; // 奖励数量
}

[Serializable]
public class TimeRewardData : RewardData
{
    public int look_time; // 观看时长（秒）
    public int ad_num; // 广告数量
}

[Serializable]
public class DayRewardData : RewardData
{
    public int dataIndex;
    public int look_time; // 观看时长（秒）
    public int ad_num; // 广告数量
    public int look_num; // 广告数量
    public int getState; // 是否领取
}


public class Init
{
    public List<SlotItem> slot_group { get; set; }

    public double[] cash_random { get; set; }
    public MultiGroup[] cash_group { get; set; }
    public MultiGroup[] gold_group { get; set; }
    public MultiGroup[] amazon_group { get; set; }
}

public class SlotItem
{
    public int multi { get; set; }
    public int weight { get; set; }
}

public class MultiGroup
{
    public int max { get; set; }
    public int multi { get; set; }
}
