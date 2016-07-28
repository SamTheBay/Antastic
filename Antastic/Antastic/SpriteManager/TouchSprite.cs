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
    class TouchSprite : AnimatedSprite
    {
        Vector2 direction;
        float magnitude;

        public TouchSprite()
            : base("fingerDown", new Point(40, 40), new Point(20, 20), 1, Vector2.Zero, Vector2.Zero)
        {
            AddAnimation(new Animation("Idle", 1, 1, 100, true, SpriteEffects.None, Color.White));
            PlayAnimation("Idle");
        }


        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
        }




        public Vector2 MoveDirection
        {
            get { return direction; }
            set { direction = value; }
        }


        public float Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }

    }
}
