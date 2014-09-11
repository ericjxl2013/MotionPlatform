/// <summary>
/// <Filename>: EditorManager.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.20
/// Version: 1.0
/// Company: Sunnytech
/// Function: 美工编辑人员编辑工具管理脚本
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
//using System.Diagnostics;

public class EditorManager : MonoBehaviour {

	MotionManager st_Manager;
	MotionEditor st_Editor;

	private string rootDirectory = "";

	public bool isJson = false;  //是否用Json读取

	//编辑窗口参数
	private Rect editorRect = new Rect(50, 50, 550, 350);
	private int toolbarInt; //工具条控制参数
	private string[] toolbarStrings;  //工具条名称
	private string timeDisplayStr = "未加载时间信息";
	private Rect sliderRect = new Rect(25, 150, 250, 10);
	private string motionTypeStr = "拆";
	private string autoLocationStr = "否";
	private string motionStateStr = "教";
	private string currentEditorExcel = "C1";  //当前编辑表格
	private int currentIDIndex = 0;
	private string newProjName = "新项目";
	private string newExcelName = "C2";
	

	void Awake ()
	{
		st_Manager = gameObject.GetComponent<MotionManager>();
		gameObject.AddComponent<MotionEditor>();
		st_Editor = gameObject.GetComponent<MotionEditor>();
		rootDirectory = Application.dataPath + "/Resources";
		
	}
	
	void Start () 
	{
		//FileInitialize(MotionPara.taskName);
		toolbarInt = 1;
		toolbarStrings = new string[] { "信息初始化", "运动控制界面", "时间和位置信息" , "装配表生成界面", "运动表格检查"};
		sliderRect = new Rect(40, 190, 300, 10);
	}

	void OnGUI()
	{
		editorRect = GUI.Window(111, editorRect, EditWindow, "运动信息填写窗口");
	}

	void EditWindow(int WindowID)
	{
		toolbarInt = GUI.Toolbar(new Rect(20, 30, 510, 30), toolbarInt, toolbarStrings);
		GUI.Box(new Rect(20, 70, 510, 260), "");
		switch (toolbarInt)
		{
			case 0:
				ProjectInit();
				break;
			case 1:
				MotionControl();
				break;
			case 2:
				TimeAndLocation();
				break;
			case 3:
				AssembleCreate();
				break;
			case 4:
				Checking();
				break;
			default:
				break;
		}
		GUI.DragWindow();
	}


