/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 08.12.2019
 * Time: 10:10
 */
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Troopy;

namespace Troopy.Xna.Controls
{
	/// <summary>
	/// Button for Xna
	/// </summary>
	public class Button : Control
	{
		public Texture2D TextureDisabled;
		public Texture2D TextureIdle;
		public Texture2D TextureHover;
		public Texture2D TexturePressed;
			
		public SpriteFont Font;
		public string Text;
		public Color BackGroundColor = Color.Gray;
		public Color FontColor = Color.Black;
		public int TextOffsetX = 5;
		public int TextOffsetY = 5;
		
		public Button()
		{
		}
		
		public void draw(SpriteBatch spriteBatch)
		{
			// Get texture
			Texture2D t;
			if(this.Enabled) {
				if(this.isPushed()) {
					t = this.TexturePressed;
				} else if(this.isHovered()) {
					t = this.TextureHover;
				} else {
					t = this.TextureIdle;
				}
			} else {
				t = this.TextureDisabled;
			}
			
			// Draw texture
			if(t != null) {
	            spriteBatch.Draw(
	            	t,
	            	new Vector2(this.Left + this.TextureOriginX, this.Top + this.TextureOriginY),
	            	null,
	            	null,
	            	new Vector2(this.TextureOriginX,this.TextureOriginY),
	            	this.TextureRotation,
	            	null,
	            	Color.White,
	            	SpriteEffects.None,
	            	0f
	            );			
			} else {
				spriteBatch.drawRectangle(this.Left, this.Top, this.Width, this.Height, this.BackGroundColor);
			}
			
			// Draw Font
			if(this.Font != null && this.Text != "") {
				spriteBatch.DrawString(
					this.Font,
					this.Text,
					new Vector2(
						this.Left + this.TextOffsetX,
						this.Top + this.TextOffsetY
					),
					this.FontColor,
					0f,
					new Vector2(0,0),
					new Vector2(1,1),
					SpriteEffects.None,
					spriteBatch.LayerDepth
				);
			}
		}
	}
}
