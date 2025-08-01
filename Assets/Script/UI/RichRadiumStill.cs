using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class RichRadiumStill : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("m_ItemIndex")]    public int m_CultNomad;
[UnityEngine.Serialization.FormerlySerializedAs("m_RewardText")]    public Text m_RadiumSago;
[UnityEngine.Serialization.FormerlySerializedAs("m_AdCountText")]    public Text m_ToImageSago;
[UnityEngine.Serialization.FormerlySerializedAs("m_AdWatchBtn")]
    public Button m_ToSlateDew;
[UnityEngine.Serialization.FormerlySerializedAs("m_RewarededBtn")]    public GameObject m_UncountedDew;
[UnityEngine.Serialization.FormerlySerializedAs("m_TimeBtn")]    public GameObject m_RichDew;
[UnityEngine.Serialization.FormerlySerializedAs("m_GetBtn")]    public Button m_OwnDew;
[UnityEngine.Serialization.FormerlySerializedAs("OnAdFinish")]    public Action<int> OfToAshcan;
[UnityEngine.Serialization.FormerlySerializedAs("OnGetFinish")]    public Action<int> OfOwnAshcan;

    private DayRewardData m_YewRadiumLoom;
    public void Volt()
    {
        m_ToSlateDew.onClick.RemoveAllListeners();
        m_OwnDew.onClick.RemoveAllListeners();
        m_ToSlateDew.onClick.AddListener(() =>
        {
            m_ToSlateDew.enabled = false;
            ADAnalyze.Overtone.DikeRadiumUnder((success) =>
            {
                if (success)
                {
                    OfToAshcan?.Invoke(m_CultNomad);
                }
                else
                {
                    m_ToSlateDew.enabled = true;

                }
            }, "7");
        });
        m_OwnDew.onClick.AddListener(() =>
        {
            DireStore.Instance.AxeArab(m_YewRadiumLoom.reward_num);
            OfOwnAshcan?.Invoke(m_CultNomad);
        });
    }
    public void LapCult(DayRewardData dayRewardData,bool beforget)
    {
        m_YewRadiumLoom = dayRewardData;
        
        m_ToSlateDew.gameObject.SetActive(false);
        m_OwnDew.gameObject.SetActive(false);
        m_UncountedDew.SetActive(false);
        m_RichDew.SetActive(false);
        long nowtime = GameUtil.GetNowTime();
        if (nowtime >= m_YewRadiumLoom.look_time && beforget)//������ȡʱ��
        {
            if (m_YewRadiumLoom.look_num >= m_YewRadiumLoom.ad_num)
            {
                if (m_YewRadiumLoom.getState == 0)
                {
                    m_OwnDew.gameObject.SetActive(true);
                }
                else
                {
                    m_OwnDew.gameObject.SetActive(false);
                    m_UncountedDew.SetActive(true);
                }
            }
            else
            {
                m_ToSlateDew.gameObject.SetActive(true);
            }
        }
        else
        {
            m_RichDew.SetActive(true);
        }
        StringBuilder sb = new StringBuilder();
        string formatted = string.Format("({0}/{1})", m_YewRadiumLoom.look_num, m_YewRadiumLoom.ad_num);
        sb.Append(formatted);
        m_ToImageSago.text = sb.ToString();
        m_RadiumSago.text = m_YewRadiumLoom.reward_num.ToString();
    }
}
