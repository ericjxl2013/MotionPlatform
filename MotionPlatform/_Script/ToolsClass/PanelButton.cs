/// <summary>
/// FileName: PanelButton.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.03.06
/// Version: 1.0
/// Company: Sunnytech
/// Function: 面板按钮触发管理
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelButton : MonoBehaviour {

	//button所在的Window的Rect
	public static Dictionary<string, Rect> btnWindow = new Dictionary<string, Rect>();
	//button自身的Rect
	public static Dictionary<string, Rect> btnRect = new Dictionary<string, Rect>();
	//button的类型:Button--false, RepeatButton--true
	public static Dictionary<string, bool> btnType = new Dictionary<string, bool>();


	//button所在的脚本
	public static Dictionary<string, string> btnScript = new Dictionary<string, string>();
	//button所对应的相应函数
	public static Dictionary<string, string> btnFunction = new Dictionary<string, string>();
	
	void Start () {

	}

	//记录button的名称，相应的window的Rect和button自己在Window上的Rect
	public static void add(Rect rec1, Rect rec2, string buttonName, bool type){
		btnWindow.Add(buttonName, rec1);
		btnRect.Add(buttonName, rec2);
		btnType.Add(buttonName, type);
	}
	
	/// <summary>
	/// 根据面板当前位置，返回指定按钮所对应的屏幕位置.
	/// </summary>
	/// <returns>
	/// 屏幕X和Y值.
	/// </returns>
	/// <param name='btn_name'>
	/// 按钮名字代号.
	///	xn, xp, yn, yp, zn, zp
	/// </param>
	public static Vector2 ButtonLocation(string btn_name, ref bool is_right){
		if(btnWindow.ContainsKey(btn_name)){
			Rect rec1 = btnWindow[btn_name];
			Rect rec2 = btnRect[btn_name];
			Vector2 result = new Vector2(rec1.x+rec2.x+rec2.width/2f, rec1.y+rec2.y+rec2.height/2f);
			is_right = true;
			return result;
		}
		else{
			is_right = false;
			return new Vector2(0,0);
		}
	}

	//GUI Button的响应 , 操作物体的函数
	public static void Btn_Function(string btn_name){
		//button名称
		if(btn_name == "TEST01"){
			GameObject.Find("Cube_1").transform.Translate(new Vector3(0,0,2));
		}
		else if(btn_name == "TEST02"){
			GameObject.Find("Cube_2").transform.Translate(new Vector3(0,0,1)*0.02f);
		}
		else if(btn_name == "TEST03"){
			GameObject.Find("Cube_2").transform.Translate(new Vector3(0,0,-1)*0.02f);
		}
		
		//Test
		else if(btn_name == "RotateCube"){
			GameObject.Find("Cube").transform.Rotate(new Vector3(0,0,1)* 0.02f* 10f);
		}
	}
	
	//教过程中,Button长按的结束判断
	public static bool DetectionLongPress(string btn_string, float step_LongPress){
		Vector3 dis = (GameObject.Find("Cube_2").transform.position-GameObject.Find("Cube_1").transform.position);
		if(btn_string == "TEST02"){
			if(Mathf.Abs(dis.magnitude- step_LongPress) <= 0.1f){
				return true;
			}
		}
		if(btn_string == "TEST03"){
			if(Mathf.Abs(dis.magnitude- step_LongPress) <= 0.1f){
				return true;
			}
		}
		
		//Test
		if(btn_string == "RotateCube"){
			if(Mathf.Abs(GameObject.Find("Cube").transform.eulerAngles.z - step_LongPress) <= 1f){
				return true;
			}
		}
		return false;
	}
}
