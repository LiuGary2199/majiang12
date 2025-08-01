using System.Runtime.InteropServices;
using UnityEngine;

public class MoodDyAnalyze : MonoBehaviour
{
    public static MoodDyAnalyze instance;
    [UnityEngine.Serialization.FormerlySerializedAs("appid")]
    public string There;
    //获取IOS函数声明
#if UNITY_IOS
    [DllImport("__Internal")]
    internal extern static void openRateUsUrl(string appId);
#endif

    private void Awake()
    {
        instance = this;
    }

    public void YelpAPVanOpenly()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        Application.OpenURL("market://details?id=" + There);
#elif UNITY_IOS
        openRateUsUrl(There);
#endif
    }
}
