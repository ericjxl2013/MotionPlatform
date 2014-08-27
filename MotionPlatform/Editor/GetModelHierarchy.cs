/// <summary>
/// FileName: GetModelHierarchy.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.07.17
/// Version: 1.0
/// Company: Sunnytech
/// Function: 获取当前物体层级或者根据已有的Excel表格生成物体层级关系
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using System.Collections.Generic;

public class GetModelHierarchy : EditorWindow {

	[MenuItem("Tools/运动平台/获取和设置GameObject层级")]
	//初始化编辑器插件窗口
	static void WindowInit () 
	{
		//创建窗口
		Rect myRect = new Rect(200, 200, 300, 300);
		GetModelHierarchy myWindow = (GetModelHierarchy)EditorWindow.GetWindowWithRect(typeof(GetModelHierarchy),
			myRect, true, "Get && Set Model Hierarchy");
		myWindow.Show();
	}

	private string excelName = "ExcelName";

	void OnGUI ()
	{
		//填写信息
		EditorGUI.LabelField(new Rect(10, 10, 180, 30), "作用：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 30, 240, 30), "将当前选择的层级关系添加到Excel表格中或者\n" +
			"从Excel表格中获取层级关系;");
		EditorGUI.LabelField(new Rect(10, 60, 180, 30), "注意：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 80, 240, 100), "生成或者获取Excel表格都在Assets目录下的\n" +
		                     "“_TempExcel”文件夹中，生成时会自动产生\n" +
		                     "该文件夹，获取时若没有该文件夹请手动创建;");
		//填写项目
		EditorGUI.LabelField(new Rect(10, 130, 180, 30), "Excel名字：", EditorStyles.boldLabel);
		excelName = EditorGUI.TextField(new Rect(30, 150, 240, 17), excelName);
		if(GUI.Button(new Rect(10, 180, 280, 20), "开始生成层级Excel表格")){
			GetHierarchy();
		}
		if(GUI.Button(new Rect(10, 210, 280, 20), "开始创建物体层级关系")){
			SetHierarchy();
		}
	}

	//这里开启窗口的重绘，不然窗口信息不会刷新
	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	//获取层级关系
	void GetHierarchy () 
	{
		if(!Directory.Exists(Application.dataPath + "/_TempExcel")){
			Directory.CreateDirectory(Application.dataPath + "/_TempExcel");
		}
		ExcelOperator excelWirter = new ExcelOperator();
		//为Excel写入做准备
		List<string> colName = new List<string>();
		colName.Add("NAME"); colName.Add("PARENT");
		List<List<string>> fileContent = new List<List<string>>();
		List<string> colContent = new List<string>();
		Transform[] selections = Selection.GetTransforms(SelectionMode.Unfiltered);
		foreach(Transform child in selections){
			TransformRecursion(child, colContent);
		}
		for(int i = 0; i < colContent.Count / 2; i++){
			List<string> newCol = new List<string>();
			newCol.Add(colContent[2 * i]);
			newCol.Add(colContent[2 * i + 1]);
			fileContent.Add(newCol);
		}
		excelWirter.ExcelWriter(Application.dataPath + "/_TempExcel/" + excelName + ".xls", 
			"NAME", colName, fileContent, true);
	}

	//递归获取selections名字
	void TransformRecursion(Transform aimTrans, List<string> nameList)
	{
		nameList.Add(aimTrans.name);
		if(aimTrans.parent != null){
			nameList.Add(aimTrans.parent.transform.name);
		}else{
			nameList.Add("null");
		}
		if(aimTrans.childCount > 0){
			foreach(Transform child in aimTrans){
				TransformRecursion(child, nameList);
			}
		}
	}

	//设置层级关系
	void SetHierarchy () 
	{
		if(!Directory.Exists(Application.dataPath + "/_TempExcel")){
			Debug.LogError("请先创建_TempExcel文件夹！");
			return;
		}
		if(!File.Exists(Application.dataPath + "/_TempExcel/" + excelName + ".xls")){
			Debug.LogError(excelName +  ".xls文件不存在！");
			return;
		}
		ExcelOperator excelReader = new ExcelOperator();
		DataTable setData = excelReader.ExcelReader(Application.dataPath + "/_TempExcel/" + 
			excelName + ".xls", "SelectionsName", "A", "B");
		string tempName = "";
		string tempParent = "";
		Transform transChild;
		Transform transParent;
		for(int i = 0; i < setData.Rows.Count; i++){
			tempName = (string)setData.Rows[i][0];
			tempParent = (string)setData.Rows[i][1];
			try{
				transChild = GameObject.Find(tempName).transform;
			}catch{
				transChild = new GameObject("EmptyObject").transform;
			}
			if(tempParent != "null"){
				try{
					transParent = GameObject.Find(tempParent).transform;
				}catch{
					transParent = new GameObject("EmptyParent").transform;
				}
			}else{
				transParent = null;
			}
			transChild.parent = transParent;
		}
	}

}
