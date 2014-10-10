/// <summary>
/// FileName: ConfigData.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.07.09
/// Version: 1.0
/// Company: Sunnytech
/// Function: 配置信息参数，用于系统模块配置
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigData {
	//隐匿式菜单设置
	public static bool topMenuOn = true;
	public static bool rightMenuOn = true;
	//上侧隐匿式菜单配置
	public static Dictionary<string, bool> topMenuState = new Dictionary<string, bool>();
	public static Dictionary<string, bool> rightMenuState = new Dictionary<string, bool>();
	
	
}
