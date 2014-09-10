using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[AddComponentMenu("可视化路径编辑器")]
public class VisualizeManager : MonoBehaviour {

	public bool initialized = false;  //当前脚本是否初始化
	public bool pathVisible = true;  //是否可见
	public string pathName = "";  //路径名称
	public Color pathColor = Color.cyan;  //路径颜色
	
	public int nodeCount;  //控制顶点个数

	public bool isCamera = true;  //控制物体是否为摄像机
	public bool isLookAt = false;  //是否对着目标物体
	public bool needExcess = false;  //相机需要过度
	public Transform lookAt = null;  //摄像机观察目标

	public bool isSlider = false;  //是否启用进度条功能
	public bool isSliderLock = false;  //进度条状态控制
	public float sliderValue = 0f;  //进度条进度
	public float sliderLock = 0f;  //进度条锁
	public Vector3[] diffVector = new Vector3[3];  //插值用

	//Camera View处理
	public List<float> cameraViewList = new List<float>() {10f, 10f};
	public float CameraView
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
	//Camera Rotation处理
	public List<Vector3> rotationList = new List<Vector3>() { Vector3.zero, Vector3.zero};
	//位置信息等显示控制
	public List<bool> displayInfoList = new List<bool>() { false, false };
	//控制顶点列表
	public List<Vector3> nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };
	//控制手柄显示控制
	public List<bool> handleDisplay = new List<bool>() { true, true }; 
 
	//插值管理
	private float[] segmentTimeRate = new float[] {0f, 1f};  //插值间隔
	private int currentIndex = 0;  //前插值点序号
	private float currentTimeRate = 0f;  //当前时间

	private Vector3[] controlPoint;
	private float endTime = 0f;
	public bool moveFlag = false;
	private float totalTime = 10f;
	public float moveSpeed = 2f;  //测试运行速度
	public bool moveAllow = false;  //是否允许运动

    public bool isFirstAddPoint = true;  //是否是程序添加的第一个点
    public bool isActive = false;  //是否是激活状态
    public Rect btnRect = new Rect(0, 0, 100f, 50);
    public int currentNum = 0;
    
	void OnDrawGizmosSelected()
	{
		if (pathVisible)
		{
			if (nodes.Count > 0)
			{
				VisualClass.DrawPath(nodes.ToArray(), pathColor);
			}
		}
	}

	// Use this for initialization
	void Start()
	{
        btnRect = new Rect(0, 0, 100f, 50);
        btnRect.x = Screen.width - btnRect.width;
        btnRect.y = Screen.height - btnRect.height;
	}

    void OnGUI()
    {
        if (isActive && EditorApplication.isPlaying)
        {
            if (GUI.Button(btnRect, currentNum + "：创建"))
            {
                if (isFirstAddPoint && currentNum == 0)
                {
                    nodes[1] = transform.position;
                    if (camera != null)
                    {
                        cameraViewList[1] = CameraView;
                    }
                    else
                    {
                        cameraViewList[1] = 10f;
                    }
                    if (isCamera && camera != null)
                    {
                        if (isLookAt && lookAt != null)
                        {
                            transform.LookAt(lookAt);
                        }
                    }
                    rotationList[1] = transform.eulerAngles;
                    isFirstAddPoint = false;
                    currentNum++;
                    AngleOptimization();
                }
                else
                {
                    nodes.Insert(currentNum + 1, transform.position);
                    if (camera != null)
                    {
                        cameraViewList.Insert(currentNum + 1, CameraView);
                    }
                    else
                    {
                        cameraViewList.Insert(currentNum + 1, 10f);
                    }
                    if (isCamera && camera != null)
                    {
                        if (isLookAt && lookAt != null)
                        {
                            transform.LookAt(lookAt);
                        }
                    }
                    rotationList.Insert(currentNum + 1, transform.eulerAngles);
                    displayInfoList.Insert(currentNum + 1, false);
                    handleDisplay.Insert(currentNum + 1, true);
                    nodeCount = nodes.Count;
                    currentNum++;
                    AngleOptimization();
                }
            }
        }
    }

	//场景Target状态插值
	public void SceneInterp(float current_value)
	{
		Vector3[] tempPoints = VisualClass.PathControlPointGenerator(nodes.ToArray());
		TimeRateGenerator(nodeCount);
		VisualClass.TestMove(tempPoints, transform, current_value);
		if (isCamera && camera != null)  //摄像机
		{
			if (isLookAt && lookAt != null)  //有中心物体
			{
				if (needExcess)
				{
					if (current_value < 0.1f)
					{
						transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], current_value * 10f);
					}
					else
					{
						transform.LookAt(lookAt);
					}
				}
				else
				{
					transform.LookAt(lookAt);
				}
				CameraView = ViewLerp(current_value);
			}
			else  //无中心物体
			{
				transform.eulerAngles = EulerAngleLerp(current_value, ref currentIndex, ref currentTimeRate);
				//二分法查找算一次
				CameraView = Mathf.Lerp(cameraViewList[currentIndex], cameraViewList[currentIndex + 1], currentTimeRate);
			}
		}
		else   //普通物体运动
		{
			transform.eulerAngles = EulerAngleLerp(current_value, ref currentIndex, ref currentTimeRate);
		}
	}

    //角度值优化，因为角度非常不确定，在类似90和-270之间
    private void AngleOptimization()
    {
        for (int i = 1; i < rotationList.Count; i++)
        {
            Vector3 tempVec = rotationList[i];
            tempVec.x = AngleClerp(rotationList[i - 1].x, rotationList[i].x, 1f);
            tempVec.y = AngleClerp(rotationList[i - 1].y, rotationList[i].y, 1f);
            tempVec.z = AngleClerp(rotationList[i - 1].z, rotationList[i].z, 1f);
            rotationList[i] = tempVec;
        }
    }

    //角度修改使其符合实际（类似-90和270这种情况处理）
    private float AngleClerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

	public void MotionActive()
	{
		if (!moveFlag)
		{
			controlPoint = VisualClass.PathControlPointGenerator(nodes.ToArray());
			TimeRateGenerator(nodeCount);
			Debug.Log("Point Number: " + controlPoint.Length);
			totalTime = VisualClass.TimeGet(controlPoint, moveSpeed);
			Debug.Log("Time: " + totalTime);
			endTime = 0.0f;
			moveFlag = true;
		}
	}

	// Update is called once per frame
	void Update()
	{

		//if (Input.GetKeyDown(KeyCode.Q))
		//{
		//	controlPoint = VisualClass.PathControlPointGenerator(nodes.ToArray());
		//	Debug.Log("Point Number: " + controlPoint.Length);
		//}

		//if (Input.GetKeyDown(KeyCode.T))
		//{
		//	totalTime = VisualClass.TimeGet(controlPoint, 1f);
		//	Debug.Log("Time: " + totalTime);
		//}

		//if (Input.GetKeyDown(KeyCode.S))
		//{
		//	endTime = 0.0f;
		//	moveFlag = true;
		//}

		if (moveFlag)
		{
			endTime += Time.deltaTime;
		}

        if (moveFlag && endTime >= totalTime)
        {
            moveFlag = false;
            VisualClass.TestMove(controlPoint, transform, 1f);
			if (camera != null)
			{
				CameraView = cameraViewList[cameraViewList.Count - 1];
			}
			if (isCamera && isLookAt && lookAt != null)
				transform.LookAt(lookAt);
			else
				transform.eulerAngles = rotationList[rotationList.Count - 1];
        }
	}

	void LateUpdate()
	{
		if (moveFlag)
		{
			VisualClass.TestMove(controlPoint, transform, endTime / totalTime);
			if (isCamera && camera != null)  //摄像机
			{
				if (isLookAt && lookAt != null)  //有中心物体
				{
					if (needExcess)
					{
						if (endTime / totalTime < 0.1f)
						{
							transform.eulerAngles = Vector3.Lerp(diffVector[0], diffVector[1], endTime / totalTime * 10f);
						}
						else
						{
							transform.LookAt(lookAt);
						}
					}
					else
					{
						transform.LookAt(lookAt);
					}
					CameraView = ViewLerp(endTime / totalTime);
				}
				else  //无中心物体
				{
					transform.eulerAngles = EulerAngleLerp(endTime / totalTime, ref currentIndex, ref currentTimeRate);
					//二分法查找算一次
					CameraView = Mathf.Lerp(cameraViewList[currentIndex], cameraViewList[currentIndex + 1], currentTimeRate);
				}
			}
			else   //普通物体运动
			{
				transform.eulerAngles = EulerAngleLerp(endTime / totalTime, ref currentIndex, ref currentTimeRate);
			}
		}
	}

	//角度和View插值时时间片段计算
	private void TimeRateGenerator(int count)
	{
        List<float> tempRate = new List<float>();
        tempRate.Add(0f);
        float segment = 1f / (count - 1);
        for (int i = 1; i < count; i++)
        {
            if (i == count - 1)
                tempRate.Add(1f);
            else
                tempRate.Add(segment * i);
        }
        segmentTimeRate = tempRate.ToArray();
	}

	//角度插值
	private Vector3 EulerAngleLerp(float time_rate, ref int current_index, ref float current_time_rate)
	{
		Vector3 rVec = Vector3.zero;
		if (time_rate <= 0f)
		{
			rVec = rotationList[0];
			current_index = 0;
			current_time_rate = 0f;
		}
		else if (time_rate >= 1f)
		{
			rVec = rotationList[rotationList.Count - 1];
			current_index = rotationList.Count - 2;
			current_time_rate = 1f;
		}
		else if (rotationList.Count == 2)
		{
			rVec = Vector3.Lerp(rotationList[0], rotationList[1], time_rate);
			current_index = 0;
			current_time_rate = time_rate;
		}
		else
		{
			float currentRate = 0f;
			int index = BisectionQuery(time_rate, out currentRate);
			rVec = Vector3.Lerp(rotationList[index], rotationList[index + 1], currentRate);
			current_index = index;
			current_time_rate = currentRate;
		}
		return rVec;
	}

	//View插值
	private float ViewLerp(float time_rate)
	{
		float rFloat = 0f;
		if (time_rate <= 0f)
		{
			rFloat = cameraViewList[0];
		}
		else if (time_rate >= 1f)
		{
			rFloat = cameraViewList[cameraViewList.Count - 1];
		}
		else if (cameraViewList.Count == 2)
		{
			rFloat = Mathf.Lerp(cameraViewList[0], cameraViewList[1], time_rate);
		}
		else
		{
			float currentRate = 0f;
			int index = BisectionQuery(time_rate, out currentRate);
			rFloat = Mathf.Lerp(cameraViewList[index], cameraViewList[index + 1], currentRate);
		}
		return rFloat;
	}

	//二分法查询
	private int BisectionQuery(float time_rate, out float current_rate)
	{
		int index = 0;
		int low = 0;
		int high = segmentTimeRate.Length;
		while (high - low > 1) 
		{
			index = (low + high) / 2;
			if (time_rate == segmentTimeRate[index]) //相等的情况处理
			{
				current_rate = 1f;
				if (index == 0)
					return index;
				else
					return index - 1;
			}
			else if (time_rate > segmentTimeRate[index])
			{
				low = index;
			}
			else 
			{
				high = index;
			}
		}
		if (time_rate >= segmentTimeRate[low] && time_rate <= segmentTimeRate[high])
		{
			index = low;
			current_rate = (time_rate - segmentTimeRate[low]) / (segmentTimeRate[high] - segmentTimeRate[low]);
		}
		else
		{
			current_rate = 1f;
			index = segmentTimeRate.Length - 1;
		}
		return index;
	}

	
}
