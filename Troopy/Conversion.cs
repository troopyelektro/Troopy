using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

namespace Troopy
{
	/// <summary>
	/// V1.00		11.12.2019
	/// - Filled Missing Conversion to double in constructor
	/// - public Conversion(float value)
	/// - public Conversion(double value)
	/// </summary>
	public class Conversion : DataSize
	{   
		public const string inValueTypeString = "string";
		public const string inValueTypeInt = "int";
		public const string inValueTypeLong = "long";
		public const string inValueTypeFloat = "float";
		public const string inValueTypeDouble = "double";
		
		private string inValueString;
		private int inValueInt;
		private long inValueLong;
		private float inValueFloat;
		private double inValueDouble;
		private string inValueType;
		
		public bool conversionOk;
		
		public Conversion(string value)
		{
			this.inValueType = inValueTypeString;
			this.inValueString = value;
			
			// value without whitespace
			string vww = Conversion.removeWhitespace(value);
			
			// Get multiplier
			double m = getMultiplier(vww);
			
			if(m == 0) {
				// Multiplier is not used
				if(Double.TryParse(vww.Replace('.',','), out inValueDouble)) {
					conversionOk = true;
				} else {
					conversionOk = false;
				}
			} else {
				// Multiplier is used
				string strNumber = vww.Substring(0, vww.Length - multiplierUnit[lastMultiplierFoundIndex].Length);
				if(Double.TryParse(strNumber.Replace('.',','), out inValueDouble)) {
					inValueDouble = inValueDouble * m;
					conversionOk = true;
				} else {
				   	conversionOk = false;
				}
			}
		}
		
		public Conversion(int value)
		{
			this.inValueType = inValueTypeInt;
			this.inValueInt = value;
			Double.TryParse(value.ToString(), out inValueDouble);
			// Create double 
		}

		public Conversion(long value)
		{
			this.inValueType = inValueTypeLong;
			this.inValueLong = value;
			Double.TryParse(value.ToString(), out inValueDouble);
			// Create double 
		}
		
		public Conversion(float value)
		{
			this.inValueType = inValueTypeFloat;
			this.inValueFloat = value;
			
			// Create double
			Double.TryParse(value.ToString(), out inValueDouble);			
		}		
		
		public Conversion(double value)
		{
			this.inValueType = inValueTypeDouble;
			// Create double
			this.inValueDouble = value;						
		}
		
		/* Deletes WhiteSpace Characters on left and right side of string
		 * @param string value ... Text to be evaluated
		 * @retrun string      ... Result */
		public static string removeWhitespace(string value)
		{
			return value.Replace(" ", String.Empty)
				        .Replace("\t", String.Empty);
		}
		
		
		/* Converts inValue to size (ddd.mm M)
		 * @param int decimals ... Number of digits before decimal point
		 * @param int mantissa ... Number of digits after decimal point
		 * @return string      ... Result value */
		public string getSize(int decimals, int mantissa, bool addSpace=false)
		{												
			int strlen = decimals + mantissa + 3;			            
            int msel = 0;            
            Conversion cnv;
			string res;            	
            do {
				cnv = new Conversion(inValueDouble / multiplierList[msel]);
				res = cnv.getString(mantissa) + " " + multiplierLetter[msel];
            	msel++;            	
			} while(res.Length > strlen);
			if(addSpace) {
				while(res.Length < strlen) {
					res = " " + res;
				}
			}
            return res;
		}
		
		/* Converts inValue to string (ddd.mm)
		 * @param int decimals ... Number of digits before decimal point
		 * @param int mantissa ... Number of digits after decimal point
		 * @return string      ... Result value */
		public string getString(int mantissa)
		{
			string res = this.inValueDouble.ToString("N" + mantissa.ToString());
			return removeWhitespace(res);
		}
		
		public string getString(int mantissa, int length, string left)
		{
			string res = this.inValueDouble.ToString("N" + mantissa.ToString());
			while(res.Length < length) {
				res = left + res;
			}
			return removeWhitespace(res);
		}		
		
