/// <summary>
/// FileName: TipsWindow.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.02.24
/// Version: 1.0
/// Company: Sunnytech
/// Function: 提示窗口
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WindowColor {Blue, LightBlue, Orange, Pink, Purple, Yellow, Black, BlueViolet, DarkBlue, Green}

public class TipsWindow : MonoBehaviour {
	
	 FunctionManager st_FuncManager;

	Rect tipsRect;
	const float LINT_HEIGHT = 19f;
	const float ORIGIN_HEIGHT = 60f;
	public GUIText tipsText;  //GUI Text用于字符宽度计算
	string displayString = "";  //提示信息内容
	int stringCount = 0;  //提示信息行数
	 Rect helpRect;
	 int helpStrCount = 0;  //帮助信息行数
	
	 Rect exerciseRect;  //练习时出现的提示窗口
	
	 Rect componentsRect;  //部件提示窗口
	
	// Use this for initialization
	void Awake () {
		tipsRect = new Rect(100f, 100f, 263f, 60f);
		try{
			tipsText = GameObject.Find("TipsText").guiText;
		}catch{
			Debug.LogError("请手动添加一个GUI Text，并且命名为“TipsText”，" + 
				"同时将GUIText属性勾选为False，用于计算Tips显示长度。");
			return;
		}
		tipsText.font = (Font)Resources.Load ("Font/msyh");
		tipsText.fontSize = 15;
		 helpRect = new Rect(100f, 100f, 249f, 60f);
		 exerciseRect = new Rect(0, 0, 305, 225); 
		 exerciseRect.x = (Screen.width - exerciseRect.width) / 2f;
		 exerciseRect.y = (Screen.height - exerciseRect.height) / 2f;
		 componentsRect = new Rect(100f, 100f, 160f, 60f);
	}
	
	void Start () {
		 st_FuncManager = gameObject.GetComponent<FunctionManager>();
	}
	
	void OnGUI () {

		GUI.skin = FuncPara.defaultSkin;
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.button.font = FuncPara.defaultFont;
		GUI.skin.button.fontSize = 13;
		GUI.skin.button.normal.textColor = Color.white;

		//提示窗口
		if(FuncPara.tipsWindow){
			tipsRect = GUI.Window(20, tipsRect, MyTipsWindow, "", FuncPara.sty_TipsWindow);  //提示信息
//			GUI.BringWindowToFront(20);
		}
		
		// //帮助信息窗口
		// if(FuncPara.helpInfo){
		// 	if(FuncPara.helpDisplay){
		// 		helpRect.x = Input.mousePosition.x + 12f;
		// 		helpRect.y = Screen.height - Input.mousePosition.y + 4f;
		// 		if(helpRect.x + helpRect.width > Screen.width)
		// 			helpRect.x = Screen.width - helpRect.width;
		// 		if(helpRect.y + helpRect.height > Screen.height)
		// 			helpRect.y = Screen.height - helpRect.height;
		// 		helpRect = GUI.Window(26, helpRect, HelpWindow, "", FuncPara.sty_HelpWindow);  //帮助信息
		// 		GUI.BringWindowToFront(26);
		// 	}
		// }
		
		 //部件提示窗口
		 if(FuncPara.componentTips){
		 	if(FuncPara.componentDisplay){
		 		componentsRect.x = Input.mousePosition.x + 12f;
		 		componentsRect.y = Screen.height - Input.mousePosition.y + 4f;
		 		if(componentsRect.x + componentsRect.width > Screen.width)
		 			componentsRect.x = Screen.width - componentsRect.width;
		 		if(componentsRect.y + componentsRect.height > Screen.height)
		 			componentsRect.y = Screen.height - componentsRect.height;
		 		componentsRect = GUI.Window(29, componentsRect, ComponentsWindow, "", FuncPara.sty_PartsWindow);  //部件提示信息
		 		GUI.BringWindowToFront(29);
		 	}
		 }
		
		// GUI.skin.window = FuncPara.skin_hiMenu.window;
		
		// //练习提示窗口
		// if(FuncPara.exerciseWindow){
		// 	exerciseRect = GUI.Window(1, exerciseRect, ExerciseWindow, "");
		// 	GUI.BringWindowToFront(1);
		// }
		// GUI.skin = null;

		GUI.skin = null;
	}
	
