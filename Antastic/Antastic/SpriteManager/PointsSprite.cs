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
    class PointsSprite : AnimatedSprite
    {
        float movementSpeed = 4.0f;
        int expireDuration = 800;
        int expireElapsed = 0;

        public PointsSprite()
            : base("Points", new Point(58, 48), new Point(29, 24), 4, Vector2.Zero, Vector2.Zero)
        {
            AddAnimation(new Animation("5", 1, 1, 100, true, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("10", 2, 2, 100, true, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("25", 3, 3, 100, true, SpriteEffects.None, Color.White));
            AddAnimation(new Animation("50", 4, 4, 100, true, SpriteEffects.None, Color.White));
        }


        public void AddIn(Vector2 position, int points)
        {
            this.position = position;
            if (points == 5)
                PlayAnimation("5");
            else if (points == 10)
                PlayAnimation("10");
            else if (points == 25)
                PlayAnimation("25");
            else
                PlayAnimation("50");
            expireElapsed = 0;
            Activate();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            position.Y -= movementSpeed;

            expireElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (expireElapsed > expireDuration)
            {
                Deactivate();
            }
        }
    }
}
