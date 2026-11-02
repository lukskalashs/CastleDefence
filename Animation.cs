using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Animation
    {
        private Texture2D _sheet;
        private Rectangle[] _frames;
        private float _frameTime;
        private bool _loop;
        private float _feetOffsetY;
        private float _timer = 0f;

        public int FrameCount => _frames.Length;
        public int CurrentFrame { get; private set; }
        public bool Finished { get; private set; }

        public Animation(Texture2D sheet, int frameCount, float frameTime, bool loop, float feetOffsetY, Rectangle? customCrop = null)
        {
            _sheet = sheet;
            int count = Math.Max(1, frameCount);
            _frames = new Rectangle[count];
            _frameTime = frameTime;
            _loop = loop;
            _feetOffsetY = feetOffsetY;

            if (customCrop.HasValue && count == 1)
            {
                _frames[0] = customCrop.Value;
            }
            else
            {
                // Slice sheet using float math so uneven sprite sheets dont drift
                int sheetW = sheet.Width;
                int sheetH = sheet.Height;
                for (int i = 0; i < count; i++)
                {
                    int x = (int)Math.Round(i * sheetW / (double)count);
                    int nextX = (int)Math.Round((i + 1) * sheetW / (double)count);
                    int w = nextX - x;
                    _frames[i] = new Rectangle(x, 0, w, sheetH);
                }
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            _timer = 0f;
            Finished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (Finished || _frames.Length <= 1)
            {
                if (_frames.Length <= 1 && !_loop)
                {
                    Finished = true;
                }
                return;
            }

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_timer >= _frameTime)
            {
                _timer -= _frameTime;
                if (CurrentFrame < _frames.Length - 1)
                {
                    CurrentFrame++;
                }
                else if (_loop)
                {
                    CurrentFrame = 0;
                }
                else
                {
                    CurrentFrame = _frames.Length - 1;
                    Finished = true;
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, float rotation, bool flipHorizontally, Color color)
        {
            Rectangle src = _frames[CurrentFrame];
            // Origin at bottom-centre so feet don't jitter(couldt fix)
            Vector2 origin = new Vector2(src.Width / 2f, src.Height);
            SpriteEffects effects = flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPos = new Vector2(position.X, position.Y + _feetOffsetY);
            spriteBatch.Draw(_sheet, drawPos, src, color, rotation, origin, scale, effects, 0f);
        }
    }
}

