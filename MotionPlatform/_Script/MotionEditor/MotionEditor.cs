using UnityEngine;
using System.Collections;
using System.Data;
using System.Data.Odbc;

public class MotionEditor : MonoBehaviour {


	public static bool show;
	//显示添加数据窗口
	bool showAddWindow;
	//选择添加类型
	bool showAddSelection;
	//添加Main
	bool addMain1;
	bool addMain2;
	int addMainRow;

	[HideInInspector]
	public Rect ExcelWriteWindow;
	Rect ExcelAddWindow;
	float btn_width = 100f;
	float btn_height = 30f;
	
	string[] SheetNames;
	int selectSheet;
	int old_selectSheet;
	System.Data.DataTable sheetTable;
	string filepath;

	//当前sheet中内容
	int sheet_rows; //行数
	int sheet_columns; //列数
	string[,] contents;
	//要添加的行
	string[] addContent;
	//列名
	string[] columnName;
	int key;
	
	//滚动条位置
	float hSbarValue;
	float vSbarValue;
	float sheet_width;
	float sheet_height;
	float add_sheet_width;
	float add_hSbarValue;
	//空行提醒
	string nullColumns;

	//数据类型
	string[] cameraCategory;
	string[] excelCategory;
	string[] maxCategory;
	string[] programCategory;
	string[,] programCategory_content;
	int selectCategory;
	int old_selectCategory;
	string[] tmp_TextFields;
	bool[] selectToggles;
	int add_i;
	//保存Excel临时数据
	string[] tmpExcelData;
	int tmpExcelSelect;
	bool excelToGroup;
	//保存Group临时数据
	string[] tmpProgramData;
	int tmpProgramSelect;
	bool programToGroup;

	//焦点控件
	string focusControl;

	//关闭
	Texture2D tex_CloseButton;

	//Trigger Type
	bool showTriggerType;
	string[] triggerType;
	//Trigger Key
	bool showTriggerKey;
	string[] triggerKey_Mouse;

	public static string mainCamera = "Main Camera";

	bool isControl;
	
	void Start () {
		ExcelWriteWindow = new Rect(100,100,1000,600);
		ExcelAddWindow = new Rect(100,100,1000,600);

		show = false;

		selectSheet = -1;
		SheetNames = new string[] {"MAIN", "CAMERA", "TIPS", "EXCEL", "3DMAX", "PROGRAM", "TRIGGER", "Group"};

		key = 0;
		filepath = UnityEngine.Application.dataPath + "/Resources/" + MotionPara.taskName + "/C/" + "C1";

		nullColumns = "";
		focusControl = "";

		selectCategory = -1;
		cameraCategory = new string[] {"相机直线运动", "相机旋转运动"};
		excelCategory = new string[] {"平移", "旋转", "平移加旋转", "任意移动", "瞬间移动"};
		maxCategory = new string[] {"不包含PC2的3dMax动画", "包含PC2的3dMax动画"};
		programCategory = new string[] {"铜棒敲击", "部件摆放或工具移出", "螺钉拧松或拧紧", "螺钉拧出或拧进"};
		programCategory_content = new string[,]{
			{"运动物体组", "敲击次数", "铜棒离敲击点距离", "被敲击物体移动的方向,以Group第一个物体为参考", "铜棒敲击的速度,选填", "敲击一次前进距离,选填","", "", ""},
			{"运动物体组", "参考物体", "相对坐标,工具移出不需要填", "相对角度,工具移出不需要填", "移动速度,选填", "自定义安全高度,绝对坐标", "", "", ""},
			{"一起运动的组,扳手名称", "参考物体,所拧的螺钉", "拧的次数", "扳手与螺钉距离", "'0,0,1'表示拧松，逆时针;'0,0，-1'表示拧紧，顺时针", "拧的角度,填60或者120", "移动速度,选填", "扳手抬出位移,选填", "扳手移动轴默认-1,0,0,选填"},
			{"一起运动的组,扳手名称", "参考物体,所拧的螺钉", "扳手与螺钉距离", "'0,0,1'表示拧松，逆时针;'0,0，-1'表示拧紧，顺时针", "螺钉所需拧出距离,用两点之差的向量表示", "移动速度,选填", "扳手移动轴默认0,-1,0,选填", "", ""}
		};

		tex_CloseButton = (Texture2D)Resources.Load("Texture/CustomMenu/关闭1");

		triggerType = new string[]{"MOUSEDOWN", "MOUSEUP", "MOUSEDOWN2", "KEYUP", "BUTTON", "SCROLLWHEEL", "KEYDOWN"};
		triggerKey_Mouse = new string[]{"0", "1", "2"};
	}

	public void setExcel(string excelName){
		filepath = excelName;

		selectSheet = -1;

		showAddWindow = false;

		excelToGroup = false;
		programToGroup = false;
	}

