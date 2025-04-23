using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spritesheetTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 🔧 Use PointClamp to prevent blurry scaling
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Rectangle sourceRectangle = new Rectangle(0, 0, 8, 8);
        Vector2 drawPosition = new Vector2(100, 100);

        _spriteBatch.Draw(
            _spritesheetTexture,
            drawPosition,
            sourceRectangle,
            Color.White,
            0f,
            Vector2.Zero,
            8f, // scale 8x
            SpriteEffects.None,
            0f
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
