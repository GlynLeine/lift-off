using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;


namespace GameProject
{
    class Overworld : Scene
    {
        public Player player;
        public List<GameObject> enemies;
        public List<GameObject> bullets;
        EasyDraw backGround;
        Sound backgroundMusic;
        SoundChannel backgroundMusicChannel;

        readonly uint enemyCount = (uint)((Game.main.width * Game.main.height)/48000f);

        public int score = 0;


        public Overworld() : base()
        {
            Start();
        }

        public override void Start()
        {
            base.Start();

            SetXY(game.width/2, game.height/2);
            backGround = new EasyDraw(Game.main.width, Game.main.height);
            backGround.autoClear = true;
            backGround.SetOrigin(backGround.width/2, backGround.height/2);
            AddChild(backGround);

            player = new Player(this, backGround);
            AddChild(player);
            bullets = new List<GameObject>();

            enemies = new List<GameObject>();
            for(int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = new Enemy(this, player, ref enemies, backGround);
                enemies.Add(enemy);
                AddChild(enemy);
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left : game.RenderRange.right, Utils.Random(0, 2) == 0 ? game.RenderRange.top : game.RenderRange.bottom);
                if(Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else    pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
                enemy.screenPosition = pos;
            }

            backgroundMusic = new Sound("Audio/music.wav", true);
            backgroundMusicChannel = backgroundMusic.Play();
        }

        public void Update(float a_dt)
        {
            position = -player.position + (new Vector2(Game.main.width, Game.main.height)*0.5f);
            backGround.position = player.position;

            while(enemies.Count < enemyCount)
            {
                Enemy enemy = new Enemy(this, player, ref enemies, backGround);
                enemies.Add(enemy);
                AddChild(enemy);
                Vector2 pos = new Vector2(Utils.Random(0, 2) == 0 ? game.RenderRange.left - 50 : game.RenderRange.right + 50, Utils.Random(0, 2) == 0 ? game.RenderRange.top - 50 : game.RenderRange.bottom + 50);
                if(Utils.Random(0, 2) == 0) pos.x = Utils.Random(game.RenderRange.left, game.RenderRange.right);
                else    pos.y = Utils.Random(game.RenderRange.top, game.RenderRange.bottom);
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
            backGround.Fill(0, 255, 05);
            backGround.Text("Score: " + score.ToString(), 50, 50);
        }
    }
}
