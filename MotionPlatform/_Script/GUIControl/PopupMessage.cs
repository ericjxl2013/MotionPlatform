using UnityEngine;
using System.Collections;

public class PopupMessage : MonoBehaviour {
	string warnningMessage1 = "";  //第一排的警告信息
	string warnningMessage2 = "";  //第二排的警告信息
	bool warnningFlag = false;
	Rect popupRect = new Rect(0, 0, 300, 151);
	// Use this for initialization
	void Start () {
		
	}
	
	void OnGUI () {
		if(warnningFlag){
			popupRect = GUI.Window(15, popupRect, PopupWindow, "", FuncPara.sty_PopupWindow);
			GUI.BringWindowToFront (15);
			GUI.FocusWindow (15);
		}
	}
	
	void PopupWindow(int WindowID)
	{
		GUI.skin.label.font = FuncPara.defaultFont;
		GUI.skin.label.fontSize = 13;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect (100f, 57f, 107f, 31f), warnningMessage1);
		GUI.Label(new Rect (100f, 77f, 107f, 31f), warnningMessage2);
		GUI.skin.label = null;
		
		if (GUI.Button (new Rect (123f, 106f, 107f, 31f), "确定", FuncPara.sty_SquareBtn)){
			warnningFlag  = false;
		}
//		if(GUI.Button (new Rect (270f, 13f, 15f, 15f), "", Main.sty_PowerNotifi_cancel))
//			warnningFlag = false;
		GUI.DragWindow();
	}
	
	/// <summary>
	/// Popup the specified message.
	/// </summary>
	/// <param name='message'>
	/// Warnning Message.
	/// </param>
	public void Popup(string message) {
		if(message.Length <= 13){
			warnningMessage1 = message;
			warnningMessage2 = "";
		}else{
			warnningMessage1 = message.Substring(0, 13);
			warnningMessage2 = message.Substring(13);		}
		popupRect.x = (Screen.width - popupRect.width) / 2;
		popupRect.y = (Screen.height - popupRect.height) / 2;
		warnningFlag = true;
		
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyUp(KeyCode.T)){
//			Popup("你好你好你好你好你好你好你好你好你好你好你好你好!");
//		}
	}
}
