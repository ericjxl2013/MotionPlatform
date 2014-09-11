using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Movement {Chai, Zhuang, Yuan}  //三种运动方式：拆、装、原理
public enum MotionState {Teaching, Exercising, Examining}  //教、练、考

public class FuncPara {
	//状态参数
	public static MotionState currentMotion = MotionState.Teaching;  //当前的运动状态
	public static Movement curentMode = Movement.Chai;  //当前运动方式

	//GUI 控制部分参数
	public static bool tipsWindow = false;  //提示窗口控制
	public static bool tipsMove = false;  //提示窗口可以移动
	public static string helpString = "";  //帮助信息内容

	//GUI 显示部分参数
	public static GUIStyle sty_TipsWindow = new GUIStyle();  //提示Style
	public static GUISkin defaultSkin;  //默认Skin
	public static Font defaultFont;  //默认字体
	public static Texture2D t2d_tipsWindow;  //提示Tips背景图
	public static Dictionary<WindowColor, Texture2D> t2d_colorWindow = new Dictionary<WindowColor, Texture2D>();  //提示背景彩图

	public static GUIStyle sty_Cursor = new GUIStyle();  //鼠标贴图
	public static bool cursorShow = false;  //是否显示教学用Cursor
	public static Vector2 cursorPosition = new Vector2(0, 0);  //Cursor位置
	public static Vector2 rightRect = new Vector2(0, 0);  //
}
