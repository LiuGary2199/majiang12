using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum TargetType
{
    Scene,
    UGUI
}
public enum LayoutType
{
    Sprite_First_Weight,
    Sprite_First_Height,
    Screen_First_Weight,
    Screen_First_Height,
    Bottom,
    Top,
    Left,
    Right
}
public enum RunTime
{
    Awake,
    Start,
    None
}
public class AutoAshcan : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("Target_Type")]    public TargetType Squash_Rate;
[UnityEngine.Serialization.FormerlySerializedAs("Layout_Type")]    public LayoutType Layout_Rate;
[UnityEngine.Serialization.FormerlySerializedAs("Run_Time")]    public RunTime Vie_Rich;
[UnityEngine.Serialization.FormerlySerializedAs("Layout_Number")]    public float Ashcan_Vacant;
    private void Awake()
    {
        if (Vie_Rich == RunTime.Awake)
        {
            AfricaEocene();
        }
    }
    private void Start()
    {
        if (Vie_Rich == RunTime.Start)
        {
            AfricaEocene();
        }
    }

    public void AfricaEocene()
    {
        if (Layout_Rate == LayoutType.Sprite_First_Weight)
        {
            if (Squash_Rate == TargetType.UGUI)
            {

                float scale = Screen.width / Ashcan_Vacant;
                //GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.width / w * h);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if (Layout_Rate == LayoutType.Screen_First_Weight)
        {
            if (Squash_Rate == TargetType.Scene)
            {
                float scale = OwnRegretLoom.GetInstance().MixBottomMason() / Ashcan_Vacant;
                transform.localScale = transform.localScale * scale;
            }
        }
        
        if (Layout_Rate == LayoutType.Bottom)
        {
            if (Squash_Rate == TargetType.Scene)
            {
                float screen_bottom_y = OwnRegretLoom.GetInstance().MixBottomDomain() / -2;
                screen_bottom_y += (Ashcan_Vacant + (OwnRegretLoom.GetInstance().MixNitricLoom(gameObject).y / 2f));
                transform.position = new Vector3(transform.position.x, screen_bottom_y, transform.position.y);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
