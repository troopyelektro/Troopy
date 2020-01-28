using System;

namespace Troopy
{
	[Serializable]
	public class Limit
	{	
		// Keep limit schematic lower <= target <= upper
		private double lower;
		public double Lower{
			get{
				return this.lower;
			}
			set{
				bool processed = false;
				
				if(!processed && value<=this.target) {
					this.lower = value;
					processed = true;
				}

				if(!processed && value>this.target && value<=this.upper) {
					this.lower = value;
					this.target = value;
					processed = true;
				}
				
				if(!processed && value>this.upper) {
					this.lower = this.upper;
					this.upper = value;
					this.target = (this.upper+this.lower)/2;
				}
			}
		}
		
		private double target;
		public double Target{
			get{
				return this.target;
			}
			set{
				bool processed = false;
				
				if(!processed && value<this.lower) {
					this.lower = value;
					this.target = value;
					processed = true;
				}
				
				if(!processed && value>this.upper) {
					this.target = value;
					this.upper = value;
					processed = true;
				}

				if(!processed && this.lower<=value && value<=this.upper) {
					this.target = value;
					processed = true;
				}				
			}
		}

		private double upper;
		public double Upper{
			get{
				return this.upper;
			}
			set{
				bool processed = false;
				
				if(!processed && this.target<=value) {
					this.upper = value;
					processed = true;
				}

				if(!processed && this.lower<=value && value<this.target) {
					this.upper = value;
					this.target = value;
					processed = true;
				}
				
				if(!processed && value<this.lower) {
					this.upper = this.lower;
					this.lower = value;
					this.target = (this.upper+this.lower)/2;
				}			
			}
		}		
		
		// Constructor
		public Limit(double lower, double target, double upper)
		{
			this.lower = lower;
			this.target = target;
			this.upper = upper;
		}
	}
}
