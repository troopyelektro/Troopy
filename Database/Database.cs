using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Troopy.Database
{
	public abstract class Database
	{
		//
		// Converts DateTime to string suitable for SQL
		//		
		public static string DateTime2sql(DateTime d)
		{
			string s;
			s = d.Year.ToString("D4") + "-" + d.Month.ToString("D2") + "-" + d.Day.ToString("D2") +
				" " +
				d.Hour.ToString("D2") + ":" + d.Minute.ToString("D2") + ":" + d.Second.ToString("D2");
			return s;
		}

		//
		// Converts DateTime to string suitable for SQL
		// Clears hours, minutes and seconds
		//		
		public static string Date2sql(DateTime d)
		{
			string s;
			s = d.Year.ToString("D4") + "-" + d.Month.ToString("D2") + "-" + d.Day.ToString("D2") +
				" " +
				0.ToString("D2") + ":" + 0.ToString("D2") + ":" + 0.ToString("D2");
			return s;
		}
	}

	// ==========
	// EXCEPTIONS
	// ==========
	public class DatabaseException : TroopyException
	{
		public DatabaseException(string message) : base(message)
		{
		}
	}
}
