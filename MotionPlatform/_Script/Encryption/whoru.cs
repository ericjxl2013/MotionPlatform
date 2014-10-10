using UnityEngine;
using System.Collections;

public class whoru : MonoBehaviour {
	public int ID;
	GameObject en;
	int dog;
	int file;
	public bool log,fail,in_;
	string name1,password;
	string url;
	Rect log_rect = new Rect(200, 200, 320, 230);
	Rect fail_rect = new Rect(200, 200, 320, 230);
	
	Rect dia_rect = new Rect(Screen.width / 2 - 90f, Screen.height / 2 - 30f, 220f, 60f);
	public GUIStyle guist = new GUIStyle();
	
	bool conn,local_fail;
	string conn_str,ip;
	int port;
	
	bool startChecking = false;  //启动时检测有没有加密狗或者文本解密
	
	// Use this for initialization
	void Start () 
	{
		ID = 0;  //TODO:可以设置为1，测试的时候，即把加密取消了，发布前请改回0
		en=this.gameObject;
		password="";
		name1="";
		fail=false;
		conn_str="";
		ip=PlayerPrefs.GetString("IP","127.0.0.1");
		port=25000;
		check();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI()
	{
		GUI.skin.window = FuncPara.skin_hiMenu.window;
		//网上注册认证窗口
		if(log)
		{
			log_rect = GUI.Window(30, log_rect, DoMyWindow30, "");
			GUI.BringWindowToFront(30);
		}
		//解密结果窗口
		if(fail)
		{
			fail_rect = GUI.Window(31, fail_rect, DoMyWindow31, "");
		}
		//提示正在解密中的窗口
//		if(in_)
//		{
//			dia_rect=GUI.Window(32, dia_rect, DoMyWindow32, "");
//		}
		//局域网认证
//		if(local_log)
//		{
//			log_rect=	GUI.Window(41, log_rect, DoMyWindow41, "");
//		}
		
		GUI.skin.window = null;
	}
		
	//认证身份
	public void check()
	{
		in_=true;
		StartCoroutine(idetify());
	}
	
	IEnumerator  idetify()
	{
		//加密狗认证
		dog = en.GetComponent<dog2>().dog ();
		//加密狗认证失败
		if(dog == 0)
		{
			file=en.GetComponent<file>().file_check();
			//file==1 C盘有相关文本认证程序 进一步认证身份
			if(file==1)
			{	
				yield return StartCoroutine(TakeABreak1());	
			}
			//文本认证失败
			else
			{	
				//网上注册认证
				if(startChecking)
					log=true;
			}
		}
		else
		{
			ID=dog;
			if(startChecking)
				fail=true;
			in_=false;
		}
		startChecking = true;
	}

	
	IEnumerator TakeABreak1()
	{
		int i = 0;
		//mark==1表明还在认证中 i<500则是没超过5秒 继续等待认证结果
		while(en.GetComponent<file>().mark == 1 && i<500)
		{ 
			yield return new WaitForSeconds (0.01F);
			i+=1;
		}
		//认证完成 或时间超过 判断 en.GetComponent<file>().h的值 true则成功
		if(en.GetComponent<file>().h==true)
		{
			ID=1;
			in_=false;
			if(startChecking)
				fail=true;
		}
		else
		{
			//认证失败 开始网上注册认证
			if(startChecking)
				log=true;
		}
	}
	
		
	void DoMyWindow30(int windowID) 
	{
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 19;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 5, 200, 30), "软件解密窗口");
		
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect(30, 65, 100, 30), "用户名：");
		GUI.Label(new Rect(30, 120, 100, 30), "密   码：");
		
		GUI.skin.settings.cursorColor = Color.black;
		GUI.SetNextControlName("UserName");
		name1 = GUI.TextField(new Rect(100, 65, 180, 32), name1, 30, FuncPara.sty_InputBar);
		name1 = name1.Replace("\n", "");
		GUI.SetNextControlName("Password");
		password = GUI.PasswordField(new Rect(100, 120, 180, 32), password, "*"[0], 30, FuncPara.sty_InputBar);
		password = password.Replace("\n", "");
		
		if(GUI.Button(new Rect(35, 177, 100, 35), "确定", FuncPara.sty_SquareBtn)){
			log=false;
			url="http://www.51cax.com/vt/authen.jsp?username="+name1;
			url+="&password="+password+"&id=mold";
			StartCoroutine(ll());
		}
		if(GUI.Button(new Rect(185, 177, 100, 35), "取消", FuncPara.sty_SquareBtn)){
			log=false;	
			in_=false;
		}
		if(log_rect.x<0)
		{log_rect.x=0;}
		if(log_rect.y<0)
		{log_rect.y=0;}
		if(log_rect.x>Screen.width-log_rect.width)
		{log_rect.x=Screen.width-log_rect.width;}
		if(log_rect.y>Screen.height-log_rect.height)
		{log_rect.y=Screen.height-log_rect.height;}
		
		
		GUI.DragWindow();
		GUI.skin.label = null;
	}
	
		
	void DoMyWindow31(int windowID) 
	{
		GUI.skin.label = FuncPara.defaultSkin.label;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 19;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 5, 200, 30), "软件解密状态");
		
