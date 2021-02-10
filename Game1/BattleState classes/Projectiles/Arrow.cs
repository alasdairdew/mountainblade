using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes.Projectiles
{
    class Arrow : Projectile
    {

        private float _distance;

        public Arrow() : base (new Vector2(0,0), new Vector2(0, 0), true)
        {

        }

        public Arrow(Vector2 position, Vector2 movement, float distance, bool team) : base (position, movement, team)
        {
            InAir = true;
            _distance = distance;
            _life = (int) (distance / movement.Length()) + 10;
            Damage = 2;
        }

        public override void Update()
        {
            _distance -=(int) _movement.Length();

            if (_distance < _movement.Length() + 10 && InAir)
            {
                InAir = false;
                _movement /= 1.5f;
            }

            base.Update();
        }

        override
        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = Utils.VectorToRadians(Utils.PerpendicularVector(Movement));
            spriteBatch.Draw(InAir ? Sprites.Arrow : Sprites.ArrowSmall, Position, null, Color.White, rotation, new Vector2(1,0), 1f, SpriteEffects.None, 0f);

            //spriteBatch.Draw(Sprites.Arrow, Position, Color.White);
        }
    }
}
