using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    class Enemy : GameObject
    {
        Vector2 m_force;

        const uint m_flockRange = 10;

        float m_maxSpeed = 200f;
        float m_maxForce = 5f;
        List<GameObject> m_enemies;
        Player m_player;

        float m_animationTimeBuffer = 0;
        float m_animationSpeed = 1f / 5f;
        AnimationSprite m_animationSprite = new AnimationSprite("Textures/galaxian.png", 2, 1);

        Hp m_hp;
        EasyDraw m_canvas;

        Sound m_crashSound;

        public Enemy(Scene a_scene, Player a_player, ref List<GameObject> a_enemies, EasyDraw a_canvas) : base(a_scene)
        {
            m_player = a_player;
            m_hp = new Hp();
            m_enemies = a_enemies;

            m_animationSprite.SetOrigin(m_animationSprite.width / 2, m_animationSprite.height / 2);
            AddChild(m_animationSprite);

            m_crashSound = new Sound("Audio/crash.wav");

            m_canvas = a_canvas;
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_animationSprite, new System.Type[] { GetType() });
        }

        public void OnCollision(GameObject other)
        {
            if (other is Bullet)
            {
                other.Destroy();
                m_hp.current -= 20f;
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

            Flock();
            if (m_player.m_hp.current > 0)
                m_force += Seek(m_player.position) * 1.5f;
            m_force += Seperate(((Overworld)m_scene).bullets) * 10f;

            //m_canvas.Stroke(System.Drawing.Color.White);
            //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_force.x * 10, screenPosition.y + m_force.y * 10);

            m_velocity += m_force;
            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity.magnitude = m_maxSpeed;

            rotation = m_velocity.angle;

            position += m_velocity * a_dt;

            m_force *= 0;

            m_animationTimeBuffer += a_dt;
            while (m_animationTimeBuffer >= m_animationSpeed)
            {
                m_animationTimeBuffer -= m_animationSpeed;
                m_animationSprite.NextFrame();
            }
        }

        protected override void RenderSelf(GLContext glContext)
        {
            float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
            Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);

            m_canvas.Stroke(System.Drawing.Color.FromArgb(color.x, color.y, 0));
            m_canvas.StrokeWeight(5);
            m_canvas.Line(screenPosition.x - m_hp.current / 2, screenPosition.y + m_animationSprite.height / 2, screenPosition.x + m_hp.current / 2, screenPosition.y + m_animationSprite.height / 2);
        }

        void Flock()
        {
            Vector2 seperation = Seperate(m_enemies);
            Vector2 alignment = Align();
            Vector2 cohesian = GetCohesion();

            seperation *= 2f;

            m_force += seperation + alignment + cohesian;
        }

        Vector2 Seek(Vector2 target)
        {
            Vector2 desired = target - position;
            desired.magnitude = m_maxSpeed;
            Vector2 steer = desired - m_velocity;
            if (steer.magnitude > m_maxForce)
            {
                steer.magnitude = m_maxForce;
            }

            //m_canvas.Stroke(System.Drawing.Color.Yellow);
            //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 10, screenPosition.y + steer.y * 10);
            return steer;
        }

        Vector2 GetCohesion()
        {
            float neighbourDistance = 400;
            Vector2 sum = new Vector2();

            int count = 0;
            foreach (Enemy other in m_enemies)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < neighbourDistance)
                {
                    sum += other.position;
                    count++;
                }
            }

            if (count > 0)
            {
                sum /= count;
                Vector2 cohesian = Seek(sum);
                //m_canvas.Stroke(System.Drawing.Color.Blue);
                //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + cohesian.x * 10, screenPosition.y + cohesian.y * 10);
                return cohesian;
            }
            else
            {
                return new Vector2();
            }
        }

        Vector2 Seperate(List<GameObject> objects)
        {
            float desiredSeparation = 150.0f;
            Vector2 steer = new Vector2();
            int count = 0;

            foreach (GameObject other in objects)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < desiredSeparation)
                {
                    Vector2 diff = (position - other.position).normal;
                    diff /= distance;
                    steer += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                steer /= count;
            }

            if (steer.magnitude > 0)
            {
                steer.magnitude = m_maxSpeed;
                steer -= m_velocity;
                if (steer.magnitude > m_maxForce)
                {
                    steer.magnitude = m_maxForce;
                }
            }

            //m_canvas.Stroke(System.Drawing.Color.Red);
            //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 15, screenPosition.y + steer.y * 15);
            return steer;
        }

        Vector2 Align()
        {
            float neighbourDistance = 200;
            Vector2 sum = new Vector2();

            int count = 0;
            foreach (Enemy other in m_enemies)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < neighbourDistance)
                {
                    sum += other.m_velocity;
                    count++;
                }
            }

            if (count > 0)
            {
                sum /= count;
                sum.magnitude = m_maxSpeed;
                Vector2 steer = sum - m_velocity;
                if (steer.magnitude > m_maxForce)
                {
                    steer.magnitude = m_maxForce;
                }

                //m_canvas.Stroke(System.Drawing.Color.Green);
                //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 10, screenPosition.y + steer.y * 10);

                return steer;
            }
            else
            {
                return new Vector2();
            }
        }
    }
}
