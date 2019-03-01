using GLXEngine.Core;
using GLXEngine;


namespace GameProject
{
    class Bullet : GameObject
    {
        Sprite m_sprite = new Sprite("Textures/bullet.png");

        public GameObject m_owner;
        public GameObject m_player;

        public float m_damage;

        public Bullet(Scene a_scene, GameObject a_shooter, GameObject a_player, float a_damage = 10) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);

            m_owner = a_shooter;
            m_player = a_player;
            m_damage = a_damage;
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);
        }

        public void OnCollision(GameObject other, Vector2 a_mtv)
        {
            if (other is WallTile)
            {
                Destroy();
            }
        }

        protected override void OnDestroy()
        {
            ((Overworld)m_scene).bullets.Remove(this);
            base.OnDestroy();
        }

        public void Update(float a_dt)
        {
            if (screenPosition.x < -game.width * 0.5f || screenPosition.x > game.width * 1.5f || screenPosition.y < -game.height * 0.5f || screenPosition.y > game.height * 1.5f)
            {
                Destroy();
            }

            position += m_velocity * a_dt + (m_velocity.normal.Dot(m_player.m_velocity * a_dt) * m_velocity.normal);
            rotation = m_velocity.angle;
        }
    }
}
