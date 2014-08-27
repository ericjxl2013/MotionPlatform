/// <summary>
/// <Filename>: ExcelOperator.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.18
/// Version: 1.0
/// Company: Sunnytech
/// Function: 运动相关控制和计算类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

//中间过程处理类
public class InterData 
{
    private static Dictionary<string, TriggerType> triggerDic = new Dictionary<string, TriggerType>();
	public static void InterDataInit()
	{
        triggerDic.Add("MOUSEDOWN", TriggerType.MouseDown); triggerDic.Add("MOUSEDOWN2", TriggerType.MouseDown2);
        triggerDic.Add("MOUSEUP", TriggerType.MouseUp); triggerDic.Add("MOUSEDRAG", TriggerType.MouseDrag);
        triggerDic.Add("SCROLLWHEEL", TriggerType.ScrollWheel); triggerDic.Add("KEYDOWN", TriggerType.KeyDown);
		triggerDic.Add("KEYUP", TriggerType.KeyUp); triggerDic.Add("BUTTON", TriggerType.Button);
	}

	/// <summary>
	/// 命令格式正确与否判断.
	/// </summary>
	/// <returns>相应运动的ID.</returns>
	/// <param name="cmd_str">从Excel中获取的命令字符.</param>
	/// <param name="motion_type">当前运动类型字符.</param>
	public static string CmdCheck(string cmd_str, string motion_type)
	{
		string[] cmdArray = cmd_str.Split('@');
		if(cmdArray.Length != 2)
			return "";
		if(cmdArray[0].ToUpper() != motion_type.ToUpper())
			return "";
		return cmdArray[1];
	}

	/// <summary>
	/// 命令格式正确与否判断,同时判断多个Command.
	/// </summary>
	/// <returns>相应运动的ID.</returns>
	/// <param name="cmd_str">从Excel中获取的命令字符.</param>
	/// <param name="motion_type">当前运动类型字符.</param>
	/// <param name="general_key">当前的key string.</param>
	public static string CmdCheck(string cmd_str, string[] motion_type, ref string general_key)
	{
		general_key = "";
		string[] cmdArray = cmd_str.Split('@');
		if(cmdArray.Length != 2)
			return "";
		for(int i = 0; i < motion_type.Length; i++){
			if(cmdArray[0].ToUpper() == motion_type[i].ToUpper()){
				general_key = cmdArray[1];
				return cmdArray[0].ToUpper();
			}
		}
		return "";
	}

    //获得触发类型
    public static TriggerType[] GetTriggerType(string type_string, ref bool is_right)
    {
        List<TriggerType> returnType = new List<TriggerType>();
        if (type_string == "")
        {
            is_right = false;
            return returnType.ToArray();
        }
        string[] typeArray = type_string.Split('&');
        for (int i = 0; i < typeArray.Length; i++)
        {
            if (triggerDic.ContainsKey(typeArray[i]))
            {
                returnType.Add(triggerDic[typeArray[i]]);
            }
            else 
            {
                is_right = false;
                break;
            }
        }
        return returnType.ToArray();
    }

    //获得字符串数组
    public static string[] GetStringArray(string type_string, ref bool is_right)
    {
        string[] typeArray = type_string.Split('&');
        return typeArray;
    }
}

/// <summary>
/// 计算基类.
/// </summary>
public class BaseCompute
{

	public BaseCompute()
	{

	}

	//字符串转化为Vector3
	public Vector3 Vector3Conversion(string info, ref bool is_right)
	{
		Vector3 rVector3 = new Vector3(0, 0, 0);
		string[] infoArray = info.Split(',','，');
		try{
			rVector3.x = (float)Convert.ToDouble(infoArray[0]);
			rVector3.y = (float)Convert.ToDouble(infoArray[1]);
			rVector3.z = (float)Convert.ToDouble(infoArray[2]);
		}catch{
			is_right = false;
		}
		return rVector3;
	}

	//字符串转化为float
	public float FloatConversion(string info, ref bool is_right)
	{
		float rFloat = 0;
		try{
			rFloat = float.Parse(info);
		}catch{
			is_right = false;
		}
		return rFloat;
	}

