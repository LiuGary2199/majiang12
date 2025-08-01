/***
 * 
 * 网络请求的get对象
 * 
 * **/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MobAutoOwnTypify 
{
    //get的url
    public string Cow;
    //get成功的回调
    public Action<UnityWebRequest> OwnBedroom;
    //get失败的回调
    public Action OwnHusk;
    public MobAutoOwnTypify(string url,Action<UnityWebRequest> success,Action fail)
    {
        Cow = url;
        OwnBedroom = success;
        OwnHusk = fail;
    }
   
}
