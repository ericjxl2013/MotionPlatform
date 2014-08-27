/// <summary>
/// <Filename>: MotionClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.27
/// Version: 1.0
/// Company: Sunnytech
/// Function: 运动模块相关的Class
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

//Motion运动模块接口，规定好通用的方法
public enum CurrentState {Old, Active, Future}  //该类当前的状态，为实现复杂的组合运动，Old表示已经运行完，
												//Active表示当前处于激活状态，Future表示还未运行

public interface IMotion
{
	//属性******
	//当前类状态
	CurrentState State {get;set;}
	//标准时间
	float StandardTime { get; set; }
	//起始时间
	float StartTime {get;set;}
	//终止时间
	float EndTime {get;set;}


	//方法******
	//结构初始化
	void Init();
	//暂停控制
	void Pause(bool play_status);
	//速率控制
	void ChangeRate(float current_rate, float current_time);
	//运动
	void Move(float current_time, float speed_rate, float delta_time);
	//时间管理
	bool TimesUp(float current_time);
	//后处理，主要是位置修正
	void PostProcess();
}

//Motion运动控制类
public class MotionClass : BaseCompute
{
	//IMotion列表，为了用接口IMotion定义的方法
	public IList<IMotion> _motionList = new List<IMotion>();
	public IList<IList<IMotion>> _complexMotionList = new List<IList<IMotion>>();

	//返回当前运动类个数
	public int Count
	{
		get {return _motionList.Count + _complexMotionList.Count;}
	}

	//如果有Tips则返回TipsMotion实例，用于有Tips情况的初始化
	public TipsMotion HasTipsMotion
	{
		get {return _hasTipsMotion;}
	}
	private TipsMotion _hasTipsMotion = null;


	//初始化
	public void Init()
	{
		for(int i = 0; i < _motionList.Count; i++){
			_motionList[i].Init();
		}
		for(int i = 0; i < _complexMotionList.Count; i++){
			for(int j = 0; j < _complexMotionList[i].Count; j++){
				if(_complexMotionList[i][j].State == CurrentState.Active){
					_complexMotionList[i][j].Init();
					break;
				}
			}
		}
	}

	//暂停控制
	public void Pause(bool play_status)
	{
		for(int i = 0; i < _motionList.Count; i++){
			_motionList[i].Pause(play_status);
		}
		for(int i = 0; i < _complexMotionList.Count; i++){
			for(int j = 0; j < _complexMotionList[i].Count; j++){
				if(_complexMotionList[i][j].State == CurrentState.Active){
					_complexMotionList[i][j].Pause(play_status);
					break;
				}
			}
		}
		MotionPara.PauseControl = play_status;
	}

	//改变播放速率
	public void ChangeRate(float current_rate, float current_time)
	{
		for(int i = 0; i < _motionList.Count; i++){
			_motionList[i].ChangeRate(current_rate, current_time);
		}
		for(int i = 0; i < _complexMotionList.Count; i++){
			for(int j = 0; j < _complexMotionList[i].Count; j++){
				if(_complexMotionList[i][j].State != CurrentState.Old){
					_complexMotionList[i][j].ChangeRate(current_rate, current_time);
				}
			}
		}
		MotionPara.SpeedRate = current_rate;
	}

	//每个运动的时间控制
	public bool TimesUp(float current_time)
	{
		//简单动画部分
		bool simpleResult = true;
		for(int i = 0; i < _motionList.Count; i++){
			bool singleResult = _motionList[i].TimesUp(current_time);
			if(i == 0)
				simpleResult = singleResult;
			else
				simpleResult = simpleResult && singleResult;
			if(singleResult){
				_motionList[i].PostProcess();
				_motionList.RemoveAt(i);
				i--;
			}
		}

		//组合动画部分
		bool complexResult = true;
		for(int i = 0; i < _complexMotionList.Count; i++){
			bool singleResult = true;
			for(int j = 0; j < _complexMotionList[i].Count; j++){
				if(_complexMotionList[i][j].State == CurrentState.Active){
					singleResult = _complexMotionList[i][j].TimesUp(current_time);
					if(singleResult){
						_complexMotionList[i][j].State = CurrentState.Old;
						_complexMotionList[i][j].PostProcess();
						if(j != _complexMotionList[i].Count - 1){
							_complexMotionList[i][j + 1].State = CurrentState.Active;
							_complexMotionList[i][j + 1].StartTime = current_time;
							_complexMotionList[i][j + 1].EndTime += current_time;
							_complexMotionList[i][j + 1].Init();
							singleResult = false;
						}
						_complexMotionList[i].RemoveAt(j);
						break;
					}
				}
			}
			if(i == 0)
				complexResult = singleResult;
			else
				complexResult = complexResult && singleResult;
		}


		return simpleResult && complexResult;
	}

	//运动
	public void Move(float current_time, float speed_rate, float delta_time) 
	{
		for(int i = 0; i < _motionList.Count; i++){
			_motionList[i].Move(current_time, speed_rate, delta_time);
		}
		for(int i = 0; i < _complexMotionList.Count; i++){
			for(int j = 0; j < _complexMotionList[i].Count; j++){
				if(_complexMotionList[i][j].State == CurrentState.Active){
					_complexMotionList[i][j].Move(current_time, speed_rate, delta_time);
					break;
				}
			}
		}
	}

