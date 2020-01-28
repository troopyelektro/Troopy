using System;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace Troopy
{
	public class Excel : Grid
	{
		// OLE DB Excel Connection;
		OleDbConnection connection;
		
		// Excel format types list
		public const int excelFormatsCount = 2;
		public enum excelFormats{
			OldXls,
			NewXlsx
		}
		
		// Selected excel format
		protected int format = (int)excelFormats.NewXlsx;
		public int Format{
			get{
				return this.format;
			}
			set{
				int iValue = (int)value;
				if((iValue >= 0) && (iValue < Excel.excelFormatsCount)) {
					this.format = iValue;
				}
			}
		}
		
		// FileName
		protected string fileName;
		public string FileName{
			get{
				return this.fileName;
			}
        	set{
        		if(File.Exists(value)) {
        			this.fileName = value;
        		}
        	}			
		}
		
		// Constructor
		public Excel(string fileName, int format)
		{
			this.FileName = fileName;
			this.Format = format;
			this.load();
		}
		
		// Open and load file
		public Excel load()
		{
			// Check if fileName exists
			if(!File.Exists(this.FileName)) {
				throw new ExcelFileNotFoundException("Excelový soubor " + fileName + " nebyl nalezen");
			}
			
			// Open excel file
			string excelProcessor = "";
			switch(format) {
				case (int)excelFormats.OldXls:
					excelProcessor = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName +
                            		 ";Extended Properties=\"Excel 8.0;HDR=Yes;\";";
					break;
				case (int)excelFormats.NewXlsx:
					excelProcessor = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName +
                    				 ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;\";";
					break;
			}
			
			try{
				this.connection = new OleDbConnection(excelProcessor);
				this.connection.Open();
				
				DataTable sheetList = this.connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
				foreach(DataRow dr in sheetList.Rows) {
					
					string sheetName = dr.ItemArray[2].ToString();
					sheetName = sheetName.Substring(0, sheetName.Length-1);

					OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * from [" + sheetName + "$]", this.connection);
					
					DataTable dt = new DataTable();
					adapter.Fill(dt);
					
					this.addTable(dt, sheetName);
				}
			}
			catch (Exception ex) {
				throw new Exception("Nesprávně zvolený excel soubor: " + ex.Message);
			}
			
			return this;
		}
	}


	
	// ==========
	// EXCEPTIONS
	// ==========
	public class ExcelException : TroopyException
	{
		public ExcelException(string message) : base(message)
   		{
   		}
   	}

	public class ExcelFileNotFoundException : ExcelException
	{
		public ExcelFileNotFoundException(string message) : base(message)
   		{
   		}
   	}

	public class ExcelInvalidFormatException : ExcelException
	{
		public ExcelInvalidFormatException(string message) : base(message)
   		{
   		}	
	}
}
