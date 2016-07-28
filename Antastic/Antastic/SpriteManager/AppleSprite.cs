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
    public enum AppleState
    {
        OnTree,
        Held,
        Falling
    }



    public enum AppleType
    {
        Red,
        Green,
        Braeburn,
        Gold
    }


    class AppleSprite : AnimatedSprite
    {
        AppleState appleState = AppleState.OnTree;
        AppleType appleType = AppleType.Red;
        int blossomDuration = 4000;
        int blossomElapsed = 0;
        bool blossomed = false;
        bool eaten = false;
        bool exploded = false;
        int startPositionIndex = 0;
        static List<Vector2> startPositions = new List<Vector2>();
        static List<bool> startPositionsTaken = new List<bool>();

        Vector2 hitDirection;
        float hitMagnitude;
        float hitMagnitudeAdjust = .02f;
        float maxHitMagnitude = 8f;
        float gravityVel = 0f;
        float gravityAcc = .3f;

        public AppleSprite()
            : base("Apple", new Point(60, 60), new Point(30, 30), 12, Vector2.Zero, Vector2.Zero)
        {
            AddAnimation(new Animation("Blossom", 5, 8, 200, false, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("Apple", 1, 1, 100, false, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("Eaten", 1, 4, 200, false, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("Explode", 8, 12, 100, false, SpriteEffects.None, Color.White));
            PlayAnimation("Blossom");
        }



        public static void InitializeStartPositions()
        {
            startPositions.Add(new Vector2(144, 494));
            startPositions.Add(new Vector2(132, 393));
            startPositions.Add(new Vector2(161, 331));
            startPositions.Add(new Vector2(141, 245));
            startPositions.Add(new Vector2(184, 170));
            startPositions.Add(new Vector2(246, 213));
            startPositions.Add(new Vector2(232, 302));
            startPositions.Add(new Vector2(220, 393));
            startPositions.Add(new Vector2(216, 502));
            startPositions.Add(new Vector2(289, 443));
            startPositions.Add(new Vector2(300, 356));
            startPositions.Add(new Vector2(302, 267));
            startPositions.Add(new Vector2(299, 156));
            startPositions.Add(new Vector2(361, 228));
            startPositions.Add(new Vector2(363, 343));
            startPositions.Add(new Vector2(394, 448));

            for (int i = 0; i < startPositions.Count; i++)
            {
                startPositions[i] = new Vector2(startPositions[i].X - 23, startPositions[i].Y - 23);
                startPositionsTaken.Add(false);
            }
        }



        public Color ColorFromAppleType(AppleType appleType)
        {
            switch (appleType)
            {
                case AppleType.Red:
                {
                    return Color.Red;
                }
                case AppleType.Green:
                {
                    return Color.Green;
                }
                case AppleType.Braeburn:
                {
                    return Color.DeepPink;
                }
                case AppleType.Gold:
                {
                    return Color.Yellow;
                }
            }
            return Color.Red;
        }

        public int PointsFromAppleType(AppleType appleType)
        {
            switch (appleType)
            {
                case AppleType.Red:
                    {
                        return 5;
                    }
                case AppleType.Green:
                    {
                        return 10;
                    }
                case AppleType.Braeburn:
                    {
                        return 25;
                    }
                case AppleType.Gold:
                    {
                        return 50;
                    }
            }
            return 5;
        }



        public Texture2D TextureFromAppleType(AppleType appleType)
        {
            switch (appleType)
            {
                case AppleType.Red:
                    {
                        return InternalContentManager.GetTexture("Apple");
                    }
                case AppleType.Green:
                    {
                        return InternalContentManager.GetTexture("GreenApple");
                    }
                case AppleType.Braeburn:
                    {
                        return InternalContentManager.GetTexture("BraeburnApple");
                    }
                case AppleType.Gold:
                    {
                        return InternalContentManager.GetTexture("YellowApple");
                    }
            }
            return InternalContentManager.GetTexture("Apple");;
        }


        public void AddApple()
        {
            int index = Util.Random.Next(0, startPositionsTaken.Count);
            for (int i = 0; i < startPositionsTaken.Count; i++)
            {
                if (startPositionsTaken[index] == false)
                    break;
                index++;
                index %= startPositionsTaken.Count;
            }
            if (startPositionsTaken[index] == true)
                return;

            int roll = Util.Random.Next(0, 100);
            if (roll < 5)
                appleType = AppleType.Gold;
            else if (roll < 15)
                appleType = AppleType.Braeburn;
            else if (roll < 35)
                appleType = AppleType.Green;
            else
                appleType = AppleType.Red;

            Texture = TextureFromAppleType(appleType);

            startPositionsTaken[index] = true;
            startPositionIndex = index;
            position = startPositions[index];
            blossomElapsed = 0;
            blossomed = false;
            appleState = AppleState.OnTree;
            gravityVel = 0f;
            hitMagnitude = 0f;
            PlayAnimation("Blossom");
            eaten = false;
            exploded = false;
            Activate();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (exploded == true && IsPlaybackComplete)
            {
                Deactivate();
            }
            else if (eaten == true && blossomed && IsPlaybackComplete)
            {
                appleState = AppleState.Falling;
            }

            switch (appleState)
            {
                case AppleState.OnTree:
                {
                    if (!blossomed && !exploded)
                    {
                        blossomElapsed += gameTime.ElapsedGameTime.Milliseconds;
                        if (blossomElapsed > blossomDuration)
                        {
                            blossomed = true;
                            PlayAnimation("Apple");
                            ParticleSystem.AddParticles(position + new Vector2(frameDimensions.X / 2, frameDimensions.Y / 2),
                                ParticleType.Starburst, sizeScale: 1.5f, lifetimeScale: 1f, color: ColorFromAppleType(appleType));
                        }
                    }
                    break;
                }
                case AppleState.Held:
                {

                    break;
                }
                case AppleState.Falling:
                {
                    position.X += hitDirection.X * hitMagnitude * hitMagnitudeAdjust;
                    position.Y += hitDirection.Y * hitMagnitude * hitMagnitudeAdjust + gravityVel;


                    if (position.X > 500 || position.X < -50 || position.Y > 820)
                    {
                        Deactivate();
                    }

                    gravityVel += gravityAcc;
                    break;
                }
            }
        }


        public override void Deactivate()
        {
            startPositionsTaken[startPositionIndex] = false;
            base.Deactivate();
        }


        public void Held()
        {
            appleState = AppleState.Held;
            gravityVel = 0;
            hitMagnitude = 0;
        }



        public void Released(Vector2 direction, float magnitude)
        {
            hitMagnitude = magnitude;
            hitDirection = direction;
            appleState = AppleState.Falling;
        }

        public override void CollisionAction(GameSprite otherSprite)
        {
            base.CollisionAction(otherSprite);

            if (exploded == true)
                return;

            if (otherSprite is TouchSprite)
            {
                TouchSprite touchSprite = (TouchSprite)otherSprite;
                if (touchSprite.Magnitude > 5f)
                {
                    if (blossomed)
                    {
                        gravityVel = 0;
                        appleState = AppleState.Falling;
                        hitDirection = touchSprite.MoveDirection;
                        hitMagnitude = touchSprite.Magnitude;
                        if (hitMagnitude > maxHitMagnitude)
                            hitMagnitude = maxHitMagnitude;
                    }
                    else if (!blossomed && IsPlaybackComplete)
                    {
                        PlayAnimation("Explode");
                        GameplayScreen.singleton.DamageTree();
                        exploded = true;
                    }
                }
            }
            else if (otherSprite is AntSprite)
            {
                AntSprite antSprite = (AntSprite)otherSprite;
                if (appleState == AppleState.OnTree && !antSprite.isDead)
                {
                    if (blossomed)
                    {
                        if (eaten == false)
                        {
                            GameplayScreen.singleton.DamageTree();
                            AudioManager.audioManager.PlaySFX("crunch");
                        }
                        PlayAnimation("Eaten");
                        eaten = true;
                    }
                    else if (!blossomed && IsPlaybackComplete)
                    {
                        AudioManager.audioManager.PlaySFX("crunch");
                        GameplayScreen.singleton.DamageTree();
                        PlayAnimation("Explode");
                        exploded = true;
                    }
                }
            }
            else if (otherSprite is BirdSprite)
            {
                BirdSprite birdSprite = (BirdSprite)otherSprite;
                if (appleState == AppleState.OnTree && !birdSprite.isDead)
                {
                    if (blossomed)
                    {
                        appleState = AppleState.Falling;
                    }
                }
            }
            else if (otherSprite is BucketSprite)
            {
                Deactivate();
                if (!eaten)
                {
                    // reward points
                    AudioManager.audioManager.PlaySFX("doodle");
                    GameplayScreen.singleton.RewardPoints(PointsFromAppleType(appleType));
                    ParticleSystem.AddParticles(position + new Vector2(frameDimensions.X / 2, frameDimensions.Y / 2),
                        ParticleType.Starburst, sizeScale: 2.5f, lifetimeScale: 1f, color: ColorFromAppleType(appleType));
                    GameplayScreen.singleton.AddPoints(position, PointsFromAppleType(appleType));
                }
            }
        }


        public AppleState AppleState
        {
            get { return appleState; }
        }


        public bool Blossomed
        {
            get { return blossomed; }
        }


        public bool Eaten
        {
            get { return eaten; }
        }
    }
}
