using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System;

//引出线运动类
public class DoLabelMotion : MonoBehaviour, IMotion
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

	//暂停状态
	public bool PauseControl
	{
		get {return MotionPara.PauseControl;}
		set {MotionPara.PauseControl = value;}
	}
	
	public static List<int> Label_layer = new List<int>();
	public static  List<string> Label_obj = new List<string>();
	public static  List<string> Label_name = new List<string>();
	public static List<Vector3> Label_pos = new List<Vector3>();

	public static List<int> Label_layer2 = new List<int>();
	public static  List<string> Label_obj2 = new List<string>();
	public static  List<string> Label_name2 = new List<string>();
	public static List<Vector3> Label_pos2 = new List<Vector3>();

	//显示方式： true--同时显示; false--顺序播放
	public bool showType = true;
	//顺序播放的次序控制
	public int labelCount;
	//Tips暂停控制中间时间值
	private float _pauseTimeRecorder = 0f;

	//结构初始化
	public void Init()
	{
		FuncPara.outlineOn = true;

		labelCount = 0;

		//同时显示
		if(showType){
			for(int i=0; i<Label_obj2.Count; i++){
				Label_layer.Add(Label_layer2[i]);
				Label_obj.Add(Label_obj2[i]);
				Label_name.Add(Label_name2[i]);
				Label_pos.Add(Label_pos2[i]);
			}
		}
		//顺序显示
		else{

		}

		Debug.Log("DoLabelMotion init");
	}



	//暂停控制
	public void Pause(bool play_status)
	{
		
	}
	
	//速率控制
	public void ChangeRate(float current_rate, float current_time)
	{
		EndTime = current_time + (EndTime - current_time) * SpeedRate / current_rate;
	}
	
	//运动
	public void Move(float current_time, float speed_rate, float delta_time)
	{

	}
	
//	//变速后的时间比率
//	private float TimeRate(float current_time)
//	{
//		currentTimeRate = startTimeRate + (current_time - postTime) / restTime;
//		return currentTimeRate;
//	}
	
	//时间管理
	public bool TimesUp(float current_time)
	{
		//只在教的时候显示
		if(FuncPara.currentMotion == MotionState.Teaching){
			if(showType){
				if(current_time > EndTime){
					return true;
				}
				else
					return false;
			}
			else{
				if (!MotionPara.isEditor){
					if(PauseControl){
						//暂停时不断增加，因为语音不会暂停
						_pauseTimeRecorder += Time.deltaTime;
					}
				}


				if((current_time + _pauseTimeRecorder) > EndTime && (labelCount == Label_obj2.Count)){
					return true;
				}
				else
					return false;
			}
		}
		else{
			return true;
		}

	}
	
	//后处理，主要是位置修正
	public void PostProcess()
	{
		FuncPara.outlineOn = false;
		Debug.Log("DoLabelMotion PostProcess");
	}
	
	//最后没运行的时间修正
	private void TimeCorrection(float delta_time, float speed_rate)
	{

	}
	

}

//Camera表格动画信息处理类
public class DoLabelInfoManager : BaseCompute
{
	private DoLabelMotion _cameraMotion;
	
	public DoLabelInfoManager ()
	{
		_cameraMotion = new DoLabelMotion();
	}
	
	
	//Camera表格信提取
	public DoLabelMotion DoLabelInfoGet(DataRow camera_row, string id, ref bool is_right)
	{

		DoLabelMotion.Label_layer.Clear();
		DoLabelMotion.Label_name.Clear();
		DoLabelMotion.Label_obj.Clear();
		DoLabelMotion.Label_pos.Clear();

		DoLabelMotion.Label_layer2.Clear();
		DoLabelMotion.Label_name2.Clear();
		DoLabelMotion.Label_obj2.Clear();
		DoLabelMotion.Label_pos2.Clear();

		//物体信息
		string obj = (string)camera_row[2].ToString();
//		ExcelOperator excelReader = new ExcelOperator();
//		DataTable sheetTable = excelReader.ExcelReader(MotionPara.taskRootPath + MotionPara.taskName + "/XIAN.xls", obj);

		JsonOperator jo2 = new JsonOperator();
		DataTable sheetTable = jo2.JsonReader(MotionPara.taskRootPath + MotionPara.taskName + "/XIAN.json", obj);

		for(int j=0; j<sheetTable.Rows.Count; j++){
			DataRow dr = sheetTable.Rows[j];

			DoLabelMotion.Label_layer2.Add(int.Parse(dr[0].ToString()));
			DoLabelMotion.Label_name2.Add(dr[1].ToString());
			DoLabelMotion.Label_obj2.Add(dr[2].ToString());

			DoLabelMotion.Label_pos2.Add(new Vector3(0,0,0));
		}

		//获得引出线详细信息
		GameObject.Find("Main Camera").GetComponent<doMoldLable>().showLabelType(camera_row[2].ToString(), true);

		//时间
		float time_tmp = FloatConversion(camera_row[3].ToString(), ref is_right);
		if (!is_right)
		{
			if (MotionPara.isEditor)
			{
				Debug.LogError(ErrorLocation.Locate("XIAN", "TIME", id) + "，引出线的时间填写错误！");
			}
			return _cameraMotion;
		}
		_cameraMotion.StandardTime = time_tmp;
		_cameraMotion.EndTime = _cameraMotion.StandardTime / _cameraMotion.SpeedRate;


		//显示方式
		string showType_tmp = camera_row[4].ToString();
		if(showType_tmp.ToUpper() == "TRUE"){
			_cameraMotion.showType = true;
		}
		else if(showType_tmp.ToUpper() == "FALSE"){
			_cameraMotion.showType = false;
		}
		else{
			Debug.LogError(ErrorLocation.Locate("XIAN", "SHOWTYPE", id) + "，引出线的显示方式填写错误！");
		}

		return _cameraMotion;
	}

}