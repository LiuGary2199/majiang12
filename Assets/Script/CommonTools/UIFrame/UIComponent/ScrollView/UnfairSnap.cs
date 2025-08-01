/**
 * 
 * 支持上下滑动的scroll view
 * 
 * **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnfairSnap : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("itemCell")]    //预支单体
    public UnfairSnapCult DietJust;
[UnityEngine.Serialization.FormerlySerializedAs("scrollRect")]    //scrollview
    public ScrollRect QuiverFlee;
[UnityEngine.Serialization.FormerlySerializedAs("content")]
    //content
    public RectTransform Episode;
[UnityEngine.Serialization.FormerlySerializedAs("spacing")]    //间隔
    public float Station= 10;
[UnityEngine.Serialization.FormerlySerializedAs("totalWidth")]    //总的宽
    public float CompoMason;
[UnityEngine.Serialization.FormerlySerializedAs("totalHeight")]    //总的高
    public float CompoDomain;
[UnityEngine.Serialization.FormerlySerializedAs("visibleCount")]    //可见的数量
    public int UtterlyImage;
[UnityEngine.Serialization.FormerlySerializedAs("isClac")]    //初始数据完成是否检测计算
    public bool UpFrog= false;
[UnityEngine.Serialization.FormerlySerializedAs("startIndex")]    //开始的索引
    public int PaperNomad;
[UnityEngine.Serialization.FormerlySerializedAs("lastIndex")]    //结尾的索引
    public int PikeNomad;
[UnityEngine.Serialization.FormerlySerializedAs("itemHeight")]    //item的高
    public float DietDomain= 50;
[UnityEngine.Serialization.FormerlySerializedAs("itemList")]
    //缓存的itemlist
    public List<UnfairSnapCult> DietFoul;
[UnityEngine.Serialization.FormerlySerializedAs("visibleList")]    //可见的itemList
    public List<UnfairSnapCult> UtterlyFoul;
[UnityEngine.Serialization.FormerlySerializedAs("allList")]    //总共的dataList
    public List<int> TinFoul;

    void Start()
    {
        CompoDomain = this.GetComponent<RectTransform>().sizeDelta.y;
        CompoMason = this.GetComponent<RectTransform>().sizeDelta.x;
        Episode = QuiverFlee.content;
        VoltLoom();

    }
    //初始化
    public void VoltLoom()
    {
        UtterlyImage = Mathf.CeilToInt(CompoDomain / BardDomain) + 1;
        for (int i = 0; i < UtterlyImage; i++)
        {
            this.AxeCult();
        }
        PaperNomad = 0;
        PikeNomad = 0;
        List<int> numberList = new List<int>();
        //数据长度
        int dataLength = 20;
        for (int i = 0; i < dataLength; i++)
        {
            numberList.Add(i);
        }
        FadLoom(numberList);
    }
    //设置数据
    void FadLoom(List<int> list)
    {
        TinFoul = list;
        PaperNomad = 0;
        if (LoomImage <= UtterlyImage)
        {
            PikeNomad = LoomImage;
        }
        else
        {
            PikeNomad = UtterlyImage - 1;
        }
        //Debug.Log("ooooooooo"+lastIndex);
        for (int i = PaperNomad; i < PikeNomad; i++)
        {
            UnfairSnapCult obj = DewCult();
            if (obj == null)
            {
                Debug.Log("获取item为空");
            }
            else
            {
                obj.gameObject.name = i.ToString();

                obj.gameObject.SetActive(true);
                obj.transform.localPosition = new Vector3(0, -i * BardDomain, 0);
                UtterlyFoul.Add(obj);
                GalaxyCult(i, obj);
            }

        }
        Episode.sizeDelta = new Vector2(CompoMason, LoomImage * BardDomain - Station);
        UpFrog = true;
    }
    //更新item
    public void GalaxyCult(int index, UnfairSnapCult obj)
    {
        int d = TinFoul[index];
        string str = d.ToString();
        obj.name = str;
        //更新数据 todo
    }
    //从itemlist中取出item
    public UnfairSnapCult DewCult()
    {
        UnfairSnapCult obj = null;
        if (DietFoul.Count > 0)
        {
            obj = DietFoul[0];
            obj.gameObject.SetActive(true);
            DietFoul.RemoveAt(0);
        }
        else
        {
            Debug.Log("从缓存中取出的是空");
        }
        return obj;
    }
    //item进入itemlist
    public void VeinCult(UnfairSnapCult obj)
    {
        DietFoul.Add(obj);
        obj.gameObject.SetActive(false);
    }
    public int LoomImage    {
        get
        {
            return TinFoul.Count;
        }
    }
    //每一行的高
    public float BardDomain    {
        get
        {
            return DietDomain + Station;
        }
    }
    //添加item到缓存列表中
    public void AxeCult()
    {
        GameObject obj = Instantiate(DietJust.gameObject);
        obj.transform.SetParent(Episode);
        RectTransform Tone= obj.GetComponent<RectTransform>();
        Tone.anchorMin = new Vector2(0.5f, 1);
        Tone.anchorMax = new Vector2(0.5f, 1);
        Tone.pivot = new Vector2(0.5f, 1);
        obj.SetActive(false);
        obj.transform.localScale = Vector3.one;
        UnfairSnapCult o = obj.GetComponent<UnfairSnapCult>();
        DietFoul.Add(o);
    }



    void Update()
    {
        if (UpFrog)
        {
            Unfair();
        }
    }
    /// <summary>
    /// 计算滑动支持上下滑动
    /// </summary>
    void Unfair()
    {
        float vy = Episode.anchoredPosition.y;
        float rollUpTop = (PaperNomad + 1) * BardDomain;
        float rollUnderTop = PaperNomad * BardDomain;

        if (vy > rollUpTop && PikeNomad < LoomImage)
        {
            //上边界移除
            if (UtterlyFoul.Count > 0)
            {
                UnfairSnapCult obj = UtterlyFoul[0];
                UtterlyFoul.RemoveAt(0);
                VeinCult(obj);
            }
            PaperNomad++;
        }
        float rollUpBottom = (PikeNomad - 1) * BardDomain - Station;
        if (vy < rollUpBottom - CompoDomain && PaperNomad > 0)
        {
            //下边界减少
            PikeNomad--;
            if (UtterlyFoul.Count > 0)
            {
                UnfairSnapCult obj = UtterlyFoul[UtterlyFoul.Count - 1];
                UtterlyFoul.RemoveAt(UtterlyFoul.Count - 1);
                VeinCult(obj);
            }

        }
        float rollUnderBottom = PikeNomad * BardDomain - Station;
        if (vy > rollUnderBottom - CompoDomain && PikeNomad < LoomImage)
        {
            //Debug.Log("下边界增加"+vy);
            //下边界增加
            UnfairSnapCult go = DewCult();
            UtterlyFoul.Add(go);
            go.transform.localPosition = new Vector3(0, -PikeNomad * BardDomain);
            GalaxyCult(PikeNomad, go);
            PikeNomad++;
        }


        if (vy < rollUnderTop && PaperNomad > 0)
        {
            //Debug.Log("上边界增加"+vy);
            //上边界增加
            PaperNomad--;
            UnfairSnapCult go = DewCult();
            UtterlyFoul.Insert(0, go);
            GalaxyCult(PaperNomad, go);
            go.transform.localPosition = new Vector3(0, -PaperNomad * BardDomain);
        }

    }
}
