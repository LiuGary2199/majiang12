using System.Collections;
using System.Collections.Generic;
using Mkey;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BegBound : ComeUIFlank
{
[UnityEngine.Serialization.FormerlySerializedAs("AD_Button")]    public Button AD_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("Restart_Button")]    public Button General_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("Use_Button")]    public Button Urn_Piston;

    [Header("倒计时精灵")]
[UnityEngine.Serialization.FormerlySerializedAs("countdownSprites")]    public Image[] ShortwaveRailway; // 5,4,3,2,1 倒计时精灵
[UnityEngine.Serialization.FormerlySerializedAs("countdownInterval")]    public float ShortwaveEfficacy= 1f; // 倒计时间隔时间

    private Coroutine ShortwaveDiversify;
[UnityEngine.Serialization.FormerlySerializedAs("AppleAD_Button")]
    public Button HonorAD_Piston;
[UnityEngine.Serialization.FormerlySerializedAs("AppleCoin_Button")]    public Button HonorDumb_Piston;



    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        AD_Piston.gameObject.SetActive(false);
        Urn_Piston.gameObject.SetActive(false);





        int shuffles = ShuffleHolder.Count; // 重洗次数
        /* 
        if (shuffles > 0)
         {
             Use_Button.gameObject.SetActive(true);
         }
         else
         {
             AD_Button.gameObject.SetActive(true);
         }
         */
        AD_Piston.gameObject.SetActive(true);

        if (FactorDram.UpHonor())
        {
            AD_Piston.gameObject.SetActive(false);
            HonorAD_Piston.gameObject.SetActive(true);
            HonorDumb_Piston.gameObject.SetActive(true);

        }
        else
        {
            AD_Piston.gameObject.SetActive(true);
            HonorAD_Piston.gameObject.SetActive(false);
            HonorDumb_Piston.gameObject.SetActive(false);

        }

        // 开始倒计时
        WoodySensitive();
    }

    // 开始倒计时
    private void WoodySensitive()
    {
        if (ShortwaveDiversify != null)
        {
            StopCoroutine(ShortwaveDiversify);
        }
        ShortwaveDiversify = StartCoroutine(SensitiveDiversify());
    }

    // 倒计时协程
    private IEnumerator SensitiveDiversify()
    {
        // 隐藏所有倒计时精灵
        foreach (var sprite in ShortwaveRailway)
        {
            if (sprite != null)
                sprite.gameObject.SetActive(false);
        }

        // 从5开始倒计时
        for (int i = 5; i >= 1; i--)
        {
            // 显示对应的倒计时精灵
            if (ShortwaveRailway != null && i - 1 < ShortwaveRailway.Length && ShortwaveRailway[i - 1] != null)
            {
                ShortwaveRailway[i - 1].gameObject.SetActive(true);
                Debug.Log($"显示倒计时精灵: {i}");
            }

            // 等待倒计时间隔
            yield return new WaitForSeconds(ShortwaveEfficacy);

            // 隐藏当前倒计时精灵
            if (ShortwaveRailway != null && i - 1 < ShortwaveRailway.Length && ShortwaveRailway[i - 1] != null)
            {
                ShortwaveRailway[i - 1].gameObject.SetActive(false);
            }
        }

        // 倒计时结束，自动选择Restart_Button
        Debug.Log("倒计时结束，自动选择Restart_Button");
        General_Piston.onClick.Invoke();
    }

    // 停止倒计时
    private void PaneSensitive()
    {
        if (ShortwaveDiversify != null)
        {
            StopCoroutine(ShortwaveDiversify);
            ShortwaveDiversify = null;
        }

        // 隐藏所有倒计时精灵
        foreach (var sprite in ShortwaveRailway)
        {
            if (sprite != null)
                sprite.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HonorAD_Piston.onClick.AddListener(() =>
     {
         PaneSensitive(); // 停止倒计时
         ADAnalyze.Overtone.DikeRadiumUnder((success) =>
         {
             if (success)
             {
                 GameBoard.Instance.ShuffleGrid(null); // 执行重洗网格逻辑
                 MediaUIDeer(GetType().Name);
             }
         }, "6");
         BaskTrialGerman.GetInstance().ArabTrial("1007", "1");

     });
        HonorDumb_Piston.onClick.AddListener(() =>
    {
        double munber = FadeLoomAnalyze.GetInstance().MixFlood();
        if (munber >= 200)
        {
            PaneSensitive(); // 停止倒计时
            FadeLoomAnalyze.GetInstance().LopFlood(-200);
             DireStore.Instance.AxeArab(0, null);
            GameBoard.Instance.ShuffleGrid(null); // 执行重洗网格逻辑
            MediaUIDeer(GetType().Name);
        }else{
            	UIAnalyze.GetInstance().BeadUIFlank(nameof(Ought), "Diamond shortage");
        }
    });

        AD_Piston.onClick.AddListener(() =>
        {
            PaneSensitive(); // 停止倒计时
            ADAnalyze.Overtone.DikeRadiumUnder((success) =>
            {
                if (success)
                {
                    GameBoard.Instance.ShuffleGrid(null); // 执行重洗网格逻辑
                    MediaUIDeer(GetType().Name);
                }
            }, "6");
            BaskTrialGerman.GetInstance().ArabTrial("1007", "1");

        });
        Urn_Piston.onClick.AddListener(() =>
        {
            PaneSensitive(); // 停止倒计时
            // 使用重洗道具的逻辑
            GameBoard.Instance.ShuffleGrid(null); // 执行重洗网格逻辑
            ShuffleHolder.Add(-1); // 减少一个重洗道具
            GameEvents.ApplyShuffleAction?.Invoke();
            MediaUIDeer(GetType().Name);

        });

        General_Piston.onClick.AddListener(() =>
        {
            PaneSensitive(); // 停止倒计时
            FadeAnalyze.Instance.GeneralFade();
            DOVirtual.DelayedCall(0.5f, () =>  //停顿
            {
                MediaUIDeer(GetType().Name);
            });
            BaskTrialGerman.GetInstance().ArabTrial("1007", "0");
        });

    }

    // 当UI关闭时停止倒计时
    public override void Hidding(System.Action finish = null)
    {
        PaneSensitive();
        base.Hidding(finish);
    }
}
