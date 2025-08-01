using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using Unity.VisualScripting;
using UnityEngine;

public class TileAnimController : MonoBehaviour
{
    [SerializeField] private AnimancerComponent m_Animancer;
    [SerializeField] private AnimationClip m_Nbroke;
    [SerializeField] private AnimationClip m_Gbroke;
    [SerializeField] private AnimationClip m_Load;
    [SerializeField] private AnimationClip m_Shak;
    AnimancerState state = null;
    public void PlayNbroke(Action Finishaction)
    {
        AnimancerState state = m_Animancer.Play(m_Nbroke);
        state.Events.OnEnd = () =>
        {
            Finishaction?.Invoke();
        };
    }
    
    public void PlayGbroke(Action Finishaction)
    {
        AnimancerState state = m_Animancer.Play(m_Gbroke);
        state.Events.OnEnd = () =>
        {
            Finishaction?.Invoke();
        };
    }

     public void PlayLoad()
    {
         state = m_Animancer.Play(m_Load);
    }
    public void stopLoad()
    {
        if (state == null) return;
        state.Stop();
    }

    public void PlayShak(Action Finishaction)
    {
        AnimancerState state = m_Animancer.Play(m_Shak);
        state.Events.OnEnd = () =>
        {
            Finishaction?.Invoke();
        };
    }

    private void Update()
    {
       
    }
}
