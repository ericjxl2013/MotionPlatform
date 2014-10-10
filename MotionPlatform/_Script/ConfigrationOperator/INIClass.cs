/// <summary>
/// FileName: INIClass
/// Author: Jiang Xiaolong && http://bitcyj.blog.51cto.com/995375/224140
/// Created Time: 2014.07.07
/// Version: 1.0
/// Company: Sunnytech
/// Function: 读写INI配置文件
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class INIClass
{  
	public string FileName;

	[DllImport("kernel32")]  
	private static extern bool WritePrivateProfileString (
		string section, string key, string val, string filePath);

	[DllImport("kernel32")]  
	private static extern int GetPrivateProfileString(
		string section, string key, string def, byte[] retVal, 
		int size, string filePath); 
	
	/// ﹤summary﹥  
	/// 构造方法，检测是否有文件，没有的话创建一个空文件
	/// ﹤/summary﹥  
	/// ﹤param name="INIPath"﹥文件路径﹤/param﹥  
	public INIClass (string INIPath)
	{  
		FileName = INIPath; 
		if(!ExistINIFile()){
			StreamWriter sw= new StreamWriter(FileName, false, System.Text.Encoding.Default);
			sw.Write("");
			sw.Close();
		}
	}
	
	/// ﹤summary﹥  
	/// 验证文件是否存在  
	/// ﹤/summary﹥  
	/// ﹤returns﹥布尔值﹤/returns﹥  
	public bool ExistINIFile ()
	{  
		return File.Exists(FileName);  
	}
	
	//写INI文件
	public void WriteString(string Section, string Ident, string Value)
	{
		if (!WritePrivateProfileString(Section, Ident, Value, FileName)){
			throw (new ApplicationException("写Ini文件出错"));
		}
	}
	
	//写整数
	public void WriteInteger(string Section, string Ident, int Value)
	{
		WriteString(Section, Ident, Value.ToString());
	}
	
	//写Bool
	public void WriteBool(string Section, string Ident, bool Value)
	{
		WriteString(Section, Ident, Convert.ToString(Value));
	}
	
	//写Float
	public void WriteFloat(string Section, string Ident, float Value)
	{
		WriteString(Section, Ident, Convert.ToString(Value));
	}
	
	//读取INI文件指定
	public string ReadString(string Section, string Ident, string Default)
	{
		Byte[] Buffer = new Byte[65535];
		int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
		//必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
		string s = Encoding.GetEncoding(0).GetString(Buffer);
		s = s.Substring(0, bufLen);
		return s.Trim();
	}
	
	//读整数
	public int ReadInteger(string Section, string Ident, int Default)
	{
		string intStr = ReadString(Section, Ident, Convert.ToString(Default));
		try
		{
			return Convert.ToInt32(intStr);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			return Default;
		}
	}
	
	//读布尔
	public bool ReadBool(string Section, string Ident, bool Default)
	{
		try
		{
			return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			return Default;
		}
	}
	
	//读浮点数
	public float ReadFloat(string Section, string Ident, float Default)
	{
		try
		{
			return (float)Convert.ToDouble(ReadString(Section, Ident, Convert.ToString(Default)));
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			return Default;
		}
	}
	
	//从Ini文件中，将指定的Section名称中的所有Ident添加到列表中
	public void ReadIdents(string Section, List<string> Idents)
	{
		Byte[] Buffer = new Byte[16384];
		Idents.Clear();
		int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
			FileName);
		//对Section进行解析
		GetStringsFromBuffer(Buffer, bufLen, Idents);
	}
	
	private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, List<string> Strings)
	{
		Strings.Clear();
		if (bufLen != 0)
		{
			int start = 0;
			for (int i = 0; i < bufLen; i++)
			{
				if ((Buffer[i] == 0) && ((i - start) > 0))
				{
					String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
					Strings.Add(s);
					start = i + 1;
				}
			}
		}
	}
	
	//从Ini文件中，读取所有的Sections的名称
	public void ReadSections(List<string> SectionList)
	{
		//Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section
		byte[] Buffer = new byte[65535];
		SectionList.Clear();
		int bufLen = 0;
		bufLen = GetPrivateProfileString(null, null, null, Buffer,
		Buffer.GetUpperBound(0), FileName);
		GetStringsFromBuffer(Buffer, bufLen, SectionList);
	}
	
	//读取指定的Section的所有Value到列表中,都为string
	public void ReadSectionValues(string Section, Dictionary<string, string> Values)
	{
		List<string> KeyList = new List<string>();
		ReadIdents(Section, KeyList);
		Values.Clear();
		foreach (string key in KeyList)
		{
			Values.Add(key, ReadString(Section, key, ""));
		}
	}
	
	//清除某个Section
	public void EraseSection(string Section)
	{
		if (!WritePrivateProfileString(Section, null, null, FileName))
		{
			throw (new ApplicationException("无法清除Ini文件中的Section"));
		}
	}
	
	//删除某个Section下的键
	public void DeleteKey(string Section, string Ident)
	{
		WritePrivateProfileString(Section, Ident, null, FileName);
	}
	
	//Note:对于Win9X，来说需要实现UpdateFile方法将缓冲中的数据写入文件
	//在Win NT, 2000和XP上，都是直接写文件，没有缓冲，所以，无须实现UpdateFile
	//执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。
	public void UpdateFile()
	{
		WritePrivateProfileString(null, null, null, FileName);
	}
	
	//检查某个Section下的某个键值是否存在
	public bool ValueExists(string Section, string Ident)
	{
		List<string> Idents = new List<string>();
		ReadIdents(Section, Idents);
		return Idents.IndexOf(Ident) > -1;
	}
	
	//写Section注释行
	public void WriteNotes(string NotesString)
	{
		StreamWriter sw= new StreamWriter(FileName, true, System.Text.Encoding.Default);
		sw.WriteLine("#" + NotesString);
		sw.Close();
	}
	
	//写Ident注释行
	public void WriteIdentNotes(string NotesString)
	{
		StreamWriter sw= new StreamWriter(FileName, true, System.Text.Encoding.Default);
		sw.WriteLine("---" + NotesString + "---");
		sw.Close();
	}
	
	//写空行
	public void WriteNullLine()
	{
		StreamWriter sw= new StreamWriter(FileName, true, System.Text.Encoding.Default);
		sw.WriteLine("");
		sw.Close();
	}
	
	//确保资源的释放
	~INIClass()
	{
		UpdateFile();
	}
} 
