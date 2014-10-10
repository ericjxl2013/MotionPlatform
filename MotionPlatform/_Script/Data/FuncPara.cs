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


	/////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////
	public static bool hierarchicalOn = false;  //层级菜单显示控制
	public static float hiMenuCornerW = 0;  //主层级菜单距离屏幕右侧位置
	public static float hiMenuCornerH = 0;  //主层级菜单距离屏幕上方位置
	public static bool introductionOn = false;  //加工中心简介窗口
	public static string aboutString = "";  //关于我们
	public static bool aboutUsOn = false;  //关于我们窗口
	public static string introductionName = "";  //设备简介
	public static string introductionString = "";  //简介信息
	public static bool outlineOn = false;  //引出线显示
	public static bool autoCameraRotate = false;  //自动旋转
	public static bool autoCameraMove = false;  //自动平移
	public static bool autoCameraZoom = false;  //自动缩放
//	public static bool tipsWindow = false;  //提示窗口控制
//	public static bool tipsMove = false;  //提示窗口可以移动
	
	public static Task taskActive = Task.None;  //当前正在进行的任务
	public static Phase phaseActive = Phase.None;  //当前任务所处的阶段
//	public static MotionState currentMotion = MotionState.Teaching;  //当前的运动状态
	public static int TeachingPara = 0;  //视角调整进行中
	public static string TeachingCommand = "";  //教练过程中用于控制每一步运动
	
	public static bool showTeachWindow = false;  //显示教学窗口
	public static string[] phaseString;  //阶段名称字符串
	public static string[] phaseTitle;  //当前阶段名
	public static string excelPath = "";  //excel目录
	public static bool isPlaying = false;  //是否在播放
	public static bool startAlready = false;  //是否已经开始播放了，用于判断当前播放是恢复暂停还是播放不同阶段
	public static float speedRate = 1f;  //播放速率
	public static bool exitWindow = false;  //退出窗口确认
	
	
	#region Public GUI Parameter
	
//	public static GUIStyle sty_TipsWindow = new GUIStyle();  //提示Style
	public static GUISkin skin_hiMenu;  //层级菜单Skin
	public static GUIStyle sty_Close = new GUIStyle();  //关闭按钮
	public static GUIStyle sty_MsyhBlack15 = new GUIStyle();  //黑色雅黑15字体
	public static GUIStyle sty_MsyhWhite19 = new GUIStyle();  //白色雅黑19字体
//	public static GUISkin defaultSkin;  //默认Skin
//	public static Font defaultFont;  //默认字体
	public static GUIStyle sty_SlowDown = new GUIStyle();
	public static GUIStyle sty_SpeedUp = new GUIStyle();
	public static GUIStyle sty_Play = new GUIStyle();
	public static GUIStyle sty_Exercise = new GUIStyle();
	public static GUIStyle sty_Prev = new GUIStyle();
	public static GUIStyle sty_Next = new GUIStyle();
	public static Texture2D t2d_playN;
	public static Texture2D t2d_playA;
	public static Texture2D t2d_pauseN;
	public static Texture2D t2d_pauseA;
//	public static GUIStyle sty_Cursor = new GUIStyle();  //鼠标贴图
	
	//新增部分
	public static GUIStyle sty_InputBar = new GUIStyle();  //输入框背景
	public static GUIStyle sty_SquareBtn = new GUIStyle();  //方形按钮
	public static GUIStyle sty_WarnningWindow = new GUIStyle();  //信息提示背景
	public static GUISkin skin_Scroll;  //滚动条皮肤
	public static Font font_STZHONGS;  //信息提示字符串字体
	public static GUIStyle sty_PopupWindow = new GUIStyle();  //弹出窗口背景
	
	//3.0新增内容
	public static GUIStyle sty_HelpWindow = new GUIStyle();  //帮助提示Style
//	public static Texture2D t2d_tipsWindow;  //提示Tips背景图
//	public static Dictionary<WindowColor, Texture2D> t2d_colorWindow = new Dictionary<WindowColor, Texture2D>();  //提示背景彩图
	
	public static GUIStyle sty_PartsWindow = new GUIStyle();  //部件提示Style
	#endregion
	
	
	
	#region For Test
	public static bool voiceTimer = false;  //是否启用声音计时
	public static bool rectWindow = false;  //查看Rect值
	public static bool testMenuWindow = false;  //测试用菜单窗口
	#endregion
	
	
//	public static bool cursorShow = false;  //是否显示教学用Cursor
//	public static Vector2 cursorPosition = new Vector2(0, 0);  //Cursor位置
//	public static Vector2 rightRect = new Vector2(0, 0);  //
	public static int loopControl = 0;  //用于控制循环线程 
	public static bool helpInfo = true;  //帮助信息
	public static bool helpDisplay = false;  //帮助信息显示控制
	public static bool helpHiding = false;  //用于隐匿式菜单和播放控制条帮助信息显示的逻辑判断
	public static bool helpProgress = false;  //用于隐匿式菜单和播放控制条帮助信息显示的逻辑判断
//	public static string helpString = "";  //帮助信息内容
	public static bool enlargeBtn = false;  //是否放大相应按钮
	public static string btnCode = "";  //放大的按钮代号
	public static bool exerciseWindow = false;  //练习时出现的提示窗口
	public static bool xManualMoveN = false;  //X轴负方向手动移动
	public static bool xManualMoveP = false;  //X轴正方向手动移动
	public static float xMeaure = 0;  //X轴方向测量参数
	public static bool yManualMoveN = false;  //Y轴负方向手动移动
	public static bool yManualMoveP = false;  //Y轴正方向手动移动
	public static float yMeasure = 0;  //Y轴方向测量参数
	public static bool zManualMoveN = false;  //Z轴负方向手动移动
	public static bool zManualMoveP = false;  //Z轴正方向手动移动
	public static float zMeasure = 0;  //Z轴方向测量参数
	public static float handleLoopStop = 0;  //手轮自动转动停止
	
	//新增参数
	//考试相关参数
	public static bool msgInputWindow = false;  //姓名和学号输入窗口
	public static bool examWindow = false;  //窗口控制
	public static bool examStart = false;  //考试是否启动
	public static string studentName = "";  //姓名
	public static string studentID = "";  //学号
	public static float examTime = 40f;  //考试总时间（分钟）
	public static string examTitle = "";  //考试窗口显示的标题
	//3.0新增参数——考试部分
	public static float examScore = 0;  //考试成绩
	public static bool exerciseVoice = true;  //练习时是否要声音
	public static Rect examRect = new Rect(0, 0, 455, 55);
	public static bool examConfirm = false;  //考试结束确认窗口
	
	//3.0新增参数——部件提示部分
	public static bool componentTips = true;  //部件提示控制
	public static bool componentDisplay = false;  //部件提示显示
	public static string componentString = "";  //部件提示信息字符串
	public const string componentsPath  = "/StreamingAssets/ObjectPart/objname.txt";
	public static Dictionary<string, string> componentsDic = new Dictionary<string, string>();  //提示信息容器
	
	//3.0新增参数——案例部分
	public static bool task1Window = false;  //案例1窗口控制
	public static bool task2Window = false;  //案例2窗口控制
}
