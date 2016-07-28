
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Antastic
{




    public class GameplayScreen : GameScreen
    {
        public static GameplayScreen singleton;
        Rectangle screenRect = new Rectangle(0, 0, 480, 800);
        public static int highScore;
        public static string highScoreString;
        Texture2D bg;
        Texture2D heart;

        List<GameSprite> sprites = new List<GameSprite>(0);
        float actionSpeedFactor = 1.0f;
        float actionSpeedAdjust = .80f;

        AntSprite[] ants = new AntSprite[100];
        int addAntDuration = 1200;
        int addAntElapsed = 0;

        AntSprite[] redAnts = new AntSprite[20];
        int addRedAntDuration = 5000;
        int addRedAntElapsed = 0;

        AppleSprite[] apples = new AppleSprite[20];
        int addAppleDuration = 2500;
        int addAppleElapsed = 0;

        BucketSprite bucketSprite;
        TouchSprite touchSprite;
        BirdSprite birdSprite;
        int addBirdDuration = 15000;
        int addBirdDurationMin = 15000;
        int addBirdDurationMax = 30000;
        int addBirdElapsed = 0;

        PointsSprite[] pointsSprites = new PointsSprite[5];

        AppleSprite heldApple = null;

        public int level = 1;
        public static int currentPoints = 0;
        int nextLevelPoints = 100;
        int nextLevelStartPoints = 100;
        float levelPointsIncreaseFactor = 1.5f;
        int prevLevelPoints = 0;
        int levelUpDuration = 3000;
        int levelUpElapsed = 0;

        int treeHealth;
        int maxTreeHealth = 10;
        bool gameOver = false;
        int gameOverDuration = 3000;
        int gameOverElapsed = 0;

        public GameplayScreen()
            : base()
        {
            singleton = this;

            bg = GameSprite.game.Content.Load<Texture2D>("Textures/background_2");
            heart = GameSprite.game.Content.Load<Texture2D>("Textures/heart");
            EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.None;

            ParticleSystem.Initialize(100);

            LoadHighScore();

            bucketSprite = new BucketSprite();
            sprites.Add(bucketSprite);

            for (int i = 0; i < ants.Length; i++)
            {
                ants[i] = new AntSprite(AntType.black);
                sprites.Add(ants[i]);
            }

            for (int i = 0; i < redAnts.Length; i++)
            {
                redAnts[i] = new AntSprite(AntType.red);
                sprites.Add(redAnts[i]);
            }

            for (int i = 0; i < apples.Length; i++)
            {
                apples[i] = new AppleSprite();
                sprites.Add(apples[i]);
            }

            birdSprite = new BirdSprite();
            sprites.Add(birdSprite);

            for (int i = 0; i < pointsSprites.Length; i++)
            {
                pointsSprites[i] = new PointsSprite();
                sprites.Add(pointsSprites[i]);
            }

            touchSprite = new TouchSprite();
            sprites.Add(touchSprite);
            ResetGame();
            gameOver = true;
        }


        public void LoadHighScore()
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("HighScore",
                FileMode.OpenOrCreate,
                FileAccess.Read,
                isoFile);

            BinaryReader reader = new BinaryReader(isoStream);

            // read out high score
            try
            {
                highScore = reader.ReadInt32();
            }
            catch (Exception)
            {
                highScore = 0;
            }
            highScoreString = "Best: " + highScore.ToString();

            reader.Close();
            isoStream.Close();
        }


        public void SaveHighScore()
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("HighScore",
                FileMode.Create,
                FileAccess.Write,
                isoFile);

            BinaryWriter writer = new BinaryWriter(isoStream);

            // write high score
            writer.Write((Int32)highScore);
            highScoreString = "Best: " + highScore.ToString();

            writer.Close();
            isoStream.Close();
        }


        public void ResetGame()
        {
            level = 1;
            currentPoints = 0;
            nextLevelPoints = nextLevelStartPoints;
            prevLevelPoints = 0;
            actionSpeedFactor = 1.0f;
            touchSprite.position = new Vector2(-1000, -1000);
            treeHealth = maxTreeHealth;
            gameOver = false;
            gameOverElapsed = 0;
            addBirdElapsed = 0;
            
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] != null && sprites[i].IsActive)
                {
                    sprites[i].Deactivate();
                }
            }

            bucketSprite.Activate();
        }





        public override void LoadContent()
        {
            ScreenManager.Game.ResetElapsedTime();
        }



        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive && !coveredByOtherScreen)
            {
                ParticleSystem.Update(gameTime);

                if (levelUpElapsed < levelUpDuration)
                {
                    levelUpElapsed += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (gameOver)
                {
                    gameOverElapsed += gameTime.ElapsedGameTime.Milliseconds;
                    if (gameOverElapsed > gameOverDuration)
                    {
                        AddNextScreenAndExit(new LeaderboardScreen());
                    }
                }

                addAntElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if ((int)(addAntDuration * actionSpeedFactor) < addAntElapsed)
                {
                    AddAnt(AntType.black);
                    addAntElapsed = 0;
                }

                if (level >= 3)
                {
                    addRedAntElapsed += gameTime.ElapsedGameTime.Milliseconds;
                    if ((int)(addRedAntDuration * actionSpeedFactor) < addRedAntElapsed)
                    {
                        AddAnt(AntType.red);
                        addRedAntElapsed = 0;
                    }
                }

                if (level >= 5 && birdSprite.IsActive == false)
                {
                    addBirdElapsed += gameTime.ElapsedGameTime.Milliseconds;
                    if ((int)(addBirdDuration * actionSpeedFactor) < addBirdElapsed)
                    {
                        if (birdSprite.IsActive == false)
                        {
                            birdSprite.StartBird();
                        }
                        addBirdElapsed = 0;
                        addBirdDuration = Util.Random.Next(addBirdDurationMin, addBirdDurationMax);
                    }
                }

                addAppleElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if ((int)(addAppleDuration * (1 - ((1 - actionSpeedFactor) / 2))) < addAppleElapsed)
                {
                    AddApple();
                    addAppleElapsed = 0;
                }

                for (int i = 0; i < sprites.Count; i++)
                {
                    if (sprites[i] != null && sprites[i].IsActive)
                    {
                        sprites[i].Update(gameTime);
                    }
                }


                // check for collision
                for (int i = 0; i < ants.Length; i++)
                {
                    if (ants[i] != null && ants[i].IsActive)
                    {
                        for (int j = 0; j < apples.Length; j++)
                        {
                            if (apples[j] != null && apples[j].IsActive)
                            {
                                bool collision = apples[j].CollisionDetect(ants[i]);
                                if (collision)
                                    apples[j].CollisionAction(ants[i]);
                            }
                        }
                    }
                }

                for (int i = 0; i < redAnts.Length; i++)
                {
                    if (redAnts[i] != null && redAnts[i].IsActive)
                    {
                        for (int j = 0; j < apples.Length; j++)
                        {
                            if (apples[j] != null && apples[j].IsActive)
                            {
                                bool collision = apples[j].CollisionDetect(redAnts[i]);
                                if (collision)
                                    apples[j].CollisionAction(redAnts[i]);
                            }
                        }
                    }
                }

                for (int j = 0; j < apples.Length; j++)
                {
                    if (apples[j] != null && apples[j].IsActive)
                    {
                        bool collision = apples[j].CollisionDetect(bucketSprite);
                        if (collision)
                            apples[j].CollisionAction(bucketSprite);
                    }
                }

                for (int j = 0; j < apples.Length; j++)
                {
                    if (apples[j] != null && apples[j].IsActive)
                    {
                        bool collision = apples[j].CollisionDetect(birdSprite);
                        if (collision)
                            apples[j].CollisionAction(birdSprite);
                    }
                }

            }
        }



        public void RewardPoints(int points)
        {
            if (!gameOver)
            {
                currentPoints += points;
                if (currentPoints > nextLevelPoints)
                {
                    LevelUp();
                }
            }
        }



        public void AddAnt(AntType antType)
        {
            if (antType == AntType.black)
            {
                for (int i = 0; i < ants.Length; i++)
                {
                    if (ants[i].IsActive == false)
                    {
                        ants[i].SetPath();
                        return;
                    }
                }
            }
            if (antType == AntType.red)
            {
                for (int i = 0; i < redAnts.Length; i++)
                {
                    if (redAnts[i].IsActive == false)
                    {
                        redAnts[i].SetPath();
                        return;
                    }
                }
            }
        }

        public void AddApple()
        {
            for (int i = 0; i < apples.Length; i++)
            {
                if (apples[i].IsActive == false)
                {
                    apples[i].AddApple();
                    return;
                }
            }
        }

        public void AddPoints(Vector2 position, int points)
        {
            for (int i = 0; i < pointsSprites.Length; i++)
            {
                if (pointsSprites[i].IsActive == false)
                {
                    pointsSprites[i].AddIn(position, points);
                    return;
                }
            }
        }

        Vector2 position = new Vector2();
        Vector2 dragPosition = new Vector2();
        bool isStart;
        bool isFinished;
        Vector2 direction = new Vector2();
        float magnitude;
        Rectangle appleBounds = new Rectangle();
        public override void HandleInput()
        {
            if (InputManager.IsBackTriggered())
            {
                AddNextScreen(new PauseScreen());
            }

            bool isDragging = InputManager.GetDrag(ref dragPosition, ref isStart, ref isFinished, ref direction, ref magnitude);
            if (isDragging)
            {
                touchSprite.MoveDirection = direction;
                touchSprite.Magnitude = magnitude;
                position.X = dragPosition.X - touchSprite.FrameDimensions.X / 2;
                position.Y = dragPosition.Y - touchSprite.FrameDimensions.Y / 2;
                if (isStart)
                {
                    // check if we are picking up an apple
                    for (int i = 0; i < apples.Length; i++)
                    {
                        apples[i].GetBoundingRectangle(ref appleBounds);
                        if (appleBounds.Contains(dragPosition.ToPoint()))
                        {
                            if (apples[i].IsActive && 
                                (apples[i].AppleState == AppleState.OnTree || apples[i].AppleState == AppleState.Falling) && 
                                apples[i].Blossomed &&
                                !apples[i].Eaten)
                            {
                                heldApple = apples[i];
                                heldApple.Held();
                                position.X = dragPosition.X - heldApple.FrameDimensions.X / 2;
                                position.Y = dragPosition.Y - heldApple.FrameDimensions.Y / 2;
                                heldApple.position = position;
                                break;
                            }
                        }
                    }

                    if (heldApple == null)
                    {
                        touchSprite.Activate();
                        touchSprite.position = position;
                        CollisionDetectTouch();
                    }
                }
                else
                {
                    if (heldApple == null)
                    {
                        while (position != touchSprite.position)
                        {
                            if (Math.Abs(position.X - touchSprite.position.X) > 10)
                            {
                                if (position.X > touchSprite.position.X)
                                {
                                    touchSprite.position.X += 10;
                                }
                                else
                                {
                                    touchSprite.position.X -= 10;
                                }
                            }
                            else
                            {
                                touchSprite.position.X = position.X;
                            }
                            if (Math.Abs(position.Y - touchSprite.position.Y) > 10)
                            {
                                if (position.Y > touchSprite.position.Y)
                                {
                                    touchSprite.position.Y += 10;
                                }
                                else
                                {
                                    touchSprite.position.Y -= 10;
                                }
                            }
                            else
                            {
                                touchSprite.position.Y = position.Y;
                            }

                            CollisionDetectTouch();
                        }
                        CollisionDetectTouch();
                    }
                    else
                    {
                        position.X = dragPosition.X - heldApple.FrameDimensions.X / 2;
                        position.Y = dragPosition.Y - heldApple.FrameDimensions.Y / 2;
                        heldApple.position = position;
                    }
                }


            }
            else if (isFinished)
            {
                if (heldApple == null)
                {
                    touchSprite.position.X = -1000;
                    touchSprite.position.Y = -1000;
                    touchSprite.Deactivate();
                }
                else
                {
                    heldApple.Released(direction, magnitude);
                    heldApple = null;
                }

            }
        }


        public void CollisionDetectTouch()
        {
            // check for collision
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] != null && sprites[i].IsActive)
                {
                    bool collision = touchSprite.CollisionDetect(sprites[i]);
                    if (collision)
                        sprites[i].CollisionAction(touchSprite);
                }
            }
        }


        public void LevelUp()
        {
            actionSpeedFactor *= actionSpeedAdjust;
            levelUpElapsed = 0;
            int lastnext = nextLevelPoints;
            int increase = (int)((nextLevelPoints - prevLevelPoints) * levelPointsIncreaseFactor);
            if (increase > 500)
                increase = 500;
            nextLevelPoints = nextLevelPoints + increase;
            prevLevelPoints = lastnext;
            level++;
            treeHealth += 3;
            if (treeHealth > maxTreeHealth)
                treeHealth = maxTreeHealth;
        }


        Vector2 scoreLocation = new Vector2(10, 10);
        Vector2 centerText = new Vector2(480 / 2, 800 / 2);
        Vector2 centerText2 = new Vector2(480 / 2, 800 / 2 + 30);
        Rectangle waterRect = new Rectangle(0,50, 480, 800);
        Vector2 heartLoc = new Vector2();
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);


            spriteBatch.DrawString(Fonts.DescriptionFont, currentPoints.ToString(), scoreLocation, Color.White);

            Vector2 stringSize = Fonts.DescriptionFont.MeasureString(highScoreString);
            stringSize.X = 470 - stringSize.X;
            stringSize.Y = 10;
            spriteBatch.DrawString(Fonts.DescriptionFont, highScoreString, stringSize, Color.White);

            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] != null && sprites[i].IsActive)
                {
                    sprites[i].Draw(spriteBatch, 0f);
                }
            }


            int height = (int)(800f * (currentPoints - prevLevelPoints) / (nextLevelPoints - prevLevelPoints));
            spriteBatch.Draw(InternalContentManager.GetTexture("Blank"),
                new Rectangle(0, 800 - height, 5, height),
                Color.Purple);


            heartLoc.X = 480 - heart.Width - 5;
            heartLoc.Y = 50;
            for (int i = 0; i < treeHealth; i++)
            {
                spriteBatch.Draw(heart, heartLoc, Color.White);
                heartLoc.Y += heart.Height + 3;
            }


            if (gameOver)
            {
                Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "Game Over", centerText, Color.Red);
            }
            else if (levelUpElapsed < levelUpDuration)
            {
                Fonts.DrawCenteredText(spriteBatch, Fonts.HeaderFont, "Level " + level.ToString(), centerText, Color.White);
            }


            spriteBatch.End();

            ParticleSystem.Draw(spriteBatch, gameTime);
        }


        public void DamageTree()
        {
            treeHealth--;
            if (treeHealth == 0)
            {
                gameOver = true;
                UpdateHighScore();
            }
        }


        public void UpdateHighScore()
        {
            if (currentPoints > highScore)
            {
                highScore = currentPoints;
                SaveHighScore();
            }
        }



        public bool IsPlaying
        {
            get { return !gameOver; }
            set { gameOver = !value; }
        }

    }
}
