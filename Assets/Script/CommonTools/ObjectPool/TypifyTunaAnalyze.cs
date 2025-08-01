/*
 * 
 *  管理多个对象池的管理类
 * 
 * **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TypifyTunaAnalyze : CopyVibration<TypifyTunaAnalyze>
{
    //管理objectpool的字典
    private Dictionary<string, TypifyTuna> m_TunaPit;
    private Transform m_JuneMeteoroid=null;
    //构造函数
    public TypifyTunaAnalyze()
    {
        m_TunaPit = new Dictionary<string, TypifyTuna>();      
    }
    
    //创建一个新的对象池
    public T IntentTypifyTuna<T>(string poolName) where T : TypifyTuna, new()
    {
        if (m_TunaPit.ContainsKey(poolName))
        {
            return m_TunaPit[poolName] as T;
        }
        if (m_JuneMeteoroid == null)
        {
            m_JuneMeteoroid = this.transform;
        }      
        GameObject obj = new GameObject(poolName);
        obj.transform.SetParent(m_JuneMeteoroid);
        T pool = new T();
        pool.Volt(poolName, obj.transform);
        m_TunaPit.Add(poolName, pool);
        return pool;
    }
    //取对象
    public GameObject OwnFadeTypify(string poolName)
    {
        if (m_TunaPit.ContainsKey(poolName))
        {
            return m_TunaPit[poolName].Own();
        }
        return null;
    }
    //回收对象
    public void BurgessFadeTypify(string poolName,GameObject go)
    {
        if (m_TunaPit.ContainsKey(poolName))
        {
            m_TunaPit[poolName].Burgess(go);
        }
    }
    //销毁所有的对象池
    public void OnDestroy()
    {
        m_TunaPit.Clear();
        GameObject.Destroy(m_JuneMeteoroid);
    }
    /// <summary>
    /// 查询是否有该对象池
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public bool HyphaTuna(string poolName)
    {
        return m_TunaPit.ContainsKey(poolName) ? true : false;
    }
}
