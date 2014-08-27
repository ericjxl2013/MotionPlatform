/// <summary>
/// <Filename>: MaxInfoClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.29
/// Version: 1.0
/// Company: Sunnytech
/// Function: 3DMAX表格运动信息处理类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class MaxMotion : IMotion
{
	//默认为Active
	public CurrentState State
	{
		get {return _state;}
		set {_state = value;}
	}
	private CurrentState _state = CurrentState.Active;
	//速率
	public float SpeedRate
	{
		get {return MotionPara.SpeedRate;}
		set {MotionPara.SpeedRate = value;}
	}
	//暂停状态
	public bool PauseControl
	{
		get {return MotionPara.PauseControl;}
		set {MotionPara.PauseControl = value;}
	}
	//时间参数
	//正常速率时间
	public float StandardTime
	{
		get { return _standardTime; }
		set { _standardTime = value; }
	}
	private float _standardTime = 0f;
	//运动开始时间
	public float StartTime
	{
		get {return _startTime;}
		set {_startTime = value;}
	}
	private float _startTime = 0f;
	//运动结束时间
	public float EndTime 
	{
		get {return _endTime;}
		set {_endTime = value;}
	} 
	private float _endTime = 0f;

	//动画宿主
	public GameObject AnimationParent = null;
	//动画名称
	public string AnimationName = "";

	//当前3dMax的进度,0为开始,1为结束
	//float CurrentProgress;
	public float RecordSpeed;

	//PC2动画:动画物体名,起始帧,结束帧
	public List<string> PC2_Animation_Name = new List<string>();
	public List<int> PC2_Animation_StartFrame = new List<int>();
	public List<int> PC2_Animation_EndFrame = new List<int>();
	float FPS = 30f;

	public void Init()
	{

		if(AnimationParent != null){
			AnimationParent.animation.Play(AnimationName);
			AnimationParent.animation[AnimationName].speed = RecordSpeed;//恢复播放速度
			//CurrentProgress = 0f;
		}
	}

	//暂停控制
	public void Pause(bool play_status)  //接口方法
	{
		if(play_status){//暂停
			RecordSpeed = AnimationParent.animation[AnimationName].speed;//记录当前播放速度
			AnimationParent.animation[AnimationName].speed = 0f;//当前播放速度设置为0
		}else{//播放
			AnimationParent.animation[AnimationName].speed = RecordSpeed;//恢复播放速度
		}
	}

	//设置3dMax动画的进度
	public void setCurrentProgress(bool isStart){
		//CurrentProgress = setProgress;
		//RecordSpeed = AnimationParent.animation[AnimationName].speed;//记录当前播放速度
		//AnimationParent.animation[AnimationName].speed = 0f;
		//AnimationParent.animation.Play(AnimationName);
		//AnimationParent.animation[AnimationName].time = AnimationParent.animation[AnimationName].length * CurrentProgress ;

		//3dmax动画最初状态
		if (isStart)
		{
			RecordSpeed = AnimationParent.animation[AnimationName].speed;//记录当前播放速度
			AnimationParent.animation[AnimationName].speed = 0f;
			AnimationParent.animation.Play(AnimationName);
			AnimationParent.animation[AnimationName].time = 0;

			//PC2动画
			for (int i = 0; i < PC2_Animation_Name.Count; i++)
			{
				GameObject obj = GameObject.Find(PC2_Animation_Name[i]);
				//动画未开始

				obj.GetComponent<MegaPointCache>().animated = true;
				obj.GetComponent<MegaPointCache>().speed = 0;
				obj.GetComponent<MegaPointCache>().time = 0;
			}
		}
		//3dmax动画结束状态
		else
		{
			RecordSpeed = AnimationParent.animation[AnimationName].speed;//记录当前播放速度
			AnimationParent.animation[AnimationName].speed = 0f;
			AnimationParent.animation.Play(AnimationName);
			AnimationParent.animation[AnimationName].time = AnimationParent.animation[AnimationName].length;

			//PC2动画
			for (int i = 0; i < PC2_Animation_Name.Count; i++)
			{
				GameObject obj = GameObject.Find(PC2_Animation_Name[i]);
				//动画已结束

				obj.GetComponent<MegaPointCache>().animated = true;
				obj.GetComponent<MegaPointCache>().speed = 0;
				obj.GetComponent<MegaPointCache>().time = obj.GetComponent<MegaPointCache>().maxtime - 0.01f;
			}
		}

	}

	//速率控制
	public void ChangeRate(float current_rate, float current_time)  //接口方法
	{
		//过程速率改变
		//根据速率改变终止时间和播放速度
		EndTime = current_time + (EndTime - current_time) * SpeedRate / current_rate;
		if(PauseControl){
			RecordSpeed = RecordSpeed * current_rate / SpeedRate; //记录修改后的播放速度
		}else{
			AnimationParent.animation[AnimationName].speed 
				= AnimationParent.animation[AnimationName].speed * current_rate / SpeedRate;
		}

	}

	//通用运动驱动函数
	public void Move(float current_time, float speed_rate, float delta_time)  //接口方法
	{
		//PC2动画
		for(int i = 0 ;i < PC2_Animation_Name.Count ; i++){
			GameObject obj = GameObject.Find(PC2_Animation_Name[i]);
			FPS = (PC2_Animation_EndFrame[i] -  PC2_Animation_StartFrame[i]) / obj.GetComponent<MegaPointCache>().maxtime;
			//动画未开始
			if(AnimationParent.animation[AnimationName].time < PC2_Animation_StartFrame[i] / FPS){
				obj.GetComponent<MegaPointCache>().animated = true;
				obj.GetComponent<MegaPointCache>().speed = 0;
				obj.GetComponent<MegaPointCache>().time = 0;
			}
			//动画已结束
			else if(AnimationParent.animation[AnimationName].time > PC2_Animation_EndFrame[i] / FPS){
				obj.GetComponent<MegaPointCache>().animated = true;
				obj.GetComponent<MegaPointCache>().speed = 0;
				obj.GetComponent<MegaPointCache>().time = obj.GetComponent<MegaPointCache>().maxtime - 0.01f;
			}
			//动画进行中
			else{


				obj.GetComponent<MegaPointCache>().animated = true;
				obj.GetComponent<MegaPointCache>().speed = 0;
				obj.GetComponent<MegaPointCache>().time = AnimationParent.animation[AnimationName].time - PC2_Animation_StartFrame[i] / FPS;

				Debug.Log("time:"+AnimationParent.animation[AnimationName].time+","+i+","+obj.GetComponent<MegaPointCache>().time+",obj:"+obj.name+","+PC2_Animation_Name.Count+","+FPS);
			}
		}

	}

	//时间控制
	public bool TimesUp(float current_time)
	{
		if(current_time > EndTime)
			return true;
		else
			return false;
	}

	//后处理，主要是位置修正
	public void PostProcess()
	{
		if(AnimationParent != null){
			AnimationParent.animation.Stop(AnimationName);
		}
		setCurrentProgress(false);
	}
}

