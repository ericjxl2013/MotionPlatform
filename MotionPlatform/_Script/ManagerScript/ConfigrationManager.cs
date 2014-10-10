/// <summary>
/// FileName: ConfigrationManager.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.07.09
/// Version: 1.0
/// Company: Sunnytech
/// Function: 配置文件管理
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;

public class ConfigrationManager : MonoBehaviour {
	private string iniPath = "";
	
	void Awake ()
	{
		iniPath = Application.dataPath + ConstData.iniFilePath;
		ReadSystemConfigFile (iniPath);
		//WriteSystemConfigFile(iniPath);
	}
	
	// Use this for initialization
	void Start () 
	{
		
		
	}
	
	//读系统配置
	private void ReadSystemConfigFile(string file_path)
	{
		//实例化ini文件管理类
		INIClass configReader = new INIClass(file_path);
		
		//隐匿式菜单设置
		ConfigData.topMenuOn = configReader.ReadBool("HidingMenu", "TopActive", true);
		ConfigData.rightMenuOn = configReader.ReadBool("HidingMenu", "RightActive", true);
		//上侧隐匿式菜单配置
		ConfigData.topMenuState.Clear();
		ConfigData.topMenuState.Add("Start", configReader.ReadBool("TopHidingMenu", "Start", true));
		ConfigData.topMenuState.Add("Equip", configReader.ReadBool("TopHidingMenu", "Equip", true));
		ConfigData.topMenuState.Add("Exam", configReader.ReadBool("TopHidingMenu", "Exam", true));
		ConfigData.topMenuState.Add("Knowledge", configReader.ReadBool("TopHidingMenu", "Knowledge", true));
		ConfigData.topMenuState.Add("ExamOnline", configReader.ReadBool("TopHidingMenu", "ExamOnline", true));
		ConfigData.topMenuState.Add("Observation", configReader.ReadBool("TopHidingMenu", "Observation", true));
		ConfigData.topMenuState.Add("BestView", configReader.ReadBool("TopHidingMenu", "BestView", true));

		ConfigData.topMenuState.Add("ComponentTips", configReader.ReadBool("TopHidingMenu", "ComponentTips", true));

		ConfigData.topMenuState.Add("Server", configReader.ReadBool("TopHidingMenu", "Server", true));
		ConfigData.topMenuState.Add("System", configReader.ReadBool("TopHidingMenu", "System", true));
		ConfigData.topMenuState.Add("About", configReader.ReadBool("TopHidingMenu", "About", true));
		ConfigData.topMenuState.Add("Exit", configReader.ReadBool("TopHidingMenu", "Exit", true));
		//右侧隐匿式菜单配置
		ConfigData.rightMenuState.Clear();
		ConfigData.rightMenuState.Add("Hide", configReader.ReadBool("RightHidingMenu", "Hide", true));
		ConfigData.rightMenuState.Add("Show", configReader.ReadBool("RightHidingMenu", "Show", true));
		ConfigData.rightMenuState.Add("Transparent", configReader.ReadBool("RightHidingMenu", "Transparent", true));
		ConfigData.rightMenuState.Add("AllShow", configReader.ReadBool("RightHidingMenu", "AllShow", true));
		ConfigData.rightMenuState.Add("AllHide", configReader.ReadBool("RightHidingMenu", "AllHide", true));
		ConfigData.rightMenuState.Add("Restore", configReader.ReadBool("RightHidingMenu", "Restore", true));
		ConfigData.rightMenuState.Add("ShowTree", configReader.ReadBool("RightHidingMenu", "ShowTree", true));
		ConfigData.rightMenuState.Add("Slice", configReader.ReadBool("RightHidingMenu", "Slice", true));
		ConfigData.rightMenuState.Add("Rotate", configReader.ReadBool("RightHidingMenu", "Rotate", true));
		
		
		
		
	}
	
	//写系统配置文件，注意:当重新写入该文件时，先把该文件删除
	private void WriteSystemConfigFile(string file_path)
	{
		INIClass configWriter = new INIClass(file_path);
		//隐匿式菜单设置
		configWriter.WriteNotes("隐匿式菜单启用设置");
		configWriter.WriteBool("HidingMenu", "TopActive", true);
		configWriter.WriteBool("HidingMenu", "RightActive", true);
		configWriter.WriteNullLine();
		
		//上侧隐匿式菜单配置
		configWriter.WriteNotes("上侧隐匿式菜单配置");
		configWriter.WriteBool("TopHidingMenu", "Start", true);
		configWriter.WriteBool("TopHidingMenu", "Equip", true);
		configWriter.WriteBool("TopHidingMenu", "Exam", true);
		configWriter.WriteBool("TopHidingMenu", "Knowledge", false);
		configWriter.WriteBool("TopHidingMenu", "ExamOnline", false);
		configWriter.WriteBool("TopHidingMenu", "Observation", true);
		configWriter.WriteBool("TopHidingMenu", "BestView", true);
		configWriter.WriteBool("TopHidingMenu", "Server", false);
		configWriter.WriteBool("TopHidingMenu", "System", true);
		configWriter.WriteBool("TopHidingMenu", "About", true);
		configWriter.WriteBool("TopHidingMenu", "Exit", true);
		configWriter.WriteNullLine();
		
		//右侧隐匿式菜单配置
		configWriter.WriteNotes("右侧隐匿式菜单配置");
		configWriter.WriteBool("RightHidingMenu", "Hide", true);
		configWriter.WriteBool("RightHidingMenu", "Show", true);
		configWriter.WriteBool("RightHidingMenu", "Transparent", true);
		configWriter.WriteBool("RightHidingMenu", "AllShow", true);
		configWriter.WriteBool("RightHidingMenu", "AllHide", true);
		configWriter.WriteBool("RightHidingMenu", "Restore", true);
		configWriter.WriteBool("RightHidingMenu", "ShowTree", true);
		configWriter.WriteBool("RightHidingMenu", "Slice", true);
		configWriter.WriteBool("RightHidingMenu", "Rotate", true);
		configWriter.WriteNullLine();
		
		//程序动画设置
		configWriter.WriteNotes("是否启用程序动画模块（程序员编程用）");
		configWriter.WriteBool("ProgramAnimation", "Active", false);
		configWriter.WriteNullLine();
		
		//程序动画设置
		configWriter.WriteNotes("启动程序是焦点模式");
		configWriter.WriteBool("FocusMode", "IsFocus", true);
		configWriter.WriteNullLine();
		
	}
}
