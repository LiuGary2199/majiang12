using UnityEngine;
using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    /// <summary>
    /// 成就基类，包含成就计数、奖励、阶段、事件等通用逻辑
    /// </summary>
    public class Achievement : MonoBehaviour
    {
        [SerializeField]
        private int targetCount; // 目标次数
        [SerializeField]
        private PopUpsController  rewardPrefabPU; // 奖励弹窗预制体
        [SerializeField]
        private AchievementsLine achievementsLinePrefab; // 成就GUI行预制体
        [SerializeField]
        private bool resetAfterGetReward; // 领奖后是否重置
        [SerializeField]
        private readonly bool dLog; // 是否调试日志

        #region default
        private string prefix = "achievement_";
        private string SaveStageName { get { return prefix + "stage_" + GetUniqueName(); } } // 阶段存储名
        private string SaveCountName { get { return prefix + "count_" + GetUniqueName(); } } // 计数存储名
        private string SaveRewardReceivedName { get { return prefix + "received_" + GetUniqueName(); } } // 奖励存储名
        #endregion default

        #region properties
        public bool TargetAchieved { get { return CurrentCount >= TargetCount; } } // 是否达成目标
        public PopUpsController RewardPUPrefab { get { return rewardPrefabPU; } } // 奖励弹窗
        public int TargetCount { get { return targetCount; } } // 目标次数
        public int CurrentCount { get; private set; } // 当前次数
        public int CurrentStage { get; private set; } // 当前阶段
        public bool RewardReceived { get; private set; } // 是否已领奖
        #endregion properties

        #region events
        public Action <int, int> ChangeCurrentCountEvent; // 次数变化事件
        public Action<int> ChangeCurrentStageEvent; // 阶段变化事件
        public Action RewardReceivedEvent; // 领奖事件
        public Action ResetReceivedEvent; // 重置领奖事件
        #endregion events

        #region reward
        /// <summary>
        /// 加载领奖状态
        /// </summary>
        protected void LoadRewardReceived()
        {
            Debug.Log("load SaveRewardReceivedName: " + SaveRewardReceivedName);
            RewardReceived = (PlayerPrefs.GetInt(SaveRewardReceivedName, 0) == 1);
        }
        /// <summary>
        /// 设置已领奖
        /// </summary>
        protected void SetRewardReceived()
        {
            RewardReceived = true;
            PlayerPrefs.SetInt(SaveRewardReceivedName, 1);
        }
        /// <summary>
        /// 重置领奖状态
        /// </summary>
        protected void ResetRewardReceived()
        {
            Debug.Log("Reset reward received");
            RewardReceived = false;
            PlayerPrefs.SetInt(SaveRewardReceivedName, 0);
            ResetReceivedEvent?.Invoke();
        }

        public void OnGetRewardEvent()
        {
            if (!TargetAchieved) return;
            SetRewardReceived();
            if(rewardPrefabPU) GuiController.Instance.ShowPopUp(rewardPrefabPU);
            RewardReceivedEvent?.Invoke();
            if (resetAfterGetReward)
            {
                ResetCurrentCount();
                ResetRewardReceived();
            }
            IncCurrentStage();
        }
        #endregion reward

        #region current achievement count
        protected void LoadCurrentCount()
        {
            Debug.Log("Load SaveCountName: " + SaveCountName);
            CurrentCount = PlayerPrefs.GetInt(SaveCountName, 0);
        }

        protected void ResetCurrentCount()
        {
            if (CurrentCount == 0) return;
            CurrentCount = 0;
            PlayerPrefs.SetInt(SaveCountName, CurrentCount);
            ChangeCurrentCountEvent?.Invoke(CurrentCount, targetCount);
            Debug.Log("Reset current count");
        }

        protected void IncCurrentCount()
        {
            CurrentCount++;
            CurrentCount = Mathf.Min(CurrentCount, TargetCount);
            ChangeCurrentCountEvent?.Invoke(CurrentCount, targetCount);
            if(dLog)  Debug.Log(GetUniqueName() + " target " + CurrentCount);
            PlayerPrefs.SetInt(SaveCountName, CurrentCount);
        }
        #endregion current achievement count

        #region current stage
        protected void LoadCurrentStage()
        {
            Debug.Log("Load Stage: " + SaveStageName);
            CurrentStage = PlayerPrefs.GetInt(SaveStageName, 0);
        }

        protected void ResetCurrentStage()
        {
            if (CurrentStage == 0) return;
            CurrentStage = 0;
            PlayerPrefs.SetInt(SaveStageName, CurrentStage);
            ChangeCurrentStageEvent?.Invoke(CurrentStage);
            Debug.Log("Reset current stage");
        }

        protected void IncCurrentStage()
        {
            CurrentStage++;
            ChangeCurrentStageEvent?.Invoke(CurrentStage);
            if (dLog) Debug.Log(GetUniqueName() + " stage " + CurrentStage);
            PlayerPrefs.SetInt(SaveStageName, CurrentStage);
        }
        #endregion current stage

        public AchievementsLine CreateGuiLine(RectTransform parent)
        {
            return (achievementsLinePrefab) ? achievementsLinePrefab.CreateInstance(parent, this) : null;
        }

        public virtual string GetUniqueName() { return "achievement"; }

        public virtual void Load()
        {

        }

        public virtual Sprite GetAchImage()
        {
            return null;
        }

        public virtual void ResetGame()
        {
            ResetCurrentCount();
            ResetRewardReceived();
            ResetCurrentStage();
        }

#if UNITY_EDITOR
        public void DrawInspector()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #region test
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginHorizontal("box");

                if (GUILayout.Button("Inc Current Count"))
                {
                    IncCurrentCount();
                }
                if (GUILayout.Button("Reset Current Count"))
                {
                    ResetCurrentCount();
                    ResetRewardReceived();
                }
                if (GUILayout.Button("Reset reward received"))
                {
                    ResetRewardReceived();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Goto play mode for test");
            }
            #endregion test
        }
#endif
    }
}