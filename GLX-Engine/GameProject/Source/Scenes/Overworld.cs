using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using System.Drawing;


namespace GameProject
{
    class Overworld : Scene
    {
        public Player player;
        public List<GameObject> enemies;
        public List<GameObject> bullets;
        EasyDraw UI;
        Sound backgroundMusic;
        SoundChannel backgroundMusicChannel;

        private uint enemyCount = 5;

        public float difficulty = 0;

        Timer frameRateCounter;
        float avgFrameTime = 1;
        float avgFrameRate = 0;

        public Overworld() : base()
        {

        }

        public override void Start()
        {
            base.Start();

            frameRateCounter = new Timer(0.1f,
                () =>
                {
                    avgFrameRate = 1 / avgFrameTime;
                },
                () =>
                {
                    avgFrameTime += Time.deltaTime;
                    avgFrameTime /= 2f;
                });
            frameRateCounter.m_timeBuffer = frameRateCounter.m_timeTrigger;

            SetXY(game.width / 2, game.height / 2);


            UI = new EasyDraw(Game.main.width, Game.main.height);
            UI.autoClear = true;
            UI.SetOrigin(UI.width / 2, UI.height / 2);
            UI.TextFont(new Font("Eight-Bit Madness", 100));
            UI.TextAlign(CenterMode.Center, CenterMode.Center);
            UI.TextSize(1);

            player = new Player(this, UI);
            m_player = player;
            TiledSceneContructor.LoadObjects(this, "Maps/level.tmx");

            AddChild(player);

            bullets = new List<GameObject>();

            enemies = new List<GameObject>();
            while (enemies.Count < enemyCount)
            {
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left : game.RenderRange.right, Utils.Random(0, 2) == 0 ? game.RenderRange.top : game.RenderRange.bottom);
                if (Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
                if (!(pos.x < screenPosition.x + 128 || pos.y < screenPosition.y + 128 || pos.x > screenPosition.x + width - 128 || pos.y > screenPosition.y + height - 128))
                {
                    Enemy enemy = new Enemy(this, player, ref enemies, UI, Utils.Random(0, 0.2f));

                    enemies.Add(enemy);
                    AddChild(enemy);
                    enemy.screenPosition = pos;
                }
            }

            backgroundMusic = new Sound("Audio/game_loop.wav", true);
            backgroundMusicChannel = backgroundMusic.Play();

            AddChild(UI);

            System.Console.WriteLine(GetDiagnostics());
        }

        public void Update(float a_dt)
        {
            if (!m_active)
                return;

            difficulty = Mathf.Min(100f, difficulty + a_dt / 3f);

            position = -player.position + (new Vector2(Game.main.width, Game.main.height) * 0.5f);
            UI.position = player.position;

            while (enemies.Count < Mathf.Floor(enemyCount + (difficulty / 3f)))
            {
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left : game.RenderRange.right, Utils.Random(0, 2) == 0 ? game.RenderRange.top : game.RenderRange.bottom);
                if (Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
                if (!(pos.x < screenPosition.x + 128 || pos.y < screenPosition.y + 128 || pos.x > screenPosition.x + width - 128 || pos.y > screenPosition.y + height - 128))
                {
                    Enemy enemy = new Enemy(this, player, ref enemies, UI, Utils.Random(0 + difficulty / 500f, 0.2f + difficulty / 200f));

                    enemies.Add(enemy);
                    AddChild(enemy);
                    enemy.screenPosition = pos;
                }
            }
        }

        protected override void OnDestroy()
        {
            if (backgroundMusicChannel != null)
                backgroundMusicChannel.Stop();
            base.OnDestroy();
        }

        public override void End()
        {
            base.End();
            backgroundMusicChannel.Stop();
        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);

            UI.Fill(0, 255, 05);
            UI.TextSize(30);

            string scorePrefix = "Score:";
            Vector2 prefixDim = new Vector2(UI.TextWidth(scorePrefix), UI.TextHeight(scorePrefix));
            UI.Text(scorePrefix, prefixDim.x / 2, prefixDim.y / 2);

            string scoreText = (game as Program).score.ToString();
            UI.Text(scoreText, prefixDim.x + UI.TextWidth(scoreText) / 2 - 10, prefixDim.y / 2);

            //UI.Fill(0);
            //UI.Text(Mathf.Round(avgFrameRate).ToString(), 30, UI.height - 30);
        }
    }
}
