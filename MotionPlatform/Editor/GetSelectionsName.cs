/// <summary>
/// FileName: GetSelectionsName.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.07.17
/// Version: 1.0
/// Company: Sunnytech
/// Function: 自动将所选择的物体的名字生成到Excel表格中
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

public class GetSelectionsName : EditorWindow {

	[MenuItem("Tools/运动平台/获取所选择物体的名字")]
	//初始化编辑器插件窗口
	static void WindowInit () 
	{
		//创建窗口
		Rect myRect = new Rect(200, 200, 300, 300);
		GetSelectionsName myWindow = (GetSelectionsName)EditorWindow.GetWindowWithRect(typeof(GetSelectionsName),
			myRect, true, "获取所选择物体的名字");
		myWindow.Show();
	}

	private string excelName = "SelectionsName";  //生成Excel表格名字

	void OnGUI ()
	{
		//填写信息
		EditorGUI.LabelField(new Rect(10, 10, 180, 30), "作用：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 30, 240, 30), "自动将所选择的物体的名字生成到Excel表格中;");
		EditorGUI.LabelField(new Rect(10, 50, 180, 30), "注意：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 70, 240, 100), "为了方便起见，生成的Excel表格中会按默认顺\n" +
			"序包含所有所选择的父物体及其子物体，请按需\n要自行在表格中手动删除;");
		EditorGUI.LabelField(new Rect(10, 120, 180, 30), "路径：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 140, 240, 100), "请在Assets目录下的“_TempExcel”文件夹中\n" +
			"查找新生成的Excel文件;");
		EditorGUI.LabelField(new Rect(10, 175, 180, 30), "Excel文件名：", EditorStyles.boldLabel);
		//填写项目
		excelName = EditorGUI.TextField(new Rect(30, 195, 240, 17), excelName);
		if(GUI.Button(new Rect(10, 220, 280, 20), "开始生成Excel表格")){
			ExcelCreate();
		}
	}

	//这里开启窗口的重绘，不然窗口信息不会刷新
	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	//表格创建
	void ExcelCreate()
	{
		if(!Directory.Exists(Application.dataPath + "/_TempExcel")){
			Directory.CreateDirectory(Application.dataPath + "/_TempExcel");
		}
		ExcelOperator excelWirter = new ExcelOperator();
		//为Excel写入做准备
		List<string> colName = new List<string>();
		colName.Add("NAME");
		List<List<string>> fileContent = new List<List<string>>();
		List<string> colContent = new List<string>();
		Transform[] selections = Selection.GetTransforms(SelectionMode.Unfiltered);
		foreach(Transform child in selections){
			TransformRecursion(child, colContent);
		}
		for(int i = 0; i < colContent.Count; i++){
			List<string> newCol = new List<string>();
			newCol.Add(colContent[i]);
			fileContent.Add(newCol);
		}
		excelWirter.ExcelWriter(Application.dataPath + "/_TempExcel/" + excelName + ".xls", 
			"NAME", colName, fileContent, true);
	}

	//递归获取selections名字
	void TransformRecursion(Transform aimTrans, List<string> nameList)
	{
		nameList.Add(aimTrans.name);
		if(aimTrans.childCount > 0){
			foreach(Transform child in aimTrans){
				TransformRecursion(child, nameList);
			}
		}
	}
}
