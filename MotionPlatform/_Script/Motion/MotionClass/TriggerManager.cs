using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//触发类型，可以继续添加
public enum TriggerType { None, MouseDown, MouseUp, MouseDown2, MouseDrag, ScrollWheel, KeyUp, KeyDown, Button }

//触发单元
public struct TriggerUnit
{
    public int ID;
    public TriggerType[] TriggerType;
    public string[] TriggerOjb;
    public string[] TriggerKey;
	public List<string> BtnFuncList;
	public float TriggerLongPress;

	//public TriggerUnit(int id, TriggerType[] trigger_type, string[] trigger_obj, string[] trigger_key)
	//{
	//	ID = id;
	//	TriggerType = trigger_type;
	//	TriggerOjb = trigger_obj;
	//	TriggerKey = trigger_key;
	//}
}

public class TriggerManager : MonoBehaviour {

	//射线Camera
	Camera rayCamera;

    //触发对象信息
    public static List<TriggerUnit> triggerUnitList = new List<TriggerUnit>();

	// Use this for initialization
	void Awake () 
	{

	}

	void OnGUI ()
	{
        for (int i = 0; i < triggerUnitList.Count; i++)
        {
            Event eCurrent = Event.current;
			//单个触发
            if (triggerUnitList[i].TriggerType.Length == 1) 
            {
				SingleTrigger(eCurrent, triggerUnitList[i]);
            }
			//组合触发
            else if(triggerUnitList[i].TriggerType.Length > 1) 
            {
				SeveralTriggers(eCurrent, triggerUnitList[i]);
            }
        }

	}

    //需要考虑单独的触发，组合触发，多个触发中的任一触发


	//单个触发控制函数
    private void SingleTrigger(Event current_event, TriggerUnit trigger_unit)
    {
        //鼠标按下或双击
        if (trigger_unit.TriggerType[0] == TriggerType.MouseDown || trigger_unit.TriggerType[0] == TriggerType.MouseDown2) 
        {
            if (current_event.type == EventType.MouseDown) 
            {
                //鼠标单击事件
                if (current_event.clickCount == 1 && trigger_unit.TriggerType[0] == TriggerType.MouseDown)
                {
                    //判断按键是否符合
                    if (current_event.button.ToString() == trigger_unit.TriggerKey[0])
                    {
                        MouseEvent(trigger_unit);
                    }
                }
                //鼠标双击事件
				else if (current_event.clickCount == 2 && trigger_unit.TriggerType[0] == TriggerType.MouseDown2)
				{
					//判断按键是否符合
					if (current_event.button.ToString() == trigger_unit.TriggerKey[0])
					{
						MouseEvent(trigger_unit);
					}
				}
            }
        }
        //鼠标抬起
        else if (trigger_unit.TriggerType[0] == TriggerType.MouseUp)
        {
			if (current_event.type == EventType.MouseUp)
			{
				//鼠标单击事件
				if (current_event.clickCount == 1)
				{
					//判断按键是否符合
					if (current_event.button.ToString() == trigger_unit.TriggerKey[0])
					{
						MouseEvent(trigger_unit);
					}
				}
			}
        }
        //鼠标拖拽
        else if (trigger_unit.TriggerType[0] == TriggerType.MouseDrag)
        {
            //鼠标拖拽事件
            if (current_event.type == EventType.MouseDrag)
            {
				//判断按键是否符合
				if (current_event.button.ToString() == trigger_unit.TriggerKey[0])
				{
					MouseEvent(trigger_unit);
				}
            }
        }
        //鼠标滚轮滚动
        else if (trigger_unit.TriggerType[0] == TriggerType.ScrollWheel)
        {
            //鼠标滚轮事件
            if (current_event.type == EventType.ScrollWheel)
            {
                MouseEvent(trigger_unit);
            }
        }
		//键盘按键按下
		else if (trigger_unit.TriggerType[0] == TriggerType.KeyDown) 
		{
            if (current_event.type == EventType.KeyDown)
            {
				//Debug.Log(current_event.keyCode.ToString());
				//判断按键是否符合
				//TODO:按键需要做一个映射
				if (current_event.keyCode.ToString().ToLower() == trigger_unit.TriggerKey[0])
				{
					KeyboardEvent(trigger_unit);
				}
            }
		}
		//键盘按键抬起
		else if (trigger_unit.TriggerType[0] == TriggerType.KeyUp) 
		{
            if (current_event.type == EventType.KeyUp)
            {
				//Debug.Log(current_event.keyCode.ToString());
				//判断按键是否符合
				//TODO:按键需要做一个映射
				if (current_event.keyCode.ToString().ToLower() == trigger_unit.TriggerKey[0])
				{
					KeyboardEvent(trigger_unit);
				}
            }
		}
    }

