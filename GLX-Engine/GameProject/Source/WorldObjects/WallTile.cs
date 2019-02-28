using GLXEngine;
using GLXEngine.Core;


namespace GameProject
{
    class WallTile : BoundsObject
    {
        Sprite m_sprite;

        public WallTile(Scene a_scene, Sprite a_sprite) : base(a_scene, a_sprite.width, a_sprite.height)
        {
            m_sprite = a_sprite;
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);
            SetOrigin(width / 2, height / 2);
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }

        public void OnCollision(GameObject other, Vector2 a_mtv)
        {
            if (other is Bullet)
            {
                other.Destroy();
            }
        }

    }
}
