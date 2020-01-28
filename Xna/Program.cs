/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 02.12.2019
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Troopy.Xna.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Troopy.Xna
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		private static void Main(string[] args)
		{
			GUI g = new GUI();
			g.Run();
		}		
	}

	/// <summary>
	/// Monogame CNC machine controller
	/// </summary>
	public class GUI : Game
	{	
		// User Interface Game
		GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Content
        ContentFactory content;
		
		// Input
		MouseStatus mouse;
		TimeStatus time;		
		
        /// <summary>
        /// Create instance of CNC Machine GUI
        /// </summary>
		public GUI()
		{			
			// Create GUI
			graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; 

            // Input
            this.mouse = new MouseStatus();
            this.time = new TimeStatus();
            Control.MouseService = this.mouse;
            Control.TimeService = this.time;
		}
		
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>        
        protected override void Initialize()
        {
			// Graphics settings
    		graphics.PreferredBackBufferWidth = 1200;
    		graphics.PreferredBackBufferHeight = 600;
    		graphics.IsFullScreen = false;
    		graphics.ApplyChanges();		
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Creates ContentFactory
            this.content = new Troopy.Xna.ContentFactory(Content);

        	// Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice, this.content);            
        }  

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        	//this.content.unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {   
        	// Update Input
        	this.mouse.update(Mouse.GetState());
        	this.time.update(gameTime);

        	// Exit
        	if(
            	GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            	Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
        		Exit();
            }
    	        
        	      	
        	        	
        	
			// Update in base class
            base.Update(gameTime);
        }  

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Init SpriteBatch
            //spriteBatch.Begin( SpriteSortMode.Deferred,null,SamplerState.LinearWrap);
            spriteBatch.Begin();
            spriteBatch.gameTime = gameTime;
            spriteBatch.mouse = Mouse.GetState();
            
            spriteBatch.End();

            base.Draw(gameTime);
        }        
	}
}