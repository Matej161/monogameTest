using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spritesheetTexture;

    private Vector2 _playerPosition = new Vector2(100, 100);
    private Vector2 _playerVelocity = Vector2.Zero;
    private Vector2 _gravity = new Vector2(0, 0.5f); // Adjust this value for stronger or weaker gravity

    private const int SpriteSize = 8; // original sprite size
    private const float SpriteScale = 4f; // how much we scale it
    private float ScaledSize => SpriteSize * SpriteScale; // 32


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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        // Load your spritesheet texture here. Replace "mySpriteSheet" with the actual name of your file in the Content folder.
        _spritesheetTexture = Content.Load<Texture2D>("spritesheet"); // <-- Add this line
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState keyboard = Keyboard.GetState();

// Only move left/right if we're on the ground
        bool isGrounded = _playerPosition.Y + ScaledSize >= _graphics.PreferredBackBufferHeight;

        if (keyboard.IsKeyDown(Keys.A))
            _playerVelocity.X = -2f;
        else if (keyboard.IsKeyDown(Keys.D))
            _playerVelocity.X = 2f;
        else
            _playerVelocity.X = 0f; // Stop when no key is held


        // Gravity
        _playerVelocity += _gravity; // Accelerate downward
        _playerPosition += _playerVelocity; // Move the player

        // Define screen and sprite sizes
        int windowWidth = _graphics.PreferredBackBufferWidth;
        int windowHeight = _graphics.PreferredBackBufferHeight;

// Clamp X position (left/right)
        if (_playerPosition.X < 0)
        {
            _playerPosition.X = 0;
            _playerVelocity.X = 0;
        }
        else if (_playerPosition.X + ScaledSize > windowWidth)
        {
            _playerPosition.X = windowWidth - ScaledSize;
            _playerVelocity.X = 0;
        }

// Clamp Y position (top/bottom)
        if (_playerPosition.Y < 0)
        {
            _playerPosition.Y = 0;
            _playerVelocity.Y = 0;
        }
        else if (_playerPosition.Y + ScaledSize > windowHeight)
        {
            _playerPosition.Y = windowHeight - ScaledSize;
            _playerVelocity.Y = 0;
        }


        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 🔧 Use PointClamp to prevent blurry scaling
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Rectangle sourceRectangle = new Rectangle(0, 0, 8, 8);
        Vector2 drawPosition = _playerPosition;

        _spriteBatch.Draw(
            _spritesheetTexture,
            _playerPosition,
            new Rectangle(0, 0, SpriteSize, SpriteSize),
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