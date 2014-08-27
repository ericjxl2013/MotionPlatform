/// <summary>
/// <Filename>: ProgramInfoClass.cs
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

public class CubeDiscolor : IMotion
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

	//起始颜色
	public Color StartColor = Color.green;
	//终止颜色
	public Color EndColor = Color.red;
	//颜色转化速率
	public Color ColorSpeed = new Color(0f, 0f, 0f, 0f);

	private GameObject cube;

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

	//结构初始化
	public void Init()
	{
		cube = GameObject.Find("Cube");
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
		cube.renderer.material.color = StartColor + ColorSpeed * current_time * speed_rate;
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
		cube.renderer.material.color = EndColor;
	}
}


//Program表格动画信息处理类
public class ProgramInfoManager : BaseCompute
{


	//PROGRAM表格信息提取
	public bool ProgramInfoGet(DataRow program_row, string id, DataTable group_table, IList<IMotion> _motionList, 
		IList<IList<IMotion>> _complexMotionList){
		//提取Program ID
		string programID = ((string)program_row[2].ToString()).ToUpper();
		//检验ID是否正确
		if(MotionPara.programMotionID.Contains(programID)){
			if(programID == "TONGBANG"){
				//铜棒敲击
				return CopperHit(program_row, id, group_table, _motionList, _complexMotionList);
			}else if(programID == "BAIFANG"){
				//物体摆放
				return ObjPut(program_row, id, group_table, _motionList, _complexMotionList);
			}else if(programID == "NINGSONG"){
				//螺钉拧松拧紧
				return NingSong(program_row, id, group_table, _motionList, _complexMotionList);
			}else if(programID == "NINGCHU"){
				//螺钉拧进拧出
				return NingChu(program_row, id, group_table, _motionList, _complexMotionList);
			}else{
				//Program ID错误
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("PROGRAM", "PROGRAMID", id) + "，" + programID + 
						"：此ProgramID暂未定义，请联系相关程序猿，请确认！");
				}
				return false;
			}
		}else{
			//Program ID错误
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "PROGRAMID", id) + "，" + programID + "：此ProgramID不存在，请确认！");
			}
			return false;
		}
	}

	//铜棒敲击处理
	private bool CopperHit(DataRow program_row, string id, DataTable group_table, IList<IMotion> _motionList, 
		IList<IList<IMotion>> _complexMotionList){
		//铜棒运动时的父亲
		GameObject empty = new GameObject();
		empty.name = "copper_empty-" + id;
		Transform copperTrans = null;
		Transform firstObj = null;
		// Transform firstObjParent = null;
		List<Transform> parentList = new List<Transform>();
		List<Transform> childList = new List<Transform>();
		try{
			//铜棒
			copperTrans = GameObject.Find(MotionPara.copperName).transform;
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(MotionPara.copperName + "：在此场景中未能找到铜棒！");
			}
			return false;
		}
		bool isRight = true;
		//Group Column
		int groupColumn = IntConversion(program_row[3].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "TBGROUP", id) + "，铜棒运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			firstObj = GameObject.Find(objName).transform;
			// firstObjParent = firstObj.parent;
			empty.transform.position = firstObj.position;
			empty.transform.eulerAngles = firstObj.eulerAngles;
			// firstObj.parent = empty.transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				childList.Add(tempTrans);
				parentList.Add(tempTrans.parent);
				// tempTrans.parent = empty.transform;	
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "TBGROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//敲击次数
		int copperTimes = IntConversion((string)program_row[4].ToString(), ref isRight);
		if(!isRight || copperTimes == 0){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "TBTIMES", id) + "，铜棒敲击次数填写有错误！");
			}
			return false;
		}
		//铜棒离敲击点距离
		Vector3 disTBVec = Vector3Conversion((string)program_row[5].ToString(), ref isRight);
		if(!isRight || disTBVec.magnitude == 0){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "TBDIS", id) + "，铜棒敲击距离填写有错误！");
			}
			return false;
		}
		float disTB = disTBVec.magnitude;  //铜棒移动距离
		//物体移动方向
		Vector3 dirTBVec = Vector3Conversion((string)program_row[6].ToString(), ref isRight);
		if(!isRight || dirTBVec.magnitude == 0){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "TBDIR", id) + "，铜棒敲击物体移动方向填写有错误！");
			}
			return false;
		}
		//移动速度确定
		float copperHitSpeed = MotionPara.copperHitSpeed;
		//检测是否有自定义速度
		if(program_row.Table.Columns.Count > 7){
			if((string)program_row[7].ToString() != ""){
				copperHitSpeed =  FloatConversion((string)program_row[7].ToString(), ref isRight);
				if(!isRight || copperHitSpeed == 0){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("EXCEL", "TBSPEED", id) + "，铜棒自定义速度填写有错误！");
					}
					return false;
				}
			}
		}
		float copperBackSpeed = copperHitSpeed / 3f;
		//铜棒敲击一次移动距离
		float copperForwardDis = MotionPara.copperForwardDis;
		//检测是否有自定义距离
		if(program_row.Table.Columns.Count > 8){
			if((string)program_row[8].ToString() != ""){
				copperForwardDis =  FloatConversion((string)program_row[8].ToString(), ref isRight);
				if(!isRight || copperForwardDis == 0){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("EXCEL", "TBFORWARDDIS", id) + "，铜棒自定义敲击移动距离填写有错误！");
					}
					return false;
				}
			}
		}
		GameObject emptyPos = new GameObject();
		emptyPos.name = "copper_empty_position--" + id;
		//敲击类创建
		IList<IMotion> tempMotionList = new List<IMotion>();
		for(int i = 0; i < copperTimes; i++)
		{
			//铜棒敲击---New Class
			BasicMotion _simpleMotion = new BasicMotion();
			Vector3 motionVec = MotionPara.copperVector * (disTB + i * copperForwardDis);
			_simpleMotion.CurrentMotion = MotionType.MoveOnly;
			_simpleMotion.ParentTrans = copperTrans;
			_simpleMotion.MoveSpeed = copperHitSpeed;
			_simpleMotion.StandardTime = motionVec.magnitude / copperHitSpeed;
			_simpleMotion.SpeedVector = motionVec / _simpleMotion.StandardTime;
			//起始信息记录
			_simpleMotion.StartPos = copperTrans.position;
			_simpleMotion.StartEurler = copperTrans.eulerAngles;
			emptyPos.transform.position = _simpleMotion.StartPos;
			emptyPos.transform.eulerAngles = _simpleMotion.StartEurler;
			emptyPos.transform.Translate(motionVec, Space.Self);
			//终止信息记录
			_simpleMotion.EndPos = emptyPos.transform.position;
			_simpleMotion.EndEurler = emptyPos.transform.eulerAngles;
			_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
			if(i == 0){
				_simpleMotion.State = CurrentState.Active;
			}else{
				_simpleMotion.State = CurrentState.Future;
			}
			tempMotionList.Add(_simpleMotion);

			//被敲击物体瞬移---New Class
			BasicMotion _simpleMotion2 = new BasicMotion();
			_simpleMotion2.State = CurrentState.Future;
			_simpleMotion2.CurrentMotion = MotionType.SetPos;
			_simpleMotion2.ParentTrans = firstObj;
			_simpleMotion2.ListCopy(childList, parentList);
			//起始信息记录
			emptyPos.transform.position = empty.transform.position;
			emptyPos.transform.eulerAngles = empty.transform.eulerAngles;
			_simpleMotion2.StartPos = emptyPos.transform.position;
			_simpleMotion2.StartEurler = emptyPos.transform.eulerAngles;
			emptyPos.transform.Translate(dirTBVec.normalized * (i + 1) * copperForwardDis, Space.Self);
			//终止信息记录
			_simpleMotion2.EndPos = emptyPos.transform.position;
			_simpleMotion2.EndEurler = emptyPos.transform.eulerAngles;
			//时间记录
			_simpleMotion2.StandardTime = 0.05f;
			_simpleMotion2.EndTime = 0.05f;
			tempMotionList.Add(_simpleMotion2);

			//铜棒返回---New Class
			BasicMotion _simpleMotion3 = new BasicMotion();
			_simpleMotion3.State = CurrentState.Future;
			_simpleMotion3.CurrentMotion = MotionType.MoveOnly;
			_simpleMotion3.ParentTrans = copperTrans;
			_simpleMotion3.MoveSpeed = copperBackSpeed;
			_simpleMotion3.StandardTime = motionVec.magnitude / copperBackSpeed;
			_simpleMotion3.SpeedVector = -motionVec / _simpleMotion3.StandardTime;
			//起始信息记录
			_simpleMotion3.StartPos = _simpleMotion.EndPos;
			_simpleMotion3.StartEurler = _simpleMotion.EndEurler;
			//终止信息记录
			_simpleMotion3.EndPos = _simpleMotion.StartPos;
			_simpleMotion3.EndEurler = _simpleMotion.StartEurler;
			_simpleMotion3.EndTime = _simpleMotion3.StandardTime / _simpleMotion3.SpeedRate;
			tempMotionList.Add(_simpleMotion3);
		}
		_complexMotionList.Add(tempMotionList);
		GameObject.DestroyImmediate(emptyPos);
		GameObject.DestroyImmediate(empty);
		return true;
	}

	//部件摆放处理
	private bool ObjPut(DataRow program_row, string id, DataTable group_table, IList<IMotion> _motionList, 
		IList<IList<IMotion>> _complexMotionList){
		Transform firstObj = null;
		List<Transform> parentList = new List<Transform>();
		List<Transform> childList = new List<Transform>();
		bool isRight = true;
		//Group Column
		int groupColumn = IntConversion((string)program_row[3].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFGROUP", id) + "，任意摆放运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			firstObj = GameObject.Find(objName).transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				childList.Add(tempTrans);
				parentList.Add(tempTrans.parent);
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFGROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//参考物体
		Transform referTrans = null;
		try{
			referTrans = GameObject.Find((string)program_row[4].ToString()).transform;
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFREFER", id) + "，任意摆放运动参考物体填写错误！");
			}
			return false;
		}
		//判断是工具移出还是任意部件移动，默认为任意部件
		bool isTool = false;
		if(program_row.Table.Columns.Count >= 5){
			if((string)program_row[5].ToString() == ""){
				isTool = true;
			}
		}
		//任意部件移动信息提取
		Vector3 finalPos = new Vector3(0f, 0f, 0f);
		Vector3 finalEuler = new Vector3(0f, 0f, 0f);
		if(!isTool){
			if(program_row.Table.Columns.Count >= 7){
				//提取相对位置
				finalPos = Vector3Conversion((string)program_row[5].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFPOS", id) + "，相对位置信息填写有错误！");
					}
					return false;
				}
				//提取相对角度
				finalEuler = Vector3Conversion((string)program_row[6].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFEULER", id) + "，相对角度信息填写有错误！");
					}
					return false;
				}
				GameObject emptyRefer = new GameObject();
				emptyRefer.name = "ObjPut_empty_Refer-" + id;
				emptyRefer.transform.parent = referTrans;
				emptyRefer.transform.localPosition = finalPos;
				emptyRefer.transform.localEulerAngles = finalEuler;
				finalPos = emptyRefer.transform.position;
				finalEuler = emptyRefer.transform.eulerAngles;
				emptyRefer.transform.parent = null;
				GameObject.DestroyImmediate(emptyRefer);
			}else{
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFEULER", id) + "，相对位置或角度信息填写有错误！");
				}
				return false;
			}
		}else{
			int toolIndex = MotionPara.toolsName.IndexOf(objName);
			if(toolIndex == -1){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFGROUP", id) + 
						"，Group号对应的Group表第一个物体在Tool.xls工具信息中找不到，请确认！");
				}
				return false;
			}else{
				finalPos = MotionPara.toolsInitPos[toolIndex];
				finalEuler = MotionPara.toolsInitEuler[toolIndex];
			}
		}
		//自定义速度提取
		float moveSpeed = MotionPara.toolMoveSpeed;
		if(program_row.Table.Columns.Count > 7){
			if((string)program_row[7].ToString() != ""){
				moveSpeed = FloatConversion((string)program_row[7].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFSPEED", id) + "，自定义速度填写有错误！");
					}
					return false;
				}
			}
		}
		//自定义安全高度
		float safeHeight = MotionPara.safeHeight;
		if(program_row.Table.Columns.Count > 8){
			if((string)program_row[8].ToString() != ""){
				safeHeight = FloatConversion((string)program_row[8].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "BFHEIGHT", id) + "，自定义安全高度填写有错误！");
					}
					return false;
				}
			}
		}

		//开始生成类数据
		IList<IMotion> tempMotionList = new List<IMotion>();
		//首先上升到安全高度
		//New Class，世界坐标移动
		BasicMotion _simpleMotion = new BasicMotion();
		_simpleMotion.State = CurrentState.Active;
		_simpleMotion.CurrentMotion = MotionType.WorldMove;
		_simpleMotion.StartPos = firstObj.position;
		_simpleMotion.StartEurler = firstObj.eulerAngles;
		_simpleMotion.ParentTrans = firstObj;
		_simpleMotion.ListCopy(childList, parentList);
		//如果部件摆放的safeHeight对应的轴要改，修改这个参数
		Vector3 motionVec = new Vector3(0f, safeHeight - _simpleMotion.StartPos.y, 0f);
		_simpleMotion.EndPos = new Vector3(_simpleMotion.StartPos.x, safeHeight, _simpleMotion.StartPos.z);
		_simpleMotion.EndEurler = _simpleMotion.StartEurler;
		if(motionVec.magnitude == 0f){
			_simpleMotion.StandardTime = 0.05f;
			_simpleMotion.SpeedVector = new Vector3(0f, 0f, 0f);
		}else{
			_simpleMotion.StandardTime = motionVec.magnitude / moveSpeed;
			_simpleMotion.SpeedVector = motionVec / _simpleMotion.StandardTime;
		}
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		tempMotionList.Add(_simpleMotion);

		//New Class---任意移动过度到目标位置上方
		BasicMotion _simpleMotion2 = new BasicMotion();
		_simpleMotion2.State = CurrentState.Future;
		_simpleMotion2.CurrentMotion = MotionType.RandomMotion;
		_simpleMotion2.StartPos = _simpleMotion.EndPos;
		_simpleMotion2.StartEurler = _simpleMotion.EndEurler;
		_simpleMotion2.ParentTrans = firstObj;
		_simpleMotion2.ListCopy(childList, parentList);
		_simpleMotion2.EndPos = new Vector3(finalPos.x, safeHeight, finalPos.z);
		_simpleMotion2.EndEurler = finalEuler;
		motionVec = _simpleMotion2.EndPos - _simpleMotion2.StartPos;
		float moveTime = 0;
		float rotateTime = 0;
		moveTime = motionVec.magnitude / moveSpeed;
		Vector3 angleDiff = AngleDiff(AngleConversion(_simpleMotion2.EndEurler) - 
			AngleConversion(_simpleMotion2.StartEurler));
		rotateTime = Vector3.Angle(_simpleMotion2.StartEurler, _simpleMotion2.EndEurler) / MotionPara.toolRotateSpeed;
		if(moveTime == 0 && rotateTime == 0){
			_simpleMotion2.StandardTime = 0.05f;
			_simpleMotion2.SpeedVector = Vector3.zero;
			_simpleMotion2.AngleVector = Vector3.zero;
		}else if(moveTime > rotateTime){
			_simpleMotion2.StandardTime = moveTime;
			_simpleMotion2.SpeedVector = motionVec / moveTime;
			_simpleMotion2.AngleVector = angleDiff / moveTime;
		}else{
			_simpleMotion2.StandardTime = rotateTime;
			_simpleMotion2.SpeedVector = motionVec / rotateTime;
			_simpleMotion2.AngleVector = angleDiff / rotateTime;
		}
		_simpleMotion2.EndTime = _simpleMotion2.StandardTime / _simpleMotion2.SpeedRate;
		tempMotionList.Add(_simpleMotion2);

		//New Class，下移到目标位置
		BasicMotion _simpleMotion3 = new BasicMotion();
		_simpleMotion3.State = CurrentState.Future;
		_simpleMotion3.CurrentMotion = MotionType.WorldMove;
		_simpleMotion3.StartPos = _simpleMotion2.EndPos;
		_simpleMotion3.StartEurler = _simpleMotion2.EndEurler;
		_simpleMotion3.ParentTrans = firstObj;
		_simpleMotion3.ListCopy(childList, parentList);
		_simpleMotion3.EndPos = finalPos;
		_simpleMotion3.EndEurler = finalEuler;
		motionVec = _simpleMotion3.EndPos - _simpleMotion3.StartPos;
		if(motionVec.magnitude == 0f){
			_simpleMotion3.StandardTime = 0.05f;
			_simpleMotion3.SpeedVector = new Vector3(0f, 0f, 0f);
		}else{
			_simpleMotion3.StandardTime = motionVec.magnitude / moveSpeed;
			_simpleMotion3.SpeedVector = motionVec / _simpleMotion3.StandardTime;
		}
		_simpleMotion3.EndTime = _simpleMotion3.StandardTime / _simpleMotion3.SpeedRate;
		tempMotionList.Add(_simpleMotion3);
		_complexMotionList.Add(tempMotionList);
		return true;
	}

	//螺钉拧松拧紧
	private bool NingSong(DataRow program_row, string id, DataTable group_table, IList<IMotion> _motionList, 
		IList<IList<IMotion>> _complexMotionList){
		Transform firstObj = null;
		Transform firstObjParent = null;
		List<Transform> parentList = new List<Transform>();
		List<Transform> childList = new List<Transform>();
		Transform screwTrans = null;
		Transform screwTransParent = null;
		bool isRight = true;
		//Group Column
		int groupColumn = IntConversion((string)program_row[3].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSGROUP", id) + "，螺钉拧松运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			firstObj = GameObject.Find(objName).transform;
			firstObjParent = firstObj.parent;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				childList.Add(tempTrans);
				parentList.Add(tempTrans.parent);
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSGROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//被拧物体
		try{
			screwTrans = GameObject.Find((string)program_row[4].ToString()).transform;
			screwTransParent = screwTrans.parent;
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSREFER", id) + "，被拧物体填写有错误！");
			}
			return false;
		}
		//拧的次数
		int twistTimes = IntConversion((string)program_row[5].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSTIMES", id) + "，拧的次数填写有错误！");
			}
			return false;
		}
		//扳手到螺钉距离
		Vector3 disNSVec = Vector3Conversion((string)program_row[6].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSDIS", id) + "，拧的距离填写有错误！");
			}
			return false;
		}
		float disNS = disNSVec.magnitude;
		//旋转轴
		Vector3 rotateAxis = Vector3Conversion((string)program_row[7].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSAXIS", id) + "，拧的旋转轴填写有错误！");
			}
			return false;
		}
		//拧的角度大小
		float twistDegree = FloatConversion((string)program_row[8].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSDEGREE", id) + "，拧的旋转轴填写有错误！");
			}
			return false;
		}
		//移动速度
		float moveSpeed = MotionPara.toolMoveSpeed;
		if(program_row.Table.Columns.Count > 9){
			if((string)program_row[9].ToString() != ""){
				moveSpeed = FloatConversion((string)program_row[9].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSSPEED", id) + "，自定义速度填写有错误！");
					}
					return false;
				}
			}
		}
		//扳手移出的微小距离
		float wrenchBackDis = MotionPara.wrenchBackDis;
		if (program_row.Table.Columns.Count > 10)
		{
			if ((string)program_row[10].ToString() != "")
			{
				wrenchBackDis = FloatConversion((string)program_row[10].ToString(), ref isRight);
				if (!isRight)
				{
					if (MotionPara.isEditor)
					{
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "NSWREHCHDIS", id) + "，自定义扳手回退距离填写有错误！");
					}
					return false;
				}
			}
		}


		//参考空物体
		GameObject emptyRefer = new GameObject();
		emptyRefer.name = "NingSong_empty-" + id;
		//开始生成类数据
		IList<IMotion> tempMotionList = new List<IMotion>();
		//首次移进  New---Class
		BasicMotion _simpleMotion = new BasicMotion();
		_simpleMotion.State = CurrentState.Active;
		_simpleMotion.CurrentMotion = MotionType.MoveOnly;
		_simpleMotion.StartPos = firstObj.position;
		_simpleMotion.StartEurler = firstObj.eulerAngles;
		emptyRefer.transform.position = firstObj.position;
		emptyRefer.transform.eulerAngles = firstObj.eulerAngles;
		_simpleMotion.ParentTrans = firstObj;
		_simpleMotion.ListCopy(childList, parentList);
			
		//扳手移进时的坐标轴，如果要通用化，请修改这里
		Vector3 moveVecUnit = new Vector3(-1.0f, 0f, 0f);
		//扳手拧一圈时移动的距离
		float screwBack = (twistDegree / 30f) * MotionPara.screwBackRate;
		Vector3 motionVec = moveVecUnit * disNS;
		emptyRefer.transform.Translate(motionVec, Space.Self);
		_simpleMotion.EndPos = emptyRefer.transform.position;
		_simpleMotion.EndEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion.StandardTime = motionVec.magnitude / moveSpeed;
		_simpleMotion.SpeedVector = motionVec / _simpleMotion.StandardTime;
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		tempMotionList.Add(_simpleMotion);

		for(int i = 0; i < twistTimes; i++){
			//拧  New---Class
			BasicMotion _simpleMotion2 = new BasicMotion();
			_simpleMotion2.State = CurrentState.Future;
			_simpleMotion2.CurrentMotion = MotionType.MoveRotateSingle;
			_simpleMotion2.ParentTrans = screwTrans;
			_simpleMotion2.ListCopy(childList, parentList);
			_simpleMotion2.ChildList.Add(firstObj);
			_simpleMotion2.ParentList.Add(firstObjParent);
			_simpleMotion2.ChildList.Add(screwTrans);
			_simpleMotion2.ParentList.Add(screwTransParent);
			GameObject emptyObj = new GameObject();
			emptyObj.name = "move-rotate_empty-" + id + "-" + i.ToString();
			emptyObj.transform.position = screwTrans.position;
			emptyObj.transform.eulerAngles = screwTrans.eulerAngles;
			//将emptyRefer当做扳手用于后面过程定位
			emptyRefer.transform.parent = emptyObj.transform;
			_simpleMotion2.StartPos = screwTrans.position;
			_simpleMotion2.StartEurler = screwTrans.eulerAngles;
			_simpleMotion2.RotateAxis = rotateAxis;
			_simpleMotion2.RotateSpeed = MotionPara.wrenchSpeed;
			_simpleMotion2.StandardTime = twistDegree / _simpleMotion2.RotateSpeed;
			motionVec = -rotateAxis * screwBack;
			emptyObj.transform.Translate(motionVec, Space.Self);
			motionVec = _simpleMotion2.ParentTrans.TransformDirection(motionVec).normalized * screwBack;
			_simpleMotion2.SpeedVector = motionVec / _simpleMotion2.StandardTime;
			_simpleMotion2.EndPos = emptyObj.transform.position;
			emptyObj.transform.Rotate(rotateAxis * twistDegree, Space.Self);
			_simpleMotion2.EndEurler = emptyObj.transform.eulerAngles;
			emptyRefer.transform.parent = null;
			emptyObj.transform.position = _simpleMotion2.StartPos;
			emptyObj.transform.eulerAngles = _simpleMotion2.StartEurler;
			_simpleMotion2.EmptyObj = emptyObj;
			_simpleMotion2.EndTime = _simpleMotion2.StandardTime / _simpleMotion2.SpeedRate;
			tempMotionList.Add(_simpleMotion2);
			//中间循环动作
			if(twistTimes - i > 1){
				//扳手退出很小的距离  New---Class
				BasicMotion _simpleMotion3 = new BasicMotion();
				_simpleMotion3.State = CurrentState.Future;
				_simpleMotion3.CurrentMotion = MotionType.MoveOnly;
				_simpleMotion3.ParentTrans = firstObj;
				_simpleMotion3.ListCopy(childList, parentList);
				_simpleMotion3.StartPos = emptyRefer.transform.position;
				_simpleMotion3.StartEurler = emptyRefer.transform.eulerAngles;
				motionVec = new Vector3(1f, 0f, 0f) * wrenchBackDis / 3;
				emptyRefer.transform.Translate(motionVec, Space.Self);
				_simpleMotion3.EndPos = emptyRefer.transform.position;
				_simpleMotion3.EndEurler = emptyRefer.transform.eulerAngles;
				_simpleMotion3.StandardTime = motionVec.magnitude / moveSpeed;
				_simpleMotion3.SpeedVector = motionVec / _simpleMotion3.StandardTime;
				_simpleMotion3.EndTime = _simpleMotion3.StandardTime / _simpleMotion3.SpeedRate;
				tempMotionList.Add(_simpleMotion3);

				//扳手回转  New---Class
				BasicMotion _simpleMotion4 = new BasicMotion();
				_simpleMotion4.State = CurrentState.Future;
				_simpleMotion4.CurrentMotion = MotionType.RotateOnly;
				_simpleMotion4.ParentTrans = firstObj;
				_simpleMotion4.ListCopy(childList, parentList);
				_simpleMotion4.StartPos = emptyRefer.transform.position;
				_simpleMotion4.StartEurler = emptyRefer.transform.eulerAngles;
				_simpleMotion4.RotateCenter = _simpleMotion4.ParentTrans.position;
				_simpleMotion4.RotateAxis = new Vector3(rotateAxis.z, 0f, 0f);
				_simpleMotion4.RotateAxis = _simpleMotion4.ParentTrans.TransformDirection(_simpleMotion4.RotateAxis);
				_simpleMotion4.RotateSpeed = MotionPara.wrenchSpeed * 4f;
				_simpleMotion4.StandardTime = twistDegree / _simpleMotion4.RotateSpeed;
				emptyRefer.transform.RotateAround(_simpleMotion4.RotateCenter, _simpleMotion4.RotateAxis, twistDegree);
				_simpleMotion4.EndPos = emptyRefer.transform.position;
				_simpleMotion4.EndEurler = emptyRefer.transform.eulerAngles;
				_simpleMotion4.EndTime = _simpleMotion4.StandardTime / _simpleMotion4.SpeedRate;
				tempMotionList.Add(_simpleMotion4);

				//扳手再次进入  New---Class
				BasicMotion _simpleMotion5 = new BasicMotion();
				_simpleMotion5.State = CurrentState.Future;
				_simpleMotion5.CurrentMotion = MotionType.MoveOnly;
				_simpleMotion5.ParentTrans = firstObj;
				_simpleMotion5.ListCopy(childList, parentList);
				_simpleMotion5.StartPos = emptyRefer.transform.position;
				_simpleMotion5.StartEurler = emptyRefer.transform.eulerAngles;
				motionVec = new Vector3(-1f, 0f, 0f) * wrenchBackDis / 3;
				emptyRefer.transform.Translate(motionVec, Space.Self);
				_simpleMotion5.EndPos = emptyRefer.transform.position;
				_simpleMotion5.EndEurler = emptyRefer.transform.eulerAngles;
				_simpleMotion5.StandardTime = motionVec.magnitude / moveSpeed;
				_simpleMotion5.SpeedVector = motionVec / _simpleMotion5.StandardTime;
				_simpleMotion5.EndTime = _simpleMotion5.StandardTime / _simpleMotion5.SpeedRate;
				tempMotionList.Add(_simpleMotion5);
			}
		}
		//最后退出  New---Class
		BasicMotion _simpleMotion6 = new BasicMotion();
		_simpleMotion6.State = CurrentState.Future;
		_simpleMotion6.CurrentMotion = MotionType.MoveOnly;
		_simpleMotion6.StartPos = emptyRefer.transform.position;
		_simpleMotion6.StartEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion6.ParentTrans = firstObj;
		_simpleMotion6.ListCopy(childList, parentList);
		motionVec = -moveVecUnit * disNS;
		emptyRefer.transform.Translate(motionVec, Space.Self);
		_simpleMotion6.EndPos = emptyRefer.transform.position;
		_simpleMotion6.EndEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion6.StandardTime = motionVec.magnitude / moveSpeed;
		_simpleMotion6.SpeedVector = motionVec / _simpleMotion6.StandardTime;
		_simpleMotion6.EndTime = _simpleMotion6.StandardTime / _simpleMotion6.SpeedRate;
		tempMotionList.Add(_simpleMotion6);
		_complexMotionList.Add(tempMotionList);
		emptyRefer.transform.parent = null;
		GameObject.DestroyImmediate(emptyRefer);
		return true;
	}

	//螺钉拧进拧出
	private bool NingChu(DataRow program_row, string id, DataTable group_table, IList<IMotion> _motionList, 
		IList<IList<IMotion>> _complexMotionList){
		Transform firstObj = null;
		Transform firstObjParent = null;
		List<Transform> parentList = new List<Transform>();
		List<Transform> childList = new List<Transform>();
		Transform screwTrans = null;
		Transform screwTransParent = null;
		bool isRight = true;
		//Group Column
		int groupColumn = IntConversion((string)program_row[3].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCGROUP", id) + "，螺钉拧出运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			firstObj = GameObject.Find(objName).transform;
			firstObjParent = firstObj.parent;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				childList.Add(tempTrans);
				parentList.Add(tempTrans.parent);
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCGROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//被拧物体
		try{
			screwTrans = GameObject.Find((string)program_row[4].ToString()).transform;
			screwTransParent = screwTrans.parent;
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCREFER", id) + "，被拧物体填写有错误！");
			}
			return false;
		}
		//扳手到螺钉距离
		Vector3 disNCVec = Vector3Conversion((string)program_row[5].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCDIS", id) + "，拧的距离填写有错误！");
			}
			return false;
		}
		float disNC = disNCVec.magnitude;
		//旋转轴
		Vector3 rotateAxis = Vector3Conversion((string)program_row[6].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCAXIS", id) + "，拧的旋转轴填写有错误！");
			}
			return false;
		}
		//螺钉拧出的距离
		Vector3 disNCLVec = Vector3Conversion((string)program_row[7].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCLDIS", id) + "，拧出来的距离填写有错误！");
			}
			return false;
		}
		float disNCL = disNCLVec.magnitude;
		//移动速度
		float moveSpeed = MotionPara.toolMoveSpeed;
		if(program_row.Table.Columns.Count > 8){
			if((string)program_row[8].ToString() != ""){
				moveSpeed = FloatConversion((string)program_row[8].ToString(), ref isRight);
				if(!isRight){
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("PROGRAM", "NCSPEED", id) + "，自定义速度填写有错误！");
					}
					return false;
				}
			}
		}
		//参考空物体
		GameObject emptyRefer = new GameObject();
		emptyRefer.name = "NingChu_empty-" + id;
		//开始生成类数据
		IList<IMotion> tempMotionList = new List<IMotion>();
		//首次移进  New---Class
		BasicMotion _simpleMotion = new BasicMotion();
		_simpleMotion.State = CurrentState.Active;
		_simpleMotion.CurrentMotion = MotionType.MoveOnly;
		_simpleMotion.StartPos = firstObj.position;
		_simpleMotion.StartEurler = firstObj.eulerAngles;
		emptyRefer.transform.position = firstObj.position;
		emptyRefer.transform.eulerAngles = firstObj.eulerAngles;
		_simpleMotion.ParentTrans = firstObj;
		_simpleMotion.ListCopy(childList, parentList);
			
		//扳手移进时的坐标轴，如果要通用化，请修改这里
		Vector3 moveVecUnit = new Vector3(0f, -1.0f, 0f);
		Vector3 motionVec = moveVecUnit * disNC;
		emptyRefer.transform.Translate(motionVec, Space.Self);
		_simpleMotion.EndPos = emptyRefer.transform.position;
		_simpleMotion.EndEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion.StandardTime = motionVec.magnitude / moveSpeed;
		_simpleMotion.SpeedVector = motionVec / _simpleMotion.StandardTime;
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		tempMotionList.Add(_simpleMotion);

		//拧出  New---Class
		BasicMotion _simpleMotion2 = new BasicMotion();
		_simpleMotion2.State = CurrentState.Future;
		_simpleMotion2.CurrentMotion = MotionType.MoveRotateSingle;
		_simpleMotion2.ParentTrans = screwTrans;
		_simpleMotion2.ListCopy(childList, parentList);
		_simpleMotion2.ChildList.Add(firstObj);
		_simpleMotion2.ParentList.Add(firstObjParent);
		_simpleMotion2.ChildList.Add(screwTrans);
		_simpleMotion2.ParentList.Add(screwTransParent);
		GameObject emptyObj = new GameObject();
		emptyObj.name = "move-rotate_empty-" + id + "-ningchu";
		emptyObj.transform.position = screwTrans.position;
		emptyObj.transform.eulerAngles = screwTrans.eulerAngles;
		//将emptyRefer当做扳手用于后面过程定位
		emptyRefer.transform.parent = emptyObj.transform;
		_simpleMotion2.StartPos = screwTrans.position;
		_simpleMotion2.StartEurler = screwTrans.eulerAngles;
		_simpleMotion2.RotateAxis = rotateAxis;
		float rotateDegree = disNCL / MotionPara.rotateDegreeRate * 360f;
		_simpleMotion2.RotateSpeed = MotionPara.wrenchSpeed * 7;
		_simpleMotion2.StandardTime = rotateDegree / _simpleMotion2.RotateSpeed;
		motionVec = -rotateAxis * disNCL;
		emptyObj.transform.Translate(motionVec, Space.Self);
		motionVec = _simpleMotion2.ParentTrans.TransformDirection(motionVec).normalized * disNCL;
		_simpleMotion2.SpeedVector = motionVec / _simpleMotion2.StandardTime;
		_simpleMotion2.EndPos = emptyObj.transform.position;
		emptyObj.transform.Rotate(rotateAxis * rotateDegree, Space.Self);
		_simpleMotion2.EndEurler = emptyObj.transform.eulerAngles;
		emptyRefer.transform.parent = null;
		emptyObj.transform.position = _simpleMotion2.StartPos;
		emptyObj.transform.eulerAngles = _simpleMotion2.StartEurler;
		_simpleMotion2.EmptyObj = emptyObj;
		_simpleMotion2.EndTime = _simpleMotion2.StandardTime / _simpleMotion2.SpeedRate;
		tempMotionList.Add(_simpleMotion2);
		//最后退出  New---Class
		BasicMotion _simpleMotion6 = new BasicMotion();
		_simpleMotion6.State = CurrentState.Future;
		_simpleMotion6.CurrentMotion = MotionType.MoveOnly;
		_simpleMotion6.StartPos = emptyRefer.transform.position;
		_simpleMotion6.StartEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion6.ParentTrans = firstObj;
		_simpleMotion6.ListCopy(childList, parentList);
		motionVec = -moveVecUnit * disNC;
		emptyRefer.transform.Translate(motionVec, Space.Self);
		_simpleMotion6.EndPos = emptyRefer.transform.position;
		_simpleMotion6.EndEurler = emptyRefer.transform.eulerAngles;
		_simpleMotion6.StandardTime = motionVec.magnitude / moveSpeed;
		_simpleMotion6.SpeedVector = motionVec / _simpleMotion6.StandardTime;
		_simpleMotion6.EndTime = _simpleMotion6.StandardTime / _simpleMotion6.SpeedRate;
		tempMotionList.Add(_simpleMotion6);
		_complexMotionList.Add(tempMotionList);
		emptyRefer.transform.parent = null;
		GameObject.DestroyImmediate(emptyRefer);


		return true;
	}
}