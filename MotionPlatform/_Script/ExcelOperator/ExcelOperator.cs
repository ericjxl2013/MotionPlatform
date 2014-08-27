/// <summary>
/// <Filename>: ExcelOperator
/// Author: Jiang Xiaolong
/// Created: 2014.02.18
/// Version: 1.0
/// Company: Sunnytech
/// Function: 操作Excel的类
///
/// Changed By: Wuhao
/// Modification Time: 8.22.2014
/// Discription: 增加插入,修改功能
/// <summary>

using UnityEngine;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;

public class ExcelOperator {

	
	public ExcelOperator()
	{
		
	}
	
	/// <summary>
	/// 读取一个Excel工作簿中完整的一张或几张sheet.
	/// </summary>
	/// <returns>
	/// DataSet.
	/// </returns>
	/// <param name='file_path'>
	/// Excel文件完整路径.
	/// </param>
	/// <param name='sheet_name'>
	/// sheet名字字符串数组.
	/// </param>
	public DataSet ExcelReader(string file_path, string[] sheet_name)
	{
		DataSet excelData = new DataSet();
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + file_path + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		for(int i = 0; i < sheet_name.Length; i++){
			try{
				odbcCon.Open();
				string selectStr = "Select * FROM [" + sheet_name[i] +"$]";
				OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(selectStr, connectionStr);
				odbcAdapter.Fill(excelData, "[" + sheet_name[i] + "$]");
				odbcCon.Close();
			}catch{
				Debug.LogError("Excel读取错误，请检查相关信息---" + "路径: " + file_path + "; Sheet名: " + sheet_name[i]);
			}
		}
		return excelData;
	}
	
	/// <summary>
	/// 读取一个Excel工作簿中一张或几张sheet的指定几列，如：A:J.
	/// </summary>
	/// <returns>
	/// DataSet.
	/// </returns>
	/// <param name='file_path'>
	/// Excel文件完整路径.
	/// </param>
	/// <param name='sheet_name'>
	/// sheet名字字符串数组.
	/// </param>
	/// <param name='start_position'>
	/// 起始行位置，如A.
	/// </param>
	/// <param name='end_position'>
	/// 终止列位置，如J.
	/// </param>
	public DataSet ExcelReader(string file_path, string[] sheet_name, string start_position, string end_position)
	{
		DataSet excelData = new DataSet();
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + file_path + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		for(int i = 0; i < sheet_name.Length; i++){
			try{
				odbcCon.Open();
				string selectStr = "Select * FROM [" + sheet_name[i] +"$" + start_position + ":" + end_position +"]";
				OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(selectStr, connectionStr);
				odbcAdapter.Fill(excelData, "[" + sheet_name[i] + "$]");
				odbcCon.Close();
			}catch{
				Debug.LogError("Excel读取错误，请检查相关信息---" + "路径: " + file_path + "; Sheet名: " + sheet_name[i] + 
					"; Start: " + start_position + "; End: " + end_position);
			}
		}
		return excelData;
	}
	
	/// <summary>
	/// 读取一个Excel工作簿中完整的一张sheet.
	/// </summary>
	/// <returns>
	/// DataTable.
	/// </returns>
	/// <param name='file_path'>
	/// Excel文件完整路径.
	/// </param>
	/// <param name='sheet_name'>
	/// Sheet名.
	/// </param>
	public DataTable ExcelReader(string file_path, string sheet_name)
	{
		DataTable excelData = new DataTable();
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + file_path + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		try{
			odbcCon.Open();
			string selectStr = "Select * FROM [" + sheet_name +"$]";
			OdbcCommand oCmd = new OdbcCommand(selectStr, odbcCon);
			OdbcDataReader odbcReader = oCmd.ExecuteReader();
			excelData.Load(odbcReader);
			odbcReader.Close();
			odbcCon.Close();
		}catch{
			Debug.LogError("Excel读取错误，请检查相关信息---" + "路径: " + file_path + "; Sheet名: " + sheet_name);
		}
		return excelData;
	}
	
