using System.Collections;
using System.Collections.Generic;
using Mkey;
using UnityEngine;

public class ClanAnalyze : MonoBehaviour
{
    public static ClanAnalyze instance;

    private bool Proof= false;
[UnityEngine.Serialization.FormerlySerializedAs("GameView")]    public GameObject FadeSnap;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Legitimately(){
        FadeSnap.SetActive(true);
    }    

    public void LimeVolt()
    {
        bool isNewPlayer = !PlayerPrefs.HasKey(CFellow.Dy_UpFoxPlayer + "Bool") || SameLoomAnalyze.OwnDeem(CFellow.Dy_UpFoxPlayer);
        CensusVoltAnalyze.Instance.VoltCensusLoom(isNewPlayer);
        //if (!FactorDram.IsApple())
        //    CensusVoltAnalyze.Instance.AdjustInit();
        if (isNewPlayer)
        {
            // 新用户
            SameLoomAnalyze.FadDeem(CFellow.Dy_UpFoxPlayer, false);
        }


        GameUtil.IsSameDayAsLastCheck();//每日奖励检测

        //PanelEre.GetInstance().PlayBg(PanelRate.SceneMusic.Sound_BGM);
        BaskTrialGerman.GetInstance().ArabTrial("1001");
        UIAnalyze.GetInstance().BeadUIFlank(nameof(DireStore));

        FadeLoomAnalyze.GetInstance().VoltFadeLoom();

        Proof = true;

        //ActivityAutoOpenManager.Instance.OpenPanel(1);
    }

}
