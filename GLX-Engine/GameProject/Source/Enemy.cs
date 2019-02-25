using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    class Enemy : Boid
    {
        Player m_player;

        float m_animTimeBuffer = 0;
        float m_animFrameTime = 0.15f;
        AnimationSprite m_sprite = new AnimationSprite("Textures/enemyAnim.png", 5, 1);

        Hp m_hp;
        EasyDraw m_canvas;

        Sound m_crashSound;

        float m_reactionScalar = Utils.Random(0.2f, 0.5f);

        Gun m_gun;

        public Enemy(Scene a_scene, Player a_player, ref List<GameObject> a_enemies, EasyDraw a_canvas) : base(a_scene, ref a_enemies, a_canvas)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            m_sprite.x -= 10;
            m_sprite.y -= 12;
            m_sprite.rotation = 45;
            AddChild(m_sprite);

            m_gun = new Gun(a_scene, ReloadStyle.COMPLETE_CLIP, this, a_player);
            m_gun.SetActive(true);
            AddChild(m_gun);

            m_player = a_player;
            m_hp = new Hp();

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

            Behaviour();

            Dodge();

            Move(a_dt);

            m_animTimeBuffer += a_dt;
            if (m_animTimeBuffer >= m_animFrameTime)
            {
                m_animTimeBuffer -= m_animFrameTime;
                m_sprite.NextFrame();
            }
        }

        private void OffScreenCheck()
        {
            if (!game.InView(m_sprite.GetExtents()))
            {
                Vector2 screenEdge = screenPosition;
                screenEdge.x = Mathf.Clamp(screenEdge.x, 0, game.width);
                screenEdge.y = Mathf.Clamp(screenEdge.y, 0, game.height);

                Vector2 direction = (screenPosition - screenEdge).normal;
                direction.angle += 90;

                Vector2 tip = new Vector2(screenEdge.x,
                                          screenEdge.y);

                Vector2 left = new Vector2(screenEdge.x - (10f * direction.x) - (10f * direction.y),
                                           screenEdge.y + (10f * direction.x) - (10f * direction.y));

                Vector2 right = new Vector2(screenEdge.x + (10f * direction.x) - (10f * direction.y),
                                            screenEdge.y + (10f * direction.x) + (10f * direction.y));

                m_canvas.Stroke(0);
                m_canvas.StrokeWeight(1);
                m_canvas.Fill(255, 0, 0);
                m_canvas.Triangle(tip.x, tip.y, left.x, left.y, right.x, right.y);

                float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
                Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);


                left.x = screenEdge.x - (m_hp.current / 2 * direction.x) - (m_sprite.height / 2 * direction.y);
                left.y = screenEdge.y + (m_sprite.height / 2 * direction.x) + (m_hp.current / 2 * direction.y);

                right.x = screenEdge.x + (m_hp.current / 2 * direction.x) - (m_sprite.height / 2 * direction.y);
                right.y = screenEdge.y + (m_sprite.height / 2 * direction.x) - (m_hp.current / 2 * direction.y);

                Vector2 bottomLeft = new Vector2(left.x - (5f * direction.y), left.y + (5f * direction.x));
                Vector2 bottomRight = new Vector2(right.x - (5f * direction.y), right.y + (5f * direction.x));

                m_canvas.Fill(color.x, color.y, 0);
                m_canvas.Quad(left.x, left.y, right.x, right.y, bottomRight.x, bottomRight.y, bottomLeft.x, bottomLeft.y);

                m_maxSpeed = 200;
            }
            else
            {
                m_maxSpeed = 500;
            }
        }

        private void Move(float a_dt)
        {
            m_velocity += m_force;

            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity.magnitude = m_maxSpeed;

            position += m_velocity * a_dt;

            m_force *= 0;
        }

        private void Behaviour()
        {
            if (HasChild(m_gun))
            {
                Flock(100, 0, 400);
            }

            if (m_player.m_hp.current > 0)
            {
                Vector2 screenEdge = screenPosition + ((screenPosition - m_player.screenPosition).normal * game.width);
                screenEdge.x = Mathf.Clamp(screenEdge.x, 0, game.width);
                screenEdge.y = Mathf.Clamp(screenEdge.y, 0, game.height);

                float screenDistance = (screenEdge - m_player.screenPosition).magnitude;

                if (HasChild(m_gun))
                {
                    Flock(new List<GameObject> { m_player }, screenDistance * 0.65f, 0, screenDistance);

                    if (Utils.Random(0, 500) <= 1 && Vector2.Distance(position, m_player.position) < 900)
                    {
                        m_gun.Shoot();
                    }

                    rotation = (m_player.position - position).angle;
                }
                else
                {
                    Flock(new List<GameObject> { m_player }, screenDistance * 1.1f, 0, screenDistance * 1.2f);
                    rotation = (position - m_player.position).angle;
                }
            }
        }

        private void Dodge()
        {

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
                if (projectedDistanceX < (200f * m_reactionScalar) && projectedDistanceX > 0 && projectedDistanceY < 0 && projectedDistanceY > -50)
                    m_force += strafeDirection * m_maxForce * 10;
            });
        }

        protected override void RenderSelf(GLContext glContext)
        {
            OffScreenCheck();

            float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
            Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);

            m_canvas.Stroke(0);
            m_canvas.Fill(color.x, color.y, 0);
            m_canvas.StrokeWeight(1);

            Vector2 left = new Vector2();
            Vector2 right = new Vector2();
            left.x = screenPosition.x - (m_hp.current / 2);
            left.y = screenPosition.y + (m_sprite.height / 2);

            right.x = screenPosition.x + (m_hp.current / 2);
            right.y = screenPosition.y + (m_sprite.height / 2);

            Vector2 bottomLeft = new Vector2(left.x, left.y + 5f);
            Vector2 bottomRight = new Vector2(right.x, right.y + 5f);

            m_canvas.Quad(left.x, left.y, right.x, right.y, bottomRight.x, bottomRight.y, bottomLeft.x, bottomLeft.y);
        }


    }
}
