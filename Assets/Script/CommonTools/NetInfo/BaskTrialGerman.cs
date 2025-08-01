using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class BaskTrialGerman : CopyVibration<BaskTrialGerman>
{
    public string version = "1.2";
    public string FadeCash= MobTownEre.instance.FadeCash;
    //channel
#if UNITY_IOS
    private string Nonself= "AppStore";
#elif UNITY_ANDROID
    private string Channel = "GooglePlay";
#else
    private string Channel = "GooglePlay";
#endif


    private void OnApplicationPause(bool pause)
    {
        BaskTrialGerman.GetInstance().EvenFadeMandible();
    }
    
    public Text Coil;

    protected override void Awake()
    {
        base.Awake();
        
        version = Application.version;
        StartCoroutine("LensSharply");
    }
    IEnumerator LensSharply()
    {
        while (true)
        {
            yield return new WaitForSeconds(120f);
            BaskTrialGerman.GetInstance().EvenFadeMandible();
        }
    }
    private void Start()
    {
        if (SameLoomAnalyze.OwnNor("event_day") != DateTime.Now.Day && SameLoomAnalyze.OwnChange("user_servers_id").Length != 0)
        {
            SameLoomAnalyze.FadNor("event_day", DateTime.Now.Day);
        }
    }
    public void ArabWeSilkTrial(string event_id)
    {
        ArabTrial(event_id);
    }

    public void EvenFadeMandible(List<string> valueList = null)
    {
        if (SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionBoldDumb) == 0)
        {
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionBoldDumb, SameLoomAnalyze.OwnShovel(CFellow.Dy_BoldDumb));
        }
        if (SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionFlood) == 0)
        {
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionFlood, SameLoomAnalyze.OwnShovel(CFellow.Dy_Flood));
        }
        if (valueList == null)
        {
            valueList = new List<string>() {
                CashOutManager.GetInstance().Money.ToString(),
                SameLoomAnalyze.OwnChange(CFellow.Dy_StageArabFlood),
                (Mkey.GameLevelHolder.CurrentLevel + 1).ToString(),
                SameLoomAnalyze.OwnChange(CFellow.Dy_AggressionFlood).ToString(),
                
            };
        }
        
        if (SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt) == null)
        {
            return;
        }
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("gameCode", FadeCash);
        wwwForm.AddField("userId", SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt));

        wwwForm.AddField("gameVersion", version);

        wwwForm.AddField("channel", Nonself);

        for (int i = 0; i < valueList.Count; i++)
        {
            wwwForm.AddField("resource" + (i + 1), valueList[i]);
        }

        StartCoroutine(ArabBask(MobTownEre.instance.ComeCow + "/api/client/game_progress", wwwForm,
        (error) =>
        {
            Debug.Log(error);
        },
        (message) =>
        {
            Debug.Log(message);
        }));
    }
    public void ArabTrial(string event_id, string p1 = null, string p2 = null, string p3 = null)
    {
        if (Coil != null)
        {
            if (int.Parse(event_id) < 9100 && int.Parse(event_id) >= 9000)
            {
                if (p1 == null)
                {
                    p1 = "";
                }
                Coil.text += "\n" + DateTime.Now.ToString() + "id:" + event_id + "  p1:" + p1;
            }
        }
        if (SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt) == null)
        {
            MobTownEre.instance.Humid();
            return;
        }
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("gameCode", FadeCash);
        wwwForm.AddField("userId", SameLoomAnalyze.OwnChange(CFellow.Dy_SullyOxygenAt));
        //Debug.Log("userId:" + SameLoomAnalyze.GetString(CFellow.sv_LocalServerId));
        wwwForm.AddField("version", version);
        //Debug.Log("version:" + version);
        wwwForm.AddField("channel", Nonself);
        //Debug.Log("channel:" + channal);
        wwwForm.AddField("operateId", event_id);
        Debug.Log("operateId:" + event_id);


        if (p1 != null)
        {
            wwwForm.AddField("params1", p1);
        }
        if (p2 != null)
        {
            wwwForm.AddField("params2", p2);
        }
        if (p3 != null)
        {
            wwwForm.AddField("params3", p3);
        }
        StartCoroutine(ArabBask(MobTownEre.instance.ComeCow + "/api/client/log", wwwForm,
        (error) =>
        {
            Debug.Log(error);
        },
        (message) =>
        {
            Debug.Log(message);
        }));
    }
    IEnumerator ArabBask(string _url, WWWForm wwwForm, Action<string> fail, Action<string> success)
    {
        //Debug.Log(SerializeDictionaryToJsonString(dic));
        using UnityWebRequest request = UnityWebRequest.Post(_url, wwwForm);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isNetworkError)
        {
            fail(request.error);
            endEnhance();
        }
        else
        {
            success(request.downloadHandler.text);
            endEnhance();
        }
    }
    private void endEnhance()
    {
        StopCoroutine("SendGet");
    }


}