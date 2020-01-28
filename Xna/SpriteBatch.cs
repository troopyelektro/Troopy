/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 08.12.2019
 * Time: 10:32
 */
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Troopy;
using Troopy.Xna.Controls;
using System.Diagnostics;

namespace Troopy.Xna
{
	/// <summary>
	/// Description of Troopy_SpriteBatch.
	/// </summary>
	public class SpriteBatch : Microsoft.Xna.Framework.Graphics.SpriteBatch
	{
		/// <summary>
		/// Content Factory
		/// </summary>
		public ContentFactory content;
		
		/// <summary>
		/// Stores GameTime to avoid passing as parameter in all methods
		/// </summary>
		public GameTime gameTime {
			get;set;
		}
		
		/// <summary>
		/// Stores Mouse State
		/// </summary>
		public MouseState mouse {
			get;set;
		}
		
		/// <summary>
		/// Texture for Line Draw
		/// </summary>
		private Texture2D textureLine;
		public Texture2D TextureLine {
			get{
				return this.textureLine;
			}
		}
		
		/// <summary>
		/// Layer Depth
		/// </summary>
		private float layerDepth;
		public float LayerDepth {
			get {
				return this.layerDepth;
			}
			set {
				this.layerDepth = value;
			}
		}
		
		/// <summary>
		/// Instance of SpriteBatch
		/// </summary>
		/// <param name="graphics"></param>
		public SpriteBatch(GraphicsDevice graphics, ContentFactory content) : base(graphics)
		{
			this.content = content;
			
			// Create texture for drawing primitives
			Texture2D t = new Texture2D(this.GraphicsDevice, 1, 1);
			t.SetData(new Color[]{Color.White});
			this.textureLine = t;
		}

		/// <summary>
		/// Draws rectangle with solid color
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="col"></param>
		public void drawRectangle(int X, int Y, int width, int height, Color col)
		{
			this.Draw(
				this.TextureLine,
				new Vector2(X, Y),
				null,
				null,
				new Vector2(0,0),
				0f,
				new Vector2(width, height),
				col,
				SpriteEffects.None,
				this.LayerDepth
			);
		}

		public void drawLine(int X1, int Y1, int Length, Color col, bool vertical=false)
		{
			// Get scale
			int scaleX = 1;
			int scaleY = 1;
			if(vertical) {
				scaleY = Length;
			} else {
				scaleX = Length;
			}
			
			Vector2 scale = new Vector2(scaleX, scaleY);

			this.Draw(
				this.TextureLine,				// Texture
				new Vector2(X1, Y1),			// Position
				null,							// Rectangle destination
				null,							// Rectangle source
				new Vector2(0, 0),				// Origin
				0,								// Rotation
				scale,							// Scale
				col,							// Color
				SpriteEffects.None,
				this.LayerDepth
			);			
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="X1"></param>
		/// <param name="Y1"></param>
		/// <param name="X2"></param>
		/// <param name="Y2"></param>
		/// <param name="width"></param>
		/// <param name="col"></param>
		public void drawLine(int X1, int Y1, int X2, int Y2, int width, Color col)
		{		
			// Get point distance
			int diffX = Math.Abs(X2 - X1);
			int diffY = Math.Abs(Y2 - Y1);
			double diff = Math.Sqrt(Math.Pow(diffX,2) + Math.Pow(diffY,2));
		
			// Get angle
			int oX = X2 - X1;
			int oY = Y2 - Y1;
			
			double angle;
			if(oX == 0 && oY == 0) {
				angle = 0;
			} else {
				if(oY > 0) {
					angle = Math.Asin(-(double)oX / diff);
					//angle = 0;
				} else {
					angle = Math.Asin((double)oX / diff) + Math.PI;
				}
			}
			
			// Get scale
			Vector2 scale = new Vector2(1, (float)diff+1);

			this.Draw(
				this.TextureLine,				// Texture
				new Vector2(X1, Y1),			// Position
				null,							// Rectangle destination
				null,							// Rectangle source
				new Vector2(0, 0),				// Origin
				(float)angle,					// Rotation
				scale,							// Scale
				col,							// Color
				SpriteEffects.None,
				this.LayerDepth
			);
		}
				
		/// <summary>
		/// Fills up Bound with tiled texture 
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="bound"></param>
		/// <param name="vertical"></param>
		public void drawTiled(Texture2D texture, Rectangle bound, bool vertical=false)
		{
			bool horizontal = !vertical;

			float origX = (float)texture.Width / 2f;
			float origY = (float)texture.Width / 2f;
			
			float startX;
			float startY;
			int stepX;
			int stepY;
			int cntX;
			float restX;			
			int cntY;
			float restY;				
			
			if(horizontal) {				
				startX = origX;
				startY = origY;
				stepX = texture.Width;
				stepY = texture.Height;				
				cntX = bound.Width / texture.Width;
				restX = (float)bound.Width % (float)texture.Width;			
				cntY = bound.Height / texture.Height;
				restY = (float)bound.Height % (float)texture.Height;
			} else {
				startX = origY;
				startY = origX;				
				stepX = texture.Height;
				stepY = texture.Width;			
				cntX = bound.Width / texture.Height;
				restX = (float)bound.Width % (float)texture.Height;			
				cntY = bound.Height / texture.Width;
				restY = (float)bound.Height % (float)texture.Width;				
			}
			
			startX = startX + bound.Left;
			startY = startY + bound.Top;
						
			// Full textures
			for(int y = 0; (y<cntY) || (restY>0 && y<=cntY); y++) {
				for(int x = 0; (x<cntX) || (restX>0 && x<=cntX); x++) {
					
					Rectangle srcrect = new Rectangle(0,0,texture.Width,texture.Height);				
					
					Vector2 position = new Vector2((float)startX + (float)x*(float)stepX, (float)startY + (float)y*(float)stepY);
					
					if(horizontal) {
						if(x==cntX) {
							srcrect.Width = (int)restX;	
						}
						if(y==cntY) {
							srcrect.Height = (int)restY;	
						}
					} else {
						if(x==cntX && y==cntY) {
							srcrect = new Rectangle(texture.Width - (int)restY, 0, (int)restY, (int)restX);	
							position = new Vector2((float)startX + (float)x*(float)stepX, (float)startY + (float)(y-1)*(float)(stepY)+restY);
						} else {
							if(x==cntX) {
								// OK
								srcrect = new Rectangle(0, 0, texture.Width, (int)restX);
							}					
							if(y==cntY) {
								srcrect = new Rectangle(texture.Width - (int)restY, 0, (int)restY, texture.Height);
								position = new Vector2((float)startX + (float)x*(float)stepX, (float)startY + (float)(y-1)*(float)(stepY)+restY);
							}		
						}
					}
					
					this.Draw(
		            	texture,
		            	position,
		            	null,
		            	srcrect,
		            	new Vector2(origX, origY),
		            	horizontal ? 0f : -(float)Math.PI/2f,
		            	null,
		            	Color.White,
		            	SpriteEffects.None,
		            	this.LayerDepth
		            );
				}
			}						
		}
	}
}
