using UnityEngine;
using System.Collections;

using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Collections.Generic;

public class TreeStructure : MonoBehaviour {

	//控制显示
	public static bool show;
	//窗口
	Rect tree_rect;
	//贴图
	public Texture2D tex_close;
	public Texture2D tex_open;
	public Texture2D tex_icon1;
	public Texture2D tex_icon2;

	//层级
	public int[] level;
	//物体
	public string[] objs;
	//文本信息
	public string[] txts;
	//计数:count--总计数，enable_count--显示的计数
	public int count, enable_count;
	//是否显示
	public bool[] enable;
	//是否展开
	public bool[] open_close;

	bool[] fake_enable;
	
	//是否展开的图标
	public Texture2D[] tex_open_close;
	//系统或部件的图标
	public Texture2D[] tex_component;
	
	
	//当前鼠标移动到的物体的索引
	int index;
	//是否选中
	bool over_in;
	
	//GUIStyle
	public GUIStyle guistyle_label;
	public GUIStyle guistyle_close;
	
	//滚动条
	private Vector2 scrollViewVector = Vector2.zero;

	//材质变换
	public Material mat_red;
	List<string> selected_Objs = new List<string>();
	List<Material> selected_Mats = new List<Material>();


	void Start () {
		tree_rect = new Rect(200, 100, 220, 480);
		
		
		initial("testEquipment");
	}
	
	public void initial(string equipmentName){
		//测试初始化--待修改
		
		ExcelOperator excelReader = new ExcelOperator();
		DataTable dataTable= excelReader.ExcelReader(Application.dataPath + "/Resources/Cognitive_Structure/" + MotionPara.deviceName+ "/down.xls", "a", "A", "C");
		
		count = dataTable.Rows.Count;
		
//		count = 7;
//		
//		level = new int[]{0,1,1,2,2,1,1};
//		objs = new string[]{"", "Cone001", "", "Tube001", "Box001", "Teapot001", "Teapot002"};
//		txts = new string[]{"测试仪器", "子系统1", "子系统2", "子部件1", "子部件2", "子系统3", "子系统4"};
		
		//未选择部件
		index = -1;
		
		//初始化
		level = new int[count];
		objs = new string[count];
		txts = new string[count];
		
		enable = new bool[count];
		open_close = new bool[count];

		fake_enable = new bool[count];
		
		tex_open_close = new Texture2D[count];
		tex_component = new Texture2D[count];
		
		for(int i=0; i<count ;i++){
			tex_open_close[i] = tex_close;
			tex_component[i] = tex_icon2;
			
			level[i] = int.Parse(dataTable.Rows[i][0].ToString());
			objs[i] = dataTable.Rows[i][1].ToString();
			txts[i] = dataTable.Rows[i][2].ToString();
			
			enable[i] = false;
			open_close[i] = false;

			fake_enable[i] = false;
		}
		
		//只显示总系统
		enable[0] = true;
		//当前只显示一个部件
		enable_count = 1;

		fake_enable[0] = true;
	}

