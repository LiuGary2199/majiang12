using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoodDyStore : ComeUIFlank
{
[UnityEngine.Serialization.FormerlySerializedAs("Stars")]    public Button[] Excel;
[UnityEngine.Serialization.FormerlySerializedAs("star1Sprite")]    public Sprite Aide1Nitric;
[UnityEngine.Serialization.FormerlySerializedAs("star2Sprite")]    public Sprite Aide2Nitric;
[UnityEngine.Serialization.FormerlySerializedAs("CLoseBtn")]    public Button CGlowDew;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Button star in Excel)
        {
            star.onClick.AddListener(() =>
            {
                string indexStr = System.Text.RegularExpressions.Regex.Replace(star.gameObject.name, @"[^0-9]+", "");
                int index = indexStr == "" ? 0 : int.Parse(indexStr);
                DwarfWoody(index);
                BaskTrialGerman.GetInstance().ArabTrial("1010", (index + 1).ToString());

            });
        }
        CGlowDew.onClick.AddListener(() =>
        {
            MediaUIDeer(GetType().Name);
        });
    }

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        for (int i = 0; i < 5; i++)
        {
            Excel[i].gameObject.GetComponent<Image>().sprite = Aide2Nitric;
        }
    }


    private void DwarfWoody(int index)
    {
        for (int i = 0; i < 5; i++)
        {
            Excel[i].gameObject.GetComponent<Image>().sprite = i <= index ? Aide1Nitric : Aide2Nitric;
        }
        BaskTrialGerman.GetInstance().ArabTrial("1301", (index + 1).ToString());
        if (index < 3)
        {
            StartCoroutine(TotalStore());
        }
        else
        {
            // 跳转到应用商店
            MoodDyAnalyze.instance.YelpAPVanOpenly();
            StartCoroutine(TotalStore());
        }

        // 打点
        //BaskTrialGerman.GetInstance().SendEvent("1210", (index + 1).ToString());
    }

    IEnumerator TotalStore(float waitTime = 0.5f)
    {
        yield return new WaitForSeconds(waitTime);
        MediaUIDeer(GetType().Name);
    }
}
