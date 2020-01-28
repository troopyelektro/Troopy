using System;
using MsExcel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;

namespace Troopy
{
	public class ExcelInterop
	{
		protected static MsExcel.Application excel;
		protected MsExcel.Workbook wb;
		
		public ExcelInterop(string fileName = null)
		{
			if(ExcelInterop.excel == null) {
				try {
					ExcelInterop.excel = new MsExcel.Application();
					ExcelInterop.excel.Visible = false;
					ExcelInterop.excel.DisplayAlerts = false;			
				} catch(COMException ex) {
					AppBase.addOutput("Nebylo možné otevřít aplikaci Excel");
					throw new ExcelInteropException(ex.Message);
				}	
			}

			if(fileName != null) {
				// Open Excel file
				this.load(fileName);
			}
		}
		
		protected ExcelInterop load(string fileName)
		{
			try{
				this.wb = excel.Workbooks.Open(fileName);
				return this;
			} catch(COMException ex) {
				AppBase.addOutput("Nebylo možné otevřít soubor " + fileName);
				throw new ExcelInteropException(ex.Message);
			}			
		}
		
		protected MsExcel.Worksheet getWorksheet(string sheetName)
		{
			foreach(MsExcel.Worksheet ws in this.wb.Worksheets) {
				if(ws.Name == sheetName) {
					return ws;
				}
			}
			
			throw new ExcelInteropException(
				"Excel - List " + sheetName + " nebyl nalezen."
			);
		}
		
		protected ExcelInterop saveAs(string fileName)
		{
			try{
				// Check, if output path exists. Create if not exists
				string dirName = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(dirName)) {
					Directory.CreateDirectory(dirName);
				}

				// Save excel file
				this.wb.SaveAs(
					fileName,
					MsExcel.XlFileFormat.xlWorkbookDefault,
					Type.Missing,
					Type.Missing,
					Type.Missing,
					false,
					MsExcel.XlSaveAsAccessMode.xlNoChange,
					MsExcel.XlSaveConflictResolution.xlLocalSessionChanges,
					Type.Missing,
					Type.Missing
				);
				return this;
			} catch(COMException ex) {
				AppBase.addOutput("Nebylo možné uložit soubor " + fileName);
				throw new ExcelInteropException(ex.Message);
			}						
		}
		
		protected ExcelInterop close()
		{
			this.wb.Close();
			Marshal.ReleaseComObject(this.wb);
			return this;
		}
		
		public static void quit()
		{
			if(ExcelInterop.excel != null) {
				ExcelInterop.excel.Quit();
				Marshal.ReleaseComObject(excel);
				ExcelInterop.excel = null;
			}
		}
	}
	
	public class ExcelInteropException : TroopyException
	{
		public ExcelInteropException (string message) : base(message)
   		{
   		}
	}
}
