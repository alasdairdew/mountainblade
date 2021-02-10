using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade
{
    class Projectile : Entity
    {
        public Vector2 Position => _position;
        public Vector2 Movement => _movement;
        public float Size { get; } = 0f;

        public bool Team { get; set; }

        protected Vector2 _position;
        protected Vector2 _movement;

        public bool Alive => _life > 0;

        protected int _life = 5000;

        public bool IsHit { get; set; }

        public bool InAir;

        public int Damage { get; set; }

        public Projectile(Vector2 position, Vector2 movement, bool team)
        {
            _position = position;
            _movement = movement;
            Team = team;
            Damage = 0;
        }

        public virtual void Hit(AITroop troop)
        {
            _life = 0;
            IsHit = true;
        }

        public virtual void Update()
        {
            if (!Alive) return;

            _position += _movement;

            _life--;

        }

        public void AddVelocity(Vector2 velocity)
        {
            _movement += velocity;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
