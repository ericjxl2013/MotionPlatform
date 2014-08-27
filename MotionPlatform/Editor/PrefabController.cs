/// <summary>
/// FileName: PrefabController.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.05.11
/// Version: 1.0
/// Company: Sunnytech
/// Function: 编辑器管理prefab
///
/// Changed By:
/// Modification Time:
/// Disciption:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Data;

public class PrefabController : EditorWindow {
	
	string writePath = "/StreamingAssets/ToolParameter/ToolsInfo.ini";
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Tools/美工/替换Prefab")]
	static void WindowInit () {
		//创建窗口
		Rect myRect = new Rect(200, 200, 300, 330);
		PrefabController myWindow = (PrefabController)EditorWindow.GetWindowWithRect(typeof(PrefabController),
			myRect, true, "替换重复物体为Prefab");
		myWindow.Show();
	}

	private string excelName = "ExcelName";
	
	void OnGUI()
	{
		//填写信息
		EditorGUI.LabelField(new Rect(10, 10, 180, 30), "作用：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 30, 240, 30), "替换Hierarchy窗口中重复的GameObject\n为Prefab;");
		EditorGUI.LabelField(new Rect(10, 60, 180, 30), "使用：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 80, 240, 100), "1、先打开【获取所选择物体的名字】面板，手\n" +
		                     "动选择Hierarchy窗口中将要被替换的物体，生\n成相应的Excel表格;" );
		EditorGUI.LabelField(new Rect(30, 130, 240, 100), "2、查看所生成的Excel表格，将表格第一行所\n" +
		                     "示物体从Hierarchy中拖动到Resources\\Pref\nabs\\ReplacePre文件夹中生成相应的Prefab;" );
		EditorGUI.LabelField(new Rect(30, 180, 240, 100), "3、填写Excel名，替换Prefab");
		//填写项目
		EditorGUI.LabelField(new Rect(10, 200, 180, 30), "Excel文件名：", EditorStyles.boldLabel);
		excelName = EditorGUI.TextField(new Rect(30, 220, 240, 17), excelName);
		if(GUI.Button(new Rect(10, 255, 280, 20), "自动替换为Prefab")){
			Prefablize();
		}
		//场景中物体名字检查
		if (GUI.Button(new Rect(10, 285, 280, 20), "自动改写场景相同的名字"))
		{
			PrefabRename();
		}
	}

	//这里开启窗口的重绘，不然窗口信息不会刷新
	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	//将所选择的物体名称写入TXT
	//这里暂时不用
	private void WriteInfo()
	{
		string currentPath = Application.dataPath + writePath;
		Transform[] selection = Selection.GetTransforms(SelectionMode.Unfiltered);
		FileStream toolStream = new FileStream(currentPath, FileMode.Append, FileAccess.Write);
		StreamWriter toolWriter = new StreamWriter(toolStream);
		string writeLine = "";
		foreach(Transform child in selection){
			writeLine = child.name + "|" + child.localPosition.x + "|" + child.localPosition.y + "|" + child.localPosition.z + "|" +
				child.localEulerAngles.x + "|" + child.localEulerAngles.y + "|" + child.localEulerAngles.z;
			toolWriter.WriteLine(writeLine);
		}
		toolWriter.Close();
	}
	
	//Prefab化
	private void Prefablize()
	{
		if(!File.Exists(Application.dataPath + "/_TempExcel/" + excelName + ".xls")){
			Debug.LogError(Application.dataPath + "/_TempExcel/" + excelName + ".xls文件不存在，请确认！");
			return;
		}
		ExcelOperator excelReader = new ExcelOperator();
		DataTable preData = excelReader.ExcelReader(Application.dataPath + "/_TempExcel/" + excelName + 
			".xls", "NAME", "A", "A");
		string prefabName = (string)preData.Rows[0][0];
		Transform tempTrans = ((GameObject)Resources.Load("Prefabs/ReplacePre/" + prefabName)).transform;
		Vector3 tempPos = Vector3.zero;
		Vector3 tempAngle = Vector3.zero;
		Vector3 tempScale = Vector3.zero;
		Transform parentTrans;
		Transform tempChild;
		for(int i = 0; i < preData.Rows.Count; i++){
			string tempName = (string)preData.Rows[i][0];
			try{
				tempChild = GameObject.Find(tempName).transform;
			}catch{
				Debug.LogError(tempName + "---该物体不存在！");
				return;
			}
			parentTrans = tempChild.parent;
			tempPos = tempChild.localPosition;
			tempAngle = tempChild.localEulerAngles;
			tempScale = tempChild.localScale;
			DestroyImmediate(tempChild.gameObject);
			Transform instanceTrans = (Transform)Instantiate(tempTrans, Vector3.zero, Quaternion.identity);
			instanceTrans.name = tempName;
			instanceTrans.parent = parentTrans;
			instanceTrans.localScale = tempScale;
			instanceTrans.localPosition = tempPos;
			instanceTrans.localEulerAngles = tempAngle;
		}
		Debug.Log("恭喜！替换已完成！");
	}

	//改写名字
	private void PrefabRename()
	{
		Transform[] selections = Selection.GetTransforms(SelectionMode.Unfiltered);
		List<string> nameList = new List<string>();
		List<int> indexList = new List<int>();
		foreach (Transform trans in selections)
		{
			TransformRecursion(trans, nameList, indexList);
		}
	}

	//递归获取selections名字
	private void TransformRecursion(Transform aimTrans, List<string> nameList, List<int> indexList)
	{
		int index = nameList.IndexOf(aimTrans.name);
		if (index == -1)
		{
			nameList.Add(aimTrans.name);
			indexList.Add(1);
		}
		else
		{
			aimTrans.name = aimTrans.name + "_" + indexList[index];
			indexList[index] = indexList[index] + 1;
			nameList.Add(aimTrans.name);
			indexList.Add(1);
		}
		if (aimTrans.childCount > 0)
		{
			foreach (Transform child in aimTrans)
			{
				TransformRecursion(child, nameList, indexList);
			}
		}
	}
}
