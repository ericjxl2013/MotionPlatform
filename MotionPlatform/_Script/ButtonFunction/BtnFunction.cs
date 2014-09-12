/// <summary>
/// FileName: BtnFunction.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.02.21
/// Version: 1.0
/// Company: Sunnytech
/// Function: 用于存储控制程序相应功能的参数
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using System.IO;

public class BtnFunction  {

	public static Dictionary<string, bool> IDDic = new Dictionary<string, bool>();  //功能管理字典数据
	public static Dictionary<string, bool> BtnDic = new Dictionary<string, bool>();  //按钮管理字典
	static string excelPath;  //excel读取路径
	//public static RPCManager link; 
	
	public BtnFunction()
	{
		
	}
	
	public static void SetBtn(string cmd_string, bool is_allow)
	{
		BtnDic[cmd_string] = is_allow;
		//LinkToPad(cmd_string + "," + is_allow.ToString());
	}
	
	
	public static void SetID(string cmd_string, bool is_allow)
	{
		IDDic[cmd_string] = is_allow;
		//LinkToPad(cmd_string + "," + is_allow.ToString());
	}
	
	/// <summary>
	/// 控制Pad端Function.
	/// </summary>
	/// <param name='cmd_string'>
	/// Cmd_string.
	/// </param>
	//public static void LinkToPad(string cmd_string)
	//{
		//if(FuncPara.connectedClientNum > 0)
			//link.networkView.RPC("RPCSender", RPCMode.Others, cmd_string);
	//}
	
	/// <summary>
	/// 是否允许该功能.
	/// </summary>
	/// <param name='ID'>
	/// 功能ID.
	/// </param>
	public static bool Allow(string ID)
	{
		if(IDDic.ContainsKey(ID))  //功能字典数据
			return IDDic[ID];
		else{
			if(BtnDic.ContainsKey(ID))  //按钮字典数据
				return BtnDic[ID];
			else   //默认为true
				return true;
		}		
	}
	
	/// <summary>
	/// 功能控制参数初始化，默认全部允许.
	/// </summary>
	static public void Initialize()
	{
		IDDic.Clear();
		BtnDic.Clear();


		//link = GameObject.Find("MainScript").GetComponent<RPCManager>();
		excelPath = MotionPara.taskRootPath + MotionPara.taskName + "/FunctionManager.xls";
		ExcelOperator excelOp = new ExcelOperator();
		DataTable funcData = excelOp.ExcelReader(excelPath, "FUNCTION", "A", "A");
		for(int i = 0; i < funcData.Rows.Count; i++){
			string ID = Convert.ToString(funcData.Rows[i][0]).ToUpper();
			if(ID != "")
				IDDic.Add(ID, true);
		}
		DataTable btnData = excelOp.ExcelReader(excelPath, "BUTTON", "A", "A");
		for(int i = 0; i < btnData.Rows.Count; i++){
			string ID = Convert.ToString(btnData.Rows[i][0]).ToUpper();
			if(ID != "")
				BtnDic.Add(ID, true);
		}
	}
	
	/// <summary>
	/// 允许所有功能.
	/// </summary>
	public static void AllAllow()
	{
		List<string> buffer = new List<string> ();
		foreach(string keys in IDDic.Keys){
			buffer.Add(keys);
		}
		foreach(string keys in buffer){
			IDDic[keys] = true;
		}
		List<string> bufferBtn = new List<string> ();
		foreach(string keys in BtnDic.Keys){
			bufferBtn.Add(keys);
		}
		foreach(string keys in bufferBtn){
			BtnDic[keys] = true;
		}
		//if(FuncPara.connectedClientNum > 0){
			//link.networkView.RPC("MostRPC", RPCMode.Others, true);
		//}
	}
	
	/// <summary>
	/// 禁止所有功能.
	/// </summary>
	public static void AllForbit()
	{
		List<string> buffer = new List<string> ();
		foreach(string keys in IDDic.Keys){
			buffer.Add(keys);
		}
		foreach(string keys in buffer){
			IDDic[keys] = false;
		}
		List<string> bufferBtn = new List<string> ();
		foreach(string keys in BtnDic.Keys){
			bufferBtn.Add(keys);
		}
		foreach(string keys in bufferBtn){
			BtnDic[keys] = false;
		}
		//if(FuncPara.connectedClientNum > 0){
			//link.networkView.RPC("MostRPC", RPCMode.Others, false);
		//}
	}
	
	/// <summary>
	/// 所有按钮允许动作.
	/// </summary>
	public static void BtnAllow()
	{
		List<string> bufferBtn = new List<string> ();
		foreach(string keys in BtnDic.Keys){
			bufferBtn.Add(keys);
		}
		foreach(string keys in bufferBtn){
			BtnDic[keys] = true;
		}
	}
	
	/// <summary>
	/// 所有按钮禁止动作.
	/// </summary>
	public static void BtnForbit()
	{
		List<string> bufferBtn = new List<string> ();
		foreach(string keys in BtnDic.Keys){
			bufferBtn.Add(keys);
		}
		foreach(string keys in bufferBtn){
			BtnDic[keys] = false;
		}
	}
	
	/// <summary>
	/// 根据响应表格禁止相应的功能.
	/// </summary>
	/// <param name='sheet_name'>
	/// Sheet_name.
	/// </param>
	public void ForbitFunction(string sheet_name)
	{
		ExcelOperator excelOp = new ExcelOperator();
		DataTable funcData = excelOp.ExcelReader(excelPath, sheet_name, "A", "A");
		AllAllow();
		string keys = "";
		for(int i = 0; i < funcData.Rows.Count; i++){
			keys = Convert.ToString(funcData.Rows[i][0]);
			if(IDDic.ContainsKey(keys))
				IDDic[keys] = false;
			else
				Debug.LogWarning("ID NOT FOUND! Check sheet '" + sheet_name + "', Position 'A" + (i + 2).ToString() + "';");
		}
	}
}
