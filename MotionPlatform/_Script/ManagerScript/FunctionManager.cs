/// <summary>
/// FileName: FunctionManager.cs
/// Author: Jiang Xiaolong
/// Created Time: 2013.02.22
/// Version: 1.0
/// Company: Sunnytech
/// Function: 教练考功能管理脚本
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;



public class FunctionManager : MonoBehaviour {
	
	Introduction st_Introduction;
	TeachWinow st_Teaching;
	CursorMove st_Cursor;

	
	whoru st_Encryption;
	ExamControl st_Exam;
	
	Dictionary<string, string> taskIndex = new Dictionary<string, string>();  //任务索引

	//动画控制
	public MotionManager st_Manager;

	//引出线
	doMoldLable do_mold_label;

	//结构树
	TreeStructure treeStructure;
	
	void Awake () {
		FunctionData.Initialize();  //功能控制初始化
//		gameObject.AddComponent<GUIInitial>();  //GUI参数初始化
		gameObject.AddComponent<HidingMenu>();  //隐匿式菜单
		gameObject.AddComponent<HierarchicalMenu>();  //层级菜单
		GameObject.Find("Main Camera").AddComponent<doMoldLable>();  //引出线!!!一定要加到摄像机上

		do_mold_label = GameObject.Find("Main Camera").GetComponent<doMoldLable>(); 

		gameObject.AddComponent<Introduction>();  //加工中心简介
		st_Introduction = gameObject.GetComponent<Introduction>();
		gameObject.AddComponent<doVoiceExe>();  //语音
		gameObject.AddComponent<TipsWindow>();  //提示窗口
		gameObject.AddComponent<TeachWinow>();  //教学窗口
		st_Teaching = gameObject.GetComponent<TeachWinow>();
		gameObject.AddComponent<CursorMove>();  //鼠标小手移动
		st_Cursor = gameObject.GetComponent<CursorMove>();

		gameObject.AddComponent<RectTest>();  //Todo:测试Rect位置， 发布时省略
//		gameObject.AddComponent<PanelButton>();  //面板按钮执行控制
		
		//新增部分
		gameObject.AddComponent<file>();  //加密模块，用于检索和认证加密txt
		gameObject.AddComponent<dog2>();  //加密模块，加密狗验证
		gameObject.AddComponent<whoru>();  //加密模块，加密界面和入口
		st_Encryption = gameObject.GetComponent<whoru>();
		
		gameObject.AddComponent<Warnning>();  //信息提示窗口
		gameObject.AddComponent<PopupMessage>();  //弹出窗口
		
		gameObject.AddComponent<ComponentsControl>();  //部件提示控制
		
		gameObject.AddComponent<ExamControl>();  //考试控制模块
		st_Exam = gameObject.GetComponent<ExamControl>();	
		

		InitializeData();
		FuncPara.excelPath = Application.dataPath + ConstData.excelPath;

		//动画控制 
		st_Manager = gameObject.GetComponent<MotionManager>();

		//结构树
//		gameObject.AddComponent<TreeStructure>();
		treeStructure = gameObject.GetComponent<TreeStructure>();
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	/// <summary>
	/// 对应任务名和字符串.
	/// </summary>
	private void InitializeData()
	{
		taskIndex.Add("2", "Task1");
		taskIndex.Add("3-0", "3_33");
	}

	
	/// <summary>
	/// 层级菜单触发函数.
	/// </summary>
	/// <param name='cmd_string'>
	/// Command String.
	/// </param>
	public void Hierarchy(string cmd_string){
		Debug.Log(cmd_string);
		
		if(cmd_string == "0"){  //加工中心简介
			//面板消失，摄像机切换到相应位置和角度
			//出现窗口和文字，开始朗读
			//摄像机自动360旋转
			//保留摄像机缩放，机床观察模式切换功能
			//机床保持原来状态
			TaskStop(FuncPara.taskActive);  //当前若有任务，则先停止
			st_Introduction.IntroductionExecute();
			FuncPara.introductionOn = true;
			FuncPara.taskActive = Task.Introduction;
			FuncPara.showTeachWindow = false;  //教和练窗口消失

			//引出线
			do_mold_label.showLabelType("主视图", false);
		}

		if(cmd_string == "1-0"){  //结构认知 -- 主视图
			TaskStop(FuncPara.taskActive);  //当前若有任务，则先停止
			FuncPara.outlineOn = true;
			FuncPara.taskActive = Task.Structrue;  
			FuncPara.showTeachWindow = false;  //教和练窗口消失

			//引出线
			do_mold_label.showLabelType("主视图", true);
		}
		if(cmd_string == "1-1"){  //结构认知 -- 俯视图
			TaskStop(FuncPara.taskActive);  //当前若有任务，则先停止
			FuncPara.outlineOn = true;
			FuncPara.taskActive = Task.Structrue;  
			FuncPara.showTeachWindow = false;  //教和练窗口消失

			//引出线
			do_mold_label.showLabelType("俯视图", true);
		}
		if(cmd_string == "1-2"){  //结构认知 -- 左视图
			TaskStop(FuncPara.taskActive);  //当前若有任务，则先停止
			FuncPara.outlineOn = true;
			FuncPara.taskActive = Task.Structrue;  
			FuncPara.showTeachWindow = false;  //教和练窗口消失

			//引出线
			do_mold_label.showLabelType("左视图", true);
		}
		
		
		if(cmd_string == "2"){  //软件基本操作

			
		}

		if(cmd_string == "3-0"){  //测试案例
			TaskStop(FuncPara.taskActive);  //当前若有任务，则先停止

			FuncPara.taskActive = Task.Task3;

			//加载软件基本操作的列表
			List<string> phaseName = new List<string>();
			ExcelExtraction(phaseName, taskIndex["3-0"]);
			st_Teaching.ActiveTeaching(phaseName.ToArray());
			FuncPara.phaseTitle = phaseName.ToArray();
			FuncPara.examTitle = "教练考测试案例";

			//WuHao -- 初始化和控制
			MotionPara.taskName = "3_33";
			st_Manager.SetLocation(1, "2");
			MotionPara.shouldStop = false;

			//引出线
			do_mold_label.showLabelType("a", false);
		}
	}
	
	//教窗口下拉列表内容提取
	void ExcelExtraction(List<string> phase_name, string excel_name)
	{

		ExcelOperator excelOp = new ExcelOperator();
		string file_path = Application.dataPath + "/Resources/"+ excel_name+"/C/CID";
		DataTable excelTable = excelOp.ExcelReader(file_path, "ID", "A", "B");
		for(int i = 0; i < excelTable.Rows.Count; i++){
			phase_name.Add(Convert.ToString(excelTable.Rows[i][1]));
		}
	}
	
	/// <summary>
	/// 结束当前任务.
	/// </summary>
	/// <param name='current_task'>
	/// Current_task.
	/// </param>
	public void TaskStop(Task current_task)
	{
		if(current_task == Task.None)
			return;
		if(current_task == Task.Introduction)
			st_Introduction.StopIntroduction();
		else if(current_task == Task.Structrue){
			FuncPara.outlineOn = false;
		}
		else if(current_task == Task.BasicOperation){

		}else if(current_task == Task.Task1){

		}else if(current_task == Task.Task2){

		}

		//WuHao -- 结束任务
		else if(current_task == Task.Task3){
			st_Manager.StopButton();
		}
		FuncPara.taskActive = Task.None;
		FuncPara.isPlaying = false;  //播放暂停
		FuncPara.startAlready = false;  //阶段是否已经在播放

	}
	
	/// <summary>
	/// 一个任务播放结束.
	/// </summary>
	public void TaskOver(){
		st_Teaching.TaskOver();
	}
	
	/// <summary>
	/// 隐匿式菜单触发函数.
	/// </summary>
	/// <param name='message_string'>
	/// Message_string.
	/// </param>
	public void HidingMenuExecute(string message_string){
		Debug.Log(message_string);
		
		if(message_string == "Start"){  //开启任务层级菜单
			FuncPara.hierarchicalOn = true;
		}else if(message_string == "language"){  //语言切换
			
		}else if(message_string == "Exam"){  //考试
			if(st_Exam.InputWindowShow()){  //如果已经输入姓名和学号
				if(!FuncPara.examWindow){  //开始开始
					Debug.Log("Exam Start: " + FuncPara.examTitle);
					ExamState("start");  //开启考试	
				}else{  //结束考试
					if(FuncPara.examStart){  //正在考试中，跳出提示菜单
						FuncPara.examConfirm = true;
						st_Exam.returnToExercise = true;  //提交完回到练习模式
					}else{  //直接回到练习模式
						Debug.Log("Exam Return to Exercise: " + FuncPara.examTitle);
						ExamState("return_exercise");  //结束考试
					}
				}
			}
		}else if(message_string == "ComponentTips"){  //部件提示
			FuncPara.componentTips = !FuncPara.componentTips;
		}else if(message_string == "help"){  //帮助信息显示控制
			FuncPara.helpInfo = !FuncPara.helpInfo;
			if(!FuncPara.helpInfo)
				FuncPara.helpString = "";  //关闭时清空字符串
		}else if(message_string == "About"){  //关于我们
			st_Introduction.aboutRect.x = Introduction.RectCentre(st_Introduction.aboutRect, "x");
			st_Introduction.aboutRect.y = Introduction.RectCentre(st_Introduction.aboutRect, "y");
			FuncPara.aboutUsOn = true;
		}else if(message_string == "Exit"){  //退出程序
			FuncPara.exitWindow = true;
		}
		
		else if(message_string == "ShowTree"){ //结构树
			TreeStructure.show = !TreeStructure.show;
		}
	}
	
	/// <summary>
	/// 考试状态设置.
	/// </summary>
	/// <param name='exam_state'>
	/// start:考试考试；return_exercise:返回练习；其他:停止考试
	/// </param>
	public void ExamState(string exam_state){
		if(FuncPara.startAlready){  //教和练过程已启动
			Task tempTask = FuncPara.taskActive;
			TaskStop(FuncPara.taskActive);	
			FuncPara.taskActive = tempTask;
			Debug.Log(FuncPara.taskActive.ToString() + " already begun. Stop this active task first.");
		}
		//Begin examing.
		if(exam_state == "start"){
			if(FuncPara.taskActive == Task.Task3){  //案例1和案例2
				st_Exam.ExamStart();
				FuncPara.showTeachWindow = false;  //教和练窗口消失
				FuncPara.currentMotion = MotionState.Examining;//教练考状态变换
				ExamBtn("exam");//隐匿式菜单切换

				//WuHao
				if (MotionPara.MotionActive) 
				{
					st_Manager.StopButton();
				}
				st_Manager.SetLocation(1, "2");
				MotionPara.shouldStop = false;
				st_Manager.StartCoroutine(st_Manager.MainEntrance(0, 0));
				//WuHao

			}
		//Back to the practice process.
		}else if(exam_state == "return_exercise"){
			st_Exam.ExamOver();
			ExamBtn("exercise");
			FuncPara.currentMotion = MotionState.Teaching;  //返回教的初始状态
			FuncPara.showTeachWindow = true;
//			PhaseSettings(0);

			if(FuncPara.taskActive == Task.Task3){
				//WuHao
				if (MotionPara.MotionActive) 
				{
					st_Manager.StopButton();
				}
				st_Manager.SetLocation(1, "2");
				MotionPara.shouldStop = false;
				st_Manager.StartCoroutine(st_Manager.MainEntrance(0, 0));
				//WuHao
			}
		//Stop the current examing process.
		}else{
			st_Exam.ExamOver();
			ExamBtn("exercise");
		}
	}
	
	/// <summary>
	/// 考试按钮状态设置.
	/// </summary>
	/// <param name='exam_state'>
	/// “exam”为考试状态，其他字符串为非考试状态.
	/// </param>
	public void ExamBtn(string exam_state){
		gameObject.GetComponent<HidingMenu>().ExamBtn(exam_state);
	}

	//考试时跳过步骤执行
	private IEnumerator SkipProcess(int phase_number)
	{
		//WuHao 设置状态
		if (MotionPara.MotionActive) 
		{
			st_Manager.StopButton();
		}
		int tableNum = int.Parse( st_Manager.tableNum);
		int rowNum = int.Parse( st_Manager.rowNum);
		
		if((rowNum+ 1) < (st_Manager.mainRows+2)){
			rowNum += 1;
		
			st_Manager.SetLocation(tableNum, rowNum.ToString());
			MotionPara.shouldStop = false;
			st_Manager.StartCoroutine(st_Manager.MainEntrance(tableNum -1, rowNum-2));
		}
		else if((rowNum+ 1) == (st_Manager.mainRows+2)){
			if((tableNum+1) < (st_Manager.idList.Count+1)){
				tableNum += 1;
				rowNum = 2;
		
				st_Manager.SetLocation(tableNum, rowNum.ToString());
				MotionPara.shouldStop = false;
				st_Manager.StartCoroutine(st_Manager.MainEntrance(tableNum -1, rowNum-2));
			}
		}

		if(phase_number == (ExamClass.StepDiscription.Count - 1)){  //跳过最后一阶段，停止考试
			ExamControl st_Exam = GameObject.Find("MainScript").GetComponent<ExamControl>();
			st_Exam.ExamFinish();
		}
		Voice("跳过步骤：" + ExamClass.StepDiscription[phase_number]);
		TipsWindow(true, "跳过步骤：" + ExamClass.StepDiscription[phase_number], true, "center", WindowColor.Orange);
		yield return new WaitForSeconds(5f);
		TipsWindow(false, "", true, "");
	}	

	/// <summary>
	/// 跳过当前步骤.
	/// </summary>
	public void PassCurrentStep(){

		if(FuncPara.taskActive == Task.Task3){

			int phase_number = ExamClass.CurrentExecuteStep();
			ExamClass.ScoreAccounts(phase_number, false);
			StopAllCoroutines();  //如果有SkipProcess还在运行，停止他
			StartCoroutine(SkipProcess(phase_number));

			//WuHao
//			if (MotionPara.MotionActive) 
//			{
//				st_Manager.StopButton();
//			}
//			int tableNum = int.Parse( st_Manager.tableNum);
//			int rowNum = int.Parse( st_Manager.rowNum);
//
//			if((rowNum+ 1) < (st_Manager.mainRows+2)){
//				rowNum += 1;
//
//				st_Manager.SetLocation(tableNum, rowNum.ToString());
//				MotionPara.shouldStop = false;
//				st_Manager.StartCoroutine(st_Manager.MainEntrance(tableNum -1, rowNum-2));
//			}
//			else if((rowNum+ 1) == (st_Manager.mainRows+2)){
//				if((tableNum+1) < (st_Manager.idList.Count+1)){
//					tableNum += 1;
//					rowNum = 2;
//
//					st_Manager.SetLocation(tableNum, rowNum.ToString());
//					MotionPara.shouldStop = false;
//					st_Manager.StartCoroutine(st_Manager.MainEntrance(tableNum -1, rowNum-2));
//				}
//			}
		}

	}
	
	#region Play Control

	
	#endregion
	
	#region Public Function
	/// <summary>
	/// 摄像机位置调整.
	/// </summary>
	/// <param name='mode'>
	/// Mode.
	/// </param>
//	public void CameraControl(CameraEnum mode){
//		GameObject.Find("Main Camera").GetComponent<camera_sm>().CameraAdjust(mode);
//	}
	
	/// <summary>
	/// 摄像机位置定义.
	/// </summary>
	/// <param name='pos'>
	/// 位置坐标.
	/// </param>
	/// <param name='angle'>
	/// 空间角度.
	/// </param>
	/// <param name='size'>
	/// 正视大小.
	/// </param>
	public void CameraSet(Vector3 pos, Vector3 angle, float size){
//		GameObject.Find("Main Camera").GetComponent<CameraOrth>().CameraSet(pos, angle, size);
	}
	
	/// <summary>
	/// 面板状态控制.
	/// </summary>
	/// <param name='panel_control'>
	/// 方式选择字符串, disappear, showoff, showoffbig, hide, switch, showoffsmall.
	/// </param>
//	public void PanelOperate(string panel_control){
//		if(panel_control == "disappear")
//			gameObject.GetComponent<PanelOperate>().Disappear();
//		
//		if(panel_control == "showoff")
//			gameObject.GetComponent<PanelOperate>().ShowOff();
//		
//		if(panel_control == "showoffbig")
//			gameObject.GetComponent<PanelOperate>().ShowOffBig();
//		
//		if(panel_control == "hide")
//			gameObject.GetComponent<PanelOperate>().Hide();
//		
//		if(panel_control == "switch")
//			gameObject.GetComponent<PanelOperate>().Switch();
//		
//		if(panel_control == "showoffsmall")
//			gameObject.GetComponent<PanelOperate>().ShowOffSmall();
//	}
	
	/// <summary>
	/// 面板状态控制.
	/// </summary>
	/// <param name='panel_control'>
	///  方式选择字符串, showoff, showoffbig, showoffsmall.
	/// </param>
	/// <param name='center_position'>
	/// 面板出现时的中心位置.
	/// </param>
//	public void PanelOperate(string panel_control, Vector2 center_position){
//		if(panel_control == "showoff")
//			gameObject.GetComponent<PanelOperate>().ShowOff(center_position);
//		
//		if(panel_control == "showoffbig")
//			gameObject.GetComponent<PanelOperate>().ShowOffBig(center_position);
//		
//		if(panel_control == "showoffsmall")
//			gameObject.GetComponent<PanelOperate>().ShowOffSmall(center_position);
//	}
	
	/// <summary>
	/// 语音.
	/// </summary>
	/// <param name='voice'>
	/// 要读的字符串，空时停止.
	/// </param>
	public void Voice(string voice_msg)
	{
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不播放语音
			return;
		}else if(FuncPara.speedRate <= 1.01f) //慢速或者1倍速时正常朗读语音
			gameObject.GetComponent<doVoiceExe>().doVoice(voice_msg);
		else{  //加速朗读语音
			string speedStr = (Mathf.FloorToInt(FuncPara.speedRate * 10 - 10) / 2).ToString();
			gameObject.GetComponent<doVoiceExe>().doVoice(voice_msg, speedStr);
		}
	}
	
