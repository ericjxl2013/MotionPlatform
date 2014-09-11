/// <summary>
/// FileName: GUIInitial.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.03.26
/// Version: 1.0
/// Company: Sunnytech
/// Function: GUI参数初始化
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIInitial : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		//Default Parameter
		FuncPara.defaultSkin = (GUISkin)Resources.Load("GUISkin/DefaultSkin");
		FuncPara.defaultFont = (Font)Resources.Load("Font/msyh");
		//Tips Parameter
		FuncPara.sty_TipsWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/Tips");
		FuncPara.sty_TipsWindow.wordWrap = true;
		FuncPara.sty_TipsWindow.border.left = 16; FuncPara.sty_TipsWindow.border.right = 16;
		FuncPara.sty_TipsWindow.border.top = 36;  FuncPara.sty_TipsWindow.border.bottom = 17;
		FuncPara.sty_TipsWindow.padding.left = 16; FuncPara.sty_TipsWindow.padding.right = 16;
		FuncPara.sty_TipsWindow.padding.top = 4;  FuncPara.sty_TipsWindow.padding.bottom = 20;
		FuncPara.t2d_tipsWindow = (Texture2D)Resources.Load("Texture/CustomMenu/Tips");
		FuncPara.t2d_colorWindow.Add(WindowColor.Blue, (Texture2D)Resources.Load("Texture/CustomMenu/Blue"));  //蓝色
		FuncPara.t2d_colorWindow.Add(WindowColor.Black, (Texture2D)Resources.Load("Texture/CustomMenu/Black"));  //黑色
		FuncPara.t2d_colorWindow.Add(WindowColor.BlueViolet, (Texture2D)Resources.Load("Texture/CustomMenu/BlueViolet"));  //紫罗兰色
		FuncPara.t2d_colorWindow.Add(WindowColor.DarkBlue, (Texture2D)Resources.Load("Texture/CustomMenu/DarkBlue"));  //深蓝色
		FuncPara.t2d_colorWindow.Add(WindowColor.Green, (Texture2D)Resources.Load("Texture/CustomMenu/Green"));  //绿色
		FuncPara.t2d_colorWindow.Add(WindowColor.LightBlue, (Texture2D)Resources.Load("Texture/CustomMenu/LightBlue"));  //浅蓝色
		FuncPara.t2d_colorWindow.Add(WindowColor.Orange, (Texture2D)Resources.Load("Texture/CustomMenu/Orange"));  //橙色
		FuncPara.t2d_colorWindow.Add(WindowColor.Pink, (Texture2D)Resources.Load("Texture/CustomMenu/Pink"));  //粉色
		FuncPara.t2d_colorWindow.Add(WindowColor.Purple, (Texture2D)Resources.Load("Texture/CustomMenu/Purple"));  //紫色
		FuncPara.t2d_colorWindow.Add(WindowColor.Yellow, (Texture2D)Resources.Load("Texture/CustomMenu/Yellow"));  //黄色

		FuncPara.sty_Cursor.normal.background = (Texture2D)Resources.Load("Texture/HandsEyes/free_for_job");
		
	}
}
