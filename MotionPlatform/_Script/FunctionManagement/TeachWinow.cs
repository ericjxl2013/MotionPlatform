/// <summary>
/// FileName: TeachWindow.cs
/// Author: Wu Hao
/// Created Time: 2013
/// Version: 1.0
/// Company: Sunnytech
/// Function: 教和练的控制窗口
///
/// Changed By: Jiang Xiaolong
/// Modification Time: 2013.02.26
/// Discription: 
/// </summary>
using UnityEngine;
using System.Collections;


public class TeachWinow : MonoBehaviour {
	
	FunctionManager st_FuncManager;

	//Teach Window
	public float teach_Window_width;
	public float teach_Window_height;
	public float teach_Window_height2;
	[HideInInspector]
	public Rect teach_Window;
	public float btn_width;
	string tex_play;
	string tex_pause;
	string tex_display;
	
	//Phase Window
	Rect phase_Window;
	bool showPhaseWindow;  //显示阶段选择窗口
	public int selected_Button;  //也表示当前选择的阶段，为了按钮的背景颜色
	public float select_btn_width;
	public float w_x;  //阶段窗口与教学窗口x方向偏移量
	public float w_y;  //阶段窗口与教学窗口y方向偏移量
	int phaseCount;  //当前任务阶段数
	const float PROGRESS_VALUE = 100.0f;  //进度条总长
	string[] phaseName;  //每一阶段的名字
	
	//horizontalValue
	float horizontalValue;
	bool mouseOverSlider;  //鼠标在进度条上
	bool drag;
	
	//Phase
	public int currentPhase; 
	
	float hs_x;
	float hs_y;
	float hs_y2;
	float hs_width;
	float btn_x;
	float btn_y;
	float btn_space;
	float labelup_y;
	float labelup_y2;
	float labeldown_y;
	float play_x_offset;
	float phase_btn_height;  //阶段按钮高度
	float phase_btn_space;  //阶段按钮间距
	float phase_btn_x; 
	float phase_btn_edge;
	
	
	// Use this for initialization
	void Start () {
		st_FuncManager = gameObject.GetComponent<FunctionManager>();
		
		teach_Window_width = 400f;
		teach_Window_height = 140f;
		teach_Window_height2 = 50f;
		teach_Window = new Rect(Screen.width-teach_Window_width, 0, teach_Window_width, teach_Window_height);
		
		w_x=0f;
		w_y=teach_Window_height;
		phase_Window=new Rect(0,0,200,280);
		phase_btn_x = 10f;
		phase_btn_edge = 20f;
		select_btn_width = teach_Window_width - 2 * phase_btn_x;
		phase_btn_height = 23f;
		phase_btn_space = 27f;
		
		
		btn_width=55f;
		tex_play="操作示范";
		tex_pause="暂    停";
		tex_display = tex_play;
		play_x_offset = 3f;
		
		hs_x=70f;
		hs_y=10f;
		hs_y2=-1f;
		hs_width=225f;
		btn_x=22.5f;;
		btn_y=50f;
		btn_space=60f;
		labelup_y = 22f;
		labelup_y2 = 11f;
		labeldown_y = 100f;
		showPhaseWindow = false;
		
		//For Test
		phaseCount = 5;
		selected_Button = -2;
//		FuncPara.showTeachWindow = true;
	}
	
	
	void OnGUI () 
	{
		GUI.skin = FuncPara.defaultSkin;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.button.font = FuncPara.defaultFont;
		GUI.skin.button.fontSize = 13;
		GUI.skin.button.normal.textColor = Color.white;
		if(FuncPara.showTeachWindow){
			if(showPhaseWindow){
				teach_Window.height = teach_Window_height;
			}else{
				teach_Window.height = teach_Window_height2;
			}
			teach_Window = GUI.Window(21, teach_Window, DrawTeachWindow, "");  //进度条窗口
			if(showPhaseWindow){
				phase_Window.x = teach_Window.x + w_x;
				phase_Window.y = teach_Window.y + w_y;
				phase_Window.width = teach_Window_width;
				phase_Window.height = phase_btn_space * phaseCount - 4f + 2 * phase_btn_edge;
				phase_Window = GUI.Window(22, phase_Window, DrawPhaseWindow, "");  //阶段选择窗口
			}
		}
		GUI.skin = null;
	}
	
	//启动教练功能
	public bool ActiveTeaching(string[] nameList)
	{
		phaseName = nameList;
		phaseCount = phaseName.Length;
		if(phaseCount >= 1){
			FuncPara.showTeachWindow = true;
			FuncPara.currentMotion = MotionState.Teaching;  //教
			FuncPara.isPlaying = false;  //暂停
			currentPhase = 0;
//			SetCurrentPhase(currentPhase);
			
			return true;
		}else{
			return false;
		}
	}
	
