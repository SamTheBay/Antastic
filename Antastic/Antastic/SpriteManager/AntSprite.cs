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

    public enum AntType
    {
        black,
        red
    }



    class AntSprite : AnimatedSprite
    {
        AntType antType = AntType.black;
        protected List<Vector2> path;
        int pathIndex = 0;
        protected Vector2 currentDestination;
        protected bool isPathFollowing = true;
        float movementSpeed = 2.0f;
        protected Direction lastDirection = Direction.Right;
        static List<Vector2>[] paths = new List<Vector2>[7];
        Vector2 hitDirection;
        float hitMagnitude;
        float hitMagnitudeAdjust = .02f;
        float maxHitMagnitude = 8f;
        float gravityVel = 0f;
        float gravityAcc = .3f;
        float rotationSpeed = .2f;
        

        public AntSprite(AntType antType)
            : base(GetTextureFromAntType(antType), new Point(14,22), new Point(7,11), 4, new Vector2(7,11), Vector2.Zero)
        {
            this.antType = antType;
            AddAnimation(new Animation("Walk", 1, 4, 100, true, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("FlickRight", 1, 1, 100, true, SpriteEffects.None, rotationSpeed));
            AddAnimation(new Animation("FlickLeft", 1, 1, 100, true, SpriteEffects.None, -rotationSpeed));
            if (antType == AntType.black)
            {
                movementSpeed = 1.3f;
            }
            if (antType == AntType.red)
            {
                movementSpeed = 2.5f;
            }
            PlayAnimation("Walk");
        }



        static public string GetTextureFromAntType(AntType antType)
        {
            if (AntType.black == antType)
            {
                return "Ant";
            }
            return "RedAnt";
        }

        static public void InitializePaths()
        {
            // pink path
            int pathIndex = 0;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(68, 812));
            paths[pathIndex].Add(new Vector2(153, 659));
            paths[pathIndex].Add(new Vector2(135, 382));
            paths[pathIndex].Add(new Vector2(165, 331));
            paths[pathIndex].Add(new Vector2(300, 355));
            paths[pathIndex].Add(new Vector2(507, 312));

            // red path
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(-25, 678));
            paths[pathIndex].Add(new Vector2(240, 549));
            paths[pathIndex].Add(new Vector2(162, 332));
            paths[pathIndex].Add(new Vector2(164, 268));
            paths[pathIndex].Add(new Vector2(-21, 39));

            // cyan path
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(188, 826));
            paths[pathIndex].Add(new Vector2(215, 757));
            paths[pathIndex].Add(new Vector2(271, 724));
            paths[pathIndex].Add(new Vector2(299, 604));
            paths[pathIndex].Add(new Vector2(217, 498));
            paths[pathIndex].Add(new Vector2(230, 302));
            paths[pathIndex].Add(new Vector2(249, 211));
            paths[pathIndex].Add(new Vector2(185, 164));
            paths[pathIndex].Add(new Vector2(44, -27));


            // yellow path <---
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(331, 823));
            paths[pathIndex].Add(new Vector2(287, 704));
            paths[pathIndex].Add(new Vector2(393, 446));
            paths[pathIndex].Add(new Vector2(302, 153));
            paths[pathIndex].Add(new Vector2(335, 65));
            paths[pathIndex].Add(new Vector2(326, -21));

            // green
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(502, 660));
            paths[pathIndex].Add(new Vector2(288, 568));
            paths[pathIndex].Add(new Vector2(296, 353));
            paths[pathIndex].Add(new Vector2(360, 226));
            paths[pathIndex].Add(new Vector2(498, 133));


            // purple
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(230, -24));
            paths[pathIndex].Add(new Vector2(168, 85));
            paths[pathIndex].Add(new Vector2(185, 164));
            paths[pathIndex].Add(new Vector2(141, 241));
            paths[pathIndex].Add(new Vector2(300, 365));
            paths[pathIndex].Add(new Vector2(303, 267));
            paths[pathIndex].Add(new Vector2(504, 141));

            // orange
            pathIndex++;
            paths[pathIndex] = new List<Vector2>();
            paths[pathIndex].Add(new Vector2(498, -25));
            paths[pathIndex].Add(new Vector2(328, 82));
            paths[pathIndex].Add(new Vector2(288, 182));
            paths[pathIndex].Add(new Vector2(302, 267));
            paths[pathIndex].Add(new Vector2(118, 355));
            paths[pathIndex].Add(new Vector2(-27, 273));
        }



        public void SetPath()
        {
            path = paths[Util.Random.Next(0, paths.Length)];
            position = path[0];
            currentDestination = path[1] - sourceOffset;
            pathIndex = 2;
            isPathFollowing = true;
            gravityVel = 0;
            PlayAnimation("Walk");
            Activate();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
                    float angle = (float)Math.Atan2(movement.Y, movement.X);

                    currentRotation = angle + (float)(Math.PI / 2);

                    // update position
                    position += movement;

                    Direction direction = GetDirectionFromVector(movement);
                    if (lastDirection != direction)
                    {
                        //PlayAnimation("Walk", direction);
                        lastDirection = direction;
                    }

                }
                else
                {
                    // we have arrived at the current destination
                    position = currentDestination;

                    if (pathIndex == path.Count)
                    {
                        isPathFollowing = false;
                        Deactivate();
                    }
                    else
                    {
                        currentDestination = path[pathIndex] - sourceOffset;
                        pathIndex++;
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
                        if (antType == AntType.black)
                        {
                            GameplayScreen.singleton.RewardPoints(1);
                        }
                        else
                        {
                            GameplayScreen.singleton.RewardPoints(3);
                        }
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



        protected Direction GetDirectionFromVector(Vector2 movement)
        {
            float angle = (float)Math.Atan2(movement.X, -movement.Y);

            if (angle > 1 * Math.PI / 8 && angle < 3 * Math.PI / 8)
            {
                return Direction.RightUp;
            }
            if (angle > 3 * Math.PI / 8 && angle < 5 * Math.PI / 8)
            {
                return Direction.Right;
            }
            if (angle > 5 * Math.PI / 8 && angle < 7 * Math.PI / 8)
            {
                return Direction.RightDown;
            }

            else if (angle < -1 * Math.PI / 8 && angle > -3 * Math.PI / 8)
            {
                return Direction.LeftUp;
            }
            else if (angle < -3 * Math.PI / 8 && angle > -5 * Math.PI / 8)
            {
                return Direction.Left;
            }
            else if (angle < -5 * Math.PI / 8 && angle > -7 * Math.PI / 8)
            {
                return Direction.LeftDown;
            }

            else if (angle > -Math.PI / 8 && angle < Math.PI / 8)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Down;
            }
        }



        public bool isDead
        {
            get { return !isPathFollowing; }
        }

    }
}