	/// <summary>
	/// Determines whether this software is encrypted.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is encrypted; otherwise, <c>false</c>.
	/// </returns>
	public bool IsEncrypted(){
		if(st_Encryption.ID == 0){
			st_Encryption.check();
			return true;
		}else 
			return false;
	}
	
	/// <summary>
	/// Determines whether this software is encrypted，用于进度条界面在加密情况下的判断.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is encrypted the specified current_phase; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='current_phase'>
	/// If set to <c>true</c> 当前段.
	/// </param>
	public bool IsEncrypted(int current_phase){
		if(st_Encryption.ID == 0){
			if(FuncPara.taskActive == Task.BasicOperation){  //基本操作
				return false;
			}else if(FuncPara.taskActive == Task.Task1){  //案例1
				if(current_phase == 0){
					return true;
				}else if(current_phase < 2){//Todo:起始阶段，自己根据需要确定
					return true;
				}else if(current_phase >= 2 && current_phase <= 5){//Todo:起始和终止阶段，自己根据需要确定
					return false;
				}else{
					return true;
				}
			}else if(FuncPara.taskActive == Task.Task2){  //案例2
				if(current_phase == 0){
					return true;
				}else if(current_phase < 2){//Todo:起始阶段，自己根据需要确定
					return true;
				}else if(current_phase >= 2 && current_phase <= 5){//Todo:起始和终止阶段，自己根据需要确定
					return false;
				}else{
					return true;
				}
			}
			else if(FuncPara.taskActive == Task.Task3){  //案例3
				if(current_phase == 0){
					return true;
				}else if(current_phase < 2){//Todo:起始阶段，自己根据需要确定
					return true;
				}else if(current_phase >= 2 && current_phase <= 5){//Todo:起始和终止阶段，自己根据需要确定
					return false;
				}else{
					return true;
				}
			}
			else{
				return true;
			}	
		}else{
			return false;
		}
	}
	