	//播放控制窗口
	void DrawTeachWindow(int WindowID)
	{

		GUI.skin = FuncPara.defaultSkin;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.button.font = FuncPara.defaultFont;
		GUI.skin.button.fontSize = 13;
		GUI.skin.button.normal.textColor = Color.white;

		GUI.depth = 0;
		if(showPhaseWindow){ //有阶段选择窗口
			GUILayout.Space(hs_y);
			GUILayout.BeginHorizontal();
			GUILayout.Space(hs_x);
			horizontalValue = GUILayout.HorizontalSlider(horizontalValue, 0.0f, PROGRESS_VALUE, GUILayout.Width(hs_width));
			if(Event.current.type == EventType.Repaint && 
				GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){
				mouseOverSlider=true;
				showPhaseWindow=true;
			}else{
				mouseOverSlider=false;
			}
			GUILayout.EndHorizontal();
			
			if(FuncPara.speedRate < 1f){
				GUI.color = Color.yellow;
				GUI.Label(new Rect(btn_x + 10f, labelup_y, btn_width, btn_width), "X" + FuncPara.speedRate.ToString("0.0"));
				GUI.color = Color.white;
			}
			if(GUI.Button(new Rect(btn_x, btn_y, btn_width, btn_width), "", FuncPara.sty_SlowDown)){
				//WuHao
				st_FuncManager.st_Manager.SpeedControl(false);
			}
			GUI.Label(new Rect(btn_x + 6f, labeldown_y, btn_width, btn_width),"减    速");
			
			if(GUI.Button(new Rect(btn_x+btn_space, btn_y, btn_width, btn_width), "", FuncPara.sty_Prev)){
				currentPhase--;
				if(currentPhase < 0){
					currentPhase = 0;
				}
				SetCurrentPhase(currentPhase);
			}
			GUI.Label(new Rect(btn_x + btn_space, labeldown_y, btn_width, btn_width),"上一阶段");
			
			if(GUI.Button(new Rect(btn_x+2*btn_space, btn_y, btn_width, btn_width), "", FuncPara.sty_Play)){
				if(tex_display == tex_pause){ //从运行到暂停
					tex_display = tex_play;
					play_x_offset = 3f;
					FuncPara.sty_Play.normal.background = FuncPara.t2d_playN;
					FuncPara.sty_Play.active.background = FuncPara.t2d_playA;

					//WuHao
					st_FuncManager.st_Manager.PauseButton();

					Debug.Log("Pause");

				}else{  //从播放到暂停

					Debug.Log("Play");
					if(FuncPara.currentMotion == MotionState.Exercising){  //练

						//WuHao
						FuncPara.currentMotion = MotionState.Teaching;
						SetCurrentPhase(currentPhase);

						if(currentPhase == 0)
							horizontalValue = 0;
						tex_display = tex_pause;
						play_x_offset = 6f;
						FuncPara.sty_Play.normal.background = FuncPara.t2d_pauseN;
						FuncPara.sty_Play.active.background = FuncPara.t2d_pauseA;
					}else if(FuncPara.currentMotion == MotionState.Teaching){  //教
						//WuHao
						int phaseSet = currentPhase;
						if(st_FuncManager.IsEncrypted(currentPhase, ref phaseSet)){
							return;
						}

						if(!MotionPara.MotionActive){//开始
							st_FuncManager.st_Manager.PlayButton();
						}
						else{//播放
							st_FuncManager.st_Manager.PauseButton();
						}

						currentPhase = phaseSet;

						tex_display = tex_pause;
						play_x_offset = 6f;
						FuncPara.sty_Play.normal.background = FuncPara.t2d_pauseN;
						FuncPara.sty_Play.active.background = FuncPara.t2d_pauseA;
					}
				}
			}
			GUI.Label(new Rect(btn_x + 2*btn_space + play_x_offset, labeldown_y, btn_width, btn_width),tex_display);
			
			
			if(GUI.Button(new Rect(btn_x+3*btn_space,btn_y,btn_width,btn_width), "", FuncPara.sty_Exercise)){
				if(st_FuncManager.IsEncrypted()){  //练加密中
					return;
				}

				//WuHao
				if (FuncPara.currentMotion != MotionState.Exercising)
				{
					FuncPara.currentMotion = MotionState.Exercising;
					SetCurrentPhase(currentPhase);
					FuncPara.cursorShow = false;
				}
			}
			GUI.Label(new Rect(btn_x + 3*btn_space + 2f,labeldown_y,btn_width,btn_width),"同步练习");
	
			if(GUI.Button(new Rect(btn_x+4*btn_space,btn_y,btn_width,btn_width), "", FuncPara.sty_Next)){
				currentPhase++;
				if(currentPhase > (phaseCount - 1)){
					currentPhase = (phaseCount - 1);
				}
				SetCurrentPhase(currentPhase);
			}
			GUI.Label(new Rect(btn_x + 4*btn_space + 2f,labeldown_y,btn_width,btn_width),"下一阶段");
			
			if(FuncPara.speedRate > 1f){
				GUI.color = Color.yellow;
				GUI.Label(new Rect(btn_x + 5*btn_space + 10f,labelup_y,btn_width,btn_width),"X" + FuncPara.speedRate.ToString("0.0"));
				GUI.color = Color.white;
			}
			if(GUI.Button(new Rect(btn_x+5*btn_space,btn_y,btn_width,btn_width), "", FuncPara.sty_SpeedUp)){
				//WuHao
				st_FuncManager.st_Manager.SpeedControl(true);
			}
			GUI.Label(new Rect(btn_x + 5*btn_space + 5f,labeldown_y,btn_width,btn_width),"加    速");
			
			if(FuncPara.helpInfo){  //帮助信息
				if(new Rect(btn_x, btn_y, btn_width, btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "减速功能，减慢教模式动画的播放速度；"){
						FuncPara.helpString = "减速功能，减慢教模式动画的播放速度；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else if(new Rect(btn_x+btn_space, btn_y, btn_width, btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "上一阶段，返回到教模式的上一阶段；"){
						FuncPara.helpString = "上一阶段，返回到教模式的上一阶段；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else if(new Rect(btn_x+2*btn_space, btn_y, btn_width, btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "播放控制，播放/暂停教模式动画的播放；"){
						FuncPara.helpString = "播放控制，播放/暂停教模式动画的播放；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else if(new Rect(btn_x+3*btn_space,btn_y,btn_width,btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "练习功能，进入练习模式；"){
						FuncPara.helpString = "练习功能，进入练习模式；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else if(new Rect(btn_x+4*btn_space,btn_y,btn_width,btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "下一阶段，返回到教模式的下一阶段；"){
						FuncPara.helpString = "下一阶段，返回到教模式的下一阶段；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else if(new Rect(btn_x+5*btn_space,btn_y,btn_width,btn_width).Contains(Event.current.mousePosition)){
					FuncPara.helpProgress = true;
					if(FuncPara.helpString != "加速功能，加快教模式动画的播放速度；"){
						FuncPara.helpString = "加速功能，加快教模式动画的播放速度；";
						st_FuncManager.HelpFormat();
						FuncPara.helpDisplay = true;
					}
				}else{
					FuncPara.helpProgress = false;
					if(!FuncPara.helpHiding && !FuncPara.helpProgress){  //两边都为Flase时触发消失
						FuncPara.helpString = "";
						FuncPara.helpDisplay = false;
					}
				}
			}
			
		}else{  //单独是进度条
			GUILayout.Space(hs_y2);
			GUILayout.BeginHorizontal();
			GUILayout.Space(hs_x);
			horizontalValue = GUILayout.HorizontalSlider(horizontalValue,0.0f, PROGRESS_VALUE,GUILayout.Width(hs_width));
			if(Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){
				mouseOverSlider = true;  //鼠标位于进度条上
				showPhaseWindow = true;  //出现阶段选择窗口
			}else{
				mouseOverSlider = false;
			}
			GUILayout.EndHorizontal();
			
			if(FuncPara.speedRate < 1f){
				GUI.color = Color.yellow;
				GUI.Label(new Rect(btn_x + 10f, labelup_y2, btn_width, btn_width),"X" + FuncPara.speedRate.ToString("0.0"));
				GUI.color = Color.white;
			}
			
			if(FuncPara.speedRate > 1f){
				GUI.color = Color.yellow;
				GUI.Label(new Rect(btn_x+5*btn_space + 10f, labelup_y2, btn_width,btn_width),"X" + FuncPara.speedRate.ToString("0.0"));
				GUI.color = Color.white;
			}
		}

		GUI.skin = null;

		GUI.DragWindow();
	}
	
	//阶段选择窗口
	void DrawPhaseWindow(int WindowID)
	{
		GUI.depth=-1;
		GUILayout.BeginVertical();
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		GUI.skin.button.contentOffset = new Vector2(10f, 0);
		for(int i = 0; i < phaseCount; i++){
			if(selected_Button == (i + 1)){
				GUI.backgroundColor = Color.blue;
			}
			if(st_FuncManager.IsEncrypted(i)){
				GUI.backgroundColor = Color.black;
			}
			if(GUI.Button(new Rect(phase_btn_x, phase_btn_edge + i * phase_btn_space, select_btn_width, phase_btn_height), 
				phaseName[i])){	
				int phaseSet = i;
				if(st_FuncManager.IsEncrypted(i, ref phaseSet)){  //检测是否允许跳转
					return;
				}
				if(i != phaseSet){  //检测是否为选择第一个按钮的情况
					if(st_FuncManager.IsEncrypted()){
						return;
					}
				}
				SetCurrentPhase(i);
			}
			if(st_FuncManager.IsEncrypted(i)){
				GUI.backgroundColor = Color.white;
			}
			if(selected_Button == (i + 1)){
				GUI.backgroundColor = Color.white;
			}
		}
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.contentOffset = new Vector2(0, 0);
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(FuncPara.showTeachWindow){
			//阶段选择窗口消失
			if(!phase_Window.Contains(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y))&&!
				teach_Window.Contains(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y))){
				showPhaseWindow = false;
			}
			
			if(Input.GetMouseButton(0) && mouseOverSlider){  //鼠标按下进度条
				drag=true;;
			}
			
			if(Input.GetMouseButtonUp(0)){  //鼠标从进度条抬起，触发进度跳转
				if(drag){
					DragSlider();
				}
				drag=false;
			}
			
			if(drag){  //鼠标拖动时调整选中菜单的位置
				CheckHorizontalValue();
			}
		}
	}
	
	//拖动进度条触发
	void DragSlider()
	{
		for(int i=0; i < phaseCount;i++){
			if(horizontalValue >= i * (PROGRESS_VALUE / phaseCount) && horizontalValue < (i+1) * 
				(PROGRESS_VALUE / phaseCount)){
				SetCurrentPhase(i);	
			}
		}
		if(horizontalValue == PROGRESS_VALUE){
			SetCurrentPhase(phaseCount - 1);
		}
	}
	
	//阶段跳转，跳转到相应阶段的初始状态
	void SetCurrentPhase(int i){
		//play settings
		tex_display = tex_play;
		play_x_offset = 3f;
		FuncPara.sty_Play.normal.background = FuncPara.t2d_playN;
		FuncPara.sty_Play.active.background = FuncPara.t2d_playA;
		
		selected_Button = i + 1;
		currentPhase = i;
		horizontalValue = i * (PROGRESS_VALUE / phaseCount);
		FuncPara.isPlaying = false;
		FuncPara.startAlready = false;


		//WuHao
		if (MotionPara.MotionActive) 
		{
			st_FuncManager.st_Manager.StopButton();
		}
		st_FuncManager.st_Manager.SetLocation(i+1, "2");
		MotionPara.shouldStop = false;
		st_FuncManager.st_Manager.StartCoroutine(st_FuncManager.st_Manager.MainEntrance(i, 0));
//		MotionPara.PauseControl = true;
		//WuHao
	}
	
	//鼠标拖动进度条时蓝色按钮的位置改变
	void CheckHorizontalValue(){
		for(int i=0; i < phaseCount; i++){
			if(horizontalValue >= i*(PROGRESS_VALUE / phaseCount) && horizontalValue < (i+1) *(PROGRESS_VALUE / phaseCount)){
				selected_Button=i+1;
			}
		}
		if(horizontalValue==100f){
			selected_Button = phaseCount;
		}
	}
	
	/// <summary>
	/// 一个任务播放结束.
	/// </summary>
	public void TaskOver(){
		FuncPara.isPlaying = false;  //播放暂停
		FuncPara.startAlready = false;  //阶段是否已经在播放
		tex_display = tex_play;
		play_x_offset = 3f;
		FuncPara.sty_Play.normal.background = FuncPara.t2d_playN;
		FuncPara.sty_Play.active.background = FuncPara.t2d_playA;
		currentPhase = 0;
	}
	
	/// <summary>
	/// 播放下一阶段.
	/// </summary>
	/// <param name='next_current'>
	/// false-现阶段，true-下一阶段.
	/// </param>
	public void PlayNextStage(bool next_current){
		if(next_current){  //如果是播放下一阶段
			currentPhase++;
			if(currentPhase > (phaseCount - 1)){
				currentPhase = (phaseCount - 1);
				return;
			}
		}
		SetCurrentPhase(currentPhase);
		FuncPara.isPlaying = true;
		FuncPara.startAlready = true;
		//进度条菜单变化
		tex_display = tex_pause;
		play_x_offset = 6f;
		FuncPara.sty_Play.normal.background = FuncPara.t2d_pauseN;
		FuncPara.sty_Play.active.background = FuncPara.t2d_pauseA;
	}
	
	
}
