using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warnning : MonoBehaviour {
	
	public bool MousePosflag = false;  //用于判断鼠标是否在提示框上，在为true，此时不隐藏界面;
	
	Rect out_rect = new Rect(0,Screen.height-30,5,30);
	bool out_flag = true;
	private float left = -500f; 
	public bool come_forth = false;
	public bool motion_start = false;
	private float time_value = 0;
	
	Rect informationRect = new Rect(-500f, Screen.height - 245f, 324f, 245f);
	public GUIText warnText;
	public Vector2 my_scrollPosition = Vector2.zero;
	public List<string> display_info_list = new List<string>(); //每一个Label里面显示的内容
	public List<float> info_string_height = new List<float>();  //每一个Label的height值
	float height_distance = 17f;  //每一行字符所占的高度
	float window_height = 160f;  //Scroll View区域的高度
	int row_count = 0;  
	float current_height = 0;
	int page_count = 0;
	int page_separate = 200;  //多少行分一个Label
	public string object_description = ""; //提示字符串
	
	float time_recorder = 0;
	bool time_switch = false;
	
	bool warnningWindow = false;
	
	// Use this for initialization
	void Start () {
		warnText = GameObject.Find("WarnningText").guiText;
		warnText.font = FuncPara.font_STZHONGS;
		warnText.fontSize = 13;

	}
	
	
	void OnGUI()
	{
		informationRect.x = left;
		
		//用于显示提示窗的方式。1、快捷键T；  2、鼠标移至左下角
//		if(Input.GetKeyDown (KeyCode.W))
//			motion_start = true;
		if(out_rect.Contains (Event.current.mousePosition) && out_flag)
		{
			warnningWindow = true;
			motion_start = true;
			come_forth = false;
		}
		
		if(informationRect.Contains (Event.current.mousePosition))
		{
			MousePosflag = true;
			time_switch = false;
		}
		else 
		{
			MousePosflag = false;
			time_switch = true;
		}
		
		if(warnningWindow)
			GUI.Window(5, informationRect, InformationWindow, "", FuncPara.sty_WarnningWindow);
	}
	
	void InformationWindow(int WindowID)
	{
		GUI.skin = FuncPara.skin_Scroll;
		my_scrollPosition = GUI.BeginScrollView(new Rect(10, 37, 300, 160), my_scrollPosition, new Rect(0, 0, 284, window_height), false, true);
		for(int i = 0; i < display_info_list.Count; i++)
		{
			GUI.skin.label.font = FuncPara.font_STZHONGS;
			GUI.skin.label.fontSize = 13;
			GUI.skin.label.normal.textColor = Color.black;
			if(i == display_info_list.Count - 1)
			{
				GUI.Label(new Rect(3, info_string_height[i], 270, window_height - info_string_height[i]), display_info_list[i]);
			}
			else
			{
				GUI.Label(new Rect(3, info_string_height[i], 270, info_string_height[i + 1] - info_string_height[i]), display_info_list[i]);
			}
			GUI.skin.label = null;
		}
		GUI.EndScrollView();
		
		if(GUI.Button(new Rect(48, 203, 80, 30), "清除", FuncPara.sty_SquareBtn))
		{
			object_description = "";
			display_info_list.Clear();
			info_string_height.Clear();
			row_count = 0;
			page_count = 0;
			current_height = 0;
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.top = 6;
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.bottom = 10;
			window_height = 160f;
		}
		
		if(GUI.Button(new Rect(170, 203, 80, 30), "关闭", FuncPara.sty_SquareBtn))
		{
			motion_start = true;
		}
	}
	
	/// <summary>
	/// 增加提示信息
	/// </summary>
	/// <param name='info_string'>
	/// 要增加的字符串
	/// </param>
	public void AddInfo(string info_string)
	{
		object_description += info_string;
		StringFormat();
		//控制VerticalScrollbarThumb的border参数
		if(row_count > 180)
		{
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.top = 0;
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.bottom = 0;
		}
		else
		{
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.top = 6;
			FuncPara.skin_Scroll.verticalScrollbarThumb.border.bottom = 10;
		}
		time_recorder = 0;
		my_scrollPosition.y = window_height;
		warnningWindow = true;
		if(motion_start)
		{
			come_forth = false;
		}
		if(!come_forth)
			motion_start = true;
	}
	
	/// <summary>
	/// 格式化提示信息字符串
	/// </summary>
	void StringFormat()
	{
		char[] infoChar = object_description.ToCharArray();
		string temp_string = "";
		string add_string = "";
		display_info_list.Clear();
		info_string_height.Clear();
		row_count = 0;
		page_count = 0;
		current_height = 0;
		for(int i = 0; i < object_description.Length; i++)
		{
			temp_string += infoChar[i];
			warnText.text = temp_string;
			if(warnText.GetScreenRect().width > 270f)  //字符串超过了一行
			{
				temp_string = "";
				row_count++;
				add_string += "\n"+infoChar[i];
				current_height += height_distance;
				if(row_count == page_separate*(page_count+1))
				{
					display_info_list.Add(add_string);
					info_string_height.Add(page_separate*page_count*height_distance);
					page_count++;
					add_string = "";
				}
			}else if(infoChar[i] == '\n'){
				temp_string = "";
				row_count++;
				add_string += infoChar[i];
				current_height += height_distance;
				if(row_count == page_separate*(page_count+1))
				{
					display_info_list.Add(add_string);
					info_string_height.Add(page_separate*page_count*height_distance);
					page_count++;
					add_string = "";
				}
			}
			else
			{
				add_string += infoChar[i];
				if(i == object_description.Length - 1)
				{
					row_count++;
					current_height += height_distance;
				}
			}
				
			if(i == object_description.Length - 1)  //最后一步处理
			{
				if(add_string != "")
				{
					display_info_list.Add(add_string);
					info_string_height.Add(page_separate*page_count*height_distance);
					page_count++;
					add_string = "";
				}
			}
		}
		if(current_height > 160)
			window_height = current_height;
		else
			window_height = 160;
	}
	
	void AddTest()
	{
		string haha = "";
		for(int i = 0; i < 1; i++)
		{
			haha +=i+"X轴超出最大行程，请检查程序及坐标hahahhah是客户端看改好了看大号个的非官方的广泛地你不可能的开个会系设定！数控程序正在运行，无法进行手动换刀！\n";
		}
		AddInfo(haha);
		Debug.Log(row_count);
	}
	
	void FixedUpdate ()
	{
		if(motion_start)
		{
			//进去
			if(come_forth)
			{
				time_value += Time.fixedDeltaTime;
				left = Mathf.Lerp(0, -500f, 2*time_value);
				if(FuncPara.examWindow){  //如果处于考试状态
					if(left + informationRect.width <= -10f)
						FuncPara.examRect.x = 0;
					else
						FuncPara.examRect.x = left + informationRect.width;
				}
				if(2*time_value > 1f)
				{ 
					out_flag = true;
					time_value = 0; 
					come_forth = false;
					motion_start = false;
					warnningWindow = false;
					FuncPara.examRect.x = 0;
				}
			}
			//出来
			else
			{
				time_value += Time.fixedDeltaTime;
				left = Mathf.Lerp(-500f, 0, 2*time_value);
				if(FuncPara.examWindow){  //如果处于考试状态
					FuncPara.examRect.x = left + informationRect.width;
				}
				if(2*time_value > 1f)
				{
					left = 0;
					out_flag = false;
					time_value = 0; 
					come_forth = true;
					motion_start = false;
					FuncPara.examRect.x = informationRect.width;
				}
				
			}
		}
		
		if(time_switch && come_forth && !motion_start)
		{
			time_recorder += Time.deltaTime;
			if(time_recorder > 6)
			{
				motion_start = true;
				time_recorder = 0;
			}
		}
		else
		{
			time_recorder = 0;
		}
		
	}
}