	//ATENTION
	//多个触发，暂时只支持两个触发，且只能是鼠标和鼠标，键盘和键盘
	private void SeveralTriggers(Event current_event, TriggerUnit trigger_unit)
	{
		//鼠标触发
		if (trigger_unit.TriggerType[0] == TriggerType.MouseDown || trigger_unit.TriggerType[0] == TriggerType.MouseUp) 
		{
			int btn1 = 0;
			try
			{
				btn1 = int.Parse(trigger_unit.TriggerKey[0]);
			}
			catch 
			{
				Debug.LogError("Key按键填写的格式错误!");
				return;
			}
			int btn2 = 0;
			try
			{
				btn2 = int.Parse(trigger_unit.TriggerKey[1]);
			}
			catch
			{
				Debug.LogError("Key按键填写的格式错误!");
				return;
			}
			//第二个按键按下
			if (trigger_unit.TriggerType[1] == TriggerType.MouseDown) {
				if (Input.GetMouseButton(btn1) && Input.GetMouseButtonDown(btn2))
				{
					Debug.Log(btn1+ ","+ btn2+ "鼠标组合键触发 Oh Yeah");
					MouseEvent(trigger_unit);
				}
			}
			//第二个按键抬起
			else if (trigger_unit.TriggerType[1] == TriggerType.MouseUp)
			{
				if (Input.GetMouseButton(btn1) && Input.GetMouseButtonUp(btn2))
				{
					Debug.Log(btn1+ ","+ btn2+ "鼠标组合键触发 Oh Yeah");
					MouseEvent(trigger_unit);
				}
			}	
		}
		//键盘触发
		else if (trigger_unit.TriggerType[0] == TriggerType.KeyDown || trigger_unit.TriggerType[0] == TriggerType.KeyUp) 
		{
			//第二个按键按下
			if (trigger_unit.TriggerType[1] == TriggerType.KeyDown) {
				if (Input.GetKey(trigger_unit.TriggerKey[0].ToLower()) && Input.GetKeyDown(trigger_unit.TriggerKey[1].ToLower())) {
					Debug.Log(trigger_unit.TriggerKey[0]+ ","+ trigger_unit.TriggerKey[1]+ "键盘组合键触发 Oh Yeah");
					KeyboardEvent(trigger_unit);
				}
			}
			//第二个按键抬起
			else if (trigger_unit.TriggerType[1] == TriggerType.KeyUp) 
			{
				if (Input.GetKey(trigger_unit.TriggerKey[0].ToLower()) && Input.GetKeyUp(trigger_unit.TriggerKey[1].ToLower()))
				{
					Debug.Log(trigger_unit.TriggerKey[0]+ ","+ trigger_unit.TriggerKey[1]+ "键盘组合键触发 Oh Yeah");
					KeyboardEvent(trigger_unit);
				}
			}
		}
	}

    //射线获得目标物体
    private GameObject RayCollision()
    {
		//ATTENTION: 请确定是从主摄像机发出的射线，不然判断会错误
		if (CameraMotion.CurrentCamera != null)
			rayCamera = CameraMotion.CurrentCamera;
		else
			rayCamera = Camera.main;
		Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    //鼠标事件判断
    private void MouseEvent(TriggerUnit trigger_unit)
    {
        //是否有目标物体
        if (trigger_unit.TriggerOjb.Length > 0)
        {
            GameObject aimObj = RayCollision();
            if (aimObj)
            {
                for (int i = 0; i < trigger_unit.TriggerOjb.Length; i++)
                {
                    if (trigger_unit.TriggerOjb[i] == aimObj.name)
                    {
                        //触发成功
                        Debug.Log(aimObj.name + "，该物体被触发， Oh Yeah");
                        MotionPara.triggerPlay = false;
                        break;
                    }
                }
            }
        }
        else
        {
            //触发成功
            Debug.Log("触发成功，无目标物体， Oh Yeah");
            MotionPara.triggerPlay = false;
        }
    }

	//键盘事件判断
	private void KeyboardEvent(TriggerUnit trigger_unit)
	{ 
		//触发成功
        MotionPara.triggerPlay = false;
	}

	//GUI按钮事件判断
	public static void GUIButtonEvent(string btn_string)
	{
		//Trigger事件处于激活状态
		if (MotionPara.triggerPlay) 
		{
			if (triggerUnitList.Count > 0 && triggerUnitList[0].TriggerType.Length > 0 && triggerUnitList[0].TriggerType[0] == TriggerType.Button)
			{
				if (triggerUnitList[0].TriggerKey.Length > 0)
				{
					if (triggerUnitList[0].TriggerKey[0].ToUpper() == btn_string.ToUpper())
					{
						Debug.Log(btn_string.ToUpper() + "按钮触发成功!");
						//触发成功
						if(triggerUnitList[0].TriggerLongPress == -1){//点击
							MotionPara.triggerPlay = false;
						}
						else{//长按
							Vector3 dis = (GameObject.Find("Cube_2").transform.position-GameObject.Find("Cube_1").transform.position);
							Debug.Log("dis:"+dis.magnitude);
							if(PanelButton.DetectionLongPress(btn_string, triggerUnitList[0].TriggerLongPress)){
								MotionPara.triggerPlay = false;
							}
						}

					}
				}
			}
		}
	}



    //可扩展触发
    public static void OtherTriggerEvent()
    { 
        
    }

	//void Update()
	//{
	//	if (Input.GetKey("1")) {
	//		Debug.Log(1);
	//	}
	//	else if (Input.GetKey("q"))
	//	{
	//		Debug.Log("q");
	//	}
	//	else if (Input.GetKey("return"))
	//	{
	//		Debug.Log("return");
	//	}
	//	else if (Input.GetKey("escape"))
	//	{
	//		Debug.Log("escape");
	//	}
	//	//else if (Input.GetKey("Escape"))
	//	//{
	//	//	Debug.Log("Escape");
	//	//}
	//}

}
