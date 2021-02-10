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
    enum TroopTypes { melee, ranged, horseMelee }

    class TroopManager
    {
        public static Random random = new Random();

        public List<Entity>[] Troops;

        public bool _team { get; set; }

        public BattleState Battlefield;

        public Vector2 Target;

        public Vector2 EnemyAveragePosition { get; set; }

        private PositionLine[] _positionLines;

        public Vector2[] AveragePositions;

        public Vector2[] TroopTargets;

        public String[] TroopCommands;

        private int counter = 0;

        public int NumberOfGroups = 2;

        public String Command = "toPosition";

        public TroopManager(Vector2 spawnPoint, bool team, BattleState battlefield)
        {
            Target = spawnPoint;
            _team = team;
            Battlefield = battlefield;
            Initialise();
        }

        public void Update()
        {
            counter++;

            if (counter % 30 == 1)
                UpdatePositions();


            UpdateTroops();

        }

        private void Initialise()
        {
            _positionLines = new PositionLine[NumberOfGroups];
            Troops = new List<Entity>[NumberOfGroups];
            AveragePositions = new Vector2[NumberOfGroups];
            TroopTargets = new Vector2[NumberOfGroups];
            TroopCommands = new string[NumberOfGroups];

            for (int i = 0; i < NumberOfGroups; i++)
            {
                Troops[i] = new List<Entity>();
                _positionLines[i] = new PositionLine();
                AveragePositions[i] = new Vector2(0, 0);
                TroopTargets[i] = new Vector2(0, 0);
                TroopCommands[i] = "toPosition";
            }

            CreateTroops();
        }

        private void UpdateTroops()
        {
            for (int i = 0; i < NumberOfGroups; i++)
            {
                IEnumerator<Vector2> commands = _positionLines[i].Positions.GetEnumerator();

                for (int j = 0; j < Troops[i].Count; j++)
                {
                    AITroop troop = (AITroop)Troops[i][j];
                    if (!troop.Alive)
                    {
                        Troops[i].RemoveAt(j);
                        j--;

                        Battlefield.AddBody(new DeadTroop(troop.Position, troop.Team));
                        continue;
                    }

                    troop.Update();

                    commands.MoveNext();
                    troop.CommandPosition = commands.Current;

                    AveragePositions[i] += troop.Position;
                }

                AveragePositions[i] /= Troops[i].Count;
            }
        }

        public List<Entity> GetAllTroops()
        {
            List<Entity> allTroops = new List<Entity>();

            for (int i = 0; i < NumberOfGroups; i++) allTroops.AddRange(Troops[i]);

            return allTroops;
        }

        public Troop GetRandomTroop(bool team)
        {
            if (team != _team) return Battlefield.GetRandomTroop(team);

            int group = random.Next(0, 2);

            if (Troops[group].Count < 1)
            {
                group = group == 0 ? 1 : 0;
            }

            return (Troop)Troops[group][random.Next(0, Troops[group].Count)];
        }

        public Vector2 GetAveragePosition()
        {
            Vector2 position = new Vector2(0, 0);

            for (int i = 0; i < NumberOfGroups; i++)
            {
                position += AveragePositions[i];
            }

            return position /= NumberOfGroups;
        }

        private void CreateTroops()
        {

            for (int i = 0; i < 100; i++)
            {
                AITroop troop = new AITroop(Target.X, Target.Y, this, _team)
                {
                    TroopType = (int)TroopTypes.melee,
                };
                Troops[(int)TroopTypes.melee].Add(troop);
            }
            /*
            for (int i = 0; i < 100; i++)
            {
                AITroop troop = new AITroop(Target.X, Target.Y, this, _team)
                {
                    TroopType = (int)TroopTypes.melee,
                    Horse = true
                };
                Troops[(int)TroopTypes.horseMelee].Add(troop);
            }
            */

            for (int i = 0; i < 100; i++)
            {
                AITroop troop = new AITroop(Target.X, Target.Y, this, _team)
                {
                    TroopType = (int)TroopTypes.ranged
                };
                Troops[(int)TroopTypes.ranged].Add(troop);
            }

        }

        private void UpdatePositions()
        {
            for (int i = 0; i < NumberOfGroups; i++)
            {
                _positionLines[i].EnemyPosition = EnemyAveragePosition;
                _positionLines[i].Position = TroopTargets[i];
                _positionLines[i].UpdatePositions2(Troops[i].Count);
            }
        }

        public void FireProjectile(Projectile projectile)
        {
            Battlefield.FireProjectile(projectile);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 vec = new Vector2(32, 32);

            spriteBatch.Draw(Sprites.Target, EnemyAveragePosition - vec, Color.White);
        }
    }
}
