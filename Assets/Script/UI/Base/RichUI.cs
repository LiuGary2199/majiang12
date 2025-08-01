using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RichUI : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("ClockText")]    public Text BrickSago;
[UnityEngine.Serialization.FormerlySerializedAs("Pointer")]    public RectTransform Federal;

    private long Shortwave;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void VoltAnyRich(long endTime)
    {
        Shortwave = endTime - FoodDram.Seminar();

        StopCoroutine(nameof(AttemptBrick));
        StartCoroutine(nameof(AttemptBrick));
    }

    private IEnumerator AttemptBrick()
    {
        float angle = 0;
        while(Shortwave > 0)
        {
            BrickSago.text = FoodDram.EcologyArmory(Shortwave);
            Federal.DORotate(new Vector3(0, 0, angle), 0.5f);
            angle = angle - 90 == -360 ? 0 : angle - 90;
            Shortwave--;
            yield return new WaitForSeconds(1);
        }
        if (Shortwave <= 0)
        {
            BrickSago.text = "Finished";
            Federal.rotation = Quaternion.identity;
        }
    }
}
