using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    // Controls stage loading, wave spawning, entity updates, funds, and win/lose conditions
    public class StageManager
    {
        private Grid _grid;
        private int _mapWidth;
        private int _mapHeight;
        private int _leakLimit;
        private int _totalStages;

        private int _knightId;
        private int _elfId;
        private int _towerId;
        private int _ogreId;
        private int _trollId;

        private int _knightCost;
        private int _elfCost;
        private int _towerCost;

        private string _notEnoughGoldMsg;
        private string _invalidPlaceMsg;

        //Textures  will be injecyed in Game1
        private Texture2D[] _stageBackgrounds;
        private Texture2D _knightAttackTex, _knightHurtTex, _knightDieTex;
        private Texture2D _elfIdleTex, _elfAttackTex, _ballTex;
        private Texture2D _towerTex, _arrowTex;
        private Texture2D _ogreWalkTex, _ogreAttackTex, _ogreHurtTex, _ogreDieTex;
        private Texture2D _trollWalkTex, _trollAttackTex, _trollHurtTex, _trollDieTex;

        // Stages (3 stages: Dungeon, Ice, Lava)
        private string[] _stageNames;
        private int[] _stageStartFunds;
        private float[] _stageHealthMults;
        private float[] _stageSpeedMults;
        private List<int>[] _stageSpawnTypes;
        private List<float>[] _stageSpawnDelays;
        private List<Vector2>[] _stageWaypoints;
        private List<Point>[] _stageLaneCells;
        private List<Point>[] _stageBlockedRoad;
        private List<Point>[] _stageBlockedScenery;

        public List<Defence> Defences { get; private set; } = new List<Defence>();
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();
        public List<FloatingText> FloatingTexts { get; private set; } = new List<FloatingText>();

        public int CurrentStageIndex { get; private set; }
        public int Funds { get; private set; }
        public int KilledCount { get; private set; }
        public int LeakedCount { get; private set; }
        public int TotalStages => _totalStages;

        private int _nextSpawnIndex = 0;
        private float _spawnTimer = 0f;

        public string CurrentStageName => _stageNames[CurrentStageIndex];
        public int TotalEnemiesInStage => _stageSpawnTypes[CurrentStageIndex].Count;
        public List<Vector2> CurrentWaypoints => _stageWaypoints[CurrentStageIndex];

        public bool IsStageLost => LeakedCount > _leakLimit;
        public bool IsStageWon
        {
            get
            {
                return !IsStageLost &&
                       _nextSpawnIndex >= TotalEnemiesInStage &&
                       Enemies.Count == 0 &&
                       (KilledCount + LeakedCount) >= TotalEnemiesInStage;
            }
        }

        public StageManager(
            Grid grid, int mapWidth, int mapHeight, int leakLimit, int totalStages,
            int knightId, int elfId, int towerId, int ogreId, int trollId,
            int knightCost, int elfCost, int towerCost,
            string notEnoughGoldMsg, string invalidPlaceMsg,
            Texture2D[] stageBackgrounds,
            Texture2D knightAttackTex, Texture2D knightHurtTex, Texture2D knightDieTex,
            Texture2D elfIdleTex, Texture2D elfAttackTex, Texture2D ballTex,
            Texture2D towerTex, Texture2D arrowTex,
            Texture2D ogreWalkTex, Texture2D ogreAttackTex, Texture2D ogreHurtTex, Texture2D ogreDieTex,
            Texture2D trollWalkTex, Texture2D trollAttackTex, Texture2D trollHurtTex, Texture2D trollDieTex)
        {
            _grid = grid;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _leakLimit = leakLimit;
            _totalStages = totalStages;

            _knightId = knightId;
            _elfId = elfId;
            _towerId = towerId;
            _ogreId = ogreId;
            _trollId = trollId;

            _knightCost = knightCost;
            _elfCost = elfCost;
            _towerCost = towerCost;

            _notEnoughGoldMsg = notEnoughGoldMsg;
            _invalidPlaceMsg = invalidPlaceMsg;

            _stageBackgrounds = stageBackgrounds;
            _knightAttackTex = knightAttackTex;
            _knightHurtTex = knightHurtTex;
            _knightDieTex = knightDieTex;
            _elfIdleTex = elfIdleTex;
            _elfAttackTex = elfAttackTex;
            _ballTex = ballTex;
            _towerTex = towerTex;
            _arrowTex = arrowTex;
            _ogreWalkTex = ogreWalkTex;
            _ogreAttackTex = ogreAttackTex;
            _ogreHurtTex = ogreHurtTex;
            _ogreDieTex = ogreDieTex;
            _trollWalkTex = trollWalkTex;
            _trollAttackTex = trollAttackTex;
            _trollHurtTex = trollHurtTex;
            _trollDieTex = trollDieTex;

            InitializeStageData();
        }

        private void InitializeStageData()
        {
            _stageNames = new string[]
            {
                "Stage 1: Dungeon Cavern",
                "Stage 2: Frozen Pass",
                "Stage 3: Infernal Citadel"
            };

            _stageStartFunds = new int[] { 220, 260, 320 };
            _stageHealthMults = new float[] { 1.0f, 1.35f, 1.75f };
            _stageSpeedMults = new float[] { 1.0f, 1.15f, 1.30f };

            _stageWaypoints = new List<Vector2>[_totalStages];
            _stageLaneCells = new List<Point>[_totalStages];
            _stageBlockedRoad = new List<Point>[_totalStages];
            _stageBlockedScenery = new List<Point>[_totalStages];

            //DUNGEON
            _stageWaypoints[0] = new List<Vector2>
            {
                new Vector2(0, 262), new Vector2(80, 290), new Vector2(110, 390),
                new Vector2(170, 470), new Vector2(300, 485), new Vector2(600, 485),
                new Vector2(800, 490), new Vector2(870, 520), new Vector2(905, 600),
                new Vector2(905, 719)
            };
            _stageLaneCells[0] = new List<Point>
            {
                new Point(0,3), new Point(1,3), new Point(1,4), new Point(1,5),
                new Point(2,5), new Point(2,6), new Point(3,5), new Point(3,6),
                new Point(4,5), new Point(4,6), new Point(5,5), new Point(5,6),
                new Point(6,5), new Point(6,6), new Point(7,5), new Point(7,6),
                new Point(8,5), new Point(8,6), new Point(9,5), new Point(9,6),
                new Point(10,6), new Point(11,6), new Point(11,7), new Point(11,8)
            };
            _stageBlockedRoad[0] = new List<Point>
            {
                new Point(8,2), new Point(8,3), new Point(8,4), new Point(9,2),
                new Point(10,1), new Point(10,2), new Point(11,0), new Point(11,1),
                new Point(11,2), new Point(10,3), new Point(9,3), new Point(9,4)
            };
            _stageBlockedScenery[0] = new List<Point>
            {
                new Point(0,0), new Point(1,0), new Point(2,0), new Point(3,0), new Point(4,0), new Point(5,0),
                new Point(0,1), new Point(1,1), new Point(2,1), new Point(3,1), new Point(4,1), new Point(5,1),
                new Point(3,2), new Point(4,2), new Point(5,2), new Point(1,2),
                new Point(2,3), new Point(3,3), new Point(4,3), new Point(5,3), new Point(6,3), new Point(7,3),
                new Point(4,4), new Point(5,4), new Point(6,4), new Point(7,4),
                new Point(6,0), new Point(8,0), new Point(9,0), new Point(6,1), new Point(13,1), new Point(13,4),
                new Point(14,3), new Point(15,3), new Point(11,5), new Point(0,6),
                new Point(4,7), new Point(5,7), new Point(12,7), new Point(13,7), new Point(14,7),
                new Point(14,0), new Point(15,0),
                new Point(0,8), new Point(1,8), new Point(2,8), new Point(3,8), new Point(6,8), new Point(8,8), new Point(9,8),
                new Point(12,8), new Point(13,8), new Point(14,8), new Point(15,8)
            };

            //Stage2:ICE
            _stageWaypoints[1] = new List<Vector2>
            {
                new Vector2(575, 2), new Vector2(599, 57), new Vector2(654, 81),
                new Vector2(713, 92), new Vector2(763, 127), new Vector2(785, 183),
                new Vector2(768, 241), new Vector2(710, 259), new Vector2(660, 293),
                new Vector2(638, 349), new Vector2(624, 408), new Vector2(586, 455),
                new Vector2(530, 477), new Vector2(480, 511), new Vector2(479, 571),
                new Vector2(513, 622), new Vector2(568, 648), new Vector2(628, 644),
                new Vector2(688, 648), new Vector2(748, 647), new Vector2(807, 658),
                new Vector2(864, 679), new Vector2(880, 716)
            };
            _stageLaneCells[1] = new List<Point>
            {
                new Point(7,0), new Point(8,0), new Point(8,1), new Point(9,1),
                new Point(9,2), new Point(10,2), new Point(8,3), new Point(9,3),
                new Point(8,4), new Point(7,4), new Point(7,5), new Point(6,5),
                new Point(6,6), new Point(5,6), new Point(5,7), new Point(6,7),
                new Point(7,7), new Point(8,7), new Point(9,7), new Point(6,8),
                new Point(7,8), new Point(8,8), new Point(9,8), new Point(10,8), new Point(11,8)
            };
            _stageBlockedRoad[1] = new List<Point>
            {
                new Point(0,4), new Point(1,4), new Point(2,4), new Point(3,4), new Point(4,4),
                new Point(2,5), new Point(3,5), new Point(4,5), new Point(5,5),
                new Point(6,0), new Point(7,1), new Point(7,3), new Point(7,6), new Point(8,2), new Point(8,5), new Point(9,0),
                new Point(10,3), new Point(10,4), new Point(11,3), new Point(11,4), new Point(12,3), new Point(12,4),
                new Point(13,3), new Point(13,4), new Point(14,3), new Point(14,4), new Point(15,4), new Point(10,7)
            };
            _stageBlockedScenery[1] = new List<Point>
            {
                new Point(1,2), new Point(2,2), new Point(3,2), new Point(4,2), new Point(4,1), new Point(5,1), new Point(5,2),
                new Point(10,0), new Point(10,1), new Point(11,1), new Point(11,0), new Point(12,0), new Point(13,0), new Point(14,0), new Point(15,0),
                new Point(12,1), new Point(13,1), new Point(14,1), new Point(15,1), new Point(13,2), new Point(14,2), new Point(15,2),
                new Point(0,0), new Point(1,0), new Point(2,0), new Point(3,0), new Point(4,0), new Point(0,1), new Point(1,1), new Point(3,1), new Point(0,2), new Point(0,3),
                new Point(7,2), new Point(6,3), new Point(5,3),
                new Point(10,5), new Point(12,5), new Point(13,5), new Point(10,6), new Point(12,6), new Point(12,7), new Point(8,6), new Point(9,6),
                new Point(14,5), new Point(15,5), new Point(13,6), new Point(14,6), new Point(15,6), new Point(13,7), new Point(14,7), new Point(15,7),
                new Point(13,8), new Point(14,8), new Point(15,8),
                new Point(0,6), new Point(0,7), new Point(0,8), new Point(1,6), new Point(1,7), new Point(1,8), new Point(2,7), new Point(3,8), new Point(4,8)
            };

            //Stage3: LAVA
            _stageWaypoints[2] = new List<Vector2>
            {
                new Vector2(3, 360), new Vector2(57, 333), new Vector2(117, 333),
                new Vector2(177, 334), new Vector2(237, 339), new Vector2(297, 337),
                new Vector2(346, 372), new Vector2(404, 388), new Vector2(464, 392),
                new Vector2(524, 392), new Vector2(584, 387), new Vector2(644, 388),
                new Vector2(703, 401), new Vector2(750, 439), new Vector2(766, 497),
                new Vector2(779, 556), new Vector2(821, 599), new Vector2(881, 608),
                new Vector2(941, 614), new Vector2(1001, 613), new Vector2(1061, 608),
                new Vector2(1115, 636), new Vector2(1156, 680), new Vector2(1175, 716)
            };
            _stageLaneCells[2] = new List<Point>
            {
                new Point(0,4), new Point(1,3), new Point(1,4), new Point(2,3),
                new Point(2,4), new Point(3,3), new Point(3,4), new Point(4,4),
                new Point(5,4), new Point(6,4), new Point(7,4), new Point(7,5),
                new Point(8,4), new Point(8,5), new Point(9,5), new Point(9,6),
                new Point(9,7), new Point(10,7), new Point(11,7), new Point(12,7),
                new Point(13,7), new Point(14,8)
            };
            _stageBlockedRoad[2] = new List<Point>
            {
                new Point(0,3), new Point(10,5), new Point(11,0), new Point(11,1),
                new Point(11,2), new Point(11,3), new Point(11,4), new Point(11,5),
                new Point(13,8), new Point(14,7)
            };
            _stageBlockedScenery[2] = new List<Point>
            {
                new Point(8,0), new Point(8,1), new Point(8,2), new Point(7,2), new Point(7,3), new Point(6,3),
                new Point(5,5), new Point(6,5), new Point(5,6), new Point(6,6), new Point(4,7), new Point(5,7), new Point(6,7), new Point(3,8), new Point(4,8), new Point(5,8),
                new Point(13,0), new Point(14,0), new Point(13,1), new Point(14,1), new Point(13,2), new Point(14,2), new Point(15,2),
                new Point(14,3), new Point(15,3), new Point(13,4), new Point(14,4), new Point(13,5), new Point(14,5), new Point(14,6), new Point(15,6),
                new Point(3,0), new Point(4,0), new Point(3,1), new Point(4,1), new Point(0,0), new Point(1,0), new Point(0,1), new Point(1,1), new Point(0,2),
                new Point(5,1), new Point(6,1), new Point(1,6), new Point(2,6), new Point(0,5), new Point(0,6), new Point(7,7), new Point(8,7), new Point(7,8), new Point(8,8)
            };

            //(1 = Ogre, 2 = Grey Troll)
            _stageSpawnTypes = new List<int>[_totalStages];
            _stageSpawnDelays = new List<float>[_totalStages];

            _stageSpawnTypes[0] = new List<int> { _ogreId, _ogreId, _ogreId, _trollId, _ogreId, _ogreId, _trollId, _ogreId };
            _stageSpawnDelays[0] = new List<float> { 1.5f, 3.2f, 3.0f, 3.2f, 2.8f, 2.6f, 3.0f, 2.5f };

            _stageSpawnTypes[1] = new List<int> { _ogreId, _ogreId, _trollId, _ogreId, _trollId, _ogreId, _trollId, _trollId, _ogreId, _trollId, _ogreId, _trollId };
            _stageSpawnDelays[1] = new List<float> { 1.2f, 2.5f, 2.6f, 2.2f, 2.4f, 2.0f, 2.2f, 2.0f, 1.8f, 1.8f, 1.8f, 1.6f };

            _stageSpawnTypes[2] = new List<int> { _trollId, _ogreId, _trollId, _trollId, _ogreId, _trollId, _trollId, _ogreId, _trollId, _trollId, _ogreId, _trollId, _trollId, _trollId, _trollId, _trollId };
            _stageSpawnDelays[2] = new List<float> { 1.0f, 2.0f, 2.0f, 1.8f, 1.6f, 1.6f, 1.6f, 1.5f, 1.5f, 1.4f, 1.4f, 1.3f, 1.3f, 1.2f, 1.2f, 1.1f };
        }

        public void LoadStage(int stageIndex)
        {
            CurrentStageIndex = Math.Clamp(stageIndex, 0, _totalStages - 1);
            _grid.LoadStageCells(_stageLaneCells[CurrentStageIndex], _stageBlockedRoad[CurrentStageIndex], _stageBlockedScenery[CurrentStageIndex]);

            Defences.Clear();
            Enemies.Clear();
            Projectiles.Clear();
            FloatingTexts.Clear();

            Funds = _stageStartFunds[CurrentStageIndex];
            KilledCount = 0;
            LeakedCount = 0;
            _nextSpawnIndex = 0;
            _spawnTimer = _stageSpawnDelays[CurrentStageIndex][0];
        }

        public int GetDefenceCost(int defenceId)
        {
            if (defenceId == _knightId) return _knightCost;
            if (defenceId == _elfId) return _elfCost;
            return _towerCost;
        }

        public bool TryPlaceDefence(int defenceId, Point cell, out string message)
        {
            if (!_grid.CanPlace(defenceId, cell))
            {
                message = _invalidPlaceMsg;
                return false;
            }

            int cost = GetDefenceCost(defenceId);
            if (Funds < cost)
            {
                message = _notEnoughGoldMsg;
                return false;
            }

            Funds -= cost;
            Vector2 center = _grid.CellCentre(cell);

            // Values will be then passed at object instantiation
            if (defenceId == _knightId)
            {
                Defences.Add(new Knight(
                    cell, center,
                    _knightAttackTex, _knightHurtTex, _knightDieTex,
                    new Rectangle(0, 0, 282, 312),
                    8, 0.09f, 6, 0.07f, 5, 0.12f,
                    240f, 28f, 90f, 1.1f,
                    0.28f, 24f, 0.45f, 2f,
                    44, 6, 1, -48f,
                    new Color(150, 25, 25), Color.LimeGreen
                ));
            }
            else if (defenceId == _elfId)
            {
                Defences.Add(new ElfMage(
                    cell, center,
                    _elfIdleTex, _elfAttackTex, _ballTex,
                    new Rectangle(790, 338, 415, 523), 1, 0.15f,
                    4, 0.11f,
                    100f, 42f, 230f, 0.85f,
                    0.28f, 0.20f, 2f, 0.35f,
                    340f, 0.225f, true, new Vector2(16f, -18f),
                    24f, 26f, _mapWidth, _mapHeight, 40f,
                    44, 6, 1, -48f,
                    new Color(150, 25, 25), Color.LimeGreen
                ));
            }
            else
            {
                Defences.Add(new ArrowTower(
                    cell, center,
                    _towerTex, _arrowTex,
                    new Rectangle(5, 8, 62, 93), 1, 0.2f,
                    100f, 22f, 280f, 1.35f,
                    0.86f, 0.18f, 2f, 0.35f,
                    460f, 2.5f, false, new Vector2(0f, -28f), MathHelper.PiOver2,
                    24f, 26f, _mapWidth, _mapHeight, 40f,
                    44, 6, 1, -48f,
                    new Color(150, 25, 25), Color.LimeGreen
                ));
            }

            _grid.SetOccupied(cell, true);
            message = string.Empty;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Spawn enemies on wave timer
            List<int> spawnTypes = _stageSpawnTypes[CurrentStageIndex];
            List<float> spawnDelays = _stageSpawnDelays[CurrentStageIndex];

            if (_nextSpawnIndex < spawnTypes.Count)
            {
                _spawnTimer -= dt;
                if (_spawnTimer <= 0f)
                {
                    int enemyType = spawnTypes[_nextSpawnIndex];
                    float hMult = _stageHealthMults[CurrentStageIndex];
                    float sMult = _stageSpeedMults[CurrentStageIndex];

                    if (enemyType == _trollId)
                    {
                        Enemies.Add(new GreyTroll(
                            _trollWalkTex, 5, 0.09f,
                            _trollAttackTex, 5, 0.10f,
                            _trollHurtTex, 5, 0.08f,
                            _trollDieTex, 6, 0.11f,
                            CurrentWaypoints,
                            190f * hMult, 72f * sMult, 30f, 1.15f,
                            35, 0.31f, 64f,
                            24f, -18f, 0.45f, 2f, 0.5f,
                            44, 6, 1, -56f,
                            new Color(150, 25, 25), Color.LimeGreen
                        ));
                    }
                    else
                    {
                        Enemies.Add(new Ogre(
                            _ogreWalkTex, 5, 0.12f,
                            _ogreAttackTex, 6, 0.11f,
                            _ogreHurtTex, 4, 0.08f,
                            _ogreDieTex, 6, 0.11f,
                            CurrentWaypoints,
                            135f * hMult, 48f * sMult, 20f, 1.0f,
                            25, 0.30f, 64f,
                            24f, -18f, 0.45f, 2f, 0.5f,
                            44, 6, 1, -56f,
                            new Color(150, 25, 25), Color.LimeGreen
                        ));
                    }

                    _nextSpawnIndex++;
                    if (_nextSpawnIndex < spawnDelays.Count)
                    {
                        _spawnTimer = spawnDelays[_nextSpawnIndex];
                    }
                }
            }

            //Update Defences
            for (int i = Defences.Count - 1; i >= 0; i--)
            {
                Defences[i].Update(gameTime, Enemies, Projectiles);
                if (Defences[i].IsDead)
                {
                    _grid.SetOccupied(Defences[i].Cell, false);
                    Defences.RemoveAt(i);
                }
            }

            //Update Projectiles
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update(gameTime, Enemies);
                if (Projectiles[i].IsDead)
                {
                    Projectiles.RemoveAt(i);
                }
            }

            //Update Enemies & Check Kills/Leaks
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = Enemies[i];
                enemy.Update(gameTime, Defences);

                if (enemy.IsDying && !enemy.RewardGranted)
                {
                    enemy.RewardGranted = true;
                    Funds += enemy.Reward;
                    KilledCount++;
                    FloatingTexts.Add(new FloatingText(
                        $"+{enemy.Reward}",
                        enemy.Position + new Vector2(-14f, -45f),
                        35f,
                        1.1f,
                        new Vector2(1f, 1f)
                    ));
                }

                if (enemy.IsDead)
                {
                    if (enemy.ReachedExit)
                    {
                        LeakedCount++;
                    }
                    Enemies.RemoveAt(i);
                }
            }

            //Update Floating Gold Texts
            for (int i = FloatingTexts.Count - 1; i >= 0; i--)
            {
                FloatingTexts[i].Update(gameTime);
                if (FloatingTexts[i].IsExpired)
                {
                    FloatingTexts.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font)
        {
            Texture2D bg = _stageBackgrounds[CurrentStageIndex];
            spriteBatch.Draw(bg, new Rectangle(0, 0, _mapWidth, _mapHeight), Color.White);

            foreach (var def in Defences) def.Draw(spriteBatch, pixel);
            foreach (var enemy in Enemies) enemy.Draw(spriteBatch, pixel);
            foreach (var proj in Projectiles) proj.Draw(spriteBatch);
            foreach (var ft in FloatingTexts) ft.Draw(spriteBatch, font);
        }
    }
}
