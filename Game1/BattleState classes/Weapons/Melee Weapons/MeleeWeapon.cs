using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes.Weapons.Melee_Weapons
{
    interface MeleeWeapon
    {
        int Charge { get; }

        int Damage { get; }

        void ResetCharge();

        void IncrementCharge(int amount);

        void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation);
    }
}
