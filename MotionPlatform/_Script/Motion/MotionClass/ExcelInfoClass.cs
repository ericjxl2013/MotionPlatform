/// <summary>
/// <Filename>: ExcelInfoClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.29
/// Version: 1.0
/// Company: Sunnytech
/// Function: Excel表格运动信息处理类
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

//基本运动方式
public enum MotionType {MoveOnly, AccelerateMove, RotateOnly, AccelerateRotate, MoveRotateSingle, MoveRotateEvery, 
	WorldMove, RandomMotion, SetPos, VisualPath}

//基本运动类
public class BasicMotion : IMotion
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

	//运动类型
	public MotionType CurrentMotion = MotionType.MoveOnly;
	//本次移动的物体，用于确定模型结构
	public Transform ParentTrans = null;
	public List<Transform> ChildList = new List<Transform>();
	public List<Transform> ParentList = new List<Transform>();
	//起始信息
	public Vector3 StartPos = new Vector3(0f, 0f, 0f);
	public Vector3 StartEurler = new Vector3(0f, 0f, 0f);
	//终止信息
	public Vector3 EndPos = new Vector3(0f, 0f, 0f);
	public Vector3 EndEurler = new Vector3(0f, 0f, 0f);
	//速度参数
	public float MoveSpeed = 1.0f;
	public Vector3 SpeedVector = new Vector3(0f, 0f, 0f);
	public float RotateSpeed = 1.0f;
	public Vector3 AngleVector = new Vector3(0f, 0f, 0f);
	//旋转中心
	public Vector3 RotateCenter = new Vector3(0f, 0f, 0f);
	public Vector3 RotateAxis = new Vector3(0f, 0f, 0f);
	//移动加速度
	public Vector3 AccelerateVec = new Vector3(0f, 0f, 0f);
	

	public bool IsActive = false;  //组合运动时的当前激活实例

	public CRSpline crSpline = new CRSpline();  //CR样条
	private int currentIndex = 0;  //当前二分法找到的参数序号
	private float viewTimeRate = 0f;  //二分法找到的参数对应的时间
	private float currentTimeRate = 0f;  //记录当前时间进程
	public float startTimeRate = 0f;  //记录变速时的当前时间进程
	private float restTime = 0f;  //剩余时间
	private float postTime = 0f;  //已经过去的时间
	private float lastFrameTime = 0f;  //记录上一帧时间

	public BasicMotion()
	{
		ChildList = new List<Transform>();
		ParentList = new List<Transform>();
	}


	public void ListCopy(List<Transform> child_list, List<Transform> parent_list)
	{
		ChildList.Clear();
		ParentList.Clear();
		for(int i = 0; i < child_list.Count; i++){
			ChildList.Add(child_list[i]);
		}
		for(int i = 0; i < parent_list.Count; i++){
			ParentList.Add(parent_list[i]);
		}
	}

	//空物体
	public GameObject EmptyObj = null;

	public void Init()
	{
		restTime = EndTime;
		//模型结构确定
		if(CurrentMotion == MotionType.MoveRotateSingle || CurrentMotion == MotionType.MoveRotateEvery){  //平移+旋转
			for(int i = 0; i < ChildList.Count; i++){
				ChildList[i].parent = EmptyObj.transform;
			}
		}else{
			for(int i = 0; i < ChildList.Count; i++){
				ChildList[i].parent = ParentTrans;
			}
			if (CurrentMotion == MotionType.VisualPath)
			{
				if (MotionPara.isEditor)  //显示绿线初始化
				{
					ParentTrans.gameObject.AddComponent<VisualUnit>();
					VisualUnit st_VUnit = ParentTrans.gameObject.GetComponent<VisualUnit>();
					st_VUnit.nodes = crSpline.controlPoints;
					st_VUnit.isInitialized = true;
				}
			}
		}
	}

	//暂停控制
	public void Pause(bool play_status)  //接口方法
	{
		
	}

	//速率控制
	public void ChangeRate(float current_rate, float current_time)  //接口方法
	{
		//直接设置位置不需要改变速率
		if(CurrentMotion != MotionType.SetPos){
			//如果为非激活状态
			if(State != CurrentState.Active){
				StartTime = 0;
				EndTime = SpeedRate * EndTime / current_rate;
			}else{
				//激活状态
				startTimeRate = currentTimeRate;
				postTime = current_time;
				EndTime = current_time + (EndTime - current_time) * SpeedRate / current_rate;
				if(CurrentMotion == MotionType.AccelerateMove){
					StartPos = ParentTrans.localPosition;
					SpeedVector = SpeedVector + AccelerateVec * (current_time - StartTime) * SpeedRate;
				}else if(CurrentMotion == MotionType.RandomMotion){
					StartPos = ParentTrans.position;
					StartEurler = ParentTrans.eulerAngles;
				}
				restTime = (EndTime - postTime) / (1f - startTimeRate);
				StartTime = current_time;
			}
		}
	}

	//通用运动驱动函数
	public void Move(float current_time, float speed_rate, float delta_time)  //接口方法
	{
		if (TimeRate(current_time) > 1.001f || current_time > EndTime)
		{
			return;
		}
		else
		{
			lastFrameTime = current_time;
		}
		if(CurrentMotion == MotionType.MoveOnly){  
			//匀速直线运动
			ParentTrans.Translate(SpeedVector * delta_time * speed_rate, Space.Self);
		}else if(CurrentMotion == MotionType.WorldMove){  
			//在世界坐标内移动
			ParentTrans.Translate(SpeedVector * delta_time * speed_rate, Space.World);
		}else if(CurrentMotion == MotionType.AccelerateMove){ 
			//匀加速直线运动 
			ParentTrans.localPosition = StartPos + SpeedVector * (current_time - StartTime) * speed_rate + 
				0.5f * AccelerateVec * ((current_time - StartTime) * speed_rate) * ((current_time - StartTime) * speed_rate);
		}else if(CurrentMotion == MotionType.RotateOnly){  
			//旋转运动
			ParentTrans.RotateAround(RotateCenter, RotateAxis, RotateSpeed * delta_time * speed_rate);
		}else if(CurrentMotion == MotionType.MoveRotateSingle){  
			//平移+旋转，围绕某一个
			if(SpeedVector != Vector3.zero)
				EmptyObj.transform.Translate(SpeedVector * delta_time * SpeedRate, Space.World);
			EmptyObj.transform.Rotate(RotateAxis * RotateSpeed * delta_time * SpeedRate, Space.Self);
		}else if(CurrentMotion == MotionType.MoveRotateEvery){  
			//平移+旋转，围绕各自
			if(SpeedVector != Vector3.zero)
				EmptyObj.transform.Translate(SpeedVector * delta_time * SpeedRate, Space.World);
			for(int i = 0; i < ChildList.Count; i++)
			{
				ChildList[i].Rotate(RotateAxis * RotateSpeed * delta_time * SpeedRate, Space.Self);
			}
		}else if(CurrentMotion == MotionType.RandomMotion){  
			//任意移动
			if(SpeedVector != Vector3.zero)
				ParentTrans.position = StartPos + SpeedVector * (current_time - StartTime) * speed_rate;
			if(AngleVector != Vector3.zero)
				ParentTrans.eulerAngles = StartEurler + AngleVector * (current_time - StartTime) * speed_rate;
		}else if (CurrentMotion == MotionType.VisualPath){
			//可视化路径移动
			ParentTrans.position = crSpline.Interp(TimeRate(current_time));
			ParentTrans.eulerAngles = crSpline.EulerAngleLerp(TimeRate(current_time), ref currentIndex, ref viewTimeRate);
		}
	}

	//变速后的时间比率
	private float TimeRate(float current_time)
	{
		currentTimeRate = startTimeRate + (current_time - postTime) / restTime;
		return currentTimeRate;
	}

	//时间控制
	public bool TimesUp(float current_time)
	{
		if(current_time > EndTime){
			//Debug.Log("Excel: " + currentTimeRate + "---" + (EndTime - lastFrameTime));
			if (EndTime - lastFrameTime > 0)  //最后没运行的时间修正
			{
				//Debug.Log("Excel: " + (EndTime - lastFrameTime));
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
		if(CurrentMotion == MotionType.MoveOnly || CurrentMotion == MotionType.AccelerateMove 
			|| CurrentMotion == MotionType.WorldMove){  //直线运动
			ParentTrans.position = EndPos;
		}else if(CurrentMotion == MotionType.RotateOnly || CurrentMotion == MotionType.RandomMotion ||
			CurrentMotion == MotionType.SetPos){  //旋转运动和任意运动，还有直接设置位置
			ParentTrans.position = EndPos;
			ParentTrans.eulerAngles = EndEurler;
		}else if(CurrentMotion == MotionType.MoveRotateSingle){ 
			if(SpeedVector != Vector3.zero)
				EmptyObj.transform.position = EndPos;
			EmptyObj.transform.eulerAngles = EndEurler;
		}else if(CurrentMotion == MotionType.MoveRotateEvery){  //平移+旋转
			if(SpeedVector != Vector3.zero)
				EmptyObj.transform.position = EndPos;
		}
		else if (CurrentMotion == MotionType.VisualPath) {
			ParentTrans.position = crSpline.controlPoints[crSpline.controlPoints.Count - 1];
			ParentTrans.eulerAngles = crSpline.rotationList[crSpline.rotationList.Count - 1];
			if (MotionPara.isEditor)  //消除绿线初始化
			{
				Component.Destroy(ParentTrans.gameObject.GetComponent<VisualUnit>());
			}
		}
		DetachChildren();
		if(EmptyObj != null)
			GameObject.DestroyImmediate(EmptyObj);
	}

	//最后没运行的时间修正
	private void TimeCorrection(float delta_time, float speed_rate)
	{
		if (CurrentMotion == MotionType.MoveOnly)
		{
			//匀速直线运动
			ParentTrans.Translate(SpeedVector * delta_time * speed_rate, Space.Self);
		}
		else if (CurrentMotion == MotionType.WorldMove)
		{
			//在世界坐标内移动
			ParentTrans.Translate(SpeedVector * delta_time * speed_rate, Space.World);
		}
		else if (CurrentMotion == MotionType.AccelerateMove)
		{
			//匀加速直线运动 
			ParentTrans.localPosition = StartPos + SpeedVector * (EndTime - StartTime) * speed_rate +
				0.5f * AccelerateVec * ((EndTime - StartTime) * speed_rate) * ((EndTime - StartTime) * speed_rate);
		}
		else if (CurrentMotion == MotionType.RotateOnly)
		{
			//旋转运动
			ParentTrans.RotateAround(RotateCenter, RotateAxis, RotateSpeed * delta_time * speed_rate);
		}
		else if (CurrentMotion == MotionType.MoveRotateSingle)
		{
			//平移+旋转，围绕某一个
			if (SpeedVector != Vector3.zero)
				EmptyObj.transform.Translate(SpeedVector * delta_time * SpeedRate, Space.World);
			EmptyObj.transform.Rotate(RotateAxis * RotateSpeed * delta_time * SpeedRate, Space.Self);
		}
		else if (CurrentMotion == MotionType.MoveRotateEvery)
		{
			//平移+旋转，围绕各自
			if (SpeedVector != Vector3.zero)
				EmptyObj.transform.Translate(SpeedVector * delta_time * SpeedRate, Space.World);
			for (int i = 0; i < ChildList.Count; i++)
			{
				ChildList[i].Rotate(RotateAxis * RotateSpeed * delta_time * SpeedRate, Space.Self);
			}
		}
		else if (CurrentMotion == MotionType.RandomMotion)
		{
			//任意移动
			if (SpeedVector != Vector3.zero)
				ParentTrans.position = StartPos + SpeedVector * (EndTime - StartTime) * speed_rate;
			if (AngleVector != Vector3.zero)
				ParentTrans.eulerAngles = StartEurler + AngleVector * (EndTime - StartTime) * speed_rate;
		}
		else if (CurrentMotion == MotionType.VisualPath)
		{
			//可视化路径移动
			ParentTrans.position = crSpline.Interp(TimeRate(EndTime));
			ParentTrans.eulerAngles = crSpline.EulerAngleLerp(TimeRate(EndTime), ref currentIndex, ref viewTimeRate);
		}
	}

	//解除之前建立的父子关系，恢复到初始状态
	private void DetachChildren()
	{
		for(int i = 0; i < ChildList.Count; i++){
			ChildList[i].parent = ParentList[i];
		}
	}
}

//Excel表格动画信息处理类
public class ExcelInfoManager : BaseCompute
{
	private BasicMotion _simpleMotion;
	private List<BasicMotion> _complicatedMotion;

	public ExcelInfoManager()
	{
		_simpleMotion = new BasicMotion();
		_complicatedMotion = new List<BasicMotion>();
	}

	//简单Excel动画处理
	public BasicMotion SimpleMotion(DataRow excel_row, string id, DataTable group_table, ref bool is_right, int motionInt)
	{
		//直线运动
		if(motionInt == 1){
			is_right = SimpleMove(excel_row, id, group_table);
		}else if(motionInt == 2){
			//旋转运动
			is_right = SimpleRotate(excel_row, id, group_table);
		}else if(motionInt == 3){
			//平移+旋转
			is_right = MoveAndRotate(excel_row, id, group_table);
		}else if(motionInt == 4){
			//任意移动
			is_right = RandomMove(excel_row, id, group_table);
		}else if (motionInt == 5){
			//直接设定位置
			is_right = SetPos(excel_row, id, group_table);
		}else if (motionInt == 6){
			//可视化路径移动
			is_right = VisualPathInfo(excel_row, id, group_table);
		}
        
		return _simpleMotion;
	}

	//复杂Excel动画处理
	public List<BasicMotion> ComplicatedMotion(DataRow excel_row, string id, DataTable group_table, ref bool is_right, int motionInt)
	{


		return _complicatedMotion;
	}


	//直线运动
	private bool SimpleMove(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		if(excel_row[4].ToString() == ""){  //匀速直线
			_simpleMotion.CurrentMotion = MotionType.MoveOnly;
		}else{  //匀加速直线运动
			_simpleMotion.CurrentMotion = MotionType.AccelerateMove;
		}
		//Group Column
		int groupColumn = IntConversion(excel_row[2].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);	
				_simpleMotion.ParentList.Add(tempTrans.parent);		
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.ParentTrans.position;
		_simpleMotion.StartEurler = _simpleMotion.ParentTrans.eulerAngles;
		//运动向量提取
		Vector3 motionVec = Vector3Conversion(excel_row[6].ToString(), ref isRight);
		if(!isRight || motionVec.magnitude == 0){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "SPEEDVECTOR", id) + "，移动向量格式填写错误！");
			}
			return false;
		}
		//终止信息记录
		GameObject empty = new GameObject();
		empty.name = "move_empty-" + id;
		empty.transform.position = _simpleMotion.StartPos;
		empty.transform.eulerAngles = _simpleMotion.StartEurler;
		empty.transform.Translate(motionVec, Space.Self);
		_simpleMotion.EndPos = empty.transform.position;
		_simpleMotion.EndEurler = empty.transform.eulerAngles;
		GameObject.DestroyImmediate(empty);
		//速度提取
		_simpleMotion.MoveSpeed = FloatConversion(excel_row[5].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "MOVESPEED", id) + "，移动速度格式填写错误！");
			}
			return false;
		}
		if(_simpleMotion.CurrentMotion == MotionType.AccelerateMove){  //加速直线运动
			float finalSpeed = FloatConversion(excel_row[4].ToString(), ref isRight);
			if(!isRight){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "ACCELERATEMOVE", id) + "，移动加速度格式填写错误！");
				}
				return false;
			}
			_simpleMotion.AccelerateVec = motionVec.normalized;
			_simpleMotion.StandardTime = motionVec.magnitude / (_simpleMotion.MoveSpeed + 
				0.5f * (finalSpeed - _simpleMotion.MoveSpeed));
			_simpleMotion.AccelerateVec = (finalSpeed * _simpleMotion.AccelerateVec - 
				_simpleMotion.MoveSpeed * _simpleMotion.AccelerateVec) / _simpleMotion.StandardTime;
			_simpleMotion.SpeedVector = motionVec.normalized * _simpleMotion.MoveSpeed;
		}else{  //匀速直线运动
			//运动信息获取
			_simpleMotion.StandardTime = motionVec.magnitude / _simpleMotion.MoveSpeed;
			_simpleMotion.SpeedVector = motionVec / _simpleMotion.StandardTime;
		}
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		return true;
	}

	//旋转运动
	private bool SimpleRotate(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		_simpleMotion.CurrentMotion = MotionType.RotateOnly;
		//Group Column
		int groupColumn = IntConversion(excel_row[2].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			_simpleMotion.RotateCenter = _simpleMotion.ParentTrans.position;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);
				_simpleMotion.ParentList.Add(tempTrans.parent);	
				_simpleMotion.RotateCenter = (_simpleMotion.RotateCenter + tempTrans.position) / 2;			
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.ParentTrans.position;
		_simpleMotion.StartEurler = _simpleMotion.ParentTrans.eulerAngles;
		if((string)excel_row[8].ToString()  != ""){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "ACCELERATEROTATE", id) + "，暂不支持匀加速旋转！");
			}
		}
		//旋转轴
		_simpleMotion.RotateAxis = Vector3Conversion(excel_row[7].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "ROTATEAXIS", id) + "，旋转轴格式填写错误！");
			}
			return false;
		}
		//旋转速度
		_simpleMotion.RotateSpeed = FloatConversion(excel_row[9].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "ANGLEVECTOR", id) + "，旋转速度格式填写错误！");
			}
			return false;
		}
		//旋转角度
		float rotateDegree = FloatConversion(excel_row[10].ToString(), ref isRight);
		if (rotateDegree <= 0)
		{
			isRight = false;
			Debug.LogError("旋转角度不能为负数！");
		}
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "DEGREE", id) + "，旋转角度格式填写错误！");
			}
			return false;
		}
		//旋转模式
		string rotateType = (string)excel_row[11].ToString();
		if(rotateType == ""){
			//普通旋转
			_simpleMotion.RotateAxis = _simpleMotion.ParentTrans.TransformDirection(_simpleMotion.RotateAxis);
		}else if(rotateType.ToUpper() == "WORLD"){
			//绕世界坐标轴旋转
			//Do Nothing
		}else{
			//绕某一物体旋转
			Transform tempTrans;
			try{
				tempTrans = GameObject.Find(rotateType).transform;
			}catch{
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "REFER", id) + "，参照物体填写错误！");
				}
				return false;
			}
			_simpleMotion.RotateAxis = tempTrans.TransformDirection(_simpleMotion.RotateAxis);
			_simpleMotion.RotateCenter = tempTrans.position;
		}
		_simpleMotion.StandardTime = rotateDegree / _simpleMotion.RotateSpeed;
		//终止信息记录
		GameObject empty = new GameObject();
		empty.name = "rotate_empty-" + id;
		empty.transform.position = _simpleMotion.StartPos;
		empty.transform.eulerAngles = _simpleMotion.StartEurler;
		empty.transform.RotateAround(_simpleMotion.RotateCenter, _simpleMotion.RotateAxis, rotateDegree);
		_simpleMotion.EndPos = empty.transform.position;
		_simpleMotion.EndEurler = empty.transform.eulerAngles;
		GameObject.DestroyImmediate(empty);
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		return true;
	}

	//平移+旋转运动
	private bool MoveAndRotate(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		if(excel_row[11].ToString() == ""){  //绕同一中心旋转
			_simpleMotion.CurrentMotion = MotionType.MoveRotateSingle;
		}else{  //绕各自中心旋转
			_simpleMotion.CurrentMotion = MotionType.MoveRotateEvery;
		}
		//Group Column
		int groupColumn = IntConversion(excel_row[2].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		GameObject emptyObj = new GameObject();
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			_simpleMotion.ChildList.Add(_simpleMotion.ParentTrans);	
			_simpleMotion.ParentList.Add(_simpleMotion.ParentTrans.parent);	
			emptyObj.name = "move-rotate_empty-" + id;
			emptyObj.transform.position = _simpleMotion.ParentTrans.position;
			emptyObj.transform.eulerAngles = _simpleMotion.ParentTrans.eulerAngles;
			_simpleMotion.EmptyObj = emptyObj;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);	
				_simpleMotion.ParentList.Add(tempTrans.parent);		
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.ParentTrans.position;
		_simpleMotion.StartEurler = _simpleMotion.ParentTrans.eulerAngles;
		//平移和旋转动作判断，当【MoveVector:5】有信息时，平移有动作，否则无；
		//当【RotateDegree:9】有时要考虑两个动作谁快谁慢，以慢的时间为标准，没有旋转就报错
		bool _hasMove = true;
		float moveTime = 0;
		float rotateTime = 0;
		string moveStr = (string)excel_row[6].ToString();
		Vector3 motionVec = new Vector3(0f, 0f, 0f);
		if(moveStr == ""){
			_hasMove = false;
		}else{
			_hasMove = true;
			//运动向量提取
			motionVec = Vector3Conversion(excel_row[6].ToString(), ref isRight);
			if(!isRight || motionVec.magnitude == 0){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "SPEEDVECTOR", id) + "，移动向量格式填写错误！");
				}
				return false;
			}
			//速度提取
			_simpleMotion.MoveSpeed = FloatConversion(excel_row[5].ToString(), ref isRight);
			if(!isRight){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "MOVESPEED", id) + "，移动速度格式填写错误！");
				}
				return false;
			}
			moveTime = motionVec.magnitude / _simpleMotion.MoveSpeed;
		}
		//旋转轴
		_simpleMotion.RotateAxis = Vector3Conversion(excel_row[7].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "ROTATEAXIS", id) + "，旋转轴格式填写错误！");
			}
			return false;
		}
		//旋转速度
		_simpleMotion.RotateSpeed = FloatConversion(excel_row[9].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "ANGLEVECTOR", id) + "，旋转速度格式填写错误！");
			}
			return false;
		}
		string rotateStr = (string)excel_row[10].ToString();
		if(rotateStr == ""){
			if(!_hasMove){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "SPEEDVECTOR", id) + "，移动数据填写不全或者应增加旋转角度！");
				}
				return false;
			}
			//以移动的时间为标准，只修正位置
			_simpleMotion.StandardTime = moveTime;
			_simpleMotion.SpeedVector = motionVec / moveTime;
		}else{
			//旋转角度
			float rotateDegree = FloatConversion(excel_row[10].ToString(), ref isRight);
			if(!isRight){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "DEGREE", id) + "，旋转速度格式填写错误！");
				}
				return false;
			}
			rotateTime = rotateDegree / _simpleMotion.RotateSpeed;
			if(_hasMove){ //对比时间，哪一个长用哪个，修正位置和角度
				if(moveTime > rotateTime){ 
					_simpleMotion.StandardTime = moveTime;
					_simpleMotion.SpeedVector = motionVec / moveTime;
					_simpleMotion.RotateSpeed = rotateDegree / moveTime;

				}else{
					_simpleMotion.StandardTime = rotateTime;
					_simpleMotion.MoveSpeed = motionVec.magnitude / rotateTime;
					_simpleMotion.SpeedVector = motionVec / rotateTime;
				}
			}else{  //以旋转的时间为标准，修正角度
				_simpleMotion.StandardTime = rotateTime;
			}
		}
		//终止信息记录,只修正位置
		//平移改成记录相对位移已经，转换成绝对位置之差
		if(_hasMove){
			emptyObj.transform.Translate(motionVec, Space.Self);
			_simpleMotion.EndPos = emptyObj.transform.position;
			_simpleMotion.SpeedVector = _simpleMotion.ParentTrans.TransformDirection(motionVec).normalized * 
				_simpleMotion.MoveSpeed;
		}
		emptyObj.transform.Rotate(_simpleMotion.RotateSpeed * _simpleMotion.StandardTime * _simpleMotion.RotateAxis, 
			Space.Self);
		_simpleMotion.EndEurler = emptyObj.transform.eulerAngles;
		emptyObj.transform.position = _simpleMotion.StartPos;
		emptyObj.transform.eulerAngles = _simpleMotion.StartEurler;
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		return true;
	}

	//任意移动
	private bool RandomMove(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		_simpleMotion.CurrentMotion = MotionType.RandomMotion;
		//Group Column
		int groupColumn = IntConversion(excel_row[2].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);	
				_simpleMotion.ParentList.Add(tempTrans.parent);		
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.ParentTrans.position;
		_simpleMotion.StartEurler = _simpleMotion.ParentTrans.eulerAngles;
		//移动速度确定，如果有自定义移动速度
		float customMoveSpeed = MotionPara.toolMoveSpeed;
		string speedStr = (string)excel_row[5].ToString();
		if(speedStr != ""){
			customMoveSpeed = FloatConversion(speedStr, ref isRight);
			if(!isRight){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "MOVESPEED", id) + "，移动速度格式填写错误！");
				}
				return false;
			}
		}
		//旋转速度确定，如果有自定义旋转速度
		float customRotateSpeed = MotionPara.toolRotateSpeed;
		string rotateSpeedStr = (string)excel_row[9].ToString();
		if(rotateSpeedStr != ""){
			customRotateSpeed = FloatConversion(rotateSpeedStr, ref isRight);
			if(!isRight){
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "ANGLEVECTOR", id) + "，旋转速度格式填写错误！");
				}
				return false;
			}
		}
		//终止位置获取
		GameObject empty = new GameObject();
		empty.name = "random_move_empty-" + id;
		Transform finalTrans;
		try{
			finalTrans = GameObject.Find((string)excel_row[11].ToString()).transform;
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "REFER", id) + "，参考物体填写错误！");
			}
			return false;
		}
		_simpleMotion.EndPos = Vector3Conversion((string)excel_row[12].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "POSITION", id) + "，参考位置填写错误！");
			}
			return false;
		}
		_simpleMotion.EndEurler = Vector3Conversion((string)excel_row[13].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "EURLER", id) + "，参考角度填写错误！");
			}
			return false;
		}
		empty.transform.parent = finalTrans;
		empty.transform.localPosition = _simpleMotion.EndPos;
		empty.transform.localEulerAngles = _simpleMotion.EndEurler;
		_simpleMotion.EndPos = empty.transform.position;
		_simpleMotion.EndEurler = empty.transform.eulerAngles;
		GameObject.DestroyImmediate(empty);
		//时间计算
		Vector3 motionVec = _simpleMotion.EndPos - _simpleMotion.StartPos;
		Vector3 angleVec = GetAngleDiff(_simpleMotion.StartEurler, ref _simpleMotion.EndEurler);
		float moveTime = 0;
		float rotateTime = 0;
		if(motionVec.magnitude != 0){
			moveTime = motionVec.magnitude / customMoveSpeed;
		}
		if(angleVec.magnitude != 0){
			rotateTime = Vector3.Angle(_simpleMotion.StartEurler, _simpleMotion.EndEurler) / customRotateSpeed;
		}
		if(moveTime == 0 && rotateTime == 0){
			Debug.LogError(ErrorLocation.Locate("EXCEL", "REFER", id) + "，参考物体所计算出的位移和角度都为0，没有移动！");
			return false;
		}
		if(moveTime > rotateTime){
			_simpleMotion.StandardTime = moveTime;
			_simpleMotion.SpeedVector = motionVec / moveTime;
			_simpleMotion.RotateSpeed = Vector3.Angle(_simpleMotion.StartEurler, _simpleMotion.EndEurler) / moveTime;
			_simpleMotion.AngleVector = angleVec / moveTime;
		}else{
			_simpleMotion.StandardTime = rotateTime;
			_simpleMotion.SpeedVector = motionVec / rotateTime;
			_simpleMotion.MoveSpeed = motionVec.magnitude / rotateTime;
			_simpleMotion.AngleVector = angleVec / rotateTime;
		}
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		return true;
	}

	//直接设定位置
	private bool SetPos(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		_simpleMotion.CurrentMotion = MotionType.SetPos;
		//Group Column
		int groupColumn = IntConversion(excel_row[2].ToString(), ref isRight) + 1;
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{			
				objName = (string)group_table.Rows[i][groupColumn].ToString();			
				if(objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);	
				_simpleMotion.ParentList.Add(tempTrans.parent);		
			}
		}catch{
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.ParentTrans.position;
		_simpleMotion.StartEurler = _simpleMotion.ParentTrans.eulerAngles;
		//终止位置获取
		_simpleMotion.EndPos = Vector3Conversion((string)excel_row[12].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "POSITION", id) + "，参考位置填写错误！");
			}
			return false;
		}
		_simpleMotion.EndEurler = Vector3Conversion((string)excel_row[13].ToString(), ref isRight);
		if(!isRight){
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("EXCEL", "EURLER", id) + "，参考角度填写错误！");
			}
			return false;
		}
		//参考物体
		string refStr = ((string)excel_row[11].ToString()).ToUpper();
		if(refStr != "" && refStr != "WORLD"){
			GameObject empty = new GameObject();
			empty.name = "random_move_empty-" + id;
			Transform finalTrans;
			try{
				finalTrans = GameObject.Find((string)excel_row[11].ToString()).transform;
			}catch{
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("EXCEL", "REFER", id) + "，参考物体填写错误！");
				}
				return false;
			}
			empty.transform.parent = finalTrans;
			empty.transform.localPosition = _simpleMotion.EndPos;
			empty.transform.localEulerAngles = _simpleMotion.EndEurler;
			_simpleMotion.EndPos = empty.transform.position;
			_simpleMotion.EndEurler = empty.transform.eulerAngles;
			GameObject.DestroyImmediate(empty);
		}
		_simpleMotion.StandardTime = 0.05f;
		_simpleMotion.EndTime = 0.05f;
		return true;
	}

	//直接设定位置
	private bool VisualPathInfo(DataRow excel_row, string id, DataTable group_table)
	{
		bool isRight = true;
		//运动类型
		_simpleMotion.CurrentMotion = MotionType.VisualPath;
		//Group Column
		int groupColumn = IntConversion((string)excel_row[2].ToString(), ref isRight) + 1;
		if (!isRight)
		{
			if (MotionPara.isEditor)
			{
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，运动Group填写错误！");
			}
			return false;
		}
		//运动物体信息
		string objName = "";
		try
		{
			objName = (string)group_table.Rows[0][groupColumn].ToString();
			_simpleMotion.ParentTrans = GameObject.Find(objName).transform;
			for (int i = 1; i < group_table.Rows.Count; i++)
			{
				objName = (string)group_table.Rows[i][groupColumn].ToString();
				if (objName == "")
					break;
				Transform tempTrans = GameObject.Find(objName).transform;
				_simpleMotion.ChildList.Add(tempTrans);
				_simpleMotion.ParentList.Add(tempTrans.parent);
			}
		}
		catch
		{
			if (MotionPara.isEditor)
			{
				Debug.LogError(ErrorLocation.Locate("EXCEL", "GROUP", id) + "，Group号对应的Group表填写有错误！");
			}
			return false;
		}
		//移动速度确定，如果有自定义移动速度
		float customMoveSpeed = MotionPara.toolMoveSpeed;
		string speedStr = (string)excel_row[5].ToString();
		if (speedStr != "")
		{
			customMoveSpeed = FloatConversion(speedStr, ref isRight);
			if (!isRight)
			{
				if (MotionPara.isEditor)
				{
					Debug.LogError(ErrorLocation.Locate("EXCEL", "MOVESPEED", id) + "，移动速度格式填写错误！");
				}
				return false;
			}
		}
		//路径信息加载
		string pathName = (string)excel_row[4].ToString();
		JsonLoad(pathName, _simpleMotion.crSpline, ref isRight, _simpleMotion.ParentTrans);
		if (!isRight)
		{
			if (MotionPara.isEditor)
			{
				Debug.LogError(ErrorLocation.Locate("EXCEL", "PATHNAME", id) + "，可视化路径名称填写错误！");
			}
			return false;
		}
		//起始信息记录
		_simpleMotion.StartPos = _simpleMotion.crSpline.controlPoints[0];
		_simpleMotion.StartEurler = _simpleMotion.crSpline.rotationList[0];
		//终止位置获取
		string endPosStr = (string)excel_row[12].ToString();
		if(endPosStr == "")
		{
			_simpleMotion.EndPos = _simpleMotion.crSpline.controlPoints[_simpleMotion.crSpline.controlPoints.Count - 1];
		}
		else
		{
			_simpleMotion.EndPos = Vector3Conversion(endPosStr, ref isRight);
			if (!isRight)
			{
				if (MotionPara.isEditor)
				{
					Debug.LogError(ErrorLocation.Locate("EXCEL", "POSITION", id) + "，参考位置填写错误！");
				}
				return false;
			}
		}
		//终止角度获取
		string endEulerStr = (string)excel_row[13].ToString();
		if (endEulerStr == "")
		{
			_simpleMotion.EndEurler = _simpleMotion.crSpline.rotationList[_simpleMotion.crSpline.rotationList.Count - 1];
		}
		else
		{
			_simpleMotion.EndEurler = Vector3Conversion(endEulerStr, ref isRight);
			if (!isRight)
			{
				if (MotionPara.isEditor)
				{
					Debug.LogError(ErrorLocation.Locate("EXCEL", "EURLER", id) + "，参考角度填写错误！");
				}
				return false;
			}
		}
		//参考物体
		string refStr = ((string)excel_row[11].ToString()).ToUpper();
		if (refStr != "" && refStr != "WORLD" && (endEulerStr != "" || endPosStr != ""))
		{
			GameObject empty = new GameObject();
			empty.name = "random_move_empty-" + id;
			Transform finalTrans;
			try
			{
				finalTrans = GameObject.Find((string)excel_row[11].ToString()).transform;
			}
			catch
			{
				if (MotionPara.isEditor)
				{
					Debug.LogError(ErrorLocation.Locate("EXCEL", "REFER", id) + "，参考物体填写错误！");
				}
				return false;
			}
			empty.transform.parent = finalTrans;
			empty.transform.localPosition = _simpleMotion.EndPos;
			empty.transform.localEulerAngles = _simpleMotion.EndEurler;
			if (endPosStr != "")
			{
				_simpleMotion.EndPos = empty.transform.position;
				_simpleMotion.crSpline.controlPoints[_simpleMotion.crSpline.controlPoints.Count - 1] = empty.transform.position;
			}
			if (endEulerStr != "") 
			{
				_simpleMotion.EndEurler = empty.transform.eulerAngles;
				_simpleMotion.crSpline.rotationList[_simpleMotion.crSpline.rotationList.Count - 1] = empty.transform.eulerAngles;
			}
			GameObject.DestroyImmediate(empty);
		}
			AngleOptimization(_simpleMotion.crSpline.rotationList);
		_simpleMotion.crSpline.pts = _simpleMotion.crSpline.PathControlPointGenerator(_simpleMotion.crSpline.controlPoints.ToArray());
		_simpleMotion.StandardTime = _simpleMotion.crSpline.TimeGet(_simpleMotion.crSpline.pts, customMoveSpeed);
		_simpleMotion.crSpline.TimeRateGenerator(_simpleMotion.crSpline.rotationList.Count);
		_simpleMotion.EndTime = _simpleMotion.StandardTime / _simpleMotion.SpeedRate;
		return true;
	}

	//Json文件加载
	private void JsonLoad(string path_name, CRSpline crSpline, ref bool is_right, Transform parent_trans)
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
			loadEmpty.transform.parent = parent_trans;
			for (int i = 0; i < jsonTable.Rows.Count; i++)
			{
				loadEmpty.transform.localPosition = ConvertToVector3((string)jsonTable.Rows[i][0].ToString());
				loadEmpty.transform.localEulerAngles = ConvertToVector3((string)jsonTable.Rows[i][1].ToString());
				crSpline.controlPoints.Add(loadEmpty.transform.position);
				crSpline.rotationList.Add(loadEmpty.transform.eulerAngles);
				crSpline.cameraViewList.Add(float.Parse((string)jsonTable.Rows[i][2].ToString()));
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
