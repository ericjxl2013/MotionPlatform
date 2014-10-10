using UnityEngine;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;

public class ProjectManager : MonoBehaviour {

	//总编辑窗口
	public static bool show;
	private Rect editorRect;

	//设备简介
	bool show_Introduction;
	private Rect introductionRect ;
	string intro_name;
	string intro_info;

	//结构认知
	bool show_Structure;
	private Rect structureRect;
	List<string> list_Structure = new List<string>();

	void Start () {
		show = true;
		editorRect = new Rect(50, 50, 250, 200);

		intro_name = "";
		intro_info = "";
		introductionRect = new Rect(50, 50, 500, 300);

		structureRect = new Rect(50, 50, 500, 300);
	}

	//总编辑窗口
	void EditWindow(int WindowID){

		if(GUI.Button(new Rect(20, 30, 200, 30), "1.设备简介")){
			EditorManager.show = false;

			show_Introduction = true;

			FuncPara.outlineOn = false;
			FuncPara.taskActive = Task.None;
			show_Structure = false;


		}
		if(GUI.Button(new Rect(20, 70, 200, 30), "2.结构认知")){
			EditorManager.show = false;

			show_Introduction = false;


			//Wait for add
			FuncPara.taskActive = Task.Structrue;
//			FuncPara.outlineOn = true;
			show_Structure = true;
//			GameObject.Find("Main Camera").GetComponent<doMoldLable>().showLabelType("主视图", false);
		}
		if(GUI.Button(new Rect(20, 110, 200, 30), "3.基本操作")){
			EditorManager.show = false;

			show_Introduction = false;



			FuncPara.outlineOn = false;
			FuncPara.taskActive = Task.None;
			show_Structure = false;
			//Wait for add

		}
		if(GUI.Button(new Rect(20, 150, 200, 30), "4.运动信息填写窗口")){
			show_Introduction = false;

			EditorManager.show = true;

			FuncPara.outlineOn = false;
			FuncPara.taskActive = Task.None;
			show_Structure = false;
		}

		GUI.DragWindow();
	}

	//设备简介
	void IntroductionWindow(int WindowID){
		
		GUI.Label(new Rect(30, 30, 100, 30), "设备名:");
		intro_name = GUI.TextField(new Rect(140, 30, 330, 30), intro_name);

		GUI.Label(new Rect(30, 70, 100, 30), "设备简介信息:");
		intro_info = GUI.TextArea(new Rect(140, 70, 330, 150), intro_info);

		if(GUI.Button(new Rect(50, 240, 100, 30), "确定")){
			string file_path = Application.dataPath + "/Resources/ReleaseFile/Excel/Introduction.json";
			string sheet_name = "Introduction";
			string[] column_name = new string[]{"name","information"};
			string[,] insert_content = new string[,]{{intro_name}, {intro_info}};

			//写入json文件
			JsonOperator jo = new JsonOperator();
			jo.JsonWriter(file_path, sheet_name, column_name, insert_content, false);

			show_Introduction = false;

			//修改
			FuncPara.introductionName = intro_name;
			FuncPara.introductionString = intro_info;
		}
		if(GUI.Button(new Rect(350, 240, 100, 30), "取消")){
			show_Introduction = false;
		}
		
		GUI.DragWindow();
	}

	//结构认知
	void StructureWindow(int WindowID){

		if(list_Structure.Count == 0){
			ExcelOperator excelReader = new ExcelOperator();
			DataTable mainTable = excelReader.ExcelReader(Application.dataPath + "/Resources/ReleaseFile/Excel/HierarchicalMenu.xls", "MENU", "A", "C");

			for(int i=1; i<mainTable.Rows.Count; i++){
				string str_tmp = mainTable.Rows[i][1].ToString();

				if(str_tmp != ""){
					list_Structure.Add(str_tmp);
				}
				else{
					break;
				}
			}
		}


		for(int i=0; i<list_Structure.Count; i++){
			if(GUI.Button(new Rect(30, 30 + 40*i, 100, 30), list_Structure[i])){
				FuncPara.outlineOn = true;
		
				string file_path = UnityEngine.Application.dataPath + "/Resources/Cognitive_Structure/" + MotionPara.deviceName + "/"+ list_Structure[i]+ ".json";
				GameObject.Find("Main Camera").GetComponent<doMoldLable>().showLabelType(list_Structure[i], File.Exists(file_path));
				
			}
		}
		
		GUI.DragWindow();
	}

	void OnGUI(){
		GUI.skin = FuncPara.defaultSkin;
		if(show){
			editorRect = GUI.Window(1111, editorRect, EditWindow, "工程信息填写窗口");
		}

		if(show_Introduction){
			introductionRect = GUI.Window(1112, introductionRect, IntroductionWindow, "设备简介填写窗口");
		}

		if(show_Structure){
			structureRect = GUI.Window(1113, structureRect, StructureWindow, "结构认知");
		}

		GUI.skin = null;
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.F1)){
			show = !show;
		}
	}
}
