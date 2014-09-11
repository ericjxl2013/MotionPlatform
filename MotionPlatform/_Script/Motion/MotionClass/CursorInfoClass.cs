/// <summary>
/// <Filename>: CursorInfoClass.cs
/// Author: Wu Hao
/// Created: 2014.09.02
/// Version: 1.0
/// Company: Sunnytech
/// Function: 小手移动动画类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public enum CursorMotionType {Line, Circular}

public class CursorMotion : IMotion{

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

	//运动属性
	public CursorMotionType CurrentMotion = CursorMotionType.Line;

	//开始位置
	public Vector2 StartPos = new Vector3(0f, 0f);
	//结束位置
	public Vector2 EndPos = new Vector3(0f, 0f);
	//速度矢量
	public float MoveSpeed = 1.0f;
	//旋转中心
	public Vector2 RotateCenter = new Vector3(0f, 0f);
	//是否顺时针
	public bool Clockwise = true;
	//旋转速度矢量
	public float RotateSpeed = 80.0f;
	//旋转角度
	public float RotateAngle = 60.0f;

	//直线运动的结束位置是否是Button
	public bool isButton = false;
	//长按的条件判断
	public float Step_LongPress = -1f;

	//鼠标贴图运动结束后是否消失
	public bool Disappear = true;
	//触发
	public string Trigger = "";

	MotionManager mm;

	//结构初始化
	public void Init()
	{
		//mm = GameObject.Find("MainScript").GetComponent<MotionManager>();
		mm = (MotionManager)GameObject.FindObjectOfType(typeof(MotionManager));
		if(CurrentMotion == CursorMotionType.Line){
			mm.st_CursorMove.MovingStart(StartPos, EndPos, MoveSpeed);
		}
		else{
			mm.st_CursorMove.RotateStart(StartPos, RotateCenter, Clockwise, RotateAngle, RotateSpeed);
		}
	}
	
	//暂停控制--通过MotionPara.PauseControl控制
	public void Pause(bool play_status)
	{
		
	}
	
	//速率控制
	public void ChangeRate(float current_rate, float current_time)
	{
		if(CurrentMotion == CursorMotionType.Line){//直线
			mm.st_CursorMove.ChangeRate(current_rate, MotionPara.SpeedRate);
		}
		else{
			mm.st_CursorMove.ChangeRate_Rotate(current_rate, MotionPara.SpeedRate);
		}
	}
	
	//运动
	public void Move(float current_time, float speed_rate, float delta_time)
	{

	}
	
	//时间管理
	public bool TimesUp(float current_time)
	{
		if(CurrentMotion == CursorMotionType.Line){
			return !mm.st_CursorMove.movingCompute;
		}
		else{
			return !mm.st_CursorMove.rotateCompute;
		}
	}
	
	//后处理，主要是位置修正
	public void PostProcess()
	{
		if(CurrentMotion == CursorMotionType.Line){//直线
			FuncPara.cursorPosition = new Vector2(EndPos.x, EndPos.y);

			//按钮单击，长按的触发的后处理，暂时不处理
		}
	}
}

public class CursorInfoManager : BaseCompute {
	private CursorMotion _cursorMotion;
	
	public CursorInfoManager ()
	{
		_cursorMotion = new CursorMotion();
	}
	public CursorMotion CursorInfoGet_Test(){
		_cursorMotion.CurrentMotion = CursorMotionType.Line;
		_cursorMotion.StartPos = new Vector2(100,100);
		_cursorMotion.EndPos = new Vector2(500,500);
		return _cursorMotion;
	}
	public CursorMotion CursorInfoGet_Test2(){
		_cursorMotion.CurrentMotion = CursorMotionType.Circular;
		_cursorMotion.StartPos = new Vector2(100,100);
		_cursorMotion.RotateCenter = new Vector2(300,300);
		_cursorMotion.Clockwise = true;
		return _cursorMotion;
	}

