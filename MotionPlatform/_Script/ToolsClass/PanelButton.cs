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

//	//button所在的Window的Rect
//	public  Dictionary<string, Rect> btnWindow = new Dictionary<string, Rect>();
//	//button自身的Rect
//	public  Dictionary<string, Rect> btnRect = new Dictionary<string, Rect>();
//	//button的类型:Button--false, RepeatButton--true
//	public  Dictionary<string, bool> btnType = new Dictionary<string, bool>();

	MotionManager motion_Manager;

	void Start () {
		motion_Manager = GameObject.Find("MainScript").GetComponent<MotionManager>();
	}

//	//记录button的名称，相应的window的Rect和button自己在Window上的Rect
//	public static void add(Rect rec1, Rect rec2, string buttonName, bool type){
//		btnWindow.Add(buttonName, rec1);
//		btnRect.Add(buttonName, rec2);
//		btnType.Add(buttonName, type);
//	}
	
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
	public Vector2 ButtonLocation(string btn_name, ref bool is_right){
//		if(btnWindow.ContainsKey(btn_name)){
//			Rect rec1 = btnWindow[btn_name];
//			Rect rec2 = btnRect[btn_name];
//			Vector2 result = new Vector2(rec1.x+rec2.x+rec2.width/2f, rec1.y+rec2.y+rec2.height/2f);

			//motion_Manager 脚本里面的 button名称
			Vector2 result = new Vector2(0, 0);
			if(btn_name == "TEST01"){
				Rect rec1 = motion_Manager.testRect;
				Rect rec2 = new Rect(310, 60, 50, 50);
				result = new Vector2(rec1.x+rec2.x+rec2.width/2f, rec1.y+rec2.y+rec2.height/2f);

				is_right = true;
				return result;
			}
			else if(btn_name == "TEST02"){
				Rect rec1 = motion_Manager.testRect;
				Rect rec2 = new Rect(310, 120, 50, 50);
				result = new Vector2(rec1.x+rec2.x+rec2.width/2f, rec1.y+rec2.y+rec2.height/2f);

				is_right = true;
				return result;
			}
			else if(btn_name == "TEST03"){
				Rect rec1 = motion_Manager.testRect;
				Rect rec2 = new Rect(310, 180, 50, 50);
				result = new Vector2(rec1.x+rec2.x+rec2.width/2f, rec1.y+rec2.y+rec2.height/2f);

				is_right = true;
				return result;
			}
			//出错
			else{
				is_right = false;
				return new Vector2(0,0);
			}
//		}
//		else{
//			is_right = false;
//			return new Vector2(0,0);
//		}
	}

	//GUI Button的响应 , 操作物体的函数
	public void Btn_Function(string btn_name){

		//motion_Manager 脚本里面的 button名称
		if(btn_name == "TEST01"){
			motion_Manager.testBtn1();
		}
		else if(btn_name == "TEST02"){
			motion_Manager.testBtn2();
		}
		else if(btn_name == "TEST03"){
			motion_Manager.testBtn3();
		}

		//Test
		else if(btn_name == "RotateCube"){
			GameObject.Find("Cube").transform.Rotate(new Vector3(0,0,1)* 0.02f* 10f);
		}
	}
	
	//教过程中,Button长按的结束判断
	public bool DetectionLongPress(string btn_string, float step_LongPress){
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