	/// <summary>
	/// 读取一个Excel工作簿中一张的指定几列，如：A:J.
	/// </summary>
	/// <returns>
	/// DataTable.
	/// </returns>
	/// <param name='file_path'>
	/// Excel文件完整路径.
	/// </param>
	/// <param name='sheet_name'>
	/// sheet名.
	/// </param>
	/// <param name='start_position'>
	/// 起始行位置，如A.
	/// </param>
	/// <param name='end_position'>
	/// 终止列位置，如J.
	/// </param>
	public DataTable ExcelReader(string file_path, string sheet_name, string start_position, string end_position)
	{
		DataTable excelData = new DataTable();
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + file_path + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		try{
			odbcCon.Open();
			string selectStr = "Select * FROM [" + sheet_name +"$" + start_position + ":" + end_position +"]";
			OdbcCommand oCmd = new OdbcCommand(selectStr, odbcCon);
			OdbcDataReader odbcReader = oCmd.ExecuteReader();
			excelData.Load(odbcReader);
			odbcReader.Close();
			odbcCon.Close();
		}catch{
			Debug.LogError("Excel读取错误，请检查相关信息---" + "路径: " + file_path + "; Sheet名: " + sheet_name + 
				"; Start: " + start_position + "; End: " + end_position);
		}
		return excelData;
	}
	
	/// <summary>
	/// 创建Excel工作簿或者在已有工作簿中插入Sheet, 目前只能以字符串的形式写入.
	/// </summary>
	/// <returns>
	/// 写入是否成功.
	/// </returns>
	/// <param name='file_path'>
	/// 完整Excel文件路径.
	/// </param>
	/// <param name='sheet_name'>
	/// 创建的Sheet名字.
	/// </param>
	/// <param name='column_name'>
	/// 创建的Column的Name.
	/// </param>
	/// <param name='insert_content'>
	/// 要插入的内容列表, 内部的List所包含的信息数量应与column_name的个数一样，可为空.
	/// </param>
	/// <param name='cover_flag'>
	/// 如果该路径下已有Excel表格存在，是否要覆盖或者在其中创建新的Sheet.
	/// </param>
	public bool ExcelWriter(string file_path, string sheet_name, List<string> column_name, 
		List<List<string>> insert_content, bool cover_flag)
	{
		if(File.Exists(file_path) && cover_flag){
			File.Delete(file_path);
		}
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=False; DriverID=790; Dbq=" + file_path + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);  
		string excelCreate = "CREATE TABLE " + sheet_name + " (";
		for(int i = 0; i < column_name.Count; i++){
			if(i != column_name.Count - 1)
				excelCreate += "[" + column_name[i] + "] VarChar,";
			else
				excelCreate += "[" + column_name[i] + "] VarChar)";
		}
		OdbcCommand oCmd = new OdbcCommand(excelCreate, odbcCon);
		odbcCon.Open();
		try{
			oCmd.ExecuteNonQuery();
		}catch{
			Debug.LogError("Creating Excel failed! Please check the insert infomation. 可能当前目录下已存在Excel表格及相应Sheet，" +
				"且未选择覆盖模式.");
			return false;
		}
		string insertStr = "";
		for(int i = 0; i < insert_content.Count; i++){
			try{
				insertStr = "INSERT INTO " + sheet_name + " VALUES('";
				for(int j = 0; j < insert_content[i].Count; j++){
					if(j != insert_content[i].Count - 1)
						insertStr += insert_content[i][j] + "','";
					else
						insertStr += insert_content[i][j] + "')";
				}
				oCmd.CommandText = insertStr;
				oCmd.ExecuteNonQuery();
			}catch{
				Debug.LogError("Inserting message failed! Insert string: " + insertStr);
				return false;
			}
		}
		odbcCon.Close();
		return true;
	}

	//更新当前sheet中数据
	//contents--更新后的表的全部记录
	//key--表的主键的列
	public void UpdateData(string filepath, string sheetName, string[,] contents, int key){	
		ExcelOperator excelReader = new ExcelOperator();
		DataTable sheetTable = excelReader.ExcelReader(filepath + ".xls", sheetName);
		
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + filepath + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		for(int i = 0; i < sheetTable.Rows.Count; i++){
			try{
				OdbcCommand oCmd = new OdbcCommand();
				oCmd.Connection = odbcCon;
				odbcCon.Open();
				string updateStr = "update ["+sheetName+"$] set ";
				for(int j = 0; j < sheetTable.Columns.Count; j++){
					if(j != key){
						if(j == sheetTable.Columns.Count-1){
							updateStr += sheetTable.Columns[j].ColumnName+"='"+contents[i,j]+"'";
						}else{
							updateStr += sheetTable.Columns[j].ColumnName+"='"+contents[i,j] +"', ";
						}
					}
				}
				updateStr += " where "+sheetTable.Columns[key].ColumnName+" ='"+contents[i,key]+"'";
				
				oCmd.CommandText = updateStr;
				oCmd.ExecuteNonQuery();
				odbcCon.Close();
				
				
			}catch{
				UnityEngine.Debug.LogError("Excel更新错误，请检查相关信息---" );
				odbcCon.Close();
			}
		}
	}
	
