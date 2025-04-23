using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spritesheetTexture;

    private Vector2 _playerPosition = new Vector2(100, 100);
    private Vector2 _playerVelocity = Vector2.Zero;
    private Vector2 _gravity = new Vector2(0, 0.5f);

    private const int SpriteSize = 8;
    private const float SpriteScale = 4f;
    private float ScaledSize => SpriteSize * SpriteScale;

    // List to store platform tile positions
    private List<Vector2> _platformTiles = new List<Vector2>();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 480;
        _graphics.ApplyChanges();

        // Create a platform under the player
        CreatePlatform();

        base.Initialize();
    }

    private void CreatePlatform()
    {
        // Clear any existing tiles
        _platformTiles.Clear();

        // Platform will be 10 tiles wide centered below the player
        int platformWidth = 20;
        float startX = (_graphics.PreferredBackBufferWidth / 2) - (platformWidth * ScaledSize / 2);
        float platformY = _graphics.PreferredBackBufferHeight - ScaledSize * 2; // 2 tiles above bottom

        for (int i = 0; i < platformWidth; i++)
        {
            _platformTiles.Add(new Vector2(startX + i * ScaledSize, platformY));
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spritesheetTexture = Content.Load<Texture2D>("spritesheet");
    }

    protected override void Update(GameTime gameTime)
{
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
        Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

    KeyboardState keyboard = Keyboard.GetState();

    // Store previous position for collision response
    Vector2 previousPosition = _playerPosition;

    // Apply movement
    if (keyboard.IsKeyDown(Keys.A))
        _playerVelocity.X = -2f;
    else if (keyboard.IsKeyDown(Keys.D))
        _playerVelocity.X = 2f;
    else
        _playerVelocity.X = 0f;

    // Apply gravity
    _playerVelocity += _gravity;
    _playerPosition += _playerVelocity;

    // Create player rectangle
    Rectangle playerRect = new Rectangle(
        (int)_playerPosition.X,
        (int)_playerPosition.Y,
        (int)ScaledSize,
        (int)ScaledSize
    );

    // Check if player is grounded (either on platform or floor)
    bool isGrounded = false;

    // 1. Check collisions with platform tiles
    foreach (var tile in _platformTiles)
    {
        Rectangle tileRect = new Rectangle(
            (int)tile.X,
            (int)tile.Y,
            (int)ScaledSize,
            (int)ScaledSize
        );

        if (playerRect.Intersects(tileRect))
        {
            // Calculate overlap on each side
            float overlapLeft = playerRect.Right - tileRect.Left;
            float overlapRight = tileRect.Right - playerRect.Left;
            float overlapTop = playerRect.Bottom - tileRect.Top;
            float overlapBottom = tileRect.Bottom - playerRect.Top;

            // Find the smallest overlap (shallowest penetration)
            float minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), 
                                       Math.Min(overlapTop, overlapBottom));

            // Resolve collision based on smallest overlap
            if (minOverlap == overlapLeft)
            {
                _playerPosition.X = tileRect.Left - playerRect.Width;
                _playerVelocity.X = 0;
            }
            else if (minOverlap == overlapRight)
            {
                _playerPosition.X = tileRect.Right;
                _playerVelocity.X = 0;
            }
            else if (minOverlap == overlapTop)
            {
                _playerPosition.Y = tileRect.Top - playerRect.Height;
                _playerVelocity.Y = 0;
                isGrounded = true;
            }
            else if (minOverlap == overlapBottom)
            {
                _playerPosition.Y = tileRect.Bottom;
                _playerVelocity.Y = 0;
            }
        }
    }

    // 2. Check collision with floor (window bottom border)
    if (_playerPosition.Y + ScaledSize >= _graphics.PreferredBackBufferHeight)
    {
        _playerPosition.Y = _graphics.PreferredBackBufferHeight - ScaledSize;
        _playerVelocity.Y = 0;
        isGrounded = true;
    }

    // Jump only when grounded
    if (isGrounded && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W)))
    {
        _playerVelocity.Y = -8f;
    }

    // Screen bounds checking (X axis only)
    if (_playerPosition.X < 0)
    {
        _playerPosition.X = 0;
        _playerVelocity.X = 0;
    }
    else if (_playerPosition.X + ScaledSize > _graphics.PreferredBackBufferWidth)
    {
        _playerPosition.X = _graphics.PreferredBackBufferWidth - ScaledSize;
        _playerVelocity.X = 0;
    }

    base.Update(gameTime);
}

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw platform tiles
        foreach (var tile in _platformTiles)
        {
            _spriteBatch.Draw(
                _spritesheetTexture,
                tile,
                new Rectangle(8, 8, 8, 8),
                Color.White,
                0f,
                Vector2.Zero,
                SpriteScale,
                SpriteEffects.None,
                0f
            );
        }

        // Draw player
        _spriteBatch.Draw(
            _spritesheetTexture,
            _playerPosition,
            new Rectangle(0, 0, SpriteSize, SpriteSize), // Same sprite as player for now
            Color.White,
            0f,
            Vector2.Zero,
            SpriteScale,
            SpriteEffects.None,
            0f
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}