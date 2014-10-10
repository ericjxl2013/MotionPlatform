/// <summary>
/// FileName: HierarchicalMenu.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.02.23
/// Version: 1.0
/// Company: Sunnytech
/// Function: 自动加载多层菜单
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;

public class HierarchicalMenu : MonoBehaviour {
	
	FunctionManager st_FuncManager;
	
	const float ROW_HEIGHT = 35f;  //行距
	const float ROW_LENGTH = 170f;  //宽度
	List<string> firstMenu = new List<string>();  //一级菜单
	Dictionary<string, string[]> secondMenu = new Dictionary<string, string[]>();  //存储所有二级菜单
	Dictionary<string, string[]> thirdMenu = new Dictionary<string, string[]>();  //存储所有三级菜单
	bool secondMenuOn = false;  //控制二级菜单显示
	bool thirdMenuOn = false;  //控制三级菜单显示
	string[] firstOrder;
	string[] secondOrder;
	string[] thirdOrder;
	string menuExcelPath;
	Rect firstRect;  
	Rect secondRect;
	Rect thirdRect;
	string firstStr = "";
	string secondStr = "";
	
	// Use this for initialization
	void Start () {
		st_FuncManager = gameObject.GetComponent<FunctionManager>();
		menuExcelPath = Application.dataPath + ConstData.excelPath + ConstData.hiearchicalMenu;  //Excel所在路径
		MenuInitialize();
		firstOrder = firstMenu.ToArray();
		firstRect = new Rect(FuncPara.hiMenuCornerW, FuncPara.hiMenuCornerH, ROW_LENGTH, 300f);
		firstRect.height = firstOrder.Length * ROW_HEIGHT + 4f;
		secondRect = new Rect(0, 0, ROW_LENGTH, 300f);
		thirdRect = new Rect(0, 0, ROW_LENGTH, 300f);
	}
	
	
	void OnGUI () {
		Event mouse_e = Event.current;
		if(mouse_e.isMouse && mouse_e.type == EventType.MouseDown && 
			(mouse_e.button == 0 || mouse_e.button == 1 || mouse_e.button == 2)){
			FuncPara.hierarchicalOn = false;
			secondMenuOn = false;
			thirdMenuOn = false;
		}
	
		GUI.skin.window = FuncPara.skin_hiMenu.customStyles[0];
		if(FuncPara.hierarchicalOn){
			GUI.Window(16, firstRect, HirearchicalWindow1, "");
			GUI.BringWindowToFront(16);
			if(secondMenuOn){
				GUI.Window(17, secondRect, HirearchicalWindow2, "");
				GUI.BringWindowToFront(17);
				if(thirdMenuOn){
					GUI.Window(18, thirdRect, HirearchicalWindow3, "");
					GUI.BringWindowToFront(18);
				}
			}
		}
		GUI.skin = null;
	}
	
	//第一层菜单
	void HirearchicalWindow1(int WindowID)
	{
		Event mouse_e = Event.current;	
		float yCursor = 0;
		float yLine = 0;
		string cmdString = "";
		bool hasArrow = false;
		for(int i = 0; i < firstOrder.Length; i++){
			if(i != 0){  //画线
				yLine = i * ROW_HEIGHT + 3f;
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[1];
				GUI.Label(new Rect(0f, yLine - 8f, 165f, 10f), "");
				GUI.skin = null;
			}
			yCursor = i * ROW_HEIGHT;
			
			if(new Rect(0, yCursor, 165, ROW_HEIGHT).Contains(mouse_e.mousePosition)){  //蓝色光标
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[3];
				GUI.Label(new Rect(0, yCursor, 166, ROW_HEIGHT), "");
				cmdString = firstStr;
				firstStr = i.ToString();
				if(secondMenu.ContainsKey(i.ToString())){  //显示第二层菜单
					secondRect.x = firstRect.x + ROW_LENGTH - 5f;
					secondRect.y = firstRect.y + yCursor;
					if(secondRect.y < firstRect.y)
						secondRect.y = firstRect.y;
					secondRect.height = ROW_HEIGHT * secondMenu[firstStr].Length + 4f;
					secondOrder = secondMenu[firstStr];
					hasArrow = true;
					thirdMenuOn = false;
					secondMenuOn = true;
				}else{  //隐藏第二、第三级菜单
					secondMenuOn = false;
					thirdMenuOn = false;
				}
			}	
			GUI.skin.label = FuncPara.skin_hiMenu.customStyles[2];
			GUI.skin.label.fontSize = 15;
			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label(new Rect(35, yCursor, 170, ROW_HEIGHT), firstOrder[i]);  //菜单内容
			if(secondMenu.ContainsKey(i.ToString())){  //子菜单，黑色箭头
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[4];
				GUI.Label(new Rect(140, yCursor + 15f, 8, 8), "");
			}
			GUI.skin = null;
		}
		
		if(mouse_e.isMouse && mouse_e.type == EventType.MouseDown && mouse_e.button == 0 && cmdString != "" && !hasArrow){
			st_FuncManager.Hierarchy(cmdString); //触发
			FuncPara.hierarchicalOn = false;
			secondMenuOn = false;
			thirdMenuOn = false;
		}
		
	}
	
