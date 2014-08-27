/// <summary>
/// <Filename>: TipsInfoClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.30
/// Version: 1.0
/// Company: Sunnytech
/// Function: Tips表格运动信息处理类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class TipsMotion : IMotion
{
	//默认为Active
	public CurrentState State
	{
		get {return _state;}
		set {_state = value;}
	}
	private CurrentState _state = CurrentState.Active;
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

	//Tips信息
	public string TipsString = "";
	//Tips播放速度
	public string TipsSpeed = "1";
	//位置是否通过字符串控制
	public bool IsString = true;
	//位置方位信息字符串
	public string PosString = "";
	//Tips具体位置
	public Vector2 PosVec2 = new Vector2(0f, 0f);
	//是否为标题
	public bool IsTitle = false;
	//是否可移动
	public bool IsMoveable = false;
	//是否是同步运动的，暂时取消，同步运动放到Motion选项中
	//public bool IsSynchronized = true;
	//Tips暂停控制中间时间值
	private float _pauseTimeRecorder = 0f;


	//结构初始化
	public void Init()
	{
		
	}

	//暂停控制
	public void Pause(bool play_status)
	{
		
	}

	//速率控制
	public void ChangeRate(float current_rate, float current_time)
	{

	}

	//运动
	public void Move(float current_time, float speed_rate, float delta_time)
	{
		
	}

	//时间管理
	public bool TimesUp(float current_time)
	{
		if(PauseControl){
			//暂停时不断增加，因为语音不会暂停
			_pauseTimeRecorder += Time.deltaTime;
		}
		if(current_time + _pauseTimeRecorder > EndTime)
			return true;
		else
			return false;
	}

	//后处理，主要是位置修正
	public void PostProcess()
	{
		
	}

	//根据当前的播放速率自动计算语音播放时长及播放速率
	public void TipsTimeControl()
	{
		//6等于1.6倍速左右, 0等于0.95倍速左右，-1---0.85, -6---0.5, -2---0.8,  7等于1.7倍速左右
		//减速播放
		if(SpeedRate < 0.99f){
			EndTime = StandardTime / 0.7f;
			TipsSpeed = "-2";
		//加速播放，大于1.6倍速以后，一直以1.6倍速播放，因为太快听不清楚了
		}else if(SpeedRate > 1.01f){
			if(SpeedRate < 1.61f){
				EndTime = StandardTime / SpeedRate;
				TipsSpeed = (Mathf.FloorToInt(SpeedRate * 10) - 9).ToString();
			}else{
				EndTime = StandardTime / 1.5f;
				TipsSpeed = "6";
			}
		//正常速度
		}else{
			EndTime = StandardTime;
			TipsSpeed = "1";
		}
	}

}

//Tips信息处理类
public class TipsInfoManager : BaseCompute
{
	private TipsMotion _tipsMotion;

	//标点符号占用语音的时间记录
	private Dictionary<char, float> punctutionDic = new Dictionary<char, float>();

	public TipsInfoManager ()
	{
		_tipsMotion = new TipsMotion();
		////标点符号占用语音的时间初始化
		punctutionDic.Add(',', 0.4f); punctutionDic.Add('，', 0.4f);
		punctutionDic.Add('.', 1.5f); punctutionDic.Add('。', 1.5f);
		punctutionDic.Add('!', 1.5f); punctutionDic.Add('！', 1.5f);
		punctutionDic.Add('?', 1.5f); punctutionDic.Add('？', 1.5f);
		punctutionDic.Add(':', 0.5f); punctutionDic.Add('：', 0.5f);
		punctutionDic.Add(';', 0.5f); punctutionDic.Add('；', 0.5f);
		punctutionDic.Add('、', 0.15f); 
	}


