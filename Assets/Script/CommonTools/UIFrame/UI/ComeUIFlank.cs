using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 基础UI窗体脚本（父类，其他窗体都继承此脚本）
/// </summary>
public class ComeUIFlank : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("_CurrentUIType")]    //当前（基类）窗口的类型
    public UIRate _SeminarUIRate= new UIRate();
    [HideInInspector]
[UnityEngine.Serialization.FormerlySerializedAs("close_button")]    public Button Total_Saturn;
    //属性，当前ui窗体类型
    internal UIRate SeminarUIRate    {
        set
        {
            _SeminarUIRate = value;
        }
        get
        {
            return _SeminarUIRate;
        }
    }
    protected virtual void Awake()
    {
        PikeCourtAxePiecework(gameObject);
        if (transform.Find("Window/Content/CloseBtn"))
        {
            Total_Saturn = transform.Find("Window/Content/CloseBtn").GetComponent<Button>();
            Total_Saturn.onClick.AddListener(() => {
                UIAnalyze.GetInstance().MediaAnPearlyUIFlank(this.GetType().Name);
            });
        }
        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        gameObject.name = GetType().Name;
    }


    public static void PikeCourtAxePiecework(GameObject goParent)
    {
        Transform parent = goParent.transform;
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform chile = parent.GetChild(i);
            if (chile.GetComponent<Button>())
            {
                chile.GetComponent<Button>().onClick.AddListener(() => {

                    PanelEre.GetInstance().InchBellow(PanelRate.UIMusic.Sound_UIButton);
                });
            }
            
            if (chile.childCount > 0)
            {
                PikeCourtAxePiecework(chile.gameObject);
            }
        }
    }

    //页面显示
    public virtual void Display(object uiFormParams)
    {
        //Debug.Log(this.GetType().Name);
        this.gameObject.SetActive(true);
        // 设置模态窗体调用(必须是弹出窗体)
        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp && _SeminarUIRate.UIForm_LucencyType != UIFormLucenyType.NoMask)
        {
            UIMiteEre.GetInstance().FadMitePointe(this.gameObject, _SeminarUIRate.UIForm_LucencyType);
        }
        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp)
        {

            //动画添加
            switch (_SeminarUIRate.UIForm_animationType)
            {
                case UIFormShowAnimationType.scale:
                    StoreroomSuccessful.DewBead(gameObject, () =>
                    {

                    });
                    break;

            }
            
        }
        //NewUserManager.GetInstance().TriggerEvent(TriggerType.panel_display);
    }
    //页面隐藏（不在栈集合中）
    public virtual void Hidding(System.Action finish = null)
    {
        //if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
        //{
        //    UIMiteEre.GetInstance().HideMaskWindow();
        //}

        //取消模态窗体调用

        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp)
        {
            switch (_SeminarUIRate.UIForm_animationType)
            {
                case UIFormShowAnimationType.scale:
                    StoreroomSuccessful.DewWarm(gameObject, () =>
                    {
                        this.gameObject.SetActive(false);
                        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp && _SeminarUIRate.UIForm_LucencyType != UIFormLucenyType.NoMask)
                        {
                            UIMiteEre.GetInstance().SisterMitePointe();
                        }
                        UIAnalyze.GetInstance().BeadLienDewHe();
                        finish?.Invoke();
                    });
                    break;
                case UIFormShowAnimationType.none:
                    this.gameObject.SetActive(false);
                    if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp && _SeminarUIRate.UIForm_LucencyType != UIFormLucenyType.NoMask)
                    {
                        UIMiteEre.GetInstance().SisterMitePointe();
                    }
                    UIAnalyze.GetInstance().BeadLienDewHe();
                    finish?.Invoke();
                    break;

            }

        }
        else
        {
            this.gameObject.SetActive(false);
            //if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
            //{
            //    UIMiteEre.GetInstance().CancelMaskWindow();
            //}
            finish?.Invoke();
        }
    }

    public virtual void Hidding()
    {
        Hidding(null);
    }

    //页面重新显示
    public virtual void Redisplay()
    {
        this.gameObject.SetActive(true);
        if (_SeminarUIRate.UIForms_Type == UIFormType.PopUp)
        {
            UIMiteEre.GetInstance().FadMitePointe(this.gameObject, _SeminarUIRate.UIForm_LucencyType); 
        }
    }
    //页面冻结（还在栈集合中）
    public virtual void Stocky()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 注册按钮事件
    /// </summary>
    /// <param name="buttonName">按钮节点名称</param>
    /// <param name="delHandle">委托，需要注册的方法</param>
    protected void SanskritPistonTypifyTrial(string buttonName,TrialRemodelIncoming.VoidDelegate delHandle)
    {
        GameObject goButton = PatchPerson.FindDigCourtKnit(this.gameObject, buttonName).gameObject;
        //给按钮注册事件方法
        if (goButton != null)
        {
            TrialRemodelIncoming.Own(goButton).onNever = delHandle;
        }
    }

    /// <summary>
    /// 打开ui窗体
    /// </summary>
    /// <param name="uiFormName"></param>
    protected void YelpUIDeer(string uiFormName)
    {
        UIAnalyze.GetInstance().BeadUIFlank(uiFormName);
    }

    /// <summary>
    /// 关闭当前ui窗体
    /// </summary>
    protected void MediaUIDeer(string uiFormName)
    {
        //处理后的uiform名称
        UIAnalyze.GetInstance().MediaAnPearlyUIFlank(uiFormName);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msgType">消息的类型</param>
    /// <param name="msgName">消息名称</param>
    /// <param name="msgContent">消息内容</param>
    protected void ArabSharply(string msgType,string msgName,object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        SharplyResort.ArabSharply(msgType, kvs);
    }

    /// <summary>
    /// 接受消息
    /// </summary>
    /// <param name="messageType">消息分类</param>
    /// <param name="handler">消息委托</param>
    public void NursingSharply(string messageType,SharplyResort.DelMessageDelivery handler)
    {
        SharplyResort.AxeCarIncoming(messageType, handler);
    }

    /// <summary>
    /// 显示语言
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string Bead(string id)
    {
        string strResult = string.Empty;
        strResult = ComplainEre.GetInstance().BeadSago(id);
        return strResult;
    }
}
