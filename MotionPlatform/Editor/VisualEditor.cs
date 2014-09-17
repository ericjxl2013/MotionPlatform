using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

[CustomEditor(typeof(VisualizeManager))]
public class VisualEditor : Editor 
{
	VisualizeManager _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	private const float SLIDERVALUE = 100f;
	private string isRelativePathStr = "加载相对路径";

	private static GUIContent
		insertContent = new GUIContent("+", "增加控制顶点"),
		deleteContent = new GUIContent("-", "删除当前控制顶点"),
		focusContent = new GUIContent("F", "只查看当前控制点"),
		transContent = new GUIContent("T", "移动当前物体到该点"),
		modifyContent = new GUIContent("M", "修改当前点为当前物体状态");
	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(25f);
		//focusWidth = GUILayout.MaxWidth(50f);

	void OnEnable()
	{
		//界面风格
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (VisualizeManager)target;
        _target.isActive = true;
        _target.btnRect.x = Screen.width - _target.btnRect.width;
        _target.btnRect.y = Screen.height - _target.btnRect.height;
        if (_target.isFirstAddPoint && _target.nodes.Count == 2)
            _target.currentNum = 0;
        else
            _target.currentNum = _target.nodes.Count - 1;
		//初始化
		if (!_target.initialized)
		{
			_target.initialized = true;
			string timeStr = System.DateTime.Now.Year + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Hour + "_" + System.DateTime.Now.Minute + "_" + System.DateTime.Now.Second;
			_target.pathName = "Path_" + timeStr;
			_target.nodes[0] = _target.transform.position;
			_target.nodes[1] = _target.transform.position + new Vector3(0.1f, 0.1f, 0.1f);
			_target.rotationList[0] = _target.rotationList[1] = _target.transform.eulerAngles;
			if (_target.camera != null)
			{ 
				_target.cameraViewList[0] = _target.cameraViewList[1] = _target.CameraView;
			}
			_target.isSliderLock = _target.isSlider;
            _target.currentNum = 0;
		}
	}

    void OnDisable()
    {
        _target.isActive = false;
    }

	//是否显示HandlePosition
	private void HandleDisplayControl(int index)
	{
		for (int i = 0; i < _target.handleDisplay.Count; i++)
		{
			if (i == index)
				_target.handleDisplay[i] = true;
			else
				_target.handleDisplay[i] = false;
		}
	}

	//是否显示HandlePosition
	private void HandleDisplayControl(bool flag)
	{
		for (int i = 0; i < _target.handleDisplay.Count; i++)
		{
			_target.handleDisplay[i] = flag;
		}
	}

	public override void OnInspectorGUI()
	{
		//路径是否可见
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Visible");
		_target.pathVisible = EditorGUILayout.Toggle(_target.pathVisible);
		EditorGUILayout.EndHorizontal();

		//路径是否可见
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Is Camera");
		_target.isCamera = EditorGUILayout.Toggle(_target.isCamera);
		EditorGUILayout.EndHorizontal();

		if (_target.isCamera)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Look At");
			_target.isLookAt = EditorGUILayout.Toggle(_target.isLookAt);
			if(_target.isLookAt)
				_target.lookAt = (Transform)EditorGUILayout.ObjectField(_target.lookAt, typeof(Transform), true);
			EditorGUILayout.EndHorizontal();
		}

		//路径名字
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Name");
		_target.pathName = EditorGUILayout.TextField(_target.pathName);
		EditorGUILayout.EndHorizontal();

		//路径颜色
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");
		_target.pathColor = EditorGUILayout.ColorField(_target.pathColor);
		EditorGUILayout.EndHorizontal();

		//样条控制点数
		EditorGUILayout.BeginHorizontal();
		//EditorGUILayout.PrefixLabel("Node Count");
		_target.nodeCount = Mathf.Max(2, EditorGUILayout.IntField("Node Count", _target.nodeCount));
		//_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider(_target.nodeCount, 0, 10), 2,100);
		EditorGUILayout.EndHorizontal();