	//字符串转化为int
	protected int IntConversion(string info, ref bool is_right)
	{
		int rInt = 0;
		try{
			rInt = int.Parse(info);
		}catch{
			is_right = false;
		}
		return rInt;
	}

	//角度值统一转化
	protected Vector3 AngleConversion(Vector3 ori_vec)
	{
		if(ori_vec.x < 0)
			ori_vec.x += 360f;
		if(ori_vec.y < 0)
			ori_vec.y += 360f;
		if(ori_vec.z < 0)
			ori_vec.z += 360f;
		return ori_vec;
	}

	protected Vector3 AngleDiff(Vector3 ori_vec)
	{
		if(ori_vec.x < -180f)
			ori_vec.x += 360f;
		else if(ori_vec.x > 180f)
			ori_vec.x = 360f - ori_vec.x;
		if(ori_vec.y < -180f)
			ori_vec.y += 360f;
		else if(ori_vec.y > 180f)
			ori_vec.y = 360f - ori_vec.y;
		if(ori_vec.z < -180f)
			ori_vec.z += 360f;
		else if(ori_vec.z > 180f)
			ori_vec.z = 360f - ori_vec.z;
		return ori_vec;
	}
}

/// <summary>
/// 用于定位Excel表格中的错误位置信息，根据外部TXT文件定位.
/// </summary>
public class ErrorLocation {
	//相关Excel表格位置存储处
	private static Dictionary<string, string> _locationDic;

	public static void Initialize ()
	{
		_locationDic = new Dictionary<string, string>();
		if(!File.Exists(Application.streamingAssetsPath + "/ErrorConfiguration/ErrorLocation.txt")){
			Debug.LogError(Application.streamingAssetsPath + 
			  "/ErrorConfiguration/ErrorLocation.txt file doesn't exist!");
			return;
		}
		FileStream fStream = new FileStream(Application.streamingAssetsPath + 
		      "/ErrorConfiguration/ErrorLocation.txt", FileMode.Open, FileAccess.Read);
		StreamReader fileReader = new StreamReader(fStream);
		string lineReader = fileReader.ReadLine();
		string[] strArray;
		while(lineReader != null){
			strArray = lineReader.Split(',');
			_locationDic.Add(strArray[0] + "-" + strArray[1], strArray[2]);
			//Debug.Log(strArray[0] + "-" + strArray[1] + " -> " + strArray[2]);
			lineReader = fileReader.ReadLine();
		}
		fileReader.Close();

	}

	//返回错误信息位置
	public static string Locate(string key, string sub_key, int column_number)
	{
		string rStr = "";
		if(_locationDic.ContainsKey(key.ToUpper() + "-" + sub_key.ToUpper())){
			rStr = " -> " + key.ToUpper() + " -> "+ "( " + (column_number + 2) + ", " 
				+ _locationDic[key.ToUpper() + "-" + sub_key.ToUpper()] + " )";
		}else{
			rStr = " -> " + key.ToUpper() + " -> "+ "( " + key.ToUpper() + "-" + sub_key.ToUpper() + 
				" is not included in the ErrorLocation.txt file! )";
		}
		return "错误位置：" + MotionPara.dataRootPath + MotionPara.excelName + ".xls" + rStr;
	}

	//返回错误信息位置
	public static string Locate(string key, string sub_key, string id)
	{
		string rStr = "";
		if(_locationDic.ContainsKey(key.ToUpper() + "-" + sub_key.ToUpper())){
			rStr = " -> " + key.ToUpper() + " -> " + "( ID:" + id + ", " + _locationDic[key.ToUpper() + 
				"-" + sub_key.ToUpper()] + " )";
		}else{
			rStr = " -> " + key.ToUpper() + " -> " + "( " + key.ToUpper() + "-" + sub_key.ToUpper() +
				" is not included in the ErrorLocation.txt file! )";
        }
		return "错误位置：" + MotionPara.dataRootPath + MotionPara.excelName + ".xls" + rStr;
    }
}

