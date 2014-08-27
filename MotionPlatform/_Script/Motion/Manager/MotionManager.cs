/// <summary>
/// <Filename>: MotionManager.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.17
/// Version: 1.0
/// Company: Sunnytech
/// Function: 运动模块控制脚本
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text;

public class MotionManager : MonoBehaviour {

	//脚本变量
	InterfaceManager st_Interface;
	LocationManager st_Location;
	TriggerManager st_Trigger;

	//当前项目名称
	public string taskName = "3_33";

	//是否启动编辑模式
	public bool isEditor = true;

	//是否加载时间信息
	public bool isTimeLoad = true;

	//是否生成Loaction表
	public bool createPosition = false;


	//ID表名字
	[HideInInspector]
	public List<string> idList = new List<string>();  
	//当前总ID表名
	[HideInInspector]
	public string IDTableName = "";
	//当前表格子路径
	[HideInInspector]
	public string subTablePath = "/C/";
	//发生错误是否要停止在当前行
	private bool stopInRow = true;
	//表号和行号
	[HideInInspector]
	public string tableNum = "1";
	[HideInInspector]
	public string rowNum = "2";
	//摄像机信息存储
	private CameraMotion cameraAdministrator = new CameraMotion();
	//Tips信息存储
	private TipsMotion tipsAdministrator = new TipsMotion();
	//运动控制类
	private MotionClass motionAdministrator = new MotionClass();

	//运动时间记录
	private float startTime = 0;
	//单步播放控制
	private bool isSinglePlay = false;
	//摄像机播放控制参数
	private bool cameraFlag = false;
	//综合运动播放控制参数
	private bool generMotionFlag = false;

	//进度条参数
	private Rect sliderRect = new Rect(25, 150, 250, 10);
	[HideInInspector]
	public float hSliderValue = 0f;
	private bool timeDisplayToggle = false;
	private bool isTimeTableLoad = false;
	private string timeDisplayStr = "不加载时间表";
	private string currentNode = "输入当前节点号";
	private string setNode = "输入设置节点号";
	[HideInInspector]
	public bool dragActive = false;  //拖动
	[HideInInspector]
	public bool dragLock = false;  //拖动锁住
	
	[HideInInspector]
	public float TotalValue = 100.0f;
	private float totalTimeS = 0f;  //是否加载时间信息
	private float perTime = 0f;  //进度条与总时间的比例
	[HideInInspector]
	public List<float> nodeTimeList = new List<float>();  //节点表时间记录
	private float currentTime = 0f;  //当前行的时间，进度条修正
	[HideInInspector]
	public float preProcess = 0f;  //按下鼠标前的进度

	//TEST PARAMETER
	//private Rect testRect = new Rect(50, 50, 360, 350);
	private bool isTeaching = true;
	private string teaBtnStr = "教";
    
    //测试: 计算时间
	public bool computeTime = false;
	private float cameraTime = 0f;
	private float tipsTime = 0f;
	private float motionsTime = 0f;
	private float addMotionTime = 0f;
	private int maxNumber = 0;
	private float cTime = 0f;
	

	void Awake ()
	{
		//编辑模式
		MotionPara.isEditor = isEditor;
		if(MotionPara.isEditor){
			ErrorLocation.Initialize();  //初始化表格填写错误报警信息
		}

		cameraAdministrator.State = CurrentState.Old;
        InterData.InterDataInit();

		//任务初始化
		MotionPara.taskName = taskName;
		TaskInitialize(MotionPara.taskName);
		//Debug.Log("Motion SpeedRate: "  + MotionPara.SpeedRate);
		//MotionPara.SpeedRate = 0.5f;

		//脚本变量初始化
		st_Interface = gameObject.GetComponent<InterfaceManager>();
		gameObject.AddComponent<LocationManager>();
		st_Location = gameObject.GetComponent<LocationManager>();
        GameObject triggerEmpty = new GameObject();
        triggerEmpty.name = "TriggerObject";
        triggerEmpty.AddComponent<TriggerManager>();
		st_Trigger = (TriggerManager)GameObject.FindObjectOfType(typeof(TriggerManager));
		st_Trigger.gameObject.SetActive(false);
		if (isEditor)
		{
			gameObject.AddComponent<EditorManager>();
		}

	}

	// Use this for initialization
	void Start () 
	{
		
	}


	void OnGUI ()
	{
		//testRect = GUI.Window(110, testRect, TestWindow, "");
		// Event guiEvent = Event.current;
		// Debug.Log(guiEvent.type.ToString());
	}


	void TestWindow(int WindowID){
		//播放
		if(GUI.Button(new Rect(10, 20, 50, 50), "Play")){
			if(!MotionPara.MotionActive){
				MotionPara.shouldStop = false;
				StartCoroutine(MainEntrance(0, 0));
			}else{
                if (isSinglePlay)
                {
                    MotionPara.singlePlay = false;
                    isSinglePlay = false;
                }
			}
		}
		//暂停
		if(GUI.Button(new Rect(70, 20, 50, 50), "Pause")){
			motionAdministrator.Pause(!MotionPara.PauseControl);
		}
		//减速
		if(GUI.Button(new Rect(130, 20, 50, 50), "Down")){
			SpeedControl(false);
		}
		//加速
		if(GUI.Button(new Rect(190, 20, 50, 50), "Up")){
			SpeedControl(true);
		}
		//单步播放
		if(GUI.Button(new Rect(250, 20, 50, 50), "Single")){
            isSinglePlay = true;
            MotionPara.singlePlay = false;
        }
		if(MotionPara.PauseControl){
			GUI.Box(new Rect(30, 80, 100, 50), "暂停中");
		}else{
			GUI.Box(new Rect(30, 80, 100, 50), "非暂停");
		}

		//教和练切换
		if (GUI.Button(new Rect(140, 90, 35, 35), teaBtnStr))
		{
			if (isTeaching)
			{
				teaBtnStr = "练";
				isTeaching = false;
				FuncPara.currentMotion = MotionState.Exercising;
			}
			else 
			{
				teaBtnStr = "教";
				isTeaching = true;
				FuncPara.currentMotion = MotionState.Teaching;
			}
		}

		//测试按钮1
		if (GUI.Button(new Rect(310, 60, 50, 50), "Test01") && BtnFunction.Allow("TEST01"))
		{
			TriggerManager.GUIButtonEvent("TEST01");
		}

		//测试按钮2
		if (GUI.Button(new Rect(310, 120, 50, 50), "Test02") && BtnFunction.Allow("TEST02"))
		{
			TriggerManager.GUIButtonEvent("TEST02");
		}

		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = 13;

		GUI.Label(new Rect(190, 80, 50, 50), "当前速率: " + MotionPara.SpeedRate.ToString());

		hSliderValue = GUI.HorizontalSlider(sliderRect, hSliderValue, 0.0f, TotalValue);
		//获得鼠标按下
		if (sliderRect.Contains(Event.current.mousePosition))
		{
			if (Input.GetMouseButtonDown(0) && !dragLock && !dragActive) 
			{
				dragActive = true;
				preProcess = hSliderValue;
			}
		}

		GUI.contentColor = Color.green;
		timeDisplayToggle = GUI.Toggle(new Rect(50, 170, 100, 20), timeDisplayToggle, timeDisplayStr);
		if(timeDisplayToggle)
		{
			timeDisplayStr = "加载时间表";
			if(!isTimeTableLoad)
			{
				// if(MotionControlEngine_Script.LoadTimeTable())
					isTimeTableLoad = true;
			}
		}
		else
			timeDisplayStr = "不加载时间表";
		GUI.contentColor = Color.white;

		GUI.Label(new Rect(10, 190, 200, 20), "Location表格生成:");
		GUI.Label(new Rect(10, 210, 80, 20),"当前节点号:");
		currentNode = GUI.TextField (new Rect (90, 210, 100, 20), currentNode);
		if(GUI.Button(new Rect(200, 210, 90, 20), "生成Location表"))
		{
			st_Location.LocationCreate(currentNode, "Test", false);
		}

		GUI.Label(new Rect(10, 240, 200, 20), "设置ID表初始状态:");
		GUI.Label(new Rect(10, 260, 80, 20),"当设置节点号:");
		setNode = GUI.TextField (new Rect (90, 260, 100, 20), setNode);
		if(GUI.Button(new Rect(200, 260, 90, 20), "设置"))
		{
			st_Location.LocationSet(setNode, "0");
		}

		GUI.DragWindow();
	}