	//Excel表格信息提取总入口
	public bool ExcelAdd(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//Motion Type
		int motionInt = IntConversion(excel_row[3].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "MOTIONTYPE", id) + "，运动方式填写错误！");
			}
			return false;
		}
		ExcelInfoManager excelMana = new ExcelInfoManager();
		if(motionInt == 1){  //直线运动
			BasicMotion moveExcel = excelMana.SimpleMotion(excel_row, id, group_table, ref isRight, motionInt);
			if(!isRight){
				return false;
			}
			_motionList.Add(moveExcel);
		}else if(motionInt == 2){  //旋转运动
			BasicMotion rotateExcel = excelMana.SimpleMotion(excel_row, id, group_table, ref isRight, motionInt);
			if(!isRight){
				return false;
			}
			_motionList.Add(rotateExcel);
		}else if(motionInt == 3){  //直线+旋转
			BasicMotion move_rotateExcel = excelMana.SimpleMotion(excel_row, id, group_table, ref isRight, motionInt);
			if(!isRight){
				return false;
			}
			_motionList.Add(move_rotateExcel);
		}else if(motionInt == 4){  //任意移动
			BasicMotion randomExcel = excelMana.SimpleMotion(excel_row, id, group_table, ref isRight, motionInt);
			if(!isRight){
				return false;
			}
			_motionList.Add(randomExcel);
		}else if(motionInt == 5){  //直接设定位置
			BasicMotion move_rotateExcel = excelMana.SimpleMotion(excel_row, id, group_table, ref isRight, motionInt);
			if(!isRight){
				return false;
			}
			_motionList.Add(move_rotateExcel);
		}else{  //报错
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "MOTIONTYPE", id) + "，运动方式填写错误，暂时未包含该种运动方式！");
			}
			return false;
		}
		simpleID.Add(simpleID.Count, id);
		return true;
	}

	//Camera表格信息提取
	public bool CameraAdd(DataRow camera_row, string id)
	{
		bool isRight = true;
		CameraInfoManager cameraMana = new CameraInfoManager();
		CameraMotion cameraExcel = cameraMana.CameraInfoGet(camera_row, id, ref isRight);
		if(!isRight){
			return false;
		}
		_motionList.Add(cameraExcel);
		simpleID.Add(simpleID.Count, id);
		return true;
	}

	//Tips表格信息提取
	public bool TipsAdd(DataRow tips_row, string id)
	{
		bool isRight = true;
		TipsInfoManager tipsMana = new TipsInfoManager();
		TipsMotion tipsExcel = tipsMana.TipsInfoGet(tips_row, id, ref isRight);
		if(!isRight){
			return false;
		}
		_motionList.Add(tipsExcel);
		_hasTipsMotion = tipsExcel;
		simpleID.Add(simpleID.Count, id);
		return true;
	}

	//3D Max表格信息提取
	public bool MaxAdd(DataRow max_row, string id)
	{
		bool isRight = true;
		MaxInfoManager maxMana = new MaxInfoManager();
		MaxMotion maxExcel = maxMana.MaxInfoGet(max_row, id, ref isRight);
		if(!isRight){
			return false;
		}
		_motionList.Add(maxExcel);
		simpleID.Add(simpleID.Count, id);
		return true;
	}

	//Program信息提取
	public bool ProgramAdd(DataRow program_row, string id, DataTable group_table)
	{
		ProgramInfoManager proMana = new ProgramInfoManager();
		bool resultFlag = proMana.ProgramInfoGet(program_row, id, group_table, _motionList, _complexMotionList);
		if (resultFlag && MotionPara.isEditor)
		{
			string programID = ((string)program_row[2].ToString()).ToUpper();
			if (programID == "TONGBANG")
			{
				//铜棒敲击
				complexID.Add(complexID.Count, id);
			}
			else if (programID == "BAIFANG")
			{
				//物体摆放
				complexID.Add(complexID.Count, id);
			}
			else if (programID == "NINGSONG")
			{
				//螺钉拧松拧紧
				complexID.Add(complexID.Count, id);
			}
			else if (programID == "NINGCHU")
			{
				//螺钉拧进拧出
				complexID.Add(complexID.Count, id);
			}
		}
		return resultFlag;
	}

	//List Clear
	public void Clear()
	{
		_motionList.Clear();
		_complexMotionList.Clear();
		_hasTipsMotion = null;
		simpleID.Clear();
		complexID.Clear();
	}

	//停止时的后处理
	public void PostProcess()
	{
		for (int i = 0; i < _motionList.Count; i++)
		{
			_motionList[i].PostProcess();
		}
		for (int i = 0; i < _complexMotionList.Count; i++)
		{
			for (int j = 0; j < _complexMotionList[i].Count; j++)
			{
				_complexMotionList[i][j].PostProcess();
			}
		}
	}

	private Dictionary<int, string> simpleID = new Dictionary<int, string>();
	private Dictionary<int, string> complexID = new Dictionary<int, string>();

	//获得当前Motion运动ID
	public string GetMotionID(string type_str, int index)
	{
		string idStr = "";
		if (type_str.ToUpper() == "SIMPLE")
		{
			idStr = simpleID[index];
		}
		else 
		{
			idStr = complexID[index];
		}
		return idStr;
	}
}




