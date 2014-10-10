using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Linq;



public class file : MonoBehaviour 
{
 	String series_path = "c:\\series_"; 
	String code_path =  "c:\\ecode.exe";
	public bool h;
	string a;
	public int mark;
	
	void Start () 
	{
		String strModuleName = "mold";
		series_path = series_path + strModuleName + ".exe";
	}
			
	IEnumerator TakeABreak() 
	{
		while((File.Exists("c:\\series_mold.txt") == false))
		{
			  yield return new WaitForSeconds (0.1F);
		}
	}
	
				 
	IEnumerator TakeABreak1() 
	{
		while( (File.Exists("c:\\flag.txt") == false))
		{
			yield return new WaitForSeconds (0.1F);
		}
	}
	
	IEnumerator hhh()
	{
		File.Delete("c:\\series_mold.txt");
		
 		System.Diagnostics.Process pp =new System.Diagnostics.Process(); 
		pp.StartInfo.FileName = series_path; 
		pp.StartInfo.WorkingDirectory = Application.dataPath; 
		               
		//注意下面两行! 
		pp.StartInfo.CreateNoWindow= true; 
		pp.StartInfo.UseShellExecute = false; //不显示Dos窗口
		pp.Start();

		yield return StartCoroutine	(TakeABreak());
		File.Delete("c:\\series.txt");
		if(File.Exists("c:\\series_mold.txt"))
		{	
			string readline;
			yield return new WaitForSeconds (0.1F);
			readline=File.ReadAllText("c:\\series_mold.txt");
			char [] all=readline.ToCharArray();
			readline="";
			for(int i=0;i<all.Length;i++)
			{
				readline+=all[i];
			}
			File.WriteAllText("c:\\series.txt",readline);

			yield return new WaitForSeconds (0.1F);
			File.Delete("c:\\series_mold.txt");
			yield return new WaitForSeconds (0.1F);
			
			File.Delete("c:\\flag.txt");
			File.Delete("C:\\productseries.txt");
			if(File.Exists("C:\\productseries_mold.txt"))
				
			{
				
				File.Copy("C:\\productseries_mold.txt","C:\\productseries.txt");
				
				System.Diagnostics.Process p =new System.Diagnostics.Process(); 
   
				p.StartInfo.FileName = code_path; 
   				p.StartInfo.WorkingDirectory = Application.dataPath; 
               //注意下面两行! 
				p.StartInfo.CreateNoWindow= true; 
				p.StartInfo.UseShellExecute = false; //不显示Dos窗口
				p.Start();

				yield return StartCoroutine	(TakeABreak1());
		
				a=File.ReadAllText("c:\\flag.txt");
				if(a=="1")
				{
					h=true;
				}
					
				mark=0;
				
			}
			else 
				mark=0;
		}
		
		else 
			mark=0;

		File.Delete("c:\\flag.txt");
	}

	public int file_check()
	{
		File.Delete("c:\\series_mold.txt");
		File.Delete("c:\\flag.txt");
		if( (File.Exists(series_path)==false) || (File.Exists(code_path)==false) ) 	   
		{
			return(0);		
		}
		else
		{
			mark=1;
			StartCoroutine(hhh ());
			return(1);
		}
	}
	
	
	
	
	
}
