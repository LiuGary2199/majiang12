/**
 * 
 * 左右滑动的页面视图
 * 
 * ***/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TaleSnap : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
[UnityEngine.Serialization.FormerlySerializedAs("rect")]    //scrollview
    public ScrollRect Tone;
    //求出每页的临界角，页索引从0开始
    List<float> NotFoul= new List<float>();
[UnityEngine.Serialization.FormerlySerializedAs("isDrag")]    //是否拖拽结束
    public bool UpTint= false;
    bool LawnRoam= true;
    //滑动的起始坐标  
    float RandomHenceforth= 0;
    float PaperTintHenceforth;
    float startTime = 0f;
[UnityEngine.Serialization.FormerlySerializedAs("smooting")]    //滑动速度  
    public float Spacious= 1f;
[UnityEngine.Serialization.FormerlySerializedAs("sensitivity")]    public float Earnestness= 0.3f;
[UnityEngine.Serialization.FormerlySerializedAs("OnPageChange")]    //页面改变
    public Action<int> OfTalePillow;
    //当前页面下标
    int AcreageTaleNomad= -1;
    void Start()
    {
        Tone = this.GetComponent<ScrollRect>();
        float horizontalLength = Tone.content.rect.width - this.GetComponent<RectTransform>().rect.width;
        NotFoul.Add(0);
        for(int i = 1; i < Tone.content.childCount - 1; i++)
        {
            NotFoul.Add(GetComponent<RectTransform>().rect.width * i / horizontalLength);
        }
        NotFoul.Add(1);
    }

    
    void Update()
    {
        if(!UpTint && !LawnRoam)
        {
            startTime += Time.deltaTime;
            float t = startTime * Spacious;
            Tone.horizontalNormalizedPosition = Mathf.Lerp(Tone.horizontalNormalizedPosition, RandomHenceforth, t);
            if (t >= 1)
            {
                LawnRoam = true;
            }
        }
        
    }
    /// <summary>
    /// 设置页面的index下标
    /// </summary>
    /// <param name="index"></param>
    void FadTaleNomad(int index)
    {
        if (AcreageTaleNomad != index)
        {
            AcreageTaleNomad = index;
            if (OfTalePillow != null)
            {
                OfTalePillow(index);
            }
        }
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        UpTint = true;
        PaperTintHenceforth = Tone.horizontalNormalizedPosition;
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = Tone.horizontalNormalizedPosition;
        posX += ((posX - PaperTintHenceforth) * Earnestness);
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        int index = 0;
        float offset = Mathf.Abs(NotFoul[index] - posX);
        for(int i = 0; i < NotFoul.Count; i++)
        {
            float temp = Mathf.Abs(NotFoul[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        FadTaleNomad(index);
        RandomHenceforth = NotFoul[index];
        UpTint = false;
        startTime = 0f;
        LawnRoam = false;
    }
}
