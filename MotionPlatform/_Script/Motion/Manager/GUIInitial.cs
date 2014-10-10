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
	

		/////////////////////////////////////////////////////
		FuncPara.skin_hiMenu = (GUISkin)Resources.Load("GUISkin/GeneralSkin");
		FuncPara.skin_hiMenu.customStyles[0].border.bottom = 5;
		FuncPara.skin_hiMenu.customStyles[0].border.right = 5;
		FuncPara.sty_Close.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/Close");
		FuncPara.sty_MsyhBlack15.font = (Font)Resources.Load ("Font/msyh");
		FuncPara.sty_MsyhBlack15.fontSize = 15;
		FuncPara.sty_MsyhBlack15.wordWrap = true;
		FuncPara.sty_MsyhWhite19.font = (Font)Resources.Load ("Font/msyh");
		FuncPara.sty_MsyhWhite19.fontSize = 19;
		FuncPara.sty_MsyhWhite19.wordWrap = true;
		FuncPara.sty_MsyhWhite19.normal.textColor = new Color(1f, 1f, 1f, 1f);
//		FuncPara.sty_TipsWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/Tips");
//		FuncPara.sty_TipsWindow.wordWrap = true;
//		FuncPara.sty_TipsWindow.border.left = 16; FuncPara.sty_TipsWindow.border.right = 16;
//		FuncPara.sty_TipsWindow.border.top = 36;  FuncPara.sty_TipsWindow.border.bottom = 17;
//		FuncPara.sty_TipsWindow.padding.left = 16; FuncPara.sty_TipsWindow.padding.right = 16;
//		FuncPara.sty_TipsWindow.padding.top = 4;  FuncPara.sty_TipsWindow.padding.bottom = 20;
		//Teaching
//		FuncPara.defaultSkin = (GUISkin)Resources.Load("GUISkin/DefaultSkin");
//		FuncPara.defaultFont = (Font)Resources.Load("Font/msyh");
		FuncPara.sty_SlowDown.normal.background = (Texture2D)Resources.Load("Texture/TeachWindow/5");
		FuncPara.sty_SlowDown.active.background = (Texture2D)Resources.Load("Texture/TeachWindow/5-5");
		FuncPara.sty_SpeedUp.normal.background = (Texture2D)Resources.Load("Texture/TeachWindow/4");
		FuncPara.sty_SpeedUp.active.background = (Texture2D)Resources.Load("Texture/TeachWindow/4-4");
		FuncPara.t2d_playN = (Texture2D)Resources.Load("Texture/TeachWindow/7");
		FuncPara.t2d_playA = (Texture2D)Resources.Load("Texture/TeachWindow/7-7");
		FuncPara.t2d_pauseN = (Texture2D)Resources.Load("Texture/TeachWindow/6");
		FuncPara.t2d_pauseA = (Texture2D)Resources.Load("Texture/TeachWindow/6-6");
		FuncPara.sty_Play.normal.background = FuncPara.t2d_playN;
		FuncPara.sty_Play.active.background = FuncPara.t2d_playA;
		FuncPara.sty_Exercise.normal.background = (Texture2D)Resources.Load("Texture/TeachWindow/1");
		FuncPara.sty_Exercise.active.background = (Texture2D)Resources.Load("Texture/TeachWindow/1-1");
		FuncPara.sty_Prev.normal.background = (Texture2D)Resources.Load("Texture/TeachWindow/3");
		FuncPara.sty_Prev.active.background = (Texture2D)Resources.Load("Texture/TeachWindow/3-3");
		FuncPara.sty_Next.normal.background = (Texture2D)Resources.Load("Texture/TeachWindow/2");
		FuncPara.sty_Next.active.background = (Texture2D)Resources.Load("Texture/TeachWindow/2-2");
		FuncPara.sty_Cursor.normal.background = (Texture2D)Resources.Load("Texture/HandsEyes/free_for_job");
		
		//2.0新增参数
		FuncPara.sty_InputBar.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/InputBar");
		FuncPara.sty_InputBar.font = FuncPara.defaultFont;
		FuncPara.sty_InputBar.fontSize = 17;
		FuncPara.sty_InputBar.normal.textColor = Color.black;
		FuncPara.sty_InputBar.alignment = TextAnchor.MiddleLeft;
		FuncPara.sty_InputBar.contentOffset = new Vector2(5f, 0);
		FuncPara.sty_SquareBtn.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/SquareBtnNormal");
		FuncPara.sty_SquareBtn.normal.textColor = Color.white;
		FuncPara.sty_SquareBtn.active.background = (Texture2D)Resources.Load("Texture/CustomMenu/SquareBtnActive");
		FuncPara.sty_SquareBtn.active.textColor = Color.black;
		FuncPara.sty_SquareBtn.font = FuncPara.defaultFont;
		FuncPara.sty_SquareBtn.fontSize = 17;
		FuncPara.sty_SquareBtn.alignment = TextAnchor.MiddleCenter;
		FuncPara.sty_SquareBtn.border.top = 3;
		FuncPara.sty_SquareBtn.border.bottom = 5;
		FuncPara.sty_SquareBtn.border.left = 3;
		FuncPara.sty_SquareBtn.border.right = 5;
		FuncPara.sty_SquareBtn.contentOffset = new Vector2(0, -3f);
		FuncPara.sty_WarnningWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/InformationDesk");
		FuncPara.skin_Scroll = (GUISkin)Resources.Load("GUISkin/ScrollSkin");
		FuncPara.font_STZHONGS = (Font)Resources.Load("Font/STZHONGS");
		FuncPara.sty_PopupWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/PopupWindow");
		
		//3.0新增参数
		FuncPara.sty_HelpWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/Help");
		FuncPara.sty_HelpWindow.wordWrap = true;
		FuncPara.sty_HelpWindow.border.left = 16; FuncPara.sty_HelpWindow.border.right = 16;
		FuncPara.sty_HelpWindow.border.top = 36;  FuncPara.sty_HelpWindow.border.bottom = 17;
		FuncPara.sty_HelpWindow.padding.left = 16; FuncPara.sty_HelpWindow.padding.right = 16;
		FuncPara.sty_HelpWindow.padding.top = 4;  FuncPara.sty_HelpWindow.padding.bottom = 20;