//Max表格动画信息处理类
public class MaxInfoManager : BaseCompute
{
	private MaxMotion _maxMotion;

	public MaxInfoManager ()
	{
		_maxMotion = new MaxMotion();
	}

	//3DMAX表格信提取
	public MaxMotion MaxInfoGet(DataRow max_row, string id, ref bool is_right)
	{
		string objName = (string)max_row[2].ToString();
		_maxMotion.AnimationParent = GameObject.Find(objName);
		if(_maxMotion.AnimationParent == null){
			is_right = false;
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("3DMAX", "OBJNAME", id) + "，动画所在物体名称错误！");
			}
			return _maxMotion;
		}
		_maxMotion.AnimationName = (string)max_row[3].ToString();
		//时间确定
		try{
			_maxMotion.StandardTime = _maxMotion.AnimationParent.animation[_maxMotion.AnimationName].length;
			_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed = 1.0f;
		}catch{
			is_right = false;
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("3DMAX", "ANINAME", id) + "，动画名字错误！");
			}
			return _maxMotion;
		}
		string customTimeStr = (string)max_row[4].ToString();
		if(customTimeStr != ""){
			_maxMotion.StandardTime = FloatConversion(customTimeStr, ref is_right);
			if(!is_right || _maxMotion.StandardTime == 0){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "Time", id) + "，自定义动画播放时长设置错误！");
				}
				return _maxMotion;
			}
			_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed = 
				_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].length / _maxMotion.StandardTime;
		}

		//根据速率改变终止时间和播放速度
		_maxMotion.EndTime = _maxMotion.StandardTime / _maxMotion.SpeedRate;
		_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed 
			= _maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed *_maxMotion.SpeedRate;

		_maxMotion.RecordSpeed = _maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed;//记录当前播放速度
		_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].speed = 0f;//当前播放速度设置为0

		if((string)max_row[5].ToString() == ""){
			return _maxMotion;
		}

		//PC2动画
		string[] pc2_1 = max_row[5].ToString().Split('|');
		string[] pc2_2 = max_row[6].ToString().Split('|');
		string[] pc2_3 = max_row[7].ToString().Split('|');
		_maxMotion.PC2_Animation_Name.Clear();
		_maxMotion.PC2_Animation_StartFrame.Clear();
		_maxMotion.PC2_Animation_EndFrame.Clear();
		for(int i = 0; i < pc2_1.Length ; i++){
			//Debug.Log(i+","+pc2_1[i]+","+pc2_2[i]+","+pc2_3[i]);

			if(GameObject.Find(pc2_1[i]) == null){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2OBJNAME", id) + "，PC2动画所在物体名称错误！");
				}
				return _maxMotion;
			}
			int num = 0;
			if(!int.TryParse(pc2_2[i],out num)){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2STARTFRAME", id) + "，PC2动画所在起始帧填写错误！");
				}
				return _maxMotion;
			}
			else{
				if(int.Parse(pc2_2[i]) < 0){
					is_right = false;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2STARTFRAME", id) + "，PC2动画所在起始帧必须大于或等于0！");
					}
					return _maxMotion;
				}
			}

			if(!int.TryParse(pc2_3[i],out num)){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2EndFRAME", id) + "，PC2动画所在结束帧填写错误！");
				}
				return _maxMotion;
			}

			if(int.Parse(pc2_2[i]) >= int.Parse(pc2_3[i])){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2STARTFRAME", id) + "，PC2动画所在起始帧必须小于结束帧！");
				}
				return _maxMotion;
			}
			int pc2_frames = int.Parse(pc2_3[i]) - int.Parse(pc2_2[i]);
			float pc2_time = pc2_frames / _maxMotion.AnimationParent.animation[_maxMotion.AnimationName].clip.frameRate;
			GameObject obj = GameObject.Find(pc2_1[i]);
			if(Mathf.Abs(pc2_time - obj.GetComponent<MegaPointCache>().maxtime) > 0.01f){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2STARTFRAME", id) + "，PC2动画所在起始帧结束帧差值不能刚好播完PC2动画！");
				}
				return _maxMotion;
			}
			float pc2_endtime = int.Parse(pc2_3[i])/_maxMotion.AnimationParent.animation[_maxMotion.AnimationName].clip.frameRate;
			if(pc2_endtime > _maxMotion.AnimationParent.animation[_maxMotion.AnimationName].length){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("3DMAX", "PC2EndFRAME", id) + "，PC2动画所在起始帧结束帧不能在父物体的动画内播完PC2动画！");
				}
				return _maxMotion;
			}

			_maxMotion.PC2_Animation_Name.Add(pc2_1[i]);
			_maxMotion.PC2_Animation_StartFrame.Add(int.Parse(pc2_2[i]));
			_maxMotion.PC2_Animation_EndFrame.Add(int.Parse(pc2_3[i]));
		}

		return _maxMotion;
	}

}
