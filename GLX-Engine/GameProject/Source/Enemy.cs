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

            Flock(100, 0, 400);

            if (m_player.m_hp.current > 0)
            {
                Flock(new List<GameObject> { m_player }, 500, 0, 900);

                if (Utils.Random(0, 500) <= 1 && Vector2.Distance(position, m_player.position) < 900)
                {
                    m_gun.Shoot();
                }

                rotation = (m_player.position - position).angle;

            }

            List<GameObject> bullets = ((Overworld)m_scene).bullets.FindAll(a_bullet => { return ((Bullet)a_bullet).m_owner.GetType().Equals(typeof(Player)); });
            bullets = bullets.FindAll(a_bullet =>
            {
                Bullet bullet = (Bullet)a_bullet;
                float angle = Mathf.Abs(bullet.m_velocity.angle - (position - bullet.position).angle);
                return angle < 90 || (angle < 360 && angle > 270);
            });

            bullets.ForEach(a_bullet =>
            {
                Bullet bullet = (Bullet)a_bullet;
                Vector2 bulletDirection = bullet.m_velocity.normal;
                Vector2 bulletToMe = (position - bullet.position);

                //m_canvas.Stroke(System.Drawing.Color.White);
                //m_canvas.Line(bullet.screenPosition.x, bullet.screenPosition.y, bullet.screenPosition.x + bulletToMe.x * 1000, bullet.screenPosition.y + bulletToMe.y * 1000);

                Vector2 strafeDirection = new Vector2(-bulletDirection.y, bulletDirection.x).normal;
                float theta = Mathf.Abs(strafeDirection.angle - bulletToMe.angle);
                if (!(theta < 90 || (theta < 360 && theta > 270)))
                    strafeDirection *= -1;

                //m_canvas.Stroke(System.Drawing.Color.FromArgb(0, 0, 255));
                //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + strafeDirection.x * 100, screenPosition.y + strafeDirection.y * 100);

                float projectedDistanceX = bulletDirection.Dot(bulletToMe);
                float projectedDistanceY = strafeDirection.Dot(bullet.position - position);
                if(projectedDistanceX < 200 && projectedDistanceX > 0 && projectedDistanceY < 0 && projectedDistanceY > -50)
                    m_force += strafeDirection*m_maxForce*10;
            });

            m_velocity += m_force;
            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity.magnitude = m_maxSpeed;

            position += m_velocity * a_dt;

            m_force *= 0;
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
