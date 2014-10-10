using UnityEngine;
using System.Collections;
using System;

using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;

public class doMoldLable : MonoBehaviour {
	
	//读取零件树
	readExcel m_read_Tree_xls;
	Material mmaterial;
	ArrayList array_pos = new ArrayList();
	ArrayList array_pos_lu = new ArrayList();
	ArrayList array_pos_ld = new ArrayList();
	ArrayList array_pos_ru = new ArrayList();
	ArrayList array_pos_rd = new ArrayList();
	ArrayList array_pos_left = new ArrayList();
	ArrayList array_pos_right = new ArrayList();
	float ftime	= 0;
	float fangle = (float)(Math.PI / 7);
	GUIStyle mGuisty_Point = new GUIStyle();
	//用于引出线背景色
	public GUIStyle mbtnsty1 = new GUIStyle();
	public GUIStyle mbtnsty2 = new GUIStyle();

	//WuHao
	int select=0;
	int selectButton = 0;
	Vector2 old_pos = new Vector2(0,0);
	//WuHao 显示方式：true--从json文件读入引出线信息和Camera信息; false--使用朱伟的自动调节
	public bool showType;	

	public int old_toolbarInt;
	public int toolbarInt = 0;
	public string[] toolbarStrings = new string[] {"朱伟的引出线", "吴浩的引出线"};
	//引出线信息保存的文件
	public string label_file;

	enum moveType
	{
		toRiht = 0,
		toLeft,
		toDown,
		toUp
	}
	struct LINEDATE
	{
			
	}
	
	string cameraType = "Orthographic";  //摄像机类型，默认正交相机：Orthographic，另一种透视相机：Perspective
	float orthographicPara = 2.5f; //正交相机小于这一值则为近距离，按照自己的模型调整
	float perspectivePara = 15f;  //透视相机小于这一值为近距离，按照自己的模型调整
	
