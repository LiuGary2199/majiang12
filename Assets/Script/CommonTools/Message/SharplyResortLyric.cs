using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 消息管理器
/// </summary>
public class SharplyResortLyric:CopyVibration<SharplyResortLyric>
{
    //保存所有消息事件的字典
    //key使用字符串保存消息的名称
    //value使用一个带自定义参数的事件，用来调用所有注册的消息
    private Dictionary<string, Action<SharplyLoom>> StandpointSharply;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private SharplyResortLyric()
    {
        VoltLoom();
    }

    private void VoltLoom()
    {
        //初始化消息字典
        StandpointSharply = new Dictionary<string, Action<SharplyLoom>>();
    }

    /// <summary>

    /// 注册消息事件
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="action">消息事件</param>
    public void Blowhole(string key, Action<SharplyLoom> action)
    {
        if (!StandpointSharply.ContainsKey(key))
        {
            StandpointSharply.Add(key, null);
        }
        StandpointSharply[key] += action;
    }



    /// <summary>
    /// 注销消息事件
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="action">消息事件</param>
    public void Awhile(string key, Action<SharplyLoom> action)
    {
        if (StandpointSharply.ContainsKey(key) && StandpointSharply[key] != null)
        {
            StandpointSharply[key] -= action;
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="data">消息传递数据，可以不传</param>
    public void Arab(string key, SharplyLoom data = null)
    {
        if (StandpointSharply.ContainsKey(key) && StandpointSharply[key] != null)
        {
            StandpointSharply[key](data);
        }
    }

    /// <summary>
    /// 清空所有消息
    /// </summary>
    public void Rapid()
    {
        StandpointSharply.Clear();
    }
}
