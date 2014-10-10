/// <summary>
/// FileName: FunctionData.cs
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

public class FunctionData {

	public static Dictionary<string, bool> IDDic = new Dictionary<string, bool>();  //功能管理字典数据
	public static Dictionary<string, bool> BtnDic = new Dictionary<string, bool>();  //按钮管理字典
	static string excelPath;  //excel读取路径
	
	public FunctionData()
	{
		
	}
	
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
		excelPath = Application.dataPath + ConstData.excelPath;
		ExcelOperator excelOp = new ExcelOperator();
		DataTable funcData = excelOp.ExcelReader(excelPath + ConstData.functionMangager, "MANAGER", "A", "A");
		for(int i = 0; i < funcData.Rows.Count; i++){
			string ID = Convert.ToString(funcData.Rows[i][0]);
			if(ID != "")
				IDDic.Add(ID, true);
		}
		DataTable btnData = excelOp.ExcelReader(excelPath + ConstData.functionMangager, "BUTTON", "A", "A");
		for(int i = 0; i < btnData.Rows.Count; i++){
			string ID = Convert.ToString(btnData.Rows[i][0]);
			if(ID != "")
				BtnDic.Add(ID, true);
		}
//		DataTable introData = excelOp.ExcelReader(excelPath + SystemArguments.functionMangager, "INTRODUCTION", "A", "A");
//		FuncPara.introductionString = Convert.ToString(introData.Rows[0][0]);
		//软件简介字符串
		JsonOperator jsonOp = new JsonOperator();
		DataTable jsonTable = jsonOp.JsonReader(Application.dataPath + "/Resources/ReleaseFile/Excel/Introduction.json", "Introduction");
		FuncPara.introductionName = jsonTable.Rows[0][0].ToString();
		FuncPara.introductionString = jsonTable.Rows[0][1].ToString();

//		FuncPara.introductionName = "加工中心简介";
//		FuncPara.introductionString = "数控加工中心是由机械设备与数控系统组成的适用于加工复杂零件的高效率自动化机床。" +
//			"数控加工中心是目前世界上产量最高、应用最广的数控机床之一。它的综合加工能力较强，工件一次装夹后能完成较多的加工内容，" +
//			"加工精度较高。\n数控加工中心是一种功能较全的数控加工机床。它把铣削、镗削、钻削、攻螺纹和切削螺纹等功能集中在一台设备上，" +
//			"使其具有多种工艺手段。加工中心设置有刀库，刀库中存放着不同数量的各种刀具或检具，在加工过程中由程序自动选用和更换。" +
//			"这是它与数控铣床、数控镗床的主要区别。特别是对于必需采用工装和专机设备来保证产品质量和效率的工件。" +
//			"这会为新产品的研制和改型换代节省大量的时间和费用，从而使企业具有较强的竞争能力。";
	}
	
	/// <summary>
	/// 允许所有功能.
	/// </summary>
	public void AllAllow()
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
	}
	
	/// <summary>
	/// 禁止所有功能.
	/// </summary>
	public void AllForbit()
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
	}
	
	/// <summary>
	/// 所有按钮允许动作.
	/// </summary>
	public void BtnAllow(){
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
	public void BtnForbit(){
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
		DataTable funcData = excelOp.ExcelReader(excelPath + ConstData.functionMangager, sheet_name, "A", "A");
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
