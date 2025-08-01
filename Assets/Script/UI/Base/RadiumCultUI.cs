using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiumCultUI : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("Icon")]    public Image Page;
[UnityEngine.Serialization.FormerlySerializedAs("NumText")]    public Text BowSago;

    public void Seaway(Sprite icon, int num = 0)
    {
        Page.sprite = icon;
        if (num == 0) {
            BowSago.gameObject.SetActive(false);
        }
        else
        {
            BowSago.text = "+" + num.ToString();
            BowSago.gameObject.SetActive(true);
        }
    }
}
