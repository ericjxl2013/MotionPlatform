/// <summary>
/// <Filename>: InterfaceManager.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.24
/// Version: 1.0
/// Company: Sunnytech
/// Function: 运动模块与原来平台的接口管理脚本
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InterfaceManager : MonoBehaviour {


	doVoiceExe st_Voice;
	TipsWindow st_Tips;

	//List<string> testVoiceStr = new List<string>();
	// int testNum = 0;
	// TipsInfoManager tipsTestInfo = new TipsInfoManager();

	void Awake ()
	{
		gameObject.AddComponent<doVoiceExe>();
		st_Voice = gameObject.GetComponent<doVoiceExe>();
		gameObject.AddComponent<TipsWindow>();
		st_Tips = gameObject.GetComponent<TipsWindow>();

	}

	// Use this for initialization
	void Start () 
	{
		// testVoiceStr.Add("我");       //2.6
		// testVoiceStr.Add("我.是");       //2.6\
		// testVoiceStr.Add("我.是.谁");       //2.6
		// testVoiceStr.Add("我是谁;");       //2.6
		// testVoiceStr.Add("我是谁啊;");       //2.8
		// testVoiceStr.Add("我是谁的人;");       //2.9
		// testVoiceStr.Add("我是谁我是谁;");       //3.1
		// testVoiceStr.Add("6.什么东西啊;");
		// testVoiceStr.Add("6,什么东西啊;");
		// testVoiceStr.Add("明天你要嫁给我，明天你要嫁给我！");
		// testVoiceStr.Add("如果紧急停止旋钮处于按下状态，请先旋松紧急停止按钮;");
		// testVoiceStr.Add("第一步，鼠标移动，至模式选择按钮，回零档位，单击切换;;");
		// testVoiceStr.Add("第三步，设定毛坯长230、宽200、高60，点击按钮加载毛坯;");
		// testVoiceStr.Add("第三步，设定毛坯长230.宽200.高60.点击按钮加载毛坯;");
		// testVoiceStr.Add("第三步，设定毛坯长230,宽200,高60,点击按钮加载毛坯;");
		// testVoiceStr.Add("第二步，X轴对刀，将工件和刀具移动到合适位置，按照工艺要求将加工原点设定在工件上表面几何中心;");
	}


	/// <summary>
	/// 语音.
	/// </summary>
	/// <param name='voice'>
	/// 要读的字符串，空时停止.
	/// </param>
	public void Voice(string voice_msg)
	{
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不播放语音
		// 	return;
		// }else 
		if(MotionPara.SpeedRate <= 1.01f) //慢速或者1倍速时正常朗读语音
			st_Voice.doVoice(voice_msg);
		else{  //加速朗读语音
			string speedStr = (Mathf.FloorToInt(MotionPara.SpeedRate * 10 - 10) / 2).ToString();
			st_Voice.doVoice(voice_msg, speedStr);
		}
	}

	/// <summary>
	/// 语音.
	/// </summary>
	/// <param name='voice_msg'>
	/// 要读的字符串，空时停止.
	/// </param>
	/// <param name='speed_str'>
	/// 语音播放速度控制.
	/// </param>
	public void Voice(string voice_msg, string speed_str)
	{
		st_Voice.doVoice(voice_msg, speed_str);
	}

	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, Vector2 target_position){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow, target_position);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, string position_string){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow, position_string);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, WindowColor window_color){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow, window_color);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='window_on'>
	/// 是否打开Tips Window.
	/// </param>
	/// <param name='tips_string'>
	/// Tips_string.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置.
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, Vector2 target_position, WindowColor window_color){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow, target_position, window_color);
		else
			FuncPara.tipsWindow = false;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void TipsWindow(bool window_on, string tips_string, bool move_allow, string position_string, WindowColor window_color){
		// if(FuncPara.currentMotion == MotionState.Exercising && !FuncPara.helpInfo){  //帮助禁止时不显示Tips窗口
		// 	FuncPara.tipsWindow = false;
		// }else 
		if(window_on)
			st_Tips.RectAdjust(tips_string, move_allow, position_string, window_color);
		else
			FuncPara.tipsWindow = false;
	}


	void Update ()
	{
		/*if(Input.GetKeyUp(KeyCode.V))
		{
			Voice(testVoiceStr[testNum], "6");  //6等于1.6倍速左右, 0等于0.95倍速左右，-1---0.85, -6---0.5, -2---0.8,  7等于1.7倍速左右
			//Debug.Log(testVoiceStr[testNum].Length);
			Debug.Log(tipsTestInfo.GetPlayTime(testVoiceStr[testNum]));
			if(testNum >= testVoiceStr.Count - 1)
				testNum = 0;
			else
				testNum++;
		}*/

		/*if(Input.GetKeyUp(KeyCode.T)){
			TipsWindow(true, "哈哈,你猜我现在在想些什么,就不告诉你,就不告诉你,就不告诉你………………………………哈哈哈", true, "top_left");
			Debug.Log("哈哈,你猜我现在在想些什么,就不告诉你,就不告诉你,就不告诉你………………………………哈哈哈".Length);
		}*/
	}
	
}