	//播放按钮
	public void PlayButton()
	{
		if (!MotionPara.MotionActive)  //从头开始播放
		{
			MotionPara.PauseControl = false;
			MotionPara.shouldStop = false;
			StartCoroutine(MainEntrance(0, 0));
		}
		else
		{
			if (isSinglePlay)  //正在单步播放
			{
				MotionPara.singlePlay = false;
				isSinglePlay = false;
			}
		}
	}

	//暂停按钮
	public void PauseButton()
	{
		motionAdministrator.Pause(!MotionPara.PauseControl);
	}

	//播放速度控制按钮
	public void SpeedControl(bool speed_flag)
	{
		//加速
		if(speed_flag){
			if(MotionPara.SpeedRate >= 0.99f && MotionPara.SpeedRate < 3.9f){
				float setRate = (float)Math.Round(MotionPara.SpeedRate + 0.5f, 1);
				motionAdministrator.ChangeRate(setRate, startTime);
				if(cameraAdministrator.State == CurrentState.Active)
					cameraAdministrator.ChangeRate(setRate, startTime);
			}else if(MotionPara.SpeedRate >= 0.49f && MotionPara.SpeedRate < 1.0f){
				float setRate = (float)Math.Round(MotionPara.SpeedRate + 0.1f, 1);
				motionAdministrator.ChangeRate(setRate, startTime);
				if(cameraAdministrator.State == CurrentState.Active)
					cameraAdministrator.ChangeRate(setRate, startTime);
			}
		}else{  //减速
			if(MotionPara.SpeedRate > 1.01f){
				float setRate = (float)Math.Round(MotionPara.SpeedRate - 0.5f, 1);
				motionAdministrator.ChangeRate(setRate, startTime);
				if(cameraAdministrator.State == CurrentState.Active)
					cameraAdministrator.ChangeRate(setRate, startTime);
			}else if(MotionPara.SpeedRate <= 1.01f && MotionPara.SpeedRate > 0.51f){
				float setRate = (float)Math.Round(MotionPara.SpeedRate - 0.1f, 1);
				motionAdministrator.ChangeRate(setRate, startTime);
				if(cameraAdministrator.State == CurrentState.Active)
					cameraAdministrator.ChangeRate(setRate, startTime);
			}
		}
	}

	//设置播放速率
	public void ChangeRate(float set_rate)
	{
		motionAdministrator.ChangeRate(set_rate, startTime);
		if (cameraAdministrator.State == CurrentState.Active)
			cameraAdministrator.ChangeRate(set_rate, startTime);
	}

	//单步播放按钮
	public void SinglePlayButton()
	{
		isSinglePlay = true;
		MotionPara.singlePlay = false;
	}

	//播放过程停止
	public void StopButton()
	{
		//Camera
		cameraFlag = false;
		MotionPara.MotionActive = false;
		cameraAdministrator.State = CurrentState.Old;
		if (cameraFlag) {
			cameraAdministrator.PostProcess();
		}
		//Trigger
		MotionPara.triggerPlay = false;
		st_Trigger.gameObject.SetActive(false);
		BtnFunction.AllForbit();
		//Tips
		st_Interface.TipsWindow(false, "", false);
		st_Interface.Voice("");
		//General
		if (generMotionFlag) {
			motionAdministrator.PostProcess();
		}
		generMotionFlag = false;
		motionAdministrator.Clear();

		StopAllCoroutines();
		MotionPara.PauseControl = false;
		//设定要初始状态
		SetLocation(1, "2");
	}

	//场景运动物体位置设定
	public void SetLocation(int node_no, string column_name)
	{
		string excelName = "";
		if (FuncPara.curentMode == Movement.Chai)
		{
			excelName = "C" + node_no;
		}
		else if (FuncPara.curentMode == Movement.Zhuang)
		{
			excelName = "Z" + node_no;
		}
		else
		{
			excelName = "Y" + node_no;
		}
		st_Location.LocationSet(excelName, column_name);
	}

	//当前任务初始化
	public void TaskInitialize(string task_name)
	{
		MotionPara.taskName = task_name;
		MotionPara.taskRootPath = Application.dataPath + "/Resources/";
		//当前运动方式和状态
		FuncPara.currentMotion = MotionState.Teaching;  //当前的运动状态---教
		FuncPara.curentMode = Movement.Chai;  //当前运动方式---拆卸
		//任务初始化
		ParaInitialize();
		//状态初始化
	}

