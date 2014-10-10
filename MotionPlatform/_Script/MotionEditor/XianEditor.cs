using UnityEngine;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System;

public class XianEditor : MonoBehaviour {

	public static bool show;
	public bool show_AddWindow;

	//UI
	Rect XianWindow;
	Rect Add_XianWindow;
	float btn_width = 100f;
	float btn_height = 30f;

	//初始化
	bool initialed;

	string filepath;
	System.Data.DataTable sheetTable;

	//选择sheet
	string[] SheetNames;
	int selectSheet;
	int old_selectSheet;

	//当前sheet中内容
	int sheet_rows; //行数
	int sheet_columns; //列数
	string[,] contents;
	//要添加的行
	string[] addContent;
	//列名
	string[] columnName;

	//添加新引出线
	string add_sheet_name = "";
	string[] add_sheet_distance = new string[]{""};
	string[] add_sheet_component = new string[]{""};
	string[] add_sheet_obj = new string[]{""};

	//焦点控件
	string focusControl;

	//关闭
	Texture2D tex_CloseButton;

	//显示窗口
	string[] WindowNames;
	public int selectWindow;
	int old_selectWindow;

	void Start () {
		XianWindow = new Rect(100,100,1000,600);
		Add_XianWindow = new Rect(100,100,1000,600);

		selectSheet = -1;
		selectWindow = -1;

		filepath = MotionPara.taskRootPath + MotionPara.taskName + "/XIAN";

		WindowNames = new string[]{"引出线显示", "添加新引出线"};
		SheetNames = new string[] {"a", "b", "c"};
		columnName = new string[]{"距离远近", "部件中文名", "物体名字"};

		focusControl = "";

		tex_CloseButton = (Texture2D)Resources.Load("Texture/CustomMenu/关闭1");
	}

	//获取数据
	void getSheetData(string filepath, string sheetname){

		//sheet名称获取
		string jsonText = @"";
		jsonText += File.ReadAllText(filepath+ ".json");
		
		DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonText);
		
		SheetNames = new string[dataSet.Tables.Count];
		for(int i=0; i<SheetNames.Length; i++){
			SheetNames[i] = dataSet.Tables[i].TableName;

			if(SheetNames[i] == sheetname){
				selectSheet = i;
			}
		}

