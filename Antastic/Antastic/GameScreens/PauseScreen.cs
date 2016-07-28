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
    class PauseScreen : MenuScreen
    {
        public static bool paused = false;
        Texture2D grayOut;

        public PauseScreen()
        {
            paused = true;
            grayOut = InternalContentManager.GetTexture("Gray");
            IsPopup = true;
            restartOnVisible = true;

            MenuEntry entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/Resume");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/ResumePress");
            entry.SetStartAnimation(new Vector2(490, 250), new Vector2(240 - entry.Texture.Width / 2, 250), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);


            entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/DropOut");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/DropOutPress");
            entry.SetStartAnimation(new Vector2(- entry.Texture.Width - 10, 400), new Vector2(240 - entry.Texture.Width / 2, 400), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);

            ResetScreen();
        }

        void entry_Selected(object sender, EventArgs e)
        {
            if (selectorIndex == 0)
            {
                ExitScreen();
            }
            else if (selectorIndex == 1)
            {
                GameplayScreen.singleton.UpdateHighScore();
                ScreenManager.RemoveScreen(GameplayScreen.singleton);
                GameplayScreen.singleton.IsPlaying = false;
                if (GameplayScreen.currentPoints > 0)
                {
                    AddNextScreenAndExit(new LeaderboardScreen());
                }
                else
                {
                    ExitScreen();
                }
            }
        }


        public override void ExitScreen()
        {
            AudioManager.audioManager.PlaySFX("whoosh");
            paused = false;
            base.ExitScreen();
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(grayOut, Antastic.ScreenSize, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        public override void ResetScreen()
        {
            AudioManager.audioManager.PlaySFX("whoosh");
            base.ResetScreen();
        }

    }
}
