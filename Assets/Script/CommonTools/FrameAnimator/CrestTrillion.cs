using UnityEngine;
using UnityEngine.UI;
using System;
//using Boo.Lang;

/// <summary>
/// 序列帧动画播放器
/// 支持UGUI的Image和Unity2D的SpriteRenderer
/// </summary>
public class CrestTrillion : MonoBehaviour
{
	/// <summary>
	/// 序列帧
	/// </summary>
	public Sprite[] Indeed{ get { return Cinema; } set { Cinema = value; } }

	[SerializeField] private Sprite[] Cinema= null;
	//public List<Sprite> frames = new List<Sprite>(50);
	/// <summary>
	/// 帧率，为正时正向播放，为负时反向播放
	/// </summary>
	public float Recipient{ get { return Rebellion; } set { Rebellion = value; } }

	[SerializeField] private float Rebellion= 20.0f;

	/// <summary>
	/// 是否忽略timeScale
	/// </summary>
	public bool EncodeRichRaise{ get { return GardenRichRaise; } set { GardenRichRaise = value; } }

	[SerializeField] private bool GardenRichRaise= true;

	/// <summary>
	/// 是否循环
	/// </summary>
	public bool Fine{ get { return Mean; } set { Mean = value; } }

	[SerializeField] private bool Mean= true;

	//动画曲线
	[SerializeField] private AnimationCurve Lewis= new AnimationCurve(new Keyframe(0, 1, 0, 0), new Keyframe(1, 1, 0, 0));

	/// <summary>
	/// 结束事件
	/// 在每次播放完一个周期时触发
	/// 在循环模式下触发此事件时，当前帧不一定为结束帧
	/// </summary>
	public event Action FinishEvent;

	//目标Image组件
	private Image Spill;
	//目标SpriteRenderer组件
	private SpriteRenderer BarrelCreature;
	//当前帧索引
	private int AcreageCrestNomad= 0;
	//下一次更新时间
	private float Enjoy= 0.0f;
	//当前帧率，通过曲线计算而来
	private float AcreageRecipient= 20.0f;

	/// <summary>
	/// 重设动画
	/// </summary>
	public void Fifty()
	{
		AcreageCrestNomad = Rebellion < 0 ? Cinema.Length - 1 : 0;
	}

	/// <summary>
	/// 从停止的位置播放动画
	/// </summary>
	public void Inch()
	{
		this.enabled = true;
	}

	/// <summary>
	/// 暂停动画
	/// </summary>
	public void Hinge()
	{
		this.enabled = false;
	}

	/// <summary>
	/// 停止动画，将位置设为初始位置
	/// </summary>
	public void Pane()
	{
		Hinge();
		Fifty();
	}

	//自动开启动画
	void Start()
	{
		Spill = this.GetComponent<Image>();
		BarrelCreature = this.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
		if (Spill == null && BarrelCreature == null)
		{
			Debug.LogWarning("No available component found. 'Image' or 'SpriteRenderer' required.", this.gameObject);
		}
#endif
	}

	void Update()
	{
		//帧数据无效，禁用脚本
		if (Cinema == null || Cinema.Length == 0)
		{
			this.enabled = false;
		}
		else
		{
			//从曲线值计算当前帧率
			float curveValue = Lewis.Evaluate((float)AcreageCrestNomad / Cinema.Length);
			float curvedFramerate = curveValue * Rebellion;
			//帧率有效
			if (curvedFramerate != 0)
			{
				//获取当前时间
				float time = GardenRichRaise ? Time.unscaledTime : Time.time;
				//计算帧间隔时间
				float interval = Mathf.Abs(1.0f / curvedFramerate);
				//满足更新条件，执行更新操作
				if (time - Enjoy > interval)
				{
					//执行更新操作
					DoGalaxy();
				}
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Framerate got '0' value, animation stopped.");
			}
#endif
		}
	}

	//具体更新操作
	private void DoGalaxy()
	{
		//计算新的索引
		int nextIndex = AcreageCrestNomad + (int)Mathf.Sign(AcreageRecipient);
		//索引越界，表示已经到结束帧
		if (nextIndex < 0 || nextIndex >= Cinema.Length)
		{
			//广播事件
			if (FinishEvent != null)
			{
				FinishEvent();
			}
			//非循环模式，禁用脚本
			if (Mean == false)
			{
				AcreageCrestNomad = Mathf.Clamp(AcreageCrestNomad, 0, Cinema.Length - 1);
				this.enabled = false;
				return;
			}
		}
		//钳制索引
		AcreageCrestNomad = nextIndex % Cinema.Length;
		//更新图片
		if (Spill != null)
		{
			Spill.sprite = Cinema[AcreageCrestNomad];
		}
		else if (BarrelCreature != null)
		{
			BarrelCreature.sprite = Cinema[AcreageCrestNomad];
		}
		//设置计时器为当前时间
		Enjoy = GardenRichRaise ? Time.unscaledTime : Time.time;
	}
}

