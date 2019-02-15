using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    class Enemy : Boid
    {
        Player m_player;

        Sprite m_sprite = new Sprite("Textures/enemy.png");

        Hp m_hp;
        EasyDraw m_canvas;

        Sound m_crashSound;

        Gun m_gun;

        public Enemy(Scene a_scene, Player a_player, ref List<GameObject> a_enemies, EasyDraw a_canvas) : base(a_scene, ref a_enemies, a_canvas)
        {
            m_gun = new Gun(a_scene, ReloadStyle.COMPLETE_CLIP, this);
            m_gun.SetActive(true);
            AddChild(m_gun);

            m_player = a_player;
            m_hp = new Hp();

            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            m_sprite.y -= 10;
            m_sprite.rotation = 45;
            AddChild(m_sprite);

            m_crashSound = new Sound("Audio/crash.wav");

            m_canvas = a_canvas;
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite, new System.Type[] { GetType() });
        }

        public void OnCollision(GameObject other)
        {
            if (other is Bullet)
            {
                if (((Bullet)other).m_owner.GetType().Equals(typeof(Player)))
                {
                    other.Destroy();
                    m_hp.current -= 20f;
                }
            }
        }

        protected override void OnDestroy()
        {
            ((Overworld)m_scene).enemies.Remove(this);
            m_crashSound.Play();
            base.OnDestroy();
        }

        public void Update(float a_dt)
        {
            if (m_hp.current <= 0)
            {
                ((Overworld)m_scene).score += 1;
                Destroy();
            }

            Flock(50, 0, 400);
            if (m_player.m_hp.current > 0)
                Flock(new List<GameObject> { m_player }, 300, 0, 900);

            m_force += Seperate(((Overworld)m_scene).bullets.FindAll(bullet => { return ((Bullet)bullet).m_owner.GetType().Equals(typeof(Player));}), 200.0f) * 100f;

            //m_canvas.Stroke(System.Drawing.Color.White);
            //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_force.x * 10, screenPosition.y + m_force.y * 10);

            m_velocity += m_force;
            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity.magnitude = m_maxSpeed;

            rotation = (m_player.position - position).angle;

            position += m_velocity * a_dt;

            m_force *= 0;

            if (Utils.Random(0, 1000) <= a_dt)
            {
                m_gun.Shoot();
            }
        }

        protected override void RenderSelf(GLContext glContext)
        {
            float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
            Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);

            m_canvas.Stroke(System.Drawing.Color.FromArgb(color.x, color.y, 0));
            m_canvas.StrokeWeight(5);
            m_canvas.Line(screenPosition.x - m_hp.current / 2, screenPosition.y + m_sprite.height / 2, screenPosition.x + m_hp.current / 2, screenPosition.y + m_sprite.height / 2);
        }


    }
}
