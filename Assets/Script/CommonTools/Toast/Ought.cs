using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ought : ComeUIFlank
{
[UnityEngine.Serialization.FormerlySerializedAs("ToastText")]    public Text OughtSago;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);

        OughtSago.text = uiFormParams.ToString();
        StartCoroutine(nameof(LensMediaOught));
    }

    private IEnumerator LensMediaOught()
    {
        yield return new WaitForSeconds(2);
        MediaUIDeer(GetType().Name);
    }

}
