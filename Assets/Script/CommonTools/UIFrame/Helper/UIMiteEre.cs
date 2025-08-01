/*
        主题： UI遮罩管理器  

        “弹出窗体”往往因为需要玩家优先处理弹出小窗体，则要求玩家不能(无法)点击“父窗体”，这种窗体就是典型的“模态窗体”
  5  *    Description: 
  6  *           功能： 负责“弹出窗体”模态显示实现
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIMiteEre : MonoBehaviour
{
    private static UIMiteEre _Overtone= null;
    //ui根节点对象
    private GameObject _MyEnergyJune= null;
    //ui脚本节点对象
    private Transform _AirUIFestiveKnit= null;
    //顶层面板
    private GameObject _MyBeStore;
    //遮罩面板
    private GameObject _MyMiteStore;
    //ui摄像机
    private Camera _UIBottom;
    //ui摄像机原始的层深
    private float _IncubateUIBottomLathe;
    //获取实例
    public static UIMiteEre GetInstance()
    {
        if (_Overtone == null)
        {
            _Overtone = new GameObject("_UIMaskMgr").AddComponent<UIMiteEre>();
        }
        return _Overtone;
    }
    private void Awake()
    {
        _MyEnergyJune = GameObject.FindGameObjectWithTag(OneStrand.SYS_TAG_CANVAS);
        _AirUIFestiveKnit = PatchPerson.FindDigCourtKnit(_MyEnergyJune, OneStrand.SYS_SCRIPTMANAGER_NODE);
        //把脚本实例，座位脚本节点对象的子节点
        PatchPerson.AxeCourtKnitBeTomatoKnit(_AirUIFestiveKnit, this.gameObject.transform);
        //获取顶层面板，遮罩面板
        _MyBeStore = _MyEnergyJune;
        _MyMiteStore = PatchPerson.FindDigCourtKnit(_MyEnergyJune, "_UIMaskPanel").gameObject;
        //得到uicamera摄像机原始的层深
        _UIBottom = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        if (_UIBottom != null)
        {
            //得到ui相机原始的层深
            _IncubateUIBottomLathe = _UIBottom.depth;
        }
        else
        {
            Debug.Log("UI_Camera is Null!,Please Check!");
        }
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    /// <param name="goDisplayUIForms">需要显示的ui窗体</param>
    /// <param name="lucenyType">显示透明度属性</param>
    public void FadMitePointe(GameObject goDisplayUIForms,UIFormLucenyType lucenyType = UIFormLucenyType.Lucency)
    {
        //顶层窗体下移
        _MyBeStore.transform.SetAsLastSibling();
        switch (lucenyType)
        {
               //完全透明 不能穿透
            case UIFormLucenyType.Lucency:
                _MyMiteStore.SetActive(true);
                Color newColor = new Color(255 / 255F, 255 / 255F, 255 / 255F, 0F / 255F);
                _MyMiteStore.GetComponent<Image>().color = newColor;
                break;
                //半透明，不能穿透
            case UIFormLucenyType.Translucence:
                _MyMiteStore.SetActive(true);
                Color newColor2 = new Color(0 / 255F, 0 / 255F, 0 / 255F, 220 / 255F);
                _MyMiteStore.GetComponent<Image>().color = newColor2;
                SharplyResortLyric.GetInstance().Arab(CFellow.Me_PointeYelp);
                break;
                //低透明，不能穿透
            case UIFormLucenyType.ImPenetrable:
                _MyMiteStore.SetActive(true);
                Color newColor3 = new Color(50 / 255F, 50 / 255F, 50 / 255F, 240F / 255F);
                _MyMiteStore.GetComponent<Image>().color = newColor3;
                break;
                //可以穿透
            case UIFormLucenyType.Penetrable:
                if (_MyMiteStore.activeInHierarchy)
                {
                    _MyMiteStore.SetActive(false);
                }
                break;
            default:
                break;
        }
        //遮罩窗体下移
        _MyMiteStore.transform.SetAsLastSibling();
        //显示的窗体下移
        goDisplayUIForms.transform.SetAsLastSibling();
        //增加当前ui摄像机的层深（保证当前摄像机为最前显示）
        if (_UIBottom != null)
        {
            _UIBottom.depth = _UIBottom.depth + 100;
        }
    }
    public void WarmMitePointe()
    {
        if (UIAnalyze.GetInstance().BoonUIFlank.Count > 0 || UIAnalyze.GetInstance().OwnSeminarDeerVocal().Count > 0)
        {
            return;
        }
        Color newColor3 = new Color(_MyMiteStore.GetComponent<Image>().color.r, _MyMiteStore.GetComponent<Image>().color.g, _MyMiteStore.GetComponent<Image>().color.b,0);
        _MyMiteStore.GetComponent<Image>().color = newColor3;
    }
    /// <summary>
    /// 取消遮罩状态
    /// </summary>
    public void SisterMitePointe()
    {
        if (UIAnalyze.GetInstance().BoonUIFlank.Count > 0 || UIAnalyze.GetInstance().OwnSeminarDeerVocal().Count > 0)
        {
            return;
        }
        // 检查是否有其他 PopUp 窗口正在显示
        bool hasOtherPopUp = false;
        var openingPanels = UIAnalyze.GetInstance().OwnExploitLovely(true);
        foreach (var panel in openingPanels)
        {
            var baseUIForm = panel.GetComponent<ComeUIFlank>();
            if (baseUIForm != null && baseUIForm.SeminarUIRate.UIForms_Type == UIFormType.PopUp)
            {
                hasOtherPopUp = true;
                // 将遮罩放在最后一个 PopUp 窗口下面
                _MyMiteStore.transform.SetAsLastSibling();
                panel.transform.SetAsLastSibling();
                break;
            }
        }

        // 只有在没有其他 PopUp 窗口时才关闭遮罩
        if (!hasOtherPopUp)
        {
            //顶层窗体上移
            _MyBeStore.transform.SetAsFirstSibling();
            //禁用遮罩窗体
            if (_MyMiteStore.activeInHierarchy)
            {
                _MyMiteStore.SetActive(false);
                SharplyResortLyric.GetInstance().Arab(CFellow.Me_PointeMedia);
            }
            //恢复当前ui摄像机的层深
            if (_UIBottom != null)
            {
                _UIBottom.depth = _IncubateUIBottomLathe;
            }
        }
    }
}
