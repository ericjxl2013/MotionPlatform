/// <summary>
/// FileName: ExamControl.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.04.3
/// Version: 1.0
/// Company: Sunnytech
/// Function: 用于考试流程控制
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExamControl : MonoBehaviour {
	FunctionManager st_FuncManager;
	
	Rect inputRect;
//	Rect examRect;
	Rect confirmRect;
	bool alreadyInput = false;  //在本次软件打开后是否已输入学号
	string timeString = "";  //倒计时字符串
	float totalTime = 0;  //总时间，秒为单位
	public bool returnToExercise = false;  //考试状态下按到隐匿式菜单回到练习模式
	
	Rect quitRect;
	
	// Use this for initialization
	void Start () {
		st_FuncManager = gameObject.GetComponent<FunctionManager>();
		inputRect = new Rect(300, 300, 320, 230);
//		examRect = new Rect(300, 300, 200, 220);
		confirmRect = new Rect(300, 300, 320, 200);
		confirmRect.x = (Screen.width - confirmRect.width) / 2;
		confirmRect.y = (Screen.height - confirmRect.height) / 2;
		returnToExercise = false; 
		
//		examRect = new Rect(0, 0, 520, 55);
		FuncPara.examRect = new Rect(0, 0, 395, 55);
		FuncPara.examRect.y = Screen.height - FuncPara.examRect.height - 1f;
		quitRect = new Rect(300, 300, 320, 200);
		quitRect.x = (Screen.width - quitRect.width) / 2;
		quitRect.y = (Screen.height - quitRect.height) / 2;

	}
	
	void OnGUI () {
		if(FuncPara.exitWindow){  //退出确认窗口
			GUI.skin.window = FuncPara.skin_hiMenu.window;
			GUI.Window(2, quitRect, QuitWindow, "");
			GUI.BringWindowToFront(2);
			GUI.skin.window = null;
		}
		
		if(FuncPara.msgInputWindow){  //姓名和学号输入窗口
			GUI.skin.window = FuncPara.skin_hiMenu.window;
			inputRect = GUI.Window(12, inputRect, InputWindow, "");
			GUI.BringWindowToFront(12);
			GUI.skin.window = null;
		}
		
		if(FuncPara.examWindow){  //考试控制窗口
			GUI.skin.window = FuncPara.defaultSkin.window;
			FuncPara.examRect = GUI.Window(27, FuncPara.examRect, ExamWindow, "");
			if(!FuncPara.examConfirm)
				GUI.BringWindowToFront(27);
			GUI.skin.window = null;
		}
		
		if(FuncPara.examConfirm){  //考试结束确认窗口
			GUI.skin.window = FuncPara.skin_hiMenu.window;
			confirmRect = GUI.Window(28, confirmRect, ConfirmWindow, "");
			GUI.BringWindowToFront(28);
			GUI.skin.window = null;
		}
		
	}
	
	//退出程序确认
	void QuitWindow(int WindowID)
	{
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 19;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 5, 200, 30), "退出确认");
		
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect(35, 60, 240, 200), "是否确认退出当前程序？");
		
		if(GUI.Button(new Rect(50, 130, 100, 35), "确定", FuncPara.sty_SquareBtn)){
			Application.Quit();
		}
		
		if(GUI.Button(new Rect(170, 130, 100, 35), "取消", FuncPara.sty_SquareBtn)){
			FuncPara.exitWindow = false;
		}
		
		GUI.skin.label = null;
	}
	
	//姓名和学号输入窗口
	void InputWindow(int WindowID){
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 19;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 5, 200, 30), "考试信息输入");
		
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect(30, 65, 100, 30), "姓  名：");
		GUI.Label(new Rect(30, 120, 100, 30), "学  号：");
		
		GUI.skin.settings.cursorColor = Color.black;
		GUI.SetNextControlName("StudentName");
		FuncPara.studentName = GUI.TextField(new Rect(90, 65, 190, 32), FuncPara.studentName, 30, FuncPara.sty_InputBar);
		FuncPara.studentName = FuncPara.studentName.Replace("\n", "");
		GUI.SetNextControlName("StudentID");
		FuncPara.studentID = GUI.TextField(new Rect(90, 120, 190, 32), FuncPara.studentID, 30, FuncPara.sty_InputBar);
		FuncPara.studentID = FuncPara.studentID.Replace("\n", "");
		
		if(GUI.Button(new Rect(35, 177, 100, 35), "确定", FuncPara.sty_SquareBtn)){
			if(FuncPara.studentID != "" && FuncPara.studentName != ""){
				alreadyInput = true;
				FuncPara.msgInputWindow = false;
				st_FuncManager.ExamState("start");  //开启考试
			}else{
				st_FuncManager.WarnningMsg("姓名和学号不能为空！\n");
			}
		}
		
		if(GUI.Button(new Rect(185, 177, 100, 35), "取消", FuncPara.sty_SquareBtn)){
			FuncPara.msgInputWindow = false;
		}
		
		GUI.DragWindow();
		GUI.skin.label = null;
	}
	
	/// <summary>
	/// 显示学号输入窗口.
	/// </summary>
	/// <returns>
	/// true：已输入过学号；false：还未输入.
	/// </returns>
	public bool InputWindowShow(){
		if(!alreadyInput){  //本次启动没有输入过，则打开输入窗口
			inputRect.x = (Screen.width - inputRect.width) / 2;
			inputRect.y = (Screen.height - inputRect.height) / 2;
			GUI.FocusControl("StudentName");
			FuncPara.msgInputWindow = true;
			return false;
		}else
			return true;
	}
	
	//考试时的状态窗口
	void ExamWindow(int WindowID)
	{
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.normal.textColor = Color.white;
		
		GUI.skin.button = FuncPara.defaultSkin.button;
		GUI.skin.button.font = FuncPara.defaultFont;
		GUI.skin.button.fontSize = 14;
		GUI.skin.button.normal.textColor = Color.white;
		
		if(FuncPara.examStart){  //考试状态
//			GUI.Label(new Rect(10, 21, 100, 30), "倒计时：");
			GUI.Label(new Rect(10, 21, 100, 30), timeString);
			GUI.Label(new Rect(318, 21, 100, 30), "分数：");
			GUI.Label(new Rect(363, 21, 100, 30), FuncPara.examScore.ToString());

			//WuHao
//			GUI.Label(new Rect(363, 21, 100, 30), st_FuncManager.st_Manager.tableNum+","+st_FuncManager.st_Manager.rowNum);
			
			if(GUI.Button(new Rect(80, 20, 100, 30), "结束考试")){
				FuncPara.examConfirm = true;
			}
			
			if(GUI.Button(new Rect(200, 20, 100, 30), "跳过本步")){
				st_FuncManager.PassCurrentStep();
			}
		}else{  //考试结束状态
//			GUI.Label(new Rect(10, 21, 100, 30), "倒计时：");
			GUI.Label(new Rect(10, 21, 100, 30), timeString);
			GUI.Label(new Rect(318, 21, 100, 30), "分数：");
			GUI.Label(new Rect(363, 21, 100, 30), FuncPara.examScore.ToString());

			//WuHao
//			GUI.Label(new Rect(363, 21, 100, 30), st_FuncManager.st_Manager.tableNum+","+st_FuncManager.st_Manager.rowNum);
			
			if(GUI.Button(new Rect(80, 20, 100, 30), "开始考试")){
				st_FuncManager.ExamState("start");
			}
			
			if(GUI.Button(new Rect(200, 20, 100, 30), "返回练习")){
				st_FuncManager.ExamState("return_exercise");
			}
		}
		
		GUI.skin.label = null;
		GUI.skin.button = null;
	}
	
	//考试结束确认
	void ConfirmWindow(int WindowID)
	{
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 19;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 5, 200, 30), "考试结束确认");
		
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect(35, 60, 240, 200), "是否确认结束当前考试，并提交考试成绩？");
		
		if(GUI.Button(new Rect(50, 130, 100, 35), "确定", FuncPara.sty_SquareBtn)){
			ExamFinish();
			FuncPara.examConfirm = false;
			if(returnToExercise){
				returnToExercise = false;
				st_FuncManager.ExamState("return_exercise");
			}
		}
		
		if(GUI.Button(new Rect(170, 130, 100, 35), "取消", FuncPara.sty_SquareBtn)){
			FuncPara.examConfirm = false;
		}
		
		GUI.skin.label = null;
	}
	
	#region Examing Control
	
	/// <summary>
	/// 考试开始.
	/// </summary>
	public void ExamStart(){
		FuncPara.examScore = 0f;
//		ExamClass.ExamDataAccess(FuncPara.examTitle);  //考试控制信息获取
		ExamClass.ExamDataAccess(MotionPara.taskName);  //考试控制信息获取
		TimeSetting("start"); //考试时间设定
		FuncPara.examStart = true;  //处于考试开始状态
		FuncPara.examWindow = true;  //考试窗口显示控制
	}
	
	/// <summary>
	/// 考试结束.
	/// </summary>
	public void ExamFinish(){
		FuncPara.examStart = false;  //处于考试结束状态
		FuncPara.examWindow = true;  //考试窗口显示控制
		TimeSetting("end");
		ExamClass.ScoreOutput(FuncPara.examTitle);
		st_FuncManager.PopupMsg(FuncPara.examTitle + "考试已完成!");

		//WuHao
		MotionPara.PauseControl = true;
	}
	
	/// <summary>
	/// 考试状态结束.
	/// </summary>
	public void ExamOver(){
		FuncPara.examStart = false;  //处于考试结束状态
		FuncPara.examWindow = false;  //考试窗口结束显示
		TimeSetting("end");
	}
	#endregion
	
	#region Time Para
	
	/// <summary>
	/// 考试时间设置.
	/// </summary>
	/// <param name='setting_msg'>
	/// "start"：开始时间；其他：结束时间.
	/// </param>
	void TimeSetting(string setting_msg){
		if(setting_msg == "start"){
			totalTime = FuncPara.examTime * 60;
			timeString = TimeFormat(totalTime);
		}else{
			timeString = TimeFormat(FuncPara.examTime * 60);
		}
	}
	
	/// <summary>
	/// 根据秒数据转化为00:00:00格式时间显示字符串.
	/// </summary>
	/// <returns>
	/// 格式化的时间显示字符串.
	/// </returns>
	/// <param name='time_value'>
	/// 当前时间，秒为单位.
	/// </param>
	string TimeFormat(float time_value){
		string timeDisplay = "00:00:00";
		string hourString = "";
		int h = Mathf.FloorToInt(time_value / 3600);  //小时
		if(h < 10){
			hourString = "0" + h.ToString();
		}else
			hourString = h.ToString();
		string minuteString = "";
		int m = Mathf.FloorToInt(time_value / 60) % 60;  //分钟
		if(m < 10){
			minuteString = "0" + m.ToString();
		}else
			minuteString = m.ToString();
		string secondString = "";
		int s = Mathf.FloorToInt(time_value - h * 3600 - m * 60);  //秒
		if(s < 10){
			secondString = "0" + s.ToString();
		}else
			secondString = s.ToString();
		timeDisplay = hourString + ":" + minuteString + ":" + secondString;
		return timeDisplay;
	}
	
	#endregion
	
	
	
	// Update is called once per frame
	void Update () {
		if(FuncPara.examWindow){  //考试窗口开启
			if(FuncPara.examStart){  //计时开启
				totalTime -= Time.deltaTime;
				timeString = TimeFormat(totalTime);
				
				//考试时间结束
				if(totalTime <= 0f){
					ExamFinish();
				}
				
			}
		}
		
//		if(Input.GetKeyUp(KeyCode.E)){
//			ExamClass.ExamDataAccess("动模板模框加工");
//		}
//		if(Input.GetKeyUp(KeyCode.R)){
//			ExamClass.ScoreOutput("动模板模框加工");
//		}
	}
	

}
