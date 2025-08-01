/***
 * 
 * AudioSource组件管理(音效，背景音乐除外)
 * 
 * **/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeaveAvenueThink 
{
    //音乐的管理者
    private GameObject HeaveEre;
    //音乐组件管理队列
    private List<AudioSource> HeavePieceworkThink;
    //音乐组件默认容器最大值  
    private int MudImage= 25;
    public HeaveAvenueThink(PanelEre audioMgr)
    {
        HeaveEre = audioMgr.gameObject;
        VoltHeaveAvenueThink();
    }
  
    /// <summary>
    /// 初始化队列
    /// </summary>
    private void VoltHeaveAvenueThink()
    {
        HeavePieceworkThink = new List<AudioSource>();
        for(int i = 0; i < MudImage; i++)
        {
            AxeHeaveAvenueEonAlsoEre();
        }
    }
    /// <summary>
    /// 给音乐的管理者添加音频组件，同时组件加入队列
    /// </summary>
    private AudioSource AxeHeaveAvenueEonAlsoEre()
    {
        AudioSource audio = HeaveEre.AddComponent<AudioSource>();
        HeavePieceworkThink.Add(audio);
        return audio;
    }
    /// <summary>
    /// 获取一个音频组件
    /// </summary>
    /// <param name="audioMgr"></param>
    /// <returns></returns>
    public AudioSource OwnHeavePiecework()
    {
        if (HeavePieceworkThink.Count > 0)
        {
            AudioSource audio = HeavePieceworkThink.Find(t => !t.isPlaying);
            if (audio)
            {
                HeavePieceworkThink.Remove(audio);
                return audio;
            }
            //队列中没有了，需额外添加
            return AxeHeaveAvenueEonAlsoEre();
            //直接返回队列中存在的组件
            //return AudioComponentQueue.Dequeue();
        }
        else
        {
            //队列中没有了，需额外添加
            return  AxeHeaveAvenueEonAlsoEre();
        }
    }
    /// <summary>
    /// 没有被使用的音频组件返回给队列
    /// </summary>
    /// <param name="audio"></param>
    public void UnUrnHeavePiecework(AudioSource audio)
    {
        if (HeavePieceworkThink.Contains(audio)) return;
        if (HeavePieceworkThink.Count >= MudImage)
        {
            GameObject.Destroy(audio);
            //Debug.Log("删除组件");
        }
        else
        {
            audio.clip = null;
            HeavePieceworkThink.Add(audio);
        }

        //Debug.Log("队列长度是" + AudioComponentQueue.Count);
    }
    
}
