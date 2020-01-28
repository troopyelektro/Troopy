/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 08.12.2019
 * Time: 10:23
 */
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Troopy.Xna.Controls
{
	/// <summary>
	/// Base class of troopy XNA Controls
	/// </summary>
	abstract public class Control
	{
		/// <summary>
		/// Mouse state
		/// </summary>
		public static MouseStatus MouseService;
		/// <summary>
		/// Mouse state
		/// </summary>
		public MouseStatus Mouse {
			get {
				return Control.MouseService;
			}
		}
		
		/// <summary>
		/// Mouse state
		/// </summary>
		public static TimeStatus TimeService;
		/// <summary>
		/// Mouse state
		/// </summary>
		public TimeStatus Time{
			get {
				return Control.TimeService;
			}
		}		

		/// <summary>
		/// Determines if control is enabled
		/// </summary>
		public bool Enabled;
		
		/// <summary>
		/// Width of CAD Panel
		/// </summary>
		public int Left {
			get; set;
		}
		
		/// <summary>
		/// Height of CAD Panel
		/// </summary>
		public int Top {
			get; set;
		}

		/// <summary>
		/// Width of CAD Panel
		/// </summary>
		protected int width;
		public int Width {
			get{
				return this.width;
			}
			set{
				this.width = value;
			}
		}
		
		/// <summary>
		/// Height of CAD Panel
		/// </summary>
		protected int height;
		public int Height {
			get{
				return this.height;
			}
			set{
				this.height = value;
			}			
		}
		
		public int TextureOriginX;
		public int TextureOriginY;
		public float TextureRotation;
		
		private float layerBase = 0;
		public float LayerBase {
			get {
				return this.layerBase;
			}
			set {
				if(value >= 0f && value <= 0.9f) {
					this.layerBase = value;
				}
			}
		}
		
		public Control()
		{
		}
		
		public bool isInRange(int value, int lower, int upper)
		{
			if(value >= lower && value <= upper) {
				return true;
			} else {
				return false;
			}
		}
		
		public bool isMouseOnControl(MouseState ms)
		{
			if((ms.X >= this.Left) &&
			   (ms.X <= (this.Left + this.Width - 1)) &&
			   (ms.Y >= this.Top) &&
			   (ms.Y <= (this.Top + this.Height - 1))
			) {
				return true;
			} else {
				return false;
			}		
		}
		
		public bool isHovered()
		{
			if(this.Enabled &&
			   this.isMouseOnControl(this.Mouse.CurrentState) &&
			   this.Mouse.CurrentState.LeftButton == ButtonState.Released
			) {
				return true;
			} else {
				return false;    
			}
		}
		
		public bool isPushed()
		{
			if(this.Enabled && 
			   this.Mouse.CurrentState.LeftButton == ButtonState.Pressed &&
			   this.isMouseOnControl(this.Mouse.LeftButtonPressed)
			) {
				return true;
			} else {
				return false;
			}			
		}
		
		public bool isClicked()
		{
			if(this.Enabled &&
			   this.isMouseOnControl(this.Mouse.LeftButtonPressed) &&
			   this.isMouseOnControl(this.Mouse.LeftButtonReleased) &&
			   this.Mouse.PreviousState.LeftButton == ButtonState.Pressed &&
			   this.Mouse.CurrentState.LeftButton == ButtonState.Released			   
			) {
				return true;
			} else {
				return false;
			}
		}
	}
}