	void MyTipsWindow(int WindowID){
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 15;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.wordWrap = true;
		GUI.Label(new Rect(19f, 17f, 230f, 200f), displayString);
		GUI.skin.label.font = null;
		if(FuncPara.tipsMove)
			GUI.DragWindow();
	}
	
	// void HelpWindow(int WindowID){
	// 	GUI.depth = 1;
	// 	if(helpStrCount > 1)
	// 		GUI.Label(new Rect(12f, 7f, 230f, 200f), FuncPara.helpString, FuncPara.sty_MsyhBlack15);
	// 	else
	// 		GUI.Label(new Rect(12f, 17f, 230f, 200f), FuncPara.helpString, FuncPara.sty_MsyhBlack15);
	// }
	
// 	void ExerciseWindow(int WindowID){
// 		GUI.skin.label = null;
// 		GUI.skin.label.font = FuncPara.defaultFont;
// 		GUI.skin.label.fontSize = 17;
// 		GUI.skin.label.normal.background = null;
// 		GUI.skin.label.normal.textColor = Color.white;
// 		GUI.Label(new Rect(10, 8, 300, 50), "练习窗口");
		
// 		if(GUI.Button(new Rect (255, 8, 25, 25), "", FuncPara.sty_Close)){
// 			FuncPara.exerciseWindow = false;
// 		}
		
// 		GUI.skin.button = FuncPara.skin_hiMenu.customStyles[5];
// 		GUI.skin.button.font = FuncPara.defaultFont;
// 		GUI.skin.button.fontSize = 17;
		
// 		if(GUI.Button(new Rect(66, 73, 173, 47), "下一阶段")){
// 			st_FuncManager.PlayNextStage(true);
// 			FuncPara.exerciseWindow = false;
// 		}
		
// 		if(GUI.Button(new Rect(66, 141, 173, 47), "重新练习")){
// 			st_FuncManager.PlayNextStage(false);
// 			FuncPara.exerciseWindow = false;
// 		}
// 		GUI.skin = null;
// //		GUI.DragWindow();
// 	}
	
