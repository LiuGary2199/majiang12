/**
 * 
 * 网络请求的post对象
 * 
 * ***/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MobAutoBaskTypify 
{
    //post请求地址
    public string URL;
    //post的数据表单
    public WWWForm Deer;
    //post成功回调
    public Action<UnityWebRequest> BaskBedroom;
    //post失败回调
    public Action BaskHusk;
    public MobAutoBaskTypify(string url,WWWForm  form,Action<UnityWebRequest> success,Action fail)
    {
        URL = url;
        Deer = form;
        BaskBedroom = success;
        BaskHusk = fail;
    }
}
