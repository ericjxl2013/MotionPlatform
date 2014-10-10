/// <summary>
/// FileName: RecrTest.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.03.02
/// Version: 1.0
/// Company: Sunnytech
/// Function: 测试用，查看Rect的当前位置
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;

public class RectTest : MonoBehaviour {
	public Rect currentRect;

	// Use this for initialization
	void Start () {
		currentRect = new Rect(500, 500, 300, 200);
	}
	
	void OnGUI () {
		GUI.skin = FuncPara.defaultSkin;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 14;
		if(FuncPara.rectWindow){
			currentRect = GUI.Window(24, currentRect, RectWindow, "");
//			FuncPara.cursorPosition.x = currentRect.x;
//			FuncPara.cursorPosition.y = currentRect.y;
		}
		
	}
	
	void RectWindow (int WindowID) {
		GUI.Label(new Rect(10, 10, 300, 30), "X值：" + currentRect.x.ToString());
		GUI.Label(new Rect(10, 40, 300, 30), "Y值：" + currentRect.y.ToString());
		GUI.Label(new Rect(10, 70, 300, 30), "X mouse：" + Event.current.mousePosition.x.ToString());
		GUI.Label(new Rect(10, 100, 300, 30), "Y mouse：" + Event.current.mousePosition.y.ToString());
		GUI.Label(new Rect(10, 130, 300, 30), "X mouse：" + FuncPara.cursorPosition.x.ToString());
		GUI.Label(new Rect(10, 160, 300, 30), "Y mouse：" + FuncPara.cursorPosition.y.ToString());
		GUI.DragWindow();
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
