using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BabyGame
{
    public class BabyGame : Game
    {
        private Texture2D ballTexture, sunTexture, starTexture, truckTexture;
        private Vector2 ballPosition, sunPosition, starPosition, truckPosition;
        private float ballSpeed, sunSpeed, starSpeed, truckSpeed;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Color _backgroundColor;
        private float _transitionTime;
        private int _currentColorIndex;
        private List<Color> _happyColors;
        private KeyboardState _currentKeyState;
        private KeyboardState _previousKeyState;
        private Random _random;

        public BabyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _backgroundColor = Color.CornflowerBlue;
            _transitionTime = 0f;
            _currentColorIndex = 0;
            _random = new Random();
            InitializeHappyColors();
        }

        protected override void Initialize()
        {
            InitializePositionsAndSpeeds();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadTextures();
        }

        protected override void Update(GameTime gameTime)
        {
            _currentKeyState = Keyboard.GetState();

            UpdateBackgroundColor(gameTime);
            UpdateBallPosition(gameTime);
            UpdateTruckPosition(gameTime);
            UpdateSunPosition(gameTime);

            _previousKeyState = _currentKeyState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            _spriteBatch.Begin();
            DrawGameObjects();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitializeHappyColors()
        {
            _happyColors = new List<Color>
            {
                Color.LightPink,
                Color.LightBlue,
                Color.LightGreen,
                Color.LightYellow,
                Color.PeachPuff,
                Color.Lavender,
                Color.MistyRose
            };
        }

        private void InitializePositionsAndSpeeds()
        {
            // initialize the ball position and speed
            ballPosition = new Vector2((_graphics.PreferredBackBufferWidth / 4),
                (_graphics.PreferredBackBufferHeight));
            ballSpeed = 100f;

            // initialize the truck position and speed
            truckPosition = new Vector2(0,
            _graphics.PreferredBackBufferHeight - 84);
            truckSpeed = 25f;

            //initialize the sun position and speed
            sunPosition = new Vector2((_graphics.PreferredBackBufferWidth / 2), _graphics.PreferredBackBufferHeight);
            sunSpeed = 5f;
        }

        private void LoadTextures()
        {
            ballTexture = Content.Load<Texture2D>("ball_orange");
            sunTexture = Content.Load<Texture2D>("sun");
            truckTexture = Content.Load<Texture2D>("truck");
            starTexture = Content.Load<Texture2D>("star");
        }

        private void UpdateBackgroundColor(GameTime gameTime)
        {
            _transitionTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Change color every 5 seconds
            if (_transitionTime > 5f)
            {
                _transitionTime = 0f;
                _currentColorIndex = (_currentColorIndex + 1) % _happyColors.Count;
            }

            // Get the next color index
            int nextColorIndex = (_currentColorIndex + 1) % _happyColors.Count;

            // Interpolate between the current color and the next color
            float lerpAmount = _transitionTime / 5f;
            _backgroundColor = Color.Lerp(_happyColors[_currentColorIndex], _happyColors[nextColorIndex], lerpAmount);
        }

        private void UpdateTruckPosition(GameTime gameTime)
        {
            if (truckPosition.X > (_graphics.PreferredBackBufferWidth + (truckTexture.Width / 2)))
            {
                truckPosition = new Vector2(-100, _graphics.PreferredBackBufferHeight - 84);
            }
            else
            {
                truckPosition.X += truckSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        // update the sun position and speed
        private void UpdateSunPosition(GameTime gameTime)
        {
            // if the sun goes off the screen, reset it to the bottom of the screen
            if (sunPosition.Y < -100)
            {
                sunPosition = new Vector2((_graphics.PreferredBackBufferWidth / 2), _graphics.PreferredBackBufferHeight + 100);
            } else
            {
                sunPosition.Y -= sunSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void UpdateBallPosition(GameTime gameTime)
        {
            if (_currentKeyState.IsKeyDown(Keys.Up))
            {
                ballPosition.Y -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Down))
            {
                ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Left))
            {
                ballPosition.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Right))
            {
                ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            ClampBallPosition();
        }

        private void ClampBallPosition()
        {
            float halfBallWidth = (ballTexture.Width * 0.5f) / 2;
            float halfBallHeight = (ballTexture.Height * 0.5f) / 2;

            ballPosition.X = MathHelper.Clamp(ballPosition.X, halfBallWidth, _graphics.PreferredBackBufferWidth - halfBallWidth);
            ballPosition.Y = MathHelper.Clamp(ballPosition.Y, halfBallHeight, _graphics.PreferredBackBufferHeight - halfBallHeight);
        }

        private void DrawGameObjects()
        {
            DrawSun();
            DrawBall();
            DrawTruck();
        }

        private void DrawBall()
        {
            _spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f, new Vector2(ballTexture.Width / 2, ballTexture.Height / 2), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
        }

        private void DrawTruck()
        {
            _spriteBatch.Draw(truckTexture, truckPosition, null, Color.White, 0f, new Vector2(truckTexture.Width / 2, 0), Vector2.One, SpriteEffects.None, 0f);
        }

        private void DrawSun()
        {
            _spriteBatch.Draw(sunTexture, sunPosition, null, Color.White, 0f, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
