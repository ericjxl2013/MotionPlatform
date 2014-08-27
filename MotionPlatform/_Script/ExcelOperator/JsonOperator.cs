/// <summary>
/// <Filename>: JsonOperator.cs
/// Author: Wu Hao
/// Created: 2014.08.21
/// Version: 1.0
/// Company: Sunnytech
/// Function: Json数据读写操作类
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System;

public class JsonOperator
{

	private string filepath;

	//获得file_path的Json数据文件中一个表sheet_name
	public DataTable JsonReader(string file_path, string sheet_name){

		string jsonText = @"";
		jsonText += File.ReadAllText(file_path);

		DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonText);
		DataTable jsonData = dataSet.Tables[sheet_name];

		return jsonData;
	}

	//向file_path的Json数据文件中插入一个表sheet_name
	public bool JsonWriter(string file_path, string sheet_name, string[] column_name, string[,] insert_content, bool over_flag){

		if(File.Exists(file_path) && over_flag){
			File.Delete(file_path);
		}

		string jsonText = @"";
		DataSet dataSet;
		if(File.Exists(file_path)){
			jsonText += File.ReadAllText(file_path);

			dataSet = JsonConvert.DeserializeObject<DataSet>(jsonText);
		}
		else{
			dataSet = new DataSet();
		}

		dataSet.Namespace = "NetFrameWork";
		//新建DataTable
		DataTable table = new DataTable(sheet_name);
		//设置DataTable的列名
		for(int i=0; i<column_name.Length; i++){
			table.Columns.Add(column_name[i], typeof(string));
		}
		//将DataTable加入dataSet
		dataSet.Tables.Add(table);
		//向DataTable加入数据
		for(int i=0; i< insert_content.GetLength(1); i++){
			DataRow newRow = table.NewRow();
			for(int j = 0; j < insert_content.GetLength(0); j++){
				newRow[column_name[j]] = insert_content[j,i];
			}
			table.Rows.Add(newRow);
		}
		//刷新数据
		dataSet.AcceptChanges();
		string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);

		string path = file_path;
		if(File.Exists(path)){
			File.Delete(path);
		}
		// Create the file.
		using (FileStream fs = File.Create(path))
		{
			Byte[] info = new UTF8Encoding(true).GetBytes(json);
			// Add some information to the file.
			fs.Write(info, 0, info.Length);
		}

		return true;
	}

	//void Update () {
	//	if(Input.GetKeyDown(KeyCode.W)){
	//		UnityEngine.Debug.Log("W");

	//		string file_path = filepath;
	//		string sheet_name = "testJson1";
	//		string[] column_name = new string[]{"name","id"};
	//		string[,] insert_content = new string[,]{{"nihao", "1"},{"world", "2"}};
	//		JsonWriter(file_path, sheet_name, column_name, insert_content, false);
	//	}
	//	if(Input.GetKeyDown(KeyCode.R)){
	//		UnityEngine.Debug.Log("R");

	//		string file_path = filepath;
	//		string sheet_name = "testJson2";
	//		DataTable posTable = JsonReader(filepath, sheet_name);
	//		for(int i = 0; i < posTable.Rows.Count; i++){
	//			for(int j=0; j< posTable.Columns.Count; j++){
	//				UnityEngine.Debug.Log(i+","+j+"--"+ posTable.Rows[i][j].ToString());
	//			}
	//		}
	//	}
	//}
}