	/// <summary>
	/// Determines whether this software is encrypted，用于播放时加密情况的判断.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is encrypted the specified current_phase phase_number; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='current_phase'>
	/// If set to <c>true</c> 当前段.
	/// </param>
	/// <param name='phase_number'>
	/// If set to <c>true</c> 设置段.
	/// </param>
	public bool IsEncrypted(int current_phase, ref int phase_number){
		if(st_Encryption.ID == 0){
			if(FuncPara.taskActive == Task.BasicOperation){  //基本操作
				return false;
			}else if(FuncPara.taskActive == Task.Task1){  //案例1
				if(current_phase == 0){
					phase_number = 2; //Todo:起始阶段，自己根据需要确定
					return false;
				}else if(current_phase < 2){//Todo:起始阶段，自己根据需要确定
					st_Encryption.check();
					return true;
				}else if(current_phase >= 2 && current_phase <= 5){//Todo:起始和终止阶段，自己根据需要确定
					return false;
				}else{
					st_Encryption.check();
					return true;
				}
			}else if(FuncPara.taskActive == Task.Task2){  //案例2
				if(current_phase == 0){
					phase_number = 2; //Todo:起始阶段，自己根据需要确定
					return false;
				}else if(current_phase < 2){//Todo:起始阶段，自己根据需要确定
					st_Encryption.check();
					return true;
				}else if(current_phase >= 2 && current_phase <= 5){//Todo:起始和终止阶段，自己根据需要确定
					return false;
				}else{
					st_Encryption.check();
					return true;
				}
			}else{
				st_Encryption.check();
				return true;
			}	
		}else{
			return false;
		}
	}
	
