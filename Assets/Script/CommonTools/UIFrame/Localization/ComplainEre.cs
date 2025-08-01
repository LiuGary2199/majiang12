/*
 * 
 * 多语言
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplainEre 
{
    public static ComplainEre _Analogous;
    //语言翻译的缓存集合
    private Dictionary<string, string> _PitComplainTimer;

    private ComplainEre()
    {
        _PitComplainTimer = new Dictionary<string, string>();
        //初始化语言缓存集合
        VoltComplainTimer();
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    /// <returns></returns>
    public static ComplainEre GetInstance()
    {
        if (_Analogous == null)
        {
            _Analogous = new ComplainEre();
        }
        return _Analogous;
    }

    /// <summary>
    /// 得到显示文本信息
    /// </summary>
    /// <param name="lauguageId">语言id</param>
    /// <returns></returns>
    public string BeadSago(string lauguageId)
    {
        string strQueryResult = string.Empty;
        if (string.IsNullOrEmpty(lauguageId)) return null;
        //查询处理
        if(_PitComplainTimer!=null && _PitComplainTimer.Count >= 1)
        {
            _PitComplainTimer.TryGetValue(lauguageId, out strQueryResult);
            if (!string.IsNullOrEmpty(strQueryResult))
            {
                return strQueryResult;
            }
        }
        Debug.Log(GetType() + "/ShowText()/ Query is Null!  Parameter lauguageID: " + lauguageId);
        return null;
    }

    /// <summary>
    /// 初始化语言缓存集合
    /// </summary>
    private void VoltComplainTimer()
    {
        //LauguageJSONConfig_En
        //LauguageJSONConfig
        IFellowAnalyze config = new FellowAnalyzeMyTuna("LauguageJSONConfig");
        if (config != null)
        {
            _PitComplainTimer = config.SeaIngoing;
        }
    }
}
