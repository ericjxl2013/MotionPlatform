/// <summary>
/// <Filename>: CameraInfoClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.29
/// Version: 1.0
/// Company: Sunnytech
/// Function: Camera表格运动信息处理类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public enum CameraMotionType {Line, Circular}

//摄像机运动类
public class CameraMotion : IMotion
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

	//静态类属性
	//当前活动相机
	public static Camera CurrentCamera = null;
	//之前的相机
	public static Camera FormerCamera = null;
	//当前字符串
	public static string CurrentStr = "";
	//当前类型,True为透视，False为正交
	public static bool CurrentType = false;
	//历史摄像机字符串，从Excel表格中获取
	public static List<string> CameraString = new List<string>();
	//历史摄像机，从Excel表格中获取
	public static List<Camera> CameraList = new List<Camera>();
	//历史摄像机类型，从Excel表格获取摄像机后自动判断,True为透视，False为正交
	public static List<bool> CameraType = new List<bool>();
	//是否要切换摄像机
	public static bool NeedChange = false;
	//观察物体
	public static Transform TargetTrans = null;

	//运动属性
	public CameraMotionType CurrentMotion = CameraMotionType.Line;

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

	//开始位置
	public Vector3 StartPos = new Vector3(0f, 0f, 0f);
	//结束位置
	public Vector3 EndPos = new Vector3(0f, 0f, 0f);
	//速度矢量
	public Vector3 SpeedVector = new Vector3(0f, 0f, 0f);
	public float RotateSpeed = 1.0f;
	//旋转中心
	public Vector3 RotateCenter = new Vector3(0f, 0f, 0f);
	public Vector3 RotateAxis = new Vector3(0f, 0f, 0f);

	//视域缩放参数
	public bool HasSizeChange = false;
	public float StartSize = 0f;
	public float EndSize = 0f;
	public float SizeSpeed = 0f;

	//结构初始化
	public void Init()
	{
		if(NeedChange){
			if(FormerCamera != null){
				FormerCamera.enabled = false;
			}
			CurrentCamera.enabled = true;
		}
	}

	//暂停控制
	public void Pause(bool play_status)
	{

	}

	//速率控制
	public void ChangeRate(float current_rate, float current_time)
	{
		if(State != CurrentState.Active){
			StartTime = 0;
			EndTime = SpeedRate * EndTime / current_rate;
		}else{
			//激活状态
			EndTime = current_time + (EndTime - current_time) * SpeedRate / current_rate;
			if(CurrentMotion == CameraMotionType.Line){
				StartPos = CurrentCamera.transform.position;
				CurrentCamera.transform.LookAt(TargetTrans);
			}
			if(HasSizeChange){
				StartSize = GetCameraPortValue();
			}
			StartTime = current_time;
		}
	}

	//运动
	public void Move(float current_time, float speed_rate, float delta_time)
	{
		if(CurrentMotion == CameraMotionType.Line){
			CurrentCamera.transform.position = StartPos + SpeedVector * (current_time - StartTime) * speed_rate;
		}else{
			CurrentCamera.transform.RotateAround(RotateCenter, RotateAxis, RotateSpeed * delta_time * speed_rate);
		}
		if(HasSizeChange)
			SetCameraPortValue(StartSize + SizeSpeed * (current_time - StartTime) * speed_rate);
		CurrentCamera.transform.LookAt(TargetTrans);
	}

	//时间管理
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
		if(CurrentMotion == CameraMotionType.Line){
			CurrentCamera.transform.position = EndPos;
			CurrentCamera.transform.LookAt(TargetTrans);
		}
		if(HasSizeChange)
			SetCameraPortValue(EndSize);
	}

	//摄像机视域信息获取
	public float GetCameraPortValue()
	{
		if(CurrentType)
			return CurrentCamera.fieldOfView;
		else
			return CurrentCamera.orthographicSize;
	}

	//摄像机视域信息设置
	private void SetCameraPortValue(float value)
	{
		if(CurrentType)
			CurrentCamera.fieldOfView = value;
		else
			CurrentCamera.orthographicSize = value;
	}
}

