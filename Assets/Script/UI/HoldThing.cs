using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldThing : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("InitGroup")]    public GameObject VoltThing;

    private GameObject AllegorySoundTypify;
    private float DietMason= 120f; // 两个item的position.x之差

    // Start is called before the first frame update
    void Start()
    {
        AllegorySoundTypify = VoltThing.transform.Find("SlotCard_1").gameObject;
        float x = DietMason * 3;
        int multiCount = MobTownEre.instance.VoltLoom.slot_group.Count;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < multiCount; j++)
            {
                GameObject fangkuai = Instantiate(AllegorySoundTypify, VoltThing.transform);
                fangkuai.transform.localPosition = new Vector3(x + DietMason * multiCount * i + DietMason * j, AllegorySoundTypify.transform.localPosition.y, 0);
                fangkuai.transform.Find("Text").GetComponent<Text>().text = "×" + MobTownEre.instance.VoltLoom.slot_group[j].multi;
            }
        }
    }

    public void TeamSound()
    {
        VoltThing.GetComponent<RectTransform>().localPosition = new Vector3(0, -10, 0);
    }

    public void Deck(int index, Action<int> finish)
    {
        PanelEre.GetInstance().InchBellow(PanelRate.UIMusic.Sound_OneArmBandit);
        StoreroomSuccessful.HenceforthUnfair(VoltThing, -(DietMason * 2 + DietMason * MobTownEre.instance.VoltLoom.slot_group.Count * 3 + DietMason * (index + 1)), () =>
        {
            finish?.Invoke(MobTownEre.instance.VoltLoom.slot_group[index].multi);
        });
    }
}