		/* Converts inValue to double
		 * @return double */
		public double getDouble()
		{
			return this.inValueDouble;
		}
		
		/* Converts inValue to int
		 * @return int */
		public int getInt()
		{
			return (int)inValueDouble;
		}

		/* Converts inValue to uint
		 * @return int */
		public uint getUint()
		{
			return (uint)this.getInt();
		}
		
		/* Converts double number into string without whitespace characters
         * @param double n
         * @return string */
        public static string double2string(double n)
        {
            // WARNING, Replace 1st param " " is not simple space you write on keyboard dude !!!
            return n.ToString("N").Replace(" ", String.Empty);
        }        

		/* Converts string to integer
		 * @param string n ... Value to be calculated
		 * @return int     ... Result (-1 if string is invalid)*/
		public static int string2int(string n)
		{
			int i=-1;
			Int32.TryParse(n, out i);
			return i;
		}
		
		/* Converts Integer to Float
		 * @param int n  ... Value to be calculated
		 * @return float ... Result */
		public static float int2float(int n)
		{
			float f;
			float.TryParse(n.ToString(), out f);
			return f;
		}
        
		//
		// COLOR => "a,r,g,b"
		//		
		public static string color2argb(Color c)
		{
			return(
				c.A.ToString() + "," +
				c.R.ToString() + "," +
				c.G.ToString() + "," +
				c.B.ToString()
			);
		}

		//
		// "a,r,g,b" => COLOR 
		//				
		public static Color argb2color(string argb)
		{
			string err = "Nelze analyzovat hodnotu ARGB, neplatná hodnota " + argb;
			string[] chops = argb.Split(',');
			// Check count of numbers
			if(chops.Length != 4) {
				throw new ConversionException(err);
			}
			int i = 0;
			int a = 0;
			int r = 0;
			int g = 0;
			int b = 0;								
			foreach(string chop in chops) {
				int x = new Conversion(chop).getInt();
				if(x < 0 || x > 255) {
					throw new ConversionException(err);
				}
				switch(i) {
					case 0:
						a = x;
						break;
					case 1:
						r = x;
						break;
					case 2:
						g = x;
						break;
					case 3:
						b = x;
						break;						
				}
				i++;
			}
			return Color.FromArgb(a, r, g, b);
		}
		
		/// <summary>
		/// Converts string[] to List<string>
		/// </summary>
		/// <param name="value">input array of strings</param>
		/// <returns>Created string list</returns>
		public static List<string> stringArr2List(string[] value)
		{
			List<string> o = new List<string>();
			foreach(string v in value) {
				o.Add(v);
			}
			return o;
		}

		/// <summary>
		/// Converts uint[] to List<uint>
		/// </summary>
		/// <param name="value">input array of uint</param>
		/// <returns>Created uint list</returns>
		public static List<uint> uintArr2List(uint[] value)
		{
			List<uint> o = new List<uint>();
			foreach(uint v in value) {
				o.Add(v);
			}
			return o;
		}
		
		
		
		/// <summary>
		/// Converts IEnumerable to List<string>
		/// </summary>
		/// <param name="value">IEnumberable strings</param>
		/// <returns>Created string list</returns>
		public static List<string> IEnumString2List(IEnumerable<string> value)
		{
			List<string> o = new List<string>();
			foreach(string v in value) {
				o.Add(v);
			}
			return o;
		}		

		/// <summary>
		/// Converts IEnumerable to List<uint>
		/// </summary>
		/// <param name="value">IEnumerable uints</param>
		/// <returns>Created uint list</returns>
		public static List<uint> IEnumUint2List(IEnumerable<uint> value)
		{
			List<uint> o = new List<uint>();
			foreach(uint v in value) {
				o.Add(v);
			}
			return o;
		}
		
	}
	
	// ==========
	// EXCEPTIONS
	// ==========
	public class ConversionException : TroopyException
	{
		public ConversionException(string message) : base(message)
   		{
   		}
   	}  
}