	/// <summary>
	/// 警告提示信息窗口显示.
	/// </summary>
	/// <param name='warnning_msg'>
	/// 要显示的字符串.
	/// </param>
	public void WarnningMsg(string warnning_msg){
		gameObject.GetComponent<Warnning>().AddInfo(warnning_msg);
	}
	
	/// <summary>
	/// 弹出窗口显示.
	/// </summary>
	/// <param name='popup_msg'>
	/// 要显示的字符串.
	/// </param>
	public void PopupMsg(string popup_msg){
		gameObject.GetComponent<PopupMessage>().Popup(popup_msg);
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, Vector2 target_position){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow, target_position);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, string position_string){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow, position_string);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, WindowColor window_color){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow, window_color);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置.
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, Vector2 target_position, WindowColor window_color){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow, target_position, window_color);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, string position_string, WindowColor window_color){
		if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
			FuncPara.tipsWindow = false;
		}else if(window_on)
			gameObject.GetComponent<TipsWindow>().RectAdjust(tips_string, move_allow, position_string, window_color);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 帮助信息格式化.
	/// </summary>
	public void HelpFormat(){
		gameObject.GetComponent<TipsWindow>().HelpInfoFormat();
	}
	
	/// <summary>
	/// 光标移动的速度调整.
	/// </summary>
	/// <param name='set_rate'>
	/// 所要设置的速率.
	/// </param>
	/// <param name='origin_rate'>
	/// 原始速率.
	/// </param>
	public void ChangeRateCursor(float set_rate, float origin_rate){
		st_Cursor.ChangeRate(set_rate, origin_rate);
	}
	
	/// <summary>
	/// 光标移动.
	/// </summary>
	/// <param name='start_position'>
	/// Start_position.
	/// </param>
	/// <param name='end_position'>
	/// End_position.
	/// </param>
	public float CursorLine(Vector2 start_position, Vector2 end_position){
//		return st_Cursor.MovingStart(start_position, end_position);
		return 0;
	}
	
	/// <summary>
	/// 光标旋转.
	/// </summary>
	/// <param name='start_position'>
	/// 起始点位置.
	/// </param>
	/// <param name='center_point'>
	/// 旋转中心位置.
	/// </param>
	/// <param name='cw'>
	/// false：逆时针；true：顺时针.
	/// </param>
	public void CursorRotate(Vector2 start_position, Vector2 center_point, bool cw){
//		st_Cursor.RotateStart(start_position, center_point, cw);
	}
	
	/// <summary>
	/// 光标移动停止.
	/// </summary>
	public void CursorStop(){
		st_Cursor.MovingStop();
	}
	
	/// <summary>
	/// 光标旋转停止，但是光标未消失.
	/// </summary>
	public void CursorStopRotate(){
		st_Cursor.RotateStop();
	}
	
	/// <summary>
	/// 启动右键菜单.
	/// </summary>
	/// <param name='switch_on'>
	/// 是否开启.
	/// </param>
	/// <param name='position'>
	/// 显示位置.
	/// </param>
