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
    class BirdSprite : AnimatedSprite
    {
        bool movingRight = true;
        protected Vector2 currentDestination;
        float movementSpeed = 6.0f;
        protected bool isPathFollowing = true;
        Vector2 hitDirection;
        float hitMagnitude;
        float hitMagnitudeAdjust = .02f;
        float maxHitMagnitude = 8f;
        float gravityVel = 0f;
        float gravityAcc = .3f;
        float rotationSpeed = .2f;

        int playSoundDuration = 900;
        int playSoundElpased = 0;

        public BirdSprite()
            : base("Bird", new Point(82,52), new Point(41,26), 5, new Vector2(41,26), Vector2.Zero)
        {
            AddAnimation(new Animation("FlyRight", 1, 5, 100, true, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("FlyLeft", 1, 5, 100, true, SpriteEffects.FlipHorizontally, Color.White));
            AddAnimation(new Animation("FlickRight", 3, 3, 100, true, SpriteEffects.None, rotationSpeed));
            AddAnimation(new Animation("FlickLeft", 3, 3, 100, true, SpriteEffects.FlipHorizontally, -rotationSpeed));
            PlayAnimation("FlyRight");
        }



        public void StartBird()
        {
            if (Util.Random.Next(0, 2) == 0)
            {
                movingRight = true;
                PlayAnimation("FlyRight");
                position.X = -50;
                position.Y = Util.Random.Next(50, 500);
                currentDestination.X = 500;
                currentDestination.Y = Util.Random.Next(50, 500);
            }
            else
            {
                movingRight = false;
                PlayAnimation("FlyLeft");
                position.X = 500;
                position.Y = Util.Random.Next(50, 500);
                currentDestination.X = -50;
                currentDestination.Y = Util.Random.Next(50, 500);
            }
            isPathFollowing = true;
            gravityVel = 0;
            Activate();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!isDead)
            {
                playSoundElpased += gameTime.ElapsedGameTime.Milliseconds;
                if (playSoundElpased > playSoundDuration)
                {
                    //AudioManager.audioManager.PlaySFX("flapping");
                    playSoundElpased = 0;
                }
            }

            if (isPathFollowing)
            {
                // move the character towards their current destination
                // determine how to move
                Vector2 movement = new Vector2();
                movement.X = currentDestination.X - Position.X;
                movement.Y = currentDestination.Y - Position.Y;

                // normalize vector
                float distance = (float)Math.Sqrt((float)Math.Pow(movement.X, 2) + (float)Math.Pow(movement.Y, 2));
                if (distance > movementSpeed)
                {
                    movement.X /= distance;
                    movement.Y /= distance;

                    movement.X *= movementSpeed;
                    movement.Y *= movementSpeed;

                    // update position
                    position += movement;

                }
                else
                {
                    // we have arrived at the current destination, flip the bird
                    if (!movingRight)
                    {
                        movingRight = true;
                        PlayAnimation("FlyRight");
                        currentDestination.X = 500;
                        currentDestination.Y = Util.Random.Next(50, 500);
                    }
                    else
                    {
                        movingRight = false;
                        PlayAnimation("FlyLeft");
                        currentDestination.X = -50;
                        currentDestination.Y = Util.Random.Next(50, 500);
                    }
                }

            }
            else
            {
                position.X += hitDirection.X * hitMagnitude * hitMagnitudeAdjust;
                position.Y += hitDirection.Y * hitMagnitude * hitMagnitudeAdjust + gravityVel;


                if (position.X > 500 || position.X < -50 || position.Y > 820)
                {
                    Deactivate();
                }

                gravityVel += gravityAcc;

            }


        }


        public override void CollisionAction(GameSprite otherSprite)
        {
            base.CollisionAction(otherSprite);

            if (otherSprite is TouchSprite)
            {
                TouchSprite touchSprite = (TouchSprite)otherSprite;
                if (touchSprite.Magnitude > 5f)
                {
                    if (isPathFollowing)
                    {
                        GameplayScreen.singleton.RewardPoints(10);
                    }

                    gravityVel = 0;
                    isPathFollowing = false;
                    hitDirection = touchSprite.MoveDirection;
                    hitMagnitude = touchSprite.Magnitude;
                    if (hitMagnitude > maxHitMagnitude)
                        hitMagnitude = maxHitMagnitude;
                    if (hitDirection.X > 0)
                        PlayAnimation("FlickRight");
                    else
                        PlayAnimation("FlickLeft");
                }
            }
        }


        public bool isDead
        {
            get { return !isPathFollowing; }
        }

    }
}
