/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 07.07.2019
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Troopy
{
	public abstract class DataSize
	{
        public const double dataSizeStep = 1024;
		public const double multiplierB = 1;        
        public const double multiplierK = multiplierB * dataSizeStep;
        public const double multiplierM = multiplierK * dataSizeStep;
        public const double multiplierG = multiplierM * dataSizeStep;
        public const double multiplierT = multiplierG * dataSizeStep;
        
       
        public static int lastMultiplierFoundIndex;
        
		public static readonly string[] multiplierUnit = {
        	"T", "G", "M", "K", "TB", "GB", "MB", "KB", "B",
        };
		public static readonly double[] multiplierValue = {
        	multiplierT, multiplierG, multiplierM, multiplierK, multiplierT, multiplierG, multiplierM, multiplierK, multiplierB
        };
        public static readonly string[] multiplierLetter = {
        	"B", "K", "M", "G", "T"
        };
        
        public static readonly double[] multiplierList = {
        	multiplierB, multiplierK, multiplierM, multiplierG, multiplierT
        };
        
        /* Checks end of string and returns multiplier of represented value
         * Returns 0 if multiplier is not used
         * @param string value ... value to be evaluated
         * @return double */
        public static double getMultiplier(string value)
        {
        	for(int i=0; i<multiplierUnit.Length; i++) {
        		if(value.EndsWith(multiplierUnit[i])) {
        			lastMultiplierFoundIndex = i;
        			return multiplierValue[i];
        		}	
        	}
        	return 0;
        }
	}
}