	void setColumnName(string sheetname){
		if(sheetname == "MAIN"){
			columnName = new string[]{"序号及描述:0", "ID", "摄像机运动:2", "语音及提示文字:3", "运动:4", "触发选项:5", "进度条控制信息:6"};
		}
		else if(sheetname == "CAMERA"){
			columnName = new string[]{"运动描述:0", "ID", "摄像机名称:2", "观察物体:3", "相对位置参考物体:4", "初始位置(不填代表当前位置):5", "目标位置:6", "视域大小变化初始值(可不填):7", 
				"视域大小变化目标值(可不填):8", "旋转参考物体:9", "旋转轴:10", "旋转角度：11", "自定义直线速度(可不填):12", "自定义角速度(可不填):13"};
		}
		else if(sheetname == "TIPS"){
			columnName = new string[]{"ID", "文本信息:1", "显示位置(默认down_left):2", "是否为标题（默认false）:3", "是否可移动（默认false）:4"};
		}
		else if(sheetname == "EXCEL"){
			columnName = new string[]{"描述", "ID", "运动物体的组号", "运动类型", "加速移动的最终速度", "移动初始速度", "移动向量", "旋转轴", "加速旋转的最终速度", "旋转初始速度", "旋转角度", "参考物体", "相对坐标", "相对角度"};
		}
		else if(sheetname == "3DMAX"){
			columnName = new string[]{"动画描述:0", "ID", "动画所在物体名称:2", "动画名字:3", "自定义动画播放时长设置:4(默认可不填)", "PC2动画的物体名", "PC2动画的起始帧", "PC2动画的结束帧"};
		}
		else if(sheetname == "PROGRAM"){
			columnName = new string[]{"运动描述", "ID", "ProgramID", "第1个参数", "第2个参数", "第3个参数", "第4个参数", "第5个参数", "第6个参数", "第7个参数", "第8个参数", "第九个参数"};
		}
		else if(sheetname == "TRIGGER"){
			columnName = new string[]{"触发描述:0", "ID", "触发类型标示符:2", "触发按钮:3", "触发物体:4"};
		}
		else if(sheetname == "Group"){
//			columnName = new string[]{"触发描述:0", "ID", "触发类型标示符:2", "是否为全局触发:3", "触发物体:4"};
		}
	}

	//更新当前sheet中数据,所有记录
	void updateData(string filepath, string sheetName, string[,] contents){
		
		ExcelOperator ewo = new ExcelOperator();
		ewo.UpdateData(filepath, sheetName, contents, key);
		
		//更新
		getSheetData(filepath,sheetName);
	}
	//更新当前sheet中数据，单条记录
	void updateData(string filepath, string sheetName, string[] contents){

		ExcelOperator ewo = new ExcelOperator();
		ewo.UpdateData(filepath, sheetName, contents, key);
		
		//更新
		getSheetData(filepath,sheetName);
	}
	
	//向当前sheet中添加数据
	void addData(string filepath, string sheetName, string[] addContent){
		ExcelOperator ewo = new ExcelOperator();
		ewo.AddData(filepath, sheetName, addContent);
		
		//更新
		getSheetData(filepath, sheetName);
	}
	
	//删除当前sheet中数据
	void deleteData(string filepath, string sheetName, string keyValue, int rowIndex){
		ExcelOperator ewo = new ExcelOperator();
		ewo.DeleteData(filepath, sheetName, keyValue, rowIndex, key);
		
		//更新数据
		getSheetData(filepath, sheetName);
	}


	//获取sheet中数据
	void getSheetData(string filepath,string sheetname){
		ExcelOperator excelReader = new ExcelOperator();
		sheetTable = excelReader.ExcelReader(filepath + ".xls", sheetname);

		sheet_rows = sheetTable.Rows.Count;
		sheet_columns = sheetTable.Columns.Count;

		for(int i = 0; i< sheetTable.Columns.Count; i++){//获得PrimiryKey的列数,即列名为ID的列数
			DataColumn dc = sheetTable.Columns[i];
			if(dc.ColumnName == "ID"){
				key = i;
				break;
			}
		}

		nullColumns = "当前表的空行总数：";
		int tmp = 0;
		string tmpSting = "空行：";
		for(int i = 0; i < sheetTable.Rows.Count; i++){ //获得sheet行数
			DataRow dr = sheetTable.Rows[i];
			if(dr[key].ToString() == null || dr[key].ToString() == ""){
				tmp++;
				tmpSting += (i+2).ToString()+ ", ";
			}
		}
		nullColumns += tmp.ToString();
		if(tmp > 0){
			nullColumns += "; "+ tmpSting;
		}

		contents = new string[sheet_rows, sheetTable.Columns.Count];
		addContent = new string[sheetTable.Columns.Count];
		columnName = new string[sheetTable.Columns.Count];

		for(int i = 0; i < sheet_rows; i++){//获得sheet中数据
			DataRow dr = sheetTable.Rows[i];
			for(int j = 0; j < sheetTable.Columns.Count; j++){
				contents[i , j] = dr[j]. ToString();
			}
		}

		for(int j = 0; j < sheetTable.Columns.Count; j++){
			addContent[j] = "";
			columnName[j] = sheetTable.Columns[j].ColumnName;
		}
		//设置列名
		setColumnName(sheetname);

		hSbarValue = 0;
		vSbarValue = 0;
	}

	//显示当前sheet中数据
	void showSheet(string filepath,string sheetname){
		if(old_selectSheet != selectSheet){
			getSheetData(filepath, sheetname);
			showAddWindow = false;
			old_selectCategory = 0;

			showAddSelection = false;
			addMain1 = false;
			addMain2 = false;
		}

		//空行提醒
		nullColumns = GUI.TextField(new Rect(10 + SheetNames.Length * 80 ,20, btn_width*2, btn_height), nullColumns);

		//滚动区域
		sheet_width = (10+ sheet_columns* btn_width+ btn_width) - ExcelWriteWindow.width;
		if(addMain1){
			sheet_width += btn_width;
			if(addMain2){
				sheet_width += btn_width;
			}
		}
		sheet_height = (60+ sheet_rows* btn_height+ btn_height+ 60) - ExcelWriteWindow.height;
		if(addMain1){
			float sheet_height_2 = (60+ addMainRow* btn_height+ 3*btn_height+ 30) - ExcelWriteWindow.height;
			if(sheet_height_2 > sheet_height){
				sheet_height = sheet_height_2;
			}
			if(addMain2){
				float sheet_height_3 = (60+ addMainRow* btn_height+ 7*btn_height+ 30) - ExcelWriteWindow.height;
				if(sheet_height_3 > sheet_height){
					sheet_height = sheet_height_3;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.P)){
			Debug.Log("sheet_width:"+ sheet_width+ ",sheet_height:"+ sheet_height+",hSbarValue:"+hSbarValue+",vSbarValue:"+vSbarValue);
		}
		bool hor = (sheet_width < 0);
		bool ver = (sheet_height < 0);
		if(!hor){
			GUI.depth = 0;
			hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelWriteWindow.height-20, ExcelWriteWindow.width-20, 30), hSbarValue, 1.0F, 0.0F, 10.0F);
			GUI.depth = 1;
		}
		if(!ver){
			GUI.depth = 0;
			vSbarValue = GUI.VerticalScrollbar (new Rect(ExcelWriteWindow.width-20, 10, 30, ExcelWriteWindow.height-50), vSbarValue, 1.0F, 0.0F, 10.0F);
			GUI.depth = 1;
		}

