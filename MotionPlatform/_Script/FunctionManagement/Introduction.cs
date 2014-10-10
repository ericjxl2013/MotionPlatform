/// <summary>
/// FileName: Introduction.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.02.24
/// Version: 1.0
/// Company: Sunnytech
/// Function: 软件简介功能
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;

public class Introduction : MonoBehaviour {
	
	FunctionManager st_FuncManager;
	Rect introRect;
	public Rect aboutRect;
	
	void Awake () {
		introRect = new Rect(0, 0, 520f, 260f);
		aboutRect = new Rect(0, 0, 515f, 170f);
	}
	
	// Use this for initialization
	void Start () {
		st_FuncManager = gameObject.GetComponent<FunctionManager>();
	}
	
	/// <summary>
	/// Executes the introduction function.
	/// </summary>
	public void IntroductionExecute()
	{
		//面板消失
//		st_FuncManager.PanelOperate("disappear");
		//摄像机切换到相应位置和角度
//		st_FuncManager.CameraControl(CameraEnum.START);
		//出现窗口位置调整
		introRect.x = RectCentre(introRect, "x");
		introRect.y = RectCentre(introRect, "y");
		//语音开始
		st_FuncManager.Voice(FuncPara.introductionString);
		//自动旋转
//		FuncPara.autoRotate = true;
		//功能控制，暂时只允许缩放
//		FunctionData funcClass = new FunctionData();
//		funcClass.AllForbit();
//		FunctionData.IDDic["GENERAL17"] = true;
	}
	
	/// <summary>
	/// Stops the introduction.
	/// </summary>
	public void StopIntroduction()
	{
		FuncPara.introductionOn = false;
		st_FuncManager.Voice("");
		FuncPara.taskActive = Task.None;
		FunctionData funcClass = new FunctionData();
		funcClass.AllAllow();
	}
	
	
	void OnGUI () {
		
		GUI.skin.window = FuncPara.skin_hiMenu.window;
		if(FuncPara.introductionOn){
			introRect = GUI.Window(19, introRect, IntroWindow, "");
		}
		if(FuncPara.aboutUsOn){
			aboutRect = GUI.Window(23, aboutRect, AboutWindow, "");
		}
		GUI.skin = null;
	}
	
	void IntroWindow(int WindowID){
		
		GUI.Label(new Rect(20, 7, 200, 40), FuncPara.introductionName, FuncPara.sty_MsyhWhite19);
		
		if(GUI.Button(new Rect(466, 7, 30, 30), "", FuncPara.sty_Close)){
			StopIntroduction();
		}
		
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.normal.textColor = new Color(0f, 0f, 0, 1.0f);
		GUI.Label(new Rect(15, 50, 480, 220), FuncPara.introductionString);
//		GUI.skin.label = null;
		
		GUI.DragWindow();
	}
	
	void AboutWindow(int WindowID){
		GUI.Label(new Rect(20, 7, 200, 40), "关于我们", FuncPara.sty_MsyhWhite19);
		
		if(GUI.Button(new Rect(466, 7, 30, 30), "", FuncPara.sty_Close)){
			FuncPara.aboutUsOn = false;
		}
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.normal.textColor = new Color(0f, 0f, 0, 1.0f);
		GUI.Label(new Rect(15, 50, 480, 220), FuncPara.aboutString);
//		GUI.skin.label = null;
		
		GUI.DragWindow();
	}
	
	/// <summary>
	/// 计算Rect显示中心位置.
	/// </summary>
	/// <returns>
	/// 左上角坐标值.
	/// </returns>
	/// <param name='aimRect'>
	/// Aim rect.
	/// </param>
	/// <param name='xy'>
	/// Xy.
	/// </param>
	public static float RectCentre(Rect aimRect, string xy){
		if(xy == "x"){
			return (Screen.width - aimRect.width) / 2f;
		}else if(xy == "y"){
			return (Screen.height - aimRect.height) / 2f;
		}else{
			return 0f;
		}
	}
	
}
