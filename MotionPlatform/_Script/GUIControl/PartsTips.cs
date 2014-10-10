/// <summary>
/// FileName: PartsTips.cs
/// Author: Jiang Xiaolong
/// Created Time: 2014.04.25
/// Version: 1.0
/// Company: Sunnytech
/// Function: 部件提示Collider控制
///
/// Changed By:
/// Modification Time:
/// Discription:
/// </summary>	
using UnityEngine;
using System.Collections;

public class PartsTips : MonoBehaviour {
	
	void OnMouseEnter()
	{
		if(FuncPara.componentTips && gameObject.renderer.isVisible){
			if(FuncPara.componentsDic.ContainsKey(gameObject.name)){
				FuncPara.componentString = FuncPara.componentsDic[gameObject.name];
				FuncPara.componentDisplay = true;
			}
		}
	}
	
	void OnMouseExit()
	{
		FuncPara.componentDisplay = false;
	}
}
