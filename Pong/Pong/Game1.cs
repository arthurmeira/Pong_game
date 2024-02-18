using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{
    public class Game1 : Game
    {
        Texture2D ball;
        Texture2D player1;
        Texture2D player2;

        Vector2 ballPosition;
        Vector2 p1Position;
        Vector2 p2Position;

        float ballSpeedX;
        float ballSpeedY;
        bool isBallInPlay = true;
        bool isBallVisible = true;
        int p1Score = 0;
        int p2Score = 0;
        int winner = 0;
        float player2Speed = 3.5f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont spriteFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            p1Position = new Vector2(20, GraphicsDevice.Viewport.Height / 2);
            p2Position = new Vector2(GraphicsDevice.Viewport.Width - 20, GraphicsDevice.Viewport.Height / 2);
            ballPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            ballSpeedX = 250;
            ballSpeedY = -250;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ball = Content.Load<Texture2D>("ball");
            player1 = Content.Load<Texture2D>("player1");
            player2 = Content.Load<Texture2D>("player2");
            spriteFont = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == 
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //Movimento UP - DOWN
            movimento(ref p1Position);

            //movimento(ref p2Position);

            //Colisão no Y
            colisaoY(ref p1Position, player1);
            colisaoY(ref p2Position, player2);
            colisaoY(ref ballPosition, ball);

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ballPosition.X += ballSpeedX * elapsedTime;
            ballPosition.Y += ballSpeedY * elapsedTime;

            // Verifique colisões com a raquete do jogador 1
            if (ballPosition.X - ball.Width / 2 < p1Position.X + player1.Width / 2 &&
                ballPosition.Y > p1Position.Y - player1.Height / 2 &&
                ballPosition.Y < p1Position.Y + player1.Height / 2)
            {
                ballSpeedX = -ballSpeedX;
            } else if (ballPosition.X < 0)
            {
                p2Score++;
                ResetBall();
            }

            // Verifique colisões com a raquete do jogador 2
            if (ballPosition.X + ball.Width / 2 > p2Position.X - player2.Width / 2 &&
                ballPosition.Y > p2Position.Y - player2.Height / 2 &&
                ballPosition.Y < p2Position.Y + player2.Height / 2)
            {
                // A bola colidiu com a raquete do jogador 2, inverte a direção horizontal
                ballSpeedX = -ballSpeedX;
            }
            else if (ballPosition.X > _graphics.PreferredBackBufferWidth)
            {
                p1Score++;
                ResetBall();
            }

            // Controle preciso da direção da bola ao atingir a parte superior/inferior da tela
            if (ballPosition.Y - ball.Height / 2 < 0 ||
                ballPosition.Y + ball.Height / 2 > _graphics.PreferredBackBufferHeight)
            {
                ballSpeedY = -ballSpeedY;
            }

            if (isBallVisible) {
            // Lógica da raquete do jogador 2 (CPU)
                if (ballPosition.Y < p2Position.Y)
                {
                    // A bola está acima da raquete do jogador 2, mova a raquete para cima
                    p2Position.Y -= player2Speed;
                }
                else if (ballPosition.Y > p2Position.Y)
                {
                    // A bola está abaixo da raquete do jogador 2, mova a raquete para baixo
                    p2Position.Y += player2Speed;
                }
            }

            //verifica se a bola está no jogo
            if (isBallInPlay)
            {
                ballPosition.X += ballSpeedX * elapsedTime;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            if (isBallVisible)
            {
                _spriteBatch.Draw(
                    ball,
                    ballPosition,
                    null,
                    Color.White,
                    0f,
                    new Vector2(ball.Width / 2, ball.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }


            //Player1
            _spriteBatch.Draw(
                player1,
                p1Position,
                null,
                Color.White,
                0f,
                new Vector2(player1.Width / 2, player1.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );

            //Player2
            _spriteBatch.Draw(
                player2,
                p2Position,
                null,
                Color.White,
                0f,
                new Vector2(player2.Width / 2, player2.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );

            // Verifique quem é o vencedor
            if (p1Score >= 5)
            {
                _spriteBatch.DrawString(spriteFont, "Player 1 wins!", new Vector2(360, 20), Color.Green);
                isBallVisible = false;
            }
            else if (p2Score >= 5)
            {
                _spriteBatch.DrawString(spriteFont, "Player 2 wins!", new Vector2(360, 20), Color.Red);
                isBallVisible = false;
            }
            else
            {
                // Ainda não há vencedor, continue a exibir os pontos
                _spriteBatch.DrawString(spriteFont, "" + p1Score, new Vector2(360, 20), Color.Green);
                _spriteBatch.DrawString(spriteFont, "" + p2Score, new Vector2(410, 20), Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Função de movimento UP - DOWN
        static void movimento(ref Vector2 player)
        {
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
            {
                player.Y -= 3.5f;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                player.Y += 3.5f;
            }
        }

        //Função de colisão no Y
        void colisaoY(ref Vector2 objPosition, Texture2D objTexture)
        {
            if (objPosition.Y > _graphics.PreferredBackBufferHeight - objTexture.Height / 2)
            {
                objPosition.Y = _graphics.PreferredBackBufferHeight - objTexture.Height / 2;
            }
            else if (objPosition.Y < objTexture.Height / 2)
            {
                objPosition.Y = objTexture.Height / 2;
            }
        }

        //Reseta a bola quando ultrapassar a raquete
        private void ResetBall()
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            isBallInPlay = true;
        }
    }
}   