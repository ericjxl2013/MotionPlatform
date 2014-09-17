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
	private string isRelativePathStr = "�������·��";

	private static GUIContent
		insertContent = new GUIContent("+", "���ӿ��ƶ���"),
		deleteContent = new GUIContent("-", "ɾ����ǰ���ƶ���"),
		focusContent = new GUIContent("F", "ֻ�鿴��ǰ���Ƶ�"),
		transContent = new GUIContent("T", "�ƶ���ǰ���嵽�õ�"),
		modifyContent = new GUIContent("M", "�޸ĵ�ǰ��Ϊ��ǰ����״̬");
	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(25f);
		//focusWidth = GUILayout.MaxWidth(50f);

	void OnEnable()
	{
		//������
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
		//��ʼ��
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

	//�Ƿ���ʾHandlePosition
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

	//�Ƿ���ʾHandlePosition
	private void HandleDisplayControl(bool flag)
	{
		for (int i = 0; i < _target.handleDisplay.Count; i++)
		{
			_target.handleDisplay[i] = flag;
		}
	}

	public override void OnInspectorGUI()
	{
		//·���Ƿ�ɼ�
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Visible");
		_target.pathVisible = EditorGUILayout.Toggle(_target.pathVisible);
		EditorGUILayout.EndHorizontal();

		//·���Ƿ�ɼ�
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

		//·������
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Name");
		_target.pathName = EditorGUILayout.TextField(_target.pathName);
		EditorGUILayout.EndHorizontal();

		//·����ɫ
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");
		_target.pathColor = EditorGUILayout.ColorField(_target.pathColor);
		EditorGUILayout.EndHorizontal();

		//�������Ƶ���
		EditorGUILayout.BeginHorizontal();
		//EditorGUILayout.PrefixLabel("Node Count");
		_target.nodeCount = Mathf.Max(2, EditorGUILayout.IntField("Node Count", _target.nodeCount));
		//_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider(_target.nodeCount, 0, 10), 2,100);
		EditorGUILayout.EndHorizontal();

		//���ӽڵ�
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
		//�Ƴ��ڵ�
		else if (_target.nodeCount < _target.nodes.Count)
		{
			if (EditorUtility.DisplayDialog("�Ƴ��ڵ㾯�棡", "ɾ���ڵ��ýڵ��������·�������������Ҳ��ɻָ����Ƿ�ȷ�ϼ���ɾ����", "ȷ��", "ȡ��"))
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

		//�ڵ�״̬��Ϣ��ʾ
		for (int i = 0; i < _target.nodes.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("��" + i + "��");
			_target.displayInfoList[i] = EditorGUILayout.Toggle(_target.displayInfoList[i]);

			if (GUILayout.Button(focusContent, EditorStyles.miniButtonLeft, buttonWidth))  //���Ƶ�
			{
				HandleDisplayControl(i);
			}
			if (GUILayout.Button(transContent, EditorStyles.miniButtonMid, buttonWidth))  //�ƶ�
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
			if (GUILayout.Button(modifyContent, EditorStyles.miniButtonMid, buttonWidth))  //�޸�
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
			if (GUILayout.Button(insertContent, EditorStyles.miniButtonMid, buttonWidth))  //����
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
			if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))  //ɾ��
			{
				if (EditorUtility.DisplayDialog("�Ƴ��ڵ㾯�棡", "ɾ���ڵ��ýڵ��������·�������������Ҳ��ɻָ����Ƿ�ȷ�ϼ���ɾ����", "ȷ��", "ȡ��"))
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
			//��ʾλ�õ�״̬��Ϣ
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

		if (GUILayout.Button("�����ֱ�ȫ����ʾ"))
		{
			HandleDisplayControl(true);
		}

		if (GUILayout.Button("��Ϊ��ʼ״̬"))
		{
			GetInitialState();
		}

		if (GUILayout.Button("���Ƶ���Ϣ����"))
		{
            JsonWirter();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("���·��");
		_target.isRelativePos = EditorGUILayout.Toggle(_target.isRelativePos);
		if (_target.isRelativePos)
			isRelativePathStr = "�������·��";
		else
			isRelativePathStr = "���ؾ���·��";
		if (GUILayout.Button(isRelativePathStr, GUILayout.MaxWidth(260f)))
		{
			JsonLoad(_target.isRelativePos);
		}
		EditorGUILayout.EndHorizontal();

		//if (GUILayout.Button("���ؿ��Ƶ���Ϣ"))
		//{
		//	JsonLoad();
		//}

		//����������
		//·���Ƿ�ɼ�
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
			//���������ٶ�
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Speed");
			_target.moveSpeed = EditorGUILayout.FloatField(_target.moveSpeed);
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("��ʼ�˶�"))
			{
				_target.MotionActive();
			}
		}

		//����и��£������»���
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
				//����������Unity�ڲ�����
				Undo.RecordObject(_target, "Adjust iTween Path");
				//��ʼ��λ�ÿ���
				//if (_target.followTarget && !_target.isSlider && !_target.moveFlag)
				//	_target.nodes[0] = _target.transform.position;
				//λ���ֱ���ʾ
				for (int i = 0; i < _target.nodes.Count; i++)
				{
					if (i != _target.nodes.Count - 1)
					{
						Handles.Label(_target.nodes[i], "�� " + i + "��", style);
						if (_target.handleDisplay[i])
							_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
					}
					else
					{
						Handles.Label(_target.nodes[i], "'" + _target.pathName + "' ����", style);
						if (_target.handleDisplay[i])
							_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
					}
				}
			}
		}
	}

	//����Target��ʼ״̬
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

	//����������λ�ò�ֵ
	private void SceneInterp(float current_value)
	{
		if (_target.sliderLock != current_value)
		{
			if (Mathf.Approximately(current_value, 0f))  //0��
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

	//�Ƕ�ֵ�Ż�����Ϊ�Ƕȷǳ���ȷ����������90��-270֮��
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

	//�Ƕ��޸�ʹ�����ʵ�ʣ�����-90��270�����������
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

    //Jsonд��
    private void JsonWirter()
    {
        string dirPath = MotionPara.taskRootPath + MotionPara.taskName;
        string filePath = dirPath + "/PathControl.json";
        string tempDir = Application.dataPath + "/_TempExcel";
        string tempPath = tempDir + "/TempPathInfo.json";
        if (Directory.Exists(dirPath))
        {
            //�ƶ������
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
                //λ����Ϣд��
                writerEmpty.transform.position = _target.nodes[i];
                writerEmpty.transform.eulerAngles = _target.rotationList[i];
                contentArray[0, i] = writerEmpty.transform.localPosition.x + "," + writerEmpty.transform.localPosition.y + "," + writerEmpty.transform.localPosition.z;
                //�Ƕ���Ϣд��
                contentArray[1, i] = writerEmpty.transform.localEulerAngles.x + "," + writerEmpty.transform.localEulerAngles.y + "," + writerEmpty.transform.localEulerAngles.z;
                //������Ϣд��
                contentArray[2, i] = _target.cameraViewList[i].ToString();
				//����λ����Ϣд��
				contentArray[3, i] = writerEmpty.transform.position.x + "," + writerEmpty.transform.position.y + "," + writerEmpty.transform.position.z;
				//���ԽǶ���Ϣд��
				contentArray[4, i] = writerEmpty.transform.eulerAngles.x + "," + writerEmpty.transform.eulerAngles.y + "," + writerEmpty.transform.eulerAngles.z;
            }
            GameObject.DestroyImmediate(writerEmpty);
            jsonOp.JsonWriter(filePath, _target.pathName, nameArray, contentArray, false);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            jsonOp.JsonWriter(tempPath, _target.pathName, nameArray, contentArray, false);
            Debug.Log(_target.pathName + "---·��д��ɹ���");
        }
        else
        {
            Debug.LogError(dirPath + ", ��·�������ڣ���������ģʽ�±��棡");
        }
    }

    //Json�ļ�����
    private void JsonLoad(bool is_relative)
    {
        string filePath = Application.dataPath + "/_TempExcel/TempPathInfo.json";
        if (File.Exists(filePath))
        {
            JsonOperator jsonOp = new JsonOperator();
            DataTable jsonTable = jsonOp.JsonReader(filePath, _target.pathName);
            if (jsonTable == null)
            {
                Debug.LogError(_target.pathName + ", ��·�����Ʋ����ڣ�");
                return;
            }
			if (is_relative)  //�������·��
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
			else  //���ؾ���·��
			{
				if (jsonTable.Columns.Count < 5) {
					Debug.LogError(_target.pathName + ", ��·�����Ʋ����ھ���λ�ã�������Ӧ�����ݱ��");
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
				//�ƶ������
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
            Debug.LogError(filePath + ", ���ļ������ڣ�");
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
