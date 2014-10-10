/// <summary>
/// FileName: HidingMenu.cs
/// Author: Original: Lu Liu 
/// Created: 2102
/// Version: 1.0
/// Company: Sunnytech
/// Function: 隐匿式菜单
///
/// Changed By: Jiang Xiaolong
/// Modification Time: 2014.02.19
/// Discription: 简化
/// <summary>
using UnityEngine;
using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

public class HidingMenu : MonoBehaviour {
	
	FunctionManager st_FuncManager;
	
	public bool showTopMenu = true;  //使用上方的隐匿式菜单
	public bool showRightMenu = true;  //使用右侧的隐匿式菜单
	public float showSpeed = 15f;  //菜单显示速度
	float topSpeed = 0;
	float rightSpeed = 0;
	float topMin = 0;
	float rightMin = 0;
	float topMax = 0;
	float rightMax = 0;
	string excelPath;  //配置Excel所在位置
	string fileContent;  //图片所在目录
	float topWidth = 54f;  //上方按钮宽度
	float topHeight = 66f;  //上方按钮高度
	float rightWidth = 54f;  //右侧按钮宽度
	float rightHeight = 66f;  //右侧按钮高度
	float topPosition = 10f;  //上方按钮起始位置
	float rightPosition = 10f;  //右侧按钮起始位置
	int topCount = 10;  //上方按钮数量
	int rightCount = 10;  //右侧按钮数量
	float dynamicHeight = 0;    
	float dynamicWidth = 0;
	bool topDisplay = false;
	bool rightDisplay = false;
	Rect topRect = new Rect(0,0,0,0);
	Rect rightRect = new Rect(0,0,0,0);
	Texture2D[]topIcon1;
	Texture2D[]topIcon2;
	Texture2D[]topIcon3;  //用于失效图标的显示
	List<int> topInvalid = new List<int>();  //自动记录失效的按钮位置
	Texture2D[]topIconNow;
	Texture2D[]rightIcon1;
	Texture2D[]rightIcon2;
	Texture2D[]rightIcon3;  //用于失效图标的显示
	List<int> rightInvalid = new List<int>();  //自动记录失效的按钮位置
	Texture2D[]rightIconNow;
	string[] topMessage;
	string[] rightMessage;
	Dictionary<string, int> iconChange = new Dictionary<string, int>();  //用于判断单击时是否需要更换图片
	
	Dictionary<string, string> helpMessage = new Dictionary<string, string>();  //用于帮助信息汇集
	
	void Awake () {
		st_FuncManager = gameObject.GetComponent<FunctionManager>();
		showRightMenu = true;  //禁用右侧菜单
		showSpeed = 15f;  //设定菜单伸展速率
		excelPath = Application.dataPath + ConstData.excelSettingPath + ConstData.hideMenu;
		fileContent = "Texture/MenuIcon/";
		FuncPara.hiMenuCornerW = topPosition;
		FuncPara.hiMenuCornerH = topHeight;
	}
	
