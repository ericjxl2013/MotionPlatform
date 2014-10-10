/// <summary>
/// FileName: ComponentsControl.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.04.25
/// Version: 1.0
/// Company: Sunnytech
/// Function: 部件提示管理
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>
using UnityEngine;
using System.Collections;
using System.IO;

public class ComponentsControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ColliderAdd();
	}
	
	private void ColliderAdd()
	{
		string filePath = Application.dataPath + FuncPara.componentsPath;
		if(File.Exists(filePath)){
			FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			StreamReader fileReader = new StreamReader(fileStream);
			string lineStr = fileReader.ReadLine();
			string[] stringArray;
			while(lineStr != null){
				stringArray = lineStr.Split(',');
				GameObject tempObj = GameObject.Find(stringArray[0]);
				if(tempObj != null){
					FuncPara.componentsDic.Add(stringArray[0], stringArray[1]);
					if(tempObj.collider == null){
						tempObj.AddComponent<MeshCollider>();
					}
					tempObj.AddComponent<PartsTips>();
				}
				lineStr = fileReader.ReadLine();
			}
			fileReader.Close();
		}
	}
}
