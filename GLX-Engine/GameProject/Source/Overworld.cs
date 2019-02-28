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

        readonly uint enemyCount = (uint)((Game.main.width * Game.main.height) / 48000f);

        Timer frameRateCounter;
        float avgFrameTime = 1;
        float avgFrameRate = 0;

        public Overworld() : base()
        {
            Start();
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
                    avgFrameTime += Time.deltaTime / 1000f;
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
            AddChild(player);

            Sprite wallSprite = new Sprite("Textures/gun.png");
            WallTile wall = new WallTile(this, wallSprite);
            wall.rotation = 90;
            AddChild(wall);

            wall = new WallTile(this, new Sprite("Textures/gun.png"));
            wall.y = wallSprite.width;
            wall.rotation = 90;
            AddChild(wall);

            wall = new WallTile(this, new Sprite("Textures/gun.png"));
            wall.y = wallSprite.width*2;
            wall.rotation = 90;
            AddChild(wall);

            PickUp pickUp = new PickUp(this, "Textures/gun.png", "GUN");
            pickUp.x = 200;
            AddChild(pickUp);

            pickUp = new PickUp(this, "Textures/gun.png", "KEY0");
            pickUp.x = 200;
            pickUp.y = 100;
            AddChild(pickUp);

            Door door = new Door(this, new Sprite("Textures/gun.png"), player);
            door.x = 300;
            AddChild(door);


            bullets = new List<GameObject>();

            enemies = new List<GameObject>();
            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = new Enemy(this, player, ref enemies, UI);
                enemies.Add(enemy);
                AddChild(enemy);
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left : game.RenderRange.right, Utils.Random(0, 2) == 0 ? game.RenderRange.top : game.RenderRange.bottom);
                if (Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
                enemy.screenPosition = pos;
            }

            backgroundMusic = new Sound("Audio/game_loop.wav", true);
            backgroundMusicChannel = backgroundMusic.Play();

            AddChild(UI);

            System.Console.WriteLine(GetDiagnostics());
        }

        public void Update(float a_dt)
        {
            position = -player.position + (new Vector2(Game.main.width, Game.main.height) * 0.5f);
            UI.position = player.position;

            while (enemies.Count < enemyCount)
            {
                Enemy enemy = new Enemy(this, player, ref enemies, UI);
                enemies.Add(enemy);
                AddChild(enemy);
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left - 50 : game.RenderRange.right + 50, Utils.Random(0, 2) == 0 ? game.RenderRange.top - 50 : game.RenderRange.bottom + 50);
                if (Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
                enemy.screenPosition = pos;
            }
        }

        protected override void OnDestroy()
        {
            backgroundMusicChannel.Stop();
            base.OnDestroy();
        }

        public override void Render(GLContext glContext)
        {
            base.Render(glContext);

            UI.Fill(0, 255, 05);
            UI.TextSize(30);

            string scorePrefix = "Score:";
            Vector2 prefixDim = new Vector2(UI.TextWidth(scorePrefix), UI.TextHeight(scorePrefix));
            UI.Text(scorePrefix, prefixDim.x / 2, prefixDim.y / 2);

            string scoreText = (game as Program).score.ToString();
            UI.Text(scoreText, prefixDim.x + UI.TextWidth(scoreText) / 2 - 10, prefixDim.y / 2);

            UI.Fill(0);
            UI.Text(Mathf.Round(avgFrameRate).ToString(), 30, UI.height - 30);
        }
    }
}