	void Awake()
	{
		mGuisty_Point.stretchWidth = true;
		mGuisty_Point.stretchHeight = true;
		mGuisty_Point.normal.background = (Texture2D)Resources.Load("LineResources/Picture/2");
		//读取零件树;
		m_read_Tree_xls = new readExcel();
		
		mbtnsty1.normal.background = (Texture2D)Resources.Load("LineResources/Picture/bg");
		mbtnsty1.hover.background = (Texture2D)Resources.Load("LineResources/Picture/bg");
		mbtnsty1.alignment = TextAnchor.MiddleCenter;
		mbtnsty1.hover.textColor = new Color(1,1,1,1);
		mbtnsty1.border.top = 10; mbtnsty1.border.bottom = 10; mbtnsty1.border.left = 10; mbtnsty1.border.right = 10;
		mbtnsty1.stretchWidth = true;
		
		mbtnsty2.normal.background = (Texture2D)Resources.Load("LineResources/Picture/bgImg");
		mbtnsty2.hover.background = (Texture2D)Resources.Load("LineResources/Picture/bgImg");
		mbtnsty2.hover.textColor = new Color(0,0,0);
		mbtnsty2.alignment = TextAnchor.MiddleCenter;
		mbtnsty2.hover.textColor = new Color(1,1,1,1);
		mbtnsty2.border.top = 10; mbtnsty2.border.bottom = 10; mbtnsty2.border.left = 10; mbtnsty2.border.right = 10;
		mbtnsty2.stretchWidth = true;
		mmaterial = (Material)Resources.Load("LineResources/LineColor/linecolor");
		if(camera.orthographic){
			cameraType = "Orthographic";  //此为正交相机
		}else{
			cameraType = "Perspective";  //此为透视相机
		}

		label_file = "主视图";
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.P)){
			Debug.Log("array_pos_lu.Count:"+array_pos_lu.Count);
		}

		if(FuncPara.outlineOn && showType){
			if(Input.GetMouseButtonDown(0)){

				Vector3 mouse_pos = Input.mousePosition;
				Vector2 mouse_pos2 = new Vector2(mouse_pos.x, Screen.height - mouse_pos.y);

				for(int i = 0 ; i < array_pos_lu.Count; i++)
				{
					if(((doLableData)array_pos_lu[i]).rGUIRect.Contains(mouse_pos2)){
						select = 1;

						selectButton = i;

						old_pos = mouse_pos2;

						break;
					}

					Rect point  = new Rect(((doLableData)(array_pos_lu[i])).vPosEnd.x - 5,((doLableData)(array_pos_lu[i])).vPosEnd.y - 5,10,10);
					if(point.Contains(mouse_pos2)){
						select = 2;
						
						selectButton = i;
						
						old_pos = mouse_pos2;
						
						break;
					}

				}
			}

			if(Input.GetMouseButton(0)){
				if(select == 0){

				}
				else{

					Vector3 mouse_pos = Input.mousePosition;
					Vector2 mouse_pos2 = new Vector2(mouse_pos.x, Screen.height - mouse_pos.y);

					if(select == 1){
						Rect tmp_rect = ((doLableData)array_pos_lu[selectButton]).rGUIRect;
						tmp_rect = new Rect(tmp_rect.x+ mouse_pos2.x - old_pos.x, tmp_rect.y+ mouse_pos2.y -old_pos.y, tmp_rect.width, tmp_rect.height);
						((doLableData)array_pos_lu[selectButton]).rGUIRect = tmp_rect;

						((doLableData)array_pos_lu[selectButton]).vPosStart = new Vector3(tmp_rect.x + tmp_rect.width/2, tmp_rect.y + tmp_rect.height/2, 0);

						old_pos = mouse_pos2;
					}
					else if(select == 2){
						((doLableData)array_pos_lu[selectButton]).vPosEnd += new Vector3(mouse_pos2.x - old_pos.x, mouse_pos2.y -old_pos.y, 0);
						
						old_pos = mouse_pos2;
					}
				}
			}

			if(Input.GetMouseButtonUp(0)){
				select = 0;

				selectButton = 0;
			}
		}
	}

	#region tool functions
	string Vector3ToString(Vector3 vec3){
		return vec3.x+ ","+ vec3.y+ ","+ vec3.z;
	}
	string RectToString(Rect rect){
		return rect.x+ ","+ rect.y+ ","+ rect.width+ ","+ rect.height;
	}
	Vector3 StringToVector3(string vec3){
		Vector3 tmp = new Vector3(0,0,0);

		string[] strs_tmp = vec3.Split(',');
		tmp.x = float.Parse(strs_tmp[0]);
		tmp.y = float.Parse(strs_tmp[1]);
		tmp.z = float.Parse(strs_tmp[2]);

		return tmp;
	}
	Rect StringToRect(string rect){
		Rect tmp = new Rect(0,0,0,0);

		string[] strs_tmp = rect.Split(',');
		tmp.x = float.Parse(strs_tmp[0]);
		tmp.y = float.Parse(strs_tmp[1]);
		tmp.width = float.Parse(strs_tmp[2]);
		tmp.height = float.Parse(strs_tmp[3]);

		return tmp;
	}
	#endregion

	//记录当前引出线信息
	void recordLabelInfo(string json_Name, string camera_sheet, string label_sheet){

		//引出线信息
		string file_path = UnityEngine.Application.dataPath + "/Resources/Cognitive_Structure/" + MotionPara.deviceName+ "/"+ json_Name+ ".json";

		if((FuncPara.taskActive == Task.Structrue)){
			file_path = UnityEngine.Application.dataPath + "/Resources/Cognitive_Structure/" + MotionPara.deviceName+ "/"+ json_Name+ ".json";
		}
		else if((FuncPara.taskActive == Task.Task3)){

			file_path = MotionPara.taskRootPath + MotionPara.taskName + "/XIAN_Content.json";
		}

		string sheet_name = label_sheet;
		string[] column_name = new string[]{"vPosStart", "vPosEnd", "rGUIRect", "strPartName"};

		string[,] insert_content = new string[4, m_read_Tree_xls.mmenu_top_array.Count];

		int sum = 0;
		for(int i = 0 ; i < array_pos_lu.Count; i++,sum++)
		{
			insert_content[0, sum] = Vector3ToString(((doLableData)(array_pos_lu[i])).vPosStart);
			insert_content[1, sum] = Vector3ToString(((doLableData)(array_pos_lu[i])).vPosEnd);
			insert_content[2, sum] = RectToString(((doLableData)(array_pos_lu[i])).rGUIRect);
			insert_content[3, sum] = ((doLableData)(array_pos_lu[i])).strPartName;
		}
		for(int i = 0 ; i < array_pos_ld.Count; i++,sum++)
		{
			insert_content[0, sum] = Vector3ToString(((doLableData)(array_pos_ld[i])).vPosStart);
			insert_content[1, sum] = Vector3ToString(((doLableData)(array_pos_ld[i])).vPosEnd);
			insert_content[2, sum] = RectToString(((doLableData)(array_pos_ld[i])).rGUIRect);
			insert_content[3, sum] = ((doLableData)(array_pos_ld[i])).strPartName.ToString();
		}
		
		for(int i = 0 ; i < array_pos_ru.Count; i++,sum++)
		{
			insert_content[0, sum] = Vector3ToString(((doLableData)(array_pos_ru[i])).vPosStart);
			insert_content[1, sum] = Vector3ToString(((doLableData)(array_pos_ru[i])).vPosEnd);
			insert_content[2, sum] = RectToString(((doLableData)(array_pos_ru[i])).rGUIRect);
			insert_content[3, sum] = ((doLableData)(array_pos_ru[i])).strPartName.ToString();
		}
		for(int i = 0 ; i < array_pos_rd.Count; i++,sum++)
		{
			insert_content[0, sum] = Vector3ToString(((doLableData)(array_pos_rd[i])).vPosStart);
			insert_content[1, sum] = Vector3ToString(((doLableData)(array_pos_rd[i])).vPosEnd);
			insert_content[2, sum] = RectToString(((doLableData)(array_pos_rd[i])).rGUIRect);
			insert_content[3, sum] = ((doLableData)(array_pos_rd[i])).strPartName.ToString();
		}

		//写入json文件
		JsonOperator jo = new JsonOperator();
		if(FuncPara.taskActive == Task.Task3){
			sheet_name = (json_Name+"_"+sheet_name);
		}
		jo.JsonWriter(file_path, sheet_name, column_name, insert_content, false);


		//Camera信息
		string sheet_name2 = camera_sheet;
		if(FuncPara.taskActive == Task.Task3){
			sheet_name2 = (json_Name+"_"+sheet_name2);
		}
		string[] column_name2 = new string[]{"name", "position", "eulerangles", "fieldofview"};

		string camera_name = "Main Camera";
		string[,] insert_content2 = new string[4,1];
		insert_content2[0,0] = camera_name;
		insert_content2[1,0] = Vector3ToString(GameObject.Find(camera_name).transform.position);
		insert_content2[2,0] = Vector3ToString(GameObject.Find(camera_name).transform.eulerAngles);
		insert_content2[3,0] = GameObject.Find(camera_name).camera.fieldOfView.ToString();
		//写入json文件
		JsonOperator jo2 = new JsonOperator();
		jo2.JsonWriter(file_path, sheet_name2, column_name2, insert_content2, false);
	}

	//载入引出线的信息
	void loadLabelInfo(string json_Name, string camera_sheet, string label_sheet){

		string file_path = "";

		if((FuncPara.taskActive == Task.Structrue)){
			m_read_Tree_xls.readXLS("/Resources/Cognitive_Structure/" + MotionPara.deviceName+"/DoLable.xls", json_Name);
		
			file_path = UnityEngine.Application.dataPath + "/Resources/Cognitive_Structure/" + MotionPara.deviceName+ "/"+ json_Name+ ".json";
		}
		else if((FuncPara.taskActive == Task.Task3)){
			m_read_Tree_xls.readJson("/Resources/" + MotionPara.taskName+ "/XIAN.json", json_Name);
		
			file_path = MotionPara.taskRootPath + MotionPara.taskName + "/XIAN_Content.json";
		}

		if(!File.Exists(file_path)){
			Debug.LogError(file_path+"文件名不存在");
		}
		else{

			//相机信息
			string sheet_name = camera_sheet;
			if(FuncPara.taskActive == Task.Task3){
				sheet_name = (json_Name+"_"+sheet_name);
			}

			JsonOperator jo = new JsonOperator();
			DataTable posTable = jo.JsonReader(file_path, sheet_name);

			if(posTable == null){

				Debug.LogError(file_path+ "文件中不存在"+ sheet_name);

				return;
			}

			string camera_name = posTable.Rows[0][0].ToString();

			if((FuncPara.taskActive == Task.Structrue)){
				StartCoroutine(setCamera(camera_name, StringToVector3(posTable.Rows[0][1].ToString()),
			                                                      StringToVector3(posTable.Rows[0][2].ToString()), float.Parse(posTable.Rows[0][3].ToString())));
			}
			else if((FuncPara.taskActive == Task.Task3)){
				GameObject.Find(camera_name).transform.position = StringToVector3(posTable.Rows[0][1].ToString());
				GameObject.Find(camera_name).transform.eulerAngles = StringToVector3(posTable.Rows[0][2].ToString());
				GameObject.Find(camera_name).camera.fieldOfView = float.Parse(posTable.Rows[0][3].ToString());
			}

			//引出线信息
			string sheet_name2 = label_sheet;
			if(FuncPara.taskActive == Task.Task3){
				sheet_name2 = (json_Name+"_"+sheet_name2);
			}

			JsonOperator jo2 = new JsonOperator();
			DataTable posTable2 = jo2.JsonReader(file_path, sheet_name2);

			//清空历史数据;
			array_pos.Clear();
			array_pos_ld.Clear();
			array_pos_lu.Clear();
			array_pos_rd.Clear();
			array_pos_ru.Clear();
			array_pos_left.Clear();
			array_pos_right.Clear();

			for(int i=0; i<posTable2.Rows.Count; i++){
				DataRow dr= posTable2.Rows[i];

				doLableData midData = new doLableData();

				((doLableData)(midData)).vPosStart = StringToVector3(dr[0].ToString());
				((doLableData)(midData)).vPosEnd = StringToVector3(dr[1].ToString());
				((doLableData)(midData)).rGUIRect = StringToRect(dr[2].ToString());
				((doLableData)(midData)).strPartName = dr[3].ToString();

				//修正位置
				float origin_width = 1280f;
				float origin_height = 890f;

				((doLableData)(midData)).vPosStart.x = ((doLableData)(midData)).vPosStart.x / origin_width * Screen.width;
				((doLableData)(midData)).vPosStart.y = ((doLableData)(midData)).vPosStart.y / origin_height * Screen.height;

				((doLableData)(midData)).vPosEnd.x = ((doLableData)(midData)).vPosEnd.x / origin_width * Screen.width;
				((doLableData)(midData)).vPosEnd.y = ((doLableData)(midData)).vPosEnd.y / origin_height * Screen.height;

				((doLableData)(midData)).rGUIRect.x = ((doLableData)(midData)).rGUIRect.x / origin_width * Screen.width;
				((doLableData)(midData)).rGUIRect.y = ((doLableData)(midData)).rGUIRect.y / origin_height * Screen.height;
				//修正位置

				array_pos_lu.Add(midData);
			}

		}
	}

	//相机视角切换
	IEnumerator setCamera(string cam_name, Vector3 cam_pos, Vector3 cam_elu, float fov){

		float time = 0f;

		Vector3 pos_tmp = cam_pos - GameObject.Find(cam_name).transform.position;
		Vector3 elu_tmp = cam_elu - GameObject.Find(cam_name).transform.eulerAngles;
		float fov_tmp = fov - GameObject.Find(cam_name).camera.fieldOfView;

		while(time <= 1f)
		{
			float tmp_time = 0.01f;

			//Camera的视角渐变
			GameObject.Find(cam_name).transform.position += pos_tmp * tmp_time;
			GameObject.Find(cam_name).transform.eulerAngles += elu_tmp * tmp_time;
			GameObject.Find(cam_name).camera.fieldOfView += fov_tmp * tmp_time;

			GameObject.Find(cam_name).transform.LookAt(GameObject.Find("Cube").transform);

			yield return new WaitForSeconds(tmp_time);
			time += tmp_time;
		}

		StartCoroutine(setCamera2(cam_name, cam_pos, cam_elu, fov));

		yield return null;
	}

	//相机视角切换
	IEnumerator setCamera2(string cam_name, Vector3 cam_pos, Vector3 cam_elu, float fov){
		
		float time = 0f;
		
		Vector3 pos_tmp = cam_pos - GameObject.Find(cam_name).transform.position;
		Vector3 elu_tmp = cam_elu - GameObject.Find(cam_name).transform.eulerAngles;
		float fov_tmp = fov - GameObject.Find(cam_name).camera.fieldOfView;
		
		while(time <= 1f)
		{
			float tmp_time = 0.01f;
			
			//Camera的视角渐变
			GameObject.Find(cam_name).transform.position += pos_tmp * tmp_time;
			GameObject.Find(cam_name).transform.eulerAngles += elu_tmp * tmp_time;
			GameObject.Find(cam_name).camera.fieldOfView += fov_tmp * tmp_time;

			yield return new WaitForSeconds(tmp_time);
			time += tmp_time;
		}
		
		GameObject.Find(cam_name).transform.position = cam_pos;
		GameObject.Find(cam_name).transform.eulerAngles = cam_elu;
		GameObject.Find(cam_name).camera.fieldOfView = fov;
		
		//相机位置调整好之后显示引出线
		FuncPara.outlineOn = true;
		
		yield return null;
	}

	//显示模式切换
	public void showLabelType(string fileName, bool showType_tmp){

		StopAllCoroutines();

		label_file = fileName;

		if(showType_tmp){//
			loadLabelInfo(label_file, "CAMERA", "XIAN");

			showType = true;
			toolbarInt = 1;
			old_toolbarInt = toolbarInt;

			FuncPara.outlineOn = false;
		}
		else{
			showType = false;
			toolbarInt = 0;
			old_toolbarInt = toolbarInt;

//			m_read_Tree_xls.readXLS("/Resources/Cognitive_Structure/" + MotionPara.deviceName+"/DoLable.xls", fileName);
			if((FuncPara.taskActive == Task.Structrue)){
				m_read_Tree_xls.readXLS("/Resources/Cognitive_Structure/" + MotionPara.deviceName+"/DoLable.xls", fileName);
			}
			else if((FuncPara.taskActive == Task.Task3)){
				m_read_Tree_xls.readJson("/Resources/" + MotionPara.taskName+ "/XIAN.json", fileName);
			}
		}
	}
	
	void OnGUI()
	{

		GUI.skin = FuncPara.defaultSkin;
//		if(FuncPara.outlineOn && (FuncPara.taskActive == Task.Structrue)){
		if(FuncPara.outlineOn && MotionPara.isEditor){
			if(GUI.Button(new Rect(Screen.width-300, 70, 250, 30), "记录当前引出线信息")){
				Debug.Log("recordLabelInfo");

				recordLabelInfo(label_file, "CAMERA", "XIAN");
			}

			old_toolbarInt = toolbarInt;
			toolbarInt = GUI.Toolbar(new Rect(Screen.width-300, 30, 250, 30), toolbarInt, toolbarStrings);

			GUI.Label(new Rect(Screen.width-300, 110, 120,50), "引出线信息保存的文件名");
			label_file = GUI.TextField(new Rect(Screen.width-170, 110, 120,30), label_file);

			if(old_toolbarInt != toolbarInt){

				showType = (toolbarInt == 0);

				if(!showType){
					showType = true;
					
					//禁止相机运动
//					CameraOrth.camera_move = false;
					
					Debug.Log("loadLabelInfo");

					loadLabelInfo(label_file, "CAMERA", "XIAN");
				}
				else{
					showType = false;
					
					//允许相机运动
//					CameraOrth.camera_move = true;

				}
			}
		}

		if(FuncPara.outlineOn && !showType){
		
			//清空历史数据;
			array_pos.Clear();
			array_pos_ld.Clear();
			array_pos_lu.Clear();
			array_pos_rd.Clear();
			array_pos_ru.Clear();
			array_pos_left.Clear();
			array_pos_right.Clear();
			
			float distanceJudge = 1f;
			float measurementPara = 1.2f;
			if(cameraType == "Orthographic"){
				distanceJudge = camera.orthographicSize;
				measurementPara = orthographicPara;
			}else{
				distanceJudge = camera.fieldOfView;
				measurementPara = perspectivePara;
			}
			
			if(distanceJudge < measurementPara)  //距离较近时
			{
				for(int j = 0 ; j < m_read_Tree_xls.mmenu_top_array.Count;j++)
				{
					for(int i = 0; i < ((ArrayList)m_read_Tree_xls.mmenu_top_array[j]).Count; i++)
					{
						string mid_str_name = ((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[j])[i])[1]; //Excel表格第三列：物体名字
					
						//Wuhao
						bool isVisible = true;

						if(GameObject.Find(mid_str_name).renderer.isVisible && isVisible)
//						if(GameObject.Find(mid_str_name).renderer.isVisible)
							//GUI.Button(new Rect(100,i*30,300,30),i + ":" + mid_str_name);
						{
							if(((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[j])[i])[2].Length > 3 )  //好像是遮挡物体字符串，一般用不到
							{
								//Array<string> mid_array;
								string [] mid_array  = 	((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[j])[i])[2].Split((",").ToCharArray());
								int k = 0;
								for(; k < mid_array.Length; k++)
								{
									if(GameObject.Find(mid_array[k]).renderer.material.color.a > 0.8f)
											break;
								}
								if(k ==  mid_array.Length)
								{
									doLableData mdoData = new doLableData();
									mdoData.strPartName = ((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[j])[i])[0];  //Excel表格第三列：部件中文名字
									Vector3 mid_vec = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find(mid_str_name).transform.position);
									mid_vec.y = Screen.height - mid_vec.y;
									mid_vec.z = 0;
									mdoData.vPosEnd = mid_vec;
									adjust_show_lable_lr(mdoData);
								}
							}
							else  //大概是没有遮挡物体的意思，一般都走这条路
							{
								doLableData mdoData = new doLableData();
								mdoData.strPartName = ((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[j])[i])[0];
								Vector3 mid_vec = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find(mid_str_name).transform.position);
								mid_vec.y = Screen.height - mid_vec.y;
								mid_vec.z = 0;
								mdoData.vPosEnd = mid_vec;
								adjust_show_lable_lr(mdoData);
							}
						}
					}
				}
			}
			else  //远距离所有都显示
			{
				for(int i = 0; i < m_read_Tree_xls.mmenu_top_array.Count; i++)
				{
					string mid_str_name = ((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[i])[0])[1]; //Excel表格第三列：物体名字

					//Wuhao
					bool isVisible = true;

					if(GameObject.Find(mid_str_name).renderer.isVisible && isVisible){
						doLableData mdoData = new doLableData();
						mdoData.strPartName = ((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[i])[0])[0];
//						Vector3 mid_vec = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find(((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[i])[0])[1]).transform.position);
						Vector3 mid_vec = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find(mid_str_name).transform.position);

						mid_vec.y = Screen.height - mid_vec.y;
						mid_vec.z = 0;
						mdoData.vPosEnd = mid_vec;
						adjust_show_lable_lr(mdoData);
					}
				}
				//for(int i=0;i < m_read_Tree_xls.mmenu_top_array.Count; i++)
				//{}
				//显示大结构,
				//show_BigPart_Lable();
			}

			//排序
			mySortCompare msort = new mySortCompare();
			array_pos_left.Sort(msort);
			array_pos_right.Sort(msort);
			
			//调节显示lable，上下.
			adjust_show_lable_ud();
			
			mySortCompare1 msort1 = new mySortCompare1();
			array_pos_lu.Sort(msort1);
			array_pos_ld.Sort(msort1);
			array_pos_ru.Sort(msort1);
			array_pos_rd.Sort(msort1);


			ftime += Time.deltaTime;
			//给array排序.
			if(ftime > 0.2f )
			{
				array_sort();
				//防止两个lable重叠;
				ajust_angle_sort();
				//防止线闯过另外一个lable
				//ajust_pos_line();
			}

			//显示标注;
			LabelDisplay();
		}

		if(FuncPara.outlineOn && showType){
			//显示标注;
			LabelDisplay();
		}


		GUI.skin = null;
	}
	
	void show_BigPart_Lable()
	{
		
		GUI.Button(new Rect(100,100,200,40),((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[0])[0])[0]);
		GUI.Button(new Rect(Screen.width - 300,100,200,40),((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[2])[0])[0]);
		GUI.Button(new Rect(100,Screen.height - 200,200,40),((string [])((ArrayList)m_read_Tree_xls.mmenu_top_array[3])[0])[0]);
		array_pos.Clear();
		//开合模部分
		doLableData mid_data = new doLableData();
		mid_data.vPosStart = new Vector3(300,140,0);
		mid_data.vPosEnd = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find("ZSJ_KHM_5").transform.position);
		mid_data.vPosEnd.y = Screen.height - mid_data.vPosEnd.y;
		mid_data.vPosEnd.z = 0;
		array_pos.Add(mid_data);
		//注射部分
		doLableData mid_data1 = new doLableData();
		mid_data1.vPosStart = new Vector3(Screen.width - 300,140,0);
		mid_data1.vPosEnd = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find("ZSJ_Inj_2").transform.position);
		mid_data1.vPosEnd.y = Screen.height - mid_data1.vPosEnd.y;
		mid_data1.vPosEnd.z = 0;
		array_pos.Add(mid_data1);
		//注射部分
		doLableData mid_data2 = new doLableData();
		mid_data2.vPosStart = new Vector3(300,Screen.height - 200,0);
		mid_data2.vPosEnd = GameObject.Find("Main Camera").camera.WorldToScreenPoint(GameObject.Find("ZSJ_JZ_68").transform.position);
		mid_data2.vPosEnd.y = Screen.height - mid_data2.vPosEnd.y;
		mid_data2.vPosEnd.z = 0;
		array_pos.Add(mid_data2);
	}
		
	/// <summary>
	/// 显示按钮型的标签
	/// </summary>
	void LabelDisplay()
	{
		for(int i = 0 ; i < array_pos_lu.Count; i++)
		{
			Rect rect_tmp = ((doLableData)(array_pos_lu[i])).rGUIRect;
			rect_tmp = new Rect(rect_tmp.x,rect_tmp.y,100,rect_tmp.height);
			
			calRectLenthsL(ref rect_tmp,((doLableData)(array_pos_lu[i])).strPartName);
			if(rect_tmp.width <= 70)
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_lu[i])).strPartName,mbtnsty1);
			}
			else
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_lu[i])).strPartName,mbtnsty2);
			}
			GUI.Button(new Rect(((doLableData)(array_pos_lu[i])).vPosEnd.x - 5,((doLableData)(array_pos_lu[i])).vPosEnd.y - 5,10,10),"",mGuisty_Point);
		}
		for(int i = 0 ; i < array_pos_ld.Count; i++)
		{
			Rect rect_tmp=((doLableData)(array_pos_ld[i])).rGUIRect;
			rect_tmp=new Rect(rect_tmp.x,rect_tmp.y,100,rect_tmp.height);
			calRectLenthsL(ref rect_tmp,((doLableData)(array_pos_ld[i])).strPartName);
			if(rect_tmp.width <= 70)
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_ld[i])).strPartName,mbtnsty1);
			}
			else
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_ld[i])).strPartName,mbtnsty2);
			}
			GUI.Button(new Rect(((doLableData)(array_pos_ld[i])).vPosEnd.x - 5,((doLableData)(array_pos_ld[i])).vPosEnd.y - 5,10,10),"",mGuisty_Point);
		}
		for(int i = 0 ; i < array_pos_ru.Count; i++)
		{
			Rect rect_tmp=((doLableData)(array_pos_ru[i])).rGUIRect;
			rect_tmp=new Rect(rect_tmp.x,rect_tmp.y,100,rect_tmp.height);
			calRectLenthsR(ref rect_tmp,((doLableData)(array_pos_ru[i])).strPartName);
			if(rect_tmp.width <= 70)
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_ru[i])).strPartName,mbtnsty1);
			}
			else
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_ru[i])).strPartName,mbtnsty2);
			}
			GUI.Button(new Rect(((doLableData)(array_pos_ru[i])).vPosEnd.x - 5,((doLableData)(array_pos_ru[i])).vPosEnd.y - 5,10,10),"",mGuisty_Point);
		
		}
		for(int i = 0 ; i < array_pos_rd.Count; i++)
		{
			Rect rect_tmp=((doLableData)(array_pos_rd[i])).rGUIRect;
			rect_tmp=new Rect(rect_tmp.x,rect_tmp.y,100,rect_tmp.height);
			calRectLenthsR(ref rect_tmp,((doLableData)(array_pos_rd[i])).strPartName);
			if(rect_tmp.width <= 70)
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_rd[i])).strPartName,mbtnsty1);
			}
			else
			{
				GUI.Button(rect_tmp,((doLableData)(array_pos_rd[i])).strPartName,mbtnsty2);
			}
			GUI.Button(new Rect(((doLableData)(array_pos_rd[i])).vPosEnd.x - 5,((doLableData)(array_pos_rd[i])).vPosEnd.y - 5,10,10),"",mGuisty_Point);
		}
	}
	
	void OnPostRender()
	{
		if(FuncPara.outlineOn){
			for(int i = 0 ; i < array_pos.Count; i++)
			{
				DrawStraightLine(((doLableData)array_pos[i]).vPosStart,((doLableData)array_pos[i]).vPosEnd);
			}
			for(int i = 0 ; i < array_pos_lu.Count; i++)
			{
				DrawStraightLine(((doLableData)(array_pos_lu[i])).vPosStart,((doLableData)(array_pos_lu[i])).vPosEnd);
			}
			for(int i = 0 ; i < array_pos_ld.Count; i++)
			{
				DrawStraightLine(((doLableData)(array_pos_ld[i])).vPosStart,((doLableData)(array_pos_ld[i])).vPosEnd);
			}
			for(int i = 0 ; i < array_pos_ru.Count; i++)
			{
				DrawStraightLine(((doLableData)(array_pos_ru[i])).vPosStart,((doLableData)(array_pos_ru[i])).vPosEnd);
			}
			for(int i = 0 ; i < array_pos_rd.Count; i++)
			{
				DrawStraightLine(((doLableData)(array_pos_rd[i])).vPosStart,((doLableData)(array_pos_rd[i])).vPosEnd);
			}
		}
	}
	
	/// <summary>
	/// 画直线
	/// </summary>
	/// 起始点
	/// Start_point.
	/// </param>
	/// <param name='end_point'>
	/// 结束点
	/// </param>
	void DrawStraightLine(Vector3 start_point, Vector3 end_point)
	{
		mmaterial.SetPass(0);
		GL.Color(new Color(0,0,0));
//		GL.Color(Color.red);
		start_point.x /= Screen.width;
		start_point.y = 1f - start_point.y / Screen.height;
		end_point.x /= Screen.width;
		end_point.y = 1f - end_point.y / Screen.height;
        GL.PushMatrix();
		GL.LoadOrtho();
//		GL.MultMatrix(GameObject.Find("lineref").transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Vertex(start_point);
        GL.Vertex(end_point);
        GL.End();
        GL.PopMatrix();
	}
	
	/// <summary>
	/// 调节屏幕上需要显示的标注的显示位置
	/// </summary>
	/// <param name='mid_doData'>
	/// Mid_do data.
	/// </param>
	void adjust_show_lable_lr(doLableData mid_doData)
	{

		//左半屏幕
		if(mid_doData.vPosEnd.x < Screen.width/2)
		{
			array_pos_left.Add(mid_doData);
			
		}
		else
		{
			array_pos_right.Add(mid_doData);
		}
	}
	
	/// <summary>
	/// 调节左右显示窗口显示到上下半个窗口.
	/// </summary>
	void adjust_show_lable_ud()
	{
		//左边窗口分上下窗口均匀显示.
		if(array_pos_left.Count > 0)		
		{
			for(int i = 0 ; i < array_pos_left.Count / 2; i++)
			{
				doLableData midData = (doLableData)array_pos_left[i];
				array_pos_lu.Add(midData);
			}
			for(int i = array_pos_left.Count/2; i < array_pos_left.Count; i++)
			{
				doLableData midData = (doLableData)array_pos_left[i];
				array_pos_ld.Add(midData);
			}
		}
		//右边窗口上下均匀显示.
		if(array_pos_right.Count > 0)
		{
			for(int i = 0 ; i < array_pos_right.Count/2; i++)
			{
				doLableData midData = (doLableData)array_pos_right[i];
				array_pos_ru.Add(midData);
			}
			for(int i = array_pos_right.Count/2; i < array_pos_right.Count; i++)
			{
				doLableData midData = (doLableData)array_pos_right[i];
				array_pos_rd.Add(midData);
			}
		}
	}
	
	/// <summary>
	/// 将所有的标注的位置排序
	/// </summary>
	void array_sort1()
	{
		for(int i = 0 ; i < array_pos_lu.Count ;i++)
		{
			((doLableData)(array_pos_lu[i])).rGUIRect = new Rect(10,30*(i+1),100,28);
			CalAngle(i,new Vector2(110,30*(i+1)+28),array_pos_lu);
		}
		
		for(int i = 0 ; i < array_pos_ld.Count ;i++)
		{
			((doLableData)(array_pos_ld[i])).rGUIRect = new Rect(10,Screen.height/2 + 30*(i+1),100,28);
			CalAngle(i,new Vector2(110,Screen.height/2 + 30*(i+1)+28),array_pos_ld);
		}
		
		for(int i = 0 ; i < array_pos_ru.Count ;i++)
		{
			((doLableData)(array_pos_ru[i])).rGUIRect = new Rect(Screen.width - 200,30*(i+1),100,28);
			CalAngle(i,new Vector2(Screen.width - 200,Screen.height/2 + 30*(i+1)+28),array_pos_ru);
		}
	}
	
	/// <summary>
	/// 将所有的标注的位置排序.
	/// </summary>
	void array_sort()
	{
		for(int i = 0 ; i < array_pos_lu.Count ;i++)
		{
			//左上部分，计算其相应的显示位置.
			Vector2 mid_2vec = new Vector2(0,0);
			float mid_y = Screen.height/2 - 50 - ((doLableData)(array_pos_lu[i])).vPosEnd.y;
			//float mid_angle = (float)(Math.PI * i / array_pos_lu.Count / 2);
			//float fangle = (float)(Math.PI / 7 );
			mid_2vec.x =  (float)(((doLableData)(array_pos_lu[i])).vPosEnd.x - Math.Abs(mid_y) * 2 * Math.Tan(fangle));
			if(mid_y > 0)
			{
				mid_2vec.y = Screen.height/2 - 50 - 3*mid_y;
			}
			else
			{
				mid_2vec.y = Screen.height/2 - 50 + mid_y;
			}	
			//调整角度,如果x<0;则沿着线方向，调整
			while(mid_2vec.x < 0)
			{
				mid_2vec.y += (float)(1/Math.Tan(fangle) * 100);
				mid_2vec.x += 100;
			}
			((doLableData)(array_pos_lu[i])).vPosStart = new Vector3(mid_2vec.x,mid_2vec.y,0);
			((doLableData)(array_pos_lu[i])).rGUIRect = new Rect(mid_2vec.x - 100,mid_2vec.y - 30,100,28);
			//CalAngle(i,new Vector2(110,30*(i+1)+28),array_pos_lu);
		}
		
		for(int i = 0 ; i < array_pos_ld.Count ;i++)
		{
			//左上部分，计算其相应的显示位置.
			Vector2 mid_2vec = new Vector2(0,0);
			float mid_y = Math.Abs(Screen.height - ((doLableData)(array_pos_ld[i])).vPosEnd.y)/2;
			//float mid_angle = (float)(Math.PI * i / array_pos_ld.Count / 2);
			//float mid_angle = (float)(Math.PI / 7 );
			mid_2vec.x =  (float)(((doLableData)(array_pos_ld[i])).vPosEnd.x - mid_y * Math.Tan(fangle));
			mid_2vec.y = Screen.height - mid_y;
			//调整角度,如果x<0;则沿着线方向，调整
			while(mid_2vec.x < 0)
			{
				mid_2vec.y += (float)(1/Math.Tan(fangle) * 100);
				mid_2vec.x += 100;
			}
			((doLableData)(array_pos_ld[i])).vPosStart = new Vector3(mid_2vec.x,mid_2vec.y,0);
			((doLableData)(array_pos_ld[i])).rGUIRect = new Rect(mid_2vec.x - 100,mid_2vec.y,100,28);
			
		}
		
		for(int i = 0 ; i < array_pos_ru.Count ;i++)
		{
			//左上部分，计算其相应的显示位置.
			Vector2 mid_2vec = new Vector2(0,0);
			float mid_y = Screen.height/2 - 50 - ((doLableData)(array_pos_ru[i])).vPosEnd.y;
			//float mid_angle = (float)(Math.PI * i / array_pos_ru.Count / 2);
			//float fangle = (float)(Math.PI / 7 );
			mid_2vec.x =  (float)(((doLableData)(array_pos_ru[i])).vPosEnd.x + Math.Abs(mid_y) * 2 * Math.Tan(fangle));
			if(mid_y > 0)
			{
				mid_2vec.y = Screen.height/2 - 50 - 3 * mid_y;
			}
			else
			{
				mid_2vec.y = Screen.height/2 - 50 + mid_y;
			}
			//调整角度,如果x<0;则沿着线方向，调整
			while(mid_2vec.x > Screen.width)
			{
				mid_2vec.y += (float)(1/Math.Tan(fangle) * 100);
				mid_2vec.x -= 100;
			}
			((doLableData)(array_pos_ru[i])).vPosStart = new Vector3(mid_2vec.x,mid_2vec.y,0);
			((doLableData)(array_pos_ru[i])).rGUIRect = new Rect(mid_2vec.x,mid_2vec.y - 30,100,28);
			//CalAngle(i,new Vector2(110,30*(i+1)+28),array_pos_ru);
		}
		
		for(int i = 0 ; i < array_pos_rd.Count ;i++)
		{
			//左上部分，计算其相应的显示位置.
			Vector2 mid_2vec = new Vector2(0,0);
			float mid_y = Math.Abs(Screen.height - ((doLableData)(array_pos_rd[i])).vPosEnd.y)/2;
			//float mid_angle = (float)(Math.PI * i / array_pos_rd.Count / 2);
			//float mid_angle = (float)(Math.PI / 7 );
			mid_2vec.x =  (float)(((doLableData)(array_pos_rd[i])).vPosEnd.x + mid_y * Math.Tan(fangle));
			mid_2vec.y = Screen.height - mid_y;
			//调整角度,如果x<0;则沿着线方向，调整
			while(mid_2vec.x > Screen.width)
			{
				mid_2vec.y += (float)(1/Math.Tan(fangle) * 100);
				mid_2vec.x -= 100;
			}
			((doLableData)(array_pos_rd[i])).vPosStart = new Vector3(mid_2vec.x,mid_2vec.y,0);
			((doLableData)(array_pos_rd[i])).rGUIRect = new Rect(mid_2vec.x,mid_2vec.y,100,28);
			
		}
	}
	
	/// <summary>
	/// 调节各个框之间不重叠.
	/// </summary>
	void  ajust_angle_sort()
	{
		for(int i = 0 ; i < array_pos_lu.Count; i++)
		{
			//Vector2 mid_2vec = new Vector2(((doLableData)(array_pos_lu[i])).vPosStart.x,((doLableData)(array_pos_lu[i])).vPosStart.y);
			while(check_cross(i,array_pos_lu))
			{
				((doLableData)(array_pos_lu[i])).vPosStart.x -= (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_lu[i])).vPosStart.y -= 30;
				((doLableData)(array_pos_lu[i])).rGUIRect.x -= (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_lu[i])).rGUIRect.y -= 30;
			}
		}
		for(int i = 0 ; i < array_pos_ld.Count; i++)
		{
			//Vector2 mid_2vec = new Vector2(((doLableData)(array_pos_lu[i])).vPosStart.x,((doLableData)(array_pos_lu[i])).vPosStart.y);
			while(check_cross(i,array_pos_ld))
			{
				((doLableData)(array_pos_ld[i])).vPosStart.x -= (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_ld[i])).vPosStart.y += 30;
				((doLableData)(array_pos_ld[i])).rGUIRect.x -= (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_ld[i])).rGUIRect.y += 30;
			}
		}
		
		for(int i = 0 ; i < array_pos_ru.Count; i++)
		{
			//Vector2 mid_2vec = new Vector2(((doLableData)(array_pos_lu[i])).vPosStart.x,((doLableData)(array_pos_lu[i])).vPosStart.y);
			while(check_cross(i,array_pos_ru))
			{
				((doLableData)(array_pos_ru[i])).vPosStart.x += (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_ru[i])).vPosStart.y -= 30;
				((doLableData)(array_pos_ru[i])).rGUIRect.x += (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_ru[i])).rGUIRect.y -= 30;
			}
		}
		for(int i = 0 ; i < array_pos_rd.Count; i++)
		{
			//Vector2 mid_2vec = new Vector2(((doLableData)(array_pos_lu[i])).vPosStart.x,((doLableData)(array_pos_lu[i])).vPosStart.y);
			while(check_cross(i,array_pos_rd))
			{
				((doLableData)(array_pos_rd[i])).vPosStart.x += (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_rd[i])).vPosStart.y += 30;
				((doLableData)(array_pos_rd[i])).rGUIRect.x += (float)(Math.Tan(fangle) * 30);
				((doLableData)(array_pos_rd[i])).rGUIRect.y += 30;
			}
		}
	}
	
	/// <summary>
	/// 防止线穿过按钮.
	/// </summary>
	void ajust_pos_line()
	{
		for(int i = 0 ; i < array_pos_lu.Count; i++)
		{
			//检查线是否经过线框;
			int num = check_cross_line_lu(i,array_pos_lu);
			if(num != -1)
			{
				//交换数据.
				float y = ((doLableData)(array_pos_lu[num])).vPosStart.y - ((doLableData)(array_pos_lu[i])).vPosStart.y;
				float x = (float)(Math.Tan(fangle) * y);
				((doLableData)(array_pos_lu[i])).vPosStart.x += x;
				((doLableData)(array_pos_lu[i])).vPosStart.y += y;
				((doLableData)(array_pos_lu[i])).rGUIRect.x += x;
				((doLableData)(array_pos_lu[i])).rGUIRect.y += y;
				((doLableData)(array_pos_lu[num])).vPosStart.x -= x;
				((doLableData)(array_pos_lu[num])).vPosStart.y -= y;
				((doLableData)(array_pos_lu[num])).rGUIRect.x -= x;
				((doLableData)(array_pos_lu[num])).rGUIRect.y -= y;
				//i = 0;
			}
			
		}
	}
	
	/// <summary>
	/// 求直线是否经过线框，线框对角线作为代表判定.
	/// </summary>
	/// <param name='mid_num'>
	/// 进行到的个数
	/// </param>
	/// <param name='mid_array'>
	/// Mid_array.
	/// </param>
	int check_cross_line_lu(int mid_num, ArrayList mid_array)
	{
		for(int i = 0 ; i < mid_array.Count; i++)
		{
			Vector2 pos1 = new Vector2(((doLableData)(mid_array[mid_num])).vPosStart.x,((doLableData)(mid_array[mid_num])).vPosStart.y);
			Vector2 pos2 = new Vector2(((doLableData)(mid_array[mid_num])).vPosEnd.x,((doLableData)(mid_array[mid_num])).vPosEnd.y);
			Vector2 pos3 = new Vector2(((doLableData)(mid_array[i])).rGUIRect.x,((doLableData)(mid_array[i])).rGUIRect.y + 28);
			Vector2 pos4 = new Vector2(((doLableData)(mid_array[i])).rGUIRect.x + 100,((doLableData)(mid_array[i])).rGUIRect.y);
			
			float a1 = (pos1.y - pos2.y)/(pos1.x - pos2.x);
			float b1 = (pos1.x*pos2.y - pos2.x * pos1.y)/(pos1.x - pos2.x);
			float a2 = (pos3.y - pos4.y)/(pos3.x - pos4.x);
			float b2 = (pos3.x*pos4.y - pos4.x * pos3.y)/(pos3.x - pos4.x);
			
			//交点
			if(a1-a2 != 0)
			{
				float x0 = (b2-b1)/(a1 - a2);
				float y0 = (a1*b2 - a2*b1)/(a1 - a2);
				//判断交点是否在线段内部.
				if((pos1.x - x0)*(pos2.x - x0) < 0 && (pos1.y - y0)*(pos2.y - y0) < 0
					&& (pos3.x - x0)*(pos4.x - x0) < 0 && (pos3.y - y0) * (pos4.y - y0) < 0)
				{
					//num = i;
					return i;
				}
			}
		}
		return -1;
	}
	
	/// <summary>
	/// 判断lable位置是否相交
	/// </summary>
	/// <param name='mid_num'>
	/// 判断进行的程度
	/// </param>
	/// <param name='mid_array'>
	/// 要进行判断的字符集.
	/// </param>
	bool check_cross(int mid_num, ArrayList mid_array)
	{
		for(int i = 0 ; i < mid_num; i++)
		{
			if(Math.Abs(((doLableData)(mid_array[i])).vPosStart.x - ((doLableData)(mid_array[mid_num])).vPosStart.x ) < 100
				&& Math.Abs(((doLableData)(mid_array[i])).vPosStart.y - ((doLableData)(mid_array[mid_num])).vPosStart.y) < 30)
			{
				return true;
			}
		}
		return false;
	}
	
	void ajust_pos(int type, float angle,Vector2 mid_2vec)
	{
		//位置下降.
		if(type == (int)moveType.toRiht)
		{
			mid_2vec.y = (float)(mid_2vec.y - Math.Tanh(angle) * 100);
			mid_2vec.x += 100;
			
		}
		else if(type == (int)moveType.toLeft)
		{
			mid_2vec.y += (float)(Math.Tanh(angle) * 100);
			mid_2vec.x -= 100;
		}
		else if(type == (int)moveType.toDown)
		{
			mid_2vec.x -= (float)(Math.Tan(angle) * 30);
			mid_2vec.y += 30;
		}
		else if(type == (int)moveType.toUp)
		{
			mid_2vec.x -= (float)moveType.toUp;
			mid_2vec.y -= 30;
		}
	}
	/// <summary>
	/// 计算角度，获取对应角度最小的点
	/// </summary>
	/// <param name='i'>
	/// 已经计算到第几个
	/// </param>
	/// <param name='mid_vec2'>
	/// 所在点
	/// </param>
	/// <param name='mid_array'>
	/// 系列点
	/// </param>
	void CalAngle(int mid_num,Vector2 mid_vec2,ArrayList mid_array)
	{
		if(mid_array.Count > mid_num)
		{
			float fangle_pre = -180;
			int	  mark_num = -1;
			for(int i = mid_num; i < mid_array.Count; i ++)
			{
				//内积公式倒推，求出角度。
				Vector2 vec1 = new Vector2(0,1);
				Vector2 vec2 = new Vector2(mid_vec2.x - ((doLableData)(mid_array[i])).vPosEnd.x,((doLableData)(mid_array[i])).vPosEnd.y - mid_vec2.y);
				float	fangle = (float)((vec1.x*vec2.x + vec1.y*vec2.y)/(System.Math.Sqrt(vec1.x*vec1.x+vec1.y+vec1.y) + System.Math.Sqrt(vec2.x*vec2.x + vec2.y*vec2.y)));
				fangle = (float)System.Math.Acos(fangle);
				//如果角度大于之前的，则记录，用于最后交换位置用.
				if(fangle > fangle_pre)
				{
					fangle_pre = fangle;
					mark_num = i;
				}
			}
			//交换当前行数与lable与最小角度的lable数据.
			if(mark_num != -1)
			{
				doLableData midData = (doLableData)mid_array[mid_num];
				doLableData midData2 = (doLableData)mid_array[mark_num];
				mid_array.RemoveAt(mid_num);
				mid_array.Insert(mid_num,midData2);
				mid_array.RemoveAt(mark_num);
				mid_array.Insert(mark_num,midData);
			}
		}
	}
	
	//计算左部所有显示名字的长度
	void calRectLenthsL(ref Rect mid_rect, string mid_Str)
	{
		if(mid_Str.Length <= 4)
		{
			mid_rect.width = 70;
		}
		else
		{
			mid_rect.width = mid_Str.Length * 13 + 20;
		}
		mid_rect.x += (100 - mid_rect.width);
	}
	
	//计算右部所有显示名字的长度
	void calRectLenthsR(ref Rect mid_rect, string mid_Str)
	{
		if(mid_Str.Length <= 4)
		{
			mid_rect.width = 70;
		}
		else
		{
			mid_rect.width = mid_Str.Length * 13 + 20;
		}
	}
}




//此处仅仅用于排序，故不单独作为一个类.
class mySortCompare:System.Collections.IComparer
{
	public int Compare(object a,object b)
	{
		return (int)((((doLableData)a).vPosEnd.y - ((doLableData)b).vPosEnd.y) * 1000); 
	}
}

//此处仅仅用于排序，故不单独作为一个类.
class mySortCompare1:System.Collections.IComparer
{
	public int Compare(object a,object b)
	{
		return (int)(-(((doLableData)a).vPosEnd.x - ((doLableData)b).vPosEnd.x) * 1000); 
	}
}

