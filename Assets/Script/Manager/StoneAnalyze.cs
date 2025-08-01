using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAnalyze : MonoBehaviour
{
    public static StoneAnalyze Overtone{ get; private set; }

    // ��ʱ�����ݽṹ
    private class TimerData
    {
        public int At;                  // ��ʱ��ID
        public float Efficacy;          // ���ʱ�䣨�룩
        public Action OfSoil;           // ÿ�δ����Ļص�
        public bool IsDandelion;        // �Ƿ��ظ�
        public bool UpDirect;           // �Ƿ���ͣ
        public float DisappearRich;     // ʣ��ʱ��
        public Coroutine Diversify;     // Э������
    }

    private readonly Dictionary<int, TimerData> _Sunset= new Dictionary<int, TimerData>();
    private int _FlopStoneAt= 1;

    private void Awake()
    {
        if (Overtone != null && Overtone != this)
        {
            Destroy(gameObject);
            return;
        }
        Overtone = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ������ʱ���������̣߳�
    /// </summary>
    /// <param name="interval">���ʱ�䣨�룩</param>
    /// <param name="onTick">ÿ�δ����Ļص�</param>
    /// <param name="isRepeating">�Ƿ��ظ�����</param>
    /// <param name="immediateFirstTick">�Ƿ�����������һ��</param>
    /// <returns>��ʱ��ID</returns>
    public int WoodyStone(float interval, Action onTick, bool isRepeating = false, bool immediateFirstTick = false)
    {
        int timerId = _FlopStoneAt++;
        var timerData = new TimerData
        {
            At = timerId,
            Efficacy = interval,
            OfSoil = onTick,
            IsDandelion = isRepeating,
            UpDirect = false,
            DisappearRich = interval
        };

        // ����Э�̣������߳�ִ�У�
        timerData.Diversify = StartCoroutine(StoneDiversify(timerData, immediateFirstTick));
        _Sunset.Add(timerId, timerData);

        return timerId;
    }

    // ��ʱ��Э�̣������̣߳�
    private IEnumerator StoneDiversify(TimerData data, bool immediateFirstTick)
    {
        // �Ƿ�����������һ��
        if (immediateFirstTick)
        {
            data.OfSoil?.Invoke();
            if (!data.IsDandelion) yield break; // ���ظ�ģʽ�£��������������
        }

        // ѭ����ʱ
        while (true)
        {
            // �ȴ�ָ��ʱ�䣨ʹ�� unscaledTime ����ʱ������Ӱ�죩
            yield return new WaitForSecondsRealtime(data.Efficacy);

            // ����Ƿ��ѱ���ͣ/ֹͣ
            if (data.UpDirect || !_Sunset.ContainsKey(data.At)) yield break;

            // �����ص�
            data.OfSoil?.Invoke();

            // ���ظ�ģʽ�£����������
            if (!data.IsDandelion)
            {
                PaneStone(data.At);
                yield break;
            }
        }
    }

    /// <summary>
    /// ��ͣ��ʱ��
    /// </summary>
    public void HingeStone(int timerId)
    {
        if (_Sunset.TryGetValue(timerId, out var data) && !data.UpDirect)
        {
            data.UpDirect = true;
            StopCoroutine(data.Diversify); // ֹͣ��ǰЭ��
        }
    }

    /// <summary>
    /// �ָ���ʱ��
    /// </summary>
    public void CowboyStone(int timerId)
    {
        if (_Sunset.TryGetValue(timerId, out var data) && data.UpDirect)
        {
            data.UpDirect = false;
            // ��������Э�̣�������ʱ
            data.Diversify = StartCoroutine(StoneDiversify(data, false));
        }
    }

    /// <summary>
    /// ֹͣ���Ƴ���ʱ��
    /// </summary>
    public void PaneStone(int timerId)
    {
        if (_Sunset.TryGetValue(timerId, out var data))
        {
            StopCoroutine(data.Diversify); // ֹͣЭ��
            _Sunset.Remove(timerId);       // ���ֵ��Ƴ�
        }
    }

    /// <summary>
    /// ֹͣ���м�ʱ��
    /// </summary>
    public void PaneAllMonroe()
    {
        foreach (var data in _Sunset.Values)
        {
            StopCoroutine(data.Diversify);
        }
        _Sunset.Clear();
    }

    /// <summary>
    /// ����ʱ���Ƿ�������
    /// </summary>
    public bool UpForeign(int timerId)
    {
        return _Sunset.TryGetValue(timerId, out var data) && !data.UpDirect;
    }
}