	//Cursor表格信提取
	public CursorMotion CursorInfoGet(DataRow cursor_row, string id, ref bool is_right)
	{
		//圆弧运动和直线运动确定，圆弧优先级高于直线，根据【旋转中心:5】是否有填来确定
		bool isCircularMotion = false;
		string rotateRefStr = (string)cursor_row[5].ToString();
		if(rotateRefStr == ""){  //直线运动
			isCircularMotion = false;
			_cursorMotion.CurrentMotion = CursorMotionType.Line;
		}else{  //旋转运动
			isCircularMotion = true;
			_cursorMotion.CurrentMotion = CursorMotionType.Circular;

			if(cursor_row[5].ToString().Contains(",")){//具体Vector2
				try{
					_cursorMotion.RotateCenter = Vector2Conversion(cursor_row[5].ToString(), ref is_right);
				}catch{
					is_right = false;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATECENTER", id) + "，鼠标贴图旋转的旋转中心出错！");
					}
					return _cursorMotion;
				}
			}
			else{//button的名称,GameObject的名称
				if(GameObject.Find((string)cursor_row[5].ToString()) != null){ //Gameobject
					try{
						GameObject obj = GameObject.Find((string)cursor_row[5].ToString());
						Vector3 obj_pos = GameObject.Find(MotionEditor.mainCamera).camera.WorldToScreenPoint(obj.transform.position);
						_cursorMotion.RotateCenter = new Vector2(obj_pos.x, Screen.height-obj_pos.y);
					}catch{
						is_right = false;
						if(MotionPara.isEditor){
							Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATECENTER", id) + "，鼠标贴图旋转的旋转中心出错！");
						}
						return _cursorMotion;
					}
				}
				else{//Button
					_cursorMotion.RotateCenter = PanelButton.ButtonLocation((string)cursor_row[5].ToString(), ref is_right);
					if(!is_right){
						if(MotionPara.isEditor){
							Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATECENTER", id) + "，鼠标贴图旋转的旋转中心出错！");
						}
						return _cursorMotion;
					}
				}
			}
		}

