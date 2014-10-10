using UnityEngine;
using System.Collections;

using System; 
using System.Data; 
using System.Data.Odbc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System;

public class readExcel {

	// Use this for initialization
	
	public ArrayList mmenu_top_array = new ArrayList();
	public ArrayList mmenu_top_array2 = new ArrayList();
	
	public string []R_a;
	public string []R_b;
	public string []R_c;
	public string []R_d;
	public string []tips2;
	
	public int count,R_count;

	
	
	public void start(){

			//readXLS(Application.dataPath+"/Resources/menu.xls");
	
	}
	
/*public void tt() {
		Debug.Log("!!!");
        readXLS(Application.dataPath + "/Book2.xls");
	
    }
 */   
    // Update is called once per frame
    void Update () {
    
    }
    
    public void readXLS( string filetoread, string shhet_name)
    {

		mmenu_top_array.Clear();
		mmenu_top_array2.Clear();

        // Must be saved as excel 2003 workbook, not 2007, mono issue really
		filetoread = Application.dataPath+filetoread;
		
        string con = "Driver={Microsoft Excel Driver (*.xls)}; DriverId=790; Dbq="+filetoread+";";
   //    Debug.Log(con);
		string yourQuery = "SELECT * FROM ["+shhet_name+"$A:D];"; 
        // our odbc connector 
        OdbcConnection oCon = new OdbcConnection(con); 
		
		//Debug.Log("in"+"一二一");
		
		
        // our command object 
        OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
		
		//Debug.Log("out");
		
	
        // table to hold the data 
       	DataTable dtYourData = new DataTable("YourData"); 
        // open the connection 
        oCon.Open(); 
        // lets use a datareader to fill that table! 
        OdbcDataReader rData = oCmd.ExecuteReader(); 
        // now lets blast that into the table by sheer man power! 
        dtYourData.Load(rData); 
		
		
        // close that reader! 
        rData.Close(); 
        // close your connection to the spreadsheet! 
        oCon.Close(); 
        // wow look at us go now! we are on a roll!!!!! 
        // lets now see if our table has the spreadsheet data in it, shall we?
		
		count=dtYourData.Rows.Count;
//		Debug.Log (count+"PPPPPP");
        if(dtYourData.Rows.Count > 0) 
        { 
			//int nCount = 0;
            // do something with the data here 
            // but how do I do this you ask??? good question! 
            for(int i = 0; i < dtYourData.Rows.Count; i++) 
            { 
				string [] a = new string[3];
				a[0] = dtYourData.Rows[i][dtYourData.Columns[1].ColumnName].ToString();
				a[1] = dtYourData.Rows[i][dtYourData.Columns[2].ColumnName].ToString();
				a[2] = dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
				
				//如果是下一个目录
				if(dtYourData.Rows[i][dtYourData.Columns[0].ColumnName].ToString() == "0" && i != 0)
				{
					mmenu_top_array.Add(mmenu_top_array2);
					mmenu_top_array2 = new ArrayList();
				}
				mmenu_top_array2.Add(a);
				if(i == dtYourData.Rows.Count - 1)
				{
					mmenu_top_array.Add(mmenu_top_array2);
				}
				//tips1[i]=dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
	
            } 
        } 
    
	//Debug.Log("here!!!!!!!!!!!!");

	}

	public void readJson( string filetoread, string shhet_name)
	{
		
		mmenu_top_array.Clear();
		mmenu_top_array2.Clear();
		
		// Must be saved as excel 2003 workbook, not 2007, mono issue really
		filetoread = Application.dataPath+filetoread;

		JsonOperator jo2 = new JsonOperator();
		DataTable dtYourData = jo2.JsonReader(filetoread, shhet_name);
		
		count=dtYourData.Rows.Count;
		//		Debug.Log (count+"PPPPPP");
		if(dtYourData.Rows.Count > 0) 
		{ 
			//int nCount = 0;
			// do something with the data here 
			// but how do I do this you ask??? good question! 
			for(int i = 0; i < dtYourData.Rows.Count; i++) 
			{ 
				string [] a = new string[3];
				a[0] = dtYourData.Rows[i][dtYourData.Columns[1].ColumnName].ToString();
				a[1] = dtYourData.Rows[i][dtYourData.Columns[2].ColumnName].ToString();
//				a[2] = dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
				a[2] = "";
				
				//如果是下一个目录
				if(dtYourData.Rows[i][dtYourData.Columns[0].ColumnName].ToString() == "0" && i != 0)
				{
					mmenu_top_array.Add(mmenu_top_array2);
					mmenu_top_array2 = new ArrayList();
				}
				mmenu_top_array2.Add(a);
				if(i == dtYourData.Rows.Count - 1)
				{
					mmenu_top_array.Add(mmenu_top_array2);
				}
				//tips1[i]=dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
				
			} 
		} 
		
		//Debug.Log("here!!!!!!!!!!!!");
		
	}

	public void readXLSsheet2(string filetoread)
	{
		filetoread = Application.dataPath+filetoread;
	 	string con = "Driver={Microsoft Excel Driver (*.xls)}; DriverId=790; Dbq="+filetoread+";";
   //    Debug.Log(con);
        string yourQuery = "SELECT * FROM [Sheet2$]"; 
         // our odbc connector 
        OdbcConnection oCon = new OdbcConnection(con); 
		
		//Debug.Log("in"+"一二一");
		
		
        // our command object 
        OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
		
		//Debug.Log("out");
		
        // table to hold the data 
       	DataTable dtYourData = new DataTable("YourData"); 
		
        // open the connection 
        oCon.Open(); 
         // lets use a datareader to fill that table! 
        OdbcDataReader rData = oCmd.ExecuteReader(); 
        // now lets blast that into the table by sheer man power! 
        dtYourData.Load(rData); 
        // close that reader! 
        rData.Close(); 
        // close your connection to the spreadsheet! 
        oCon.Close(); 
        // wow look at us go now! we are on a roll!!!!! 
        // lets now see if our table has the spreadsheet data in it, shall we?
		R_a  =new string[dtYourData.Rows.Count];
		R_b  =new string[dtYourData.Rows.Count];
		R_c  =new string[dtYourData.Rows.Count];
		R_d  =new string[dtYourData.Rows.Count];
		tips2=new string[dtYourData.Rows.Count];

		R_count=dtYourData.Rows.Count;
        if(dtYourData.Rows.Count > 0) 
        { 
            // do something with the data here 
            // but how do I do this you ask??? good question! 
            for (int i = 0; i < dtYourData.Rows.Count; i++) 
            { 
				R_a[i]=dtYourData.Rows[i][dtYourData.Columns[0].ColumnName].ToString();
				R_b[i]=dtYourData.Rows[i][dtYourData.Columns[1].ColumnName].ToString();
				R_c[i]=dtYourData.Rows[i][dtYourData.Columns[2].ColumnName].ToString();
				R_d[i]=dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
			//	tips2[i]=dtYourData.Rows[i][dtYourData.Columns[3].ColumnName].ToString();
				
				
				
				
		//	Debug.Log (R_c[i]);
			}   
        } 
	}
}