		if(sheetname == ""){//json文件为空时
			selectSheet = -1;

			sheet_rows = 0;
			sheet_columns = 0;
		}
		else{

			//获得sheet中数据
			JsonOperator jo2 = new JsonOperator();
			sheetTable = jo2.JsonReader(filepath + ".json", sheetname);

			sheet_rows = sheetTable.Rows.Count;
			sheet_columns = sheetTable.Columns.Count;

			contents = new string[sheet_rows, sheet_columns];
			addContent = new string[sheetTable.Columns.Count];

			for(int i = 0; i < sheet_rows; i++){
				DataRow dr = sheetTable.Rows[i];
				for(int j = 0; j < sheetTable.Columns.Count; j++){
					contents[i , j] = dr[j]. ToString();
				}
			}

			for(int j = 0; j < sheetTable.Columns.Count; j++){
				addContent[j] = "";
			}
		}
	}

	//显示
	void showSheet(string filepath,string sheetname){
		if(old_selectSheet != selectSheet){

			getSheetData(filepath, sheetname);
		}

		//列名
		for(int j = 0; j < sheet_columns; j++){
			columnName[j] = GUI.TextField(new Rect(10 + btn_width*j, 100, btn_width, btn_height), columnName[j]);
		}

		//更新数据
		if(GUI.Button(new Rect(10 + btn_width*3, 100, btn_width, btn_height), "更新")){
			//Wait for adding
			//向json中更新信息
			JsonOperator jo = new JsonOperator();
			string[] column_name = new string[]{"distance", "component_name", "obj_name"};
			string[,] insert_content = new string[3, contents.GetLength(0)];
			for(int k=0; k<contents.GetLength(0); k++){
				insert_content[0, k] = contents[k, 0];
				insert_content[1, k] = contents[k, 1];
				insert_content[2, k] = contents[k, 2];
			}

			jo.JsonWriter(filepath + ".json", SheetNames[selectSheet], column_name, insert_content, false);
			
			//更新信息
			getSheetData(filepath, SheetNames[selectSheet]);
		}

		//显示
		for(int i = 0; i < sheet_rows; i++){
			
			for(int j = 0; j < sheet_columns; j++){
				GUI.SetNextControlName(i.ToString()+","+j.ToString());
				contents[i , j] = GUI.TextField(new Rect(10 + btn_width*j, 100+ btn_height + btn_height * i, btn_width, btn_height), contents[i , j]);
			}

			//删除数据
			if(GUI.Button(new Rect(10 + btn_width*sheet_columns, 100+ btn_height + btn_height * i, btn_width, btn_height), "删除")){
				//Wait for adding
				//向json中删除信息
				if(contents.GetLength(0) == 1){//最后一行

					JsonOperator jo = new JsonOperator();

					jo.JsonDelete(filepath + ".json", SheetNames[selectSheet]);

					Debug.LogWarning("当前sheet只有最后一行,删除最后一行就是删除当前sheet:"+SheetNames[selectSheet]);

					//更新信息
					if(selectSheet > 0){
						getSheetData(filepath, SheetNames[selectSheet-1]);
					}
					else if(selectSheet == 0 && SheetNames.Length > 1){
						getSheetData(filepath, SheetNames[1]);
					}
					else{//删空
						getSheetData(filepath, "");
					}
				}else{

					JsonOperator jo = new JsonOperator();
					string[] column_name = new string[]{"distance", "component_name", "obj_name"};
					string[,] insert_content = new string[3, contents.GetLength(0)-1];

					for(int k=0, m=0; k<contents.GetLength(0); k++){
						if(k != i){
							insert_content[0, m] = contents[k, 0];
							insert_content[1, m] = contents[k, 1];
							insert_content[2, m] = contents[k, 2];

							m++;
						}
					}

					jo.JsonWriter(filepath + ".json", SheetNames[selectSheet], column_name, insert_content, false);
					
					//更新信息
					getSheetData(filepath, SheetNames[selectSheet]);
				}
			}
		}

		//添加行
		for(int j = 0; j < sheet_columns; j++){
			
			addContent[j] = GUI.TextField(new Rect(10 + btn_width*j, 100+ btn_height + btn_height* sheet_rows, btn_width, btn_height), addContent[j]);
		}
		if(GUI.Button(new Rect(10 + btn_width*sheet_columns, 100+ btn_height + btn_height * sheet_rows, btn_width, btn_height),"添加")){
			if(selectSheet >= 0 && selectSheet < SheetNames.Length){
				
				//Wait for adding
				//向json中更新信息
				JsonOperator jo = new JsonOperator();
				string[] column_name = new string[]{"distance", "component_name", "obj_name"};
				string[,] insert_content = new string[3, contents.GetLength(0)+1];
				for(int k=0; k<contents.GetLength(0); k++){
					insert_content[0, k] = contents[k, 0];
					insert_content[1, k] = contents[k, 1];
					insert_content[2, k] = contents[k, 2];
				}

				insert_content[0, contents.GetLength(0)] = addContent[0];
				insert_content[1, contents.GetLength(0)] = addContent[1];
				insert_content[2, contents.GetLength(0)] = addContent[2];
				
				jo.JsonWriter(filepath + ".json", SheetNames[selectSheet], column_name, insert_content, false);
			
				//更新信息
				getSheetData(filepath, SheetNames[selectSheet]);
			}
		}
	}

	//数据显示
	void drawWindow(int windowID){

		if(GUI.Button(new Rect(XianWindow.width-50, 10, 30, 30), tex_CloseButton)){
			show = false;
		}

		//初始化
		if(!initialed){

			if(!File.Exists(filepath+ ".json")){
				//创建json文件
				DataSet dataSets = new DataSet();
				string json = JsonConvert.SerializeObject(dataSets, Formatting.Indented);

				// Create the file.
				using (FileStream fs = File.Create(filepath+ ".json"))
				{
					Byte[] info = new UTF8Encoding(true).GetBytes(json);
					// Add some information to the file.
					fs.Write(info, 0, info.Length);
				}
			}

			string jsonText = @"";
			jsonText += File.ReadAllText(filepath+ ".json");
			
			DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonText);

			SheetNames = new string[dataSet.Tables.Count];
			for(int i=0; i<SheetNames.Length; i++){
				SheetNames[i] = dataSet.Tables[i].TableName;
			}

			initialed = true;
		}

		old_selectWindow = selectWindow;
		selectWindow = GUI.Toolbar(new Rect(10, 20, WindowNames.Length * 200, 30), selectWindow, WindowNames);

		if(selectWindow == 0){

			if(SheetNames.Length == 0){
				GUI.Label(new Rect(10, 60, 400, 30), "XIAN.json中没有包含引出线信息的sheet");
			}

			old_selectSheet = selectSheet;
			selectSheet = GUI.Toolbar(new Rect(10, 60, SheetNames.Length * 80, 30), selectSheet, SheetNames);

			for(int i = 0; i < SheetNames.Length; i++){
				if(selectSheet == i){
					showSheet(filepath, SheetNames[i]);
					break;
				}
			}
		}
		else if(selectWindow == 1){
			drawAddWindow(101);
		}

		GUI.DragWindow();
	}

	//编辑器类传递消息
	public void getInputString(string str){

		Debug.Log("getInputString:"+focusControl+",str:"+str);

		if(selectWindow == 1){
			string[] tmp_ctl = str.Split(',');

			add_sheet_distance = new string[tmp_ctl.Length];
			add_sheet_component = new string[tmp_ctl.Length];
			add_sheet_obj = new string[tmp_ctl.Length];

			for(int i=0; i<tmp_ctl.Length; i++){
				add_sheet_distance[i] = "0";
				add_sheet_component[i] = "";
				add_sheet_obj[i] = tmp_ctl[i];
			}
		}
	}

	void drawAddWindow(int WindowID){

		GUI.Label(new Rect(10, 60, 180, 30), "新引出线的sheet名称:");
		add_sheet_name = GUI.TextField(new Rect(200, 60, 200, 30), add_sheet_name);


		//列名
		for(int j = 0; j < 3; j++){
			columnName[j] = GUI.TextField(new Rect(10 + btn_width*j, 100, btn_width, btn_height), columnName[j]);
		}

		//添加
		if(GUI.Button(new Rect(10 + btn_width*3, 100, btn_width, btn_height), "确认")){
			if(add_sheet_name == "" || add_sheet_name == null){
				Debug.LogError("新添加的引出线sheet名不能为空");
			}
			else if(add_sheet_distance.Length == 1 && add_sheet_obj[0] == ""){
				Debug.LogError("新添加的引出线sheet内容不能为空");
			}
			else{
				//向json中添加信息
				JsonOperator jo = new JsonOperator();
				string[] column_name = new string[]{"distance", "component_name", "obj_name"};
				string[,] insert_content = new string[3, add_sheet_obj.Length];
				for(int k=0; k<add_sheet_obj.Length; k++){
					insert_content[0, k] = add_sheet_distance[k];
					insert_content[1, k] = add_sheet_component[k];
					insert_content[2, k] = add_sheet_obj[k];
				}

				jo.JsonWriter(filepath + ".json", add_sheet_name, column_name, insert_content, false);

				//更新信息
				getSheetData(filepath, add_sheet_name);

				//跳转到显示窗口
				selectWindow = 0;

				if(MotionEditor.jumpToXian){
					GameObject.Find("MainScript").GetComponent<MotionEditor>().xianBack(add_sheet_name);
				}
			}
		}

		//内容
		for(int j=0; j<add_sheet_distance.Length; j++){

			add_sheet_distance[j] = GUI.TextField(new Rect(10 + btn_width*0, 100+ btn_height+ btn_height*j, btn_width, btn_height), add_sheet_distance[j]);
			add_sheet_component[j] = GUI.TextField(new Rect(10 + btn_width*1, 100+ btn_height+ btn_height*j, btn_width, btn_height), add_sheet_component[j]);
			GUI.SetNextControlName(j.ToString());
			add_sheet_obj[j] = GUI.TextField(new Rect(10 + btn_width*2, 100+ btn_height+ btn_height*j, btn_width, btn_height), add_sheet_obj[j]);
		}

	}

	void OnGUI(){

		GUI.skin = FuncPara.defaultSkin;

		if(show){
			XianWindow = GUI.Window(503, XianWindow, drawWindow, "引出线表格填写");
		}

		//当前获得焦点的控件
		focusControl = GUI.GetNameOfFocusedControl();

		GUI.skin = null;
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.S)){
			show = !show;
		}

	}
}
