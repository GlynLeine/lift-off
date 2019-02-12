using GLXEngine.Core;
using GLXEngine;


namespace GameProject
{
    class Bullet : GameObject
    {
        Sprite m_sprite = new Sprite("Textures/verticalline.png");

        public Bullet(Scene a_scene) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);
        }
        protected override void OnDestroy()
        {
            ((Overworld)m_scene).bullets.Remove(this);
            base.OnDestroy();
        }

        public void Update(float a_dt)
        {
            if(!game.InView(m_sprite.GetExtents()))
            {
                Destroy();
            }

            position += m_velocity * a_dt;
            rotation = m_velocity.angle;
        }
    }
}
