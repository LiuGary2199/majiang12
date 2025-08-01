using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RadiumLysStore : ComeUIFlank
{
    [Header("按钮")]
[UnityEngine.Serialization.FormerlySerializedAs("ADButton")]    public Button ADPiston;
[UnityEngine.Serialization.FormerlySerializedAs("NextLevelButton")]    public Button NextTenthPiston;
[UnityEngine.Serialization.FormerlySerializedAs("RewardText")]    public Text RadiumSago;
[UnityEngine.Serialization.FormerlySerializedAs("rewardTrans")]    public Transform CellarExist;
    private double CellarMoose;
[UnityEngine.Serialization.FormerlySerializedAs("parObj")]    public GameObject WinSun;
    private bool UteRecitalToDew;
[UnityEngine.Serialization.FormerlySerializedAs("adobj")]    public GameObject Clear;
[UnityEngine.Serialization.FormerlySerializedAs("adrettrfansform")]    public RectTransform Inconspicuously;
[UnityEngine.Serialization.FormerlySerializedAs("tween")]    public Tween Sieve;
    private string ToCrane= "1";

    // Start is called before the first frame update
    void Start()
    {
        ADPiston.onClick.AddListener(() =>
        {
            ADPiston.enabled = false;
            NextTenthPiston.enabled = false;
            if (UpFoxTilt())
            {
                SameLoomAnalyze.FadDeem(CFellow.Dy_YoungLysRadium, false);
                BowDebt();
            }
            else
            {
                ADAnalyze.Overtone.DikeRadiumUnder((success) =>
                {
                    if (success)
                    {
                        ToCrane = "1";
                        SameLoomAnalyze.FadDeem(CFellow.Dy_YoungLysRadium, false);
                        BowDebt();
                    }
                    else
                    {
                        ADPiston.enabled = true;
                        NextTenthPiston.enabled = true;
                    }
                }, "1");
            }
        });

        NextTenthPiston.onClick.AddListener(() =>
        {
            ToCrane = "0";
            ADPiston.enabled = false;
            NextTenthPiston.enabled = false;
            DireStore.Instance.AxeArab(CellarMoose, CellarExist);
            ADAnalyze.Overtone.WeIgniteAxeImage();
            MediaUIDeer(GetType().Name);
        });
    }

    public void BowDebt()
    {
        StoreroomSuccessful.PillowVacant(CellarMoose, CellarMoose * 5, 0, RadiumSago, "+", () =>
        {
            CellarMoose = CellarMoose * 5;
            RadiumSago.text = "+" + VacantDram.ShovelBeOwe(CellarMoose);
            UteRecitalToDew = true;
            DireStore.Instance.AxeArab(CellarMoose, CellarExist);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                MediaUIDeer(GetType().Name);
            });
        });
    }

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        WinSun.SetActive(false);
        WinSun.SetActive(true);
        ADPiston.enabled = true;
        NextTenthPiston.enabled = true;
        NextTenthPiston.gameObject.SetActive(false);
        CellarMoose = VacantDram.BeatReboundBeShovel(uiFormParams);
        RadiumSago.text = "+" + VacantDram.ReboundYolkSqueezeCue(uiFormParams);
        if (UpFoxTilt())
        {
            Clear.SetActive(false);
            Inconspicuously.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            Clear.SetActive(true);
            Inconspicuously.anchoredPosition = new Vector2(41.35f, 0);
        }
        Sieve?.Kill();
        Sieve = DOVirtual.DelayedCall(1f, () =>
        {
            Sieve?.Kill();
              if (!UpFoxTilt())
            {
            NextTenthPiston.gameObject.SetActive(true);}
        });
    }

    public override void Hidding()
    {
        base.Hidding();
        BaskTrialGerman.GetInstance().ArabTrial("1003", ToCrane);
        BeadMoodUsStore();
    }

    private void BeadMoodUsStore()
    {
        if (FactorDram.UpHonor())
        {
            return;
        }
        if (SameLoomAnalyze.OwnChange(CFellow.Dy_Wold_Gold_Tram_us) != "done")
        {
            YelpUIDeer(nameof(MoodDyStore));
            SameLoomAnalyze.FadChange(CFellow.Dy_Wold_Gold_Tram_us, "done");
        }
    }

    private bool UpFoxTilt()
    {
        return !PlayerPrefs.HasKey(CFellow.Dy_YoungLysRadium + "Bool") || SameLoomAnalyze.OwnDeem(CFellow.Dy_YoungLysRadium);
    }
}
