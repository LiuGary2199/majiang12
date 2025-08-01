using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	/// <summary>
	/// 模式切换按钮，支持编辑/游戏模式切换及关卡切换
	/// </summary>
	public class ModeButton : MonoBehaviour
	{
        [SerializeField]
        private Button modeButton; // 模式切换按钮
        [SerializeField]
        private Text modeText; // 模式文本
        #region temp vars

        #endregion temp vars
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } } // 配置集

        #region regular
        /// <summary>
        /// 初始化模式按钮，绑定切换事件
        /// </summary>
        private void Start()
		{
#if UNITY_EDITOR
            if (modeButton)
            {
                modeButton.gameObject.SetActive(true);
                if(modeText)  modeText.text = (GameBoard.GMode == GameMode.Edit) ? "To Play Mode" : "To Edit Mode";
                modeButton.onClick.AddListener(() =>
                {
                    if (GameBoard.GMode == GameMode.Edit)
                    {
                        GameBoard.GMode = GameMode.Play;
                        if (modeText) modeText.text = "To Edit Mode";
                    }
                    else
                    {
                        SimpleTween.ForceCancelAll();
                        GameBoard.GMode = GameMode.Edit;
                        if (modeText) modeText.text = "To Play Mode";
                    }
                    SceneLoader.Instance.ReLoadCurrentScene(false);
                });
            }
#else
           if (modeButton) modeButton.gameObject.SetActive(false); 
#endif
        // modeButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// 切换到下一关
        /// </summary>
        public void NextLevel()
        {
            if (!GCSet) return;
            int levelCount = GCSet.LevelCount;
            if (GameLevelHolder.CurrentLevel < levelCount - 1)
            {
                GameLevelHolder.CurrentLevel++;
                SceneLoader.Instance.ReLoadCurrentScene(false);
            }
        }

        /// <summary>
        /// 切换到上一关
        /// </summary>
        public void PrevLevel()
        {
            if (!GCSet) return;
            if (GameLevelHolder.CurrentLevel > 0)
            {
                GameLevelHolder.CurrentLevel--;
                SceneLoader.Instance.ReLoadCurrentScene(false);
            }
        }
        #endregion regular
    }
}
