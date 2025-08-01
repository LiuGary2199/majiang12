using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoxTiltFlintStore : ComeUIFlank
{
    public static FoxTiltFlintStore instance;
[UnityEngine.Serialization.FormerlySerializedAs("Hand")]
    public GameObject Bark;

    /// <summary>
    /// 高亮显示目标
    /// </summary>
    private GameObject Random;
[UnityEngine.Serialization.FormerlySerializedAs("Text")]
    public Text Sago;
    /// <summary>
    /// 区域范围缓存
    /// </summary>
    private Vector3[] Auction= new Vector3[4];
    /// <summary>
    /// 最终的偏移x
    /// </summary>
    private float RandomOffsetX= 0;
    /// <summary>
    /// 最终的偏移y
    /// </summary>
    private float RandomAdhereY= 0;
    /// <summary>
    /// 遮罩材质
    /// </summary>
    private Material Lifetime;
    /// <summary>
    /// 当前的偏移x
    /// </summary>
    private float AcreageAdhereX= 0f;
    /// <summary>
    /// 当前的偏移y
    /// </summary>
    private float AcreageAdhereY= 0f;
    /// <summary>
    /// 高亮区域缩放的动画时间
    /// </summary>
    private float UnsureRich= 0.1f;
    /// <summary>
    /// 事件渗透组件
    /// </summary>
    private IntentlyTrialVolunteer StrutVolunteer;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 显示引导遮罩
    /// </summary>
    /// <param name="_target">要引导到的目标对象</param>
    /// <param name="text">引导说明文案</param>

    public void BeadFlint(GameObject _target, string text)
    {
        if (_target == null)
        {
            Bark.SetActive(false);
            if (Lifetime == null)
            {
                Lifetime = GetComponent<Image>().material;
            }
            Lifetime.SetVector("_Center", new Vector4(0, 0, 0, 0));
            Lifetime.SetFloat("_SliderX", 0);
            Lifetime.SetFloat("_SliderY", 0);
            // 如果没有target，点击任意区域关闭引导
            GetComponent<Button>().onClick.AddListener(() =>
            {
                MediaUIDeer(GetType().Name);
            });
        }
        else
        {
            DOTween.Kill("NewUserHandAnimation");
            Volt(_target);
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        if (!string.IsNullOrEmpty(text))
        {
            Sago.text = text;
            Sago.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            Sago.transform.parent.gameObject.SetActive(false);
        }
    }

    private float RandomMason= 1;
    private float RandomDomain= 1;
    public void Volt(GameObject _target)
    {
        this.Random = _target;

        StrutVolunteer = GetComponent<IntentlyTrialVolunteer>();
        // 删除 eventPenetrate.SetTargetImage(_target.GetComponent<Image>()); 相关调用

        Canvas canvas = UIAnalyze.GetInstance().ClanEnergy.GetComponent<Canvas>();

        //获取高亮区域的四个顶点的世界坐标
        if (Random.GetComponent<RectTransform>() != null)
        {
            Random.GetComponent<RectTransform>().GetWorldCorners(Auction);
        }
        else
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(_target.transform.position);
            pos = UIAnalyze.GetInstance()._AirUIBottom.GetComponent<Camera>().ScreenToWorldPoint(pos);
            Auction[0] = new Vector3(pos.x - RandomMason, pos.y - RandomDomain);
            Auction[1] = new Vector3(pos.x - RandomMason, pos.y + RandomDomain);
            Auction[2] = new Vector3(pos.x + RandomMason, pos.y + RandomDomain);
            Auction[3] = new Vector3(pos.x + RandomMason, pos.y - RandomDomain);
        }
        //计算高亮显示区域在画布中的范围
        RandomOffsetX = Vector2.Distance(ShadeBeEnergyOak(canvas, Auction[0]), ShadeBeEnergyOak(canvas, Auction[3])) / 2f;
        RandomAdhereY = Vector2.Distance(ShadeBeEnergyOak(canvas, Auction[0]), ShadeBeEnergyOak(canvas, Auction[1])) / 2f;
        //计算高亮显示区域的中心
        float x = Auction[0].x + ((Auction[3].x - Auction[0].x) / 2);
        float y = Auction[0].y + ((Auction[1].y - Auction[0].y) / 2);
        Vector3 centerWorld = new Vector3(x, y, 0);
        Vector2 Sleepy= ShadeBeEnergyOak(canvas, centerWorld);
        //设置遮罩材质中的中心变量
        Vector4 centerMat = new Vector4(Sleepy.x, Sleepy.y, 0, 0);
        Lifetime = GetComponent<Image>().material;
        Lifetime.SetVector("_Center", centerMat);
        //计算当前高亮显示区域的半径
        RectTransform canRectTransform = canvas.transform as RectTransform;
        if (canRectTransform != null)
        {
            //获取画布区域的四个顶点
            canRectTransform.GetWorldCorners(Auction);
            //计算偏移初始值
            for (int i = 0; i < Auction.Length; i++)
            {
                if (i % 2 == 0)
                {
                    AcreageAdhereX = Mathf.Max(Vector3.Distance(ShadeBeEnergyOak(canvas, Auction[i]), Sleepy), AcreageAdhereX);
                }
                else
                {
                    AcreageAdhereY = Mathf.Max(Vector3.Distance(ShadeBeEnergyOak(canvas, Auction[i]), Sleepy), AcreageAdhereY);
                }
            }
        }
        //设置遮罩材质中当前偏移的变量
        Lifetime.SetFloat("_SliderX", AcreageAdhereX);
        Lifetime.SetFloat("_SliderY", AcreageAdhereY);
        Bark.transform.localScale = new Vector3(1, 1, 1);
        StartCoroutine(BeadBark(Sleepy));
    }

    private IEnumerator BeadBark(Vector2 center)
    {
        Bark.SetActive(false);
        yield return new WaitForSeconds(UnsureRich);

        Bark.transform.localPosition = center;
        BarkStoreroom();

        Bark.SetActive(true);
    }
    /// <summary>
    /// 收缩速度
    /// </summary>
    private float UnsureExertionX= 0f;
    private float UnsureExertionY= 0f;
    private void Update()
    {
        if (Lifetime == null) return;

        AcreageAdhereX = RandomOffsetX;
        Lifetime.SetFloat("_SliderX", AcreageAdhereX);
        AcreageAdhereY = RandomAdhereY;
        Lifetime.SetFloat("_SliderY", AcreageAdhereY);
        //从当前偏移量到目标偏移量差值显示收缩动画
        //float valueX = Mathf.SmoothDamp(currentOffsetX, targetOffsetX, ref shrinkVelocityX, shrinkTime);
        //float valueY = Mathf.SmoothDamp(currentOffsetY, targetOffsetY, ref shrinkVelocityY, shrinkTime);
        //if (!Mathf.Approximately(valueX, currentOffsetX))
        //{
        //    currentOffsetX = valueX;
        //    material.SetFloat("_SliderX", currentOffsetX);
        //}
        //if (!Mathf.Approximately(valueY, currentOffsetY))
        //{
        //    currentOffsetY = valueY;
        //    material.SetFloat("_SliderY", currentOffsetY);
        //}


    }

    /// <summary>
    /// 世界坐标转换为画布坐标
    /// </summary>
    /// <param name="canvas">画布</param>
    /// <param name="world">世界坐标</param>
    /// <returns></returns>
    private Vector2 ShadeBeEnergyOak(Canvas canvas, Vector3 world)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
        return position;
    }

    public void BarkStoreroom()
    {

        var s = DOTween.Sequence();
        s.Append(Bark.transform.DOLocalMoveY(Bark.transform.localPosition.y + 10f, 0.5f));
        s.Append(Bark.transform.DOLocalMoveY(Bark.transform.localPosition.y, 0.5f));
        s.Join(Bark.transform.DOScaleY(1.1f, 0.125f));
        s.Join(Bark.transform.DOScaleX(0.9f, 0.125f).OnComplete(() =>
        {
            Bark.transform.DOScaleY(0.9f, 0.125f);
            Bark.transform.DOScaleX(1.1f, 0.125f).OnComplete(() =>
            {
                Bark.transform.DOScale(1f, 0.125f);
            });
        }));
        s.SetLoops(-1);
        s.SetId("NewUserHandAnimation");
    }

    public void OnDisable()
    {
        DOTween.Kill("NewUserHandAnimation");
    }
}
