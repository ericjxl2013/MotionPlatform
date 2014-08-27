using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class doVoiceExe : MonoBehaviour {
	
	Process myProcess;
	string my_str_path;
	
// Use this for initialization
	void Start () {
		my_str_path = "D:\\Debug\\T7_Console.exe";
		// my_str_path = Application.dataPath + "/Resources/ReleaseFile/Debug/T7_Console.exe";
		myProcess = new Process();	
		ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(my_str_path, "");
		myProcessStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;	
		myProcess.StartInfo = myProcessStartInfo;
		myProcess.Start();
//		FuncPara.voiceTimer = true;
		
//		doVoice("二十个字吗二十个字吗二十个字吗二十个字吗", "10");
	}
	
	/// <summary>
	/// Dos the voice.
	/// </summary>
	/// <param name='str'>
	/// 需要读的内容
	/// </param>
	public void doVoice(string str_para)
	{
		try{
			myProcess.Kill();
		}catch{
			//UnityEngine.Debug.Log("Killing process failed!");
		}	
		// Stopwatch sw = new Stopwatch();
//		if(FuncPara.voiceTimer){  //计算语音进程精确时间
			// sw.Start();
//		}
		ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(my_str_path, str_para);
		myProcessStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		myProcess.StartInfo = myProcessStartInfo;
		myProcess.Start();
//		if(FuncPara.voiceTimer){  //计算语音进程精确时间
			// myProcess.WaitForExit();
			// sw.Stop();
			// UnityEngine.Debug.LogWarning(sw.ElapsedTicks/(decimal)Stopwatch.Frequency);
			
//		}
	}
	
	/// <summary>
	/// 带调节速度参数的语音接口
	/// </summary>
	/// <param name='str'>
	/// 要调整的速度：1为正常速度，0为慢速(在本程序中没有使用)，2为1.1倍速度，3为1.2倍速度，依次内推。
	/// </param>
	public void doVoice(string str_para, string str_speed)
	{
		try{
			myProcess.Kill();
		}catch{
			//UnityEngine.Debug.Log("Killing process failed!");
		}	
		// Stopwatch sw = new Stopwatch();
//		if(FuncPara.voiceTimer){  //计算语音进程精确时间
			// sw.Start();
//		}
		string strarr = str_para+ "|" + str_speed;
		ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(my_str_path, strarr);
		myProcessStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		myProcess.StartInfo = myProcessStartInfo;
		myProcess.Start();
//		if(FuncPara.voiceTimer){  //计算语音进程精确时间
			// myProcess.WaitForExit();
			// sw.Stop();
			// UnityEngine.Debug.LogWarning(sw.ElapsedTicks/(decimal)Stopwatch.Frequency);
//		}
	}
	
	/// <summary>
	///	关掉相应的exe程序，比如下一次要进行语音时或者要退出某个语言播放是，关掉上一个语音
	/// </summary>
	/// 要关掉的exe程序名字（本语音参数统一为："T7_Console"）;
	/// Name.
	/// </param>
	public void CloseSoundApp(string name)
    {
        Process[] pProcess;
        pProcess = Process.GetProcesses();
        for (int i = 1; i <= pProcess.Length - 1; i++)
        {
			try{
            	if (pProcess[i].ProcessName == name)
           	 	{
                	pProcess[i].Kill();
                	break;
            	}
			}catch{
				
			}
        }
    }
}




