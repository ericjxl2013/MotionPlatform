#define DEBUG_LEVEL_LOG
#define DEBUG_LEVEL_WARN
#define DEBUG_LEVEL_ERROR
using UnityEngine;
using System;
using System.Collections;

// 自定义Debug类
// setting the conditional to the platform of choice will only compile the method for that platform
// alternatively, use the #defines at the top of this file
public class D
{
	[System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
	public static void LogFormat (object format, params object[] paramList)
	{
		if (format is string)
			Debug.Log (string.Format (format as string, paramList));
		else
			Debug.Log (format);
	}
	
	[System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
	public static void Log (object format)
	{
		if (format is string || format is int)
			Debug.Log (format.ToString ());
		else if (format is Vector2) {
			Vector2 tempVec2 = (Vector2)format;
			Debug.Log (Math.Round (tempVec2.x, 6).ToString () + "," + Math.Round (tempVec2.y, 6).ToString ());
		} else if (format is Vector3) {
			Vector3 tempVec3 = (Vector3)format;
			Debug.Log (Math.Round (tempVec3.x, 6).ToString () + "," + Math.Round (tempVec3.y, 6).ToString () +
				"," + Math.Round (tempVec3.z, 6).ToString ());
		} else if (format is float) {
			Debug.Log (Math.Round ((float)format, 6));
		} else if (format is double) {
			Debug.Log (Math.Round ((double)format, 6));
		}else {
			Debug.Log (format);
		}
	}
	
	[System.Diagnostics.Conditional("DEBUG_LEVEL_WARN")]
	public static void WarnFormat (object format, params object[] paramList)
	{
		if (format is string)
			Debug.LogWarning (string.Format (format as string, paramList));
		else
			Debug.LogWarning (format);
	}
	
	[System.Diagnostics.Conditional("DEBUG_LEVEL_WARN")]
	public static void Warn (object format)
	{
		Debug.LogWarning (format);
	}
	
	[System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
	public static void ErrorFromat (object format, params object[] paramList)
	{
		if (format is string)
			Debug.LogError (string.Format (format as string, paramList));
		else
			Debug.LogError (format);
	}
	
	[System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
	public static void Error (object format, params object[] paramList)
	{
		Debug.LogError (format);
	}
    //This is a test line.
}