		//列名
		for(int j = 0; j < sheet_columns; j++){
			columnName[j] = GUI.TextField(new Rect(10 + btn_width*j- (sheet_width)*hSbarValue/8f, 60- (sheet_height)*vSbarValue/8f, btn_width, btn_height), columnName[j]);
		}
		//更新
		if(GUI.Button(new Rect(10 + btn_width*sheet_columns- (sheet_width)*hSbarValue/8f, 60- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"更新")){
			if(selectSheet >= 0 && selectSheet < SheetNames.Length){
				updateData(filepath, SheetNames[selectSheet], contents);
			}
		}

		//显示,删除
		for(int i = 0; i < sheet_rows; i++){

			for(int j = 0; j < sheet_columns; j++){
				GUI.SetNextControlName(i.ToString()+","+j.ToString());
				contents[i , j] = GUI.TextField(new Rect(10 + btn_width*j- (sheet_width)*hSbarValue/8f, 60+ btn_height + btn_height * i- (sheet_height)*vSbarValue/8f, btn_width, btn_height), contents[i , j]);
			}

			if(selectSheet == 0){
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* i- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"添加内容")){
					addMain1 = true;
					addMainRow = i;
				}
			}
		}

		//添加行
		for(int j = 0; j < sheet_columns; j++){
			
			addContent[j] = GUI.TextField(new Rect(10 + btn_width*j- (sheet_width)*hSbarValue/8f, 60+ btn_height + btn_height* sheet_rows- (sheet_height)*vSbarValue/8f, btn_width, btn_height), addContent[j]);
		}
		if(GUI.Button(new Rect(10 + btn_width*sheet_columns- (sheet_width)*hSbarValue/8f, 60+ btn_height + btn_height * sheet_rows - (sheet_height)*vSbarValue/8f, btn_width, btn_height),"添加")){
			if(selectSheet >= 0 && selectSheet < SheetNames.Length){
//				addData(filepath, SheetNames[selectSheet], addContent);

				if(selectSheet == 0){
					addMain1 = true;
					addMainRow = sheet_rows;
				}
				else{
					showAddWindow = true;
				}
			}
		}

		if(addMain1 && selectSheet == 0){
			if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* addMainRow- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"Camera")){
				addAction("CAMERA", 2, addMainRow);
				addMain1 = false;
				addMain2 = false;
			}
			if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (1+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"TIPS")){
				addAction("TIPS", 3, addMainRow);
				addMain1 = false;
				addMain2 = false;
			}
			if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (2+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"Motions")){
				addMain2 = true;
			}

			if(addMain2){
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ 2*btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (2+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"Camera")){
					addAction("CAMERA", 4, addMainRow);
					addMain1 = false;
					addMain2 = false;
				}
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ 2*btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (3+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"TIPS")){
					addAction("TIPS", 4, addMainRow);
					addMain1 = false;
					addMain2 = false;
				}
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ 2*btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (4+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"EXCEL")){
					addAction("EXCEL", 4, addMainRow);
					addMain1 = false;
					addMain2 = false;
				}
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ 2*btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (5+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"3DMAX")){
					addAction("3DMAX", 4, addMainRow);
					addMain1 = false;
					addMain2 = false;
				}
				if(GUI.Button(new Rect(10 + btn_width*sheet_columns+ 2*btn_width- (sheet_width)*hSbarValue/8f, 60+ btn_height+ btn_height* (6+addMainRow)- (sheet_height)*vSbarValue/8f, btn_width, btn_height),"PROGRAM")){
					addAction("PROGRAM", 4, addMainRow);
					addMain1 = false;
					addMain2 = false;
				}
			}
		}
	}

	//向MAIN中添加新的Motion
	void addAction(string actionName, int column, int row){
		
		tmp_TextFields = new string[sheet_columns];
		for(int i=0; i<sheet_columns; i++){
			tmp_TextFields[i] = "";
		}
		
		if(column == 2 && actionName == "CAMERA"){
			
			//添加数据
			if(row == sheet_rows){
				//自动化生成数据:ID,
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				//自动化生成数据
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				tmp_TextFields[column] = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				
				//添加数据
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			//插入数据
			else{
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = contents[row, i];
				}
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				string actName = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				if(tmp_TextFields[column] == ""){
					tmp_TextFields[column] = actName;
				}
				else{					
					tmp_TextFields[column] += "|"+actName;
				}
				
				//更新数据
				updateData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			
			//跳转表格
			selectSheet = 1;				
			
		}
		else if(column == 3 && actionName == "TIPS"){
			
			//添加数据
			if(row == sheet_rows){
				//自动化生成数据:ID,
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				//自动化生成数据
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				tmp_TextFields[column] = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				
				//添加数据
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			//插入数据
			else{
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = contents[row, i];
				}
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				string actName = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				tmp_TextFields[column] = actName;
				
				//更新数据
				updateData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			
			//跳转表格
			selectSheet = 2;
		}
		else if(column == 4){
			
			//添加数据
			if(row == sheet_rows){
				//自动化生成数据:ID,
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				//自动化生成数据
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				tmp_TextFields[column] = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				
				//添加数据
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			//插入数据
			else{
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = contents[row, i];
				}
				
				//获得actionName的新ID
				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", actionName);
				string actName = actionName+ "@"+ (sheetTable.Rows.Count+2).ToString();
				if(tmp_TextFields[column] == ""){
					tmp_TextFields[column] = actName;
				}
				else{
					tmp_TextFields[column] += "|"+actName;
				}
				
				//更新数据
				updateData(filepath, SheetNames[selectSheet], tmp_TextFields);
			}
			
			//跳转表格
			if(actionName == "CAMERA"){
				selectSheet = 1;
			}
			if(actionName == "TIPS"){
				selectSheet = 2;
			}
			if(actionName == "EXCEL"){
				selectSheet = 3;
			}
			if(actionName == "3DMAX"){
				selectSheet = 4;
			}
			if(actionName == "PROGRAM"){
				selectSheet = 5;
			}
			
		}
		
		getSheetData(filepath, SheetNames[selectSheet]);
		showAddWindow = false;
		old_selectCategory = 0;
		showAddSelection = false;
	}

	//数据显示窗口
	void drawWindow(int id){

		if(GUI.Button(new Rect(ExcelAddWindow.width-50, 10, 30, 30), tex_CloseButton)){
			show = false;
			showAddWindow = false;
			showAddSelection = false;
			old_selectCategory = 0;
		}
		
		old_selectSheet = selectSheet;
		selectSheet = GUI.Toolbar(new Rect(10, 20, SheetNames.Length * 80, 30), selectSheet, SheetNames);

		for(int i = 0; i < SheetNames.Length; i++){
			if(selectSheet == i){
				showSheet(filepath, SheetNames[i]);
				break;
			}
		}

		GUI.DragWindow();
	}

	//编辑器类传递消息
	public void getInputString(string str){
		int result = 0;

		Debug.Log("getInputString:"+focusControl);

		if(focusControl.Contains(",")){
			string[] tmp_ctl = focusControl.Split(',');
			int result1 = 0, result2 = 0;
			if(int.TryParse(tmp_ctl[0], out result1) && int.TryParse(tmp_ctl[1], out result2)){
				contents[result1 , result2] = str;
			}
		}
		else{
			if(int.TryParse(focusControl, out result)){
				tmp_TextFields[result] = str;
			}

		}
	}

	//向GROUP中添加数据
	void addGroupData(string column, string objsName){
		string[] objs = objsName.Split(',');

		string col="";
		if(column.Contains("Group")){
			col = column.Substring(5);
		}
		else{
			col = column;
		}
		int columnNumber = int.Parse(col)+ 1;
		Debug.Log(column+","+col+","+columnNumber);

		for(int i=0; i<objs.Length; i++){
			string[] tmp_content = new string[sheet_columns];
			for(int j = 0; j < sheet_columns; j++){
				tmp_content[j] = "";
			}
			if(i < sheet_rows){
				for(int j = 0; j < sheet_columns; j++){
					tmp_content[j] = contents[i, j];
				}
				tmp_content[columnNumber] = objs[i];
				updateData(filepath, SheetNames[selectSheet], tmp_content);
			}
			else{
				tmp_content[key] = (i+2).ToString();
				tmp_content[columnNumber] = objs[i];
				addData(filepath, SheetNames[selectSheet], tmp_content);
			}
		}
	}

	//添加数据的窗口
	void drawAddWindow(int id){

		if(GUI.Button(new Rect(ExcelAddWindow.width-50, 10, 30, 30), tex_CloseButton)){
			showAddWindow = false;
			showAddSelection = false;
			old_selectCategory = 0;
		}

		if(SheetNames[selectSheet] == "MAIN"){

		}
		else if(SheetNames[selectSheet] == "CAMERA"){

			old_selectCategory = selectCategory;
			selectCategory = GUI.Toolbar(new Rect(10, 20, SheetNames.Length * 80, 30), selectCategory, cameraCategory);

			if(GUI.Button(new Rect(30+SheetNames.Length*80, 20, 80, 30),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
	
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}

			//更新
			if(old_selectCategory != selectCategory || !showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID,摄像机
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				tmp_TextFields[2] = mainCamera;
				//自动化生成数据

				if(selectCategory == 0){
					selectToggles[9] = false;
					selectToggles[10] = false;
					selectToggles[11] = false;
					selectToggles[13] = false;

					add_sheet_width = (10+ (sheet_columns-4)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 1){
					selectToggles[6] = false;
					selectToggles[12] = false;

					add_sheet_width = (10+ (sheet_columns-2)* btn_width+ btn_width) - ExcelAddWindow.width;
				}

				add_hSbarValue = 0;

				add_i = 0;
			}
	
			if(selectCategory >= 0){
				//滑动条
				bool hor = (add_sheet_width < 0);
				if(!hor){
					GUI.depth = 0;
					add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
					GUI.depth = 1;
				}

				//列名,数据
				for(int i=0,j = 0; j < sheet_columns; j++){
					if(selectToggles[j]){
						columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);

						GUI.SetNextControlName(j.ToString());
						tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
						i++;
					}
				}

				//添加数据
				bool isNecessary = false;
				if(selectCategory == 0){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==6);
				}
				else if(selectCategory == 1){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==5 || add_i==9 || add_i==10 || add_i==11 );
				}
				if(selectToggles[add_i]){
					if(isNecessary){
						GUI.color = Color.green;
					}
					columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
					GUI.SetNextControlName(add_i.ToString());
					tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
					if(isNecessary){
						GUI.color = Color.white;
					}
				}else{
					if(add_i == (sheet_columns-1)){

						addData(filepath, SheetNames[selectSheet], tmp_TextFields);

						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}else{
						add_i++;
					}
				}
				if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){
					if(add_i < (sheet_columns-1)){
						add_i++;
					}
					else{

						addData(filepath, SheetNames[selectSheet], tmp_TextFields);

						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}
				}
			}
		}
		else if(SheetNames[selectSheet] == "TIPS"){
			//更新
			if(!showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID,摄像机
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				tmp_TextFields[2] = "down_left";
				tmp_TextFields[3] = "false";
				tmp_TextFields[4] = "false";
				//自动化生成数据

				add_hSbarValue = 0f;
				add_sheet_width = (10+ (sheet_columns)* btn_width+ btn_width) - ExcelAddWindow.width;

				add_i = 0;
			}
			

			//滑动条
			bool hor = (add_sheet_width < 0);
			if(!hor){
				GUI.depth = 0;
				add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
				GUI.depth = 1;
			}
				
			//列名,数据
			for(int i=0,j = 0; j < sheet_columns; j++){
				if(selectToggles[j]){
					columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);
						
					GUI.SetNextControlName(j.ToString());
					tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
					i++;
				}
			}
			if(GUI.Button(new Rect(10+ btn_width*sheet_columns- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
				
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}
				
			//添加数据
			bool isNecessary = false;
			isNecessary = (add_i==0 || add_i==1);
			if(selectToggles[add_i]){
				if(isNecessary){
					GUI.color = Color.green;
				}
				columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
				GUI.SetNextControlName(add_i.ToString());
				tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
				if(isNecessary){
					GUI.color = Color.white;
				}
			}else{
				if(add_i == (sheet_columns-1)){
					addData(filepath, SheetNames[selectSheet], tmp_TextFields);

					showAddWindow = false;
					showAddSelection = false;
					old_selectCategory = 0;
				}else{
					add_i++;
				}
			}
			if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){
				if(add_i < (sheet_columns-1)){
					add_i++;
				}
				else{
					addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
					showAddWindow = false;
					showAddSelection = false;
					old_selectCategory = 0;
				}
			}
		}
		else if(SheetNames[selectSheet] == "EXCEL"){
			old_selectCategory = selectCategory;
			selectCategory = GUI.Toolbar(new Rect(10, 20, SheetNames.Length * 80, 30), selectCategory, excelCategory);
			
			if(GUI.Button(new Rect(30+SheetNames.Length*80, 20, 80, 30),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
				
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}
			
			//更新
			if(old_selectCategory != selectCategory || !showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID,摄像机
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				tmp_TextFields[3] = (selectCategory+1).ToString();
				//自动化生成数据
				
				if(selectCategory == 0){
					selectToggles[7] = false;
					selectToggles[8] = false;
					selectToggles[9] = false;
					selectToggles[10] = false;
					
					selectToggles[11] = false;
					selectToggles[12] = false;
					selectToggles[13] = false;

					add_sheet_width = (10+ (sheet_columns-7)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 1){
					selectToggles[4] = false;
					selectToggles[5] = false;
					selectToggles[6] = false;
					
					
					selectToggles[12] = false;
					selectToggles[13] = false;

					add_sheet_width = (10+ (sheet_columns-5)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 2){
					
					selectToggles[4] = false;
					selectToggles[8] = false;
					
					selectToggles[12] = false;
					selectToggles[13] = false;

					add_sheet_width = (10+ (sheet_columns-4)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 3){
					selectToggles[4] = false;
					selectToggles[6] = false;
					
					selectToggles[7] = false;
					selectToggles[8] = false;
					selectToggles[10] = false;

					add_sheet_width = (10+ (sheet_columns-5)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 4){
					selectToggles[4] = false;
					selectToggles[5] = false;
					selectToggles[6] = false;
					
					selectToggles[7] = false;
					selectToggles[8] = false;
					selectToggles[9] = false;
					selectToggles[10] = false;

					add_sheet_width = (10+ (sheet_columns-7)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				
				add_hSbarValue = 0;
				
				add_i = 0;
			}
			
			if(selectCategory >= 0){
				//滑动条
				bool hor = (add_sheet_width < 0);
				if(!hor){
					GUI.depth = 0;
					add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
					GUI.depth = 1;
				}
				
				//列名,数据
				for(int i=0,j = 0; j < sheet_columns; j++){
					if(selectToggles[j]){
						columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);

						GUI.SetNextControlName(j.ToString());
						tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
						i++;
					}
				}
				
				//添加数据
				bool isNecessary = false;
				if(selectCategory == 0){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==5 || add_i==6);
				}
				else if(selectCategory == 1){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==7 || add_i==9 || add_i==10);
				}
				else if(selectCategory == 2){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==5 || add_i==6 || add_i==7 || add_i==9 || add_i==10);
				}
				else if(selectCategory == 3){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==11 || add_i==12 || add_i==13 );
				}
				else if(selectCategory == 4){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==11 || add_i==12 || add_i==13 );
				}
				if(selectToggles[add_i]){
					if(isNecessary){
						GUI.color = Color.green;
					}
					columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
					GUI.SetNextControlName(add_i.ToString());
					tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
					if(isNecessary){
						GUI.color = Color.white;
					}
				}else{
					if(add_i == (sheet_columns-1)){
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}else{
						add_i++;
					}
				}
				if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){
					if(add_i < (sheet_columns-1)){
						add_i++;
					}
					else{
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}
				}
				//新建Group
				if(add_i == 2){
					if(GUI.Button(new Rect(10+ 4*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"新建Group")){
						//保存Excel临时数据
						tmpExcelData = new string[tmp_TextFields.Length];
						for(int i=0; i<tmp_TextFields.Length; i++){
							tmpExcelData[i] = tmp_TextFields[i];
						}
						ExcelOperator excelReader = new ExcelOperator();
						sheetTable = excelReader.ExcelReader(filepath + ".xls", "Group");
						DataRow dr = sheetTable.Rows[0];
						int newGroup = -1;
						for(int i=0; i<sheetTable.Columns.Count; i++){
							if(dr[i].ToString() == "0" || dr[i].ToString() == ""){
								newGroup = i-1;
								break;
							}
						}
						Debug.Log("newGroup:"+newGroup);
						if(newGroup != -1){
							tmpExcelData[2] = newGroup.ToString();
							excelToGroup = true;
							tmpExcelSelect = selectCategory;

							//跳转Group表
							selectSheet = 7;
							getSheetData(filepath, SheetNames[selectSheet]);
							showAddWindow = false;
							old_selectCategory = 0;
							showAddSelection = false;
						}
						else{
							if(MotionPara.isEditor){
								Debug.LogError("Group中没有空的列,请手动在Group中添加列.");
							}
						}
					}
				}
			}
		}
		else if(SheetNames[selectSheet] == "3DMAX"){
			old_selectCategory = selectCategory;
			selectCategory = GUI.Toolbar(new Rect(10, 20, SheetNames.Length * 80, 30), selectCategory, maxCategory);
			
			if(GUI.Button(new Rect(30+SheetNames.Length*80, 20, 80, 30),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
				
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}
			
			//更新
			if(old_selectCategory != selectCategory || !showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID,摄像机
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				//自动化生成数据
				
				if(selectCategory == 0){
					selectToggles[5] = false;
					selectToggles[6] = false;
					selectToggles[7] = false;

					add_sheet_width = (10+ (sheet_columns-3)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else{
					add_sheet_width = (10+ (sheet_columns)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				
				add_hSbarValue = 0;
				
				add_i = 0;
			}
			
			if(selectCategory >= 0){
				//滑动条
				bool hor = (add_sheet_width < 0);
				if(!hor){
					GUI.depth = 0;
					add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
					GUI.depth = 1;
				}
				
				//列名,数据
				for(int i=0,j = 0; j < sheet_columns; j++){
					if(selectToggles[j]){
						columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);

						GUI.SetNextControlName(j.ToString());
						tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
						i++;
					}
				}
				
				//添加数据
				bool isNecessary = false;
				if(selectCategory == 0){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 );
				}
				else if(selectCategory == 1){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==5 || add_i==6 || add_i==7);
				}
				if(selectToggles[add_i]){
					if(isNecessary){
						GUI.color = Color.green;
					}
					columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
					GUI.SetNextControlName(add_i.ToString());
					tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
					if(isNecessary){
						GUI.color = Color.white;
					}
				}else{
					if(add_i == (sheet_columns-1)){
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}else{
						add_i++;
					}
				}
				if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){
					if(add_i < (sheet_columns-1)){
						add_i++;
					}
					else{
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}
				}
			}
		}
		else if(SheetNames[selectSheet] == "PROGRAM"){
			old_selectCategory = selectCategory;
			selectCategory = GUI.Toolbar(new Rect(10, 20, SheetNames.Length * 80, 30), selectCategory, programCategory);
			
			if(GUI.Button(new Rect(30+SheetNames.Length*80, 20, 80, 30),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
				
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}
			
			//更新
			if(old_selectCategory != selectCategory || !showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID,摄像机
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				if(selectCategory == 0){
					tmp_TextFields[2] = "TongBang";
				}
				else if(selectCategory == 1){
					tmp_TextFields[2] = "BaiFang";
				}
				else if(selectCategory == 2){
					tmp_TextFields[2] = "NingSong";
					tmp_TextFields[11] = "-1,0,0";
				}
				else if(selectCategory == 3){
					tmp_TextFields[2] = "NingChu";
					tmp_TextFields[9] = "0,-1,0";
				}
				//自动化生成数据
				
				if(selectCategory == 0){
					selectToggles[9] = false;
					selectToggles[10] = false;
					selectToggles[11] = false;
					
					add_sheet_width = (10+ (sheet_columns-1)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 1){
					selectToggles[9] = false;
					selectToggles[10] = false;
					selectToggles[11] = false;

					add_sheet_width = (10+ (sheet_columns-2)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 2){
					add_sheet_width = (10+ (sheet_columns)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				else if(selectCategory == 3){
					selectToggles[10] = false;
					selectToggles[11] = false;

					add_sheet_width = (10+ (sheet_columns-1)* btn_width+ btn_width) - ExcelAddWindow.width;
				}
				
				add_hSbarValue = 0;
				
				add_i = 0;
			}
			
			if(selectCategory >= 0){
				//滑动条
				bool hor = (add_sheet_width < 0);
				if(!hor){
					GUI.depth = 0;
					add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
					GUI.depth = 1;
				}
				
				//列名,数据
				for(int i=0,j = 0; j < sheet_columns; j++){
					if(selectToggles[j]){
						if(j <= 2){
							columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);
						}else{
							programCategory_content[selectCategory, j-3] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), programCategory_content[selectCategory, j-3]);
						}

						GUI.SetNextControlName(j.ToString());
						tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
						i++;
					}
				}
				
				//添加数据
				bool isNecessary = false;
				if(selectCategory == 0){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==4 || add_i==5 || add_i==6);
				}
				else if(selectCategory == 1){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==4);
				}
				else if(selectCategory == 2){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==4 || add_i==5 || add_i==6 || add_i==7 || add_i==8);
				}
				else if(selectCategory == 3){
					isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==4 || add_i==5 || add_i==6 || add_i==7);
				}

				if(selectToggles[add_i]){
					if(isNecessary){
						GUI.color = Color.green;
					}

					if(add_i <= 2){
						columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
					}else{
						programCategory_content[selectCategory, add_i-3] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), programCategory_content[selectCategory, add_i-3]);
					}

					GUI.SetNextControlName(add_i.ToString());
					tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
					if(isNecessary){
						GUI.color = Color.white;
					}
				}else{
					if(add_i == (sheet_columns-1)){
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}else{
						add_i++;
					}
				}
				if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){
					if(add_i < (sheet_columns-1)){
						add_i++;
					}
					else{
						
						addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
						showAddWindow = false;
						showAddSelection = false;
						old_selectCategory = 0;
					}
				}

				//新建Group
				if(add_i == 3){
					if(GUI.Button(new Rect(10+ 4*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"新建Group")){
						//保存Program临时数据
						tmpProgramData = new string[tmp_TextFields.Length];
						for(int i=0; i<tmp_TextFields.Length; i++){
							tmpProgramData[i] = tmp_TextFields[i];
						}
						ExcelOperator excelReader = new ExcelOperator();
						sheetTable = excelReader.ExcelReader(filepath + ".xls", "Group");
						DataRow dr = sheetTable.Rows[0];
						int newGroup = -1;
						for(int i=0; i<sheetTable.Columns.Count; i++){
							if(dr[i].ToString() == "0" || dr[i].ToString() == ""){
								newGroup = i-1;
								break;
							}
						}
						Debug.Log("newGroup:"+newGroup);
						if(newGroup != -1){
							tmpProgramData[3] = newGroup.ToString();
							programToGroup = true;
							tmpProgramSelect = selectCategory;
							
							//跳转Group表
							selectSheet = 7;
							getSheetData(filepath, SheetNames[selectSheet]);
							showAddWindow = false;
							old_selectCategory = 0;
							showAddSelection = false;
						}
						else{
							if(MotionPara.isEditor){
								Debug.LogError("Group中没有空的列,请手动在Group中添加列.");
							}
						}
					}
				}
			}
		}
		else if(SheetNames[selectSheet] == "TRIGGER"){
			//更新
			if(!showAddSelection){
				tmp_TextFields = new string[sheet_columns];
				selectToggles = new bool[sheet_columns];
				showAddSelection = true;
				showTriggerType = true;
				showTriggerKey = true;
				for(int i=0; i<sheet_columns; i++){
					tmp_TextFields[i] = "";
					selectToggles[i] = true;
				}
				//自动化生成数据:ID
				tmp_TextFields[key] = (sheet_rows+2).ToString();
				//自动化生成数据
				
				add_hSbarValue = 0f;
				add_sheet_width = (10+ (sheet_columns)* btn_width+ btn_width) - ExcelAddWindow.width;
				
				add_i = 0;
			}
			
			
			//滑动条
			bool hor = (add_sheet_width < 0);
			if(!hor){
				GUI.depth = 0;
				add_hSbarValue = GUI.HorizontalScrollbar(new Rect(10, ExcelAddWindow.height-20, ExcelAddWindow.width-20, 30), add_hSbarValue, 1.0F, 0.0F, 10.0F);
				GUI.depth = 1;
			}
			
			//列名,数据
			for(int i=0,j = 0; j < sheet_columns; j++){
				if(selectToggles[j]){
					columnName[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60, btn_width, btn_height), columnName[j]);

					GUI.SetNextControlName(j.ToString());
					tmp_TextFields[j] = GUI.TextField(new Rect(10+ btn_width*i- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height), tmp_TextFields[j]);
					i++;
				}
			}
			if(GUI.Button(new Rect(10+ btn_width*sheet_columns- (add_sheet_width)*add_hSbarValue/8f, 60+ btn_height, btn_width, btn_height),"添加")){
				addData(filepath, SheetNames[selectSheet], tmp_TextFields);
				
				showAddWindow = false;
				showAddSelection = false;
				old_selectCategory = 0;
			}
			
			//添加数据
			bool isNecessary = false;
			isNecessary = (add_i==0 || add_i==1 || add_i==2 || add_i==3 || add_i==4);
			if(selectToggles[add_i]){
				if(isNecessary){
					GUI.color = Color.green;
				}
				columnName[add_i] = GUI.TextField(new Rect(10- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, 2*btn_width, btn_height), columnName[add_i]);
				GUI.SetNextControlName(add_i.ToString());
				tmp_TextFields[add_i] = GUI.TextField(new Rect(10+ 2*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height), tmp_TextFields[add_i]);
				if(isNecessary){
					GUI.color = Color.white;
				}
			}else{
				if(add_i == (sheet_columns-1)){
					addData(filepath, SheetNames[selectSheet], tmp_TextFields);
					
					showAddWindow = false;
					showAddSelection = false;
					old_selectCategory = 0;
				}else{
					add_i++;
				}
			}
			if(GUI.Button(new Rect(10+ 3*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ 3*btn_height, btn_width, btn_height),"确认")){

				if(add_i < (sheet_columns-1)){
					add_i++;
				}
				else{
					addData(filepath, SheetNames[selectSheet], tmp_TextFields);
						
					showAddWindow = false;
					showAddSelection = false;
					old_selectCategory = 0;
				}
			}

			//Trigger
			if(showTriggerType && add_i == 2){
				for(int k=0; k<triggerType.Length; k++){
					if(GUI.Button(new Rect(10+ 4*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ (3+k)*btn_height, btn_width*2, btn_height), triggerType[k])){
						if(tmp_TextFields[2] == ""){
							tmp_TextFields[2] = triggerType[k];
						}else{
							if(tmp_TextFields[2].Contains("&")){
								string[] tmpStr = tmp_TextFields[2].Split('&');
								tmp_TextFields[2] = tmpStr[1]+"&"+triggerType[k];
							}
							else{
								tmp_TextFields[2] = tmp_TextFields[2]+"&"+triggerType[k];
							}
						}
					}
				}
			}
			if(showTriggerKey && add_i == 3){
				if(tmp_TextFields[2].Contains("MOUSE")){//鼠标操作
					for(int k=0; k<triggerKey_Mouse.Length; k++){
						if(GUI.Button(new Rect(10+ 4*btn_width- (add_sheet_width)*add_hSbarValue/8f, 60+ (3+k)*btn_height, btn_width*2, btn_height), triggerKey_Mouse[k])){
							if(tmp_TextFields[3] == ""){
								tmp_TextFields[3] = triggerKey_Mouse[k];
							}else{
								if(tmp_TextFields[3].Contains("&")){
									string[] tmpStr = tmp_TextFields[3].Split('&');
									tmp_TextFields[3] = tmpStr[1]+ "&"+ triggerKey_Mouse[k];
								}
								else{
									tmp_TextFields[3] = tmp_TextFields[3]+ "&"+ triggerKey_Mouse[k];
								}
							}
						}
					}
				}
				else if(tmp_TextFields[2].Contains("KEY")){

				}
				else if(tmp_TextFields[2].Contains("KEY")){
					
				}
				else if(tmp_TextFields[2].Contains("SCROLLWHEEL")){
					
				}
			}
		}
		else if(SheetNames[selectSheet] == "Group"){
			if(!showAddSelection){
				tmp_TextFields = new string[2];
				showAddSelection = true;
				for(int i=0; i<tmp_TextFields.Length; i++){
					tmp_TextFields[i] = "";
				}

				ExcelOperator excelReader = new ExcelOperator();
				sheetTable = excelReader.ExcelReader(filepath + ".xls", "Group");
				DataRow dr = sheetTable.Rows[0];
				int newGroup = -1;
				for(int i=0; i<sheetTable.Columns.Count; i++){
					if(dr[i].ToString() == "0" || dr[i].ToString() == ""){
						newGroup = i-1;
						break;
					}
				}
				Debug.Log("newGroup:"+newGroup);
				tmp_TextFields[0] = newGroup.ToString();
			}
			
			GUI.color = Color.green;
			GUI.Label(new Rect(10, 60+30*0, 100, 30), "列名");
			GUI.Label(new Rect(10, 60+30*1, 100, 30), "物体名");
			GUI.SetNextControlName("0");
			tmp_TextFields[0] = GUI.TextField(new Rect(120, 60+30*0, 200, 30), tmp_TextFields[0]);
			GUI.SetNextControlName("1");
			tmp_TextFields[1] = GUI.TextField(new Rect(120, 60+30*1, 200, 30), tmp_TextFields[1]);
			string tmp = "注:可在Hierarchy菜单中选中多个物体,然后打开插件Tool/Wuhao/获取选中物体信息中,点击‘获得当前选中多个Gameobject’";
			GUI.Label(new Rect(320, 60+30*1, 200, 100), tmp);
			GUI.color = Color.white;
			
			if(GUI.Button(new Rect(10, 60+30*2, 100, 30),"确认")){
				Debug.Log("add 列名:"+ tmp_TextFields[0]+ ",物体名:"+ tmp_TextFields[1]);

				string col="";
				if(tmp_TextFields[0].Contains("Group")){
					col = tmp_TextFields[0].Substring(5);
				}else{
					col = tmp_TextFields[0];
				}
				int columnNumber = int.Parse(col)+ 1;
				if(!(contents[0, columnNumber] =="" || contents[0, columnNumber] =="0")){
					if(MotionPara.isEditor){
						Debug.LogError("插入Group的列不为空,不允许插入.");
					}
				}else{
					
					addGroupData(tmp_TextFields[0], tmp_TextFields[1]);
					
					showAddSelection = false;
					showAddWindow = false;
					old_selectCategory = 0;

					//跳转Excel
					if(excelToGroup){

						excelToGroup = false;

						//跳转Excel表
						selectSheet = 3;
						getSheetData(filepath, SheetNames[selectSheet]);
						showAddWindow = false;
						old_selectCategory = 0;
						showAddSelection = false;

						//恢复数据
						showAddWindow = true;
						showAddSelection = true;
						selectCategory = tmpExcelSelect;
						tmp_TextFields = new string[tmpExcelData.Length];
						for(int i=0; i<tmp_TextFields.Length; i++){
							tmp_TextFields[i] = tmpExcelData[i];
						}
					}
					//跳转Program
					if(programToGroup){
						
						programToGroup = false;
						
						//跳转Program表
						selectSheet = 5;
						getSheetData(filepath, SheetNames[selectSheet]);
						showAddWindow = false;
						old_selectCategory = 0;
						showAddSelection = false;
						
						//恢复数据
						showAddWindow = true;
						showAddSelection = true;
						selectCategory = tmpProgramSelect;
						tmp_TextFields = new string[tmpProgramData.Length];
						for(int i=0; i<tmp_TextFields.Length; i++){
							tmp_TextFields[i] = tmpProgramData[i];
						}
					}
				}
			}
		}

		GUI.DragWindow();
	}




	void OnGUI(){
		if(show){
			ExcelWriteWindow = GUI.Window(501, ExcelWriteWindow, drawWindow, "Excel读写");
		}

		if(showAddWindow){
			ExcelAddWindow = GUI.Window(502, ExcelAddWindow, drawAddWindow, "向"+ SheetNames[selectSheet]+ "中添加数据" );
		}

		//当前获得焦点的控件
		focusControl = GUI.GetNameOfFocusedControl();


		//Ctrl
		if(Input.GetKey(KeyCode.LeftControl)){
			isControl = true;
		}
		if(Input.GetKeyUp(KeyCode.LeftControl)){
			isControl = false;
		}
		//中键滚轮
		if(Input.GetAxis("Mouse ScrollWheel") != 0 ){
			if(!isControl && (sheet_height > 0)){
				vSbarValue += -1f*Input.GetAxis("Mouse ScrollWheel");
				if(vSbarValue < 0){
					vSbarValue = 0;
				}
				else if(vSbarValue > 9){
					vSbarValue = 9;
				}
			}
			else if(isControl && (sheet_width > 0)){
				hSbarValue += -1f*Input.GetAxis("Mouse ScrollWheel");
				if(hSbarValue < 0){
					hSbarValue = 0;
				}
				else if(hSbarValue > 9){
					hSbarValue = 9;
				}
			}

		}
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.F1)){
			show = true;
		}

		//if(Input.GetKeyDown(KeyCode.T)){
		//	Debug.Log("T");
		//	setExcel(UnityEngine.Application.dataPath + "/Resources/"+"3_33"+"/C/"+"C1");
		//}
		//if(Input.GetKeyDown(KeyCode.Y)){
		//	Debug.Log("Y");
		//	setExcel(UnityEngine.Application.dataPath + "/Resources/"+"3_33"+"/C/"+"C2");
		//}
	}
}