	// Use this for initialization
	void Start () {
		//贴图资源初始化
		if(ConfigData.topMenuOn)
			showTopMenu = true;
		else
			showTopMenu = false;
		if(ConfigData.rightMenuOn)
			showRightMenu = true;
		else
			showRightMenu = false;
		TextureInitialze(excelPath);
		topSpeed = 0;
		topMin = (-1) * topHeight;
		topMax = 0;
		rightSpeed = 0;
		rightMin = 0;
		rightMax = rightWidth;
		dynamicWidth = 0;
		dynamicHeight = (-1) * topHeight;
	}
	
	
	void OnGUI () {
		GUI.skin.button = FuncPara.defaultSkin.button;
		//显示上方按钮
		if(showTopMenu){
			for (int i = 0; i < topCount; i++) {
				if(topInvalid.IndexOf(i) == -1){
					if(GUI.Button(new Rect(topPosition + topWidth * i, dynamicHeight, topWidth, topHeight), 
						new GUIContent(topIconNow[i], "top" + (i + 1).ToString()))){
						if(topMessage[i] == "Exam"){
							if(st_FuncManager.IsEncrypted()){  //加密时无触发
								return;
							}
							if(FuncPara.taskActive == Task.Task1 || FuncPara.taskActive == Task.Task2 || FuncPara.taskActive == Task.Task3){  //有考试模块时才启动考试判断
								//执行程序
								st_FuncManager.HidingMenuExecute(topMessage[i]);  //直接执行，不更换按钮
								return;
							}else
								return;
						}
						//有需要变换的图片处理
						if(iconChange.ContainsKey(topMessage[i])){
							if(iconChange[topMessage[i]] == 1){
								topIconNow[i] = topIcon2[i];
								iconChange[topMessage[i]] = 2;
							}else{
								topIconNow[i] = topIcon1[i];
								iconChange[topMessage[i]] = 1;
							}
						}
						//执行程序
						st_FuncManager.HidingMenuExecute(topMessage[i]);
					}
				}else{
					GUI.Button(new Rect(topPosition + topWidth * i, dynamicHeight, topWidth, topHeight), 
						new GUIContent(topIcon3[i], "top" + (i + 1).ToString()));
				}
			}
		}

		//显示右侧按钮
		if(showRightMenu){
			for (int i = 0; i < rightCount; i++) {	
				if(rightInvalid.IndexOf(i) == -1){
					if(GUI.Button(new Rect(Screen.width - dynamicWidth, rightPosition + rightHeight * i, rightWidth, rightHeight), 
					new GUIContent(rightIconNow[i], "right" + (i + 1).ToString()))) {
						//有需要变换的图片处理
						if(iconChange.ContainsKey(rightMessage[i])){
							if(iconChange[rightMessage[i]] == 1){
								rightIconNow[i] =  rightIcon2[i];
								iconChange[rightMessage[i]] = 2;
							}else{
								rightIconNow[i] = rightIcon2[i];
								iconChange[rightMessage[i]] = 1;
							}
						}
						//执行程序
						st_FuncManager.HidingMenuExecute(rightMessage[i]);
					}
				}else{
					GUI.Button(new Rect(Screen.width - dynamicWidth, rightPosition + rightHeight * i, rightWidth, rightHeight), 
					new GUIContent(rightIcon3[i], "right" + (i + 1).ToString()));
				}
			}
		}
		
		//帮助信息控制
		if (topRect.Contains(Input.mousePosition) || rightRect.Contains(Input.mousePosition)) {  //帮助信息控制
			FuncPara.helpHiding = true;
			if(FuncPara.helpInfo){  //帮助信息显示控制
				if(showTopMenu){  //上部菜单
					for (int i = 0; i < topCount; i++) {
						if(topInvalid.IndexOf(i) == -1){
							if(new Rect(topPosition + topWidth * i, dynamicHeight, topWidth, topHeight).Contains(Event.current.mousePosition)){
								if(FuncPara.helpString != helpMessage[topMessage[i]]){
									FuncPara.helpString = helpMessage[topMessage[i]];
									st_FuncManager.HelpFormat();
									FuncPara.helpDisplay = true;
								}
								break;
							}
						}
					}
				}
				
				if(showRightMenu){  //右侧菜单
					for (int i = 0; i < rightCount; i++) {
						if(rightInvalid.IndexOf(i) == -1){
							if(new Rect(Screen.width - dynamicWidth, rightPosition + rightHeight * i, rightWidth, rightHeight).Contains(Event.current.mousePosition)){
								if(FuncPara.helpString != helpMessage[rightMessage[i]]){
									FuncPara.helpString = helpMessage[rightMessage[i]];
									st_FuncManager.HelpFormat();
									FuncPara.helpDisplay = true;
								}
								break;
							}
						}
					}
				}
			}
		}else{
			FuncPara.helpHiding = false;
			if(!FuncPara.helpHiding && !FuncPara.helpProgress){  //两边都为Flase时触发消失
				FuncPara.helpString = "";
				FuncPara.helpDisplay = false;
			}
		}
		
		GUI.skin = null;
	}
	