	 void ComponentsWindow(int WindowID)
	 {
	 	GUI.depth = 1;
	 	GUI.skin.label = FuncPara.defaultSkin.label;
	 	GUI.skin.label.alignment = TextAnchor.UpperCenter;
	 	GUI.skin.label.font = FuncPara.defaultFont;
	 	GUI.skin.label.fontSize = 15;
	 	GUI.skin.label.normal.background = null;
	 	GUI.skin.label.normal.textColor = Color.black;
	 	GUI.Label(new Rect(0f, 15f, 160f, 40f), FuncPara.componentString);
	 	GUI.skin.label.alignment = TextAnchor.UpperLeft;  //要回到默认对齐模式，不然会影响到其余的排版
	 }
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow)
	{
		displayString = tips_message;
		if(FuncPara.sty_TipsWindow.normal.background != FuncPara.t2d_tipsWindow){
			FuncPara.sty_TipsWindow.normal.background = FuncPara.t2d_tipsWindow;
		}
		//stringCount = TipsFormat(tips_message);
		TipsFormat(tips_message);
		FuncPara.tipsWindow = true;
		FuncPara.tipsMove = move_allow;
		tipsRect.x = Screen.width - tipsRect.width;
		tipsRect.y = Screen.height - tipsRect.height;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow, WindowColor window_color)
	{
		displayString = tips_message;
		FuncPara.sty_TipsWindow.normal.background = FuncPara.t2d_colorWindow[window_color];
		//stringCount = TipsFormat(tips_message);
		TipsFormat(tips_message);
		FuncPara.tipsWindow = true;
		FuncPara.tipsMove = move_allow;
		tipsRect.x = Screen.width - tipsRect.width;
		tipsRect.y = Screen.height - tipsRect.height;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow, Vector2 target_position)
	{
		RectAdjust(tips_message, move_allow);
		tipsRect.x = target_position.x;
		tipsRect.y = target_position.y;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='aim_rect'>
	/// 显示位置
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow, Vector2 target_position, WindowColor window_color)
	{
		RectAdjust(tips_message, move_allow, window_color);
		tipsRect.x = target_position.x;
		tipsRect.y = target_position.y;
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow, string position_string)
	{
		RectAdjust(tips_message, move_allow);
		if(position_string == "down_left"){
			tipsRect.x = 0;
			tipsRect.y = Screen.height - tipsRect.height;
		}else if(position_string == "down_right"){
			tipsRect.x = Screen.width - tipsRect.width;
			tipsRect.y = Screen.height - tipsRect.height;
		}else if(position_string == "top_right"){
			tipsRect.x = Screen.width - tipsRect.width;
			tipsRect.y = 0;
		}else if(position_string == "top_left"){
			tipsRect.x = 0;
			tipsRect.y = 0;
		}else{
			tipsRect.x = (Screen.width - tipsRect.width) / 2;
			tipsRect.y = (Screen.height - tipsRect.height) / 2;
		}
	}
	
	/// <summary>
	/// 显示并格式化提示信息.
	/// </summary>
	/// <param name='tips_message'>
	/// Tips_message.
	/// </param>
	/// <param name='move_allow'>
	/// 是否允许移动窗口.
	/// </param>
	/// <param name='position_string'>
	/// 显示位置： down_left, down_right, top_right, top_left, center
	/// </param>
	/// <param name='window_color'>
	/// 背景颜色选择.
	/// </param>
	public void RectAdjust(string tips_message, bool move_allow, string position_string, WindowColor window_color)
	{
		RectAdjust(tips_message, move_allow, window_color);
		if(position_string == "down_left"){
			tipsRect.x = 0;
			tipsRect.y = Screen.height - tipsRect.height;
		}else if(position_string == "down_right"){
			tipsRect.x = Screen.width - tipsRect.width;
			tipsRect.y = Screen.height - tipsRect.height;
		}else if(position_string == "top_right"){
			tipsRect.x = Screen.width - tipsRect.width;
			tipsRect.y = 0;
		}else if(position_string == "top_left"){
			tipsRect.x = 0;
			tipsRect.y = 0;
		}else{
			tipsRect.x = (Screen.width - tipsRect.width) / 2;
			tipsRect.y = (Screen.height - tipsRect.height) / 2;
		}
	}
	
	//提示窗口字符格式化
	private int TipsFormat(string tips_message)
	{
		string tempStr = "";
		int rowCount = 0;
		for(int i = 0; i < tips_message.Length; i++){
			tempStr += tips_message[i].ToString();
			tipsText.text = tempStr;
			if(tipsText.GetScreenRect().width > 220f){  //超过了一行
				rowCount++;
				tempStr = "";
			}
		}
		if(tempStr != "")
			rowCount++;
		if(rowCount < 2){
			tipsRect.height = ORIGIN_HEIGHT;
			tipsText.text = displayString;
			tipsRect.width = (19f * 2f + tipsText.GetScreenRect().width);
		}else{
			tipsRect.height = ORIGIN_HEIGHT + (rowCount - 2) * LINT_HEIGHT + 20f;
			tipsRect.width = 263f;
		}
		return rowCount;
	}
	
	// /// <summary>
	// /// 帮助提示信息格式化.
	// /// </summary>
	 public void HelpInfoFormat(){
	 	string tempStr = "";
	 	int rowCount = 0;
	 	for(int i = 0; i < FuncPara.helpString.Length; i++){
	 		if(FuncPara.helpString[i] == '\n'){
	 			rowCount++;
	 			tempStr = "";
	 			continue;
	 		}
	 		tempStr += FuncPara.helpString[i].ToString();
	 		tipsText.text = tempStr;
	 		if(tipsText.GetScreenRect().width > 220f){  //超过了一行
	 			rowCount++;
	 			tempStr = "";
	 		}
	 	}
	 	if(tempStr != "")
	 		rowCount++;
	 	if(rowCount < 3)
	 		helpRect.height = ORIGIN_HEIGHT;
	 	else
	 		helpRect.height = ORIGIN_HEIGHT + (rowCount - 2) * LINT_HEIGHT;
	 	helpStrCount = rowCount;
	 }
	
}