	//第二层菜单
	void HirearchicalWindow2(int WindowID)
	{
		Event mouse_e = Event.current;	
		float yCursor = 0;
		float yLine = 0;
		string cmdString = "";
		bool hasArrow = false;
		for(int i = 0; i < secondOrder.Length; i++){
			if(i != 0){  //画线
				yLine = i * ROW_HEIGHT + 3f;
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[1];
				GUI.Label(new Rect(0f, yLine - 8f, 165f, 10f), "");
				GUI.skin = null;
			}
			yCursor = i * ROW_HEIGHT;
			
			if(new Rect(0, yCursor, 165, ROW_HEIGHT).Contains(mouse_e.mousePosition)){  //蓝色光标
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[3];
				GUI.Label(new Rect(0, yCursor, 166, ROW_HEIGHT), "");
				secondStr = firstStr + "-" + i.ToString();
				cmdString = secondStr;
				if(thirdMenu.ContainsKey(secondStr)){  //显示第三层菜单
					thirdRect.x = secondRect.x + ROW_LENGTH - 5f;
					thirdRect.y = secondRect.y + yCursor;
					if(thirdRect.y < firstRect.y)
						thirdRect.y = firstRect.y;
					thirdRect.height = ROW_HEIGHT * thirdMenu[secondStr].Length + 4f;
					thirdOrder = thirdMenu[secondStr];
					hasArrow = true;
					thirdMenuOn = true;
				}else{
					thirdMenuOn = false;
				}
			}	
			GUI.skin.label = FuncPara.skin_hiMenu.customStyles[2];
			GUI.skin.label.fontSize = 15;
			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label(new Rect(35, yCursor, 170, ROW_HEIGHT), secondOrder[i]);  //菜单内容
			if(thirdMenu.ContainsKey(firstStr + "-" + i.ToString())){  //子菜单，黑色箭头
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[4];
				GUI.Label(new Rect(140, yCursor + 15f, 8, 8), "");
			}
			GUI.skin = null;
		}
		if(mouse_e.isMouse && mouse_e.type == EventType.MouseDown && mouse_e.button == 0 && cmdString != "" && !hasArrow){
			st_FuncManager.Hierarchy(cmdString); //触发
			FuncPara.hierarchicalOn = false;
			secondMenuOn = false;
			thirdMenuOn = false;
		}
	}
	
	//第三层菜单
	void HirearchicalWindow3(int WindowID)
	{	
		Event mouse_e = Event.current;	
		float yCursor = 0;
		float yLine = 0;
		string cmdString = "";
		for(int i = 0; i < thirdOrder.Length; i++){
			if(i != 0){  //画线
				yLine = i * ROW_HEIGHT + 3f;
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[1];
				GUI.Label(new Rect(0f, yLine - 8f, 165f, 10f), "");
				GUI.skin = null;
			}
			yCursor = i * ROW_HEIGHT;
			
			if(new Rect(0, yCursor, 165, ROW_HEIGHT).Contains(mouse_e.mousePosition)){  //蓝色光标
				GUI.skin.label = FuncPara.skin_hiMenu.customStyles[3];
				GUI.Label(new Rect(0, yCursor, 166, ROW_HEIGHT), "");
				cmdString = secondStr + "-" + i.ToString();
			}	
			GUI.skin.label = FuncPara.skin_hiMenu.customStyles[2];
			GUI.skin.label.fontSize = 15;
			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label(new Rect(35, yCursor, 170, ROW_HEIGHT), thirdOrder[i]);  //菜单内容
			GUI.skin = null;
		}
		if(mouse_e.isMouse && mouse_e.type == EventType.MouseDown && mouse_e.button == 0 && cmdString != ""){
			st_FuncManager.Hierarchy(cmdString); //触发
			FuncPara.hierarchicalOn = false;
			secondMenuOn = false;
			thirdMenuOn = false;
		}
	}
	
	//菜单信息初始化
	void MenuInitialize()
	{
		ExcelOperator excelOp = new ExcelOperator();
		DataTable menuData = excelOp.ExcelReader(menuExcelPath, "MENU", "A", "C");
		string firstStr = "";
		string secondStr = "";
		string thirdStr = "";
		int firstCount = -1;
		int secondCount = -1;
		for(int i = 0; i < menuData.Rows.Count; i++){
			firstStr = Convert.ToString(menuData.Rows[i][0]);
			if(firstStr != ""){ //第一层
				firstMenu.Add(firstStr);
				firstCount++;
			}
			secondStr = Convert.ToString(menuData.Rows[i][1]);
			if(secondStr != ""){ //有第二层
				secondCount = -1;
				List<string> secondList = new List<string>();
				string skeyStr = firstCount.ToString();
				secondList.Add(secondStr);
				secondCount++;
				for(int j = i; j < menuData.Rows.Count; j++){
					thirdStr = Convert.ToString(menuData.Rows[j][2]);
					if(thirdStr != ""){ //有第三层
						List<string> thirdList = new List<string>();
						string tkeyStr = firstCount.ToString() + "-" + secondCount.ToString();
						thirdList.Add(thirdStr);
						if(j + 1 == menuData.Rows.Count){  //到了最后一行
							thirdMenu.Add(tkeyStr, thirdList.ToArray());
							i = j;
							break;
						}
						for(int k = j + 1; k < menuData.Rows.Count; k++){
							thirdStr = Convert.ToString(menuData.Rows[k][2]);
							if(thirdStr != ""){
								thirdList.Add(thirdStr);
							}else{
								thirdMenu.Add(tkeyStr, thirdList.ToArray());
								j = k - 1;  //回到第二层
								break;
							}
						}
					}else{  //第二层继续
						if(j + 1 == menuData.Rows.Count){
							secondMenu.Add(skeyStr, secondList.ToArray());
							skeyStr = "";
							i = j;  //到了最后一行
							break;
						}else{
							secondStr = Convert.ToString(menuData.Rows[j + 1][1]);
							if(secondStr != ""){
								secondList.Add(secondStr);
								secondCount++;
							}else{
								secondMenu.Add(skeyStr, secondList.ToArray());
								skeyStr = "";
								i = j + 1;  //回到第一层
								break;
							}
						}
					}
				}	
				if(skeyStr != ""){  //第二层补救
					secondMenu.Add(skeyStr, secondList.ToArray());
				}
			}	
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