//		GUI.skin.button = FuncPara.skin_hiMenu.customStyles[6];
//		GUI.skin.button.font = FuncPara.defaultFont;
//		GUI.skin.button.fontSize = 17;
		
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = Color.black;

		if(ID==0)
			GUI.Label(new Rect(35, 60, 240, 200), "权限认证失败！\n您可以使用加密狗、认证证书或网上注册获得权限。");
		else
		{
			GUI.Label(new Rect(35, 60, 260, 120), "权限认证成功！\n您现在可以使用加密模块。");
			log=false;
			conn=false;
		}
		
		
		if(GUI.Button(new Rect(35, 177, 100, 35), "确定", FuncPara.sty_SquareBtn)){
			fail=false;
		}
		if(GUI.Button(new Rect(185, 177, 100, 35), "取消", FuncPara.sty_SquareBtn)){
			
			fail=false;	
		}
			
		if(fail_rect.x<0)
		{fail_rect.x=0;}
		if(fail_rect.y<0)
		{fail_rect.y=0;}
		if(fail_rect.x>Screen.width-fail_rect.width)
		{fail_rect.x=Screen.width-fail_rect.width;}
		if(fail_rect.y>Screen.height-fail_rect.height)
		{fail_rect.y=Screen.height-fail_rect.height;}
		
		
		GUI.DragWindow();
		GUI.skin.label = null;
	}
	
	
	void DoMyWindow32(int windowID) 
	{
		GUI.Label(new Rect(30, 20, 220,34), "加密模块，正在身份认证。。。", guist);

		if(dia_rect.x<0)
		{dia_rect.x=0;}
		if(dia_rect.y<0)
		{dia_rect.y=0;}
		if(dia_rect.x>Screen.width-dia_rect.width)
		{dia_rect.x=Screen.width-dia_rect.width;}
		if(dia_rect.y>Screen.height-dia_rect.height)
		{dia_rect.y=Screen.height-dia_rect.height;}
		
		
		GUI.DragWindow();
	}
	
	
	void DoMyWindow41(int windowID) 
	{

		if(conn==true)
		{
			GUI.Label(new Rect(25, 35, 260,60),conn_str);
		}
			
			
		GUI.Label(new Rect(25, 75, 260,24), "服务器IP地址");
	
		ip=GUI.TextField(new Rect(125, 75, 100,24),ip);
	
		GUI.Label(new Rect(25, 105, 260,24), "端口");
	
		port =int.Parse( GUI.TextField(new Rect(125,105,100,24),port.ToString()));
		
		if(	GUI.Button(new Rect(25, 155, 80,32), "确定"))
		{
			local_fail=false;
			Network.Connect(ip, port);
			StopCoroutine("TakeABreak2");
			StartCoroutine("TakeABreak2");
		}
		if(	GUI.Button(new Rect(215, 155, 80,32), "取消"))
		{
			log=true;
			conn=false;
		}
		if(log_rect.x<0)
		{log_rect.x=0;}
		if(log_rect.y<0)
		{log_rect.y=0;}
		if(log_rect.x>Screen.width-log_rect.width)
		{log_rect.x=Screen.width-log_rect.width;}
		if(log_rect.y>Screen.height-log_rect.height)
		{log_rect.y=Screen.height-log_rect.height;}
		
		
		GUI.DragWindow();
		
	}
		 
	IEnumerator TakeABreak2()
	{
		conn=true;
		conn_str= "连接服务器中。。。";
		Debug.Log ("11");
		while(Network.peerType == NetworkPeerType.Disconnected&&local_fail==false)
		{
		   yield return new WaitForSeconds (0.01F);
		}

		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			conn_str= "连接失败，请确认IP与端口信息正确";
		}
		else
		{
			PlayerPrefs.SetString("IP",ip);
			conn=false;
			ID=3;
			in_=false;
		}
	}
	  
	void OnFailedToConnect(NetworkConnectionError error) 
	{
        Debug.Log("Could not connect to server!!: " + error);
		local_fail=true;
    }
	
	
	
	 IEnumerator ll() 
	{

		WWW www = new WWW(url);
		yield return www;

			
		if(www.error!=null)
		{
			Debug.Log (www.error);
			if(ID==0)	
			{
				fail=true;
				in_=false;
			}
		}
		else
		{
			if(www.text.Length<16)
			{
				string x=www.text;
				int xx=int.Parse(x);
				Debug.Log ("xx"+xx.ToString());
		
				if(xx==1)
				{
					ID=1;
					in_=false;
					fail=true;
				}
		
				if(ID==0)
				{
					fail=true;
					in_=false;
				}
			}
			else
			{
				if(ID==0)
				{
					fail=true;
					in_=false;
				}
			}
		}
    
    }
	
	
	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		if(ID==3)
		{
			ID=0;
		}
		else{
			
		}
	}
}
