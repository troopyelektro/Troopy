/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 08.07.2019
 * Time: 5:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Troopy
{	
	public abstract class AppBase
	{
		private static bool useOutputFileLog = false;
		private static bool useOutputTextbox = false;
		
		private static List<DateTime> outDateTime;
		private static List<string> outMessage;
		
		private static TextBox output;
		private static StreamWriter outSw;
		private static bool outSwFirstTime = true;
		

		public static void setOutputFileLog(bool enabled)
		{
			if(outSwFirstTime && enabled) {
				outSwFirstTime = false;
				outSw = new StreamWriter(getFileLogNewFilename());
			}
			useOutputFileLog = enabled;
		}
		
		public static void setOutputTextBox(TextBox tb)
		{
			AppBase.output = tb;
			useOutputTextbox = true;
		}
		
		public static void addOutput(string message)
		{
			if(outDateTime==null) {
				outDateTime = new List<DateTime>();
				outMessage = new List<string>();
			}
			
			var d = new DateTime();
			d = DateTime.Now;
			outDateTime.Add(d);
			outMessage.Add(message);
			
			if(useOutputTextbox) {
				output.AppendText(Environment.NewLine);
				output.AppendText("[" + d.ToString() + "] " + message);
				output.ScrollToCaret();
			}
			
			if(useOutputFileLog) {
				outSw.WriteLine("[" + d.ToString() + "] " + message);
			}
			
		}
		
		public static void endOutput()
		{
			if(!outSwFirstTime) {
				outSw.Close();
			}
		}
		
		public static void clearOutput()
		{
			output.Clear();
		}
		
		public static string getAppPath()
		{
			return Application.StartupPath;
		}

		public static string getFileLogDirectory()
		{
			string d = getAppPath() + "\\Logs";
			if (!Directory.Exists(d)) {
				Directory.CreateDirectory(d);
			}
			return d;
		}
		
		public static string getFileLogNewFilename()
		{
			DateTime d = DateTime.Now;
			string ds = d.ToString("yyyy_MM_dd_hh_mm");
			int fcntI = 0;
			string fcntSfx = "";
			string fileName = getFileLogDirectory() + "\\" +ds+ ".txt";
			while(File.Exists(fileName)) {
				fcntSfx = "_" + new Conversion(fcntI).getString(0,3,"0");
				fileName = getFileLogDirectory() + "\\" + ds +  fcntSfx + ".txt";
				fcntI++;
			}
			return fileName;
		}
		
		// Handle expception
		public static void UIThreadException(object sender, ThreadExceptionEventArgs t)
		{
			logException(t.Exception);
		}
		public static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			logException((Exception)e.ExceptionObject);
		}
		
		public static void logException(Exception ex)
		{
			frmException f = new frmException();
			f.ShowDialog();
			addOutput(
				Environment.NewLine + 
				Environment.NewLine + "   Došlo k neočekávané chybě v programu. Prosím předejte vývojaři poslední log soubor, nebo následující text:" +
				Environment.NewLine + "   Typ vyjímky: " + ex.GetType().ToString() +
				Environment.NewLine + "   Zpráva: " + ex.Message.ToString() +
				Environment.NewLine +
			    Environment.NewLine + "   Výpis zásobníku volaných funkcí:" +
			    Environment.NewLine +
			    Environment.NewLine + ex.StackTrace.ToString() +
			    Environment.NewLine
			);
		}
	}
	
	// ==========
	// EXCEPTIONS
	// ==========
	public class TroopyException : Exception {
		
		public TroopyException(string message) : base(message)
   		{
   		
   		}
		
   	}	
}
