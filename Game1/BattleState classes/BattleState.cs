using MountainBlade;
using MountainBlade.BattleState_classes;
using MountainBlade.BattleState_classes.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.BattleState_classes.Projectiles;

namespace MountainBlade
{
    class BattleState : State
    {
        public int Size { get; set; }


        private TroopManager _troops1, _troops2;

        private CollisionQTree _collisionTree;

        private List<Entity> _projectiles, _deadProjectiles;

        private List<Entity> _renderEntities;

        private List<Entity> _deadBodies;

        public Random rng;

        public int width = 1920, height = 1920;

        private Vector2 _wind, _windChangeDir;

        public String selected;

        public BattleState()
        {
            Initialise();
        }

        private void Initialise()
        {
            Vector2 spawn1 = new Vector2(300, 200);

            Vector2 spawn2 = new Vector2(1700, 800);

            _troops1 = new TroopManager(spawn1, true, this);

            _troops2 = new TroopManager(spawn2, false, this);

            _collisionTree = new CollisionQTree(0, 0, width, height, 0);

            _projectiles = new List<Entity>();

            _deadProjectiles = new List<Entity>();
            _deadBodies = new List<Entity>();

            rng = new Random();

            _renderEntities = new List<Entity>();

            _wind = new Vector2(0.2f, 0.2f);
            _windChangeDir = new Vector2(0, 0);
        }

        override
        public void Update()
        {
            _collisionTree.Clear();

            MouseState state = Mouse.GetState();

            if (state.RightButton.Equals(ButtonState.Pressed))
            {
                DebugRay ray = new DebugRay(new Vector2(state.X, state.Y), Vector2.Zero, true);

                _projectiles.Add(ray);
            }


            UpdateWind();

            _troops1.EnemyAveragePosition = _troops2.GetAveragePosition();
            _troops2.EnemyAveragePosition = _troops1.GetAveragePosition();

            _troops1.Update();
            _troops2.Update();

            UpdateProjectiles();

            _collisionTree.Insert(_troops1.GetAllTroops());
            _collisionTree.Insert(_troops2.GetAllTroops());

            _collisionTree.CheckCollisions();



            KeyboardState kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.R))
            {
                selected = "archers";
            }

            if (kState.IsKeyDown(Keys.T))
            {
                selected = "infantry";
            }

            if (kState.IsKeyDown(Keys.A))
            {
                selected = "all";
            }

            if (state.LeftButton.Equals(ButtonState.Pressed))
            {
                Vector2 target = new Vector2(state.X, state.Y);

                switch (selected)
                {
                    case ("archers"):
                        _troops1.TroopTargets[(int)TroopTypes.ranged] = target;
                        break;

                    case ("infantry"):
                        _troops1.TroopTargets[(int)TroopTypes.melee] = target;
                        break;

                    default:
                        _troops1.TroopTargets[(int)TroopTypes.melee] = target;
                        _troops1.TroopTargets[(int)TroopTypes.ranged] = target;
                        break;
                }


            }

            if (state.RightButton.Equals(ButtonState.Pressed))
            {
                Vector2 target = new Vector2(state.X, state.Y);

                switch (selected)
                {
                    case ("archers"):
                        _troops2.TroopTargets[(int)TroopTypes.ranged] = target;
                        break;

                    case ("infantry"):
                        _troops2.TroopTargets[(int)TroopTypes.melee] = target;
                        break;

                    default:
                        _troops2.TroopTargets[(int)TroopTypes.melee] = target;
                        _troops2.TroopTargets[(int)TroopTypes.ranged] = target;
                        break;
                }


            }


            if (state.MiddleButton.Equals(ButtonState.Pressed))
            {
                _troops2.TroopCommands[0] = "charge";
                _troops1.TroopCommands[0] = "charge";
                _troops2.TroopCommands[1] = "charge";
                _troops1.TroopCommands[1] = "charge";
            }
        }

        public void UpdateWind()
        {
            if (Utils.random.Next(0, 1000) <= 10)
            {
                _windChangeDir = new Vector2((float)(Utils.random.NextDouble() - 0.5), (float)(Utils.random.NextDouble() - 0.5));
                _windChangeDir.Normalize();
            }

            _wind += _windChangeDir * (float)(Utils.random.NextDouble() / 100);
            _wind.Normalize();
            _wind *= 0.1f;

        }

        public Troop GetRandomTroop(bool team)
        {
            if (team) return _troops1.GetRandomTroop(team);

            else return _troops2.GetRandomTroop(team);
        }

        private void UpdateProjectiles()
        {
            foreach (Projectile projectile in _projectiles.ToList<Entity>())
            {
                if (!projectile.Alive || IsOffScreen(projectile))
                {
                    if (!projectile.IsHit)
                        _deadProjectiles.Add(projectile);
                    _projectiles.Remove(projectile);
                    continue;
                }

                //projectile.AddVelocity(_wind);
                projectile.Update();

                if (!projectile.InAir) _collisionTree.Insert(projectile);


            }
        }

        private bool IsOffScreen(Entity entity)
        {
            if (entity.Position.X > width || entity.Position.X < 0)
                if (entity.Position.Y > height || entity.Position.Y < 0) return true;

            return false;
        }

        public void FireProjectile(Projectile projectile)
        {
            _projectiles.Add(projectile);
        }

        public void AddBody(Entity body)
        {
            _deadBodies.Add(body);
        }

        private void CreateProjectiles()
        {
            for (int i = 0; i < 1; i++)
            {
                //Arrow arrow = new Arrow(new Vector2((float) rng.NextDouble() * 1024, (float) rng.NextDouble() * 1024), new Vector2(rng.Next(-4, 4), rng.Next(-4, 4)));
                // _projectiles.Add(arrow);
            }
        }

        override
        public void Draw(SpriteBatch spriteBatch)
        {
            _renderEntities.Clear();

            _renderEntities.AddRange(_deadBodies);
            _renderEntities.AddRange(_deadProjectiles);
            _renderEntities.AddRange(_troops1.GetAllTroops());
            _renderEntities.AddRange(_troops2.GetAllTroops());
            _renderEntities.AddRange(_projectiles);


            foreach (Entity entity in _renderEntities)
            {
                entity.Draw(spriteBatch);
            }

            _troops1.Draw(spriteBatch);
            _troops2.Draw(spriteBatch);
        }
    }

    enum TroopTypes { melee, ranged, horseMelee }
}
