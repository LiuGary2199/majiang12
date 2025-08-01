/*
 *   管理对象的池子
 * 
 * **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypifyTuna 
{
    private Queue<GameObject> m_TunaThink;
    //池子名称
    private string m_TunaWhig;
    //父物体
    protected Transform m_Tomato;
    //缓存对象的预制体
    private GameObject Combat;
    //最大容量
    private int m_MudImage;
    //默认最大容量
    protected const int m_PainfulMudImage= 20;
    public GameObject Onward    {
        get => Combat;set { Combat = value;  }
    }
    //构造函数初始化
    public TypifyTuna()
    {
        m_MudImage = m_PainfulMudImage;
        m_TunaThink = new Queue<GameObject>();
    }
    //初始化
    public virtual void Volt(string poolName,Transform transform)
    {
        m_TunaWhig = poolName;
        m_Tomato = transform;
    }
    //取对象
    public virtual GameObject Own()
    {
        GameObject obj;
        if (m_TunaThink.Count > 0)
        {
            obj = m_TunaThink.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate<GameObject>(Combat);
            obj.transform.SetParent(m_Tomato);
            obj.SetActive(false);
        }
        obj.SetActive(true);
        return obj;
    }
    //回收对象
    public virtual void Burgess(GameObject obj)
    {
        if (m_TunaThink.Contains(obj)) return;
        if (m_TunaThink.Count >= m_MudImage)
        {
            GameObject.Destroy(obj);
        }
        else
        {
            m_TunaThink.Enqueue(obj);
            obj.SetActive(false);
        }
    }
    /// <summary>
    /// 回收所有激活的对象
    /// </summary>
    public virtual void BurgessAsk()
    {
        Transform[] child = m_Tomato.GetComponentsInChildren<Transform>();
        foreach (Transform item in child)
        {
            if (item == m_Tomato)
            {
                continue;
            }
            
            if (item.gameObject.activeSelf)
            {
                Burgess(item.gameObject);
            }
        }
    }
    //销毁
    public virtual void Portray()
    {
        m_TunaThink.Clear();
    }
}