	//Tips表格信提取
	public TipsMotion TipsInfoGet(DataRow tips_row, string id, ref bool is_right)
	{
		_tipsMotion.TipsString = (string)tips_row[1].ToString();
		if(_tipsMotion.TipsString == ""){
			is_right = false;
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("TIPS", "CONTENT", id) + "，Tips文本信息为空！");
			}
			return _tipsMotion;
		}
		//处理文本播放时长
		_tipsMotion.StandardTime = GetPlayTime(_tipsMotion.TipsString);
		//TODO:语音播放时长要与播放速率匹配，调节起来与一般调速不一样，还需要再调节
		_tipsMotion.TipsTimeControl();
		//显示位置处理
		DisplayPos((string)tips_row[2].ToString(), id, ref is_right);
		//是否为标题判断
		_tipsMotion.IsTitle = BoolenJudge((string)tips_row[3].ToString(), id, "TITLE", ref is_right);
		//是否可移动判断
		_tipsMotion.IsMoveable = BoolenJudge((string)tips_row[4].ToString(), id, "MOVE", ref is_right);
		//是否同步运动判断，暂时取消，同步运动放到Motion选项中
		//_tipsMotion.IsSynchronized = BoolenJudge((string)tips_row[5].ToString(), id, "SYNCHRONIZE", ref is_right);

		return _tipsMotion;
	}

	//语音播放时间
	public float GetPlayTime (string tips_str)
	{
		float rTime = 0;
		//标点符号占用时间
		float punctuationTime = 0;
		//字符数量
		int charNum = 0;
		char[] tipsCharArray = tips_str.ToCharArray();
		for(int i = 0; i < tipsCharArray.Length; i++){
			if(punctutionDic.ContainsKey(tipsCharArray[i])){
				if(i != tipsCharArray.Length - 1){
					punctuationTime += punctutionDic[tipsCharArray[i]];
				}
			}else{
				charNum++;
			}
		}
		if(charNum <= 4)
			rTime = 3f + punctuationTime;
		else
			rTime = 3f + punctuationTime + (charNum - 4) * 0.2f;
		//Debug.LogError("字符数：" + charNum + "; 标点符号占用时间: " + punctuationTime + "; 字符占用时间：" + (rTime - punctuationTime).ToString());
		return rTime;
	}

	//Tips显示位置处理
	private void DisplayPos(string pos_str, string id, ref bool is_right)
	{
		if(pos_str == ""){
			_tipsMotion.PosString = "down_left";
			_tipsMotion.IsString = true;
		}else{
			if(pos_str.ToLower() == "down_left"){
				_tipsMotion.PosString = "down_left";
				_tipsMotion.IsString = true;
			}else if(pos_str.ToLower() == "down_right"){
				_tipsMotion.PosString = "down_right";
				_tipsMotion.IsString = true;
			}else if(pos_str.ToLower() == "top_left"){
				_tipsMotion.PosString = "top_left";
				_tipsMotion.IsString = true;
			}else if(pos_str.ToLower() == "top_right"){
				_tipsMotion.PosString = "top_right";
				_tipsMotion.IsString = true;
			}else if(pos_str.ToLower() == "center" || pos_str.ToLower() == "centre"){
				_tipsMotion.PosString = "center";
				_tipsMotion.IsString = true;
			}else{
				string[] strArray = pos_str.Split(',', '，');
				if(strArray.Length != 2){
					_tipsMotion.PosString = "down_left";
					_tipsMotion.IsString = true;
					Debug.LogWarning(ErrorLocation.Locate("TIPS", "POS", id) + ", 位置填写错误，默认为down_left");
					is_right = false;
				}else{
					try{
						_tipsMotion.PosVec2.x = float.Parse(strArray[0]);
						_tipsMotion.PosVec2.y = float.Parse(strArray[1]);
					}catch{
						_tipsMotion.PosString = "down_left";
						_tipsMotion.IsString = true;
						Debug.LogWarning(ErrorLocation.Locate("TIPS", "POS", id) + ", 位置填写错误，默认为down_left");
						is_right = false;
						return;
					}
					_tipsMotion.IsString = false;
				}
			}
		}
	}

	//标题和移动标志位判断
	private bool BoolenJudge(string bool_str, string id, string grid_type, ref bool is_right)
	{
		if(bool_str == ""){
			return false;
		}else{
			bool_str = bool_str.ToUpper();
			if(bool_str == "TRUE"){
				return true;
			}else if(bool_str == "FALSE"){
				return false;
			}else{
				is_right = false;
				if(grid_type.ToUpper() == "TITLE"){
					Debug.LogWarning(ErrorLocation.Locate("TIPS", "TITLE", id) + ", 标题填写错误，默认为非标题");
					return false;
				}else if(grid_type.ToUpper() == "MOVE"){
					Debug.LogWarning(ErrorLocation.Locate("TIPS", "MOVE", id) + ", 移动选项填写错误，默认为非移动");
					return false;
				}else{
					Debug.LogWarning(ErrorLocation.Locate("TIPS", "SYNCHRONIZE", id) + ", 同步运动选项填写错误，默认为同步");
					return true;
				}
			}
		}
	}

}
