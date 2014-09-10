/// <summary>
/// <Filename>: VisualClass.cs
/// Author: Jiang Xiaolong
/// Created: 2014.08.31
/// Version: 1.0
/// Company: Sunnytech
/// Function: 可视化路径（Catmull–Rom Spline）
///
/// Changed By:
/// Modification Time:
/// Discription:
/// <summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CRSpline
{
	//控制顶点列表
	public Vector3[] pts;
    //控制顶点List
    public List<Vector3> controlPoints = new List<Vector3>();
    //旋转角度
    public List<Vector3> rotationList = new List<Vector3>();
    //View参数
    public List<float> cameraViewList = new List<float>();
    //时间片段
    public float[] segmentTimeRate;

    public CRSpline()
    {
        
    }

	public CRSpline(params Vector3[] pts)
	{
		this.pts = new Vector3[pts.Length];
		System.Array.Copy(pts, this.pts, pts.Length);
	}

    //控制顶点生成
    public Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return vector3s;
    }

    //获得时间
    public float TimeGet(Vector3[] path, float move_speed)
    {
        float distance = 0.0f;
        Vector3 prevPt = Interp(0);
        int SmoothAmount = path.Length * 20;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(pm);
            distance += (currPt - prevPt).magnitude;
            prevPt = currPt;
        }
        return distance / move_speed;
    }

    //角度和View插值时时间片段计算
    public void TimeRateGenerator(int count)
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

	//样条插值
	public Vector3 Interp(float t)
	{
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float u = t * (float)numSections - (float)currPt;

		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}

    //角度插值
    public Vector3 EulerAngleLerp(float time_rate, ref int current_index, ref float current_time_rate)
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
    public float ViewLerp(float time_rate)
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

	//样条的一阶导数，斜率
	public Vector3 Velocity(float t)
	{
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float u = t * (float)numSections - (float)currPt;

		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
				+ (2f * a - 5f * b + 4f * c - d) * u
				+ .5f * c - .5f * a;
	}

	//Scene场景中画线
	public void GizmoDraw(float t)
	{
		Gizmos.color = Color.white;
		Vector3 prevPt = Interp(0);

		for (int i = 1; i <= 20; i++)
		{
			float pm = (float)i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}

		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
}

public class VisualClass
{
	/// <summary>
	/// 画线入口
	/// </summary>
	/// <param name="path">路径控制顶点数组</param>
	/// <param name="color">路径颜色</param>
	public static void DrawPath(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(path, color, "gizmos");
		}
	}

	private static void DrawPathHelper(Vector3[] path, Color color, string method)
	{
		Vector3[] vector3s = PathControlPointGenerator(path);
		//Line Draw:
		Vector3 prevPt = Interp(vector3s, 0);
		Gizmos.color = color;
		int SmoothAmount = path.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float pm = (float)i / SmoothAmount;
			Vector3 currPt = Interp(vector3s, pm);
			if (method == "gizmos")
			{
				Gizmos.DrawLine(currPt, prevPt);
			}
			else if (method == "handles")
			{
				Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
				//UnityEditor.Handles.DrawLine(currPt, prevPt);
			}
			prevPt = currPt;
		}
	}

	public static float TimeGet(Vector3[] path, float move_speed)
	{
		float distance = 0.0f;
		Vector3 prevPt = Interp(path, 0);
		int SmoothAmount = path.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float pm = (float)i / SmoothAmount;
			Vector3 currPt = Interp(path, pm);
			distance += (currPt - prevPt).magnitude;
			prevPt = currPt;
		}
		return distance / move_speed;
	}

	public static void TestMove(Vector3[] path, Transform target, float rate)
	{
		target.position = Interp(path, rate);
	}

	//控制顶点生成
	public static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		Vector3[] suppliedPath;
		Vector3[] vector3s;

		//create and store path points:
		suppliedPath = path;

		//populate calculate path;
		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length + offset];
		Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if (vector3s[1] == vector3s[vector3s.Length - 2])
		{
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
			tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
			tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
			vector3s = new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
		}
		return vector3s;
	}

	//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
	public static Vector3 Interp(Vector3[] pts, float t)
	{
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float u = t * (float)numSections - (float)currPt;

		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}

	//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
	private class CRSpline
	{
		public Vector3[] pts;

		public CRSpline(params Vector3[] pts)
		{
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}


		public Vector3 Interp(float t)
		{
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
			float u = t * (float)numSections - (float)currPt;
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
			return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
		}
	}
}
