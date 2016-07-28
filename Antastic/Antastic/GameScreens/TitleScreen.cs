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
    class TitleScreen : MenuScreen
    {


        public TitleScreen()
        {
            restartOnVisible = true;
            IsPopup = true;

            MenuEntry entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/Start");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/StartPress");
            entry.SetStartAnimation(new Vector2(-entry.Texture.Width - 10, 225), new Vector2(240 - entry.Texture.Width / 2, 225), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);

            entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/Leaderboard");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/LeaderboardPress");
            entry.SetStartAnimation(new Vector2(490, 335), new Vector2(240 - entry.Texture.Width / 2, 335), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);

            entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/About");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/AboutPress");
            entry.SetStartAnimation(new Vector2(-entry.Texture.Width - 10, 445), new Vector2(240 - entry.Texture.Width / 2, 445), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);

            entry = new MenuEntry("");
            entry.Selected += new EventHandler<EventArgs>(entry_Selected);
            entry.Font = Fonts.HeaderFont;
            entry.Texture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/Games");
            entry.PressTexture = GameSprite.game.Content.Load<Texture2D>("Textures/Buttons/GamesPress");
            entry.SetStartAnimation(new Vector2(490, 555), new Vector2(240 - entry.Texture.Width / 2, 555), 0, 1000, 1000);
            entry.SetAnimationType(AnimationType.Slide);
            MenuEntries.Add(entry);

            ResetScreen();
        }



        void entry_Selected(object sender, EventArgs e)
        {
            if (selectorIndex == 0)
            {
                AddNextScreen(new InstructionsScreen());
            }
            else if (selectorIndex == 1)
            {
                AddNextScreen(new LeaderboardScreen());
            }
            else if (selectorIndex == 2)
            {
                AddNextScreen(new AboutScreen());
            }
            else if (selectorIndex == 3)
            {
                AddNextScreen(new OtherGamesScreen());
            }
        }


        public override void AddNextScreen(GameScreen nextScreen)
        {
            AudioManager.audioManager.PlaySFX("whoosh");
            base.AddNextScreen(nextScreen);
        }


        public override void ResetScreen()
        {
            AudioManager.audioManager.PlaySFX("whoosh");
            base.ResetScreen();
        }

        public override void ExitScreen()
        {
            Antastic.sigletonGame.Exit();
        }

    }
}
