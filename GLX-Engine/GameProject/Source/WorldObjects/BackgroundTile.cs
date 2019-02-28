using GLXEngine;

namespace GameProject
{
    class BackgroundTile : GameObject
    {
         Sprite m_sprite;

        public BackgroundTile(Scene a_scene, Sprite a_sprite) : base(a_scene)
        {
            m_sprite = a_sprite;
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);
        }
    }
}
