using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Mkey;
public class FadeAnalyze : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("gameBoard")]    public GameBoard LimeRatio;
    static public FadeAnalyze Instance;
    private Tween RichLust;
    private float Emigrant= 15;
[UnityEngine.Serialization.FormerlySerializedAs("m_isCommbo")]    public bool m_UpArctic;
[UnityEngine.Serialization.FormerlySerializedAs("m_ComboCount")]    public int m_ClumpImage= 0;
[UnityEngine.Serialization.FormerlySerializedAs("eventSystem")]    public EventSystem StrutRegret;

    public void SequoiaTrial()
    {
        StrutRegret.enabled = false;
    }
    public void WoodyTrial()
    {
        StrutRegret.enabled = true;
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SharplyResort.AxeCarIncoming(CFellow.Me_OfClumpCheese, OnComboUpdate);
    }

    public void GeneralFade()
    {
        LimeRatio.RestartLevel();
        m_ClumpImage = 0;
    }
    public void LienFade()
    {
        LimeRatio.CompleteAndLoadNextLevel();
        m_ClumpImage = 0;
    }
    private void OnComboUpdate(KeyValuesUpdate kv)
    {
        m_ClumpImage += 1;
        RichLust?.Kill();
        RichLust = DOVirtual.DelayedCall(Emigrant, () =>
        {
            m_ClumpImage = 0;
            m_UpArctic = false;
        });
        if (m_ClumpImage>=3)
        {
            m_UpArctic = true;
            ArabLoom sendData = new ArabLoom();
            sendData.ClumpImage = m_ClumpImage;
            sendData.Demise3Not = (Vector3) kv.Tongue;
            KeyValuesUpdate keyfly = new KeyValuesUpdate(CFellow.Me_OfClumpBead, sendData);
            SharplyResort.ArabSharply(CFellow.Me_OfClumpBead, keyfly);
        }
       /* int goldMul = MobTownEre.instance.GameData.combogold;
        if (m_ComboCount > 0 && m_ComboCount % goldMul == 0 && m_ComboCount != goldMul)
        {
            gameBoard.ChangeGold();
        }

        if (m_ComboCount >= goldMul)
        {

        }*/
    }
}
