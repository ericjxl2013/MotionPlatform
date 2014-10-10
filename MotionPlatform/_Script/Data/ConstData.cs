/// <summary>
/// FileName: ConstData.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.03.26
/// Version: 1.0
/// Company: Sunnytech
/// Function: 存储常量信息
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;

//任务和阶段枚举，用于区别每一个阶段
public enum Task {None, Introduction, Structrue, BasicOperation, Task1, Task2, Task3}  //总任务状态
public enum Phase {None, Introduction, CameraMove, MoveOperate,
	PanelSwitch, SystemStart, BackToZero, 
	MachineBackZero, BlankClamping, XYToolSetting, CloseDoor, CenterDrill, Drill, RoughMillSide, FinishMillSide, FinishMillBottom}  //阶段任务状态
//教练考状态
//public enum MotionState {Teaching, Exercising, Examining}

public class ConstData  {
	
	public const string excelPath = "/Resources/ReleaseFile/Excel/";  //Excel所在路径
	public const string excelSettingPath = "/StreamingAssets/Excel/";  //Excel设置文件路径
	public const string functionMangager = "FunctionManager.xls";  //功能管理Excel名字
	public const string hiearchicalMenu = "HierarchicalMenu.xls";  //层级菜单Excel名字
	public const string hideMenu = "MenuSettings.xls";  //隐匿式菜单Excel名字
	public const string iniFilePath = "/StreamingAssets/SystemConfigration/SystemConfig.ini";  //配置文件路径
}
