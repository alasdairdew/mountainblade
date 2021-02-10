using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes.Weapons.Ranged_Weapons
{
    interface RangedWeapon
    {
        int CoolDown { get; }
        void MinusCoolDown(int amount);

        Projectile GetProjectile(Vector2 position, Vector2 target, float accuracy);

        void ResetCoolDown();

        int Range { get; }

        void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation);
    }
}
