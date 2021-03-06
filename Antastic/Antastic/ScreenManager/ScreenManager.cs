#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text;
using System.Threading;
#endregion

namespace Antastic
{

    public class ScreenManager : DrawableGameComponent
    {
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();
        SpriteBatch spriteBatch;
        bool isInitialized;
        bool traceEnabled;
        GameScreen lastTopFullScreen = null;
        public static bool frozen = false;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
        }


        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }


        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (frozen)
                return;

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;


            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // check if this screen has acquired the top full state
                if (!screen.IsPopup && !coveredByOtherScreen && screen != lastTopFullScreen)
                {
                    screen.TopFullScreenAcquired();
                    lastTopFullScreen = screen;
                }

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        if (screen.IsStable && screen.IsExiting == false && screen.IsLoadingNext == false)
                            screen.HandleInput();
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }


            // draw the default ad
            Antastic.adControlManager.Draw(gameTime);
        }


        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }
            
            screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }


        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            // reset the new in focus screen if it is set
            int index = screens.IndexOf(screen);
            if (index > 0)
            {
                if (screens[index - 1].RestartOnVisible)
                    screens[index - 1].ResetScreen();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
            if (screens.Count > 0)
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            else
                TouchPanel.EnabledGestures = GestureType.None;
        }


        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

    }
}
