using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CastleDefence
{
    public class ElfMage : Defence
    {
        private Animation _idleAnim;
        private Animation _attackAnim;
        private Texture2D _ballTex;
        private float _projSpeed;
        private float _projScale;
        private bool _homing;
        private Vector2 _spawnOffset;
        private float _hitRadius;
        private int _mapWidth;
        private int _mapHeight;
        private float _offscreenMargin;

        private bool _isAttacking = false;
        private bool _firedBall = false;
        private Enemy _lockedTarget;

        public ElfMage(
            Point cell, Vector2 position,
            Texture2D idleTex, Texture2D attackTex, Texture2D ballTex,
            Rectangle idleCrop, int idleFrames, float idleFrameTime,
            int attackFrames, float attackFrameTime,
            float maxHealth, float damage, float range, float fireRate,
            float scale, float maxTilt, float flipDeadzone, float tiltFactor,
            float projSpeed, float projScale, bool homing, Vector2 spawnOffset,
            float feetOffsetY, float hitRadius, int mapWidth, int mapHeight, float offscreenMargin,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
            : base(
                cell, position, maxHealth, damage, range, fireRate,
                scale, maxTilt, flipDeadzone, tiltFactor,
                barWidth, barHeight, barBorder, barOffsetY, barBackColor, barFillColor)
        {
            _ballTex = ballTex;
            _projSpeed = projSpeed;
            _projScale = projScale;
            _homing = homing;
            _spawnOffset = spawnOffset;
            _hitRadius = hitRadius;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _offscreenMargin = offscreenMargin;

            _idleAnim = new Animation(idleTex, idleFrames, idleFrameTime, true, feetOffsetY, idleCrop);
            _attackAnim = new Animation(attackTex, attackFrames, attackFrameTime, false, feetOffsetY);

            _currentAnim = _idleAnim;
        }

        public override void Update(GameTime gameTime, List<Enemy> enemies, List<Projectile> projectiles)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_cooldownTimer > 0f) _cooldownTimer -= dt;

            Enemy nearest = FindNearestEnemy(enemies);
            if (nearest != null)
            {
                AimAtTarget(nearest.Position);
            }

            if (!_isAttacking && _cooldownTimer <= 0f && nearest != null)
            {
                _isAttacking = true;
                _firedBall = false;
                _lockedTarget = nearest;
                _attackAnim.Reset();
                _currentAnim = _attackAnim;
                _cooldownTimer = 1f / FireRate;
            }

            _currentAnim.Update(gameTime);

            if (_isAttacking)
            {
                // Fire blue energy ball on the last frame (frame 4) of attack animation
                if (!_firedBall && _attackAnim.CurrentFrame == _attackAnim.FrameCount - 1)
                {
                    Enemy targetToShoot = (_lockedTarget != null && !_lockedTarget.IsDying && !_lockedTarget.IsDead)
                        ? _lockedTarget
                        : FindNearestEnemy(enemies);

                    if (targetToShoot != null)
                    {
                        AimAtTarget(targetToShoot.Position);
                        float offsetX = _flipHorizontal ? -_spawnOffset.X : _spawnOffset.X;
                        Vector2 spawnPos = new Vector2(Position.X + offsetX, Position.Y + _spawnOffset.Y);

                        projectiles.Add(new EnergyBall(
                            _ballTex,
                            spawnPos,
                            targetToShoot,
                            _projSpeed,
                            Damage,
                            _projScale,
                            _homing,
                            _hitRadius,
                            _mapWidth,
                            _mapHeight,
                            _offscreenMargin
                        ));
                    }
                    _firedBall = true;
                }

                if (_attackAnim.Finished)
                {
                    _isAttacking = false;
                    _currentAnim = _idleAnim;
                }
            }
        }
    }

}
