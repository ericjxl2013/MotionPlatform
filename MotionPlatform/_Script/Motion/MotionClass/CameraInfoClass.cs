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
using System.IO;

public enum CameraMotionType {Line, Circular, VisualPath}

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

	public bool needExcess = false;  //是否需要过渡
	public bool needSetPos = false;  //是否需要设置初始位置
    public CRSpline crSpline = new CRSpline();  //CR样条
    public Vector3[] diffVector = new Vector3[] { Vector3.zero, Vector3.zero };  //角度过度
    private int currentIndex = 0;  //当前二分法找到的参数序号
    private float viewTimeRate = 0f;  //二分法找到的参数对应的时间
    private float currentTimeRate = 0f;  //记录当前时间进程
    public float startTimeRate = 0f;  //记录变速时的当前时间进程
    private float restTime = 0f;  //剩余时间
    private float postTime = 0f;  //已经过去的时间
	private float lastFrameTime = 0f;  //记录上一帧时间

	//结构初始化
	public void Init()
	{
        restTime = EndTime;
		if(NeedChange){
			if(FormerCamera != null){
				FormerCamera.enabled = false;
			}
			CurrentCamera.enabled = true;
		}
		if (needSetPos)
		{
			CurrentCamera.transform.position = StartPos;
		}
        if (CurrentMotion == CameraMotionType.VisualPath)
        {
            if (MotionPara.isEditor)  //显示绿线初始化
            {
                CurrentCamera.gameObject.AddComponent<VisualUnit>();
                VisualUnit st_VUnit = CurrentCamera.gameObject.GetComponent<VisualUnit>();
                st_VUnit.nodes = crSpline.controlPoints;
                st_VUnit.isInitialized = true;
            }
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
            startTimeRate = currentTimeRate;
            postTime = current_time;
			EndTime = current_time + (EndTime - current_time) * SpeedRate / current_rate;
            restTime = (EndTime - postTime) / (1f - startTimeRate);
			if(CurrentMotion == CameraMotionType.Line){
				StartPos = CurrentCamera.transform.position;
                //CurrentCamera.transform.LookAt(TargetTrans);
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
		if (TimeRate(current_time) > 1.001f || current_time > EndTime)
		{
			return;
		}
		else
		{
			lastFrameTime = current_time;
		}
		if(CurrentMotion == CameraMotionType.Line){  //直线
			CurrentCamera.transform.position = StartPos + SpeedVector * (current_time - StartTime) * speed_rate;
            if (HasSizeChange)
                SetCameraPortValue(StartSize + SizeSpeed * (current_time - StartTime) * speed_rate);
            if (needExcess)
            {
                if (TimeRate(current_time) <= 0.1f)
                {
                    CurrentCamera.transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], TimeRate(current_time) * 10f);
                }
                else
                {
                    CurrentCamera.transform.LookAt(TargetTrans);
                }
            }
            else
            {
                CurrentCamera.transform.LookAt(TargetTrans);
            }
        }
        else if (CurrentMotion == CameraMotionType.Circular)  //圆弧
        {
            CurrentCamera.transform.RotateAround(RotateCenter, RotateAxis, RotateSpeed * delta_time * speed_rate);
            if (HasSizeChange)
                SetCameraPortValue(StartSize + SizeSpeed * (current_time - StartTime) * speed_rate);
            CurrentCamera.transform.LookAt(TargetTrans);
        }
        else  //可视化路径运动
        {
            CurrentCamera.transform.position = crSpline.Interp(TimeRate(current_time));
            if (TargetTrans != null)
            {
                if (needExcess)
                {
                    if (TimeRate(current_time) <= 0.1f)
                    {
                        CurrentCamera.transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], TimeRate(current_time) * 10f);
                    }
                    else
                    {
                        CurrentCamera.transform.LookAt(TargetTrans);
                    }
                }
                else
                {
                    CurrentCamera.transform.LookAt(TargetTrans);
                }
				SetCameraPortValue(crSpline.ViewLerp(TimeRate(current_time)));
            }
            else
            {
                CurrentCamera.transform.eulerAngles = crSpline.EulerAngleLerp(TimeRate(current_time), ref currentIndex, ref viewTimeRate);
                //二分法查找算一次
                SetCameraPortValue(Mathf.Lerp(crSpline.cameraViewList[currentIndex], crSpline.cameraViewList[currentIndex + 1], viewTimeRate));
            }
        }
	}

    //变速后的时间比率
    private float TimeRate(float current_time)
    {
        currentTimeRate = startTimeRate + (current_time - postTime) / restTime;
        return currentTimeRate;
    }

	//时间管理
	public bool TimesUp(float current_time)
	{
		if(current_time > EndTime){
			if (EndTime - lastFrameTime > 0)  //最后没运行的时间修正
			{
				TimeCorrection(EndTime - lastFrameTime, SpeedRate);
			}
			return true;
		}
		else
			return false;
	}

	//后处理，主要是位置修正
	public void PostProcess()
	{
        if (CurrentMotion == CameraMotionType.Line || CurrentMotion == CameraMotionType.Circular)
        {
            CurrentCamera.transform.position = EndPos;
            CurrentCamera.transform.LookAt(TargetTrans);
            if (HasSizeChange)
                SetCameraPortValue(EndSize);
        }
        else
        {
            if (MotionPara.isEditor)  //消除绿线初始化
            {
                Component.Destroy(CurrentCamera.gameObject.GetComponent<VisualUnit>());
            }
			CurrentCamera.transform.position = crSpline.controlPoints[crSpline.controlPoints.Count - 1];
			SetCameraPortValue(crSpline.cameraViewList[crSpline.cameraViewList.Count - 1]);
			if (TargetTrans != null)
			{
				CurrentCamera.transform.LookAt(TargetTrans);
			}
			else
			{
				CurrentCamera.transform.eulerAngles = crSpline.rotationList[crSpline.rotationList.Count - 1];
			}
        }
	}

	//最后没运行的时间修正
	private void TimeCorrection(float delta_time, float speed_rate)
	{
		if (CurrentMotion == CameraMotionType.Line)
		{  //直线
			CurrentCamera.transform.position = StartPos + SpeedVector * (EndTime - StartTime) * speed_rate;
			if (HasSizeChange)
				SetCameraPortValue(StartSize + SizeSpeed * (EndTime - StartTime) * speed_rate);
			if (needExcess)
			{
				if (TimeRate(EndTime) <= 0.1f)
				{
					CurrentCamera.transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], TimeRate(EndTime) * 10f);
				}
				else
				{
					CurrentCamera.transform.LookAt(TargetTrans);
				}
			}
			else
			{
				CurrentCamera.transform.LookAt(TargetTrans);
			}
		}
		else if (CurrentMotion == CameraMotionType.Circular)  //圆弧
		{
			CurrentCamera.transform.RotateAround(RotateCenter, RotateAxis, RotateSpeed * delta_time * speed_rate);
			if (HasSizeChange)
				SetCameraPortValue(StartSize + SizeSpeed * (EndTime - StartTime) * speed_rate);
			CurrentCamera.transform.LookAt(TargetTrans);
		}
		else  //可视化路径运动
		{
			CurrentCamera.transform.position = crSpline.Interp(TimeRate(EndTime));
			if (TargetTrans != null)
			{
				if (needExcess)
				{
					if (TimeRate(EndTime) <= 0.1f)
					{
						CurrentCamera.transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], TimeRate(EndTime) * 10f);
					}
					else
					{
						CurrentCamera.transform.LookAt(TargetTrans);
					}
				}
				else
				{
					CurrentCamera.transform.LookAt(TargetTrans);
				}
				SetCameraPortValue(crSpline.ViewLerp(TimeRate(EndTime)));
			}
			else
			{
				CurrentCamera.transform.eulerAngles = crSpline.EulerAngleLerp(TimeRate(EndTime), ref currentIndex, ref viewTimeRate);
				//二分法查找算一次
				SetCameraPortValue(Mathf.Lerp(crSpline.cameraViewList[currentIndex], crSpline.cameraViewList[currentIndex + 1], viewTimeRate));
			}
		}
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

    //直线上的点
    public Vector3 LineInterp(float rate)
    {
        return Vector3.Lerp(StartPos, EndPos, rate);
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
		//圆弧运动、直线和路径运动确定，圆弧优先级高于其他，根据【旋转参考物体:9】是否有填来确定
		string rotateRefStr = (string)camera_row[9].ToString();
        string pathName = "";
		if(rotateRefStr == ""){  
            pathName = (string)camera_row[14].ToString();
            if(pathName == "")
                _cameraMotion.CurrentMotion = CameraMotionType.Line;  //直线运动
            else
                _cameraMotion.CurrentMotion = CameraMotionType.VisualPath;  //可视化路径运动
		}else{  //圆弧运动
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
        //观察中心物体处理
        string targetTransStr = (string)camera_row[3].ToString();
        if (targetTransStr != "")
        {
            try
            {
                CameraMotion.TargetTrans = GameObject.Find(targetTransStr).transform;
            }
            catch
            {
                is_right = false;
                //摄像机名称出错
                if (MotionPara.isEditor)
                {
                    Debug.LogError(ErrorLocation.Locate("CAMERA", "OBSERVER", id) + "，观察物体填写错误！");
                }
                return _cameraMotion;
            }
        }
        else
        {
            if (_cameraMotion.CurrentMotion != CameraMotionType.VisualPath)
            {
                is_right = false;
                //摄像机名称出错
                if (MotionPara.isEditor)
                {
                    Debug.LogError(ErrorLocation.Locate("CAMERA", "OBSERVER", id) + "，直线和圆弧模式下必须有观察物体！");
                }
                return _cameraMotion; 
            }
        }
		//起始位置确定
		bool isRelativePos = false;  //用于目标位置确定
		string relativeRefStr = (string)camera_row[4].ToString();
		string startPosStr = (string)camera_row[5].ToString();
        string endPosStr = (string)camera_row[6].ToString();
		GameObject empty = new GameObject();
		empty.name = "camera_motion_empty-" + id;
		Transform relativeTrans = null;
		if(startPosStr == ""){
			_cameraMotion.StartPos = CameraMotion.CurrentCamera.transform.position;
			_cameraMotion.needSetPos = false;
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
			if ((_cameraMotion.StartPos - CameraMotion.CurrentCamera.transform.position).magnitude > 0.01f)
				_cameraMotion.needSetPos = true;
			else
				_cameraMotion.needSetPos = false;
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
		//直线、可视化路径、旋转运动分别处理
		if(_cameraMotion.CurrentMotion == CameraMotionType.Circular){  //圆弧过程
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
			_cameraMotion.RotateAxis = GameObject.Find(rotateRefStr).transform.TransformDirection(_cameraMotion.RotateAxis);
			//终止信息记录
			GameObject emptyCamera = new GameObject();
			emptyCamera.name = "camera_empty-" + id;
			emptyCamera.transform.position = _cameraMotion.StartPos;
			emptyCamera.transform.eulerAngles = CameraMotion.CurrentCamera.transform.eulerAngles;
			emptyCamera.transform.RotateAround(_cameraMotion.RotateCenter, _cameraMotion.RotateAxis, rotateDegree);
			_cameraMotion.EndPos = emptyCamera.transform.position;
			GameObject.DestroyImmediate(emptyCamera);
			//时间
			_cameraMotion.StandardTime = rotateDegree / _cameraMotion.RotateSpeed;
		}else{  
			//直线速度获取
			float moveSpeed = MotionPara.cameraLineSpeed;
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
            if (endPosStr == "" && _cameraMotion.CurrentMotion == CameraMotionType.VisualPath)
            {
                //Do nothing
            }
            else
            {
                //默认为世界坐标位置
                _cameraMotion.EndPos = Vector3Conversion(endPosStr, ref is_right);
                if (!is_right)
                {
                    if (MotionPara.isEditor)
                    {
                        Debug.LogError(ErrorLocation.Locate("CAMERA", "APOSITION", id) + "，摄像机目标位置错误，直线时必须填！");
                    }
                    return _cameraMotion;
                }
                //是相对位置
                if (isRelativePos)
                {
                    empty.transform.parent = relativeTrans;
                    empty.transform.localPosition = _cameraMotion.EndPos;
                    _cameraMotion.EndPos = empty.transform.position;
                    empty.transform.parent = null;
                }
            
            }
            if (_cameraMotion.CurrentMotion == CameraMotionType.VisualPath)  //可视化路径运动
            {
                JsonLoad(pathName, _cameraMotion.crSpline, ref is_right);
                if (!is_right)
                {
                    if (MotionPara.isEditor)
                    {
                        Debug.LogError(ErrorLocation.Locate("CAMERA", "PATHNAME", id) + "，摄像机可视化路径名称填写错误！");
                    }
                    return _cameraMotion;
                }
                if (startPosStr != "")
                    _cameraMotion.crSpline.controlPoints[0] = _cameraMotion.StartPos;
                if (endPosStr != "")
                    _cameraMotion.crSpline.controlPoints[_cameraMotion.crSpline.controlPoints.Count - 1] = _cameraMotion.EndPos;
                AngleOptimization(_cameraMotion.crSpline.rotationList);
                _cameraMotion.crSpline.pts = _cameraMotion.crSpline.PathControlPointGenerator(_cameraMotion.crSpline.controlPoints.ToArray());
                _cameraMotion.StandardTime = _cameraMotion.crSpline.TimeGet(_cameraMotion.crSpline.pts, moveSpeed);
                _cameraMotion.crSpline.TimeRateGenerator(_cameraMotion.crSpline.rotationList.Count);
            }
            else  //直线运动过程
            {
                //运动向量
                Vector3 motionVec = _cameraMotion.EndPos - _cameraMotion.StartPos;
                //时间
                _cameraMotion.StandardTime = motionVec.magnitude / moveSpeed;
                //线速度
                _cameraMotion.SpeedVector = motionVec / _cameraMotion.StandardTime;
            }
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
		//判断摄像机是否需要过渡
		if (CameraMotion.TargetTrans != null && _cameraMotion.CurrentMotion != CameraMotionType.Circular && !_cameraMotion.needSetPos)
		{
			_cameraMotion.diffVector[0] = CameraMotion.CurrentCamera.transform.eulerAngles;
			if (_cameraMotion.CurrentMotion == CameraMotionType.Line)
			{
				empty.transform.position = _cameraMotion.LineInterp(0.1f);
			}
			else
			{
				empty.transform.position = _cameraMotion.crSpline.Interp(0.1f);
			}
			empty.transform.LookAt(CameraMotion.TargetTrans);
			_cameraMotion.diffVector[1] = empty.transform.eulerAngles;
			_cameraMotion.diffVector[1] = new Vector3(AngleClerp(_cameraMotion.diffVector[0].x, _cameraMotion.diffVector[1].x, 1f), AngleClerp(_cameraMotion.diffVector[0].y, _cameraMotion.diffVector[1].y, 1f), AngleClerp(_cameraMotion.diffVector[0].z, _cameraMotion.diffVector[1].z, 1f));
			if ((_cameraMotion.diffVector[1] - _cameraMotion.diffVector[0]).magnitude > 0.1f)
			{
				_cameraMotion.needExcess = true;
			}
			else
			{
				_cameraMotion.needExcess = false;
			}
		}
		else
		{
			_cameraMotion.needExcess = false;
		}
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


    //Json文件加载
    private void JsonLoad(string path_name, CRSpline crSpline, ref bool is_right)
    {
        string filePath = MotionPara.taskRootPath + MotionPara.taskName + "/PathControl.json";
        if (File.Exists(filePath))
        {
            JsonOperator jsonOp = new JsonOperator();
            DataTable jsonTable = jsonOp.JsonReader(filePath, path_name);
            if (jsonTable == null)
            {
                Debug.LogError(path_name + ", 该路径名称不存在！");
                return;
            }
            GameObject loadEmpty = new GameObject();
            loadEmpty.name = "JsonLoad_empty";
            loadEmpty.transform.parent = CameraMotion.CurrentCamera.transform;
			if (FuncPara.curentMode == Movement.Zhuang)
			{
				for (int i = jsonTable.Rows.Count - 1; i >= 0; i--)
				{
					loadEmpty.transform.localPosition = ConvertToVector3((string)jsonTable.Rows[i][0].ToString());
					loadEmpty.transform.localEulerAngles = ConvertToVector3((string)jsonTable.Rows[i][1].ToString());
					crSpline.controlPoints.Add(loadEmpty.transform.position);
					crSpline.rotationList.Add(loadEmpty.transform.eulerAngles);
					crSpline.cameraViewList.Add(float.Parse((string)jsonTable.Rows[i][2].ToString()));
				}
			}
			else
			{
				for (int i = 0; i < jsonTable.Rows.Count; i++)
				{
					loadEmpty.transform.localPosition = ConvertToVector3((string)jsonTable.Rows[i][0].ToString());
					loadEmpty.transform.localEulerAngles = ConvertToVector3((string)jsonTable.Rows[i][1].ToString());
					crSpline.controlPoints.Add(loadEmpty.transform.position);
					crSpline.rotationList.Add(loadEmpty.transform.eulerAngles);
					crSpline.cameraViewList.Add(float.Parse((string)jsonTable.Rows[i][2].ToString()));
				}
			}
            GameObject.DestroyImmediate(loadEmpty);
        }
        else
        {
            Debug.LogError(filePath + ", 该文件不存在！");
        }
    }

    //string to vector3
    private Vector3 ConvertToVector3(string vec_string)
    {
        string[] stringArray = vec_string.Split(',');
        Vector3 rVec = Vector3.zero;
        rVec.x = float.Parse(stringArray[0]);
        rVec.y = float.Parse(stringArray[1]);
        rVec.z = float.Parse(stringArray[2]);
        return rVec;
    }

    //角度值优化，因为角度非常不确定，在类似90和-270之间
    private void AngleOptimization(List<Vector3> rotation_list)
    {
        for (int i = 1; i < rotation_list.Count; i++)
        {
            Vector3 tempVec = rotation_list[i];
            tempVec.x = AngleClerp(rotation_list[i - 1].x, rotation_list[i].x, 1f);
            tempVec.y = AngleClerp(rotation_list[i - 1].y, rotation_list[i].y, 1f);
            tempVec.z = AngleClerp(rotation_list[i - 1].z, rotation_list[i].z, 1f);
            rotation_list[i] = tempVec;
        }
    }

    //角度修改使其符合实际（类似-90和270这种情况处理）
    private float AngleClerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }
}
