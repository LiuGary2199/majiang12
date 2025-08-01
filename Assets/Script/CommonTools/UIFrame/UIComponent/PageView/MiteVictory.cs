using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiteVictory : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("mask")]    public RectTransform Gnaw;
[UnityEngine.Serialization.FormerlySerializedAs("mypageview")]    public TaleSnap Antagonism;
    private void Awake()
    {
        Antagonism.OfTalePillow = Strengthen;
    }

    void Strengthen(int index)
    {
        if (index >= this.transform.childCount) return;
        Vector3 pos= this.transform.GetChild(index).GetComponent<RectTransform>().position;
        Gnaw.GetComponent<RectTransform>().position = pos;
    }
}
