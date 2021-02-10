using MountainBlade;
using MountainBlade.BattleState_classes;
using MountainBlade.BattleState_classes.Weapons.Melee_Weapons;
using MountainBlade.BattleState_classes.Weapons.Ranged_Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade
{
    class Troop : Entity
    {
        /*
         * Troop on the field
         * 
         */
        public static float DefaultSize = 15f;

        public TroopManager Manager;

        public static float MoveAwaySpeed { get; } = 0.8f;
        public float Size { get; } = 15f;

        protected Vector2 _position;
        protected int _health = 10;

        protected float _strength;

        protected Movement _movement;

        public Vector2 NextPosition { get => _position + _movement.Momentum; }

        public Vector2 Position => _position;
        public bool Alive => _health > 0;
        public bool Team { get; set; }

        public Vector2 Facing { get; set; }

        protected RangedWeapon _rangedWeapon;
        protected MeleeWeapon _meleeWeapon;

        protected SortedSet<Troop> _inVicinity;

        protected bool EnemyNearby { get => _inVicinity.Count > 0; }

        protected int _troopType;
        protected int _usingWeapon = 4;

        private float rotation2;

        private ByDistance _byDistanceComparer;

        private Boolean _damaged = false;

        public Boolean Horse;

        protected int _changeWeapon;

        protected Boolean ChangingWeapon { get => _changeWeapon > 0; }

        public Troop(float x, float y, TroopManager manager, bool team)
        {
            _position = new Vector2(x, y);
            _movement = new Movement(0.8f);

            Team = team;

            _rangedWeapon = new Bow(team);
            _meleeWeapon = new Sword();

            Manager = manager;

            _byDistanceComparer = new ByDistance(this);

            _inVicinity = new SortedSet<Troop>(_byDistanceComparer);

            _strength = 0.8f;

        }

        public void Hit(Projectile projectile)
        {
            _position += projectile.Movement / 10;

            _rangedWeapon.ResetCoolDown();

            Damage(projectile.Damage);
        }

        public void Damage(int amount)
        {
            _health -= amount;
            _damaged = true;
        }

        public void TroopCollision(Troop troop)
        {
            float distanceFrom = Vector2.Distance(_position, troop.Position);

            if (distanceFrom < Size)
            {
                Vector2 moveAway = Utils.DirectionTo(troop.Position, Position);

                _position += moveAway * _strength;

                troop.Push(Vector2.Negate(moveAway));
            }

            else if(troop.Team != Team && distanceFrom < 100)
            {
                _inVicinity.Add(troop);
                troop.InVicinity(this);
            }
        }

        public void Push(Vector2 amount)
        {
            _position += amount * _strength;
        }

        public void InVicinity(Troop troop)
        {
            _inVicinity.Add(troop);
        }

        public virtual void Update()
        {
            if (_movement.Facing.Length() > 0.01f) Facing = _movement.Facing;

            if (_health <= 0) return;

            if (ChangingWeapon)
                _changeWeapon--;

            _movement.Update();

            if (Horse) _position += _movement.Momentum * 2;
            
            else _position += _movement.Momentum;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Alive)
            {
                if (rotation2 == 0)
                    rotation2 = (float)(Utils.random.NextDouble() - 0.5) * 6;

                spriteBatch.Draw(Sprites.DeadTroop, Position, null, Team ? Color.BurlyWood : Color.DarkTurquoise, rotation2, new Vector2(Size / 2, Size / 2), 1f, SpriteEffects.None, 0f);
                return;
            }


            //spriteBatch.Draw(, CenterPosition() , Color.White);

            float rotation = Utils.VectorToRadians(Utils.PerpendicularVector(Facing));

            //Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size * 2, (int)Size * 2);
            //spriteBatch.Draw(Team ? Sprites.OrangeTeam : Sprites.BlueTeam, Position,null, Color.White, rotation, new Vector2(Size / 2, Size / 2), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Sprites.Troop, Position, null, rect, new Vector2(Size / 2, Size / 2), rotation2, new Vector2(2f, 2f), Team ? Color.BurlyWood : Color.DarkTurquoise, SpriteEffects.None, 0f);
            spriteBatch.Draw(Sprites.Troop, Position, null, Team ? Color.RosyBrown : Color.DarkTurquoise, rotation, new Vector2(Size / 2, Size / 2), 1f, SpriteEffects.None, 0f);

            if (_usingWeapon == 1 && !ChangingWeapon)
                _rangedWeapon.Draw(spriteBatch, _position, rotation);

            if (_usingWeapon == 0 && !ChangingWeapon)
                _meleeWeapon.Draw(spriteBatch, _position, rotation);

            if (_damaged)
            {
                spriteBatch.Draw(Sprites.Blood, Position, null, Color.White, rotation, new Vector2(Size / 2, Size / 2), 1f, SpriteEffects.None, 0f);
                _damaged = false;
            }
        }

        public Vector2 CenterPosition()
        {
            return _position - new Vector2(Size / 2, Size / 2);
        }
    }

    class DeadTroop : Entity
    {

        public Vector2 Position { get => _position; }

        private Vector2 _position;
        private bool _team;


        public float Size { get; }

        float rotation;

        public DeadTroop(Vector2 position, bool team)
        {
            _position = position;
            _team = team;
            Size = Troop.DefaultSize;
            rotation = (float)(Utils.random.NextDouble() - 0.5) * 6;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprites.DeadTroop, Position, null, _team ? Color.RosyBrown : Color.DarkTurquoise, rotation, new Vector2(Size / 2, Size / 2), 1f, SpriteEffects.None, 0f);
        }

    }

    class ByDistance : IComparer<Troop>
    {
        private Troop _thisTroop;

        public ByDistance(Troop thisTroop)
        {
            _thisTroop = thisTroop;
        }

        public int Compare(Troop troop1, Troop troop2)
        {
            float distance1 = Vector2.Distance(troop1.Position, _thisTroop.Position);

            float distance2 = Vector2.Distance(troop2.Position, _thisTroop.Position);

            if (distance1 > distance2) return 1;

            else return -1;

        }
    }

    class Movement
    {

        /*
         * Controls troops movement
         * 
        */

        private readonly static float Force = 0.1f, Decceleration = 0.8f, MaxSpeed = 0.3f, MaxHorseSpeed = 0.5f;

        public Vector2 Momentum { get => _momentum; set => _momentum = value; }
        public Vector2 Direction { get => _direction; set => _direction = value; }

        public float SpeedMod { get; set; }

        private Vector2 _direction, _momentum;

        private bool slowToStop = false;

        public Vector2 Facing { get; set; }

        private bool Horse;

        private float _maxSpeed;

        public Movement(float maxSpeed)
        {
            _maxSpeed = maxSpeed;

            SpeedMod = 1;

            _direction = new Vector2(0, 0);
            _momentum = new Vector2(0, 0);
        }

        public void SlowToStop()
        {
            slowToStop = true;
        }

        public void CancelSlow()
        {
            slowToStop = false;
        }

        public void SetDirection(Vector2 direction)
        {
            this._direction.X = direction.X;
            this._direction.Y = direction.Y;
        }

        public void Update()
        {
            Vector2 currentDirection = _momentum;

            currentDirection.Normalize();

            Facing = Vector2.Lerp(_direction, currentDirection, 0.5f);
 

            if (slowToStop)
            {
                if (_momentum.Length() > 0.1f)
                { _momentum *= Decceleration; }

                else
                { _momentum = Vector2.Zero; slowToStop = false; }
            }
            
            else
            {

                float change = Math.Min((Horse ? MaxHorseSpeed : MaxSpeed )/ Vector2.Distance(currentDirection, Facing), 1);

                if (float.IsNaN(change))
                {
                    change = 0.1f;
                }

                _momentum += (_direction * Force) * change;

                if (_momentum.Length() > _maxSpeed * SpeedMod * change)
                {
                    _momentum.Normalize();
                    _momentum *= _maxSpeed * SpeedMod * change;
                }
            }
  
            SpeedMod = 1;
        }
    }

    class AITroop : Troop
    {
        public Vector2 CommandPosition { get; set; }

        private Troop _troopTargeted;

        public bool TargetAlive { get => _troopTargeted != null && _troopTargeted.Alive;  }

        private float _accuracy = 0.1f;

        public int TroopType { get => _troopType; set => _troopType = value; }

        public static Random random = new Random();

        public AITroop(float x, float y, TroopManager manager, bool team) : base(x, y, manager, team)
        {
   
        }

        private void GoTo(Vector2 target)
        {
            _movement.Direction = Utils.DirectionTo(Position, target);
        }

        private void FindRandomEnemy()
        {

            _troopTargeted = Manager.GetRandomTroop(!Team);
        }

        private void MeleeAttack()
        {
            if (_usingWeapon != (int)TroopTypes.melee) {
                _changeWeapon = 10;
                _usingWeapon = (int)TroopTypes.melee;
            }

            if (ChangingWeapon)
            {
                _changeWeapon--;
                return;   
            }

            if (Vector2.Distance(Position, _troopTargeted.Position) > 18)
            {
                GoTo(_troopTargeted.Position);
            }

            else
            {
                _movement.SlowToStop();
            }

            if (_meleeWeapon.Charge > 60 && Vector2.Distance(Position, _troopTargeted.Position) < 20)
            {
                _troopTargeted.Damage(_meleeWeapon.Damage);
                _meleeWeapon.ResetCharge();
            }

            else
            {
                _meleeWeapon.IncrementCharge(1);
            }
        }

        private void FireWhenReady()
        {
            if (_usingWeapon != (int)TroopTypes.ranged)
            {
                _changeWeapon = 10;
                _usingWeapon = (int)TroopTypes.ranged;
            }

            if (ChangingWeapon)
            {
                _changeWeapon--;
                return;
            }

            if (!TargetAlive) _troopTargeted = Manager.GetRandomTroop(!Team);

            if (_rangedWeapon.CoolDown < 0 && Vector2.Distance(Position, _troopTargeted.Position) < _rangedWeapon.Range)
            {
                Manager.FireProjectile(_rangedWeapon.GetProjectile(Position, _troopTargeted.NextPosition, _accuracy));
                _rangedWeapon.ResetCoolDown();
            }

            if(_rangedWeapon.CoolDown < 500)
            {
                _movement.SpeedMod = 0.75f;
                Facing = Vector2.Lerp(Facing, _troopTargeted.Position, 0.8f);
            }
            

            _rangedWeapon.MinusCoolDown(10);

            if (_rangedWeapon.CoolDown < 150 && _rangedWeapon.CoolDown > 0)
            {
                _movement.SlowToStop();
                Vector2 leTarget = Utils.DirectionTo(Position, _troopTargeted.Position);
                Facing = leTarget;
                    //Vector2.Lerp( leTarget, Facing, 0.8f);
            }
        }

        private void GetNearbyTarget()
        {
            _troopTargeted = _inVicinity.First();
        }

        public override void Update()
        {
            switch (Manager.TroopCommands[TroopType])
            {

            case ("charge"):

                        switch (_troopType)
                        {
                        case (int)TroopTypes.melee:


                            if (!TargetAlive)
                            {
                                if (!EnemyNearby)
                                {
                                    FindRandomEnemy();
                                }

                                else
                                {
                                    GetNearbyTarget();
                                }
                            }


                            else if (EnemyNearby && (Vector2.Distance(_troopTargeted.Position, _position) > 50))
                            {
                                GetNearbyTarget();
                            }

                            MeleeAttack();

                        break;

                        case (int)TroopTypes.ranged:

                            if (EnemyNearby)
                            {
                               // _rangedWeapon.ResetCoolDown();
                                GetNearbyTarget();
                                MeleeAttack();
                                break;
                            }

                            float distanceFromTarget = Vector2.Distance(Position, _troopTargeted.Position);

                            if (distanceFromTarget > _rangedWeapon.Range - 50) _movement.Direction = Utils.DirectionTo(Position, _troopTargeted.Position);

                            else if(distanceFromTarget < _rangedWeapon.Range/2)
                            {
                                _movement.Direction = Utils.DirectionTo(_troopTargeted.Position, Position);
                            }

                            else _movement.SlowToStop();
                            
                            FireWhenReady();
                            

                            break;

                        default:

                            break;
            }

            break;


            case ("toPosition"):

                    if (_troopType == (int)TroopTypes.ranged)
                    {
                        FireWhenReady();
                    } 

                    if(EnemyNearby)
                    {
                        GetNearbyTarget();
                        MeleeAttack();
                    }

                    float distanceToTarget = Vector2.Distance(Position, CommandPosition);

                    GoTo(CommandPosition);

                    if (distanceToTarget < 4)
                    {
                        _movement.SlowToStop();
                    }

                    else if (distanceToTarget < 40)
                    {
                        _movement.SpeedMod = distanceToTarget / 40;
                    }

                        break;

            default:

            break;
            }


            base.Update();
            _inVicinity.Clear();

            if(TargetAlive)
            {
                Facing = Vector2.Lerp(Utils.DirectionTo(Position, _troopTargeted.Position), Facing, 0.5f);
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine("TroopType=" + _troopType + " _usingWeapon=" + _usingWeapon + "_changeWeapon=" + _changeWeapon);
        }
    }

}