	// Update is called once per frame
	void Update () {
		//上方按钮的显示控制
		if(showTopMenu){
			if (Input.mousePosition.y >= Screen.height - 1) {	
				if (Mathf.Approximately(dynamicHeight, (-1 * topHeight)))	
					topDisplay = true;
			}
			if(topDisplay) {
				topSpeed += 0.01f * showSpeed;
				dynamicHeight = Mathf.Lerp (topMin, topMax, topSpeed);
				if (Mathf.Approximately (dynamicHeight, topMax)) {
					topDisplay = false;
					topMax = topMin;
					topMin = dynamicHeight;
					topSpeed = 0;
				}
			}
		}
		
		//右侧按钮的显示控制
		if(showRightMenu){
			if (Input.mousePosition.x >= Screen.width - 1) {	
				if (Mathf.Approximately (dynamicWidth, 0))				
					rightDisplay = true;
			}
			if (rightDisplay) {
				rightSpeed += 0.01f * showSpeed;
				dynamicWidth = Mathf.Lerp (rightMin, rightMax, rightSpeed); 
				if (Mathf.Approximately (dynamicWidth, rightMax)) {
					rightDisplay = false;
					rightMax = rightMin;
					rightMin = dynamicWidth;
					rightSpeed = 0;
				}
			}
		}
		
		//隐藏菜单
		if (topRect.Contains(Input.mousePosition) || rightRect.Contains(Input.mousePosition)) { 
			//点击按钮时不隐藏
		}else{
			if (Input.GetMouseButtonDown(0)) {					
				if (Mathf.Approximately(dynamicHeight, 0)) {										
					topDisplay = true;						
				}
				if (Mathf.Approximately (dynamicWidth, rightWidth)) {									
					rightDisplay = true;											
				}			
			}
		}
		
		//动态计算按钮所在区域
		if(topDisplay)
		{
			topRect.x = topPosition;
			topRect.width = topWidth * topCount;
			topRect.height = topHeight;
			topRect.y = Screen.height - dynamicHeight - topHeight;
		}
		if(rightDisplay)
		{
			rightRect.y = Screen.height - rightPosition - rightHeight * rightCount;
			rightRect.x = Screen.width - dynamicWidth;
			rightRect.width = rightWidth;
			rightRect.height = rightHeight * rightCount;
		}
	}
	