//		FuncPara.t2d_tipsWindow = (Texture2D)Resources.Load("Texture/CustomMenu/Tips");
//		FuncPara.t2d_colorWindow.Add(WindowColor.Blue, (Texture2D)Resources.Load("Texture/CustomMenu/Blue"));  //蓝色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Black, (Texture2D)Resources.Load("Texture/CustomMenu/Black"));  //黑色
//		FuncPara.t2d_colorWindow.Add(WindowColor.BlueViolet, (Texture2D)Resources.Load("Texture/CustomMenu/BlueViolet"));  //紫罗兰色
//		FuncPara.t2d_colorWindow.Add(WindowColor.DarkBlue, (Texture2D)Resources.Load("Texture/CustomMenu/DarkBlue"));  //深蓝色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Green, (Texture2D)Resources.Load("Texture/CustomMenu/Green"));  //绿色
//		FuncPara.t2d_colorWindow.Add(WindowColor.LightBlue, (Texture2D)Resources.Load("Texture/CustomMenu/LightBlue"));  //浅蓝色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Orange, (Texture2D)Resources.Load("Texture/CustomMenu/Orange"));  //橙色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Pink, (Texture2D)Resources.Load("Texture/CustomMenu/Pink"));  //粉色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Purple, (Texture2D)Resources.Load("Texture/CustomMenu/Purple"));  //紫色
//		FuncPara.t2d_colorWindow.Add(WindowColor.Yellow, (Texture2D)Resources.Load("Texture/CustomMenu/Yellow"));  //黄色
		
		FuncPara.sty_PartsWindow.normal.background = (Texture2D)Resources.Load("Texture/CustomMenu/Purple");
		FuncPara.sty_PartsWindow.wordWrap = true;
		FuncPara.sty_PartsWindow.border.left = 16; FuncPara.sty_PartsWindow.border.right = 16;
		FuncPara.sty_PartsWindow.border.top = 36;  FuncPara.sty_PartsWindow.border.bottom = 17;
		FuncPara.sty_PartsWindow.padding.left = 16; FuncPara.sty_PartsWindow.padding.right = 16;
		FuncPara.sty_PartsWindow.padding.top = 4;  FuncPara.sty_PartsWindow.padding.bottom = 20;
	}
}
