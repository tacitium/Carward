using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Carward
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Carward : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDevice device;
        GraphicsDeviceManager graphics;


        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        KeyboardState current;
        KeyboardState previous;

        enum gamestate
        {
            Mode,
            Level1,
            Level2,
            Level3,
            Level4,
            Main,
            Game,
            Instruction,
            Credit,
            Pause,
        }
        gamestate GameState;
        Texture2D mMain;
        Texture2D mPause;
        Texture2D mCredit;
        Texture2D mMode;
        Texture2D mInstruction;

        //Check for single click
        float KeyPressCheckDelay = 0.25f;
        float TotalElapsedTime = 0;
        private bool wasContinuePressed;

        // Options in menu list, pause list, mode list
        private int menuSelected = 1;
        private int pauseSelected = 1;
        private int modeSelected = 1;

        // Font
        private SpriteFont hudFont;
        private SpriteFont menuFont;
        private SpriteFont instructionFont;

        // Player car rotation
        private Vector2 origin;
        private float RotationAngle;
        private float speed = 10 / 400.0f;
        private float r;

        // Rectangle of player car
        Rectangle carRectangle;
        Rectangle easyEnemyCarRectangle;
        Rectangle mediumEnemyCarRectangle;
        Rectangle hardEnemyCarRectangle;
        Rectangle easyEnemyCarRectangle2;
        Rectangle mediumEnemyCarRectangle2;
        Rectangle hardEnemyCarRectangle2;
        Rectangle missileBoundRectangle;

        // The images we will draw
        Texture2D carTexture;
        Texture2D easyEnemyCarTexture;
        Texture2D mediumEnemyCarTexture;
        Texture2D hardEnemyCarTexture;
        Texture2D missileTexture;
        Texture2D starTexture;
        Texture2D roadTexture;
        Texture2D grassTexture;

        // The color data for the images; used for per pixel collision
        Color[] carTextureData;
        Color[] enemycarTextureData;
        Color[] starTextureData;
        Color[] missileTextureData;

        // Texture List
        List<Vector2> leftGrassPositions = new List<Vector2>();
        List<Vector2> rightGrassPositions = new List<Vector2>();
        List<Vector2> roadPositions = new List<Vector2>();
        int textureFallSpeed = 2;

        // The images will be drawn with this SpriteBatch
        SpriteBatch spriteBatch;

        // Player Car
        Vector2 carPosition;
        const int CarMoveSpeed = 5;

        // EnemyCars
        List<Vector2> easyEnemyCarPositions = new List<Vector2>();
        List<Vector2> mediumEnemyCarPositions = new List<Vector2>();
        List<Vector2> hardEnemyCarPositions = new List<Vector2>();
        float EnemyCarSpawnProbability = 0.005f;
        int EnemyCarFallSpeed = 3;

        // Star
        List<Vector2> starPositions = new List<Vector2>();
        float StarSpawnProbability = 0.0005f;
        int StarFallSpeed = 3;

        // Missile
        List<Vector2> missilePositions = new List<Vector2>();
        const int missileSpeed = 8;

        Random random = new Random();

        // For when a collision is detected
        //bool carHit = false;
        bool isDead = false;

        // The sub-rectangle of the drawable area which should be visible on all TVs
        Rectangle safeBounds;
        // Percentage of the screen on every side is the safe area
        const float SafeAreaPortionVertical = 0.05f;
        const float SafeAreaPortionHorizontal = 0.2f;

        int ammo = 9999;
        int star;
        int fuel = 210;
        int timeSecs, distSecs = 10;
        int life = 3;
        int km = 1;
        int distance, level = 0;
        int distLevel = 1;
        bool shoot, added, minused, addSpeed, minusSpeed, addLife, addPoints = false;

        public SpriteFont HudFont
        {
            get { return hudFont; }
            set { hudFont = value; }
        }

        public SpriteFont MenuFont
        {
            get { return menuFont; }
            set { menuFont = value; }
        }
        public SpriteFont InstructionFont
        {
            get { return instructionFont; }
            set { instructionFont = value; }
        }

        private void DrawMainScreen()
        {
            spriteBatch.Draw(mMain, Vector2.Zero, Color.White);
        }
        private void DrawPauseScreen()
        {
            spriteBatch.Draw(mPause, Vector2.Zero, Color.White);
        }
        private void DrawCredits()
        {
            spriteBatch.Draw(mCredit, Vector2.Zero, Color.White);
        }
        private void DrawInstruction()
        {
            spriteBatch.Draw(mInstruction, Vector2.Zero, Color.White);
        }

        private void DrawMode()
        {
            spriteBatch.Draw(mMode, Vector2.Zero, Color.White);
        }

        private void UpdateMainScreen()
        {
            current = Keyboard.GetState();

            if (current.IsKeyUp(Keys.Up) == true || previous.IsKeyDown(Keys.Up) == true)
            {
                menuSelected++;
            }
            if (current.IsKeyUp(Keys.Down) == true || previous.IsKeyDown(Keys.Down) == true)
            {
                menuSelected--;
            }
            if (menuSelected <= 0)
            {
                menuSelected = 5;
            }
            if (menuSelected >= 6)
            {
                menuSelected = 1;
            }
            if (menuSelected == 1 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                GameState = gamestate.Game;
                return;
            }
            if (menuSelected == 2 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                GameState = gamestate.Instruction;
                return;
            }
            if (menuSelected == 3 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                GameState = gamestate.Mode;
                return;
            }
            if (menuSelected == 4 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                GameState = gamestate.Credit;
                return;
            }
            if (menuSelected == 5 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                Exit();
                return;
            }

            previous = current;

        }
        private void UpdatePauseScreen()
        {
            current = Keyboard.GetState();

            if (current.IsKeyUp(Keys.Up) == true || previous.IsKeyDown(Keys.Up) == true)
            {
                pauseSelected++;
            }
            if (current.IsKeyUp(Keys.Down) == true || previous.IsKeyDown(Keys.Down) == true)
            {
                pauseSelected--;
            }
            if (pauseSelected <= 0)
            {
                pauseSelected = 3;
            }
            if (pauseSelected >= 4)
            {
                pauseSelected = 1;
            }

            if (pauseSelected == 1 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                GameState = gamestate.Game;
            }

            if (pauseSelected == 2 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                // Restart Level
            }
            if (pauseSelected == 3 && Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                Exit();
                return;
            }


            previous = current;

        }
        private void UpdateCredit()
        {
            current = Keyboard.GetState();

            if (current.IsKeyUp(Keys.Back) == true || previous.IsKeyDown(Keys.Back) == true)
            {
                UpdateMainScreen();
                GameState = gamestate.Main;
                return;
            }

        }
        private void UpdateInstruction()
        {
            current = Keyboard.GetState();

            if (current.IsKeyUp(Keys.Back) == true || previous.IsKeyDown(Keys.Back) == true)
            {
                UpdateMainScreen();
                GameState = gamestate.Main;
                return;
            }

        }


        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;

        public Carward()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
        }

        // randomize positions of falling obiects (into 8 lanes)
        public void randomizePositions()
        {
            int number = random.Next(8);
            // Spawn new falling enemycars
            if (random.NextDouble() < EnemyCarSpawnProbability)
            {
                float x = 170.0f + number * 60.0f;
                easyEnemyCarPositions.Add(new Vector2(x, -100.0f));
            }
            
            number = random.Next(8);
            if (random.NextDouble() < EnemyCarSpawnProbability)
            {
                float x = 170.0f + number * 60.0f;
                mediumEnemyCarPositions.Add(new Vector2(x, -100.0f));
            }
            number = random.Next(8);
            if (random.NextDouble() < EnemyCarSpawnProbability)
            {
                float x = 170.0f + number * 60.0f;
                hardEnemyCarPositions.Add(new Vector2(x, -100.0f));
            }

            // Spawn new falling star
            if (random.NextDouble() < StarSpawnProbability)
            {
                float x = 170.0f + number * 60.0f;
                starPositions.Add(new Vector2(x, 50.0f - starTexture.Height));
            }

            if (shoot == true && ammo > 0)
            {
                // Spawn missile
                missilePositions.Add(new Vector2(carPosition.X, carPosition.Y));
                ammo--;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to
        /// run. This is where it can query for any required services and load any
        /// non-graphic related content.  Calling base.Initialize will enumerate through
        /// any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Make mouse visible
            this.IsMouseVisible = true;

            // Calculate safe bounds based on current resolution
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            safeBounds = new Rectangle(
                (int)(viewport.Width * SafeAreaPortionHorizontal),
                (int)(viewport.Height * SafeAreaPortionVertical),
                (int)(viewport.Width * (1 - 2 * SafeAreaPortionHorizontal)),
                (int)(viewport.Height * (1 - 2 * SafeAreaPortionVertical)));
            // Start the player in the center along the bottom of the screen
            carPosition.X = 160.0f + (safeBounds.Width - carTexture.Width) / 2;
            carPosition.Y = safeBounds.Height - carTexture.Height;

            timeRemaining = TimeSpan.FromMinutes(0);

            // Spawn grass texture
            leftGrassPositions.Add(new Vector2(0, 600.0f - grassTexture.Height));
            leftGrassPositions.Add(new Vector2(0, 400.0f - grassTexture.Height));
            leftGrassPositions.Add(new Vector2(0, 200.0f - grassTexture.Height));
            leftGrassPositions.Add(new Vector2(0, 0.0f - grassTexture.Height));
            leftGrassPositions.Add(new Vector2(0, -200.0f - grassTexture.Height));
            leftGrassPositions.Add(new Vector2(0, -400.0f - grassTexture.Height));

            rightGrassPositions.Add(new Vector2(650.0f, 600.0f - grassTexture.Height));
            rightGrassPositions.Add(new Vector2(650.0f, 400.0f - grassTexture.Height));
            rightGrassPositions.Add(new Vector2(650.0f, 200.0f - grassTexture.Height));
            rightGrassPositions.Add(new Vector2(650.0f, 0.0f - grassTexture.Height));
            rightGrassPositions.Add(new Vector2(650.0f, -200.0f - grassTexture.Height));
            rightGrassPositions.Add(new Vector2(650.0f, -400.0f - grassTexture.Height));

            // Spawn road texture
            roadPositions.Add(new Vector2(150.0f, 0.0f));
            roadPositions.Add(new Vector2(150.0f, -600.0f));

        }
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load mainmenu
            GameState = gamestate.Main;


            //add all menu content
            mMain = Content.Load<Texture2D>("Images\\Main");
            mPause = Content.Load<Texture2D>("Images\\Main");
            mCredit = Content.Load<Texture2D>("Images\\Main");
            mMode = Content.Load<Texture2D>("Images\\Main");
            mInstruction = Content.Load<Texture2D>("Images\\Main");
            // Load fonts
            hudFont = Content.Load<SpriteFont>("Sprites/Hud");
            menuFont = Content.Load<SpriteFont>("Sprites/Menu");
            instructionFont = Content.Load<SpriteFont>("Sprites/Instruction");

            // Load textures
            easyEnemyCarTexture = Content.Load<Texture2D>("Images/EnemyCar1");
            mediumEnemyCarTexture = Content.Load<Texture2D>("Images/EnemyCar2");
            hardEnemyCarTexture = Content.Load<Texture2D>("Images/EnemyCar3");
            carTexture = Content.Load<Texture2D>("Images/PlayerCar");
            missileTexture = Content.Load<Texture2D>("Images/Missile");
            starTexture = Content.Load<Texture2D>("Images/Star");
            roadTexture = Content.Load<Texture2D>("Images/Road");
            grassTexture = Content.Load<Texture2D>("Images/Grass");

            // Extract collision data
            enemycarTextureData =
                new Color[easyEnemyCarTexture.Width * easyEnemyCarTexture.Height];
            easyEnemyCarTexture.GetData(enemycarTextureData);
            mediumEnemyCarTexture.GetData(enemycarTextureData);
            hardEnemyCarTexture.GetData(enemycarTextureData);

            carTextureData =
                new Color[carTexture.Width * carTexture.Height];
            carTexture.GetData(carTextureData);

            starTextureData =
                new Color[starTexture.Width * starTexture.Height];
            starTexture.GetData(starTextureData);

            missileTextureData =
                new Color[missileTexture.Width * missileTexture.Height];
            missileTexture.GetData(missileTextureData);

            origin.X = (carTexture.Width / carTexture.Width);
            origin.Y = (carTexture.Height / carTexture.Height);

            // Create a sprite batch to draw those textures
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Game time remaining
            timeRemaining += gameTime.ElapsedGameTime;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalElapsedTime += elapsed;
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GameState == gamestate.Main)
            {

                UpdateMainScreen();

            }
            else if (GameState == gamestate.Credit)
            {
                UpdateMainScreen();
            }
            else if (GameState == gamestate.Instruction)
            {
                UpdateMainScreen();
            }
            else if (GameState == gamestate.Game)
            {

                if (TotalElapsedTime >= KeyPressCheckDelay)
                {
                    if (currentKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        GameState = gamestate.Pause;
                    }

                }
            }

            else
            {
                if (TotalElapsedTime >= KeyPressCheckDelay)
                {
                    UpdatePauseScreen();
                    base.Update(gameTime);
                }
            }

            // Get input of player (Arrow keys and exit)
            HandleInput();

            // Handle collisions of spawns, missiles
            HandleCollisions();

            // Get the bounding rectangle of the car
            carRectangle = new Rectangle((int)carPosition.X, (int)carPosition.Y, carTexture.Width, carTexture.Height);

            // spawn enemy cars & power-ups..etc at random positions
            randomizePositions();

            // Update grass & road movement
            UpdateTextures();

            // Update each enemycar
            UpdateEnemyCars();

            // Update each star
            UpdateStars();

            // Update each missile
            UpdateMissiles();

            // Reset carHit to false
            //carHit = false;

            //determine the distance
            Distance();

            //determine level
            Level();

            //determine bonus
            Bonus();

            //determine the amount of fuel left
            Fuel();

            //determine speed
            Speed();

            // Death
            if (isDead == true)
            {
                Reset();
            }
            if (life < 1)
                isDead = true;
       
            // Prevent the car from moving off of the boundaries
            carPosition.X = MathHelper.Clamp(carPosition.X,
                10.0f + safeBounds.Left, safeBounds.Right - carTexture.Width - 10.0f);
            carPosition.Y = MathHelper.Clamp(carPosition.Y,
                safeBounds.Top, safeBounds.Bottom - carTexture.Height);

            base.Update(gameTime);
        }
        private void Reset()
        {
            timeRemaining = TimeSpan.FromMinutes(0.0);
            life = 3;
            fuel = 200;
            star = 0;
        }
        private void Level()
        {
            if (distance == distLevel)
            {
                level++;
                EnemyCarFallSpeed++;
                //textureFallSpeed++;
                distLevel +=2;
                star += 200;
                if (level % 2 == 0)
                    addLife = true;
                if (level % 3 == 0)
                    addPoints = true;
            }
        }
        private void Fuel()
        {
            if (timeRemaining.Seconds.Equals(timeSecs))
            {
                fuel -= 10;
                timeSecs += 10;
                if (timeSecs == 60)
                    timeSecs = 10;
            }
        }
        private void Distance()
        {
            if (timeRemaining.Seconds.Equals(distSecs))
            {
                distance += km;
                distSecs += 10;
                if (distSecs == 60)
                    distSecs = 10;
            }
        }
        private void Bonus()
        {
            //int bonusLife = 2;
            if (addLife == true)
            {
                life++;
                addLife = false;
            }

            if (addPoints == true)
            {
                star += 1000;
                addPoints = false;
            }
        }
        private void Speed()
        {
            while (addSpeed == true && added == false)
            {
                StarFallSpeed++;
                textureFallSpeed++;
                EnemyCarFallSpeed++;
                added = true;
            }
            if (addSpeed == false && added == true)
            {
                StarFallSpeed--;
                textureFallSpeed--;
                EnemyCarFallSpeed--;
                added = false;
            }
            while (minusSpeed == true && minused == false)
            {
                StarFallSpeed--;
                textureFallSpeed--;
                EnemyCarFallSpeed--;
                minused = true;
            }
            if (minusSpeed == false && minused == true)
            {
                StarFallSpeed++;
                textureFallSpeed++;
                EnemyCarFallSpeed++;
                minused = false;
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice device = graphics.GraphicsDevice;

            if (GameState == gamestate.Main)
            {

                spriteBatch.Begin();
                DrawMainScreen();

               
                spriteBatch.DrawString(menuFont, "Start game", new Vector2(300, 160), Color.White);
                spriteBatch.DrawString(menuFont, "Instruction", new Vector2(300, 200), Color.White);
                spriteBatch.DrawString(menuFont, "Level Select", new Vector2(300, 240), Color.White);
                spriteBatch.DrawString(menuFont, "Credits", new Vector2(300, 280), Color.White);
                spriteBatch.DrawString(menuFont, "Exit", new Vector2(300, 320), Color.White);



                if (menuSelected == 1)
                {
                    spriteBatch.DrawString(menuFont, "Start game", new Vector2(300, 160), Color.Red);
                }
                if (menuSelected == 2)
                {
                    spriteBatch.DrawString(menuFont, "Instruction", new Vector2(300, 200), Color.Red);
                }
                if (menuSelected == 3)
                {
                    spriteBatch.DrawString(menuFont, "Level Select", new Vector2(300, 240), Color.Red);
                }
                if (menuSelected == 4)
                {
                    spriteBatch.DrawString(menuFont, "Credits", new Vector2(300, 280), Color.Red);
                }
                if (menuSelected == 5)
                {
                    spriteBatch.DrawString(menuFont, "Exit", new Vector2(300, 320), Color.Red);
                }
                spriteBatch.End();

            }
            else if (GameState == gamestate.Credit)
            {
                current = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Back))
                {
                    spriteBatch.Begin();
                    DrawMainScreen();
                    GameState = gamestate.Main;
                    spriteBatch.End();
                }

                else
                {
                    spriteBatch.Begin();
                    DrawCredits();

                    spriteBatch.DrawString(MenuFont, "Carward Production :", new Vector2(230, 160), Color.Red);
                    spriteBatch.DrawString(MenuFont, "Kong Boon Jun", new Vector2(230, 200), Color.White);
                    spriteBatch.DrawString(MenuFont, "Terence Tan", new Vector2(230, 240), Color.White);
                    spriteBatch.DrawString(MenuFont, "Khairunnisa", new Vector2(230, 280), Color.White);
                    spriteBatch.DrawString(MenuFont, "Paul Lim", new Vector2(230, 320), Color.White);
                    spriteBatch.End();



                }
            }
            else if (GameState == gamestate.Instruction)
            {
                current = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Back) == true)
                {
                    spriteBatch.Begin();
                    DrawMainScreen();
                    GameState = gamestate.Main;
                    spriteBatch.End();
                }

                else
                {
                    spriteBatch.Begin();
                    DrawInstruction();

                    spriteBatch.DrawString(instructionFont, "Instruction :", new Vector2(340, 160), Color.Red);
                    spriteBatch.DrawString(instructionFont, "The player controls a car and has to avoid enemy cars coming from a", new Vector2(70, 200), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "different direction. There will level increment as distance/time passes.", new Vector2(70, 220), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "The cars will start out slow. The level of difficulty will gradually", new Vector2(70, 240), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "increase and enemy cars speed will also increase. After a certain level", new Vector2(70, 260), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "the player will have to face a boss stage where player has to evade police", new Vector2(70, 280), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "cars/trucks for a certain period of time. Throughout the game,the player", new Vector2(70, 300), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "car will have limited fuel and must collect stars[points] in order to buy", new Vector2(70, 320), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "fuels. Points can also be used to buy a weapons/armours. The player will", new Vector2(70, 340), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "have 3 lives.", new Vector2(70, 360), Color.Yellow);
                    spriteBatch.DrawString(instructionFont, "Key Up    - Acceleration", new Vector2(70, 400), Color.Red);
                    spriteBatch.DrawString(instructionFont, "Key Down  - Slow down", new Vector2(70, 420), Color.Red);
                    spriteBatch.DrawString(instructionFont, "Key Left  - Drift towards left", new Vector2(70, 440), Color.Red);
                    spriteBatch.DrawString(instructionFont, "Key Right - Drift towards right", new Vector2(70, 460), Color.Red);
                    spriteBatch.DrawString(instructionFont, "Key T     - Fire missile", new Vector2(70, 480), Color.Red);

                    spriteBatch.End();



                }
            }


            else if (GameState == gamestate.Pause)
            {
                spriteBatch.Begin();
                DrawPauseScreen();

                spriteBatch.DrawString(menuFont, "Resume Game", new Vector2(280, 200), Color.White);
                spriteBatch.DrawString(menuFont, "Restart Level", new Vector2(280, 240), Color.White);
                spriteBatch.DrawString(menuFont, "Exit", new Vector2(280, 280), Color.White);


                if (pauseSelected == 1)
                {
                    spriteBatch.DrawString(menuFont, "Resume Game", new Vector2(280, 200), Color.Red);
                }
                if (pauseSelected == 2)
                {
                    spriteBatch.DrawString(menuFont, "Restart Level", new Vector2(280, 240), Color.Red);
                }
                if (pauseSelected == 3)
                {
                    spriteBatch.DrawString(menuFont, "Exit", new Vector2(280, 280), Color.Red);
                }
                spriteBatch.End();
            }

            /*// Change the background to red when the car was hit by a enemycar
            if (carHit)
            {
                device.Clear(Color.Red);
            }
            else
            {
                device.Clear(Color.Black);
            }*/
            if (GameState == gamestate.Game)
            {
                spriteBatch.Begin();
                // Draw grass texture
                foreach (Vector2 grassPosition in leftGrassPositions)
                    spriteBatch.Draw(grassTexture, grassPosition, Color.White);
                foreach (Vector2 grassPosition in rightGrassPositions)
                    spriteBatch.Draw(grassTexture, grassPosition, Color.White);
                foreach (Vector2 roadPosition in roadPositions)
                    spriteBatch.Draw(roadTexture, roadPosition, Color.White);

                // Draw HUD
                DrawHud();

                // Draw car
                spriteBatch.Draw(carTexture, carPosition, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);

                // Draw enemycars
                foreach (Vector2 enemycarPosition in easyEnemyCarPositions)
                {
                    spriteBatch.Draw(easyEnemyCarTexture, enemycarPosition, Color.White);
                }
                foreach (Vector2 enemycarPosition in mediumEnemyCarPositions)
                {
                    spriteBatch.Draw(mediumEnemyCarTexture, enemycarPosition, Color.White);
                }
                foreach (Vector2 enemycarPosition in hardEnemyCarPositions)
                {
                    spriteBatch.Draw(hardEnemyCarTexture, enemycarPosition, Color.White);
                }

                // Draw star
                foreach (Vector2 starPosition in starPositions)
                    spriteBatch.Draw(starTexture, starPosition, Color.White);

                // Draw missile
                foreach (Vector2 missilePosition in missilePositions)
                    spriteBatch.Draw(missileTexture, missilePosition, Color.White);


                spriteBatch.End();
            }


            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            string distStr = "Distance: " + distance + " km";
            float timeWidth = hudFont.MeasureString(distStr).X;

            DrawShadowedString(hudFont, distStr, hudLocation, Color.AntiqueWhite);
            // Draw life
            DrawShadowedString(hudFont, "Life: " + life.ToString(), hudLocation + new Vector2(0.0f, 15.0f), Color.AntiqueWhite);
            // Draw star
            DrawShadowedString(hudFont, "Points: " + star.ToString(), hudLocation + new Vector2(0.0f, 30.0f), Color.AntiqueWhite);
            // Draw fuel
            DrawShadowedString(hudFont, "Fuel: " + fuel.ToString(), hudLocation + new Vector2(650.0f, 0.0f), Color.AntiqueWhite);
            // Draw level
            DrawShadowedString(hudFont, "Level: " + level.ToString(), hudLocation + new Vector2(650.0f, 15.0f), Color.AntiqueWhite);
            // Draw ammo
            DrawShadowedString(hudFont, "Ammo: " + ammo.ToString(), hudLocation + new Vector2(650.0f, 30.0f), Color.AntiqueWhite);
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }

        // Determines if there is overlap of the non-transparent pixels between two sprites.
        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        private void HandleInput()
        {
            // Get input
            KeyboardState keyboard = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            //MouseState mouseStateCurrent, mouseStatePrevious;

            // Allows the game to pause
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                GameState = gamestate.Pause;
            }

            // Determining speed increment/decrement when moving up and down
            KeyboardState currentState = Keyboard.GetState();
            
            if (currentState.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            {
                addSpeed = true;
            }
            else
                addSpeed = false;

            if (currentState.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            {
                minusSpeed = true;
            }
            else
                minusSpeed = false;

            if (!currentState.IsKeyDown(Keys.P) && previous.IsKeyDown(Keys.P))
            {
                if (star > 0)
                {
                    ammo += 1;
                    star -= 50;
                }
            }

            if (!currentState.IsKeyDown(Keys.T) && previous.IsKeyDown(Keys.T))
            {
                shoot = true;
            }
            else
                shoot = false;
            
            previous = currentState;
            

            // Move the player left and right with arrow keys or d-pad
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            {
                carPosition.Y -= CarMoveSpeed;
                if (RotationAngle < 0 || RotationAngle > 0)
                    RotationAngle = 0;
            }
            else if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            {
                carPosition.Y += CarMoveSpeed;
                if (RotationAngle < 0 || RotationAngle > 0)
                    RotationAngle = 0;
            }
            else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                if (RotationAngle > 0)
                    RotationAngle = 0;
                carPosition.X -= CarMoveSpeed;
                r = -speed * 0.7f;
            }
            else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                if (RotationAngle < 0)
                    RotationAngle = 0;
                carPosition.X += CarMoveSpeed;
                r = speed * 0.7f;
            }
            else
            {
                RotationAngle = 0;
                r = 0;
            }

            if (RotationAngle < speed * 10 && RotationAngle > -speed * 10)
            {
                RotationAngle += r;
                RotationAngle = RotationAngle % (MathHelper.Pi * 2.0f);
            }
        }

        // Method to update all star positions
        private void UpdateStars()
        {
            for (int a = 0; a < starPositions.Count; a++)
            {
                // Animate this star falling
                starPositions[a] =
                    new Vector2(starPositions[a].X,
                                starPositions[a].Y + StarFallSpeed);

                // Get the bounding rectangle of this enemycar
                Rectangle starRectangle =
                    new Rectangle((int)starPositions[a].X, (int)starPositions[a].Y,
                    starTexture.Width, starTexture.Height);

                // Check collision with car
                if (IntersectPixels(carRectangle, carTextureData,
                                    starRectangle, starTextureData))
                {
                    star += 50;
                    starPositions.RemoveAt(a);
                    a--;
                }

                // Remove this star if it have fallen off the screen
                else if (starPositions[a].Y > Window.ClientBounds.Height)
                {
                    starPositions.RemoveAt(a);

                    // When removing a enemycar, the next enemycar will have the same index
                    // as the current enemycar. Decrement i to prevent skipping a enemycar.
                    a--;
                }
            }
        }

        // Method to update all enemy car positions
        private void UpdateEnemyCars()
        {
            // ============================= EASY ENEMY CAR ============================= //
            for (int i = 0; i < easyEnemyCarPositions.Count; i++)
            {
                // Animate this enemycar falling
                easyEnemyCarPositions[i] =
                    new Vector2(easyEnemyCarPositions[i].X,
                                easyEnemyCarPositions[i].Y + EnemyCarFallSpeed);

                // Get the bounding rectangle of this enemycar
                Rectangle enemycarRectangle =
                    new Rectangle((int)easyEnemyCarPositions[i].X, (int)easyEnemyCarPositions[i].Y,
                    easyEnemyCarTexture.Width, easyEnemyCarTexture.Height);

                // Check collision with car
                if (IntersectPixels(carRectangle, carTextureData,
                                    enemycarRectangle, enemycarTextureData))
                {
                    //carHit = true;
                    easyEnemyCarPositions.RemoveAt(i);
                    i--;
                    life--;
                }

                // Remove this enemycar if it have fallen off the screen
                else if (easyEnemyCarPositions[i].Y > Window.ClientBounds.Height)
                {
                    easyEnemyCarPositions.RemoveAt(i);

                    // When removing a enemycar, the next enemycar will have the same index
                    // as the current enemycar. Decrement i to prevent skipping a enemycar.
                    i--;
                }
            }
            // ============================= MEDIUM ENEMY CAR ============================= //
            for (int i = 0; i < mediumEnemyCarPositions.Count; i++)
            {
                // Animate this enemycar falling
                mediumEnemyCarPositions[i] =
                    new Vector2(mediumEnemyCarPositions[i].X,
                                mediumEnemyCarPositions[i].Y + EnemyCarFallSpeed + 1); // increase speed by +1

                // Get the bounding rectangle of this enemycar
                Rectangle enemycarRectangle =
                    new Rectangle((int)mediumEnemyCarPositions[i].X, (int)mediumEnemyCarPositions[i].Y,
                    mediumEnemyCarTexture.Width, mediumEnemyCarTexture.Height);

                // Check collision with car
                if (IntersectPixels(carRectangle, carTextureData,
                                    enemycarRectangle, enemycarTextureData))
                {
                    //carHit = true;
                    mediumEnemyCarPositions.RemoveAt(i);
                    i--;
                    life--;
                }

                // Remove this enemycar if it have fallen off the screen
                else if (mediumEnemyCarPositions[i].Y > Window.ClientBounds.Height)
                {
                    mediumEnemyCarPositions.RemoveAt(i);

                    // When removing a enemycar, the next enemycar will have the same index
                    // as the current enemycar. Decrement i to prevent skipping a enemycar.
                    i--;
                }
            }
            // ============================= HARD ENEMY CAR ============================= //
            for (int i = 0; i < hardEnemyCarPositions.Count; i++)
            {
                // Animate this enemycar falling
                hardEnemyCarPositions[i] =
                    new Vector2(hardEnemyCarPositions[i].X,
                                hardEnemyCarPositions[i].Y + EnemyCarFallSpeed + 2); // increase speed by +2

                // Get the bounding rectangle of this enemycar
                Rectangle enemycarRectangle =
                    new Rectangle((int)hardEnemyCarPositions[i].X, (int)hardEnemyCarPositions[i].Y,
                    hardEnemyCarTexture.Width, hardEnemyCarTexture.Height);

                // Check collision with car
                if (IntersectPixels(carRectangle, carTextureData,
                                    enemycarRectangle, enemycarTextureData))
                {
                    //carHit = true;
                    hardEnemyCarPositions.RemoveAt(i);
                    i--;
                    life--;
                }
                // Remove this enemycar if it have fallen off the screen
                else if (hardEnemyCarPositions[i].Y > Window.ClientBounds.Height)
                {
                    hardEnemyCarPositions.RemoveAt(i);

                    // When removing a enemycar, the next enemycar will have the same index
                    // as the current enemycar. Decrement i to prevent skipping a enemycar.
                    i--;
                }
            }
        }

        // Method to update textures (Grass and road)
        private void UpdateTextures()
        {
            // Update grass positions within boundaries
            if (leftGrassPositions[2].Y >= 600.0f || rightGrassPositions[2].Y >= 600.0f)
            {
                leftGrassPositions[0] = new Vector2(leftGrassPositions[1].X, 0.0f - grassTexture.Height);
                leftGrassPositions[1] = new Vector2(leftGrassPositions[1].X, -200.0f - grassTexture.Height);
                leftGrassPositions[2] = new Vector2(leftGrassPositions[1].X, -400.0f - grassTexture.Height);
                rightGrassPositions[0] = new Vector2(rightGrassPositions[1].X, 0.0f - grassTexture.Height);
                rightGrassPositions[1] = new Vector2(rightGrassPositions[1].X, -200.0f - grassTexture.Height);
                rightGrassPositions[2] = new Vector2(rightGrassPositions[1].X, -400.0f - grassTexture.Height);
            }
            if (leftGrassPositions[5].Y >= 600.0f || rightGrassPositions[5].Y >= 600.0f)
            {
                leftGrassPositions[3] = new Vector2(leftGrassPositions[1].X, 0.0f - grassTexture.Height);
                leftGrassPositions[4] = new Vector2(leftGrassPositions[1].X, -200.0f - grassTexture.Height);
                leftGrassPositions[5] = new Vector2(leftGrassPositions[1].X, -400.0f - grassTexture.Height);
                rightGrassPositions[3] = new Vector2(rightGrassPositions[1].X, 0.0f - grassTexture.Height);
                rightGrassPositions[4] = new Vector2(rightGrassPositions[1].X, -200.0f - grassTexture.Height);
                rightGrassPositions[5] = new Vector2(rightGrassPositions[1].X, -400.0f - grassTexture.Height);
            }
            for (int i = 0; i < leftGrassPositions.Count; i++)
            {
                leftGrassPositions[i] = new Vector2(leftGrassPositions[i].X, leftGrassPositions[i].Y + textureFallSpeed);
                rightGrassPositions[i] = new Vector2(rightGrassPositions[i].X, rightGrassPositions[i].Y + textureFallSpeed);
            }

            if (roadPositions[1].Y >= 600.0f)
                roadPositions[1] = new Vector2(roadPositions[1].X, -600.0f);
            if (roadPositions[0].Y >= 600.0f)
                roadPositions[0] = new Vector2(roadPositions[0].X, -600.0f);
            // Update road positions within boundaries
            for (int i = 0; i < roadPositions.Count; i++)
            {
                roadPositions[i] = new Vector2(roadPositions[i].X, roadPositions[i].Y + textureFallSpeed);
            }
        }

        // Method to update all star positions
        private void UpdateMissiles()
        {
            for (int a = 0; a < missilePositions.Count; a++)
            {
                // Animate this star falling
                missilePositions[a] =
                    new Vector2(missilePositions[a].X,
                                missilePositions[a].Y - missileSpeed);

                // Get the bounding rectangle of this enemycar
                Rectangle missileRectangle =
                    new Rectangle((int)missilePositions[a].X, (int)missilePositions[a].Y,
                    missileTexture.Width, missileTexture.Height);

                // Remove this missile if it have fallen off the screen
                if (missilePositions[a].Y > Window.ClientBounds.Height)
                {
                    missilePositions.RemoveAt(a);

                    // When removing a enemycar, the next enemycar will have the same index
                    // as the current enemycar. Decrement i to prevent skipping a enemycar.
                    a--;
                    shoot = false;
                }
            }
        }

        private void HandleCollisions()
        {
            for (int i = easyEnemyCarPositions.Count - 1; i >= easyEnemyCarPositions.Count/2; i--)
            {
                easyEnemyCarRectangle2 =
                    new Rectangle((int)easyEnemyCarPositions[i].X, (int)easyEnemyCarPositions[i].Y,
                    easyEnemyCarTexture.Width, easyEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                easyEnemyCarRectangle2, enemycarTextureData))
                {
                    easyEnemyCarPositions.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < easyEnemyCarPositions.Count/2; i++)
            {
                // Get the bounding rectangle of this enemycar
                easyEnemyCarRectangle =
                    new Rectangle((int)easyEnemyCarPositions[i].X, (int)easyEnemyCarPositions[i].Y,
                    easyEnemyCarTexture.Width, easyEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                easyEnemyCarRectangle, enemycarTextureData))
                {
                    easyEnemyCarPositions.RemoveAt(i);
                    i--;
                }
                if (IntersectPixels(mediumEnemyCarRectangle, enemycarTextureData, easyEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(hardEnemyCarRectangle, enemycarTextureData, easyEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(easyEnemyCarRectangle2, enemycarTextureData, easyEnemyCarRectangle, enemycarTextureData))
                {
                    easyEnemyCarPositions[i] =
                    new Vector2(easyEnemyCarPositions[i].X, -100.0f);
                }
            }

            for (int i = mediumEnemyCarPositions.Count - 1; i >= mediumEnemyCarPositions.Count/2; i--)
            {
                mediumEnemyCarRectangle2 =
                    new Rectangle((int)mediumEnemyCarPositions[i].X, (int)mediumEnemyCarPositions[i].Y,
                    mediumEnemyCarTexture.Width, mediumEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                mediumEnemyCarRectangle2, enemycarTextureData))
                {
                    mediumEnemyCarPositions.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < mediumEnemyCarPositions.Count/2; i++)
            {
                // Get the bounding rectangle of this enemycar
                mediumEnemyCarRectangle =
                    new Rectangle((int)mediumEnemyCarPositions[i].X, (int)mediumEnemyCarPositions[i].Y,
                    mediumEnemyCarTexture.Width, mediumEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                mediumEnemyCarRectangle, enemycarTextureData))
                {
                    mediumEnemyCarPositions.RemoveAt(i);
                    i--;
                }
                if (IntersectPixels(easyEnemyCarRectangle, enemycarTextureData, mediumEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(hardEnemyCarRectangle, enemycarTextureData, mediumEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(mediumEnemyCarRectangle2, enemycarTextureData, mediumEnemyCarRectangle, enemycarTextureData))
                {
                    mediumEnemyCarPositions[i] =
                    new Vector2(mediumEnemyCarPositions[i].X, -100.0f);
                }
            }

            for (int i = hardEnemyCarPositions.Count - 1; i >= hardEnemyCarPositions.Count/2; i--)
            {
                hardEnemyCarRectangle2 =
                    new Rectangle((int)hardEnemyCarPositions[i].X, (int)hardEnemyCarPositions[i].Y,
                    hardEnemyCarTexture.Width, hardEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                hardEnemyCarRectangle2, enemycarTextureData))
                {
                    hardEnemyCarPositions.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < hardEnemyCarPositions.Count/2; i++)
            {
                // Get the bounding rectangle of this enemycar
                hardEnemyCarRectangle =
                    new Rectangle((int)hardEnemyCarPositions[i].X, (int)hardEnemyCarPositions[i].Y,
                    hardEnemyCarTexture.Width, hardEnemyCarTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData,
                                hardEnemyCarRectangle, enemycarTextureData))
                {
                    hardEnemyCarPositions.RemoveAt(i);
                    i--;
                }
                if (IntersectPixels(easyEnemyCarRectangle, enemycarTextureData, hardEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(mediumEnemyCarRectangle, enemycarTextureData, hardEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(hardEnemyCarRectangle2, enemycarTextureData, hardEnemyCarRectangle, enemycarTextureData))
                {
                    hardEnemyCarPositions[i] =
                    new Vector2(hardEnemyCarPositions[i].X, -100.0f);
                }
            }

            for (int a = 0; a < missilePositions.Count; a++)
            {
                // Get the bounding rectangle of this missile
                missileBoundRectangle =
                    new Rectangle((int)missilePositions[a].X, (int)missilePositions[a].Y,
                    missileTexture.Width, missileTexture.Height);

                if (IntersectPixels(missileBoundRectangle, missileTextureData, easyEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(missileBoundRectangle, missileTextureData, easyEnemyCarRectangle2, enemycarTextureData)
                    || IntersectPixels(missileBoundRectangle, missileTextureData, mediumEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(missileBoundRectangle, missileTextureData, mediumEnemyCarRectangle2, enemycarTextureData)
                    || IntersectPixels(missileBoundRectangle, missileTextureData, hardEnemyCarRectangle, enemycarTextureData)
                    || IntersectPixels(missileBoundRectangle, missileTextureData, hardEnemyCarRectangle2, enemycarTextureData))
                {
                    missilePositions.RemoveAt(a);
                    a--;
                    shoot = false;
                }
            }

        }
    }
}
