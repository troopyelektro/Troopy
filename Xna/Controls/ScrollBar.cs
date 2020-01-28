/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 09.12.2019
 * Time: 11:23
 */
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Troopy;

namespace Troopy.Xna.Controls
{
	/// <summary>
	/// Description of Troopy_XnaControls_ScrollBar.
	/// </summary>
	public class ScrollBar : Control
	{
		//
		// Configuration
		//		
		
		/// <summary>
		/// Determines if scrollbar is horizontal 
		/// </summary>
		private bool horizontal;
		
		/// <summary>
		/// Determines if scrollbar is horizontal 
		/// </summary>
		public bool Horizontal {
			get {
				return this.horizontal;
			}
			set {
				this.horizontal = value;
				this.vertical = !value;			
			}			
		}

		/// <summary>
		/// Determines if scrollbar is vertical 
		/// </summary>		
		private bool vertical;

		/// <summary>
		/// Determines if scrollbar is vertical 
		/// </summary>		
		public bool Vertical {
			get {
				return this.vertical;
			}
			set {
				this.horizontal = !value;
				this.vertical = value;
			}			
		}
		
		//
		// Value
		//
		/// <summary>
		/// Value 0 to 1 
		/// </summary>
		public float Value;

		/// <summary>
		/// Value 0 to 1
		/// Remembers value on Left Button Pressed
		/// </summary>
		private float onLeftButtonPressedValue;
		
		/// <summary>
		/// Range of Bar in Scrool Area 0 to 1 
		/// </summary>		
		public float Range;
		
		/// <summary>
		/// Determines value step per one second
		/// </summary>
		
		private float speed = 1f;
		/// <summary>
		/// Determines value step per one second
		/// </summary>
		public float Speed {
			get {
				return this.speed;
			}
			set {
				this.speed = value;
			}
		}
		
		//
		// Textures
		//
		public Texture2D TextureArrowOn;
		public Texture2D TextureArrowOff;
		public Texture2D TextureArrowHover;
		public Texture2D TextureArrowDisabled;
		public Texture2D TextureRangeOn;
		public Texture2D TextureRangeDisabled;
		public Texture2D TextureBarEdgeOn;
		public Texture2D TextureBarEdgeOff;
		public Texture2D TextureBarEdgeHover;
		public Texture2D TextureBarCenterOn;
		public Texture2D TextureBarCenterOff;
		public Texture2D TextureBarCenterHover;		
				
		/// <summary>
		/// Creates instance of Scrollbar Troopy XNA Control
		/// </summary>
		public ScrollBar()
		{
			this.horizontal = true;
			this.vertical = false;
		}
		
