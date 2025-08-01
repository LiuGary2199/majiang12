using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GutCult : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("FlyButton")]    public Button GutPiston;
[UnityEngine.Serialization.FormerlySerializedAs("CashValue")]    public Text ArabMoose;

    private Sequence _Maw1;
    private Sequence _Maw2;

    private double _LienBow;

    private void Awake()
    {
        GutPiston.onClick.AddListener(() => {
            //if (NewbieManager.GetInstance().IsOpenNewbie) { return; }
            //if (BubbleManager.GetInstance().IsWinGame()) { return; }
            GutAnalyze.Instance.UpYelpGut = true;
            GutAnalyze.Instance.YelpIEGut();
            BaskTrialGerman.GetInstance().ArabTrial("1011");
            OwnRadium();

        });
        OilWashInjure();
    }


    public void GutInch()
    {
        transform.DOPlay();
        _Maw1.Play();
        _Maw2.Play();
    }

    public void GutHinge()
    {
        transform.DOPause();
        _Maw1.Pause();
        _Maw2.Pause();
    }

    public void GutKill()
    {
        _Maw1.Kill();
        _Maw2.Kill();
        transform.DOKill();
    }

    private void OwnRadium()
    {
        //RewardPanelData data = new RewardPanelData();
        //data.MiniType = "Fly";
        //data.Dic_Reward.Add(RewardType.cash, _cashNum);
        //RewardManager.GetInstance().OpenLevelCompletePanel(data);
        ADAnalyze.Overtone.DikeRadiumUnder((success) =>
        {
            if (success)
            {
                DireStore.Instance.AxeArab(_LienBow, this.transform);
                PortrayGutCult();
                 BaskTrialGerman.GetInstance().ArabTrial("1009");
            }
        }, "5");
    }

    private void OilWashInjure()
    {
        _LienBow = MobTownEre.instance.FadeLoom.bubbledatalist[0].reward_num * GameUtil.GetCashMulti();
        _LienBow = Mathf.Ceil((float)_LienBow);
        ArabMoose.text = "+" + _LienBow;
        _Maw1 = DOTween.Sequence();
        _Maw2 = DOTween.Sequence();
        /*int leftOrRight = Random.Range(0, 2);
        if (leftOrRight == 0)
        {*/
            OvenGut();
        /*}
        else
        {
            RigthFly();
        }*/
    }

    private void OvenGut()
    {
        transform.localPosition = new Vector3(-450f, 0, 0);
        _Maw1 = DOTween.Sequence();
        _Maw2 = DOTween.Sequence();
        _Maw1.Append(transform.DOLocalMoveY(-250f - Random.Range(-100f, 100f), 2.5f).SetEase(Ease.InSine));
        _Maw1.Append(transform.DOLocalMoveY(0, 2.5f).SetEase(Ease.InSine));
        _Maw1.SetLoops(-1);
        _Maw1.Play();

        _Maw2.Append(transform.DOScale(1.1f, 0.5f).SetEase(Ease.Linear));
        _Maw2.Append(transform.DOScale(1f, 0.5f).SetEase(Ease.Linear));
        _Maw2.SetLoops(-1);
        _Maw2.Play();
        transform.DOLocalMoveX(650, 10f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (GutAnalyze.Instance.UpYelpGut)
            {
                PortrayGutCult();
            }
            else
            {
                GutKill();
                StartCoroutine(FineGut(() => { SillyGut(); }));
            }
        });
    }

    private void SillyGut()
    {
        transform.localPosition = new Vector3(450, 100, 0);
        _Maw1 = DOTween.Sequence();
        _Maw2 = DOTween.Sequence();
        _Maw1.Append(transform.DOLocalMoveY(0, 2.5f).SetEase(Ease.InSine));
        _Maw1.Append(transform.DOLocalMoveY(100, 2.5f).SetEase(Ease.InSine));
        _Maw1.SetLoops(-1);
        _Maw1.Play();

        _Maw2.Append(transform.DOScale(1.1f, 0.5f).SetEase(Ease.Linear));
        _Maw2.Append(transform.DOScale(1f, 0.5f).SetEase(Ease.Linear));
        _Maw2.SetLoops(-1);
        _Maw2.Play();
        transform.DOLocalMoveX(-650, 10f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (GutAnalyze.Instance.UpYelpGut)
            {
                PortrayGutCult();
            }
            else
            {
                GutKill();
                StartCoroutine(FineGut(() => { OvenGut(); }));
            }

        });
    }

    IEnumerator FineGut(Action action)
    {
        yield return new WaitForSeconds(5f);
        action?.Invoke();
    }

    public void PortrayGutCult()
    {
        GutKill();
        GetComponent<RectTransform>().DOKill();
        Destroy(gameObject);
    }

}