		//增加节点
		if (_target.nodeCount > _target.nodes.Count)
		{
			for (int i = 0; i < _target.nodeCount - _target.nodes.Count; i++)
			{
				_target.nodes.Add(_target.nodes[_target.nodes.Count - 1]);
				_target.rotationList.Add(_target.rotationList[_target.rotationList.Count - 1]);
				_target.cameraViewList.Add(10f);
				_target.displayInfoList.Add(false);
				_target.handleDisplay.Add(true);
				AngleOptimization();
			}
		}
		//移除节点
		else if (_target.nodeCount < _target.nodes.Count)
		{
			if (EditorUtility.DisplayDialog("移除节点警告！", "删除节点后该节点会立即从路径上消除，并且不可恢复，是否确认继续删除？", "确认", "取消"))
			{
				int removeCount = _target.nodes.Count - _target.nodeCount;
				_target.nodes.RemoveRange(_target.nodes.Count - removeCount, removeCount);
				_target.rotationList.RemoveRange(_target.nodes.Count - removeCount, removeCount);
				_target.cameraViewList.RemoveRange(_target.nodes.Count - removeCount, removeCount);
				_target.displayInfoList.RemoveRange(_target.nodes.Count - removeCount, removeCount);
				_target.handleDisplay.RemoveRange(_target.nodes.Count - removeCount, removeCount);
				AngleOptimization();
                if (_target.currentNum > _target.nodes.Count - 1)
                    _target.currentNum = _target.nodes.Count - 1;
				return;
			}
			else
			{
				_target.nodeCount = _target.nodes.Count;
			}
		}

