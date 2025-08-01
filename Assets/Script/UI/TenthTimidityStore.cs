using System.Collections;
using System.Collections.Generic;
using Mkey;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using DG.Tweening;

public class TenthTimidityStore : ComeUIFlank
{
    [Header("按钮")]
[UnityEngine.Serialization.FormerlySerializedAs("ADButton")]    public Button ADPiston;
[UnityEngine.Serialization.FormerlySerializedAs("NextLevelButton")]    public Button NextTenthPiston;
[UnityEngine.Serialization.FormerlySerializedAs("ADText")]    public GameObject ADSago;
    [Header("转盘组")]
[UnityEngine.Serialization.FormerlySerializedAs("SlotBG")]    public HoldThing HoldBG;
[UnityEngine.Serialization.FormerlySerializedAs("RewardCashImage")]
    public GameObject RadiumArabUtter;
[UnityEngine.Serialization.FormerlySerializedAs("RewardGoldImage")]    public GameObject RadiumBoldUtter;
[UnityEngine.Serialization.FormerlySerializedAs("RewardText")]    public Text RadiumSago;

    private double CellarMoose;
    private bool UteRecitalToDew;
[UnityEngine.Serialization.FormerlySerializedAs("grtMoreRect")]    public RectTransform ToeIdleFlee;
[UnityEngine.Serialization.FormerlySerializedAs("m_SkeletonGraphic")]    public SkeletonGraphic m_EnforcerUpdraft;
    // Start is called before the first frame update
    private string ATrance= "1";
[UnityEngine.Serialization.FormerlySerializedAs("tween")]    public Tween Sieve;
    void Start()
    {
        // 监听动画结束事件
        m_EnforcerUpdraft.AnimationState.Complete += OnAnimationComplete;
        ADPiston.onClick.AddListener(() =>
        {
            ADPiston.enabled = false;
            NextTenthPiston.enabled = false;
            if (UpFoxTilt())
            {
                DikeHold();
            }
            else
            {
                ADAnalyze.Overtone.DikeRadiumUnder((success) =>
                {
                        if (success)
                        {
                            DikeHold();
                            ATrance = "1";
                        }
                        else
                        {
                            ADPiston.enabled = true;
                            NextTenthPiston.enabled = true;
                        }
                }, "2" );
            }
        });

        NextTenthPiston.onClick.AddListener(() =>
        {
            MyLien();
            NextTenthPiston.enabled = false;
            DireStore.Instance.AxeArab(CellarMoose, RadiumArabUtter.transform);
            if (!UteRecitalToDew)
            {
                ADAnalyze.Overtone.WeIgniteAxeImage();
            }
            MediaUIDeer(GetType().Name);
            ATrance = "0";
          
        });
    }
    public void MyLien()
    {     LienFade();
        NextTenthPiston.enabled = false;
        DireStore.Instance.AxeArab(CellarMoose, RadiumArabUtter.transform);
        if (!UteRecitalToDew)
        {
            ADAnalyze.Overtone.WeIgniteAxeImage();
        }
        MediaUIDeer(GetType().Name);
        BaskTrialGerman.GetInstance().ArabTrial("1004", ATrance);
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry != null)
        {
            if (trackEntry.Animation.Name == "1start")
            {
                m_EnforcerUpdraft.AnimationState.SetAnimation(0, "2idle", true);
            }
        }
    }

    public void LienFade()
    {
        FadeAnalyze.Instance.m_ClumpImage = 0;
        //FadeAnalyze.Instance.NextGame();
        Mkey.GameLevelHolder.SetCurrentLevelAndSave(Mkey.GameLevelHolder.CurrentLevel + 1);
        // 先清理旧麻将牌对象
        if (GameBoard.Instance != null)
        {
            GameBoard.Instance.DestroyGrid(); // 清理旧麻将
            GameBoard.Instance.CreateGameBoard(); // 用新关卡号重建关卡
        }
        // 触发关卡开始事件，刷新UI（包括GameLevelHelper等）
        Mkey.GameLevelHolder.StartLevel();
        // 刷新主界面关卡号显示
    }

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        NextTenthPiston.gameObject.SetActive(false);
        m_EnforcerUpdraft.AnimationState.ClearTracks();
        m_EnforcerUpdraft.AnimationState.SetAnimation(0, "1start", false);
        ADPiston.enabled = true;
        NextTenthPiston.enabled = true;
        if (UpFoxTilt())
        {
            ADSago.SetActive(false);
            ToeIdleFlee.anchoredPosition = new Vector2(0, 0);
            NextTenthPiston.gameObject.SetActive(false);
        }
        else
        {
            ADSago.SetActive(true);
            ToeIdleFlee.anchoredPosition = new Vector2(41.35f, 0);
        }
        UteRecitalToDew = false;

        // 根据实际项目计算奖励
        //rewardValue = FactorDram.IsApple() ? MobTownEre.instance.InitData.box_gold_price * GameUtil.GetGoldMulti() : MobTownEre.instance.InitData.passlevel_cash_price * GameUtil.GetCashMulti();
        CellarMoose = MobTownEre.instance.FadeLoom.leveldatalist[0].reward_num * GameUtil.GetCashMulti();
        //rewardValue = 1 * GameUtil.GetCashMulti();
        RadiumSago.text = "+" + VacantDram.ShovelBeOwe(CellarMoose);

        HoldBG.TeamSound();
        Sieve = DOVirtual.DelayedCall(2f, () =>
       {
           Sieve?.Kill();
           if (!UpFoxTilt())
           {
               NextTenthPiston.gameObject.SetActive(true);
           }

       });
        //DOVirtual.DelayedCall(1f, () =>
        //{
           // NextGame();
       // });

    }

    private bool UpFoxTilt()
    {
        return !PlayerPrefs.HasKey(CFellow.Dy_YoungHold + "Bool") || SameLoomAnalyze.OwnDeem(CFellow.Dy_YoungHold);
    }
    // 计算本次slot应该获得的奖励
    private int MixHoldSoundNomad()
    {
        // 新用户，第一次固定翻5倍
        if (UpFoxTilt())
        {
            int index = 0;
            foreach (SlotItem wg in MobTownEre.instance.VoltLoom.slot_group)
            {
                if (wg.multi == 5)
                {
                    return index;
                }
                index++;
            }
        }
        else
        {
            int sumWeight = 0;
            foreach (SlotItem wg in MobTownEre.instance.VoltLoom.slot_group)
            {
                sumWeight += wg.weight;
            }
            int r = Random.Range(0, sumWeight);
            int nowWeight = 0;
            int index = 0;
            foreach (SlotItem wg in MobTownEre.instance.VoltLoom.slot_group)
            {
                nowWeight += wg.weight;
                if (nowWeight > r)
                {
                    return index;
                }
                index++;
            }

        }
        return 0;
    }
    public override void Hidding()
    {
        base.Hidding();
        m_EnforcerUpdraft.AnimationState.SetAnimation(1, "3end", false);
    }

    private void DikeHold()
    {
        NextTenthPiston.enabled = false;
        ADPiston.enabled = false;
        int index = MixHoldSoundNomad();
        HoldBG.Deck(index, (multi) =>
        {
            // slot结束后的回调
            StoreroomSuccessful.PillowVacant(CellarMoose, CellarMoose * multi, 0, RadiumSago, "+", () =>
            {
                CellarMoose = CellarMoose * multi;
                RadiumSago.text = "+" + VacantDram.ShovelBeOwe(CellarMoose);
                UteRecitalToDew = true;
           
                DOVirtual.DelayedCall(0.5f, () =>
              {
                  MyLien();
              });
            });
        });

        SameLoomAnalyze.FadDeem(CFellow.Dy_YoungHold, false);
    }
}
