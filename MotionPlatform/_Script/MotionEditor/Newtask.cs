using UnityEngine;
using System.Collections;
using System.IO;

public class Newtask : MonoBehaviour {



	//新建任务
	public void createTask(string taskName){
		
		//创建文件夹
		string foleName = Application.dataPath + "/Resources/" +taskName;
		
		System.IO.Directory.CreateDirectory(foleName);

		foleName = Application.dataPath + "/Resources/" +taskName +"/C";
		
		System.IO.Directory.CreateDirectory(foleName);
		
		foleName = Application.dataPath + "/Resources/" +taskName +"/Y";
		
		System.IO.Directory.CreateDirectory(foleName);
		
		foleName = Application.dataPath + "/Resources/" +taskName +"/Z";
		
		System.IO.Directory.CreateDirectory(foleName);

		//拷贝Excel文件
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/C1.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/C/C1.xls");
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/CID.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/C/CID.xls");
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/Tools.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/Tools.xls");
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/ProgramMotionID.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/ProgramMotionID.xls");
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/ObjectName.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/ObjectName.xls");
		CopyFile(Application.dataPath +"/StreamingAssets/ExcelMotionData/ToolsVariable.xls"
		         ,Application.dataPath + "/Resources/" +taskName +"/ToolsVariable.xls");
	}

	void CopyFile(string src_path, string tar_oath){
		string path = src_path;
		string path2 = tar_oath;
		FileInfo fi1 = new FileInfo(path);
		FileInfo fi2 = new FileInfo(path2);
		
		try 
		{
			// Create the file and clean up handles.
			using (FileStream fs = fi1.Create()) {}
			
			//Ensure that the target does not exist.
			fi2.Delete();
			
			//Copy the file.
			fi1.CopyTo(path2);
			
			//Try to copy it again, which should succeed.
			fi1.CopyTo(path2, true);

		} 
		catch 
		{

		}
	}


}