//	public void RightClickOn(bool switch_on, Vector2 position){
//		GameObject.Find("MainScript").GetComponent<RightclickMenu>().RightClickControl(switch_on, position);
//	}
	
	/// <summary>
	/// 阶段设置.
	/// </summary>
	/// <param name='phase_number'>
	/// 阶段号.
	/// </param>
	public void PhaseSettings(int phase_number){
		st_Teaching.currentPhase = phase_number;
		st_Teaching.selected_Button = phase_number + 1;
	}
	
	/// <summary>
	/// 面板按钮执行.
	/// </summary>
	/// <param name='btn_name'>
	/// 按钮名字代号.
	/// power_on, power_off;
	/// 
	/// </param>
	public void ButtonExecute(string btn_name){
//		gameObject.GetComponent<PanelButton>().ButtonExecute(btn_name);
	}
	
	/// <summary>
	/// 根据面板当前位置，返回指定按钮所对应的屏幕位置.
	/// </summary>
	/// <returns>
	/// 屏幕X和Y值.
	/// </returns>
	/// <param name='btn_name'>
	/// 按钮名字代号.
	/// power_on, power_off;
	/// 
	/// </param>
	public Vector2 ButtonLocation(string btn_name){
//		return gameObject.GetComponent<PanelButton>().ButtonLocation(btn_name);
		return new Vector2(0,0);
	}
	
	/// <summary>
	/// 设置TeachWindow位置.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void TeachWindowPos(Vector2 position){
		st_Teaching.teach_Window.x = position.x;
		st_Teaching.teach_Window.y = position.y;
	}
	
	/// <summary>
	/// 播放下一阶段.
	/// </summary>
	/// <param name='next_current'>
	/// false-现阶段，true-下一阶段.
	/// </param>
	public void PlayNextStage(bool next_current){
		st_Teaching.PlayNextStage(next_current);
	}
	
	
	#endregion
	
}
