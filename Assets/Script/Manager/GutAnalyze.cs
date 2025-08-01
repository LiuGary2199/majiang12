using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
public class GutAnalyze : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("GutCult")]    public GameObject GutCult;
    public static GutAnalyze Instance;
[UnityEngine.Serialization.FormerlySerializedAs("isOpenFly")]
    public bool UpYelpGut;
[UnityEngine.Serialization.FormerlySerializedAs("leftOrRight")]    public int HartAnFlask;

    private int _PaperYelpRich;
    private int _SumAxeRich;
    
    private void Awake()
    {
        Instance = this;
        _SumAxeRich = 0;
        UpYelpGut = true;
        _PaperYelpRich = MobTownEre.instance.FadeLoom.bubble_cd;
        HartAnFlask = 0;
    }

    private void OnEnable()
    {
        YelpIEGut();
    }
   
    public void YelpIEGut()
    {
        StopCoroutine(nameof(YelpGutMonkey));
        StartCoroutine(nameof(YelpGutMonkey));
    }
    IEnumerator YelpGutMonkey()
    {
        while (UpYelpGut)
        {   
            if (_SumAxeRich >= _PaperYelpRich)
            {
                IntentGutCult();
            }
            _SumAxeRich++;
            yield return new WaitForSeconds(1);
        }
    }

    public void AdhereGutCult()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<GutCult>().PortrayGutCult();
            UpYelpGut = true;
        }
    }

    public void IntentGutCult()
    {
        if (!UpYelpGut) { return; }
        // 新增：引导阶段禁止飞行气泡
        if (GameLevelHolder.CurrentLevel <= 1)
        {
            return;
        }
        //if (BubbleManager.GetInstance().IsWinGame()) { return; }
        //  if ( LevelManager.GetInstance().CurLevel > 1 && !FactorDram.IsApple
      if ( !FactorDram.UpHonor())
        {
            UpYelpGut = false;
            _SumAxeRich = 0;
            GameObject obj = Instantiate(GutCult.gameObject);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = HartAnFlask == 0 ? new Vector3(-650, 0, 0) : new Vector3(650, 0, 0);
        }
    }

    //public void SendFlyCollider(BubbleItem bubble)
    //{
    //    KeyValuesUpdate key = new KeyValuesUpdate(StringConst.SendFlyCollider, bubble);
    //    SharplyResort.SendMessage(StringConst.SendFlyCollider, key);
    //}
}
