﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace BabyGame
{
    public class BabyGame : Game
    {
        private Texture2D babyShipTexture, sunTexture, starTexture, truckTexture;
        private Vector2 babyShipPosition, sunPosition, starPosition, truckPosition;
        bool isBabyShipMoving = false;
        // effect variables for vehicles
        float bobbleAmplitude = 1f; // Amplitude of the bobble effect
        float bobbleSpeed = 10f; // Speed of the bobble effect
        float rotationAmplitude = 0.015f; // Amplitude of the rotation effect (in radians)
        float rotationSpeed = 0.5f; // Speed of the rotation effect
        float rotationAngle = 0f; // Current rotation angle
        float sineTime = 0f; // Time variable to track the sine wave

        private GraphicsDeviceManager _graphics;
        (int Width, int Height, int X, int Y) screenSize = ScreenHelper.GetTotalScreenSize();
        private SpriteBatch _spriteBatch;
        private Color _backgroundColor;
        private float _transitionTime;
        private int _currentColorIndex;
        private List<Color> _happyColors;
        private KeyboardState _currentKeyState;
        private KeyboardState _previousKeyState;
        private Random _random;

        private List<(Vector2 Position, float Timer)> starPositions;
        private const float StarLifespan = 4f; // Each star lasts for 5 seconds
        private const int MaxStars = 15; // Maximum number of stars on screen
        private float babyShipSpeed = 100f, sunSpeed = 5f, starSpeed, truckSpeed = 30f;
        private int sunDivider = 3, babyShipDivider = 4;
        // Audio assets
        private Song backgroundMusic;
        private SoundEffect babyShipSound;
        private SoundEffectInstance babyShipSoundInstance;
        private SoundEffect starAppearPopSound;
        private SoundEffectInstance starAppearPopSoundInstance;

        public BabyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = true;
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;

            _backgroundColor = Color.CornflowerBlue;
            _transitionTime = 0f;
            _currentColorIndex = 0;
            _random = new Random();
            InitializeHappyColors();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Set the preferred back buffer size to the total screen size
            SetFullScreenSizeAcrossAllMonitors();

            InitializePositionsAndSpeeds();
        }

        private void SetFullScreenSizeAcrossAllMonitors()
        {
            Window.Position = new Point(screenSize.X, screenSize.Y);

            _graphics.PreferredBackBufferWidth = screenSize.Width;
            _graphics.PreferredBackBufferHeight = screenSize.Height;
            _graphics.ApplyChanges();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadTextures();
            LoadAudioAssets();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currentKeyState = Keyboard.GetState();

            // if the user presses the escape key & x, exit the game
            if (_currentKeyState.IsKeyDown(Keys.Escape) && _currentKeyState.IsKeyDown(Keys.X))
                Exit();

            UpdateBackgroundColor(gameTime);
            UpdateBabyShipPosition(gameTime);
            UpdateTruckPosition(gameTime);
            UpdateSunPosition(gameTime);
            UpdateStarPositions(gameTime);

            _previousKeyState = _currentKeyState;
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

        private void LoadAudioAssets()
        {
            // load the background music
            backgroundMusic = Content.Load<Song>("23-Arriva-Caribe_Full_FM2135");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            // load the spacehip sound effect
            babyShipSound = Content.Load<SoundEffect>("BweepBeepLoop");
            babyShipSoundInstance = babyShipSound.CreateInstance();
            babyShipSoundInstance.IsLooped = true;
            babyShipSoundInstance.Volume = 0.15f;

            // load the star appear sound effect
            starAppearPopSound = Content.Load<SoundEffect>("BubblePopBloopy");
            starAppearPopSoundInstance = starAppearPopSound.CreateInstance();
            starAppearPopSoundInstance.Volume = 0.8f;
        }

        private void InitializePositionsAndSpeeds()
        {
            // initialize the baby ship position and speed
            babyShipPosition = new Vector2((_graphics.PreferredBackBufferWidth / babyShipDivider),
                (_graphics.PreferredBackBufferHeight));

            // initialize the truck position and speed
            truckPosition = new Vector2(0,
            _graphics.PreferredBackBufferHeight - 84);

            //initialize the sun position and speed
            sunPosition = new Vector2((_graphics.PreferredBackBufferWidth / sunDivider), _graphics.PreferredBackBufferHeight);

            // initialize the star position and speed
            starPositions = new List<(Vector2 Position, float Timer)>();
        }

        private void LoadTextures()
        {
            babyShipTexture = Content.Load<Texture2D>("babyship");
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
                var baseY = _graphics.PreferredBackBufferHeight - 80;
                // Update the time variable
                sineTime += (float)gameTime.ElapsedGameTime.TotalSeconds * bobbleSpeed;
                // Calculate the vertical offset for bumpy road sim. using a sine wave
                float verticalOffset = (float)Math.Sin(sineTime) * bobbleAmplitude;
                // Calculate the rotation angle for bumpy road sim. using a sine wave
                rotationAngle = (float)Math.Sin(sineTime * rotationSpeed) * rotationAmplitude;
                // update the truck position and speed
                truckPosition.Y = baseY + verticalOffset; // 100 is the base Y position
                truckPosition.X += truckSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        // update the sun position and speed
        private void UpdateSunPosition(GameTime gameTime)
        {
            // if the sun goes off the screen, reset it to the bottom of the screen
            if (sunPosition.Y < -100)
            {
                sunPosition = new Vector2((_graphics.PreferredBackBufferWidth / sunDivider), _graphics.PreferredBackBufferHeight + 100);
            } else
            {
                sunPosition.Y -= sunSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void UpdateBabyShipPosition(GameTime gameTime)
        {
            isBabyShipMoving = false;
            if (_currentKeyState.IsKeyDown(Keys.Up))
            {
                isBabyShipMoving = true;
                babyShipPosition.Y -= babyShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Down))
            {
                isBabyShipMoving = true;
                babyShipPosition.Y += babyShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Left))
            {
                isBabyShipMoving = true;
                babyShipPosition.X -= babyShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_currentKeyState.IsKeyDown(Keys.Right))
            {
                isBabyShipMoving = true;
                babyShipPosition.X += babyShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (isBabyShipMoving)
            {
                if (babyShipSoundInstance.State != SoundState.Playing)
                {
                    babyShipSoundInstance.Play();
                }
            }
            else
            {
                babyShipSoundInstance.Stop();
            }

            ClampBabyShipPosition();
        }

        private void ClampBabyShipPosition()
        {
            float halfBabyShipWidth = (babyShipTexture.Width * 0.5f) / 2;
            float halfBabyShipHeight = (babyShipTexture.Height * 0.5f) / 2;

            babyShipPosition.X = MathHelper.Clamp(babyShipPosition.X, halfBabyShipWidth, _graphics.PreferredBackBufferWidth - halfBabyShipWidth);
            babyShipPosition.Y = MathHelper.Clamp(babyShipPosition.Y, halfBabyShipHeight, _graphics.PreferredBackBufferHeight - halfBabyShipHeight);
        }

        private void DrawGameObjects()
        {
            DrawSun();
            DrawBabyShip();
            DrawTruck();
            DrawStars();
        }

        private void DrawStars()
        {
            foreach (var star in starPositions)
            {
                _spriteBatch.Draw(
                    starTexture,
                    star.Position,
                    null,
                    Color.White,
                    0f,
                    new Vector2(starTexture.Width / 2, starTexture.Height / 2),
                    new Vector2(0.25f, 0.25f), // Scale down to 1/4 size
                    SpriteEffects.None,
                    0f
                );
            }
        }

        private void DrawBabyShip()
        {
            _spriteBatch.Draw(babyShipTexture, babyShipPosition, null, Color.White, 0f, new Vector2(babyShipTexture.Width / 2, babyShipTexture.Height / 2), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
        }

        private void DrawTruck()
        {
            _spriteBatch.Draw(truckTexture, truckPosition, null, Color.White, rotationAngle, new Vector2(truckTexture.Width / 2, 0), Vector2.One, SpriteEffects.None, 0f);
        }

        private void DrawSun()
        {
            _spriteBatch.Draw(sunTexture, sunPosition, null, Color.White, 0f, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }

        private void UpdateStarPositions(GameTime gameTime)
        {
            // Update the lifespan timer for each star and remove expired stars
            for (int i = starPositions.Count - 1; i >= 0; i--)
            {
                starPositions[i] = (starPositions[i].Position, starPositions[i].Timer + (float)gameTime.ElapsedGameTime.TotalSeconds);

                if (starPositions[i].Timer >= StarLifespan)
                {
                    starPositions.RemoveAt(i);
                }
            }

            if (IsStarKeyJustPressed() && starPositions.Count < MaxStars)
            {
                SpawnNewStar();
            }
        }

        private bool IsStarKeyJustPressed()
        {
            Keys[] starKeys = { Keys.S, Keys.T, Keys.A, Keys.R };
            foreach (var key in _currentKeyState.GetPressedKeys())
            {
                if (Array.Exists(starKeys, k => k == key) && !_previousKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }

        private void SpawnNewStar()
        {
            var randomX = _random.Next(0, _graphics.PreferredBackBufferWidth - starTexture.Width);
            var randomY = _random.Next(0, _graphics.PreferredBackBufferHeight - starTexture.Height);
            starPositions.Add((new Vector2(randomX, randomY), 0f));
            starAppearPopSoundInstance.Play();
        }
    }
}
