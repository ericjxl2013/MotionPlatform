using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class PositionAndRotationPrint : EditorWindow {
	string PositionString1st = "1st Position of Selection";
	
	Vector3 Position1st = new Vector3(0,0,0);
	
	string PositionString2nd = "2nd Position of Selection";
	
	Vector3 Position2nd = new Vector3(0,0,0);
	
	string PosDiff = "Position Diff From 2nd To 1st";
	
	Vector3 PosDiffValue = new Vector3(0,0,0);
	
	string RotationString1st = "1st Rotation of Selection";
	
	Vector3 Rotation1st = new Vector3(0,0,0);
	
	string RotationString2nd = "2nd Rotation of Selection";
	
	Vector3 Rotation2nd = new Vector3(0,0,0);
	
	string RotDiff = "Rotation Diff From 2nd To 1st";
	
	Vector3 RotDiffValue = new Vector3(0,0,0);
	
	Transform SelectedTransform;
		

	List<Vector3> RecordData = new List<Vector3>();
	List<Vector3> RecordData2 = new List<Vector3>();
	List<Vector3> RecordData3 = new List<Vector3>();
	List<Vector3> RecordData4 = new List<Vector3>();
	string ListIndex = "";
	string ListData = "";
	string ListData2 = "";
	string ListData3 = "";
	string ListData4 = "";

	// Add menu named "My Window" to the Window menu
	[MenuItem("Tools/运动平台/获得选中物体信息")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		PositionAndRotationPrint window = (PositionAndRotationPrint)EditorWindow.GetWindow (typeof (PositionAndRotationPrint));
		
		window.Show();

	}
	
	
	Vector3 GetPosition () {
				
		SelectedTransform = Selection.activeTransform;
		
		return SelectedTransform.localPosition;

	}
	
	Vector3 GetRotation () {
				
		SelectedTransform = Selection.activeTransform;
		
		return SelectedTransform.localEulerAngles;

	}
	
	Vector3 GetPosDiff () {
		
		return Position2nd - Position1st;
	}
	
	Vector3 GetRotDiff () {
		
		return Rotation2nd - Rotation1st;
	}
	
	void OnGUI () {
		
		GUILayout.Label ("位置信息:", EditorStyles.boldLabel);
		
		PositionString1st = EditorGUILayout.TextField ("Position 1st:", PositionString1st);
		
		if(GUILayout.Button("获得物体的第1个位置"))
		{
			Position1st = GetPosition();
			
			if(Position1st.x > -0.000001F && Position1st.x < 0.000001F)
				Position1st.x = 0;
			
			if(Position1st.y > -0.000001F && Position1st.y < 0.000001F)
				Position1st.y = 0;
			
			if(Position1st.z > -0.000001F && Position1st.z < 0.000001F)
				Position1st.z = 0;
			
		    PositionString1st = (string)Convert.ToString(Position1st);
		    this.Repaint();
			Debug.Log(Position1st.x+","+Position1st.y+","+Position1st.z + "  1st Pos");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Position1st.x+","+
			                     Position1st.y+","+
			                     Position1st.z);
		}
		
		PositionString2nd = EditorGUILayout.TextField ("Position 2st:", PositionString2nd);
		
		if(GUILayout.Button("获得物体的第2个位置"))
		{
			Position2nd = GetPosition();
			
			if(Position2nd.x > -0.000001F && Position2nd.x < 0.000001F)
				Position2nd.x = 0;
			
			if(Position2nd.y > -0.000001F && Position2nd.y < 0.000001F)
				Position2nd.y = 0;
			
			if(Position2nd.z > -0.000001F && Position2nd.z < 0.000001F)
				Position2nd.z = 0;
			
			PositionString2nd = (string)Convert.ToString(Position2nd);
			this.Repaint();
			Debug.Log(Position2nd.x+","+Position2nd.y+","+Position2nd.z + "  2nd Pos");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Position2nd.x+","+
			                     Position2nd.y+","+
			                     Position2nd.z);
		}
		
		PosDiff = EditorGUILayout.TextField ("Pos Diff:", PosDiff);
		
		if(GUILayout.Button("获得物体两个位置之间的位移"))
		{
		    PosDiffValue = GetPosDiff();
			
			if(PosDiffValue.x > -0.000001F && PosDiffValue.x < 0.000001F)
				PosDiffValue.x = 0;
			
			if(PosDiffValue.y > -0.000001F && PosDiffValue.y < 0.000001F)
				PosDiffValue.y = 0;
			
			if(PosDiffValue.z > -0.000001F && PosDiffValue.z < 0.000001F)
				PosDiffValue.z = 0;
			
			PosDiff = (string)Convert.ToString(PosDiffValue);
			this.Repaint();
			Debug.Log(PosDiffValue.x+","+PosDiffValue.y+","+PosDiffValue.z + "  Pos Diff");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(PosDiffValue.x+","+
			                     PosDiffValue.y+","+
			                     PosDiffValue.z);
			
		}
		
		GUILayout.Label ("角度信息:", EditorStyles.boldLabel);
		
		RotationString1st = EditorGUILayout.TextField ("Position 1st:", RotationString1st);
		
		if(GUILayout.Button("获得物体的第1个角度"))
		{
			Rotation1st = GetRotation();
			
			if(Rotation1st.x > -0.000001F && Rotation1st.x < 0.000001F)
				Rotation1st.x = 0;
			
			if(Rotation1st.y > -0.000001F && Rotation1st.y < 0.000001F)
				Rotation1st.y = 0;
			
			if(Rotation1st.z > -0.000001F && Rotation1st.z < 0.000001F)
				Rotation1st.z = 0;
			
			RotationString1st = (string)Convert.ToString(Rotation1st);
		    this.Repaint();
     		Debug.Log(Rotation1st.x+","+Rotation1st.y+","+Rotation1st.z+ "  1st Rot");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Rotation1st.x+","+
			                     Rotation1st.y+","+
			                     Rotation1st.z);
		}
		
		RotationString2nd = EditorGUILayout.TextField ("Position 2st:", RotationString2nd);
		
		if(GUILayout.Button("获得物体的第2个角度"))
		{
			Rotation2nd = GetRotation();
			
			if(Rotation2nd.x > -0.000001F && Rotation2nd.x < 0.000001F)
				Rotation2nd.x = 0;
			
			if(Rotation2nd.y > -0.000001F && Rotation2nd.y < 0.000001F)
				Rotation2nd.y = 0;
			
			if(Rotation2nd.z > -0.000001F && Rotation2nd.z < 0.000001F)
				Rotation2nd.z = 0;
			
			RotationString2nd = (string)Convert.ToString(Rotation2nd);
			this.Repaint();
			Debug.Log(Rotation2nd.x+","+Rotation2nd.y+","+Rotation2nd.z + "  2nd Rot");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Rotation2nd.x+","+
			                     Rotation2nd.y+","+
			                     Rotation2nd.z);
		}
		
		RotDiff = EditorGUILayout.TextField ("Rot Diff:", RotDiff);
		
		if(GUILayout.Button("获得物体两个角度的角度差"))
		{
			RotDiffValue = GetRotDiff();
			RotDiff = (string)Convert.ToString(RotDiffValue);
			this.Repaint();
			Debug.Log(RotDiffValue.x+","+RotDiffValue.y+","+RotDiffValue.z + "  Rot Diff");

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(RotDiffValue.x+","+
			                     RotDiffValue.y+","+
			                     RotDiffValue.z);
		}
		
		GUILayout.Label ("Reset Position or Rotation:", EditorStyles.boldLabel);
		if(GUILayout.Button("重置位置信息"))
		{
			SelectedTransform.localPosition = Vector3.zero;
		}
		
		if(GUILayout.Button("重置角度信息"))
		{
			SelectedTransform.localEulerAngles = Vector3.zero;
		}
		
		GUILayout.Label ("Get Absolute Position or Rotation:", EditorStyles.boldLabel);
		if(GUILayout.Button("获得当前Gameobject绝对位置"))
		{
			Debug.Log(Selection.activeTransform.position.x + "," + Selection.activeTransform.position.y + "," + Selection.activeTransform.position.z);

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Selection.activeTransform.position.x+","+
			                     Selection.activeTransform.position.y+","+
			                     Selection.activeTransform.position.z);
		}
		
		if(GUILayout.Button("获得当前Gameobject绝对角度"))
		{
			Debug.Log(Selection.activeTransform.eulerAngles.x + "," + Selection.activeTransform.eulerAngles.y + "," + Selection.activeTransform.eulerAngles.z);

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Selection.activeTransform.eulerAngles.x+","+
			                     Selection.activeTransform.eulerAngles.y+","+
			                     Selection.activeTransform.eulerAngles.z);
		}
		if(GUILayout.Button("获得当前Gameobject名称"))
		{
			Debug.Log(Selection.activeTransform.eulerAngles.x + "," + Selection.activeTransform.eulerAngles.y + "," + Selection.activeTransform.eulerAngles.z);
		
			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(Selection.activeGameObject.name);
		}

		if(GUILayout.Button("获得当前选中多个Gameobject名称"))
		{

			Transform[] selections = Selection.GetTransforms(SelectionMode.Unfiltered);
			string tmp = selections[0].name;
			for(int i=1; i<selections.Length; i++){
				tmp += (","+selections[i].name);
			}

			Debug.Log(selections.Length+":"+tmp);

			MotionEditor excel = GameObject.Find("MainScript").GetComponent<MotionEditor>();
			excel.getInputString(tmp);
		}
	
		if(GUILayout.Button("采集数据"))
		{
			RecordData.Add(new Vector3(Selection.activeTransform.position.x,
			                           Selection.activeTransform.position.y,
			                           Selection.activeTransform.position.z)
			               );
			RecordData2.Add(new Vector3(Selection.activeTransform.localPosition.x,
			                           Selection.activeTransform.localPosition.y,
			                           Selection.activeTransform.localPosition.z)
			               );
			RecordData3.Add(new Vector3(Selection.activeTransform.eulerAngles.x,
			                            Selection.activeTransform.eulerAngles.y,
			                            Selection.activeTransform.eulerAngles.z)
			                );
			RecordData4.Add(new Vector3(Selection.activeTransform.localEulerAngles.x,
			                            Selection.activeTransform.localEulerAngles.y,
			                            Selection.activeTransform.localEulerAngles.z)
			                );
			Debug.Log(RecordData.Count+",采集数据绝对坐标:"+Selection.activeTransform.position.x + "," + Selection.activeTransform.position.y + "," + Selection.activeTransform.position.z);
			Debug.Log(RecordData.Count+",采集数据相对坐标:"+Selection.activeTransform.localPosition.x + "," + Selection.activeTransform.localPosition.y + "," + Selection.activeTransform.localPosition.z);
			Debug.Log(RecordData.Count+",采集数据绝对角度:"+Selection.activeTransform.eulerAngles.x + "," + Selection.activeTransform.eulerAngles.y + "," + Selection.activeTransform.eulerAngles.z);
			Debug.Log(RecordData.Count+",采集数据相对角度:"+Selection.activeTransform.localEulerAngles.x + "," + Selection.activeTransform.localEulerAngles.y + "," + Selection.activeTransform.localEulerAngles.z);

		}

		ListIndex = EditorGUILayout.TextField ("数据索引:", ListIndex);
		if(GUILayout.Button("查询"))
		{
			int result=0;
			if(int.TryParse(ListIndex, out result)){
				if(result >= 0 && result < RecordData.Count){
					ListData = 
							RecordData[result].x.ToString()+","+
							RecordData[result].y.ToString()+","+
							RecordData[result].z.ToString();
					ListData2 = 
							RecordData2[result].x.ToString()+","+
							RecordData2[result].y.ToString()+","+
							RecordData2[result].z.ToString();
					ListData3 = 
							RecordData3[result].x.ToString()+","+
							RecordData3[result].y.ToString()+","+
							RecordData3[result].z.ToString();
					ListData4 = 
							RecordData4[result].x.ToString()+","+
							RecordData4[result].y.ToString()+","+
							RecordData4[result].z.ToString();
					Debug.Log("查询数据ID:"+ result+ ","+ ListData+","+ListData2+","+ListData3+","+ListData4);
				}
				else{
					Debug.LogError("数据索引超出范围");
				}
			}else{
				Debug.LogError("数据索引无效");
			}
		}
		ListData = EditorGUILayout.TextField ("查询到的绝对坐标:", ListData);
		ListData2 = EditorGUILayout.TextField ("查询到的相对坐标:", ListData2);
		ListData3 = EditorGUILayout.TextField ("查询到的绝对角度:", ListData3);
		ListData4 = EditorGUILayout.TextField ("查询到的相对角度:", ListData4);
	}
}