/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 08.12.2019
 * Time: 10:37
 */
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Troopy.Xna
{
	/// <summary>
	/// Description of Troopy_ContentFactory.
	/// </summary>
	public class ContentFactory
	{
		// Content Manager
		protected ContentManager contentManager;

		public ContentFactory(ContentManager cm)
		{
			this.contentManager = cm;
		}
	}
}