	//读取Excel表中贴图信息
	bool TextureInitialze(string file_name)
	{
		ExcelOperator excelClass = new ExcelOperator();
		//上方菜单参数提取
		if(showTopMenu){
			DataTable topData = new DataTable("topTable");
			topData = excelClass.ExcelReader(file_name, "UP");
			//List变量暂时存储读取的值以实现icon是否加载的动态判断
			List<Texture2D> topIcon1List = new List<Texture2D>();
			List<Texture2D> topIcon2List = new List<Texture2D>();
			List<Texture2D> topIcon3List = new List<Texture2D>();
			List<Texture2D> topIconNowList = new List<Texture2D>();
			List<string> topMessageList = new List<string>();
			bool isAdding = false;  //是否启用
			int tempIndex = -1;  //List序号控制
			try{
				for(int i = 0; i < topData.Rows.Count; i++){
					string tempMsg = (string)topData.Rows[i][2];
					if(ConfigData.topMenuState.ContainsKey(tempMsg)){
						if(ConfigData.topMenuState[tempMsg])
							isAdding = true;
						else
							isAdding = false;
					}else{
						isAdding = true;  //程序员新增按钮
					}
					if(isAdding){
						tempIndex++;
						topIcon1List.Add((Texture2D)Resources.Load(fileContent + (string)topData.Rows[i][0]));
						topIconNowList.Add(topIcon1List[tempIndex]);
						topIcon2List.Add((Texture2D)Resources.Load(fileContent + (string)topData.Rows[i][1]));
						topMessageList.Add(tempMsg);
						if(topIcon1List[tempIndex] != topIcon2List[tempIndex]){
							iconChange.Add(tempMsg, 1);
						}
						if((string)Convert.ToString(topData.Rows[i][3]) != ""){
							topIcon3List.Add((Texture2D)Resources.Load(fileContent + (string)topData.Rows[i][3]));
							topInvalid.Add(tempIndex);
						}
						helpMessage.Add(tempMsg, Convert.ToString(topData.Rows[i][4]));  //载入帮助提示信息
					}
				}
			}catch{
				Debug.LogError("提取上方菜单参数时出错，请检查相应的Excel表格!");
				return false;
			}
			topIcon1 = topIcon1List.ToArray();
			topIcon2 = topIcon2List.ToArray();
			topIcon3 = topIcon3List.ToArray();
			topIconNow = topIconNowList.ToArray();
			topMessage = topMessageList.ToArray();
			topCount = topIcon1List.Count;
		}
		
		//右侧菜单参数提取
		if(showRightMenu){
			DataTable rightData = new DataTable("rightTable");
			rightData = excelClass.ExcelReader(file_name, "RIGHT");
			//List变量暂时存储读取的值以实现icon是否加载的动态判断
			List<Texture2D> rightIcon1List = new List<Texture2D>();
			List<Texture2D> rightIcon2List = new List<Texture2D>();
			List<Texture2D> rightIcon3List = new List<Texture2D>();
			List<Texture2D> rightIconNowList = new List<Texture2D>();
			List<string> rightMessageList = new List<string>();
			bool isAdding = false;  //是否启用
			int tempIndex = -1;  //List序号控制
			try{
				for(int i = 0; i < rightData.Rows.Count; i++){
					string tempMsg = (string)rightData.Rows[i][2];
					if(ConfigData.rightMenuState.ContainsKey(tempMsg)){
						if(ConfigData.rightMenuState[tempMsg])
							isAdding = true;
						else
							isAdding = false;
					}else{
						isAdding = true;  //程序员新增按钮
					}
					if(isAdding){
						tempIndex++;
						rightIcon1List.Add((Texture2D)Resources.Load(fileContent + (string)rightData.Rows[i][0]));
						rightIconNowList.Add(rightIcon1List[tempIndex]);
						rightIcon2List.Add((Texture2D)Resources.Load(fileContent + (string)rightData.Rows[i][1]));
						rightMessageList.Add(tempMsg);
						if(rightIcon1List[tempIndex] != rightIcon2List[tempIndex]){
							iconChange.Add(tempMsg, 1);
						}
						if((string)Convert.ToString(rightData.Rows[i][3]) != ""){
							rightIcon3List.Add((Texture2D)Resources.Load(fileContent + (string)rightData.Rows[i][3]));
							rightInvalid.Add(tempIndex);
						}
						helpMessage.Add(tempMsg, Convert.ToString(rightData.Rows[i][4]));  //载入帮助提示信息
					}
				}
			}catch{
				Debug.LogError("提取右侧菜单参数时出错，请检查相应的Excel表格!");
				return false;
			}
			rightIcon1 = rightIcon1List.ToArray();
			rightIcon2 = rightIcon2List.ToArray();
			rightIcon3 = rightIcon3List.ToArray();
			rightIconNow = rightIconNowList.ToArray();
			rightMessage = rightMessageList.ToArray();
			rightCount = rightIcon1List.Count;
		}
		//About窗口内容，暂不开放到Excel表格中，由程序员固定填写
		FuncPara.aboutString = "《数控加工中心仿真软件》由浙大旭日科技和诸暨凯达机床厂合作开发。" +
			"我们专业为各院校、公司提供各类教学资源开发服务，包括虚拟教学软件、教学资源库，教材等。" +
			"所开发的教学资源具有技术领先、特色鲜明、使用方便、效果突出的特点，是院校专业建设的最佳合作伙伴。\n" + 
			"购买咨询：0571-28811226, market01@sunnytech.cn";
		//DataTable aboutData = new DataTable("About");
		//aboutData = excelClass.ExcelReader(file_name, "ABOUT");
		//FuncPara.aboutString = Convert.ToString(aboutData.Rows[0][1]);
		return true;
	}
	
	/// <summary>
	/// 考试按钮状态设置.
	/// </summary>
	/// <param name='exam_state'>
	/// “exam”为考试状态，其他字符串为非考试状态.
	/// </param>
	public void ExamBtn(string exam_state){
		for(int i = 0; i < topCount; i++) {
			if(topMessage[i] == "Exam"){
				if(exam_state == "exam"){  //切换到考试状态
					topIconNow[i] = topIcon2[i];
					iconChange[topMessage[i]] = 2;
				}else{  //切换到练习状态
					topIconNow[i] = topIcon1[i];
					iconChange[topMessage[i]] = 1;
				}
				break;
			}
		}
	}
	
}
