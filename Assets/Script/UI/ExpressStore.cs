using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ExpressStore : MonoBehaviour
{
[UnityEngine.Serialization.FormerlySerializedAs("sliderImage")]    public Image AppealUtter;
[UnityEngine.Serialization.FormerlySerializedAs("progressText")]    public Text ForelimbSago;
    // Start is called before the first frame update
    void Start()
    {
        AppealUtter.fillAmount = 0;
        ForelimbSago.text = "0%";
        CashOutManager.GetInstance().StartTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    // Update is called once per frame
    void Update()
    {
        if (AppealUtter.fillAmount <= 0.8f || (MobTownEre.instance.Proof && CashOutManager.GetInstance().Ready))
        {
            AppealUtter.fillAmount += Time.deltaTime / 3f;
            ForelimbSago.text = (int)(AppealUtter.fillAmount * 100) + "%";
            if (AppealUtter.fillAmount >= 1)
            {
                FactorDram.UpHonor();
                Destroy(transform.parent.gameObject);
                ClanAnalyze.instance.LimeVolt();
                CashOutManager.GetInstance().ReportEvent_LoadingTime();
            }
        }
    }
}
