using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotionPara
{

	//当前设备名，用于索引到相应的引出线信息
	public static string deviceName = "testEquipment";

	//当前任务名称，用于索引到相应的资源文件
	public static string taskName = "";
	//当前任务的根目录
	public static string taskRootPath = "";
	//任务阶段名称列表
	public static List<string> phaseNameList = new List<string>();
	//是否为编辑器模式下，为true时，读取表格时格式错误会报错
	public static bool isEditor = true;
	//Corutione是否终止
	public static bool shouldStop = false;


	//功能表根目录
	public static string dataRootPath = "";
	//当前表格名字
	public static  string excelName = "";
	//主表当前行号
	public static  int mainRowNumber = 0;
	//运动暂停控制
	public static bool PauseControl = false;
	//运动激活控制
	public static bool MotionActive = false;
	//运动速率
	public static float SpeedRate = 1.0f;

	//Excel和Camera运动通用参数，来自ToolsVariable.xls
	//安全高度
	public static float safeHeight = 1.0f;
	//程序中在默认条件下物体移动的统一速度
	public static float toolMoveSpeed = 1.0f;
	//程序中在默认条件下物体旋转的统一速度
	public static float toolRotateSpeed = 60.0f;
	//程序中在默认条件下铜棒名字
	public static string copperName = "T02_1";
	//程序中在默认条件下铜棒的敲击速度
	public static float copperHitSpeed = 1.0f;
	//程序中在默认条件下铜棒敲击一次前进的距离
	public static float copperForwardDis = 1.0f;
	//程序中在默认条件下铜棒用Y轴负方向敲击其他物体
	public static Vector3 copperVector = new Vector3(0f, -1f, 0f);
	//程序中在默认条件下扳手拧紧、拧松螺钉时每次换角度退出的距离
	public static float wrenchBackDis = 0.05f;  
	//程序中在默认条件下扳手拧进、拧出的速度
	public static float wrenchSpeed = 60f;  
	//程序中在默认条件下拧松、拧紧螺钉每次移动的距离系数
	public static float screwBackRate = 0.0002F;  
	//程序中在默认条件下拧进、拧出每圈移动距离系数
	public static float rotateDegreeRate = 0.0025F;  

	
	//摄像机直线速度
	public static float cameraLineSpeed = 0;
	//摄像机角速度
	public static float cameraAngleSpeed = 0;
	//程序动画ID
	public static List<string> programMotionID = new List<string>();
	//预存工具名称
	public static List<string> toolsName = new List<string>();
	//预存工具初始位置
	public static List<Vector3> toolsInitPos = new List<Vector3>();
	//预存工具初始角度
	public static List<Vector3> toolsInitEuler = new List<Vector3>();

    //循环控制参数
    //单步播放循环控制
    public static bool singlePlay = false;
    //触发循环控制
    public static bool triggerPlay = false;

	//鼠标贴图直线运动速度
	public static float Move_Speed = 80F;
	//鼠标贴图旋转运动角速度
	public static float Rotate_Speed = 60F;  
}
