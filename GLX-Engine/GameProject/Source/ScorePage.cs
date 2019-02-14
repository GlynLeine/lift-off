using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    class ScorePage : Scene
    {
        Dictionary<string, int> scoreList;
        EasyDraw backGround;
        bool entry = true;


        public ScorePage() : base()
        {
            
        }

        public override void Start()
        {
            scoreList = new Dictionary<string, int>();
            backGround = new EasyDraw(Game.main.width, Game.main.height);
        }

        public void Update(float a_dt)
        {
            if (entry)
            {

            }
            else
            {
                if(GLContext.GetKey(Key.ENTER) || GLContext.GetKey(Key.SPACE))
                    ((Program)game).Restart();

            }
        }

        public override void Render(GLContext glContext)
        {
            base.Render(glContext);
            if (entry)
            {

            }
            else
            {
                backGround.Fill(255);
                int i = 0;
                foreach (KeyValuePair<string, int> score in scoreList)
                {
                    backGround.Text(score.Key + ": " + score.Value.ToString(), game.width / 2, 50 + i * 20);
                    i++;
                }
            }
        }

    }
}