		//节点状态信息显示
		for (int i = 0; i < _target.nodes.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("第" + i + "号");
			_target.displayInfoList[i] = EditorGUILayout.Toggle(_target.displayInfoList[i]);

			if (GUILayout.Button(focusContent, EditorStyles.miniButtonLeft, buttonWidth))  //控制点
			{
				HandleDisplayControl(i);
			}
			if (GUILayout.Button(transContent, EditorStyles.miniButtonMid, buttonWidth))  //移动
			{
				_target.transform.position = _target.nodes[i];
				if (_target.camera != null)
				{
					_target.CameraView = _target.cameraViewList[i];
				}
				if (_target.isCamera && _target.camera != null && i != 0)
				{
					if (_target.isLookAt && _target.lookAt != null)
					{
						_target.transform.LookAt(_target.lookAt);
					}
					else
					{
						_target.transform.eulerAngles = _target.rotationList[i];
					}
				}
				else
				{
					_target.transform.eulerAngles = _target.rotationList[i];
				}
                _target.currentNum = i;
			}
			if (GUILayout.Button(modifyContent, EditorStyles.miniButtonMid, buttonWidth))  //修改
			{
				_target.nodes[i] = _target.transform.position;
				if (_target.camera != null)
				{
					_target.cameraViewList[i] = _target.CameraView;
				}
				if (_target.isCamera && _target.camera != null)
				{
					if (_target.isLookAt && _target.lookAt != null)
					{
						_target.transform.LookAt(_target.lookAt);
						_target.rotationList[i] = _target.transform.eulerAngles;
					}
					else
					{
						_target.rotationList[i] = _target.transform.eulerAngles;
					}
				}
				else
				{
					_target.rotationList[i] = _target.transform.eulerAngles;
				}
                _target.currentNum = i;
				AngleOptimization();
			}
			if (GUILayout.Button(insertContent, EditorStyles.miniButtonMid, buttonWidth))  //插入
			{
				_target.nodes.Insert(i + 1, _target.transform.position);
				if (_target.camera != null)
				{
					_target.cameraViewList.Insert(i + 1, _target.CameraView);
				}
				else
				{
					_target.cameraViewList.Insert(i + 1, 10f);
				}
				if (_target.isCamera && _target.camera != null)
				{
					if (_target.isLookAt && _target.lookAt != null)
					{
						_target.transform.LookAt(_target.lookAt);
						_target.rotationList.Insert(i + 1, _target.transform.eulerAngles);
					}
					else
					{
						_target.rotationList.Insert(i + 1, _target.transform.eulerAngles);
					}
				}
				else
				{
					_target.rotationList.Insert(i + 1, _target.transform.eulerAngles);
				}
				_target.displayInfoList.Insert(i + 1, false);
				_target.handleDisplay.Insert(i + 1, true);
				_target.nodeCount = _target.nodes.Count;
				AngleOptimization();
                _target.currentNum = i + 1;
			}
			if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))  //删除
			{
				if (EditorUtility.DisplayDialog("移除节点警告！", "删除节点后该节点会立即从路径上消除，并且不可恢复，是否确认继续删除？", "确认", "取消"))
				{
					if (_target.nodeCount > 2)
					{
						_target.nodes.RemoveAt(i);
						_target.rotationList.RemoveAt(i);
						_target.cameraViewList.RemoveAt(i);
						_target.displayInfoList.RemoveAt(i);
						_target.handleDisplay.RemoveAt(i);
						_target.nodeCount = _target.nodes.Count;
						AngleOptimization();
                        if (_target.currentNum > _target.nodes.Count - 1)
                            _target.currentNum = _target.nodes.Count - 1;
						return;
					}
				}
				else
				{
					//Do nothing
				}
			}
			//显示位置等状态信息
			if (_target.displayInfoList[i])
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical();
				_target.nodes[i] = EditorGUILayout.Vector3Field("", _target.nodes[i], GUILayout.MaxHeight(20f));
				_target.rotationList[i] = EditorGUILayout.Vector3Field("", _target.rotationList[i], GUILayout.MaxHeight(20f));
				if (_target.isCamera)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("CameraView");
					_target.cameraViewList[i] = EditorGUILayout.FloatField(_target.cameraViewList[i], GUILayout.MaxHeight(20f));
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginHorizontal();
			}

			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("控制手柄全部显示"))
		{
			HandleDisplayControl(true);
		}

		if (GUILayout.Button("设为初始状态"))
		{
			GetInitialState();
		}

		if (GUILayout.Button("控制点信息保存"))
		{
            JsonWirter();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("相对路径");
		_target.isRelativePos = EditorGUILayout.Toggle(_target.isRelativePos);
		if (_target.isRelativePos)
			isRelativePathStr = "加载相对路径";
		else
			isRelativePathStr = "加载绝对路径";
		if (GUILayout.Button(isRelativePathStr, GUILayout.MaxWidth(260f)))
		{
			JsonLoad(_target.isRelativePos);
		}
		EditorGUILayout.EndHorizontal();

		//if (GUILayout.Button("加载控制点信息"))
		//{
		//	JsonLoad();
		//}

		//进度条控制
		//路径是否可见
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Use Silder");
		_target.isSlider = EditorGUILayout.Toggle(_target.isSlider);
		EditorGUILayout.EndHorizontal();
		if (_target.isSlider)
		{
			_target.sliderValue = GUILayout.HorizontalSlider(_target.sliderValue, 0f, SLIDERVALUE);
			SceneInterp(_target.sliderValue / SLIDERVALUE);
		}

		if (_target.isSliderLock != _target.isSlider)
		{
			_target.isSliderLock = _target.isSlider;
			if (!_target.isSlider)
			{
				_target.transform.position = _target.nodes[0];
			}
		}

		if (EditorApplication.isPlaying)
		{
			//测试运行速度
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Speed");
			_target.moveSpeed = EditorGUILayout.FloatField(_target.moveSpeed);
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("开始运动"))
			{
				_target.MotionActive();
			}
		}

		//如果有更新，则重新绘制
		if (GUI.changed)
		{
			EditorUtility.SetDirty(_target);
		}
	}

	void OnSceneGUI()
	{
		if (_target.pathVisible)
		{
			if (_target.nodes.Count > 0)
			{
				//撤销操作，Unity内部函数
				Undo.RecordObject(_target, "Adjust iTween Path");
				//开始点位置控制
				//if (_target.followTarget && !_target.isSlider && !_target.moveFlag)
				//	_target.nodes[0] = _target.transform.position;
				//位置手柄显示
				for (int i = 0; i < _target.nodes.Count; i++)
				{
					if (i != _target.nodes.Count - 1)
					{
						Handles.Label(_target.nodes[i], "第 " + i + "号", style);
						if (_target.handleDisplay[i])
							_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
					}
					else
					{
						Handles.Label(_target.nodes[i], "'" + _target.pathName + "' 结束", style);
						if (_target.handleDisplay[i])
							_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
					}
				}
			}
		}
	}

	//设置Target初始状态
	private void GetInitialState()
	{
		_target.nodes[0] = _target.transform.position;
		_target.rotationList[0] = _target.transform.eulerAngles;
		if (_target.isCamera && _target.camera != null)
		{
			_target.cameraViewList[0] = _target.CameraView;
			if (_target.isLookAt && _target.lookAt != null)
			{
				_target.diffVector[0] = _target.transform.eulerAngles;
				GameObject cameraEmpty = new GameObject();
				cameraEmpty.name = "CameraEmpty";
				Vector3[] controlPoint = VisualClass.PathControlPointGenerator(_target.nodes.ToArray());
				cameraEmpty.transform.position = VisualClass.Interp(controlPoint, 0.1f);
				cameraEmpty.transform.LookAt(_target.lookAt);
				_target.diffVector[1] = cameraEmpty.transform.eulerAngles;
				_target.diffVector[1] = new Vector3(AngleClerp(_target.diffVector[0].x, _target.diffVector[1].x, 1f), AngleClerp(_target.diffVector[0].y, _target.diffVector[1].y, 1f), AngleClerp(_target.diffVector[0].z, _target.diffVector[1].z, 1f));
				float diff = (_target.diffVector[1] - _target.diffVector[0]).magnitude;
				if (diff > 0.1f)
				{
					_target.needExcess = true;

				}
				else
				{
					_target.needExcess = false;
				}
				//Debug.Log(diff);
				//Debug.Log(_target.diffVector[1].x + "," + _target.diffVector[1].y + ", " + _target.diffVector[1].z);
				//Debug.Log(_target.diffVector[0].x + "," + _target.diffVector[0].y + ", " + _target.diffVector[0].z);
				DestroyImmediate(cameraEmpty);
			}
		}
	}

	//场景中物体位置插值
	private void SceneInterp(float current_value)
	{
		if (_target.sliderLock != current_value)
		{
			if (Mathf.Approximately(current_value, 0f))  //0点
			{
				_target.transform.position = _target.nodes[0];
				_target.transform.eulerAngles = _target.rotationList[0];
				if (_target.camera != null)
				{
					_target.CameraView = _target.cameraViewList[0];
				}
			}
			else
			{
				_target.SceneInterp(current_value);
			}
			_target.sliderLock = current_value;
		}
	}

	//角度值优化，因为角度非常不确定，在类似90和-270之间
	private void AngleOptimization()
	{
		for (int i = 1; i < _target.rotationList.Count; i++)
		{
			Vector3 tempVec = _target.rotationList[i];
			tempVec.x = AngleClerp(_target.rotationList[i - 1].x, _target.rotationList[i].x, 1f);
			tempVec.y = AngleClerp(_target.rotationList[i - 1].y, _target.rotationList[i].y, 1f);
			tempVec.z = AngleClerp(_target.rotationList[i - 1].z, _target.rotationList[i].z, 1f);
			_target.rotationList[i] = tempVec;
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

    //Json写入
    private void JsonWirter()
    {
        string dirPath = MotionPara.taskRootPath + MotionPara.taskName;
        string filePath = dirPath + "/PathControl.json";
        string tempDir = Application.dataPath + "/_TempExcel";
        string tempPath = tempDir + "/TempPathInfo.json";
        if (Directory.Exists(dirPath))
        {
            //移动到起点
            _target.transform.position = _target.nodes[0];
            if (_target.camera != null)
            {
                _target.CameraView = _target.cameraViewList[0];
            }
            _target.transform.eulerAngles = _target.rotationList[0];
            JsonOperator jsonOp = new JsonOperator();
            string[] nameArray = new string[] { "POSITION", "EULERANGLE", "VIEW", "ABSOLUTEPOS", "ABSOLUTEANGLE" };
            string[,] contentArray = new string[nameArray.Length, _target.nodeCount];
            GameObject writerEmpty = new GameObject();
            writerEmpty.name = "JsonWriter_empty";
            writerEmpty.transform.parent = _target.transform;
            for (int i = 0; i < _target.nodeCount; i++)
            {
                //位置信息写入
                writerEmpty.transform.position = _target.nodes[i];
                writerEmpty.transform.eulerAngles = _target.rotationList[i];
                contentArray[0, i] = writerEmpty.transform.localPosition.x + "," + writerEmpty.transform.localPosition.y + "," + writerEmpty.transform.localPosition.z;
                //角度信息写入
                contentArray[1, i] = writerEmpty.transform.localEulerAngles.x + "," + writerEmpty.transform.localEulerAngles.y + "," + writerEmpty.transform.localEulerAngles.z;
                //视域信息写入
                contentArray[2, i] = _target.cameraViewList[i].ToString();
				//绝对位置信息写入
				contentArray[3, i] = writerEmpty.transform.position.x + "," + writerEmpty.transform.position.y + "," + writerEmpty.transform.position.z;
				//绝对角度信息写入
				contentArray[4, i] = writerEmpty.transform.eulerAngles.x + "," + writerEmpty.transform.eulerAngles.y + "," + writerEmpty.transform.eulerAngles.z;
            }
            GameObject.DestroyImmediate(writerEmpty);
            jsonOp.JsonWriter(filePath, _target.pathName, nameArray, contentArray, false);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            jsonOp.JsonWriter(tempPath, _target.pathName, nameArray, contentArray, false);
            Debug.Log(_target.pathName + "---路径写入成功！");
        }
        else
        {
            Debug.LogError(dirPath + ", 该路径不存在，请在运行模式下保存！");
        }
    }

    //Json文件加载
    private void JsonLoad(bool is_relative)
    {
        string filePath = Application.dataPath + "/_TempExcel/TempPathInfo.json";
        if (File.Exists(filePath))
        {
            JsonOperator jsonOp = new JsonOperator();
            DataTable jsonTable = jsonOp.JsonReader(filePath, _target.pathName);
            if (jsonTable == null)
            {
                Debug.LogError(_target.pathName + ", 该路径名称不存在！");
                return;
            }
			if (is_relative)  //加载相对路径
			{
				GameObject loadEmpty = new GameObject();
				loadEmpty.name = "JsonLoad_empty";
				loadEmpty.transform.parent = _target.transform;
				_target.nodes.Clear();
				_target.rotationList.Clear();
				_target.cameraViewList.Clear();
				_target.displayInfoList.Clear();
				_target.handleDisplay.Clear();
				for (int i = 0; i < jsonTable.Rows.Count; i++)
				{
					loadEmpty.transform.localPosition = ConvertToVector3((string)jsonTable.Rows[i][0].ToString());
					loadEmpty.transform.localEulerAngles = ConvertToVector3((string)jsonTable.Rows[i][1].ToString());
					_target.nodes.Add(loadEmpty.transform.position);
					_target.rotationList.Add(loadEmpty.transform.eulerAngles);
					_target.cameraViewList.Add(float.Parse((string)jsonTable.Rows[i][2].ToString()));
					_target.displayInfoList.Add(false);
					_target.handleDisplay.Add(true);
				}
				GameObject.DestroyImmediate(loadEmpty);
			}
			else  //加载绝对路径
			{
				if (jsonTable.Columns.Count < 5) {
					Debug.LogError(_target.pathName + ", 该路径名称不存在绝对位置，请检查相应的数据表格！");
					return;
				}
				_target.nodes.Clear();
				_target.rotationList.Clear();
				_target.cameraViewList.Clear();
				_target.displayInfoList.Clear();
				_target.handleDisplay.Clear();
				for (int i = 0; i < jsonTable.Rows.Count; i++)
				{
					_target.nodes.Add(ConvertToVector3((string)jsonTable.Rows[i][3].ToString()));
					_target.rotationList.Add(ConvertToVector3((string)jsonTable.Rows[i][4].ToString()));
					_target.cameraViewList.Add(float.Parse((string)jsonTable.Rows[i][2].ToString()));
					_target.displayInfoList.Add(false);
					_target.handleDisplay.Add(true);
				}
				//移动到起点
				//_target.transform.position = _target.nodes[0];
				//_target.transform.eulerAngles = _target.rotationList[0];
				//if (_target.camera != null)
				//{
				//	_target.CameraView = _target.cameraViewList[0];
				//}
			}
            _target.nodeCount = _target.nodes.Count;
            AngleOptimization();
        }
        else
        {
            Debug.LogError(filePath + ", 该文件不存在！");
        }
    }

    //string to vector3
    private Vector3 ConvertToVector3(string vec_string)
    {
        string[] stringArray = vec_string.Split(',');
        Vector3 rVec = Vector3.zero;
        rVec.x = float.Parse(stringArray[0]);
        rVec.y = float.Parse(stringArray[1]);
        rVec.z = float.Parse(stringArray[2]);
        return rVec;
    }

}