	//当前任务参数初始化
	public bool ParaInitialize()
	{
		string idPath = MotionPara.taskRootPath + MotionPara.taskName + "/";
		if(FuncPara.curentMode == Movement.Chai){
			idPath += "C/CID.xls";
			MotionPara.dataRootPath = MotionPara.taskRootPath + MotionPara.taskName + "/C/";
			IDTableName = "CID";
			subTablePath = "/C/";
		}else if(FuncPara.curentMode == Movement.Zhuang){
			idPath += "Z/ZID.xls";
			MotionPara.dataRootPath = MotionPara.taskRootPath + MotionPara.taskName + "/Z/";
			IDTableName = "ZID";
			subTablePath = "/Z/";
		}else{
			idPath += "Y/YID.xls";
			MotionPara.dataRootPath = MotionPara.taskRootPath + MotionPara.taskName + "/Y/";
			IDTableName = "YID";
			subTablePath = "/Y/";
		}
		ExcelOperator excelReader = new ExcelOperator();
		DataTable idTable = excelReader.ExcelReader(idPath, "ID", "A", "C");
		if(idTable.Rows.Count == 0){
			Debug.LogError(MotionPara.taskName + " Project's CID sheet is null. Please check it! ");
			return false;
		}
		MotionPara.phaseNameList.Clear();
		idList.Clear();
		nodeTimeList.Clear();
		totalTimeS = 0f;
		for(int i = 0; i < idTable.Rows.Count; i++){
			idList.Add((string)idTable.Rows[i][0]);
			MotionPara.phaseNameList.Add((string)idTable.Rows[i][1].ToString());
			if (isTimeLoad)
			{
				nodeTimeList.Add(float.Parse((string)idTable.Rows[i][2].ToString()));
				totalTimeS += nodeTimeList[i];
			}
			//Debug.Log(idList[i] + "---" + MotionPara.phaseNameList[i]);
		}
		//加载进度条总时间
		if (isTimeLoad) 
		{
			perTime = TotalValue / totalTimeS;
		}
		//运动通用参数初始化
		if(!File.Exists(MotionPara.taskRootPath + MotionPara.taskName + "/ToolsVariable.xls")){
			Debug.LogError(MotionPara.taskRootPath + MotionPara.taskName + "/ToolsVariable.xls，该表格不存在，请检查！");
			return false;
		}
		DataTable toolVariable = excelReader.ExcelReader(MotionPara.taskRootPath + MotionPara.taskName + 
														 "/ToolsVariable.xls", "Tools", "A", "B");
		try{
			MotionPara.safeHeight = float.Parse((string)toolVariable.Rows[0][1].ToString());
			MotionPara.toolMoveSpeed = float.Parse((string)toolVariable.Rows[1][1].ToString()); 
			MotionPara.toolRotateSpeed = float.Parse((string)toolVariable.Rows[2][1].ToString()); 
			MotionPara.copperName = (string)toolVariable.Rows[3][1].ToString(); 
			MotionPara.copperHitSpeed = float.Parse((string)toolVariable.Rows[4][1].ToString()); 
			MotionPara.copperForwardDis = float.Parse((string)toolVariable.Rows[5][1].ToString()); 
			string[] copperVectorArray = ((string)toolVariable.Rows[6][1].ToString()).Split(',', '，');
			MotionPara.copperVector.x = float.Parse(copperVectorArray[0]);
			MotionPara.copperVector.y = float.Parse(copperVectorArray[1]);
			MotionPara.copperVector.z = float.Parse(copperVectorArray[2]);
			MotionPara.wrenchBackDis = float.Parse((string)toolVariable.Rows[7][1].ToString());
			MotionPara.wrenchSpeed = float.Parse((string)toolVariable.Rows[8][1].ToString());
			MotionPara.screwBackRate = float.Parse((string)toolVariable.Rows[9][1].ToString());
			MotionPara.rotateDegreeRate = float.Parse((string)toolVariable.Rows[10][1].ToString());
			MotionPara.cameraLineSpeed = float.Parse((string)toolVariable.Rows[11][1].ToString());
			MotionPara.cameraAngleSpeed = float.Parse((string)toolVariable.Rows[12][1].ToString());
		}catch{
			Debug.LogError(MotionPara.taskRootPath + MotionPara.taskName + "/ToolsVariable.xls表中参数填写错误！");
			return false;
		}
				
		//Program运动相关ID初始化
		DataTable programIDTable = excelReader.ExcelReader(Application.streamingAssetsPath + "/ExcelMotionData" +
			"/ProgramMotionID.xls", "ID", "A", "B");
		for(int i = 0; i < programIDTable.Rows.Count; i++){
			MotionPara.programMotionID.Add(((string)programIDTable.Rows[i][1].ToString()).ToUpper());
		}

		//Tools参数初始化
		MotionPara.toolsName.Clear();
		MotionPara.toolsInitPos.Clear();
		MotionPara.toolsInitEuler.Clear();
		if(File.Exists(MotionPara.taskRootPath + MotionPara.taskName + "/Tools.xls")){
			DataTable toolInitTable = excelReader.ExcelReader(MotionPara.taskRootPath + MotionPara.taskName + 
				"/Tools.xls", "Tools", "A", "C");
			for(int i = 0; i < toolInitTable.Rows.Count; i++){
				string tempName = (string)toolInitTable.Rows[i][0].ToString();
				if(tempName == ""){
					Debug.LogError(MotionPara.taskRootPath + MotionPara.taskName + 
						"/Tools.xls表中（" + (i + 2) + ", A）名字为空！");
				}
				MotionPara.toolsName.Add(tempName);
				string tempPos = (string)toolInitTable.Rows[i][1].ToString();
				string[] posArray = tempPos.Split(',','，');
				Vector3 tempPosVec = new Vector3(0f, 0f, 0f);
				try{
					tempPosVec.x = float.Parse(posArray[0]);
					tempPosVec.y = float.Parse(posArray[1]);
					tempPosVec.z = float.Parse(posArray[2]);
				}catch{
					Debug.LogError(MotionPara.taskRootPath + MotionPara.taskName + 
						"/Tools.xls表中（" + (i + 2) + ", B）位置信息错误！");
				}
				MotionPara.toolsInitPos.Add(tempPosVec);
				string tempEuler = (string)toolInitTable.Rows[i][2].ToString();
				string[] eulerArray = tempEuler.Split(',','，');
				Vector3 tempEulerVec = new Vector3(0f, 0f, 0f);
				try{
					tempEulerVec.x = float.Parse(eulerArray[0]);
					tempEulerVec.y = float.Parse(eulerArray[1]);
					tempEulerVec.z = float.Parse(eulerArray[2]);
				}catch{
					Debug.LogError(MotionPara.taskRootPath + MotionPara.taskName + 
						"/Tools.xls表中（" + (i + 2) + ", C）角度信息错误！");
				}
				MotionPara.toolsInitEuler.Add(tempEulerVec);
			}
		}

		//按钮和功能控制信息读取
		BtnFunction.Initialize();

		return true;
	}

