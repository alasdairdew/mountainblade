using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MountainBlade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes.Weapons.Melee_Weapons
{
    class Sword : MeleeWeapon
    {
        public int Damage { get; } = 2;

        public int Charge { get => _charge; }

        private int _charge;

        private static Vector2 _renderOffset = new Vector2(0,0);

        public void ResetCharge()
        {
            _charge = 0;
        }

        public void IncrementCharge(int amount)
        {
            _charge += amount;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            //spriteBatch.Draw(Sprites.Sword, position, null, Color.White, rotation, new Vector2(Troop.DefaultSize / 2, Troop.DefaultSize / 2), 1f, SpriteEffects.None, 0f);

            float newRotation = (float) (Math.Max(-3f, Charge / 20)+ rotation)/2;

            
            spriteBatch.Draw(Sprites.Sword, position, null, Color.White, rotation, new Vector2(Troop.DefaultSize / 2, Troop.DefaultSize / 2) - _renderOffset, 1f, SpriteEffects.None, 0f);
        }
    }
}
