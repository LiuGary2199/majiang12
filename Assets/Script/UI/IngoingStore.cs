using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class IngoingStore : ComeUIFlank
{
[UnityEngine.Serialization.FormerlySerializedAs("Sound_Button")]    public Button Wispy_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("Music_Button")]    public Button Panel_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("SoundIcon")]    public Image WispyPage;
[UnityEngine.Serialization.FormerlySerializedAs("MusicIcon")]    public Image PanelPage;
[UnityEngine.Serialization.FormerlySerializedAs("Continue_Button")]    public Button Geologic_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("CLose_Button")]    public Button CGlow_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("Restart_Button")]
    public Button General_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("MusicCloseSprite")]    public Sprite PanelMediaNitric;
[UnityEngine.Serialization.FormerlySerializedAs("MusicOpenSprite")]    public Sprite PanelYelpNitric;
[UnityEngine.Serialization.FormerlySerializedAs("SoundCloseSprite")]    public Sprite WispyMediaNitric;
[UnityEngine.Serialization.FormerlySerializedAs("SoundOpenSprite")]    public Sprite WispyYelpNitric;

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        PanelPage.sprite = PanelEre.GetInstance().OxPanelDesire ? PanelYelpNitric : PanelMediaNitric;
        WispyPage.sprite = PanelEre.GetInstance().BellowPanelDesire ? WispyYelpNitric : WispyMediaNitric;
    }
    // Start is called before the first frame update
    void Start()
    {
        CGlow_Piston.onClick.AddListener(() => {
            MediaUIDeer(GetType().Name);
        });
        Geologic_Piston.onClick.AddListener(() => {
            MediaUIDeer(GetType().Name);
        });
        General_Piston.onClick.AddListener(() => {
            FadeAnalyze.Instance.GeneralFade();
            DOVirtual.DelayedCall(0.5f, () =>  //停顿
            {
                MediaUIDeer(GetType().Name);
            });
        });
        
        Panel_Piston.onClick.AddListener(() =>
        {

            PanelEre.GetInstance().OxPanelDesire = !PanelEre.GetInstance().OxPanelDesire;
            PanelPage.sprite = PanelEre.GetInstance().OxPanelDesire ? PanelYelpNitric : PanelMediaNitric;
        });
        Wispy_Piston.onClick.AddListener(() =>
        {

            PanelEre.GetInstance().BellowPanelDesire = !PanelEre.GetInstance().BellowPanelDesire;
            WispyPage.sprite = PanelEre.GetInstance().BellowPanelDesire ? WispyYelpNitric : WispyMediaNitric;
        });
    }

}
