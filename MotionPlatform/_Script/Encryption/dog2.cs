using UnityEngine;
using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Linq;

public class dog2 : MonoBehaviour
{
	
//	static SetDllpath setdllpath = new SetDllpath("~/Dll");
	
	[DllImport("Syunew3D",CallingConvention = CallingConvention.StdCall)]
	public static  extern int FindPort_2 (int start, int in_data, int verf_data, StringBuilder  OutPath);
	
	[DllImport("Syunew3D",CharSet = CharSet.Ansi,CallingConvention = CallingConvention.StdCall)]

	public static  extern int  FindPort (int start, StringBuilder outpath);

	[DllImport("Syunew3D",CharSet = CharSet.Ansi,CallingConvention = CallingConvention.StdCall)]
	public static  extern int  YReadString (StringBuilder string1, short Address, int len, StringBuilder HKey, StringBuilder LKey, StringBuilder Path1);
	
	int doginfo;
	public  StringBuilder DevicePath;
	
	void Start ()
	{
		
	}

	public int dog ()
	{
		doginfo = 0;
		DevicePath = new StringBuilder (260);

		if (!(FindPort (0, DevicePath) != 0)) {
			if (FindPort_2 (0, 1, 74532473, DevicePath) != 0) {
				doginfo = 2;
			} else if (FindPort_2 (0, 1, -379880570, DevicePath) != 0)
				doginfo = 2;
		}
		
		/*	
	//用于获取节点数 用于局域网加密的	
	if(YReadString(cNumber,0,3,x1,x2,DevicePath)==0)
	{
//		Debug.Log(cNumber+"@@");
//		net.GetComponent<ConnectGui1>().player_num=int.Parse(cNumber.ToString());
	}
	else
	{
//		net.GetComponent<ConnectGui1>().player_num=0;	
	}
	*/
		return(doginfo);

	}

}