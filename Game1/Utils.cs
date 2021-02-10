using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MountainBlade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade
{
    static class Utils
    {

        public static Random random = new Random();

        public static Vector2 DirectionTo(Vector2 v1, Vector2 v2)
        {
            Vector2 v = new Vector2(v2.X - v1.X, v2.Y - v1.Y);

            v.Normalize();

            return v;
        }

        public static Vector2 PerpendicularVector(Vector2 v)
        {
            float sin = (float)Math.Sin(1.57);
            float cos = (float)Math.Cos(1.57);

            float tx = v.X;
            float ty = v.Y;

            return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
        }

        public static bool Intersects(Entity e1, Entity e2)
        {
            if (Vector2.Distance(e1.Position, e2.Position) < e1.Size/2 + e2.Size/2)
            {
                return true;
            }

            else return false;
        }

        public static bool InVicinity(Entity e1, Entity e2)
        {
            if (Vector2.Distance(e1.Position, e2.Position) < e1.Size * 4 + e2.Size * 4)
            {
                return true;
            }

            else return false;
        }

        public static float VectorToRadians(Vector2 v)
        {
            v.Normalize();

            return (float) Math.Atan2(v.Y, v.X);
        }
    }

    class PositionLine
    {
        public Vector2 Position { get; set; }

        public Vector2 EnemyPosition { get; set; }

        private Vector2 _angle, _end1, _end2;

        private float _length;
        private float spreadX = 1.6f, spreadY = 1.6f;
        private int _rows = 4;
        private int _perRow;

        public List<Vector2> Positions;

        public PositionLine()
        {
            Positions = new List<Vector2>();
        }

        private void Update()
        {
            if (EnemyPosition.X == 0)
            {
                EnemyPosition = new Vector2(100, 1000);
            }

            if (Position.X == 0)
            {
                Position = new Vector2(10, 10);
            }
            UpdateAngle();
            UpdateEndPositions();
        }

        private void UpdateAngle()
        {
            _angle = Utils.DirectionTo(Position, EnemyPosition);

            _angle = Utils.PerpendicularVector(_angle);
        }

        private void UpdateEndPositions()
        {
            _end1 = new Vector2(_angle.X, _angle.Y);
            _end1 *= _length/2;
            _end1 += Position;

            _end2 = new Vector2(_angle.X, _angle.Y);
            _end2 *= -_length/2;
            _end2 += Position;
        }

        public List<Vector2> UpdatePositions(int numberOfTroops)
        {
            Positions.Clear();

            _perRow = numberOfTroops / _rows + (numberOfTroops % _rows == 0 ? 0 : 1);

            _length = ((_perRow) * (Troop.DefaultSize*spreadX)) - Troop.DefaultSize;

            Update();

            for (int j = 0; j < _rows; j++)
            {
                Vector2 offsetY = Utils.PerpendicularVector(_angle);
                offsetY *= (Troop.DefaultSize*spreadY) * j;

                for (int i = 0; i < _perRow; i++)
                {
                    Vector2 position = _end2;
                    Vector2 offsetX = _angle;
                    offsetX *= (Troop.DefaultSize*spreadX) * i;
                    offsetX += offsetY;
                    position += offsetX;
                    Positions.Add(position);
                }
            }

            //Positions.Reverse();

            return Positions;
        }

        public void UpdatePositions2(int numberOfTroops)
        {
            Positions.Clear();

            _perRow = numberOfTroops / _rows + (numberOfTroops % _rows == 0 ? 0 : 1);

            _length = ((_perRow) * (Troop.DefaultSize * spreadX)) - Troop.DefaultSize;

            Update();
            for (int j = 0; j < _perRow; j++)
            {
                for (int i = 0; i < _rows; i++)
                {
                    Vector2 offsetX = _angle;
                    offsetX *= (Troop.DefaultSize * spreadX) * j;

                    Vector2 position = _end2;
                    Vector2 offsetY = Utils.PerpendicularVector(_angle);
                    offsetY *= (Troop.DefaultSize * spreadY) * i;
                    offsetX += offsetY;
                    position += offsetX;
                    Positions.Add(position);
                }
            }
        }
    }

    public static class Sprites
    {
        public static Texture2D OrangeTeam, BlueTeam, QBox, Arrow, Target, Blood, Troop, ArrowSmall, DeadTroop, Bow, Sword, Horse;
    }

}
