using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class ArrowTower : Defence
    {
        private Texture2D _arrowTex;
        private float _projSpeed;
        private float _projScale;
        private bool _homing;
        private Vector2 _spawnOffset;
        private float _arrowRotationOffset;
        private float _hitRadius;
        private int _mapWidth;
        private int _mapHeight;
        private float _offscreenMargin;

        public ArrowTower(Point cell, Vector2 position,Texture2D towerTex, Texture2D arrowTex,Rectangle towerCrop, int idleFrames, float idleFrameTime, float maxHealth, float damage, float range, float fireRate,
            float scale, float maxTilt, float flipDeadzone, float tiltFactor,
            float projSpeed, float projScale, bool homing, Vector2 spawnOffset, float arrowRotationOffset,
            float feetOffsetY, float hitRadius, int mapWidth, int mapHeight, float offscreenMargin,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
            : base(
                cell, position, maxHealth, damage, range, fireRate,
                scale, maxTilt, flipDeadzone, tiltFactor,
                barWidth, barHeight, barBorder, barOffsetY, barBackColor, barFillColor)
        {
            _arrowTex = arrowTex;
            _projSpeed = projSpeed;
            _projScale = projScale;
            _homing = homing;
            _spawnOffset = spawnOffset;
            _arrowRotationOffset = arrowRotationOffset;
            _hitRadius = hitRadius;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _offscreenMargin = offscreenMargin;

            _currentAnim = new Animation(towerTex, idleFrames, idleFrameTime, true, feetOffsetY, towerCrop);
        }

        public override void Update(GameTime gameTime, List<Enemy> enemies, List<Projectile> projectiles)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_cooldownTimer > 0f) _cooldownTimer -= dt;

            Enemy target = FindNearestEnemy(enemies);
            if (target != null)
            {
                AimAtTarget(target.Position);
                if (_cooldownTimer <= 0f)
                {
                    Vector2 spawnPos = Position + _spawnOffset;
                    projectiles.Add(new Arrow(
                        _arrowTex,
                        spawnPos,
                        target,
                        _projSpeed,
                        Damage,
                        _projScale,
                        _homing,
                        _arrowRotationOffset,
                        _hitRadius,
                        _mapWidth,
                        _mapHeight,
                        _offscreenMargin
                    ));
                    _cooldownTimer = 1f / FireRate;
                }
            }
            else
            {
                _rotation = 0f;
            }

            _currentAnim.Update(gameTime);
        }
    }

}
