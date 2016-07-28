using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Antastic
{
    class InstructionsScreen : MenuScreen
    {
        Texture2D redApple;
        Texture2D yellowApple;
        Texture2D braeburnApple;
        Texture2D greenApple;
        Texture2D redAnt;
        Texture2D blackAnt;
        Texture2D bird;


        public InstructionsScreen()
        {
            IsPopup = true;

            redApple = InternalContentManager.GetTexture("Apple");
            yellowApple = InternalContentManager.GetTexture("YellowApple");
            greenApple = InternalContentManager.GetTexture("GreenApple");
            braeburnApple = InternalContentManager.GetTexture("BraeburnApple");
            redAnt = InternalContentManager.GetTexture("RedAnt");
            blackAnt = InternalContentManager.GetTexture("Ant");
            bird = InternalContentManager.GetTexture("Bird");
        }


        public override void HandleInput()
        {
            base.HandleInput();

            if (InputManager.IsLocationPressed(Antastic.ScreenSize))
            {
                GameplayScreen screen = new GameplayScreen();
                screen.ResetGame();
                AddNextScreenAndExit(screen);
            }
        }



        Vector2 instruction1 = new Vector2(480 / 2, 800 / 2 - 150);
        Vector2 instruction2 = new Vector2(480 / 2, 800 / 2 - 120);
        Vector2 instruction3 = new Vector2(480 / 2, 800 / 2 - 20);
        Vector2 boost1 = new Vector2(480 / 2, 800 / 2 + 10);
        Vector2 boost2 = new Vector2(480 / 2, 800 / 2 + 110);
        Vector2 boost3 = new Vector2(480 / 2, 800 / 2 + 140);
        Vector2 boost4 = new Vector2(480 / 2, 800 / 2 + 170);
        Vector2 boost5 = new Vector2(480 / 2, 800 / 2 + 270);
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "Flick ants and birds", instruction1, Color.White);
            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "away with your finger", instruction2, Color.White);

            spriteBatch.Draw(blackAnt, new Vector2(160 - 7, 800 / 2 - 90), new Rectangle(0, 0, 14, 22), Color.White);
            spriteBatch.Draw(redAnt, new Vector2(320 - 7, 800 / 2 - 90), new Rectangle(0, 0, 14, 22), Color.White);
            spriteBatch.Draw(bird, new Vector2(240 - 41, 800 / 2 - 100), new Rectangle(0, 0, 82, 52), Color.White);


            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "Pick apples and drop", instruction3, Color.White);
            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "them in the bucket", boost1, Color.White);

            spriteBatch.Draw(redApple, new Vector2(96 - 16, 800 / 2 + 40), new Rectangle(14, 14, 32, 32), Color.White);
            spriteBatch.Draw(greenApple, new Vector2(192 - 16, 800 / 2 + 40), new Rectangle(14, 14, 32, 32), Color.White);
            spriteBatch.Draw(braeburnApple, new Vector2(288 - 16, 800 / 2 + 40), new Rectangle(14, 14, 32, 32), Color.White);
            spriteBatch.Draw(yellowApple, new Vector2(384 - 16, 800 / 2 + 40), new Rectangle(14, 14, 32, 32), Color.White);


            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "If ants eat apples or", boost2, Color.White);
            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "blossoms it will kill", boost3, Color.White);
            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "your tree", boost4, Color.White);

            Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "Tap Screen to Start", boost5, Color.Red);

            spriteBatch.End();
        }

    }
}
