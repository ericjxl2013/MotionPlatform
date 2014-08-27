/// <summary>
/// <Filename>: EditorManager.cs
/// Author: Jiang Xiaolong
/// Created: 2014.07.20
/// Version: 1.0
/// Company: Sunnytech
/// Function: 美工编辑人员编辑工具管理脚本
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
//using System.Diagnostics;

public class LocationManager : MonoBehaviour {

	//private string rootDirectory = "";

	public bool isJson = false;  //是否用Json读取

	// Use this for initialization
	void Start () {
		//rootDirectory = Application.dataPath + "/Resources";
		isJson = true;
	}

	//生成节点初始化表
	public void LocationCreate(string nodeName, string column_name, bool cover_flag)
	{
		if (isJson)  //用Json方式
		{
			JsonLocationCreate(nodeName, column_name, cover_flag);
		}
		else  //用Excel方式
		{
			ExcelLocationCreate(nodeName, column_name, cover_flag);
		}
	}

	//Excel数据处理
	private void ExcelLocationCreate(string nodeName, string column_name, bool cover_flag)
	{
		nodeName = nodeName.ToUpper();
		//检查是否存在以该数模命名的文件夹
		string locationPath = MotionPara.taskRootPath + MotionPara.taskName + "/";
		string subPath = "";
		if (nodeName.StartsWith("C"))
		{
			subPath = "C/";
		}
		else if (nodeName.StartsWith("Y"))
		{
			subPath = "Y/";
		}
		else if (nodeName.StartsWith("Z"))
		{
			subPath = "Z/";
		}
		else
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		//检查数字是否正确
		int fileNum = 0;
		try
		{
			fileNum = int.Parse(nodeName.Substring(1));
		}
		catch
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		string filePreName = nodeName.Substring(0, 1) + fileNum.ToString();
		//检验根目录是否存在
		if (Directory.Exists(locationPath))
		{
			//Excel处理类
			ExcelOperator excelOp = new ExcelOperator();
			if (File.Exists(locationPath + "ObjectName.xls"))
			{
				//首先处理ObjectName文件
				DataTable objNameTab = excelOp.ExcelReader(locationPath + "ObjectName.xls", "NAME", "A", "A");
				//为Excel写入做准备
				List<string> colName = new List<string>();
				colName.Add("NAME"); colName.Add("POSITION"); colName.Add("EULERANGLES");
				List<List<string>> fileContent = new List<List<string>>();
				List<string> colContent = new List<string>();
				string objName = "";
				string objPosition = "";
				string objRotation = "";
				Transform objTransform = null;
				for (int i = 0; i < objNameTab.Rows.Count; i++)
				{
					try
					{
						objTransform = GameObject.Find((string)objNameTab.Rows[i][0].ToString()).transform;
					}
					catch
					{
						UnityEngine.Debug.LogError((string)objNameTab.Rows[i][0].ToString() + "，该物体在当前场景中不存在，请确认！");
						return;
					}
					objName = objTransform.name;
					objPosition = objTransform.position.x.ToString() + "," + objTransform.position.y.ToString() +
						"," + objTransform.position.z.ToString();
					objRotation = objTransform.eulerAngles.x.ToString() + "," + objTransform.eulerAngles.y.ToString() +
						"," + objTransform.eulerAngles.z.ToString();
					colContent = new List<string>();
					colContent.Add(objName); colContent.Add(objPosition); colContent.Add(objRotation);
					fileContent.Add(colContent);
				}
				excelOp.ExcelWriter(locationPath + subPath + filePreName + "Location.xls", "LOCATION" + column_name, colName, fileContent, cover_flag);
				UnityEngine.Debug.Log(filePreName + "Location.xls表格LOCATION" + column_name + " sheet生成成功！");
				//然后处理SpecialName文件
				string speName = "";
				string speType = "";
				string speValue = "";
				if (File.Exists(locationPath + "ObjectName.xls"))
				{
					DataTable specialNameTab = excelOp.ExcelReader(locationPath + "ObjectName.xls", "SPECIAL", "A", "B");
					if (specialNameTab.Rows.Count > 0)
					{
						List<string> colName1 = new List<string>();
						List<List<string>> fileContent1 = new List<List<string>>();
						List<string> colContent1 = new List<string>();
						colName1.Add("NAME"); colName1.Add("TYPE"); colName1.Add("CONTENT");
						bool isRight = true;
						for (int i = 0; i < specialNameTab.Rows.Count; i++)
						{
							speName = (string)specialNameTab.Rows[i][0].ToString();
							speType = (string)specialNameTab.Rows[i][1].ToString();
							speValue = GetSpecialObjInfo(speName, speType, ref isRight);
							if (!isRight)
							{
								return;
							}
							else
							{
								colContent1 = new List<string>();
								colContent1.Add(speName); colContent1.Add(speType); colContent1.Add(speValue);
								fileContent1.Add(colContent1);
							}
						}
						excelOp.ExcelWriter(locationPath + subPath + filePreName + "Location.xls", "SPECIAL" + column_name, colName1, fileContent1, false);
						UnityEngine.Debug.Log(filePreName + "Location.xls表格SPECIAL" + column_name + " sheet生成成功！");
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError(locationPath + "ObjectName.xls，该文件不存在，请确认！");
				return;
			}
		}
		else
		{
			UnityEngine.Debug.LogError(locationPath + "，该路径不存在，请检查！");
			return;
		}
	}

	//JASON读取
	private void JsonLocationCreate(string nodeName, string column_name, bool cover_flag)
	{
		nodeName = nodeName.ToUpper();
		//检查是否存在以该数模命名的文件夹
		string locationPath = MotionPara.taskRootPath + MotionPara.taskName + "/";
		string subPath = "";
		if (nodeName.StartsWith("C"))
		{
			subPath = "C/";
		}
		else if (nodeName.StartsWith("Y"))
		{
			subPath = "Y/";
		}
		else if (nodeName.StartsWith("Z"))
		{
			subPath = "Z/";
		}
		else
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		//检查数字是否正确
		int fileNum = 0;
		try
		{
			fileNum = int.Parse(nodeName.Substring(1));
		}
		catch
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		string filePreName = nodeName.Substring(0, 1) + fileNum.ToString();
		//检验根目录是否存在
		if (Directory.Exists(locationPath))
		{
			//Json处理类
			JsonOperator jsonOp = new JsonOperator();
			ExcelOperator excelOp = new ExcelOperator();
			if (File.Exists(locationPath + "ObjectName.xls"))
			{
				//首先处理ObjectName文件
				DataTable objNameTab = excelOp.ExcelReader(locationPath + "ObjectName.xls", "NAME", "A", "A");
				//为Json写入做准备
				string[] nameArray = new string[] { "NAME", "POSITION", "EULERANGLES" };
				string[,] contentArray = new string[3, objNameTab.Rows.Count];
				string objName = "";
				string objPosition = "";
				string objRotation = "";
				Transform objTransform = null;
				for (int i = 0; i < objNameTab.Rows.Count; i++)
				{
					try
					{
						objTransform = GameObject.Find((string)objNameTab.Rows[i][0].ToString()).transform;
					}
					catch
					{
						UnityEngine.Debug.LogError((string)objNameTab.Rows[i][0].ToString() + "，该物体在当前场景中不存在，请确认！");
						return;
					}
					objName = objTransform.name;
					objPosition = objTransform.position.x.ToString() + "," + objTransform.position.y.ToString() +
						"," + objTransform.position.z.ToString();
					objRotation = objTransform.eulerAngles.x.ToString() + "," + objTransform.eulerAngles.y.ToString() +
						"," + objTransform.eulerAngles.z.ToString();
					contentArray[0, i] = objName;
					contentArray[1, i] = objPosition;
					contentArray[2, i] = objRotation;
				}
				jsonOp.JsonWriter(locationPath + subPath + filePreName + "Location.json", "LOCATION" + column_name, nameArray, contentArray, cover_flag);
				UnityEngine.Debug.Log(filePreName + "Location.json表格LOCATION" + column_name + " sheet生成成功！");
				//然后处理SpecialName文件
				string speName = "";
				string speType = "";
				string speValue = "";
				if (File.Exists(locationPath + "ObjectName.xls"))
				{
					DataTable specialNameTab = excelOp.ExcelReader(locationPath + "ObjectName.xls", "SPECIAL", "A", "B");
					if (specialNameTab.Rows.Count > 0)
					{
						string[] nameArrayS = new string[] { "NAME", "TYPE", "CONTENT" };
						string[,] contentArrayS = new string[3, specialNameTab.Rows.Count];
						bool isRight = true;
						for (int i = 0; i < specialNameTab.Rows.Count; i++)
						{
							speName = (string)specialNameTab.Rows[i][0].ToString();
							speType = (string)specialNameTab.Rows[i][1].ToString();
							speValue = GetSpecialObjInfo(speName, speType, ref isRight);
							if (!isRight)
							{
								return;
							}
							else
							{
								contentArrayS[0, i] = speName;
								contentArrayS[1, i] = speType;
								contentArrayS[2, i] = speValue;
							}
						}
						jsonOp.JsonWriter(locationPath + subPath + filePreName + "Location.json", "SPECIAL" + column_name, nameArrayS, contentArrayS, false);
						UnityEngine.Debug.Log(filePreName + "Location.json表格SPECIAL" + column_name + " sheet生成成功！");
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError(locationPath + "ObjectName.xls，该文件不存在，请确认！");
				return;
			}
		}
		else
		{
			UnityEngine.Debug.LogError(locationPath + "，该路径不存在，请检查！");
			return;
		}
	}

	//位置设置
	public void LocationSet(string nodeName, string column_name)
	{
		if (isJson)  //用Json
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			JsonLocationSet(nodeName, column_name);
			//sw.Stop();
			//UnityEngine.Debug.LogWarning(sw.ElapsedTicks / (decimal)Stopwatch.Frequency);
		}
		else  //用Excel
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			ExcelLocationSet(nodeName, column_name);
			//sw.Stop();
			//UnityEngine.Debug.LogWarning(sw.ElapsedTicks / (decimal)Stopwatch.Frequency);
		}
	}

	//Excel数据位置设置
	private void ExcelLocationSet(string nodeName, string column_name)
	{
		nodeName = nodeName.ToUpper();
		//检查是否存在以该数模命名的文件夹
		string locationPath = MotionPara.taskRootPath + MotionPara.taskName + "/";
		string subPath = "";
		if (nodeName.StartsWith("C"))
		{
			subPath = "C/";
		}
		else if (nodeName.StartsWith("Y"))
		{
			subPath = "Y/";
		}
		else if (nodeName.StartsWith("Z"))
		{
			subPath = "Z/";
		}
		else
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		//检查数字是否正确
		int fileNum = 0;
		try
		{
			fileNum = int.Parse(nodeName.Substring(1));
		}
		catch
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		string filePreName = nodeName.Substring(0, 1) + fileNum.ToString();
		//Excel处理类
		ExcelOperator excelOp = new ExcelOperator();
		if (File.Exists(locationPath + subPath + filePreName + "Location.xls"))
		{
			DataTable posTable = excelOp.ExcelReader(locationPath + subPath + filePreName + "Location.xls", "LOCATION" + column_name, "A", "C");
			Transform objTransform = null;
			bool isRight = true;
			BaseCompute baseCal = new BaseCompute();
			for (int i = 0; i < posTable.Rows.Count; i++)
			{
				try
				{
					objTransform = GameObject.Find((string)posTable.Rows[i][0].ToString()).transform;
				}
				catch
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][0].ToString() + "，该物体在当前场景中不存在，请确认！");
					return;
				}
				objTransform.position = baseCal.Vector3Conversion((string)posTable.Rows[i][1].ToString(), ref isRight);
				if (!isRight)
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][1].ToString() + "，位置向量转化错误，请检查！");
					return;
				}
				objTransform.eulerAngles = baseCal.Vector3Conversion((string)posTable.Rows[i][2].ToString(), ref isRight);
				if (!isRight)
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][2].ToString() + "，角度向量转化错误，请检查！");
					return;
				}
			}
			if (File.Exists(locationPath + subPath + filePreName + "Location.xls"))
			{
				DataTable speTable = excelOp.ExcelReader(locationPath + subPath + filePreName + "Location.xls", "SPECIAL" + column_name, "A", "C");
				string speName = "";
				string speType = "";
				string speValue = "";
				for (int i = 0; i < speTable.Rows.Count; i++)
				{
					speName = (string)speTable.Rows[i][0].ToString();
					speType = (string)speTable.Rows[i][1].ToString();
					speValue = (string)speTable.Rows[i][2].ToString();
					SetSpecialObjInfo(speName, speType, speValue, ref isRight);
					if (!isRight)
					{
						UnityEngine.Debug.LogError((string)speTable.Rows[i][0].ToString() + "，该处信息有误，请检查！");
						return;
					}
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogError(locationPath + subPath + filePreName + "Location.xls，该文件不存在，请确认！");
			return;
		}
	}

	//Json数据提取
	private void JsonLocationSet(string nodeName, string column_name)
	{
		nodeName = nodeName.ToUpper();
		//检查是否存在以该数模命名的文件夹
		string locationPath = MotionPara.taskRootPath + MotionPara.taskName + "/";
		string subPath = "";
		if (nodeName.StartsWith("C"))
		{
			subPath = "C/";
		}
		else if (nodeName.StartsWith("Y"))
		{
			subPath = "Y/";
		}
		else if (nodeName.StartsWith("Z"))
		{
			subPath = "Z/";
		}
		else
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		//检查数字是否正确
		int fileNum = 0;
		try
		{
			fileNum = int.Parse(nodeName.Substring(1));
		}
		catch
		{
			UnityEngine.Debug.LogError("输入的节点名称：" + nodeName + "，格式不正确！");
			return;
		}
		string filePreName = nodeName.Substring(0, 1) + fileNum.ToString();
		//Json处理类
		JsonOperator jsonOp = new JsonOperator();
		if (File.Exists(locationPath + subPath + filePreName + "Location.xls"))
		{
			DataTable posTable = jsonOp.JsonReader(locationPath + subPath + filePreName + "Location.json", "LOCATION" + column_name);
			Transform objTransform = null;
			bool isRight = true;
			BaseCompute baseCal = new BaseCompute();
			for (int i = 0; i < posTable.Rows.Count; i++)
			{
				try
				{
					objTransform = GameObject.Find((string)posTable.Rows[i][0].ToString()).transform;
				}
				catch
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][0].ToString() + "，该物体在当前场景中不存在，请确认！");
					return;
				}
				objTransform.position = baseCal.Vector3Conversion((string)posTable.Rows[i][1].ToString(), ref isRight);
				if (!isRight)
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][1].ToString() + "，位置向量转化错误，请检查！");
					return;
				}
				objTransform.eulerAngles = baseCal.Vector3Conversion((string)posTable.Rows[i][2].ToString(), ref isRight);
				if (!isRight)
				{
					UnityEngine.Debug.LogError((string)posTable.Rows[i][2].ToString() + "，角度向量转化错误，请检查！");
					return;
				}
			}
			if (File.Exists(locationPath + subPath + filePreName + "Location.xls"))
			{
				DataTable speTable = jsonOp.JsonReader(locationPath + subPath + filePreName + "Location.json", "SPECIAL" + column_name);
				string speName = "";
				string speType = "";
				string speValue = "";
				for (int i = 0; i < speTable.Rows.Count; i++)
				{
					speName = (string)speTable.Rows[i][0].ToString();
					speType = (string)speTable.Rows[i][1].ToString();
					speValue = (string)speTable.Rows[i][2].ToString();
					SetSpecialObjInfo(speName, speType, speValue, ref isRight);
					if (!isRight)
					{
						UnityEngine.Debug.LogError((string)speTable.Rows[i][0].ToString() + "，该处信息有误，请检查！");
						return;
					}
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogError(locationPath + subPath + filePreName + "Location.xls，该文件不存在，请确认！");
			return;
		}
	}

	//特殊物体属性处理
	private string GetSpecialObjInfo(string spe_name, string spe_type, ref bool is_right)
	{
		string info = "null";
		Transform speTrans = null;
		try
		{
			speTrans = GameObject.Find(spe_name).transform;
		}
		catch
		{
			UnityEngine.Debug.LogError(spe_name + "，该物体在当前场景中不存在，请确认！");
			is_right = false;
			return "null";
		}
		if (spe_type.ToUpper() == "CAMERAMOTION")
		{  //Camera View的大小
			Camera tempCamera;
			try
			{
				tempCamera = speTrans.gameObject.camera;
			}
			catch
			{
				UnityEngine.Debug.LogError(spe_name + "，该物体不是摄像机或者Camera无效，请确认！");
				is_right = false;
				return "null";
			}
			if (tempCamera.orthographic)
			{
				info = tempCamera.orthographicSize.ToString();
			}
			else
			{
				info = tempCamera.fieldOfView.ToString();
			}
		}
		else if (spe_type.ToUpper() == "CAMERAACTIVE")
		{  //Camera当前是否处于激活状态，为了考虑多个Camera情况
			Camera tempCamera;
			try
			{
				tempCamera = speTrans.gameObject.camera;
			}
			catch
			{
				UnityEngine.Debug.LogError(spe_name + "，该物体不是摄像机或者Camera无效，请确认！");
				is_right = false;
				return "null";
			}
			info = tempCamera.enabled.ToString().ToUpper();
		}
		else
		{
			info = "null";
		}
		return info;
	}

	//特殊物体属性处理
	private void SetSpecialObjInfo(string spe_name, string spe_type, string spe_value, ref bool is_right)
	{
		Transform speTrans = null;
		try
		{
			speTrans = GameObject.Find(spe_name).transform;
		}
		catch
		{
			UnityEngine.Debug.LogError(spe_name + "，该物体在当前场景中不存在，请确认！");
			is_right = false;
			return;
		}
		if (spe_type.ToUpper() == "CAMERAMOTION")
		{  //Camera View的大小
			Camera tempCamera;
			try
			{
				tempCamera = speTrans.gameObject.camera;
			}
			catch
			{
				UnityEngine.Debug.LogError(spe_name + "，该物体不是摄像机或者Camera无效，请确认！");
				is_right = false;
				return;
			}
			BaseCompute baseCal = new BaseCompute();
			float sValue = baseCal.FloatConversion(spe_value, ref is_right);
			if (!is_right)
			{
				UnityEngine.Debug.LogError(spe_value + "，该值无效，请检查！");
				return;
			}
			if (tempCamera.orthographic)
			{
				tempCamera.orthographicSize = sValue;
			}
			else
			{
				tempCamera.fieldOfView = sValue;
			}
		}
		else if (spe_type.ToUpper() == "CAMERAACTIVE")
		{  //Camera当前是否处于激活状态，为了考虑多个Camera情况
			Camera tempCamera;
			try
			{
				tempCamera = speTrans.gameObject.camera;
			}
			catch
			{
				UnityEngine.Debug.LogError(spe_name + "，该物体不是摄像机或者Camera无效，请确认！");
				is_right = false;
				return;
			}
			if (spe_value.ToString() == "TRUE")
			{
				if (!tempCamera.enabled)
					tempCamera.enabled = true;
			}
			else
			{
				if (tempCamera.enabled)
					tempCamera.enabled = false;
			}
		}
		else
		{
			//TODO
		}
	}
}
