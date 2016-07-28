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
    class BucketSprite : AnimatedSprite
    {

        public BucketSprite()
            : base("Bucket", new Point(94, 72), new Point(47, 36), 1, Vector2.Zero, Vector2.Zero)
        {
            AddAnimation(new Animation("Idle", 1, 1, 100, true, SpriteEffects.None, Color.DarkGray));
            PlayAnimation("Idle");
            position = new Vector2(380, 650);
            Activate();
        }


        public override Rectangle GetBoundingRectangle(ref Rectangle rect)
        {
            base.GetBoundingRectangle(ref rect);
            rect.X += 20;
            rect.Y += 20;
            rect.Width -= 40;
            rect.Height -= 40;
            return rect;
        }

    }
}
