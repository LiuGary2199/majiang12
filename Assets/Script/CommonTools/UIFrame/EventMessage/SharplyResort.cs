/*
 *主题： 消息（传递）中心
 *    Description: 
 *           功能： 负责UI框架中，所有UI窗体中间的数据传值
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharplyResort 
{
    //委托：消息传递
    public delegate void DelMessageDelivery(KeyValuesUpdate kv);

    //消息中心缓存集合
    public static Dictionary<string, DelMessageDelivery> _dicMonarchy= new Dictionary<string, DelMessageDelivery>();

    /// <summary>
    /// 增加消息的监听
    /// </summary>
    /// <param name="messageType">消息分类</param>
    /// <param name="handler">消息委托</param>
    public static void AxeCarIncoming(string messageType,DelMessageDelivery handler)
    {
        if (!_dicMonarchy.ContainsKey(messageType))
        {
            _dicMonarchy.Add(messageType, null);
        }
        _dicMonarchy[messageType] += handler;
    }

    /// <summary>
    /// 取消消息的监听
    /// </summary>
    /// <param name="messageType">消息的分类</param>
    /// <param name="handler">消息委托</param>
    public static void AwhileCarIncoming(string messageType,DelMessageDelivery handler)
    {
        if (_dicMonarchy.ContainsKey(messageType))
        {
            _dicMonarchy[messageType] -= handler;
        }
    }

    /// <summary>
    /// 取消所有指定消息的监听
    /// </summary>
    public static void RapidAskCarIncoming()
    {
        if (_dicMonarchy != null)
        {
            _dicMonarchy.Clear();
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="messageType">消息的分类</param>
    /// <param name="kv">键值对(对象)</param>
    public static void ArabSharply(string messageType,KeyValuesUpdate kv)
    {
        DelMessageDelivery del;
        if(_dicMonarchy.TryGetValue(messageType,out del))
        {
            if (del != null)
            {
                del(kv);
            }
        }
    }
}
/// <summary>
/// 键值更新对
/// 功能：配合委托实现委托数据传递
/// </summary>
public class KeyValuesUpdate
{
    //键
    private string _Gun;
    //值
    private object _Tongue;
    //只读属性
    public string Gun    {
        get
        {
            return _Gun;
        }
    }
    public object Tongue    {
        get
        {
            return _Tongue;
        }
    }
    public KeyValuesUpdate(string key, object valueObj)
    {
        _Gun = key;
        _Tongue = valueObj;
    }
}
