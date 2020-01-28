using System;
using System.Collections.Generic;

namespace Troopy
{	
	/// <summary>
	/// Version 1.00 Added bool option 
	/// </summary>
	public class ParameterCollection
	{
		private List<string> names = new List<string>();
		private List<string> values = new List<string>();
		private List<bool> assigned = new List<bool>();
		
		public string multilineStringIn = "\r\n";
		public string multilineStringOut = "~NewLine~";
		
		private const char paramDelimiter = '\n';
		private const char paramAssign = '=';
		
		
		//
		// Get string data collection
		//
		public string Data{
			get{
				string d="";
				if(this.Count>0) {
					for(int i=0; i<this.Count; i++) {
						d += names[i] + paramAssign.ToString() + values[i];
						bool last = ((i+1) == this.Count);
						if(!last) {
							d += paramDelimiter.ToString();
						}
					}
				}
				return d;
			}
		}
		
		//
		// Parameter Count
		//
		public int Count {
			get {
				return this.names.Count;
			}
		}
		
		public ParameterCollection(string[] parameterNames, string data, string[] defaults=null)
		{
			// Create list of names
			foreach(string parameterName in parameterNames) {
				if(!this.names.Contains(parameterName)) {
					this.names.Add(parameterName);
					this.values.Add("");
					this.assigned.Add(false);
				}
			}
			
			// Process data
			string[] expressions = data.Split(paramDelimiter);
			foreach(string expression in expressions) {
				// Each Expression
				string[] operands = expression.Split(new char[]{paramAssign}, 2);
				if(operands.Length == 2) {
					string p = operands[0];
					string v = operands[1];
					int i = this.names.IndexOf(p);
					if(i >= 0) {
						this.values[i] = v;
						this.assigned[i] = true;
					}
				}			
			}
			
			// Defaults
			if(defaults != null) {
				if(defaults.Length > parameterNames.Length) {
					throw new ParameterCollectionException(
						"Nelze vytvořit kolekci parametrů, protože seznam výchozích hodnot je větší než seznam parametrů."
					);
				} else {
					int i=0;
					foreach(string dv in defaults) {
						if(!this.assigned[i]) {
							this.values[i] = dv;
						}
						i++;
					}					
				}
			}						
		}
					
		//
		// String
		//
		public string getString(string p)
		{
			int i=this.names.IndexOf(p);
			if(i == -1) {
				throw new ParameterCollectionException(
					"Parameter " + p + " v kolekci nenalezen."
				);
			} else {
				return this.values[i];
			}
		}
		
		public void setString(string p, string v)
		{
			int i=this.names.IndexOf(p);
			if(i == -1) {
				throw new ParameterCollectionException(
					"Parameter " + p + " v kolekci nenalezen."
				);
			} else {
				this.values[i] = v;
			}			
		}

		//
		// Multiline String
		//
		public string getMultilineString(string p)
		{
			int i=this.names.IndexOf(p);
			if(i == -1) {
				throw new ParameterCollectionException(
					"Parameter " + p + " v kolekci nenalezen."
				);
			} else {
				return this.values[i].Replace(this.multilineStringOut, this.multilineStringIn);
			}
		}
		
		public void setMultilineString(string p, string v)
		{
			int i=this.names.IndexOf(p);
			if(i == -1) {
				throw new ParameterCollectionException(
					"Parameter " + p + " v kolekci nenalezen."
				);
			} else {
				this.values[i] = v.Replace(this.multilineStringIn, this.multilineStringOut);
			}			
		}
		
		//
		// Int
		//
		public int getInt(string p)
		{
			return new Conversion(this.getString(p)).getInt();
		}
		
		public void setInt(string p, int v)
		{
			this.setString(p, v.ToString());
		}	

		//
		// Double
		//
		public double getDouble(string p)
		{
			return new Conversion(this.getString(p)).getDouble();
		}

		public void setDouble(string p, double v)
		{
			this.setString(p, v.ToString());
		}	
		
		//
		// DateTime
		//
		public DateTime getDateTime(string p)
		{
			DateTime dt;
			DateTime.TryParse(this.getString(p), out dt);
			return dt;
		}
		
		public void setDateTime(string p, DateTime v)
		{
			this.setString(p, v.ToString("dd.MM.yyyy HH:mm:ss"));
		}
		
		//
		// Bool
		//
		public bool getBool(string p)
		{
			if(this.getString(p).ToLower() == "true") {
				return true;
			} else {
				return false;
			}
		}

		public void setBool(string p, bool v)
		{
			if(v) {
				this.setString(p, "true");
			} else {
				this.setString(p, "false");
			}
			
		}		
	}
	
	// ==========
	// EXCEPTIONS
	// ==========
	public class ParameterCollectionException : TroopyException
	{
		public ParameterCollectionException(string message) : base(message)
   		{
   		}
   	}  	
}
