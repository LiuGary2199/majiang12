using UnityEngine;
using UnityEngine.UI;

public class FleeMiteStore : MonoBehaviour
{
    [Header("目标设置")]
[UnityEngine.Serialization.FormerlySerializedAs("targetObj")]    public GameObject RandomSun;
[UnityEngine.Serialization.FormerlySerializedAs("padding")]    public float Hostile= 10f; // 目标周围的边距

    [Header("动画设置")]
[UnityEngine.Serialization.FormerlySerializedAs("shrinkTime")]    public float UnsureRich= 0.3f;
[UnityEngine.Serialization.FormerlySerializedAs("targetOffsetX")]    public float RandomOffsetX;
[UnityEngine.Serialization.FormerlySerializedAs("targetOffsetY")]    public float RandomAdhereY;

    private Material Lifetime;
    private RectTransform RandomFlee;
    private Canvas RandomEnergy;
    private RectTransform GnawFlee;
[UnityEngine.Serialization.FormerlySerializedAs("currentOffsetX")]
    public float AcreageAdhereX;
[UnityEngine.Serialization.FormerlySerializedAs("currentOffsetY")]    public float AcreageAdhereY;
[UnityEngine.Serialization.FormerlySerializedAs("targetPosX")]    public float RandomOakX;
[UnityEngine.Serialization.FormerlySerializedAs("targetPosY")]    public float RandomOakY;

    private float UnsureExertionX= 0f;
    private float UnsureExertionY= 0f;
    private IntentlyTrialVolunteer StrutVolunteer;
    private bool IcySquashSun= false;

    private void Start()
    {
        GnawFlee = GetComponent<RectTransform>();
        Lifetime = GetComponent<Image>().material;
        StrutVolunteer = GetComponent<IntentlyTrialVolunteer>();

        // 检查是否有目标对象
        if (RandomSun != null)
        {
            RandomFlee = RandomSun.GetComponent<RectTransform>();
            if (RandomFlee != null)
            {
                RandomEnergy = RandomSun.GetComponentInParent<Canvas>();
                if (RandomEnergy != null)
                {
                    IcySquashSun = true;
                    GalaxySquashVoluminous();
                }
            }
        }

        if (!IcySquashSun)
        {
            // 原逻辑：使用Inspector中设置的参数
            Vector4 centerMat = new Vector4(RandomOakX, RandomOakY, 0, 0);
            Lifetime.SetVector("_Center", centerMat);
        }

        if (StrutVolunteer != null && IcySquashSun)
        {
            StrutVolunteer.FadSquashFlee(RandomFlee);
        }
    }

    private void Update()
    {
        if (IcySquashSun)
        {
            GalaxySquashVoluminous();
        }

        // 原逻辑：平滑动画
        float valueX = Mathf.SmoothDamp(AcreageAdhereX, RandomOffsetX, ref UnsureExertionX, UnsureRich);
        float valueY = Mathf.SmoothDamp(AcreageAdhereY, RandomAdhereY, ref UnsureExertionY, UnsureRich);

        if (!Mathf.Approximately(valueX, AcreageAdhereX))
        {
            AcreageAdhereX = valueX;
            Lifetime.SetFloat("_SliderX", AcreageAdhereX);
        }

        if (!Mathf.Approximately(valueY, AcreageAdhereY))
        {
            AcreageAdhereY = valueY;
            Lifetime.SetFloat("_SliderY", AcreageAdhereY);
        }
    }

    private void GalaxySquashVoluminous()
    {
        // 获取目标在世界空间的中心点
        Vector3 worldCenter = RandomFlee.TransformPoint(RandomFlee.rect.center);
        // 转换为屏幕空间坐标
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(RandomEnergy.worldCamera, worldCenter);

        // 转换为遮罩面板的本地坐标
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GnawFlee, screenPos, RandomEnergy.worldCamera, out localPos);

        // Debug输出详细信息
      //  Debug.Log($"[MaskPanel] 挖孔世界中心 worldCenter={worldCenter}, screenPos={screenPos}, localPos={localPos}, targetCanvas={targetCanvas}, worldCamera={targetCanvas.worldCamera}");
       // Debug.Log($"[MaskPanel] targetRect.position={targetRect.position}, sizeDelta={targetRect.sizeDelta}, rect={targetRect.rect}");

        // 设置遮罩中心为目标中心
        RandomOakX = localPos.x;
        RandomOakY = localPos.y;
        Lifetime.SetVector("_Center", new Vector4(RandomOakX, RandomOakY, 0, 0));

        // 设置遮罩大小为目标大小加上边距
        RandomOffsetX = (RandomFlee.rect.width / 2) + Hostile;
        RandomAdhereY = (RandomFlee.rect.height / 2) + Hostile;
    }

    // 外部调用：设置新的目标对象
    public void FadSquash(GameObject newTarget)
    {
        RandomSun = newTarget;

        if (RandomSun != null)
        {
            RandomFlee = RandomSun.GetComponent<RectTransform>();
            if (RandomFlee != null)
            {
                RandomEnergy = RandomSun.GetComponentInParent<Canvas>();
                if (RandomEnergy != null)
                {
                    IcySquashSun = true;
                    GalaxySquashVoluminous();

                    if (StrutVolunteer != null)
                    {
                        StrutVolunteer.FadSquashFlee(RandomFlee);
                    }
                }
            }
        }
        else
        {
            IcySquashSun = false;
        }
    }
}