		//初始位置检查
		string startPos = (string)cursor_row[2].ToString();
		if(startPos != ""){
			if(startPos.Contains(",")){//具体Vector2
				_cursorMotion.StartPos = Vector2Conversion(startPos, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "STARTPOSITION", id) + "，鼠标贴图运动的初始位置出错！");
					}
					return _cursorMotion;
				}
			}
			else{//button的名称,GameObject的名称
				if(GameObject.Find(startPos) != null){ //Gameobject
					try{
						GameObject obj = GameObject.Find(startPos);
						Vector3 obj_pos = GameObject.Find(MotionEditor.mainCamera).camera.WorldToScreenPoint(obj.transform.position);
						_cursorMotion.StartPos = new Vector2(obj_pos.x, Screen.height-obj_pos.y);
					}catch{
						is_right = false;
						if(MotionPara.isEditor){
							Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "STARTPOSITION", id) + "，鼠标贴图运动的初始位置出错！");
						}
						return _cursorMotion;
					}
				}
				else{//Button
					_cursorMotion.StartPos = PanelButton.ButtonLocation(startPos, ref is_right);
					if(!is_right){
						if(MotionPara.isEditor){
							Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "STARTPOSITION", id) + "，鼠标贴图运动的初始位置出错！");
						}
						return _cursorMotion;
					}
				}
			}
		}
		else{
			//默认为当前位置
			_cursorMotion.StartPos = new Vector2(FuncPara.cursorPosition.x, FuncPara.cursorPosition.y);
		}
		//最终位置检查
		string endPos = (string)cursor_row[3].ToString();
		_cursorMotion.isButton = false;
		if(endPos == ""){
			if(!isCircularMotion){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ENDPOSITION", id) + "，鼠标贴图直线运动的最终位置不能为空！");
				}
				return _cursorMotion;
			}
		}
		else{
			if(!isCircularMotion){
				if(endPos.Contains(",")){//具体Vector2
					_cursorMotion.EndPos = Vector2Conversion(endPos, ref is_right);
					if(!is_right){
						if(MotionPara.isEditor){
							Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ENDPOSITION2", id) + "，鼠标贴图直线运动的最终位置出错！");
						}
						return _cursorMotion;
					}
				}
				else{//button的名称,GameObject的名称
					if(GameObject.Find(endPos) != null){ //Gameobject
						try{
							GameObject obj = GameObject.Find(endPos);
							Vector3 obj_pos = GameObject.Find(MotionEditor.mainCamera).camera.WorldToScreenPoint(obj.transform.position);
							_cursorMotion.EndPos = new Vector2(obj_pos.x, Screen.height-obj_pos.y);
						}catch{
							is_right = false;
							if(MotionPara.isEditor){
								Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ENDPOSITION2", id) + "，鼠标贴图直线运动的最终位置出错！");
							}
							return _cursorMotion;
						}
					}
					else{//Button
						_cursorMotion.EndPos = PanelButton.ButtonLocation(endPos, ref is_right);
						_cursorMotion.isButton = true;
						if(!is_right){
							if(MotionPara.isEditor){
								Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ENDPOSITION2", id) + "，鼠标贴图直线运动的最终位置出错！");
							}
							return _cursorMotion;
						}

//						string step_longpress = (string)cursor_row[9].ToString();
//						if(step_longpress != ""){
//							_cursorMotion.Step_LongPress = FloatConversion(step_longpress, ref is_right);
//							if(!is_right){
//								if(MotionPara.isEditor){
//									Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "LONGPRESS", id) + "，鼠标贴图直线运动的长按按钮的状态出错！");
//								}
//								return _cursorMotion;
//							}
//						}
					}
				}
			}
		}

		if(!isCircularMotion){//直线运动时间
			_cursorMotion.StandardTime = (_cursorMotion.EndPos - _cursorMotion.StartPos).magnitude / CursorMove.SPEED_MOVE;
			_cursorMotion.StandardTime = _cursorMotion.StandardTime/MotionPara.SpeedRate;
		}

		//直线移动速度
		_cursorMotion.MoveSpeed = MotionPara.Move_Speed;
		string moveSpeed = (string)cursor_row[4].ToString();
		if(moveSpeed != ""){
			if(!isCircularMotion){
				_cursorMotion.MoveSpeed = FloatConversion(moveSpeed, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "MOVESPEED", id) + "，鼠标贴图直线运动的速度出错！");
					}
					return _cursorMotion;
				}


				_cursorMotion.StandardTime = _cursorMotion.StandardTime/_cursorMotion.MoveSpeed;
			}
		}

		//是否顺时针
		string isClockwise = (string)cursor_row[6].ToString();
		if(isClockwise == ""){
			if(isCircularMotion){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATEDIRECTION", id) + "，鼠标贴图旋转运动的方向为空！");

				}
				return _cursorMotion;
			}
		}
		else{
			if(isCircularMotion){
				isClockwise = isClockwise.ToUpper();
				if(isClockwise == "TRUE"){
					_cursorMotion.Clockwise = true;
				}
				else if(isClockwise == "FALSE"){
					_cursorMotion.Clockwise = false;
				}
				else{
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATEDIRECTION2", id) + "，鼠标贴图旋转运动的方向出错！");
				}
			}
		}

		//旋转角度
		string rotateAngle = (string)cursor_row[8].ToString();
		if(rotateAngle == ""){
			if(isCircularMotion){
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATEANGLE", id) + "，鼠标贴图旋转运动的旋转角度为空！");
				}
				return _cursorMotion;
			}
		}
		else{
			if(isCircularMotion){
				_cursorMotion.RotateAngle = FloatConversion(rotateAngle, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATEANGLE2", id) + "，鼠标贴图旋转运动的旋转角度出错！");
					}
					return _cursorMotion;
				}
			}
		}

		if(isCircularMotion){//旋转运动时间
			_cursorMotion.StandardTime = _cursorMotion.RotateAngle / CursorMove.SPEED_ROTATE;
			_cursorMotion.StandardTime = _cursorMotion.StandardTime/MotionPara.SpeedRate;
		}

		//旋转速度
		_cursorMotion.RotateSpeed = MotionPara.Rotate_Speed;
		string rotateSpeed = (string)cursor_row[7].ToString();
		if(rotateSpeed != ""){
			if(isCircularMotion){
				_cursorMotion.RotateSpeed = FloatConversion(rotateSpeed, ref is_right);
				if(!is_right){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "ROTATESPEED", id) + "，鼠标贴图旋转运动的旋转速度出错！");
					}
					return _cursorMotion;
				}

				_cursorMotion.StandardTime = _cursorMotion.StandardTime/_cursorMotion.RotateSpeed;
			}
		}

		string step_longpress = (string)cursor_row[9].ToString();
		if(step_longpress != ""){
			_cursorMotion.Step_LongPress = FloatConversion(step_longpress, ref is_right);
			if(!is_right){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "LONGPRESS", id) + "，鼠标贴图直线运动的长按按钮的状态出错！");
				}
				return _cursorMotion;
			}
		}

		string disappear = (string)cursor_row[10].ToString();
		if(disappear != ""){
			if(disappear.ToUpper() == "TRUE"){
				_cursorMotion.Disappear = true;
			}
			else if(disappear.ToUpper() == "FALSE"){
				_cursorMotion.Disappear = false;
			}
			else{
				is_right = false;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("CURSORMOVE", "DISAPPEAR", id) + "，鼠标贴图运动的结束贴图显示设置出错！");
				}
				return _cursorMotion;
			}
		}

		string trigger = (string)cursor_row[11].ToString();
		if(trigger != ""){
			_cursorMotion.Trigger = trigger;
		}

		return _cursorMotion;
	}
	
}
