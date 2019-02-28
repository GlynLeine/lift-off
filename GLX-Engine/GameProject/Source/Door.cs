using GLXEngine;

namespace GameProject
{
    class Door : WallTile
    {
        static int count = 0;
        Player m_player;
        int m_ID;

        public Door(Scene a_scene, Sprite a_sprite, Player a_player, int a_ID = -1) : base(a_scene, a_sprite)
        {
            m_player = a_player;

            if (a_ID >= 0)
                m_ID = a_ID;
            else
            {
                m_ID = count;
                count++;
            }
        }

        public void Update(float a_dt)
        {
            if (m_player.m_tags.Contains("KEY" + m_ID))
                Destroy();
        }
    }
}
