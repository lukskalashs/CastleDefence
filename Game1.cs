using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState _currentState = GameState.Splash;

        //Window and grid setup values
        private int _windowWidth = 1280;
        private int _windowHeight = 800;
        private int _mapWidth = 1280;
        private int _mapHeight = 720;
        private int _hudHeight = 80;
        private int _cellSize = 80;
        private int _gridCols = 16;
        private int _gridRows = 9;
        private int _leakLimit = 3;
        private int _totalStages = 3;

        //Ids & Costs
        private int _knightId = 1;
        private int _elfId = 2;
        private int _towerId = 3;
        private int _ogreId = 1;
        private int _trollId = 2;

        private int _knightCost = 60;
        private int _elfCost = 100;
        private int _towerCost = 80;

        private Rectangle _knightCrop = new Rectangle(0, 0, 282, 312);
        private Rectangle _elfCrop = new Rectangle(790, 338, 415, 523);
        private Rectangle _towerCrop = new Rectangle(5, 8, 62, 93);

        private float _knightScale = 0.28f;
        private float _elfScale = 0.28f;
        private float _towerScale = 0.86f;
        private float _feetOffsetY = 24f;

        //Systeming
        private Grid _grid;
        private StageManager _stageManager;
        private List<ShopButton> _shopButtons = new List<ShopButton>();

        //Textures
        private Texture2D _pixel;
        private SpriteFont _font;
        private Texture2D _dungeonBgTex, _iceBgTex, _lavaBgTex;
        private Texture2D _knightAttackTex, _knightHurtTex, _knightDieTex;
        private Texture2D _elfIdleTex, _elfAttackTex, _ballTex;
        private Texture2D _towerTex, _arrowTex;
        private Texture2D _ogreWalkTex, _ogreAttackTex, _ogreHurtTex, _ogreDieTex;
        private Texture2D _trollWalkTex, _trollAttackTex, _trollHurtTex, _trollDieTex;

        //Selection & input tracking
        private int _selectedDefenceId = 0;
        private MouseState _prevMouse;
        private KeyboardState _prevKeyboard;
        private bool _showDebugOverlay = false;

        private string _statusMessage = string.Empty;
        private float _statusTimer = 0f;
        private float _statusDuration = 2.0f;
        private float _stageCompleteTimer = 0f;
        private float _stageCompleteDelay = 2.5f;

        private string _notEnoughGoldMsg = "Not enough gold";
        private string _invalidPlaceMsg = "Can't place here";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = _windowWidth;
            _graphics.PreferredBackBufferHeight = _windowHeight;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _grid = new Grid(
                _cellSize, _gridCols, _gridRows, _mapWidth, _mapHeight,
                _knightId, _elfId, _towerId,
                new Color(0, 200, 0, 95),
                new Color(220, 20, 20, 105),
                new Color(235, 140, 0, 105),
                new Color(255, 255, 255, 90),
                8, 3f, new Vector2(4f, 4f), 0.72f
            );

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _font = Content.Load<SpriteFont>("File");

            _elfIdleTex = Content.Load<Texture2D>("idle_elf");
            _elfAttackTex = Content.Load<Texture2D>("elf_attack");
            _ballTex = Content.Load<Texture2D>("elf_blue_energy_ball");

            _knightAttackTex = Content.Load<Texture2D>("attack_knight1");
            _knightHurtTex = Content.Load<Texture2D>("hurt_knight1");
            _knightDieTex = Content.Load<Texture2D>("die_knight1");

            _ogreWalkTex = Content.Load<Texture2D>("walk_org1");
            _ogreAttackTex = Content.Load<Texture2D>("attack_org1");
            _ogreHurtTex = Content.Load<Texture2D>("hurt_org1");
            _ogreDieTex = Content.Load<Texture2D>("die_org1");

            _trollWalkTex = Content.Load<Texture2D>("run_org2");
            _trollAttackTex = Content.Load<Texture2D>("attack_org2");
            _trollHurtTex = Content.Load<Texture2D>("hurt_org2");
            _trollDieTex = Content.Load<Texture2D>("die_org2");

            _towerTex = Content.Load<Texture2D>("Tower");
            _arrowTex = Content.Load<Texture2D>("Arrow");

            _dungeonBgTex = Content.Load<Texture2D>("Dungeon_background");
            _iceBgTex = Content.Load<Texture2D>("Ice_Background");
            _lavaBgTex = Content.Load<Texture2D>("Lava_Background");

            Texture2D[] stageBgs = { _dungeonBgTex, _iceBgTex, _lavaBgTex };

            _stageManager = new StageManager(
                _grid, _mapWidth, _mapHeight, _leakLimit, _totalStages,
                _knightId, _elfId, _towerId, _ogreId, _trollId,
                _knightCost, _elfCost, _towerCost,
                _notEnoughGoldMsg, _invalidPlaceMsg,
                stageBgs,
                _knightAttackTex, _knightHurtTex, _knightDieTex,
                _elfIdleTex, _elfAttackTex, _ballTex,
                _towerTex, _arrowTex,
                _ogreWalkTex, _ogreAttackTex, _ogreHurtTex, _ogreDieTex,
                _trollWalkTex, _trollAttackTex, _trollHurtTex, _trollDieTex
            );
            _stageManager.LoadStage(0);

            // Create the 3 shop buttons in the bottom HUD
            int btnY = _mapHeight + 6;
            int w = 185;
            int h = 68;
            int s = 14;
            int startX = 200;

            Rectangle iconOff = new Rectangle(8, 6, 56, 56);
            Vector2 titleOff = new Vector2(70f, 12f);
            Vector2 costOff = new Vector2(70f, 36f);
            Color normalBg = new Color(52, 56, 68);
            Color hoverBg = new Color(70, 75, 90);
            Color selectBg = new Color(65, 115, 75);
            Color disableBg = new Color(45, 45, 50);
            Color borderCol = new Color(140, 145, 160);
            Color disableIcon = new Color(110, 110, 110, 180);
            Color disableCost = new Color(170, 90, 90);

            _shopButtons.Add(new ShopButton(
                _knightId, "Knight", _knightCost, _knightAttackTex, _knightCrop,
                new Rectangle(startX, btnY, w, h), iconOff, titleOff, costOff, 2,
                normalBg, hoverBg, selectBg, disableBg, borderCol, disableIcon, disableCost
            ));

            _shopButtons.Add(new ShopButton(
                _elfId, "Elf Mage", _elfCost, _elfIdleTex, _elfCrop,
                new Rectangle(startX + (w + s), btnY, w, h), iconOff, titleOff, costOff, 2,
                normalBg, hoverBg, selectBg, disableBg, borderCol, disableIcon, disableCost
            ));

            _shopButtons.Add(new ShopButton(
                _towerId, "Arrow Tower", _towerCost, _towerTex, _towerCrop,
                new Rectangle(startX + (w + s) * 2, btnY, w, h), iconOff, titleOff, costOff, 2,
                normalBg, hoverBg, selectBg, disableBg, borderCol, disableIcon, disableCost
            ));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();

            bool leftClicked = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;
            bool rightClicked = mouse.RightButton == ButtonState.Pressed && _prevMouse.RightButton == ButtonState.Released;
            bool enterPressed = kb.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter);
            bool escPressed = kb.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape);
            

            
            if (_statusTimer > 0f) _statusTimer -= dt;

            //STATE: SPLASH SCREEN, GAME OVER, OR VICTORY
            if (_currentState == GameState.Splash || _currentState == GameState.GameOver || _currentState == GameState.Victory)
            {
                if (escPressed) Exit();
                if (leftClicked || enterPressed)
                {
                    _selectedDefenceId = 0;
                    _stageManager.LoadStage(0);
                    _currentState = GameState.Playing;
                }
                _prevMouse = mouse;
                _prevKeyboard = kb;
                return;
            }

            //STATE: STAGE COMPLETE
            if (_currentState == GameState.StageComplete)
            {
                _stageCompleteTimer -= dt;
                if (leftClicked || enterPressed || _stageCompleteTimer <= 0f)
                {
                    _selectedDefenceId = 0;
                    _stageManager.LoadStage(_stageManager.CurrentStageIndex + 1);
                    _currentState = GameState.Playing;
                }
                _prevMouse = mouse;
                _prevKeyboard = kb;
                return;
            }

            //STATE: PLAYING
            if (rightClicked || escPressed)
            {
                _selectedDefenceId = 0;
            }

            if (leftClicked)
            {
                bool clickedShop = false;
                foreach (var btn in _shopButtons)
                {
                    if (btn.Contains(mouse.Position))
                    {
                        clickedShop = true;
                        if (_stageManager.Funds >= btn.Cost)
                        {
                            _selectedDefenceId = btn.DefenceId;
                        }
                        else
                        {
                            _statusMessage = _notEnoughGoldMsg;
                            _statusTimer = _statusDuration;
                        }
                        break;
                    }
                }

                //Snap and place defence on grid cell
                if (!clickedShop && _selectedDefenceId != 0 && _grid.IsMouseInMap(mouse.Position))
                {
                    Point cell = _grid.ToCell(mouse.Position);
                    if (_stageManager.TryPlaceDefence(_selectedDefenceId, cell, out string error))
                    {
                        if (_stageManager.Funds < _stageManager.GetDefenceCost(_selectedDefenceId))
                        {
                            _selectedDefenceId = 0;
                        }
                    }
                    else
                    {
                        _statusMessage = error;
                        _statusTimer = _statusDuration;
                    }
                }
            }

            _stageManager.Update(gameTime);

            if (_stageManager.IsStageLost)
            {
                _selectedDefenceId = 0;
                _currentState = GameState.GameOver;
            }
            else if (_stageManager.IsStageWon)
            {
                _selectedDefenceId = 0;
                if (_stageManager.CurrentStageIndex + 1 >= _stageManager.TotalStages)
                {
                    _currentState = GameState.Victory;
                }
                else
                {
                    _stageCompleteTimer = _stageCompleteDelay;
                    _currentState = GameState.StageComplete;
                }
            }

            _prevMouse = mouse;
            _prevKeyboard = kb;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            //Draw Game World
            _stageManager.Draw(_spriteBatch, _pixel, _font);

           
            //Draw Ghost Preview when placing a tower/defence
            if (_currentState == GameState.Playing && _selectedDefenceId != 0 && _grid.IsMouseInMap(_prevMouse.Position))
            {
                Point cell = _grid.ToCell(_prevMouse.Position);
                int cost = _stageManager.GetDefenceCost(_selectedDefenceId);
                bool valid = _grid.CanPlace(_selectedDefenceId, cell) && _stageManager.Funds >= cost;
                Rectangle cellRect = _grid.GetCellRect(cell);

                _spriteBatch.Draw(_pixel, cellRect, valid ? new Color(30, 220, 60, 85) : new Color(230, 30, 30, 95));

                Texture2D previewTex = _towerTex;
                Rectangle src = _towerCrop;
                float scale = _towerScale;

                if (_selectedDefenceId == _knightId)
                {
                    previewTex = _knightAttackTex;
                    src = _knightCrop;
                    scale = _knightScale;
                }
                else if (_selectedDefenceId == _elfId)
                {
                    previewTex = _elfIdleTex;
                    src = _elfCrop;
                    scale = _elfScale;
                }

                Vector2 center = _grid.CellCentre(cell);
                Vector2 origin = new Vector2(src.Width / 2f, src.Height);
                Color tint = valid ? new Color(120, 255, 120, 185) : new Color(255, 100, 100, 185);

                _spriteBatch.Draw(previewTex, new Vector2(center.X, center.Y + _feetOffsetY), src, tint, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            //Draw HUD Overlay at bottom (1280x80) got complete measurements from the original game screenshot
            Rectangle hudRect = new Rectangle(0, _mapHeight, _windowWidth, _hudHeight);
            _spriteBatch.Draw(_pixel, hudRect, new Color(26, 28, 36));
            _spriteBatch.Draw(_pixel, new Rectangle(0, _mapHeight, _windowWidth, 2), new Color(90, 95, 115));

            _spriteBatch.DrawString(_font, $"Gold: {_stageManager.Funds}", new Vector2(18f, 736f), Color.Gold);
           

            foreach (var btn in _shopButtons)
            {
                bool isSelected = _selectedDefenceId == btn.DefenceId;
                bool canAfford = _stageManager.Funds >= btn.Cost;
                bool isHovered = btn.Contains(_prevMouse.Position);
                btn.Draw(_spriteBatch, _pixel, _font, isSelected, canAfford, isHovered);
            }

            _spriteBatch.DrawString(_font, _stageManager.CurrentStageName, new Vector2(805f, 734f), Color.White);
            _spriteBatch.DrawString(_font, $"Killed: {_stageManager.KilledCount}/{_stageManager.TotalEnemiesInStage}", new Vector2(805f, 764f), Color.LightGreen);
            _spriteBatch.DrawString(_font, $"Leaked: {_stageManager.LeakedCount}/{_leakLimit}", new Vector2(980f, 764f), Color.Salmon);

            if (_statusTimer > 0f && !string.IsNullOrEmpty(_statusMessage))
            {
                Vector2 msgSize = _font.MeasureString(_statusMessage);
                Rectangle banner = new Rectangle((int)((_mapWidth - msgSize.X) / 2f) - 16, _mapHeight - 44, (int)msgSize.X + 32, 34);
                _spriteBatch.Draw(_pixel, banner, new Color(20, 20, 25, 220));
                _spriteBatch.DrawString(_font, _statusMessage, new Vector2(banner.X + 16, banner.Y + 6), Color.Yellow);
            }

            //Draw Splash / End Screens
            if (_currentState == GameState.Splash)
            {
                DrawSplashOverlay(
                    "CASTLE DEFENCE",
                    "Defend the realm across 3 progressive stages: Dungeon, Ice, and Lava!",
                    "Click anywhere or press ENTER to Start",
                    Color.Gold
                );
            }
            else if (_currentState == GameState.StageComplete)
            {
                DrawSplashOverlay(
                    $"{_stageManager.CurrentStageName} Cleared!",
                    $"Advancing to Stage {_stageManager.CurrentStageIndex + 2}...",
                    "Click or press ENTER to continue",
                    Color.LightGreen
                );
            }
            else if (_currentState == GameState.GameOver)
            {
                DrawSplashOverlay(
                    "DEFEAT! THE CASTLE HAS FALLEN!",
                    "More than 3 enemies breached your defences.",
                    "Click anywhere or press ENTER to Play Again",
                    Color.Crimson
                );
            }
            else if (_currentState == GameState.Victory)
            {
                DrawSplashOverlay(
                    "VICTORY! THE REALM IS SAVED!",
                    "You conquered all 3 stages of Castle Defence!",
                    "Click anywhere or press ENTER to Play Again",
                    Color.Gold
                );
            }

            _spriteBatch.End();

            base.Update(gameTime);
        }

 

        private void DrawSplashOverlay(string title, string subtitle, string promptinng, Color titleColor)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(0, 0, _windowWidth, _windowHeight), new Color(12, 14, 22, 225));
            _spriteBatch.Draw(_pixel, new Rectangle(220, 200, 840, 380), new Color(28, 32, 46));

            Vector2 titleSize = _font.MeasureString(title);
            Vector2 subSize = _font.MeasureString(subtitle);
            Vector2 promptSize = _font.MeasureString(promptinng);

            _spriteBatch.DrawString(_font, title, new Vector2((_windowWidth - titleSize.X) / 2f, 275f), titleColor);
            _spriteBatch.DrawString(_font, subtitle, new Vector2((_windowWidth - subSize.X) / 2f, 355f), Color.White);
            _spriteBatch.DrawString(_font, promptinng, new Vector2((_windowWidth - promptSize.X) / 2f, 460f), Color.Gold);
        }
    }


    
}