		/// <summary>
		/// Scroll Bar Coordinate, left/top edge of minus button
		/// </summary>
		private int CoordBtnMinusLower {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.Left;
				} else {
					// Vertical
					return this.Top;
				}
			}
		}

		/// <summary>
		/// Scroll Bar Coordinate, right/bottom edge of minus button
		/// </summary>
		private int CoordBtnMinusUpper {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.Left + this.Height - 1;
				} else {
					// Vertical
					return this.Top + this.Width - 1;
				}
			}
		}		

		/// <summary>
		/// Scroll Bar Coordinate, left/top edge of plus button
		/// </summary>
		private int CoordBtnPlusLower {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.Left + this.Width - this.Height - 1;
				} else {
					// Vertical
					return this.Top + this.Height - this.Width - 1;
				}
			}
		}

		/// <summary>
		/// Scroll Bar Coordinate, right/bottom edge of plus button
		/// </summary>
		private int CoordBtnPlusUpper {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.Left + this.Width - 1;
				} else {
					// Vertical
					return this.Top + this.Height - 1;
				}
			}
		}

		/// <summary>
		/// Scroll Bar Coordinate, Start of track
		/// </summary>		
		private int CoordTrackStart {
			get {
				return this.CoordBtnMinusUpper + 1;
			}
		}

		/// <summary>
		/// Scroll Bar Coordinate, End of track
		/// </summary>		
		private int CoordTrackEnd {
			get {
				return this.CoordBtnPlusLower - 1;
			}
		}
		
		private int CoordTrackLength {
			get {
				return this.CoordTrackEnd - this.CoordTrackStart + 1; 
			}
		}

		/// <summary>
		/// Minimum Bar size (Edge 1 + Center + Edge 2)
		/// </summary>
		private int MinBarSize {
			get {
				if(this.Horizontal) {
					return 3 * this.Height;
				} else {
					return 3 * this.Width;
				}
			}
		}
		
		/// <summary>
		/// Calculates size of Viewed Bar
		/// </summary>
		private int BarSize {
			get {
				int calcBarSize = (int)(this.Range * (float)this.CoordTrackLength);
				if(calcBarSize < this.MinBarSize) {
					return this.MinBarSize;
				} else {
					return calcBarSize;
				}
			}
		}

		private int CoordEdge1Lower {
			get { 
				float f = (float)(this.CoordTrackLength - this.BarSize) * this.Value;
				return (int)f + this.CoordTrackStart;
			}
		}		

		private int CoordCenterLower {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.CoordEdge1Lower + this.Height;
				} else {
					// Vertical
					return this.CoordEdge1Lower + this.Width;
				}
			}
		}
		
		private int CoordEdge2Lower {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.CoordEdge1Lower + this.BarSize - this.Height;
				} else {
					// Vertical
					return this.CoordEdge1Lower + this.BarSize - this.Width;
				}
			}
		}
		
		private int CoordEdge2Upper {
			get {
				if(this.horizontal) {
					// Horizontal
					return this.CoordEdge2Lower + this.Height - 1;
				} else {
					// Vertical
					return this.CoordEdge2Lower + this.Width - 1;
				}
			}
		}		
		
		/// <summary>
		/// Determines if Button Minus is hovered
		/// </summary>
		private bool BtnMinusHovered {
			get {
				if(this.isHovered() && this.IsMouseOnBtnMinus(this.Mouse.CurrentState)) {
					return true;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// Determines if Button Minus is hovered
		/// </summary>
		private bool BtnPlusHovered {
			get {
				if(this.isHovered() && this.IsMouseOnBtnPlus(this.Mouse.CurrentState)) {
					return true;
				} else {
					return false;
				}
			}
		}	

		/// <summary>
		/// Determines if Button Minus is hovered
		/// </summary>
		private bool BarHovered {
			get {
				if(this.isHovered() && this.IsMouseOnBar(this.Mouse.CurrentState)) {
					return true;
				} else {
					return false;
				}
			}
		}	

		/// <summary>
		/// Determines if Button Minus is pushed
		/// </summary>
		private bool BtnMinusPushed {
			get {
				if(this.isPushed() && this.IsMouseOnBtnMinus(this.Mouse.LeftButtonPressed)) {
					return true;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// Determines if Button Minus is pushed
		/// </summary>
		private bool BtnPlusPushed {
			get {
				if(this.isPushed() && this.IsMouseOnBtnPlus(this.Mouse.LeftButtonPressed)) {
					return true;
				} else {
					return false;
				}
			}
		}	

		/// <summary>
		/// Determines if Button Minus is pushed
		/// </summary>
		private bool _barPushed;
		private bool BarPushed {
			get {
				if(this.isPushed()) {
					if(!this._barPushed && this.Mouse.LeftButtonPressedFlag && this.IsMouseOnBar(this.Mouse.CurrentState)) {
						this._barPushed = true;
					}					
				} else {
					this._barPushed = false;
				}
				return this._barPushed;
			}
		}	
		
		/// <summary>
		/// Determines, if Mouse is in Current State on Button Minus
		/// </summary>
		/// <param name="ms">MouseState for comparison</param>
		/// <returns></returns>
		private bool IsMouseOnBtnMinus(MouseState ms) {
			if(this.horizontal) {
				if(this.isInRange(ms.X, this.CoordBtnMinusLower, this.CoordBtnMinusUpper)) {
					return true;
				} else {
					return false;
				}
			} else {
				if(this.isInRange(ms.Y, this.CoordBtnMinusLower, this.CoordBtnMinusUpper)) {
					return true;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// Determines, if Mouse is in Current State on Button Plus
		/// </summary>
		/// <param name="ms">MouseState for comparison</param>
		/// <returns></returns>
		private bool IsMouseOnBtnPlus(MouseState ms) {
			if(this.horizontal) {
				if(this.isInRange(ms.X, this.CoordBtnPlusLower, this.CoordBtnPlusUpper)) {
					return true;
				} else {
					return false;
				}
			} else {
				if(this.isInRange(ms.Y, this.CoordBtnPlusLower, this.CoordBtnPlusUpper)) {
					return true;
				} else {
					return false;
				}
			}
		}
		
		/// <summary>
		/// Determines, if Mouse is in Current State on Bar
		/// </summary>
		/// <param name="ms">MouseState for comparison</param>
		/// <returns></returns>
		private bool IsMouseOnBar(MouseState ms) {
			if(this.horizontal) {
				if(this.isInRange(ms.X, this.CoordEdge1Lower, this.CoordEdge2Upper)) {
					return true;
				} else {
					return false;
				}
			} else {
				if(this.isInRange(ms.Y, this.CoordEdge1Lower, this.CoordEdge2Upper)) {
					return true;
				} else {
					return false;
				}
			}
		}		
		
		/// <summary>
		/// Updates scroll bar
		/// </summary>
		public void update()
		{
			// Check, if left button just pressed and stores value on this state
			if(this.Mouse.PreviousState.LeftButton == ButtonState.Released &&
			   this.Mouse.CurrentState.LeftButton == ButtonState.Pressed) {
				this.onLeftButtonPressedValue = this.Value;
			}
			
			// Button Minus Pushed
			if(this.BtnMinusPushed) {
				double diff = this.Time.CurrentState.TotalMilliseconds - this.Time.PreviousState.TotalMilliseconds;
				float diffS = (float)diff / 1000f;
				float step = this.Speed * diffS;
				this.Value -= step;
				if(this.Value < 0) {
					this.Value = 0;
				}
			}

			// Button Plus Pushed
			if(this.BtnPlusPushed) {
				double diff = this.Time.CurrentState.TotalMilliseconds - this.Time.PreviousState.TotalMilliseconds;
				float diffS = (float)diff / 1000f;
				float step = this.Speed * diffS;
				this.Value += step;
				if(this.Value > 1) {
					this.Value = 1;
				}
			}

			// Button Plus Pushed
			if(this.BarPushed) {
				int diffP = 0;
				if(this.Horizontal) {
					diffP = this.Mouse.CurrentState.X - this.Mouse.LeftButtonPressed.X;
				} else {
					diffP = this.Mouse.CurrentState.Y - this.Mouse.LeftButtonPressed.Y;
				}
				float ratio = 1f / (float)(this.CoordTrackLength - this.BarSize);
				this.Value = this.onLeftButtonPressedValue + (float)diffP * ratio;
				
				if(this.Value < 0) {
					this.Value = 0;
				}
				if(this.Value > 1) {
					this.Value = 1;
				}				
			}			
		}
		
		
		
		/// <summary>
		/// Draws scroll bar
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void draw(SpriteBatch spriteBatch)
		{						
			Texture2D texBtnMinus;
			Texture2D texBtnPlus;
			Texture2D texBarEdge;
			Texture2D texBarCenter;
			Texture2D texTrack;
			
			if(this.Enabled) {
			
				// Texture Button Minus			
				if(this.BtnMinusHovered) {
					texBtnMinus = this.TextureArrowHover;
				} else if(this.BtnMinusPushed) {
					texBtnMinus = this.TextureArrowOn;
				} else {
					texBtnMinus = this.TextureArrowOff;
				}
				
				// Texture Button Plus
				
				if(this.BtnPlusHovered) {
					texBtnPlus = this.TextureArrowHover;
				} else if(this.BtnPlusPushed) {
					texBtnPlus = this.TextureArrowOn;					
				} else {
					texBtnPlus = this.TextureArrowOff;
				}
				// Texture track
				texTrack = this.TextureRangeOn;
				
				// Texture Bar
				if(this.BarHovered) {
					texBarEdge = this.TextureBarEdgeHover;
					texBarCenter = this.TextureBarCenterHover;
				} else if(this.BarPushed) {
					texBarEdge = this.TextureBarEdgeOn;
					texBarCenter = this.TextureBarCenterOn;				
				} else {
					texBarEdge = this.TextureBarEdgeOff;
					texBarCenter = this.TextureBarCenterOff;
				}
				
			} else {
				texBtnMinus = this.TextureArrowDisabled;
				texBtnPlus = this.TextureArrowDisabled;
				texTrack = this.TextureRangeDisabled;
				texBarEdge = this.TextureBarEdgeOff;
				texBarCenter = this.TextureBarCenterOff;				
			}
			
			if(this.Horizontal) {
				float orig = (float)this.Height / 2f;
				
				// Button Minus
				if(texBtnMinus != null) {
		            spriteBatch.Draw(
		            	texBtnMinus,
		            	new Vector2((float)this.CoordBtnMinusLower + orig, (float)this.Top + orig),
		            	null,
		            	null,
		            	new Vector2(orig, orig),
		            	-(float)(Math.PI / 2f),
		            	null,
		            	Color.White,
		            	SpriteEffects.None,
		            	spriteBatch.LayerDepth
		            );			
				}	

				// Button Plus
				if(texBtnPlus != null) {
		            spriteBatch.Draw(
		            	texBtnPlus,
		            	new Vector2((float)this.CoordBtnPlusLower + orig, (float)this.Top + orig),
		            	null,
		            	null,
		            	new Vector2(orig, orig),
		            	+(float)(Math.PI / 2f),
		            	null,
		            	Color.White,
		            	SpriteEffects.None,
		            	spriteBatch.LayerDepth
		            );			
				}

				// Track
				if(texTrack != null) {		            
					spriteBatch.drawTiled(
						texTrack,
						new Rectangle(this.CoordTrackStart, this.Top, this.CoordTrackLength, this.Height)
					);
				}
				
				// Bar only if Enabled
				if(this.Enabled) {
				
					// Bar EDGE 1
					if(texBarEdge != null) {		            
						spriteBatch.Draw(
			            	texBarEdge,
			            	new Vector2((float)this.CoordEdge1Lower + orig, (float)this.Top + orig),
			            	null,
			            	null,
			            	new Vector2(orig, orig),
			            	0,
			            	null,
			            	Color.White,
			            	SpriteEffects.None,
			            	spriteBatch.LayerDepth
			            );		
					}				
	
					// Bar Center								
					if(texBarCenter != null) {
						spriteBatch.drawTiled(
							texBarCenter,
							new Rectangle(this.CoordCenterLower, this.Top, this.BarSize - 2*this.Height, this.Height)
						);
					}				
					
	
					// Bar EDGE 2
					if(texBarEdge != null) {		            
						spriteBatch.Draw(
			            	texBarEdge,
			            	new Vector2((float)this.CoordEdge2Lower + orig, (float)this.Top + orig),
			            	null,
			            	null,
			            	new Vector2(orig, orig),
			            	(float)Math.PI,
			            	null,
			            	Color.White,
			            	SpriteEffects.None,
			            	spriteBatch.LayerDepth
			            );		
					}
				}
			}
			
			if(this.Vertical) {
				float orig = (float)this.Width / 2f;	

				// Button Minus
				if(texBtnMinus != null) {
		            spriteBatch.Draw(
		            	texBtnMinus,
		            	new Vector2((float)this.Left + orig, (float)this.CoordBtnMinusLower + orig),
		            	null,
		            	null,
		            	new Vector2(orig, orig),
		            	0,
		            	null,
		            	Color.White,
		            	SpriteEffects.None,
		            	spriteBatch.LayerDepth
		            );			
				}

				// Button Plus
				if(texBtnPlus != null) {
		            spriteBatch.Draw(
		            	texBtnPlus,
		            	new Vector2((float)this.Left + orig, (float)this.CoordBtnPlusLower + orig),
		            	null,
		            	null,
		            	new Vector2(orig, orig),
		            	(float)Math.PI,
		            	null,
		            	Color.White,
		            	SpriteEffects.None,
		            	spriteBatch.LayerDepth
		            );			
				}

				// Track
				if(texTrack != null) {
					spriteBatch.drawTiled(
						texTrack,
						new Rectangle(this.Left, this.CoordTrackStart, this.Width, this.CoordTrackLength),
						true
					);			
				}

				// Bar only if enabled
				if(this.Enabled) {
				
					// Bar EDGE 1
					if(texBarEdge != null) {		            
						spriteBatch.Draw(
			            	texBarEdge,
			            	new Vector2((float)this.Left + orig, (float)this.CoordEdge1Lower + orig),
			            	null,
			            	null,
			            	new Vector2(orig, orig),
			            	(float)Math.PI / 2f,
			            	null,
			            	Color.White,
			            	SpriteEffects.None,
			            	spriteBatch.LayerDepth
			            );		
					}				
					
					// Bar Center								
					if(texBarCenter != null) {
						spriteBatch.drawTiled(
							texBarCenter,
							new Rectangle(this.Left, this.CoordCenterLower, this.Width, this.BarSize - 2*this.Width),
							true
						);
					}
	
					// Bar EDGE 2
					if(texBarEdge != null) {		            
						spriteBatch.Draw(
			            	texBarEdge,
			            	new Vector2((float)this.Left + orig, (float)this.CoordEdge2Lower + orig),
			            	null,
			            	null,
			            	new Vector2(orig, orig),
			            	-(float)Math.PI / 2f,
			            	null,
			            	Color.White,
			            	SpriteEffects.None,
			            	spriteBatch.LayerDepth
			            );		
					}
				}
			}
		}
		
		
	}
}
