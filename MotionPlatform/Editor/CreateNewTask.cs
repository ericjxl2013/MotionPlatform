using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateNewTask : EditorWindow {

	string newTask = "";

	[MenuItem("Tools/运动平台/创建新的任务")]
	//初始化编辑器插件窗口
	static void WindowInit () 
	{
		//创建窗口
		Rect myRect = new Rect(200, 200, 300, 300);
		CreateNewTask myWindow = (CreateNewTask)EditorWindow.GetWindowWithRect(typeof(CreateNewTask),
		                                                                               myRect, true, "Create new task");
		myWindow.Show();
	}

	void OnGUI(){
		//填写信息
		EditorGUI.LabelField(new Rect(10, 10, 180, 30), "作用：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 30, 240, 30), "向当前的工程中添加新的任务\n");
		EditorGUI.LabelField(new Rect(10, 60, 180, 30), "注意：", EditorStyles.boldLabel);
		EditorGUI.LabelField(new Rect(30, 80, 240, 100), "生成的主文件夹在Assets/Resource目录下\n" +
		                     "名字为输入的任务名,里面有三个子文件夹C,Y,Z\n" +
		                     "主文件夹和C文件夹里面有自动生成的xls文件;");
		//填写项目
		EditorGUI.LabelField(new Rect(10, 130, 180, 30), "任务名字：", EditorStyles.boldLabel);
		newTask = EditorGUI.TextField(new Rect(30, 150, 240, 17), newTask);
		if(GUI.Button(new Rect(30,180,240,20),"创建新的任务")){
			if(newTask != ""){
				createTask(newTask);
			}
		}
	}

	void createTask(string taskname){
		Newtask newtask = new Newtask();
		newtask.createTask(newTask);
	}
}