	//当前任务状态初始化
	public bool StateInitialize(Movement current_mode)
	{


		return true;
	}

	//平台运动主入口函数
	public IEnumerator MainEntrance(int id_number, int row_number)
	{
		//ID号准确性检查
		if(id_number >= idList.Count || id_number < 0){
			Debug.LogError("输入的ID号超出范围, 请检查输入的ID号或者相应的ID表.");
			yield break;
		}
		//根据ID加载功能表格
		ExcelOperator excelReader = new ExcelOperator();
		string[] sheetArray = new string[] {"MAIN", "CAMERA", "TIPS", "EXCEL", "Group", "3DMAX", "TRIGGER", "PROGRAM"};
        //3DMAX信息读取
        maxNumber = 0;
		bool readedMax = false;
		//ID表遍历
		for(int i = id_number; i < idList.Count; i++){
            cTime = 0f;
			addMotionTime = 0f;
			tableNum = (i + 1).ToString();
			MotionPara.excelName = idList[i];
			DataSet motionDataSet = excelReader.ExcelReader(MotionPara.dataRootPath + MotionPara.excelName + ".xls", sheetArray);
			//检查功能表格正确性
			if(motionDataSet.Tables.Count != 0){
				//总流程表遍历
				//设置表格主键
				motionDataSet.Tables["[MAIN$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[MAIN$]"].Columns["ID"]};
				motionDataSet.Tables["[CAMERA$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[CAMERA$]"].Columns["ID"]};
				motionDataSet.Tables["[TIPS$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[TIPS$]"].Columns["ID"]};
				motionDataSet.Tables["[EXCEL$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[EXCEL$]"].Columns["ID"]};
				motionDataSet.Tables["[3DMAX$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[3DMAX$]"].Columns["ID"]};
				motionDataSet.Tables["[TRIGGER$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[TRIGGER$]"].Columns["ID"]};
				motionDataSet.Tables["[PROGRAM$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[PROGRAM$]"].Columns["ID"]};
				motionDataSet.Tables["[Group$]"].PrimaryKey = new DataColumn[] {motionDataSet.Tables["[Group$]"].Columns["ID"]};
				//修正时间设定
				if (isTimeLoad){
					if (i == 0){
						currentTime = 0f;
					}else{
						currentTime = nodeTimeList[i - 1];
					}
				}
				for(int j = row_number; j < motionDataSet.Tables["[MAIN$]"].Rows.Count; j++){
					rowNum = (j + 2).ToString();
					//自动生成Location等状态信息
					if (createPosition){
						if (j == 0){
							st_Location.LocationCreate(MotionPara.excelName, (j + 2).ToString(), false);
						}else{
							st_Location.LocationCreate(MotionPara.excelName, (j + 2).ToString(), false);
						}
					}
					MotionPara.mainRowNumber = j;
					//检查到有错停止运行
					MotionPara.shouldStop = false;
					//检查到有空行停止运行
					bool isRowBlank = false;
					//先解决CAMERA问题，考过程不触发
					string cameStr = (string)motionDataSet.Tables["[MAIN$]"].Rows[j][2].ToString();
					if(cameStr != ""){
						isRowBlank = isRowBlank || true;
						st_Interface.TipsWindow(false, "", false);
						yield return StartCoroutine(CameraCoroutine(cameStr, motionDataSet.Tables["[CAMERA$]"]));
					}else{
						isRowBlank = isRowBlank || false;
					}

					//再解决TIPS问题，教触发，练触发可选，考不触发
					string tipsStr = (string)motionDataSet.Tables["[MAIN$]"].Rows[j][3].ToString();
					if(tipsStr != ""){
						isRowBlank = isRowBlank || true;
						yield return StartCoroutine(TipsCoroutine(tipsStr, motionDataSet.Tables["[TIPS$]"]));
					}else{
						st_Interface.TipsWindow(false, "", false);
						isRowBlank = isRowBlank || false;
					}

					//如果是练过程，等待触发完成再继续往下运动,Trigger，TODO：触发的是程序内部的某个动作而不是表格动作
					if(FuncPara.currentMotion == MotionState.Exercising){
						string triggerStr = (string)motionDataSet.Tables["[MAIN$]"].Rows[j][5].ToString();
						if(triggerStr != ""){
							isRowBlank = isRowBlank || true;
							//练触发
                            yield return StartCoroutine(TriggerCoroutine(triggerStr, motionDataSet.Tables["[TRIGGER$]"]));
						}else{
							isRowBlank = isRowBlank || false;
						}
					}

					//Excel、3DMax、Tips、Camera、Program动画同时运动
					string motionStr = (string)motionDataSet.Tables["[MAIN$]"].Rows[j][4].ToString();
					motionAdministrator.Clear();
					if(motionStr != ""){
						isRowBlank = isRowBlank || true;
						yield return StartCoroutine(GeneralCoroutine(motionStr, motionDataSet.Tables["[CAMERA$]"], 
							motionDataSet.Tables["[EXCEL$]"], motionDataSet.Tables["[3DMAX$]"], 
							motionDataSet.Tables["[Group$]"], motionDataSet.Tables["[TIPS$]"],
							motionDataSet.Tables["[PROGRAM$]"]));
					}else{
						isRowBlank = isRowBlank || false;
					}

					if(!isRowBlank){
						Debug.LogError(MotionPara.dataRootPath + MotionPara.excelName + 
							".xls" + "表格，[MAIN]Sheet，第" + (MotionPara.mainRowNumber + 2) + "行为空！");
						yield break;
					}

					if(MotionPara.shouldStop){
						yield break;
					}
                    
                    if(computeTime){
						float totalTime = (cameraTime+ tipsTime+ motionsTime);
						float tmp_addMotionTime = addMotionTime;
						addMotionTime += totalTime;
						//Debug.Log("_ID: " + motionDataSet.Tables["[MAIN$]"].Rows[j][1] + ";" + addMotionTime);
						//更新数据库CN中sheet MAIN，记录CN每条运动的时间
						ExcelOperator ewo = new ExcelOperator();
						string[] contents = new string[motionDataSet.Tables["[MAIN$]"].Columns.Count];
						for (int k = 0; k < contents.Length; k++){
							contents[k] = motionDataSet.Tables["[MAIN$]"].Rows[j][k].ToString();
						}
						contents[6] = addMotionTime.ToString();
						ewo.UpdateData(MotionPara.dataRootPath + MotionPara.excelName, "MAIN", contents, 1);

						cTime += (totalTime);

						//更新数据库CID中sheet 3DMAX，记录CN每条运动的时间
						for (int k = 0; k < motionAdministrator._motionList.Count; k++)
						{
							if(motionAdministrator._motionList[k].GetType().ToString() == "MaxMotion")
							{
								if(!readedMax){
									DataTable sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + "CID"		 + ".xls", "3DMAX");
									if(sheetTable.Rows.Count > 0){
										Debug.LogError("CID.xls中工作表3DMAX中已有数据,请删除最后一个ID为2的行之前		的数据");
									}
									readedMax = true;
								}

								MaxMotion mm = (MaxMotion)motionAdministrator._motionList[k];

								string[] addContent = new string[10];
								addContent[0] = (maxNumber + 2).ToString();
								addContent[1] = MotionPara.excelName;
								addContent[2] = mm.AnimationParent.name;
								addContent[3] = mm.AnimationName;
								addContent[4] = mm.StandardTime.ToString();
								if (mm.PC2_Animation_Name.Count > 0)
								{
									addContent[5] = mm.PC2_Animation_Name[0].ToString();
									addContent[6] = mm.PC2_Animation_StartFrame[0].ToString();
									addContent[7] = mm.PC2_Animation_EndFrame[0].ToString();

									for (int m = 1; m < mm.PC2_Animation_Name.Count; m++)
									{
										addContent[5] += ("|" + mm.PC2_Animation_Name[m].ToString());
										addContent[6] += ("|" + mm.PC2_Animation_StartFrame[m].ToString());
										addContent[7] += ("|" + mm.PC2_Animation_EndFrame[m].ToString());
									}
								}

								addContent[8] = tmp_addMotionTime.ToString();
								addContent[9] = addMotionTime.ToString();

								ewo.AddData(MotionPara.dataRootPath + "CID", "3DMAX", addContent);
								maxNumber++;

							}
						}
					}
					//修正时间设定
					if (isTimeLoad && !computeTime)
					{
						try
						{
							hSliderValue = (currentTime + float.Parse((string)motionDataSet.Tables["[MAIN$]"].Rows[j][6].ToString())) * perTime;
						}
						catch 
						{
							Debug.LogError("时间信息可能为空，位置：" + ErrorLocation.Locate("MAIN", "TIME", MotionPara.mainRowNumber));
						}
					}
				}
				//row参数清零
				row_number = 0;
				//单步播放
                if (isSinglePlay)
                {
                    MotionPara.singlePlay = true;
					yield return StartCoroutine(SingleStepTimer());
				}
				//生成位置
				if(i == idList.Count - 1 && createPosition){
					st_Location.LocationCreate(MotionPara.excelName, "END", false);
				}
			}else{
				Debug.LogError(MotionPara.dataRootPath + idList[i] + ".xls" + "表格为空，请检查！");
				yield break;
			}

			if (computeTime)
			{
				//更新数据库CID中sheet ID,记录CN的总时间
				Debug.Log(idList[i] + ": " + cTime);
				string[] contents = new string[3];

				DataTable sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + "CID" + ".xls", "ID");
				DataRow dr = sheetTable.Rows[i];
				contents = new string[sheetTable.Columns.Count];
				for (int k = 0; k < contents.Length; k++)
				{
					contents[k] = dr[k].ToString();
				}
				contents[2] = cTime.ToString();

				ExcelOperator ewo = new ExcelOperator();
				ewo.UpdateData(MotionPara.dataRootPath + "CID", "ID", contents, 0);
			}
        }
		computeTime = false;
		yield return null;
	}
	
	//摄像机运动模块
	private IEnumerator CameraCoroutine(string came_str, DataTable camera_table)
	{
		string[] cameArray = came_str.Split('|');
        float camera_time = 0f;
		for(int i = 0; i < cameArray.Length; i++){
			string cameKeyStr = InterData.CmdCheck(cameArray[i], "CAMERA");
			if(cameKeyStr != ""){
				//摄像机运动信息提取
				DataRow cameRowData = camera_table.Rows.Find(cameKeyStr);
				//ID获取CAMERA行失败
				if(cameRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "CAMERA", MotionPara.mainRowNumber) + ", CAMERA 没有这个ID!");
					}
					continue;
				}
				bool isRight = true;
				CameraInfoManager tempInfoManager = new CameraInfoManager();
				cameraAdministrator = tempInfoManager.CameraInfoGet(cameRowData, cameKeyStr, ref isRight);
				if(!isRight){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
				if (!computeTime)
				{
					cameraAdministrator.Init();
					cameraFlag = true;
					startTime = 0;
					MotionPara.MotionActive = true;
					cameraAdministrator.State = CurrentState.Active;
					yield return StartCoroutine(CameraTimer());
				}
				else
				{
					cameraAdministrator.Init();
                    camera_time += cameraAdministrator.StandardTime;
					cameraAdministrator.PostProcess();
				}
			}else{
				MotionPara.shouldStop = stopInRow;
				//编辑器模式下会出现警报信息
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("MAIN", "CAMERA", MotionPara.mainRowNumber));
				}
			}	
		}
        if (computeTime)
		{
			cameraTime = camera_time;
		}
		yield return null;
	}

	//Camera运动模块时间控制，待修改
	private IEnumerator CameraTimer()
	{
		while(!cameraAdministrator.TimesUp(startTime))
		{
			yield return new WaitForSeconds(0.01f);
		}
		cameraFlag = false;
		MotionPara.MotionActive = false;
		cameraAdministrator.State = CurrentState.Old;
		cameraAdministrator.PostProcess();
	}


	//Tips控制模块
	private IEnumerator TipsCoroutine(string tips_str, DataTable tips_table)
	{
		//获取Tips ID
		string tipsKeyStr = InterData.CmdCheck(tips_str, "TIPS");
		if(tipsKeyStr != ""){
			//Tips信息提取
			DataRow tipsRowData = tips_table.Rows.Find(tipsKeyStr);
			//ID获取TIPS行失败
			if(tipsRowData == null){
				MotionPara.shouldStop = stopInRow;
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("MAIN", "TIPS", MotionPara.mainRowNumber) + ", TIPS 没有这个ID!");
				}
				yield break;
			}
			bool isRight = true;
			TipsInfoManager tempInfoManager = new TipsInfoManager();
			tipsAdministrator = tempInfoManager.TipsInfoGet(tipsRowData, tipsKeyStr, ref isRight);
			if(!isRight){
				MotionPara.shouldStop = stopInRow;
				yield break;
			}
			if (!computeTime)
			{
				tipsAdministrator.Init();
				startTime = 0;
				MotionPara.MotionActive = true;
				st_Interface.Voice(tipsAdministrator.TipsString, tipsAdministrator.TipsSpeed);
				if (tipsAdministrator.IsString)
				{
					if (tipsAdministrator.IsTitle)
						st_Interface.TipsWindow(true, tipsAdministrator.TipsString, tipsAdministrator.IsMoveable,
							tipsAdministrator.PosString, WindowColor.Blue);
					else
						st_Interface.TipsWindow(true, tipsAdministrator.TipsString, tipsAdministrator.IsMoveable, tipsAdministrator.PosString);
				}
				else
				{
					if (tipsAdministrator.IsTitle)
						st_Interface.TipsWindow(true, tipsAdministrator.TipsString, tipsAdministrator.IsMoveable,
							tipsAdministrator.PosVec2, WindowColor.Blue);
					else
						st_Interface.TipsWindow(true, tipsAdministrator.TipsString, tipsAdministrator.IsMoveable, tipsAdministrator.PosVec2);
				}
				yield return StartCoroutine(TipsTimer());
			}
            else
			{//加速运行,计算时间
				tipsTime = tipsAdministrator.StandardTime;
				tipsAdministrator.PostProcess();
			}
		}else{
			MotionPara.shouldStop = stopInRow;
			//编辑器模式下会出现警报信息
			if(MotionPara.isEditor){
				Debug.LogError(ErrorLocation.Locate("MAIN", "TIPS", MotionPara.mainRowNumber));
			}
		}
		yield return null;
	}

	//Tips模块时间控制，待修改
	private IEnumerator TipsTimer()
	{
		while(!tipsAdministrator.TimesUp(startTime))
		{
			yield return new WaitForSeconds(0.01f);
		}
		//遇到暂停情况，则暂停到这里，或者可以直接向下运行，进行计算
		while(MotionPara.PauseControl)
		{
			yield return new WaitForSeconds(0.01f);
		}
		MotionPara.MotionActive  = false;
	}

    //Trigger控制模块
    private IEnumerator TriggerCoroutine(string trigger_str, DataTable trigger_table)
    {
        TriggerManager.triggerUnitList.Clear();
        string[] triggerArray = trigger_str.Split('|');
        for (int i = 0; i < triggerArray.Length; i++) {
            //获取Trigger ID
			string triggerKeyStr = InterData.CmdCheck(triggerArray[i], "TRIGGER");
            if (triggerKeyStr != "") {
                //TRIGGER信息提取
                DataRow triggerRowData = trigger_table.Rows.Find(triggerKeyStr);
                //ID获取TRIGGER行失败
                if (triggerRowData == null)
                {
                    MotionPara.shouldStop = stopInRow;
                    if (MotionPara.isEditor)
                    {
                        Debug.LogError(ErrorLocation.Locate("MAIN", "TRIGGER", MotionPara.mainRowNumber) + ", TRIGGER 没有这个ID!");
                    }
                    yield break;
                }
                //开始提取表格信息
                TriggerUnit tempUnit = new TriggerUnit();
                //ID
                tempUnit.ID = i;
                //触发类型
                string typeStr = ((string)triggerRowData[2].ToString()).ToUpper();
                bool isRight = true;
                tempUnit.TriggerType = InterData.GetTriggerType(typeStr, ref isRight);
                if (!isRight)
                {
                    MotionPara.shouldStop = stopInRow;
                    //编辑器模式下会出现警报信息
                    if (MotionPara.isEditor)
                    {
                        Debug.LogError(ErrorLocation.Locate("TRIGGER", "TYPE", triggerKeyStr));
                        yield break;
                    }
                }
                //触发按键
                string keyStr = ((string)triggerRowData[3].ToString()).ToLower();
                tempUnit.TriggerKey = keyStr.Split('&');
                //触发物体
                string objStr = (string)triggerRowData[4].ToString();
                tempUnit.TriggerOjb = objStr.Split('&');
				//按钮触发类型
				for (int j = 0; j < tempUnit.TriggerType.Length; j++)
				{
					if (j == 0)
						tempUnit.BtnFuncList = new List<string>();
					if (tempUnit.TriggerType[j] == TriggerType.Button)
					{
						try
						{
							tempUnit.BtnFuncList.Add(tempUnit.TriggerKey[j].ToUpper());
						}
						catch 
						{
							Debug.LogError(ErrorLocation.Locate("TRIGGER", "KEY", triggerKeyStr) + "，Key的个数要与触发类型的个数一致!");
							yield break;
						}
					}
				}

				TriggerManager.triggerUnitList.Add(tempUnit);
            }else{
                MotionPara.shouldStop = stopInRow;
                //编辑器模式下会出现警报信息
                if (MotionPara.isEditor)
                {
                    Debug.LogError(ErrorLocation.Locate("MAIN", "TRIGGER", MotionPara.mainRowNumber));
                    yield break;
                }
            }
        }
        st_Trigger.gameObject.SetActive(true);
        MotionPara.triggerPlay = true;
		//如果有GUI按钮的情况
		BtnFunction.AllForbit();
		for (int i = 0; i < TriggerManager.triggerUnitList.Count; i++)
		{
			if (TriggerManager.triggerUnitList[i].BtnFuncList != null) 
			{
				for (int j = 0; j < TriggerManager.triggerUnitList[i].BtnFuncList.Count; j++)
				{
					BtnFunction.SetBtn(TriggerManager.triggerUnitList[i].BtnFuncList[j], true);
				}
			}
		}
        yield return StartCoroutine(TriggerTimer());

        //yield return null;
    }

    /// <summary>
    /// 等待Trigger的动作
    /// </summary>
    private IEnumerator TriggerTimer()
    {
        while (MotionPara.triggerPlay)
        {
            yield return new WaitForSeconds(0.02f);
        }
        st_Trigger.gameObject.SetActive(false);
		BtnFunction.AllForbit();
    }

	//综合运动模块
	private IEnumerator GeneralCoroutine(string motion_str, DataTable camera_table, DataTable excel_table, 
		DataTable max_table, DataTable group_table, DataTable tips_table, DataTable program_table)
	{
		string[] generalArray = motion_str.Split('|');
        float motions_Time = 0f;
		for(int i = 0; i < generalArray.Length; i++){
			string generKeyStr = "";
			string motionTypeStr = "";
			//提取当前运动类型
			motionTypeStr = InterData.CmdCheck(generalArray[i], 
				new string[] {"CAMERA", "TIPS", "EXCEL", "3DMAX", "PROGRAM"}, ref generKeyStr);
			if(motionTypeStr == ""){
				MotionPara.shouldStop = stopInRow;
				//编辑器模式下会出现警报信息
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
						"，" + generalArray[i] + "：该运动类型填写有错误");
					yield break;
				}
			}
			//Camera运动处理
			if(motionTypeStr == "CAMERA"){
				//摄像机运动信息提取
				DataRow cameRowData = camera_table.Rows.Find(generKeyStr);
				//ID获取CAMERA行失败
				if(cameRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
							"，" + generalArray[i] + "：CAMERA表没有这个ID!");
					}
					continue;
				}
				//如果Camera表参数有误，则退出当前函数
				if(!motionAdministrator.CameraAdd(cameRowData, generKeyStr)){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
			//Tips信息处理
			}else if(motionTypeStr == "TIPS"){
				//Tips信息提取
				DataRow tipsRowData = tips_table.Rows.Find(generKeyStr);
				//ID获取Tips行失败
				if(tipsRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
							"，" + generalArray[i] + "：TIPS表没有这个ID!");
					}
					continue;
				}
				//如果Tips表参数有误，则退出当前函数
				if(!motionAdministrator.TipsAdd(tipsRowData, generKeyStr)){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
			//Excel运动信息处理
			}else if(motionTypeStr == "EXCEL"){
				//Excel运动信息提取
				DataRow excelRowData = excel_table.Rows.Find(generKeyStr);
				//ID获取Excel行失败
				if(excelRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
							"，" + generalArray[i] + "：EXCEL表没有这个ID!");
					}
					continue;
				}
				//如果EXCEL表参数有误，则退出当前函数
				if(!motionAdministrator.ExcelAdd(excelRowData, generKeyStr, group_table)){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
			//3DMax运动信息处理
			}else if(motionTypeStr == "3DMAX"){
				//3DMAX表格信息提取
				DataRow maxRowData = max_table.Rows.Find(generKeyStr);
				//ID获取3DMAX行失败
				if(maxRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
							"，" + generalArray[i] + "：3DMAX表没有这个ID!");
					}
					continue;
				}
				//如果3DMAX表参数有误，则退出当前函数
				if(!motionAdministrator.MaxAdd(maxRowData, generKeyStr)){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
			//Program运动信息处理
			}else if(motionTypeStr == "PROGRAM"){
				//PROGRAM表格信息提取
				DataRow programRowData = program_table.Rows.Find(generKeyStr);
				//ID获取PROGRAM行失败
				if(programRowData == null){
					MotionPara.shouldStop = stopInRow;
					if(MotionPara.isEditor){
						Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
							"，" + generalArray[i] + "：PROGRAM表没有这个ID!");
					}
					continue;
				}
				//如果PROGRAM表参数有误，则退出当前函数
				if(!motionAdministrator.ProgramAdd(programRowData, generKeyStr, group_table)){
					MotionPara.shouldStop = stopInRow;
					yield break;
				}
			//意外的情况，不应该出现
			}else{
				MotionPara.shouldStop = stopInRow;
				//编辑器模式下会出现警报信息
				if(MotionPara.isEditor){
					Debug.LogError(ErrorLocation.Locate("MAIN", "MOTION", MotionPara.mainRowNumber) + 
						"，" + generalArray[i] + "：运动类型填写错误，没有这个运动方式！");
					yield break;
				}
			}		
		}
		//有运动
		if(motionAdministrator.Count > 0){
				motionAdministrator.Init();
				//有TipsMotion的情况
				if (motionAdministrator.HasTipsMotion != null && !computeTime)
				{
					st_Interface.Voice(motionAdministrator.HasTipsMotion.TipsString, tipsAdministrator.TipsSpeed);
					if (motionAdministrator.HasTipsMotion.IsString)
					{
						if (motionAdministrator.HasTipsMotion.IsTitle)
							st_Interface.TipsWindow(true, motionAdministrator.HasTipsMotion.TipsString,
								motionAdministrator.HasTipsMotion.IsMoveable,
								motionAdministrator.HasTipsMotion.PosString, WindowColor.Blue);
						else
							st_Interface.TipsWindow(true, motionAdministrator.HasTipsMotion.TipsString,
								motionAdministrator.HasTipsMotion.IsMoveable, motionAdministrator.HasTipsMotion.PosString);
					}
					else
					{
						if (motionAdministrator.HasTipsMotion.IsTitle)
							st_Interface.TipsWindow(true, motionAdministrator.HasTipsMotion.TipsString,
								motionAdministrator.HasTipsMotion.IsMoveable,
								motionAdministrator.HasTipsMotion.PosVec2, WindowColor.Blue);
						else
							st_Interface.TipsWindow(true, motionAdministrator.HasTipsMotion.TipsString,
								motionAdministrator.HasTipsMotion.IsMoveable, motionAdministrator.HasTipsMotion.PosVec2);
					}
				}
			if (!computeTime)
			{
				startTime = 0;
				MotionPara.MotionActive = true;
				generMotionFlag = true;
				yield return StartCoroutine(GeneralTimer());
			}
			else
			{//加速运行,计算时间
				for (int i = 0; i < motionAdministrator._motionList.Count; i++)
				{
					if (motions_Time < motionAdministrator._motionList[i].StandardTime)
					{
						motions_Time = motionAdministrator._motionList[i].StandardTime;
					}
					motionAdministrator._motionList[i].Init();
					motionAdministrator._motionList[i].PostProcess();
				}
				for (int i = 0; i < motionAdministrator._complexMotionList.Count; i++)
				{
					float tmpMotionTime = 0f;
					for (int j = 0; j < motionAdministrator._complexMotionList[i].Count; j++)
					{
						tmpMotionTime += (motionAdministrator._complexMotionList[i][j].StandardTime);
						motionAdministrator._complexMotionList[i][j].Init();
						motionAdministrator._complexMotionList[i][j].PostProcess();
					}

					if (motions_Time < tmpMotionTime)
					{
						motions_Time = tmpMotionTime;
					}
				}

				motionsTime = motions_Time;
			}
		}else{
			yield return null;
		}
	}

	//综合运动控制器
	private IEnumerator GeneralTimer()
	{
		while(!motionAdministrator.TimesUp(startTime))
		{
			yield return new WaitForSeconds(0.01f);
		}
		//Debug.Log(startTime);
		generMotionFlag = false;
		MotionPara.MotionActive = false;
	}

	/// <summary>
	/// 单步运行控制器
	/// </summary>
	private IEnumerator SingleStepTimer()
	{
		while(MotionPara.singlePlay)
		{                          
			yield return new WaitForSeconds(0.02f);
		}
	}


	// Update is called once per frame
	void Update () 
	{
		/*//TEST, TO START
		if(Input.GetKeyUp(KeyCode.Space)){
			StartCoroutine(MainEntrance(0, 0));
			MotionPara.shouldStop = false;
		}

		//TEST, TO PAUSE
		if(Input.GetKeyUp(KeyCode.P)){
			MotionPara.PauseControl = !MotionPara.PauseControl;
		}*/

		//Time Control
		if (MotionPara.MotionActive && !MotionPara.PauseControl)
		{
			startTime += Time.deltaTime;
		}
		if (isTimeLoad && MotionPara.MotionActive && !MotionPara.PauseControl)
		{
			hSliderValue += Time.deltaTime * perTime * MotionPara.SpeedRate;
		}

		//综合运动控制
		if (generMotionFlag && MotionPara.MotionActive && !MotionPara.PauseControl)
		{
			motionAdministrator.Move(startTime, MotionPara.SpeedRate, Time.deltaTime);
		}

		//进度条触发
		if (!dragLock && dragActive && isTimeLoad)
		{
			ProcessTrigger();
		}
	}

	//进度条触发
	private void ProcessTrigger()
	{
		//鼠标抬起
		if (Input.GetMouseButtonUp(0))
		{
			dragActive = false;
			dragLock = true;
			float processValue = hSliderValue;
			//停止当前运动
			StopButton();
			//拖到最后停止
			if (Mathf.Abs(processValue - TotalValue) < 0.01f || processValue > TotalValue)
			{
				//设置到最终位置，结束
				hSliderValue = TotalValue;
				//设置
				SetLocation(idList.Count, "END");
			}
			//拖到开始位置
			else if (processValue < 0.01f)
			{
				//从头开始播
				hSliderValue = 0f;
				//设置
				SetLocation(1, "2");
				StartCoroutine(MainEntrance(0, 0));
			}
			//中间位置
			else
			{
				//当前时间
				float presentTime = processValue / perTime;
				//Debug.Log(presentTime);
				//节点号
				int nodeNo = 0;
				//行号
				//int rowNo = 0;
				ExcelOperator excelReader = new ExcelOperator();
				float timeRecorder = 0f;
				for (int i = 0; i < nodeTimeList.Count; i++)
				{
					timeRecorder += nodeTimeList[i];
					if (presentTime <= timeRecorder)
					{
						nodeNo = i + 1;
						float basicTime = timeRecorder - nodeTimeList[i];
						DataTable mainTable = excelReader.ExcelReader(MotionPara.dataRootPath + idList[i] + ".xls", "MAIN", "G", "G");
						for (int j = 0; j < mainTable.Rows.Count; j++)
						{
							if (presentTime < float.Parse((string)mainTable.Rows[j][0].ToString()) + basicTime)
							{
								SetLocation(nodeNo, (j + 2).ToString());
								if (j != 0)
								{
									hSliderValue = (basicTime + float.Parse((string)mainTable.Rows[j - 1]				[0].ToString())) * perTime;
								}
								else
								{
									hSliderValue = basicTime * perTime;
								}
								//快速跳转
								StartCoroutine(MainEntrance(i, j));
								//rowNo = j;
								//Debug.Log("Node: " + nodeNo + "---Row: " + rowNo);
								i = nodeTimeList.Count; //跳出第二个循环
								break;
							}
						}
					}
				}
			}
			//3DMAX处理
			MaxStateProcess(hSliderValue, preProcess);
			//锁住解除
			dragLock = false;
		}
	}

	//3DMax状态处理
	private void MaxStateProcess(float current_value, float pre_process)
	{
		ExcelOperator excelReader = new ExcelOperator();
		DataTable maxTable = excelReader.ExcelReader(MotionPara.dataRootPath + IDTableName + ".xls", "3DMAX", "A", "J");
		//表格行重复判断，因为编辑人员的疏忽可能导致3DMAX表格内容重复
		List<string> repeatedJudge = new List<string>();
		for (int i = 0; i < maxTable.Rows.Count; i++) 
		{
			string keyStr = (string)maxTable.Rows[i][0].ToString();
			if (repeatedJudge.Contains(keyStr))
			{
				return;  //以下重复，退出
			}
			else 
			{
				repeatedJudge.Add(keyStr);
				//ID表
				string tableID = (string)maxTable.Rows[i][1].ToString();
				int idIndex = idList.IndexOf(tableID);
				if (idIndex == -1)
				{
					Debug.LogError(IDTableName + ".xls，（" + (i + 2).ToString() + "，B）ID表填写错误！");
					return;
				}
				else 
				{
					//读入后面的数据
					float basicTime = 0f;
					if (idIndex != 0)
						basicTime = nodeTimeList[idIndex - 1];
					float endValue = (float.Parse((string)maxTable.Rows[i][9].ToString()) + basicTime) * perTime;
					//首先判断需不需要改变Max状态
					float valMin = pre_process;
					float valMax = current_value;
					if (pre_process > current_value) 
					{
						valMin = current_value;
						valMax = pre_process;
					}
					if (endValue < valMin || endValue > valMax)  //无需更改状态
					{
						continue;
					}
					else  //需要更改状态
					{
						DataRow maxRow = maxTable.Rows[i];
						bool isRight = true;
						MaxInfoManager maxInfo = new MaxInfoManager();
						MaxMotion maxMotion = maxInfo.MaxInfoGet(maxRow, keyStr, ref isRight);
						if (endValue < current_value)
						{
							//已经运行过了
							maxMotion.setCurrentProgress(false);
						}
						else
						{
							//没有运行到
							maxMotion.setCurrentProgress(true);
						}
					}
				}
			}
		}
	}

	//Motion Update
	void LateUpdate ()
	{
		//单独摄像机动作
		if(cameraFlag && MotionPara.MotionActive && !MotionPara.PauseControl){
			cameraAdministrator.Move(startTime, MotionPara.SpeedRate, Time.deltaTime);
		}

		////综合运动控制
		//if(generMotionFlag && MotionPara.MotionActive && !MotionPara.PauseControl){
		//	motionAdministrator.Move(startTime, MotionPara.SpeedRate, Time.deltaTime);
		//}
	}
}
