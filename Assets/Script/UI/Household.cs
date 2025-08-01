using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System.Text;
using DG.Tweening;
using Spine;


public class Household : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic m_EnforcerUpdraft;
[UnityEngine.Serialization.FormerlySerializedAs("textsObj")]    public GameObject TraitSun;
[UnityEngine.Serialization.FormerlySerializedAs("numberText")]    public Text CooperSago;

    [Header("Ч������")]
    private Vector3 IdentityRaise;
    private Tween AcreageTower;
    [SerializeField] private float CandidateRaise= 1.3f;   // ��������ֵ
    [SerializeField] private float Shortage= 0.6f;         // ������ʱ��
    [SerializeField] private Ease LoudNo= Ease.OutQuad;    // ��0��overshoot�Ļ�������
    [SerializeField] private Ease LoudBut= Ease.InQuad;    // ��overshoot��1�Ļ�������
    public void InchDebt(int number)
    {
        // 防止对象被隐藏或销毁时操作Tween
        if (!gameObject.activeInHierarchy || TraitSun == null || m_EnforcerUpdraft == null) return;

        // Kill旧Tween（更健壮）
        if (AcreageTower != null && AcreageTower.IsActive())
            AcreageTower.Kill();
        DG.Tweening.DOTween.Kill(this);

        m_EnforcerUpdraft.gameObject.SetActive(false);
        m_EnforcerUpdraft.gameObject.SetActive(true);
        m_EnforcerUpdraft.AnimationState.ClearTracks();
        m_EnforcerUpdraft.AnimationState.SetAnimation(0, "animation", false);
        IdentityRaise = Vector3.one;
        TraitSun.gameObject.SetActive(true);
        TraitSun.transform.localScale = Vector3.zero;
        StringBuilder stringBuilder = new StringBuilder(); 
        stringBuilder.Append("x");
        stringBuilder.Append(number);
        CooperSago.text = stringBuilder.ToString();
        Sequence sequence = DOTween.Sequence().SetId(this);
        sequence.Append(TraitSun.transform.DOScale(IdentityRaise * CandidateRaise, Shortage * 0.35f)
            .SetEase(LoudNo));
        sequence.Append(TraitSun.transform.DOScale(IdentityRaise, 0.1f)
            .SetEase(LoudBut));
        sequence.Insert(0.8f, DOVirtual.DelayedCall(0.1f, () => { }));
        sequence.OnComplete(() =>
        {
            if (TraitSun != null)
                TraitSun.transform.localScale = Vector3.zero;
        });
        AcreageTower = sequence;
    }
    public void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            InchDebt(1);
        }
    }

    private void OnDestroy()
    {
        if (AcreageTower != null && AcreageTower.IsActive())
            AcreageTower.Kill();
        DG.Tweening.DOTween.Kill(this);
    }
}
