using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ClientGild : ComeUIFlank
{
    public static ClientGild Instance;
[UnityEngine.Serialization.FormerlySerializedAs("rewardText")]
    public Text CellarSago;

   
    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
    }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
    }

    public void VoltLoom(double num)
    {
        CellarSago.text = num.ToString();
    }
    public override void Hidding()
    {
        base.Hidding();
    }
}