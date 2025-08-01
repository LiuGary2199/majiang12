using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLoomAnalyze : CopyVibration<FadeLoomAnalyze>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void VoltFadeLoom()
    {
    }

    // 金币
    public double MixBold()
    {

        return SameLoomAnalyze.OwnShovel(CFellow.Dy_BoldDumb);
    }

    public void LopBold(double gold)
    {
        LopBold(gold, ClanAnalyze.instance.transform);
    }

    public void LopBold(double gold, Transform startTransform)
    {
        double oldGold = SameLoomAnalyze.OwnShovel(CFellow.Dy_BoldDumb);
        SameLoomAnalyze.FadShovel(CFellow.Dy_BoldDumb, oldGold + gold);
        if (gold > 0)
        {
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionBoldDumb, SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionBoldDumb) + gold);
        }
        SharplyLoom md = new SharplyLoom(oldGold);
        md.ChileMeteoroid = startTransform;
        SharplyResortLyric.GetInstance().Arab(CFellow.Me_So_Arrange, md);
    }
    
    // 现金
    public double MixFlood()
    {
        //return SameLoomAnalyze.GetDouble(CFellow.sv_Token);
        return CashOutManager.GetInstance().Money;
    }

    public void LopFlood(double token)
    {
        CashOutManager.GetInstance().AddMoney((float)token);

        double oldToken = PlayerPrefs.HasKey(CFellow.Dy_Flood) ? double.Parse(SameLoomAnalyze.OwnChange(CFellow.Dy_Flood)) : 0;
        double newToken = oldToken + token;
        SameLoomAnalyze.FadShovel(CFellow.Dy_Flood, newToken);
        if (token > 0)
        {
            double allToken = SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionFlood);
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionFlood, allToken + token);
        }

        //addToken(token, ClanAnalyze.instance.transform);
    }
    public void LopFlood(double token, Transform startTransform)
    {
        double oldToken = PlayerPrefs.HasKey(CFellow.Dy_Flood) ? double.Parse(SameLoomAnalyze.OwnChange(CFellow.Dy_Flood)) : 0;
        double newToken = oldToken + token;
        SameLoomAnalyze.FadShovel(CFellow.Dy_Flood, newToken);
        if (token > 0)
        {
            double allToken = SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionFlood);
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionFlood, allToken + token);
        }
        SharplyLoom md = new SharplyLoom(oldToken);
        md.ChileMeteoroid = startTransform;
        SharplyResortLyric.GetInstance().Arab(CFellow.Me_So_Ordnance, md);
    }

    //Amazon卡
    public double MixMotion()
    {
        return SameLoomAnalyze.OwnShovel(CFellow.Dy_Amazon);
    }

    public void LopMotion(double amazon)
    {
        LopMotion(amazon, ClanAnalyze.instance.transform);
    }
    public void LopMotion(double amazon, Transform startTransform)
    {
        double oldAmazon = PlayerPrefs.HasKey(CFellow.Dy_Amazon) ? double.Parse(SameLoomAnalyze.OwnChange(CFellow.Dy_Amazon)) : 0;
        double newAmazon = oldAmazon + amazon;
        SameLoomAnalyze.FadShovel(CFellow.Dy_Amazon, newAmazon);
        if (amazon > 0)
        {
            double allAmazon = SameLoomAnalyze.OwnShovel(CFellow.Dy_AggressionMotion);
            SameLoomAnalyze.FadShovel(CFellow.Dy_AggressionMotion, allAmazon + amazon);
        }
        SharplyLoom md = new SharplyLoom(oldAmazon);
        md.ChileMeteoroid = startTransform;
        SharplyResortLyric.GetInstance().Arab(CFellow.Me_So_Retentive, md);
    }
}
