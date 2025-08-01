/*
 *     主题： 事件触发监听      
 *    Description: 
 *           功能： 实现对于任何对象的监听处理。
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrialRemodelIncoming : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onNever;
    public VoidDelegate OxLust;
    public VoidDelegate OxMarsh;
    public VoidDelegate OxKind;
    public VoidDelegate OxHe;
    public VoidDelegate OxCanada;
    public VoidDelegate OxGalaxyCanada;

    /// <summary>
    /// 得到监听器组件
    /// </summary>
    /// <param name="go">监听的游戏对象</param>
    /// <returns></returns>
    public static TrialRemodelIncoming Own(GameObject go)
    {
        TrialRemodelIncoming listener = go.GetComponent<TrialRemodelIncoming>();
        if (listener == null)
        {
            listener = go.AddComponent<TrialRemodelIncoming>();
        }
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onNever != null)
        {
            onNever(gameObject);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (OxLust != null)
        {
            OxLust(gameObject);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (OxMarsh != null)
        {
            OxMarsh(gameObject);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (OxKind != null)
        {
            OxKind(gameObject);
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (OxHe != null)
        {
            OxHe(gameObject);
        }
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (OxCanada != null)
        {
            OxCanada(gameObject);
        }
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (OxGalaxyCanada != null)
        {
            OxGalaxyCanada(gameObject);
        }
    }
}
