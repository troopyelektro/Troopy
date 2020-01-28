/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 10.12.2019
 * Time: 16:54
 */
using System;
using Microsoft.Xna.Framework.Input;

namespace Troopy.Xna
{
	/// <summary>
	/// Extended Mouse Status
	/// </summary>
	public class MouseStatus
	{
		/// <summary>
		/// Current Mouse Status
		/// </summary>
		private MouseState currentState;
		public MouseState CurrentState {
			get {
				return this.currentState;
			}
		}
		
		/// <summary>
		/// Mouse status before last update
		/// </summary>
		private MouseState previousState;
		public MouseState PreviousState {
			get {
				return this.previousState;
			}
		}
		
		/// <summary>
		/// Mouse status on left button pressed
		/// </summary>
		private MouseState leftButtonPressed;
		public MouseState LeftButtonPressed {
			get {
				return this.leftButtonPressed;
			}
		}
		
		/// <summary>
		/// Mouse status on left button pressed
		/// </summary>
		private MouseState leftButtonReleased;
		public MouseState LeftButtonReleased {
			get {
				return this.leftButtonReleased;
			}
		}

		/// <summary>
		/// Determines, if in current states went left button from released state to pressed state
		/// </summary>
		public bool LeftButtonPressedFlag {
			get {
				// Store state on mouse left press
				if(previousState.LeftButton == ButtonState.Released && this.currentState.LeftButton == ButtonState.Pressed) {
					return true;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// Determines, if in current states went left button from pressed state to released state
		/// </summary>		
		public bool LeftButtonReleasedFlag {
			get {
				// Store state on mouse left press
				if(previousState.LeftButton == ButtonState.Pressed && this.currentState.LeftButton == ButtonState.Released) {
					return true;
				} else {
					return false;
				}
			}
		}		
		
		/// <summary>
		/// 
		/// </summary>
		public MouseStatus()
		{
		}
		
		public void update(MouseState state)
		{
			this.previousState = this.currentState;
			this.currentState = state;		
			
			// Store state on mouse left press
			if(previousState.LeftButton == ButtonState.Released && this.currentState.LeftButton == ButtonState.Pressed) {
				this.leftButtonPressed = this.currentState;
			}
			
			// Store state on mouse left release
			if(previousState.LeftButton == ButtonState.Pressed && this.currentState.LeftButton == ButtonState.Released) {
				this.leftButtonReleased = this.currentState;
			}			
		}		
	}
}