	//运动控制界面
	void MotionControl()
	{
		GUI.contentColor = Color.white;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;

		GUI.Label(new Rect(30, 80, 130, 20), "从该节点开始运行:");
		st_Manager.tableNum = GUI.TextField(new Rect(145, 80, 40, 20), st_Manager.tableNum);
		GUI.Label(new Rect(190, 80, 130, 20), "ROW:");
		st_Manager.rowNum = GUI.TextField(new Rect(230, 80, 40, 20), st_Manager.rowNum);
		if (GUI.Button(new Rect(280, 80, 70, 20), "运行"))
		{
			//首先判断节点号
			int nodeNum = 0;
			try
			{
				nodeNum = int.Parse(st_Manager.tableNum);
			}
			catch
			{
				Debug.LogError("ID号不是数字！");
				return;
			}
			if (nodeNum < 1)
			{
				Debug.LogError("ID号从1开始！");
				return;
			}
			//处理ID号
			int rowNum = 0;
			try
			{
				rowNum = int.Parse(st_Manager.rowNum);
			}
			catch
			{
				Debug.LogError("ROW处应填Excel对应的行号，从2开始！");
				return;
			}
			if (rowNum < 2)
			{
				Debug.LogError("ROW号从2开始！");
				return;
			}
			if (MotionPara.MotionActive) 
			{
				st_Manager.StopButton();
			}
			st_Manager.SetLocation(nodeNum, st_Manager.rowNum);
			MotionPara.PauseControl = false;
			StartCoroutine(st_Manager.MainEntrance(nodeNum - 1, rowNum - 2));
		}
		GUI.contentColor = Color.green;
		//时间信息
		st_Manager.isTimeLoad = GUI.Toggle(new Rect(380, 80, 300, 20), st_Manager.isTimeLoad, timeDisplayStr);
		if (st_Manager.isTimeLoad)
		{
			timeDisplayStr = "已加载时间信息";
		}
		else
		{ 
			timeDisplayStr = "未加载时间信息";
		}
		//位置信息
		st_Manager.createPosition = GUI.Toggle(new Rect(380, 100, 300, 20), st_Manager.createPosition, autoLocationStr);
		if (st_Manager.createPosition)
			autoLocationStr = "播放时生成位置信息";
		else
			autoLocationStr = "播放时不生成位置信息";
		GUI.contentColor = Color.white;

		//表格编辑器控制
		GUI.contentColor = Color.cyan;
		if (GUI.Button(new Rect(380, 130, 130, 30), "打开表格编辑器"))
		{
			MotionEditor.show = true;
			st_Editor.ExcelWriteWindow.x = 0;
			st_Editor.ExcelWriteWindow.y = 0;
		}
		currentEditorExcel = GUI.TextField(new Rect(380, 170, 55, 30), currentEditorExcel);
		if (GUI.Button(new Rect(450, 170, 60, 30), "切换表格"))
		{
			string filePath = Application.dataPath + "/Resources/" + MotionPara.taskName + st_Manager.subTablePath + currentEditorExcel;
			if (File.Exists(filePath + ".xls"))
			{
				currentIDIndex = st_Manager.idList.IndexOf(currentEditorExcel); ;
				st_Editor.setExcel(filePath);
			}
			else
			{
				Debug.LogError("该文件不存在---路径：" + filePath);
				return;
			}
		}
		if (GUI.Button(new Rect(380, 210, 60, 30), "上一个"))
		{
			int formerID = 0;
			if (currentIDIndex == -1) 
			{
				return;
			}
			else if (currentIDIndex == 0)
			{
				formerID = st_Manager.idList.Count - 1;
				currentEditorExcel = st_Manager.idList[formerID];
			}
			else 
			{
				formerID = currentIDIndex - 1;
				currentEditorExcel = st_Manager.idList[formerID];
			}
			string filePath = Application.dataPath + "/Resources/" + MotionPara.taskName + st_Manager.subTablePath + currentEditorExcel;
			if (File.Exists(filePath + ".xls"))
			{
				currentIDIndex = st_Manager.idList.IndexOf(currentEditorExcel); ;
				st_Editor.setExcel(filePath);
			}
			else
			{
				Debug.LogError("该文件不存在---路径：" + filePath);
				return;
			}
		}
		if (GUI.Button(new Rect(450, 210, 60, 30), "下一个"))
		{
			int formerID = 0;
			if (currentIDIndex == -1)
			{
				return;
			}
			else if (currentIDIndex == st_Manager.idList.Count - 1)
			{
				formerID = 0;
				currentEditorExcel = st_Manager.idList[formerID];
			}
			else
			{
				formerID = currentIDIndex + 1;
				currentEditorExcel = st_Manager.idList[formerID];
			}
			string filePath = Application.dataPath + "/Resources/" + MotionPara.taskName + st_Manager.subTablePath + currentEditorExcel;
			if (File.Exists(filePath + ".xls"))
			{
				currentIDIndex = st_Manager.idList.IndexOf(currentEditorExcel); ;
				st_Editor.setExcel(filePath);
			}
			else
			{
				Debug.LogError("该文件不存在---路径：" + filePath);
				return;
			}
		}


		GUI.contentColor = Color.white;
		

		GUI.Label(new Rect(30, 110, 130, 20), "运动控制:");
		if (GUI.Button(new Rect(40, 140, 40, 40), "开始"))
		{
			st_Manager.PlayButton();
		}
		if (GUI.Button(new Rect(90, 140, 40, 40), "暂停"))
		{
			st_Manager.PauseButton();
		}
		if (GUI.Button(new Rect(140, 140, 40, 40), "减速"))
		{
			st_Manager.SpeedControl(false);
		}
		if (GUI.Button(new Rect(190, 140, 40, 40), "加速"))
		{
			st_Manager.SpeedControl(true);
		}
		if (GUI.Button(new Rect(240, 140, 40, 40), "停止"))
		{
			st_Manager.StopButton();
		}
		if (GUI.Button(new Rect(290, 140, 60, 40), "单步运行"))
		{
			st_Manager.SinglePlayButton();
		}
		st_Manager.hSliderValue = GUI.HorizontalSlider(sliderRect, st_Manager.hSliderValue, 0.0f, st_Manager.TotalValue);
		if (sliderRect.Contains(Event.current.mousePosition))
		{
			if (Input.GetMouseButtonDown(0) && !st_Manager.dragLock && !st_Manager.dragActive)
			{
				st_Manager.dragActive = true;
				st_Manager.preProcess = st_Manager.hSliderValue;
			}
		}

		if (GUI.Button(new Rect(40, 210, 90, 30), "超快速播放"))
		{
			st_Manager.ChangeRate(100f);
		}
		GUI.Box(new Rect(140, 205, 120, 40), "");
		GUI.Label(new Rect(145, 208, 120, 30), "当前速度值:");
		GUI.contentColor = Color.green;
		GUI.Label(new Rect(225, 208, 120, 30), MotionPara.SpeedRate.ToString());
		GUI.contentColor = Color.white;
		GUI.Label(new Rect(145, 223, 120, 30), "播放状态:");
		GUI.contentColor = Color.green;
		if (MotionPara.MotionActive)
		{
			if (MotionPara.PauseControl)
			{
				GUI.Label(new Rect(215, 223, 120, 30), "暂停中");
			}
			else
			{
				GUI.Label(new Rect(215, 223, 120, 30), "播放中");
			}
		}
		else 
		{
			GUI.Label(new Rect(215, 223, 120, 30), "停止中");
		}
		
		GUI.contentColor = Color.white;
		if (GUI.Button(new Rect(270, 210, 80, 30), "速度恢复1"))
		{
			st_Manager.ChangeRate(1f);
		}
		
		GUI.Box(new Rect(40, 250, 140, 30), "");
		GUI.Label(new Rect(45, 253, 130, 20), "当前运动方式:");
		GUI.contentColor = Color.green;
		GUI.Label(new Rect(130, 253, 100, 20), motionTypeStr);
		GUI.contentColor = Color.white;
		if (GUI.Button(new Rect(40, 290, 40, 30), "拆"))
		{
			//if (FuncPara.curentMode != Movement.Chai)
			//{
			//	motionTypeStr = "拆动画";
				
			//}
		}
		if (GUI.Button(new Rect(90, 290, 40, 30), "装"))
		{
			//if (FuncPara.curentMode != Movement.Zhuang)
			//{
			//	motionTypeStr = "装动画";
				
			//}
		}
		if (GUI.Button(new Rect(140, 290, 40, 30), "原理"))
		{
			//if (FuncPara.curentMode != Movement.Yuan)
			//{
			//	motionTypeStr = "原理动画";
				
			//}
		}

		GUI.Box(new Rect(200, 250, 140, 30), "");
		GUI.Label(new Rect(205, 253, 130, 20), "当前运动状态:");
		GUI.contentColor = Color.green;
		GUI.Label(new Rect(290, 253, 100, 20), motionStateStr);
		GUI.contentColor = Color.white;
		//教
		if (GUI.Button(new Rect(200, 290, 40, 30), "教"))
		{
			if (FuncPara.currentMotion != MotionState.Teaching)
			{
				motionStateStr = "教";
				FuncPara.currentMotion = MotionState.Teaching;
			}
		}
		//练
		if (GUI.Button(new Rect(250, 290, 40, 30), "练"))
		{
			if (FuncPara.currentMotion != MotionState.Exercising)
			{
				motionStateStr = "练";
				FuncPara.currentMotion = MotionState.Exercising;
			}
		}
		//考
		if (GUI.Button(new Rect(300, 290, 40, 30), "考"))
		{
			//if (FuncPara.currentMotion != MotionState.Examining)
			//{
			//	motionStateStr = "考";

			//}
		}
		GUI.contentColor = Color.white;
	}

