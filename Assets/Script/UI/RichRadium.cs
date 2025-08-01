using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class RichRadium : ComeUIFlank
{
[UnityEngine.Serialization.FormerlySerializedAs("CloseButton")]    public Button MediaPiston;
[UnityEngine.Serialization.FormerlySerializedAs("timeRewardItems")]    public List<RichRadiumStill> KnotRadiumStill;
    List<DayRewardData> EarRadiumArrow= new List<DayRewardData>();

    // Start is called before the first frame update
    void Start()
    {
        MediaPiston.onClick.AddListener(() =>
        {
            MediaUIDeer(GetType().Name);
        });
    }

    public override void Display(object uiFormParams)
    {
        base.Display(uiFormParams);
        for (int i = 0; i < KnotRadiumStill.Count; i++)
        {
            RichRadiumStill rewardItem = KnotRadiumStill[i];
            rewardItem.Volt();
        }
        LapRichRadium();
    }

    public void OwnRadium(int rewardIndex)
    {
        double reward = EarRadiumArrow[rewardIndex].reward_num;
        DireStore.Instance.AxeArab(reward);
    }
    public void MowLoom()
    {
        string[] datas = new string[4];
        for (int i = 0; i < EarRadiumArrow.Count; i++)
        {
            DayRewardData oldData = EarRadiumArrow[i];
            string jsondata = JsonMapper.ToJson(oldData);
            datas[i] = jsondata;
        }
        SameLoomAnalyze.FadChangeUrban(CFellow.Dy_FieldRadium, datas);
        SharplyResort.ArabSharply(CFellow.Me_CheeseYewRadium, null);
    }

    public void LapRichRadium()
    {
        EarRadiumArrow.Clear();
        string[] datas = new string[4];
        datas = SameLoomAnalyze.OwnChangeUrban(CFellow.Dy_FieldRadium);
        long nowtime = GameUtil.GetNowTime();
        bool redState = false;
        for (int i = 0; i < datas.Length; i++)
        {
            string data = datas[i];
            RichRadiumStill rewardItem = KnotRadiumStill[i];
            DayRewardData dayData = JsonMapper.ToObject<DayRewardData>(data);
            EarRadiumArrow.Add(dayData);
            rewardItem.OfOwnAshcan = null;
            rewardItem.OfOwnAshcan = (ItemIndex) =>
            {
                BaskTrialGerman.GetInstance().ArabTrial("1008", (ItemIndex + 1).ToString());
                EarRadiumArrow[ItemIndex].getState = 1;
                OwnRadium(ItemIndex);
                MowLoom();
                LapRichRadium();
            };

            rewardItem.OfToAshcan = null;
            rewardItem.OfToAshcan = (ItemIndex) =>
            {
                EarRadiumArrow[ItemIndex].look_num += 1;
                MowLoom();
                LapRichRadium();
            };
            DayRewardData deforRewardData = EarRadiumArrow.Find(x => x.dataIndex == (i - 1));
            bool beforGet = true;
            if (deforRewardData != null)
            {
                beforGet = deforRewardData.getState == 1;
            }
            rewardItem.LapCult(dayData, beforGet);
            if (dayData.look_time > nowtime && beforGet)
            {

            }
        }
    }
}
