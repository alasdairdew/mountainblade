using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MountainBlade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade.BattleState_classes
{
    class CollisionQTree
    {
        private readonly static int ProjectileCapacity = 4, TroopCapacity = 3, MaxDepth = 7;

        private CollisionQTree[] _nodes;

        private List<Troop> _troops;
        private List<Projectile> _projectiles;

        private int _x, _y, _width, _height;

        private int _depth;

        private bool Subdivided => _nodes != null;


        public CollisionQTree(int x, int y, int width, int height, int depth)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _depth = depth;

            _troops = new List<Troop>();
            _projectiles = new List<Projectile>();
        }

        public void Insert(List<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                Insert(entity);
            }
        }

        public void Insert(Entity entity)
        {
            if (Subdivided)
            {
                MoveDown(entity);
                return;
            }

            if (entity.Size > 3) //TODO: find way to check type correctly
            {
                if (_troops.Count > TroopCapacity && _depth < MaxDepth)
                {
                    MoveDown(entity);
                }

                else
                {
                    _troops.Add((Troop)entity);
                }

                return;
            }


            if (_projectiles.Count > ProjectileCapacity && _depth < MaxDepth)
            {
                MoveDown(entity);
            }

            else
            {
                _projectiles.Add((Projectile)entity);
            }
        }

        private void MoveDown(Entity ent)
        {
            if (!Subdivided) Subdivide();

            if (ent.Size > 0)
            {

                MoveDownTroop(ent);
            }

            else
            {
                MoveDownProjectile(ent);
            }

        }

        private void Subdivide()
        {
            _nodes = new CollisionQTree[4];

            int newWidth = _width / 2;
            int newHeight = _height / 2;
            int newDepth = _depth + 1;

            //	0 | 2
            //  -----  
            //	1 | 3

            _nodes[0] = new CollisionQTree(_x, _y, newWidth, newHeight, newDepth);
            _nodes[1] = new CollisionQTree(_x, _y + newHeight, newWidth, newHeight, newDepth);
            _nodes[2] = new CollisionQTree(_x + newWidth, _y, newWidth, newHeight, newDepth);
            _nodes[3] = new CollisionQTree(_x + newWidth, _y + newHeight, newWidth, newHeight, newDepth);

            FlushToNodes();
        }

        private void MoveDownTroop(Entity troop)
        {
            bool[] entered = new bool[4];

            int sector = InSector(troop.Position.X + troop.Size / 2, troop.Position.Y);
            _nodes[sector].Insert(troop);
            entered[sector] = true;

            sector = InSector(troop.Position.X + troop.Size / 2, troop.Position.Y + troop.Size / 2);

            if (!entered[sector])
            {
                _nodes[sector].Insert(troop);
                entered[sector] = true;
            }

            sector = InSector(troop.Position.X, troop.Position.Y + troop.Size / 2);

            if (!entered[sector])
            {
                _nodes[sector].Insert(troop);
                entered[sector] = true;
            }

            sector = InSector(troop.Position.X, troop.Position.Y);

            if (!entered[sector])
            {
                _nodes[sector].Insert(troop);
                entered[sector] = true;
            }
        }

        private void MoveDownProjectile(Entity proj)
        {
            _nodes[InSector(proj.Position.X, proj.Position.Y)].Insert(proj);
        }

        private int InSector(float x, float y)
        {
            int relativeX = x > _x + _width / 2 ? 1 : 0;
            int relativeY = y > _y + _height / 2 ? 1 : 0;

            return relativeY + (relativeX * 2);
        }

        private void FlushToNodes()
        {
            foreach (Projectile projectile in _projectiles)
            {
                MoveDownProjectile(projectile);
            }

            foreach (Troop troop in _troops)
            {
                MoveDownTroop(troop);
            }

            _troops.Clear();
            _projectiles.Clear();
        }

        public void Clear()
        {
            _nodes = null;
            _troops.Clear();
            _projectiles.Clear();
        }

        public void CheckCollisions()
        {
            if (Subdivided)
                foreach (CollisionQTree node in _nodes)
                {
                    node.CheckCollisions();
                }

            foreach (Troop troop in _troops.ToList<Troop>())
            {
                if (!troop.Alive) continue;

                foreach (Projectile projectile in _projectiles)
                {
                    if (!projectile.Alive || projectile.InAir) continue;

                    if (Utils.Intersects(troop, projectile))
                    {
                        if (projectile.Team == troop.Team) continue;
                        projectile.Hit((AITroop) troop);
                        troop.Hit(projectile);
                    }
                }

                _troops.Remove(troop);

                foreach (Troop troop2 in _troops)
                {
                    if (troop.Position.X == troop2.Position.X) continue;

                    troop.TroopCollision(troop2);

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = new Rectangle(_x, _y, _width, _height);

            spriteBatch.Draw(Sprites.QBox, rect, Color.White);

            if (Subdivided)
            {
                for (int i = 0; i < 4; i++)
                {
                    _nodes[i].Draw(spriteBatch);
                }
            }
        }
    }
}
