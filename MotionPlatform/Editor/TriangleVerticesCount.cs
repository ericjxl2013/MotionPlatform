using UnityEngine;
using System.Collections;
using UnityEditor;

public class TriangleVerticesCount : EditorWindow {
	private int gameObjCount = 0;
	private int emptyObjCount = 0;
	private int faceCount = 0;
	private int verticeCount = 0;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Tools/美工/场景模型面片数和顶点数计算")]
	static void Init () {
		TriangleVerticesCount window = (TriangleVerticesCount)EditorWindow.GetWindow (typeof(TriangleVerticesCount));
		window.Show();
	}
	
	void OnGUI()
	{
		GUILayout.Label("请在Hierarchy窗口选择需要计算的GameObject", EditorStyles.boldLabel);
		if(GUILayout.Button("计算"))
		{
			Calculate();
		}
		GUILayout.Label("1、GameObject数量：", EditorStyles.boldLabel);
		GUILayout.Label(gameObjCount.ToString());
		GUILayout.Label("2、空GameObject数量：", EditorStyles.boldLabel);
		GUILayout.Label(emptyObjCount.ToString());
		GUILayout.Label("3、总面片数：", EditorStyles.boldLabel);
		GUILayout.Label(faceCount.ToString());
		GUILayout.Label("4、总顶点数：", EditorStyles.boldLabel);
		GUILayout.Label(verticeCount.ToString());
		if(GUILayout.Button("Clear"))
		{
			gameObjCount = 0;
			emptyObjCount = 0;
			faceCount = 0;
			verticeCount = 0;
		}
	}
	
	//计算GameObject面片数和定点数
	private void Calculate()
	{
		gameObjCount = 0;
		emptyObjCount = 0;
		faceCount = 0;
		verticeCount = 0;
		Transform[] selections = Selection.GetTransforms(SelectionMode.Unfiltered);
		for(int i = 0; i < selections.Length; i++){
			CalRecursion(selections[i]);
		}
	}
	
	//递归计算
	private void CalRecursion(Transform father)
	{
		gameObjCount++;
		if(father.renderer == null){
			emptyObjCount++;
		}else{
			try{
				faceCount += father.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
				verticeCount += father.GetComponent<MeshFilter>().sharedMesh.vertexCount;
			}catch{
				Debug.LogError(father.name + "该物体不含有Mesh，请确认!");
			}
		}
		if(father.childCount > 0){
			foreach(Transform child in father){
				if(child.childCount > 0){
					CalRecursion(child);
				}else{
					gameObjCount++;
					if(child.renderer == null){
						emptyObjCount++;
					}else{
						try{
							faceCount += child.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
							verticeCount += child.GetComponent<MeshFilter>().sharedMesh.vertexCount;
						}catch{
							Debug.LogError(child.name + "该物体不含有Mesh，请确认!");
						}
					}
				}
			}
		}
	}
	
}
