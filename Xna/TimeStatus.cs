/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 11.12.2019
 * Time: 7:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Microsoft.Xna.Framework;

namespace Troopy.Xna
{
	/// <summary>
	/// Description of Troopy_TimeStatus.
	/// </summary>
	public class TimeStatus
	{
		/// <summary>
		/// Current Mouse Status
		/// </summary>
		private TimeSpan currentState;
		public TimeSpan CurrentState {
			get {
				return this.currentState;
			}
		}
		
		/// <summary>
		/// Mouse status before last update
		/// </summary>
		private TimeSpan previousState;
		public TimeSpan PreviousState {
			get {
				return this.previousState;
			}
		}
		
		public TimeStatus()
		{
			this.previousState = new TimeSpan(0);
			this.currentState = new TimeSpan(0);
		}
		
		public void update(GameTime state)
		{
			this.previousState = this.currentState;					
			this.currentState = state.TotalGameTime;						
		}
	}
}
