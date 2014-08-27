using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class PositionAndCameraAdjust : EditorWindow {

	string Gameobject_adjust = "物体位置调整";
	static string Gameobject_localPosition = "";
	static string Gameobject_localEulerangles = "";

	string Camera_adjust = "相机位置调整";
	string Gameobject_Name = "";
	string Gameobject_Camera = "";

	static string filepath = Application.dataPath+ "/StreamingAssets/MotionEditor/PositionAndEulerangles.txt";

	// Add menu named "My Window" to the Window menu
	[MenuItem("Tools/运动平台/调整物体和相机的位置角度")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		PositionAndCameraAdjust window = (PositionAndCameraAdjust)EditorWindow.GetWindow (typeof (PositionAndCameraAdjust));
		
		window.Show();

		StreamReader objReader = new StreamReader(filepath);
		string sLine="";
		int i = 0;  
		while (sLine != null)
		{
			sLine = objReader.ReadLine();
			if (sLine != null && !sLine.Equals("")){
				if(i == 0){
					string[] tmpStr = sLine.Split('|');
					Gameobject_localPosition = tmpStr[0];
					Gameobject_localEulerangles = tmpStr[1];
				}
			}
		}
		objReader.Close();
	}
	
	void OnGUI(){
		GUILayout.Label (Gameobject_adjust, EditorStyles.boldLabel);
		Gameobject_localPosition = EditorGUILayout.TextField ("物体相对坐标:", Gameobject_localPosition);
		Gameobject_localEulerangles = EditorGUILayout.TextField ("物体相对角度:", Gameobject_localEulerangles);
		if(GUILayout.Button("获得物体信息"))
		{
			Gameobject_localPosition = 	Selection.activeTransform.localPosition.x.ToString()+","+
				Selection.activeTransform.localPosition.y.ToString()+","+
				Selection.activeTransform.localPosition.z.ToString();
			Gameobject_localEulerangles = Selection.activeTransform.localEulerAngles.x.ToString()+","+
				Selection.activeTransform.localEulerAngles.y.ToString()+","+
				Selection.activeTransform.localEulerAngles.z.ToString();

			if(File.Exists(filepath)){
				File.Delete(filepath);
			}
			FileStream fs = new FileStream(filepath, FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(Gameobject_localPosition+ "|"+ Gameobject_localEulerangles);
			sw.Flush();
			sw.Close();
			fs.Close();
		}
		if(GUILayout.Button("设置物体信息"))
		{
			string[] tmpPos = Gameobject_localPosition.Split(',');
			string[] tmpElu = Gameobject_localEulerangles.Split(',');
			Selection.activeTransform.localPosition = new Vector3(float.Parse(tmpPos[0]), float.Parse(tmpPos[1]), float.Parse(tmpPos[2]));
			Selection.activeTransform.localEulerAngles = new Vector3(float.Parse(tmpElu[0]), float.Parse(tmpElu[1]), float.Parse(tmpElu[2]));
		}

		GUILayout.Label (Camera_adjust, EditorStyles.boldLabel);
		Gameobject_Name = EditorGUILayout.TextField ("物体名:", Gameobject_Name);
		if(GUILayout.Button("获得相机关注物体"))
		{
			Gameobject_Name = Selection.activeTransform.name;
		}
		Gameobject_Camera = EditorGUILayout.TextField ("相机名:", Gameobject_Camera);
		if(GUILayout.Button("获得相机"))
		{
			string tmp_Camera = Selection.activeTransform.name;
			Debug.Log("tmp_Camera:"+ GameObject.Find(tmp_Camera).camera);
			if(GameObject.Find(tmp_Camera).camera != null){
				Gameobject_Camera = Selection.activeTransform.name;
			}
		}
		if(GUILayout.Button("调整相机位置"))
		{
			if(Gameobject_Camera != "" && Gameobject_Name != ""){
				GameObject.Find(Gameobject_Camera).transform.LookAt(GameObject.Find(Gameobject_Name).transform);
			}
		}
	}
}
