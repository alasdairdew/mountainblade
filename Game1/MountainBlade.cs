using MountainBlade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MountainBlade
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            State.CurrentState = new BattleState();

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Sprites.OrangeTeam = Content.Load<Texture2D>("orangeteam");
            Sprites.BlueTeam = Content.Load<Texture2D>("greenteam");
            Sprites.QBox = Content.Load<Texture2D>("qBox");
            Sprites.Arrow = Content.Load<Texture2D>("arrow");
            Sprites.Target = Content.Load<Texture2D>("target");
            Sprites.Blood = Content.Load<Texture2D>("Blood");
            Sprites.Troop = Content.Load<Texture2D>("troop3");
            Sprites.ArrowSmall = Content.Load<Texture2D>("arrow2");
            Sprites.DeadTroop = Content.Load<Texture2D>("deadtroop5");
            Sprites.Bow = Content.Load<Texture2D>("bow3");
            Sprites.Sword = Content.Load<Texture2D>("sword");
            Sprites.Horse = Content.Load<Texture2D>("horse");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            State.CurrentState.Update();



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkOliveGreen);

            spriteBatch.Begin();

            State.CurrentState.Draw(spriteBatch);

            spriteBatch.End();

            //base.Draw(gameTime);
        }
    }

    public abstract class State
    {

        public static State CurrentState { get; set; } = null;

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}


