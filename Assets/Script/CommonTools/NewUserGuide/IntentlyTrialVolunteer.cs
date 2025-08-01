using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 事件渗透
/// </summary>
public class IntentlyTrialVolunteer : MonoBehaviour, ICanvasRaycastFilter
{
    private RectTransform RandomFlee;
[UnityEngine.Serialization.FormerlySerializedAs("isclick")]    public bool Outline= false;

    public void FadSquashFlee(RectTransform rect)
    {
        RandomFlee = rect;
        Outline = false;
    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (RandomFlee == null)
        {
            Debug.Log("[Penetrate] targetRect is null, return false");
            return false;
        }
        bool inHole = RectTransformUtility.RectangleContainsScreenPoint(RandomFlee, sp, eventCamera);
        
        //Debug.Log($"[Penetrate] sp={sp}, eventCamera={eventCamera}, targetRect={targetRect}, inHole={inHole}");
        return inHole;
    }
}