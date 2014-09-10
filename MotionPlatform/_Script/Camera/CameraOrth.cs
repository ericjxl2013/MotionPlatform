using UnityEngine;
using System.Collections;

public class CameraOrth : MonoBehaviour 
{
	Camera mainCamera;  //主摄像机
	public Transform upDownTrans;  //鼠标上下方向拖动旋转时的参考物体（位置根据具体的被旋转物体基本固定）
	public Transform rightLeftTrans;  //鼠标左右方向拖动旋转时的参考物体（位置会随摄像机移动而移动，在初始化时应设置在合适位置，即物体在摄像机中心时该空物体也在物体中心）
	//private Vector3 upDownSavePoint = Vector3.zero;  //初始点位置保存（不用修改）
	Vector3 axe;  //旋转轴参数（不用修改）
	private const float MOVE_RATIO = 0.125f;  //移动速率控制系数（摄像机缩放时，移动速率修正，一般不用改，可微调）
	private const float SCALE_RATIO = 1.6f;  //缩放速率控制系数（摄像机缩放时，缩放速率修正，一般不用改，可微调）
	private float rotateFactor = 7f;  //旋转速率调节(主要调这个)
	private float scaleFactor = 0.8f;  //缩放速率调节(主要调这个)
	private float moveFactor = 13f;  //移动速率调节(主要调这个)

	//摄像机缩放参数
	private float CameraView
	{
		get
		{
			if (camera.orthographic)
				return camera.orthographicSize;
			else return camera.fieldOfView;
		}

		set 
		{
			if (camera.orthographic)
				camera.orthographicSize = value;
			else camera.fieldOfView = value;
		}
	}

	void Awake()
	{
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		upDownTrans = GameObject.Find("upDownTrans").transform;
		//upDownSavePoint = upDownTrans.position;
		rightLeftTrans = GameObject.Find("rightLeftTrans").transform;
		rightLeftTrans.rotation = mainCamera.transform.rotation;
		rightLeftTrans.parent = mainCamera.transform;
	}
	
	void LateUpdate()
	{
		//中键旋转		
		if(Input.GetMouseButton(2) && !Input.GetMouseButton(1))	
		{
			axe.y = -Input.GetAxis("Mouse Y")*rotateFactor;
			axe.x = Input.GetAxis("Mouse X")*rotateFactor;
			mainCamera.transform.parent = upDownTrans;
			upDownTrans.Rotate(new Vector3(axe.y,0,0)); //上下旋转
			mainCamera.transform.RotateAround(rightLeftTrans.position, Vector3.up, axe.x);  //左右旋转
			mainCamera.transform.parent = null;
			upDownTrans.rotation = mainCamera.transform.rotation;  //角度修正
		}
		//中键+右键平移	
		if(Input.GetMouseButton(2) && Input.GetMouseButton(1))
		{
			float delta_x, delta_y;
			delta_x = Input.GetAxis("Mouse X") * moveFactor * CameraView * MOVE_RATIO;
			delta_y = Input.GetAxis("Mouse Y") * moveFactor * CameraView * MOVE_RATIO;
			mainCamera.transform.Translate(new Vector3(-delta_x, -delta_y, 0)*Time.deltaTime, Space.Self);
		}					
		//滚轮放大
		if (Input.GetAxis("Mouse ScrollWheel") != 0) 					
		{
			float delta_z;
			delta_z = Input.GetAxis("Mouse ScrollWheel") * scaleFactor * CameraView / SCALE_RATIO;
			
//			//上下旋转点位置修正  根据具体情况设置
//			if(mainCamera.orthographicSize < 0.56f){
//				if(ClampControl_Script.ClampOn)
//					upDownTrans.position = GameObject.Find (SystemArguments.BlankName).transform.position;
//				else
//					upDownTrans.position = GameObject.Find ("workbench_1").transform.position;
//			}else{
//				upDownTrans.position = upDownSavePoint;
//			}

			if (CameraView > 0.02f || delta_z > 0)
			{
				CameraView += delta_z;
			}

			if (CameraView < 0.02f)
			{
				CameraView = 0.02F;
			}
			else if (CameraView > 100f)
			{
				CameraView = 100f;
			}
		}		
	}
}
