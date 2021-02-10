using Microsoft.Xna.Framework;
using MountainBlade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.BattleState_classes.Projectiles
{
    class DebugRay : Projectile
    {
        public DebugRay(Vector2 position, Vector2 direction, bool team) : base(position, direction, team)
        {
            _life = 10;
        }

        public override void Hit(AITroop troop)
        {
            troop.PrintDebug();
            IsHit = true;
            _life = 0;
        }
    }
}
