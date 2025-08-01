/***
 * 
 * 音乐管理器
 * 
 * **/
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEre : CopyVibration<PanelEre>
{
    //音频组件管理队列的对象
    private HeaveAvenueThink HeaveThink;
    // 用于播放背景音乐的音乐源
    private AudioSource m_bgPanel=null;
    //播放音效的音频组件管理列表
    private List<AudioSource> InchHeaveAvenueFoul;
    //检查已经播放的音频组件列表中没有播放的组件的更新频率
    private float SpoutEfficacy= 2f; 
    //背景音乐开关
    private bool _OxPanelDesire;
    //音效开关
    private bool _BellowPanelDesire;
    //音乐音量
    private float _OxImport=1f;
    //音效音量
    private float _BellowImport=1f;
    string BGM_Whig= "";

    public Dictionary<string, HeaveFaith> HeaveIngoingCope;

    // 控制背景音乐音量大小
    public float OxImport    {
        get { 
            return OxPanelDesire ? MixImport(BGM_Whig) : 0f; 
        }
        set {
            _OxImport = value;
            //背景音乐开的状态下，声音随控制调节
        }
    }

    //控制音效音量的大小
    public float BellowHyksos    {
        get { return _BellowImport; }
        set { 
            _BellowImport = value;
            FadAskBellowImport();
        }
    }
    //控制背景音乐开关
    public bool OxPanelDesire    {
        get {

            _OxPanelDesire = SameLoomAnalyze.OwnDeem("_BgMusicSwitch");
            return _OxPanelDesire; 
        }
        set {
            if(m_bgPanel)
            {
                _OxPanelDesire = value;
                SameLoomAnalyze.FadDeem("_BgMusicSwitch", _OxPanelDesire);
                m_bgPanel.volume = OxImport; 
            }
        }
    }
    public void MapSkyMediaSetRich()
    {
        m_bgPanel.volume = 0;
    }
    public void MapSkyAchieveSetRich()
    {
        m_bgPanel.volume = OxImport;
    }
    //控制音效开关
    public bool BellowPanelDesire    {
        get {
            _BellowPanelDesire = SameLoomAnalyze.OwnDeem("_EffectMusicSwitch");
            return _BellowPanelDesire; 
        }
        set {
            _BellowPanelDesire = value;
            SameLoomAnalyze.FadDeem("_EffectMusicSwitch", _BellowPanelDesire);
            
        }
    }
    public PanelEre()
    {
        InchHeaveAvenueFoul = new List<AudioSource>();      
    }
    protected override void Awake()
    {
        if (!PlayerPrefs.HasKey("first_music_setBool") || !SameLoomAnalyze.OwnDeem("first_music_set"))
        {
            SameLoomAnalyze.FadDeem("first_music_set", true);
            SameLoomAnalyze.FadDeem("_BgMusicSwitch", true);
            SameLoomAnalyze.FadDeem("_EffectMusicSwitch", true);
        }
        HeaveThink = new HeaveAvenueThink(this);

        TextAsset json = Resources.Load<TextAsset>("Audio/AudioInfo");
        HeaveIngoingCope = JsonMapper.ToObject<Dictionary<string, HeaveFaith>>(json.text);
    }
    private void Start()
    {
        StartCoroutine("SpoutIDUrnHeavePiecework");
    }
    /// <summary>
    /// 定时检查没有使用的音频组件并回收
    /// </summary>
    /// <returns></returns>
    IEnumerator SpoutIDUrnHeavePiecework()
    {
        while (true)
        {
            //定时更新
            yield return new WaitForSeconds(SpoutEfficacy);
            for (int i = 0; i < InchHeaveAvenueFoul.Count; i++)
            {
                //防止数据越界
                if (i < InchHeaveAvenueFoul.Count)
                {
                    //确保物体存在
                    if (InchHeaveAvenueFoul[i])
                    {
                        //音频为空或者没有播放为返回队列条件
                        if ((InchHeaveAvenueFoul[i].clip == null || !InchHeaveAvenueFoul[i].isPlaying))
                        {
                            //返回队列
                            HeaveThink.UnUrnHeavePiecework(InchHeaveAvenueFoul[i]);
                            //从播放列表中删除
                            InchHeaveAvenueFoul.Remove(InchHeaveAvenueFoul[i]);
                        }
                    }
                    else
                    {
                        //移除在队列中被销毁但是是在list中存在的垃圾数据
                        InchHeaveAvenueFoul.Remove(InchHeaveAvenueFoul[i]);
                    }                 
                }            
               
            }
        }
    }
    /// <summary>
    /// 设置当前播放的所有音效的音量
    /// </summary>
    private void FadAskBellowImport()
    {
        for (int i = 0; i < InchHeaveAvenueFoul.Count; i++)
        {
            if (InchHeaveAvenueFoul[i] && InchHeaveAvenueFoul[i].isPlaying)
            {
                InchHeaveAvenueFoul[i].volume = _BellowPanelDesire ? _BellowImport : 0f;
            }
        }
    }
    /// <summary>
    /// 播放背景音乐，传进一个音频剪辑的name
    /// </summary>
    /// <param name="bgName"></param>
    /// <param name="restart"></param>
    private void InchOxCome(object bgName, bool restart = false)
    {

        BGM_Whig = bgName.ToString();
        if (m_bgPanel == null)
        {
            //拿到一个音频组件  背景音乐组件在某一时间段唯一存在
            m_bgPanel = HeaveThink.OwnHeavePiecework();
            //开启循环
            m_bgPanel.loop = true;
            //开始播放
            m_bgPanel.playOnAwake = false;
            //加入播放列表
            //PlayAudioSourceList.Add(m_bgMusic);
        }

        if (!OxPanelDesire)
        {
            m_bgPanel.volume = 0;
        }

        //定义一个空的字符串
        string curBgName = string.Empty;
        //如果这个音乐源的音频剪辑不为空的话
        if (m_bgPanel.clip != null)
        {
            //得到这个音频剪辑的name
            curBgName = m_bgPanel.clip.name;
        }

        // 根据用户的音频片段名称, 找到AuioClip, 然后播放,
        //ResourcesMgr是提前定义好的查找音频剪辑对应路径的单例脚本，并动态加载出来
        AudioClip clip = Resources.Load<AudioClip>(HeaveIngoingCope[BGM_Whig].filePath);
        //如果找到了，不为空
        if (clip != null)
        {
            //如果这个音频剪辑已经复制给类音频源，切正在播放，那么直接跳出
            if (clip.name == curBgName && !restart)
            {
                return;
            }
            //否则，把改音频剪辑赋值给音频源，然后播放
            m_bgPanel.clip = clip;
            m_bgPanel.volume = OxImport;
            m_bgPanel.Play();
        }
        else
        {
            //没找到直接报错
            // 异常, 调用写日志的工具类.
            //UnityEngine.Debug.Log("没有找到音频片段");
            if (m_bgPanel.isPlaying)
            {
                m_bgPanel.Stop();
            }
            m_bgPanel.clip = null;
        }
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="defAudio"></param>
    /// <param name="volume"></param>
    private void InchBellowCome(object effectName, bool defAudio = true, float volume = 1f)
    {
        if (!BellowPanelDesire)
        {
            return;
        }
        //获取音频组件
        AudioSource m_effectMusic = HeaveThink.OwnHeavePiecework();
        if (m_effectMusic.isPlaying) {
            //Debug.Log("-------------------------------当前音效正在播放,直接返回");
            return;
        };
        m_effectMusic.loop = false;
        m_effectMusic.playOnAwake = false;
        m_effectMusic.volume = MixImport(effectName.ToString());
        //Debug.Log(m_effectMusic.volume);
        //根据查找路径加载对应的音频剪辑
        AudioClip clip = Resources.Load<AudioClip>(HeaveIngoingCope[effectName.ToString()].filePath);
        //如果为空的话，直接报错，然后跳出
        if (clip == null)
        {
            //UnityEngine.Debug.Log("没有找到音效片段");
            //没加入播放列表直接返回给队列
            HeaveThink.UnUrnHeavePiecework(m_effectMusic);
            return;
        }
        m_effectMusic.clip = clip;
        //加入播放列表
        InchHeaveAvenueFoul.Add(m_effectMusic);
        //否则，就是clip不为空的话，如果defAudio=true，直接播放
        if (defAudio)
        {
            m_effectMusic.PlayOneShot(clip, volume);
        }
        else
        {
            //指定点播放
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
        }
    }

    //播放各种音频剪辑的调用方法，PanelRate是提前写好的存放各种音乐名称的枚举类，便于外面直接调用
    public void InchOx(PanelRate.UIMusic bgName, bool restart = false)
    {
        InchOxCome(bgName, restart);
    }

    public void InchOx(PanelRate.SceneMusic bgName, bool restart = false)
    {
        InchOxCome(bgName, restart);
    }

    //播放各种音频剪辑的调用方法，PanelRate是提前写好的存放各种音乐名称的枚举类，便于外面直接调用
    public void InchBellow(PanelRate.UIMusic effectName, bool defAudio = true, float volume = 1f)
    {
        InchBellowCome(effectName, defAudio, volume);
    }

    public void InchBellow(PanelRate.SceneMusic effectName, bool defAudio = true, float volume = 1f)
    {
        InchBellowCome(effectName, defAudio, volume);
    }
    float MixImport(string name)
    {
        if (HeaveIngoingCope == null)
        {
            TextAsset json = Resources.Load<TextAsset>("Audio/AudioInfo");
            HeaveIngoingCope = JsonMapper.ToObject<Dictionary<string, HeaveFaith>>(json.text);
        }

        if (HeaveIngoingCope.ContainsKey(name))
        {
             return (float)HeaveIngoingCope[name].volume;

        }
        else
        {
            return 1;
        }
    }

}