//Camera表格动画信息处理类
public class CameraInfoManager : BaseCompute
{
	private CameraMotion _cameraMotion;

	public CameraInfoManager ()
	{
		_cameraMotion = new CameraMotion();
	}


	//Camera表格信提取
	public CameraMotion CameraInfoGet(DataRow camera_row, string id, ref bool is_right)
	{
		//摄像机名称检查
		string cameraName = (string)camera_row[2].ToString();
		is_right = Check(cameraName);
		if(!is_right){
			//摄像机名称出错
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("CAMERA", "NAME", id) + "，摄像机名称错误！");
			}
			return _cameraMotion;
		}
		//观察中心物体处理
		try{
			CameraMotion.TargetTrans = GameObject.Find((string)camera_row[3].ToString()).transform;
		}catch{
			is_right = false;
			//摄像机名称出错
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("CAMERA", "OBSERVER", id) + "，观察物体填写错误！");
			}
			return _cameraMotion;
		}
		//圆弧运动和直线运动确定，圆弧优先级高于直线，根据【旋转参考物体:9】是否有填来确定
		bool isCircularMotion = false;
		string rotateRefStr = (string)camera_row[9].ToString();
		if(rotateRefStr == ""){  //直线运动
			isCircularMotion = false;
			_cameraMotion.CurrentMotion = CameraMotionType.Line;
		}else{  //圆弧运动
			isCircularMotion = true;
			_cameraMotion.CurrentMotion = CameraMotionType.Circular;
			try{
				_cameraMotion.RotateCenter = GameObject.Find(rotateRefStr).transform.position;
			}catch{
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "ROTATEREF", id) + "，旋转参考物体填写错误！");
				}
				return _cameraMotion;
			}
		}
		//起始位置确定
		bool isRelativePos = false;  //用于目标位置确定
		string relativeRefStr = (string)camera_row[4].ToString();
		string startPosStr = (string)camera_row[5].ToString();
		GameObject empty = new GameObject();
		empty.name = "camera_motion_empty-" + id;
		Transform relativeTrans = null;
		if(startPosStr == ""){
			if(isCircularMotion){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "SPOSITION", id) + "，摄像机圆弧运动一定要设定起始位置！");
				}
				return _cameraMotion;
			}else{
				_cameraMotion.StartPos = CameraMotion.CurrentCamera.transform.position;
			}
		}else{
			//默认为世界坐标位置
			_cameraMotion.StartPos = Vector3Conversion(startPosStr, ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "SPOSITION", id) + "，摄像机初始位置错误！");
				}
				return _cameraMotion;
			}
			if(relativeRefStr != ""){  //相对坐标位置
				isRelativePos = true;
				try{
					relativeTrans = GameObject.Find(relativeRefStr).transform;
				}catch{
					is_right = false;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CAMERA", "RELATIVE", id) + "，相对位置参考物体填写错误！");
					}
					return _cameraMotion;
				}
				empty.transform.parent = relativeTrans;
				empty.transform.localPosition = _cameraMotion.StartPos;
				_cameraMotion.StartPos = empty.transform.position;
				empty.transform.parent = null;
			}
		}
		//视域缩放参数确定
		string endSizeStr = (string)camera_row[8].ToString();
		if(endSizeStr == ""){
			_cameraMotion.HasSizeChange = false;
		}else{
			_cameraMotion.HasSizeChange = true;
			_cameraMotion.EndSize = FloatConversion(endSizeStr, ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "APORT", id) + "，摄像机视域变化终值错误！");
				}
				return _cameraMotion;
			}
			string startSizeStr = (string)camera_row[7].ToString();
			if(startSizeStr == ""){
				_cameraMotion.StartSize = _cameraMotion.GetCameraPortValue();
			}else{
				_cameraMotion.StartSize = FloatConversion(startSizeStr, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CAMERA", "SPORT", id) + "，摄像机视域变化初值错误！");
					}
					return _cameraMotion;
				}
			}	
		}
		//直线和旋转分别处理
		if(isCircularMotion){  //圆弧过程
			//旋转速度获取
			_cameraMotion.RotateSpeed = MotionPara.cameraAngleSpeed;
			string customRotStr = (string)camera_row[13].ToString();
			if(customRotStr != ""){
				_cameraMotion.RotateSpeed = FloatConversion(customRotStr, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CAMERA", "ROTATESPEED", id) + "，自定义角速度错误！");
					}
					return _cameraMotion;
				}
			}
			//旋转角度
			float rotateDegree = FloatConversion((string)camera_row[11].ToString(), ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "DEGREE", id) + "，旋转角度错误！");
				}
				return _cameraMotion;
			}
			//旋转轴
			_cameraMotion.RotateAxis = Vector3Conversion((string)camera_row[10].ToString(), ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "AXIS", id) + "，旋转轴错误！");
				}
				return _cameraMotion;
			}
			//时间
			_cameraMotion.StandardTime = rotateDegree / _cameraMotion.RotateSpeed;
		}else{  //直线过程
			//旋转速度获取
			float moveSpeed = MotionPara.cameraAngleSpeed;
			string customMoveStr = (string)camera_row[12].ToString();
			if(customMoveStr != ""){
				moveSpeed = FloatConversion(customMoveStr, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CAMERA", "MOVESPEED", id) + "，自定义直线速度错误！");
					}
					return _cameraMotion;
				}
			}
			//默认为世界坐标位置
			_cameraMotion.EndPos = Vector3Conversion((string)camera_row[6].ToString(), ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CAMERA", "APOSITION", id) + "，摄像机目标位置错误，直线时必须填！");
				}
				return _cameraMotion;
			}
			//是相对位置
			if(isRelativePos){
				empty.transform.parent = relativeTrans;
				empty.transform.localPosition = _cameraMotion.EndPos;
				_cameraMotion.EndPos = empty.transform.position;
				empty.transform.parent = null;
			}
			//运动向量
			Vector3 motionVec = _cameraMotion.EndPos - _cameraMotion.StartPos;
			//时间
			_cameraMotion.StandardTime = motionVec.magnitude / moveSpeed;
			//线速度
			_cameraMotion.SpeedVector = motionVec / _cameraMotion.StandardTime;
		}
		if(_cameraMotion.StandardTime == 0){
			is_right = false;
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("CAMERA", "ID", id) + "，计算出的移动时间为0，请检查信息填写是否正确！");
			}
			return _cameraMotion;
		}
		if(_cameraMotion.HasSizeChange){
			_cameraMotion.SizeSpeed = (_cameraMotion.EndSize - _cameraMotion.StartSize) / _cameraMotion.StandardTime;
		}
		_cameraMotion.EndTime = _cameraMotion.StandardTime / _cameraMotion.SpeedRate;
		GameObject.DestroyImmediate(empty);
		return _cameraMotion;
	}

	//检测是否需要将当前摄像机加入列表
	private bool Check(string camera_string)
	{
		if(camera_string == CameraMotion.CurrentStr){
			CameraMotion.NeedChange = false;
			return true;
		}
		int strIndex = CameraMotion.CameraString.IndexOf(camera_string);
		if(strIndex == -1){  //不存在
			Camera tempCamera;
			try{
				tempCamera = GameObject.Find(camera_string).camera;
			}catch{
				return false;
			}
			CameraMotion.CameraString.Add(camera_string);
			CameraMotion.CameraList.Add(tempCamera);
			if(tempCamera.orthographic){
				CameraMotion.CameraType.Add(false);  //正交
				CameraMotion.CurrentType = false;
			}else{
				CameraMotion.CameraType.Add(true);  //透视
				CameraMotion.CurrentType = true;
			}
			CameraMotion.FormerCamera = CameraMotion.CurrentCamera;
			CameraMotion.CurrentCamera = tempCamera;
			CameraMotion.CurrentStr = camera_string;
		}else{  //存在
			CameraMotion.FormerCamera = CameraMotion.CurrentCamera;
			CameraMotion.CurrentCamera = CameraMotion.CameraList[strIndex];
			CameraMotion.CurrentStr = CameraMotion.CameraString[strIndex];
			CameraMotion.CurrentType = CameraMotion.CameraType[strIndex];
		}
		CameraMotion.NeedChange = true;
		return true;
	}

}
