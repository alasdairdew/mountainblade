using MountainBlade.BattleState_classes.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MountainBlade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes.Weapons.Ranged_Weapons
{
    class Bow : RangedWeapon
    {
        public int CoolDown { get => _coolDown; }

        private int _coolDown = 1000;

        private int _maxCoolDown = 2000;

        private float _speed = 10f;

        private bool _team { get; set; }

        public int Range { get => 500; }

        private Arrow _renderArrow;
        private static Vector2 _arrowRenderOffset = new Vector2(7,0);

        public Bow()
        {
            _renderArrow = new Arrow();
        }

        public Bow(bool team)
        {
            _team = team;
        }

        public void MinusCoolDown(int amount)
        {
            _coolDown -= amount;
        }

        public void ResetCoolDown()
        {
            _coolDown = _maxCoolDown;
        }

        public Projectile GetProjectile(Vector2 position, Vector2 target, float accuracy)
        {
            float distance = Vector2.Distance(position, target);

            Vector2 offset = new Vector2((float)((Utils.random.NextDouble() - 0.5) * accuracy * distance), (float)((Utils.random.NextDouble() - 0.5) * accuracy * distance));

            Vector2 velocity = Utils.DirectionTo(position, target + offset) * _speed;

            return new Arrow(position, velocity, distance, _team);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            spriteBatch.Draw(Sprites.Bow, position, null, Color.White, rotation, new Vector2(Troop.DefaultSize / 2, Troop.DefaultSize / 2), 1f, SpriteEffects.None, 0f);


            if (_coolDown < 1000)
            {
                _arrowRenderOffset.Y = Math.Min(-3, -_coolDown / 100);
                spriteBatch.Draw(Sprites.Arrow, position, null, Color.White, rotation, new Vector2(Troop.DefaultSize / 2, Troop.DefaultSize / 2) - _arrowRenderOffset, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