	void TreeWindow(int WindowID){
		
		//鼠标位置
		float mouse_x = Event.current.mousePosition.x;
		float mouse_y = Event.current.mousePosition.y;

//		Debug.Log(mouse_x+","+mouse_y);

		//关闭按钮
		if(GUI.Button(new Rect(196, 0, 24, 24), "", guistyle_close)){
			TreeStructure.show = false;
		}
		
		//Window位置调整
		if(tree_rect.x < 0){
			tree_rect.x = 0;
		}
		if(tree_rect.y < 0){
			tree_rect.y = 0;
		}
		if(tree_rect.x > (Screen.width - tree_rect.width)){
			tree_rect.x = (Screen.width - tree_rect.width);
		}
		if(tree_rect.y > (Screen.height - tree_rect.height)){
			tree_rect.y = (Screen.height - tree_rect.height);
		}
		
		//滚动条
		scrollViewVector = GUI.BeginScrollView (new Rect (10, 25, 200, 440), scrollViewVector, new Rect (0, 0, 180, 20*enable_count));
//		GUI.Box(new Rect(0,0,200,count*20),"");

		int j = 0;
		over_in = false;
		for(int i=0; i<count; i++){
			if(enable[i]){
				j+=1;
				if(objs[i] == ""){//部件名为空,当前部件为系统或子系统
					tex_component[i] = tex_icon1;
				}
				
				//显示部件名,显示部件和系统的图标
				if(GUI.Button(new Rect(level[i]*10+20, j*20-20, 200, 20), new GUIContent(txts[i], tex_component[i]), guistyle_label)){
					//Wait for Add
					
				}

				mouse_x = Event.current.mousePosition.x;
				mouse_y = Event.current.mousePosition.y;
				
				//鼠标移动来改变部件选择
				if((new Rect(level[i]*10+20, j*20-20, 160-level[i]*10, 20)).Contains(new Vector2(mouse_x, mouse_y))){
					over_in = true;
					
//					if(index != (i-1)){
//						index = (i-1);
//						//Wait for Add
//						selectedChanged();
//					}

					Debug.Log("i:"+i);

					if(index != i){
						index = i;
						//Wait for Add
						selectedChanged();
					}
				}
				
				//显示是否展开的图标
				if( (i < count -1) && (level[i] < level[i+1])){//当前图标有下一层：0-1,1-2
					if(GUI.Button(new Rect (level[i]*10, j*20-20, 20, 20), tex_open_close[i], guistyle_label)){
						if(enable[i+1] == true){//当前图标的下一层也显示
							open_close[i] = false;
							
							tex_open_close[i] = tex_close;
						}
						else{
							open_close[i] = true;
							
							tex_open_close[i] = tex_open;
						}
						
						//close
						if(!open_close[i]){
							for(int m=i; m<count-1; m++){
								if(level[m+1] <= level[i]){//如果碰到与当前图标同级或更高级，跳出
									break;
								}
								else{//当前图标的子部件
									if(enable[m+1] == true){
										enable[m+1] = false; 	//不显示子部件
										enable_count -= 1;		//显示部件数减少
										
										//Wait for Add
//										if(open_close[m+1] == true){
//											open_close[m+1] = false;
//							
//											tex_open_close[m+1] = tex_close;
//										}
										if(level[m+1] == level[i] + 1){
											fake_enable[m+1] = false;
										}
									}
								}
								
							}
						}
						//open
						else{
							int now_level = level[i]+1;
							for(int m=i; m<count-1; m++){
								if(level[m+1] <= level[i]){//如果碰到与当前图标同级或更高级，跳出
									break;
								}
								
								if(level[m+1] == level[i]+1){//如果是当前图标的下一级别部件
									enable[m+1] = true;		//不显示子部件
									enable_count += 1;		//显示部件数增加
										
									//Wait for Add
									fake_enable[m+1] = true;
									now_level=level[i]+2;
								}

								//Wait for Add
								if(level[m+1]<=now_level && level[m+1]>level[i]+1){
									if(fake_enable[m+1]){
										enable[m+1] = true;
										enable_count+=1;
										now_level=level[m+1]+1;
									}
								}
							}
						}
					}
				}
			}
		}
		
		
		if(over_in == false){
			
			//Wait for Add
			leave();

			index = -1;
		}
		
		GUI.EndScrollView();
		
		GUI.DragWindow();
	}

	//鼠标移动到不同部件上
	void selectedChanged(){

		Debug.Log("selectedChanged:"+ objs[index]+ ",index:"+ index);

		//材质恢复
		for(int i=0; i<selected_Objs.Count; i++){
			GameObject.Find(selected_Objs[i]).renderer.material = selected_Mats[i];
		}
		//清空
		selected_Objs.Clear();
		selected_Mats.Clear();

		if(objs[index] != ""){
			selected_Objs.Add(objs[index]);
			selected_Mats.Add(GameObject.Find(objs[index]).renderer.material);
		}

		for(int m=index; m<count-1; m++){
			if(level[index] < level[m+1]){	//如果都是该图标的子集
				if(objs[m+1] != ""){
					selected_Objs.Add(objs[m+1]);
					selected_Mats.Add(GameObject.Find(objs[m+1]).renderer.material);
				}
			}
			else{
				break;						//如果都是该图标的同级或上级，跳出
			}
		}

		for(int i=0; i<selected_Objs.Count; i++){
			GameObject.Find(selected_Objs[i]).renderer.material = mat_red;

		}

		Debug.Log("Count:"+ selected_Objs.Count);
	}

	void leave(){
		for(int i=0; i<selected_Objs.Count; i++){
			GameObject.Find(selected_Objs[i]).renderer.material = selected_Mats[i];
		}
		//清空
		selected_Objs.Clear();
		selected_Mats.Clear();
	}

	void OnGUI(){
		
		GUI.skin = FuncPara.defaultSkin;
		
		if(show){
			tree_rect = GUI.Window(1114, tree_rect, TreeWindow, "");
		}
	}

	void Update () {
//		Debug.Log("index:"+ index);
	}
}