	//更新当前sheet中数据
	//contents--更新表的单条记录
	//key--表的主键的列
	public void UpdateData(string filepath, string sheetName, string[] contents, int key){
		ExcelOperator excelReader = new ExcelOperator();
		DataTable sheetTable = excelReader.ExcelReader(filepath + ".xls", sheetName);
		
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + filepath + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		
		try{
			OdbcCommand oCmd = new OdbcCommand();
			oCmd.Connection = odbcCon;
			odbcCon.Open();
			string updateStr = "update ["+sheetName+"$] set ";
			for(int j = 0; j < sheetTable.Columns.Count; j++){
				if(j != key){
					if(j == sheetTable.Columns.Count-1){
						updateStr += sheetTable.Columns[j].ColumnName+"='"+contents[j]+"'";
					}else{
						updateStr += sheetTable.Columns[j].ColumnName+"='"+contents[j] +"', ";
					}
				}
			}
			updateStr += " where "+sheetTable.Columns[key].ColumnName+" ='"+contents[key]+"'";
			oCmd.CommandText = updateStr;
			oCmd.ExecuteNonQuery();
			odbcCon.Close();
			
			
		}
		//catch{
		//    UnityEngine.Debug.LogError("Excel更新错误，请检查相关信息---" );
		//    odbcCon.Close();
		//}
		catch(System.Exception e){
			UnityEngine.Debug.LogError(e.Message);
			UnityEngine.Debug.LogError("Excel更新错误，请检查相关信息---" );
			odbcCon.Close();
		}
	}
	
	//向当前sheet中添加数据
	//addContent--添加到表的新的一条记录
	public void AddData(string filepath, string sheetName, string[] addContent){
		ExcelOperator excelReader = new ExcelOperator();
		DataTable sheetTable = excelReader.ExcelReader(filepath + ".xls", sheetName);
		
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + filepath + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		
		try{
			OdbcCommand oCmd = new OdbcCommand();
			oCmd.Connection = odbcCon;
			odbcCon.Open();
			string updateStr="";
			
			updateStr = "insert into ["+sheetName+"$] values(";
			for(int j = 0; j < sheetTable.Columns.Count; j++){
				
				if(j == sheetTable.Columns.Count-1){
					updateStr += "'"+addContent[j]+"')";
				}else{
					updateStr += "'"+addContent[j]+"',";
				}
			}
			
			oCmd.CommandText = updateStr;
			oCmd.ExecuteNonQuery();
			odbcCon.Close();
			
		}catch{
			UnityEngine.Debug.LogError("Excel更新错误，请检查相关信息---" );
			odbcCon.Close();
		}
		
		
	}
	
	//删除当前sheet中数据
	//keyValue--要删除的一条记录的主键值
	//key--表的主键的列
	public void DeleteData(string filepath, string sheetName, string keyValue, int rowIndex, int key){
		ExcelOperator excelReader = new ExcelOperator();
		DataTable sheetTable = excelReader.ExcelReader(filepath + ".xls", sheetName);
		
		string connectionStr = "Driver={Microsoft Excel Driver (*.xls)}; ReadOnly=True; DriverID=790; Dbq=" + filepath + ";";
		OdbcConnection odbcCon = new OdbcConnection(connectionStr);
		
		//删除为空行
		try{
			OdbcCommand oCmd = new OdbcCommand();
			oCmd.Connection = odbcCon;
			odbcCon.Open();
			string updateStr = "update ["+sheetName+"$] set ";
			for(int j = 0; j < sheetTable.Columns.Count; j++){
				
				if(j == sheetTable.Columns.Count-1){
					updateStr += sheetTable.Columns[j].ColumnName+"=null";
				}else{
					updateStr += sheetTable.Columns[j].ColumnName+"=null, ";
				}
			}
			updateStr += " where "+sheetTable.Columns[key].ColumnName+" ='"+keyValue+"'";
			
			oCmd.CommandText = updateStr;
			oCmd.ExecuteNonQuery();
			odbcCon.Close();
		}catch{
			UnityEngine.Debug.LogError("Excel更新错误，请检查相关信息---" );
			odbcCon.Close();
		}
		
	}
}