	//初始化
	void ProjectInit()
	{
        GUI.contentColor = Color.white;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(30, 80, 300, 20), "新项目文件系统初始化:");
		GUI.Label(new Rect(40, 110, 130, 20), "输入新项目名称:");
		newProjName = GUI.TextField(new Rect(150, 110, 100, 20), newProjName);
		if (GUI.Button(new Rect(270, 110, 100, 20), "文件生成"))
		{
			if (Directory.Exists(Application.dataPath + "/Resources/" + newProjName))
			{
				Debug.LogError("该项目已存在工程目录");
			}
			else
			{
				createProject(newProjName);
			}
		}

		GUI.Label(new Rect(40, 140, 300, 20), "新项目ObjectName表检查:");
		if (GUI.Button(new Rect(200, 140, 100, 20), "表格检查"))
		{
			//NameCreate_Script.Create();
		}

		GUI.Label(new Rect(30, 180, 300, 20), "运动表格管理:");
		GUI.Label(new Rect(40, 210, 300, 20), "新增运动表:");
		newExcelName = GUI.TextField(new Rect(120, 210, 130, 20), newExcelName);
		if (GUI.Button(new Rect(270, 210, 100, 20), "表格生成"))
		{
			newExcelName = newExcelName.ToUpper();
			string subStr = "";
			if (newExcelName.StartsWith("C"))
			{
				subStr = "/C/" + newExcelName + ".xls";
			}
			else if (newExcelName.StartsWith("Y"))
			{
				subStr = "/Y/" + newExcelName + ".xls";
			}
			else if (newExcelName.StartsWith("Z"))
			{
				subStr = "/Z/" + newExcelName + ".xls";
			}
			else
			{
				Debug.LogError("\"" + newExcelName + "\"表格名格式错误！");
				return;
			}
			File.Copy(Application.streamingAssetsPath + "/ExcelMotionData/C1.xls", Application.dataPath + "/Resources/" + MotionPara.taskName + subStr, false);
			ExcelOperator excelOp = new ExcelOperator();
			DataTable idTable = excelOp.ExcelReader(Application.dataPath + "/Resources/" + MotionPara.taskName + st_Manager.subTablePath + st_Manager.IDTableName + ".xls", "ID", "A", "A");
			bool toInsert = true;
			for (int i = 0; i < idTable.Rows.Count; i++)
			{
				if ((string)idTable.Rows[i][0].ToString() == newExcelName)
				{
					toInsert = false;
					break;
				}
			}
			if (toInsert)
			{
				excelOp.AddData(Application.dataPath + "/Resources/" + MotionPara.taskName + st_Manager.subTablePath + st_Manager.IDTableName, "ID", new string[] { newExcelName, "Description", "100"});
			}
		}
	}

	//新建任务
	public void createProject(string taskName)
	{
		//创建文件夹
		string foleName = Application.dataPath + "/Resources/" + taskName;

		System.IO.Directory.CreateDirectory(foleName);

		foleName = Application.dataPath + "/Resources/" + taskName + "/C";

		System.IO.Directory.CreateDirectory(foleName);

		foleName = Application.dataPath + "/Resources/" + taskName + "/Y";

		System.IO.Directory.CreateDirectory(foleName);

		foleName = Application.dataPath + "/Resources/" + taskName + "/Z";

		System.IO.Directory.CreateDirectory(foleName);

		//拷贝Excel文件

		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/C1.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/C/C1.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/CID.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/C/CID.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/C1.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/Y/Y1.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/YID.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/Y/YID.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/C1.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/Z/Z1.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/ZID.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/Z/ZID.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/Tools.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/Tools.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/FunctionManager.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/FunctionManager.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/ProgramMotionID.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/ProgramMotionID.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/ObjectName.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/ObjectName.xls"
				  , false);
		File.Copy(Application.dataPath + "/StreamingAssets/ExcelMotionData/ToolsVariable.xls"
				 , Application.dataPath + "/Resources/" + taskName + "/ToolsVariable.xls"
				  , false);
	}

	//时间和位置信息管理
	void TimeAndLocation()
	{
        GUI.contentColor = Color.white;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(30, 80, 300, 20), "时间和位置信息管理:");
		GUI.Label(new Rect(40, 110, 130, 20), "开始生成时间信息:");
		if (GUI.Button(new Rect(160, 110, 140, 20), "生成时间信息"))
		{
			if (MotionPara.MotionActive)
			{
				st_Manager.StopButton();
			}
			MotionPara.PauseControl = false;
			st_Manager.computeTime = true;
			StartCoroutine(st_Manager.MainEntrance(0, 0));
		}
		GUI.contentColor = Color.green;
		//位置信息
		st_Manager.createPosition = GUI.Toggle(new Rect(320, 110, 300, 20), st_Manager.createPosition, autoLocationStr);
		if (st_Manager.createPosition)
			autoLocationStr = "播放时生成位置信息";
		else
			autoLocationStr = "播放时不生成位置信息";
		GUI.contentColor = Color.white;
	}

	//装配表自动生成
	void AssembleCreate()
	{
		GUI.Label(new Rect(30, 80, 300, 20), "请程序吃糖再改:");
		
	}

	//运动表格正确性检查
	void Checking()
	{
        GUI.contentColor = Color.white;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(30, 80, 300, 20), "运动表格正确性检查:");
		GUI.Label(new Rect(40, 110, 230, 20), "1.物体Scale信息检查(放初始化里):");
		if (GUI.Button(new Rect(280, 110, 80, 20), "检查"))
		{
			checkScale();
		}

		GUI.Label(new Rect(40, 170, 230, 20), "2.运动物体填写检查:");
		if (GUI.Button(new Rect(280, 170, 80, 20), "检查"))
		{
			checkMotionObject();
		}
	}

	//检查模型比例
	void checkScale()
	{
		Debug.Log("物体Scale信息检查");
		ExcelOperator excelReader = new ExcelOperator();
		System.Data.DataTable sheetTable = excelReader.ExcelReader(MotionPara.taskRootPath + MotionPara.taskName + "/ObjectName.xls", "NAME", "A", "A");

		for (int i = 0; i < sheetTable.Rows.Count; i++)
		{
			DataRow dr = sheetTable.Rows[i];
			string objName = dr[0].ToString();
			if (GameObject.Find(objName) != null)
			{
				float scale_x = GameObject.Find(objName).transform.localScale.x;
				float scale_y = GameObject.Find(objName).transform.localScale.y;
				float scale_z = GameObject.Find(objName).transform.localScale.z;
				if ((Mathf.Abs(scale_x - 1.0f) < 0.01f) && (Mathf.Abs(scale_y - 1.0f) < 0.01f) && (Mathf.Abs(scale_z - 1.0f) < 0.01f))
				{

				}
				else
				{
					Debug.LogError(MotionPara.taskName + "工程中ObjectName.xls中NAME工作表中第" + (i + 2) + "行的物体" + objName + "的Scale出错");
				}
			}
			else
			{
				Debug.LogError(MotionPara.taskName + "工程中ObjectName.xls中NAME工作表中第" + (i + 2) + "行的物体" + objName + "不存在");
			}
		}

		Debug.Log("物体Scale信息检查——完毕");
	}
	void checkMotionObject()
	{
		Debug.Log("运动物体填写检查");

		//List保存ObjectName的物体名
		List<string> objNames = new List<string>();
		ExcelOperator excelReader = new ExcelOperator();
		System.Data.DataTable sheetTable = excelReader.ExcelReader(MotionPara.taskRootPath + MotionPara.taskName + "/ObjectName.xls", "NAME", "A", "A");
		for (int i = 0; i < sheetTable.Rows.Count; i++)
		{
			DataRow dr = sheetTable.Rows[i];
			objNames.Add(dr[0].ToString());
		}

		//ID表遍历
		string excel_Name = "";
		for (int i = 0; i < GameObject.Find("MainScript").GetComponent<MotionManager>().idList.Count; i++)
		{
			excel_Name = GameObject.Find("MainScript").GetComponent<MotionManager>().idList[i];

			//			Debug.Log("excel_Name:"+excel_Name);

			//Camera 
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "CAMERA", "D", "D");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];
				//				Debug.Log("CAMERA,"+ j+ ","+ dr[0].ToString());
				if (!objNames.Contains(dr[0].ToString()))
				{
					Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中CAMERA工作表中第" + (j + 2) + "行的参考物体" + dr[0].ToString() + "在ObjectName.xls中不存在");
				}
			}

			//EXCEL
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "EXCEL", "L", "L");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];
				if (dr[0].ToString() != "")
				{
					//					Debug.Log("EXCEL,"+ j+ ","+ dr[0].ToString());
					if (!objNames.Contains(dr[0].ToString()))
					{
						Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中EXCEL工作表中第" + (j + 2) + "行的参考物体" + dr[0].ToString() + "在ObjectName.xls中不存在");
					}
				}
			}

			//3DMAX
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "3DMAX", "C", "C");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];
				//				Debug.Log("3DMAX,"+ j+ ","+ dr[0].ToString());
				if (!objNames.Contains(dr[0].ToString()))
				{
					Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中3DMAX工作表中第" + (j + 2) + "行的参考物体" + dr[0].ToString() + "在ObjectName.xls中不存在");
				}
			}

			//PROGRAM
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "PROGRAM", "C", "E");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];

				if (dr[0].ToString() == "BaiFang" || dr[0].ToString() == "NingSong" || dr[0].ToString() == "NingChu")
				{
					//					Debug.Log("PROGRAM,"+ j+ ","+ dr[2].ToString());
					if (!objNames.Contains(dr[2].ToString()))
					{
						Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中PROGRAM工作表中第" + (j + 2) + "行的参考物体" + dr[2].ToString() + "在ObjectName.xls中不存在");
					}
				}
			}

			//TRIGGER
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "TRIGGER", "E", "E");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];

				if (dr[0].ToString() != "")
				{
					//					Debug.Log("TRIGGER,"+ j+ ","+ dr[0].ToString());
					if (GameObject.Find(dr[0].ToString()) == null)
					{
						Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中TRIGGER工作表中第" + (j + 2) + "行的物体" + dr[0].ToString() + "不存在");
					}
				}
			}

			//GROUP
			sheetTable = excelReader.ExcelReader(MotionPara.dataRootPath + excel_Name + ".xls", "GROUP");
			for (int j = 0; j < sheetTable.Rows.Count; j++)
			{
				DataRow dr = sheetTable.Rows[j];

				for (int k = 1; k < sheetTable.Columns.Count; k++)
				{
					if (dr[k].ToString() != "" && dr[k].ToString() != "0")
					{
						//						Debug.Log("GROUP,"+ j+ ","+ k+ ","+ dr[k].ToString());
						if (!objNames.Contains(dr[k].ToString()))
						{
							Debug.LogError(MotionPara.taskName + "工程中" + excel_Name + ".xls中GROUP工作表中第" + (j + 2) + "行第" + (k) + "列的物体" + dr[k].ToString() + "在ObjectName.xls中不存在");
						}
					}
				}

			}
		}

		Debug.Log("运动物体填写检查完毕");
	}

	//开始一个新任务时的文件系统初始化
	void FileInitialize(string task_name)
	{
		DirectoryCreate(rootDirectory);
		DirectoryCreate(rootDirectory + "/" + task_name);
		DirectoryCreate(rootDirectory + "/" + task_name + "/C");
		DirectoryCreate(rootDirectory + "/" + task_name + "/Z");
		DirectoryCreate(rootDirectory + "/" + task_name + "/Y");
	}

	//判断是否要创建文件夹
	void DirectoryCreate(string dir_path)
	{
		if(!Directory.Exists(dir_path)){
			Directory.CreateDirectory(dir_path);
		}
	}

	

}
