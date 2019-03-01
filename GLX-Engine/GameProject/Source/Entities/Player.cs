using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    public class Player : GameObject
    {
        const float m_speed = 600;
        const float m_angularAcceleration = 5f;

        public List<string> m_tags = new List<string>();

        Vector2 m_direction = new Vector2();

        Vector2 m_dodgeForce = new Vector2();
        Vector2 m_movementForce = new Vector2();

        float m_dodgeTimeBuffer = 0;
        float m_dodgeCooldownTime = 1f;
        bool m_dodgeCooldown = false;

        float m_animTimeBuffer = 0;
        float m_animFrameTime = 0.15f;
        AnimationSprite m_sprite = new AnimationSprite("Textures/playerAnim.png", 6, 1);

        public Hp m_hp = new Hp();
        EasyDraw m_canvas;

        Sound m_deathSound;
        SoundChannel m_deathSoundChannel;

        List<Gun> m_guns;

        int m_currentGun = 0;

        public Player(Scene a_scene, EasyDraw a_canvas) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            m_sprite.x -= 10;
            m_sprite.y -= 12;
            m_sprite.rotation = 50;
            AddChild(m_sprite);

            m_guns = new List<Gun> { new TommyGun(a_scene, this, this), new ShotGun(a_scene, this, this), new Carbine(a_scene, this, this) };
            foreach (Gun gun in m_guns)
            {
                gun.y += 12;
                gun.x += 15;
                AddChild(gun);
            }
            m_guns[0].SetActive(true);

            m_canvas = a_canvas;
            //(collider as BoxCollider).m_canvas = a_canvas;

            m_deathSound = new Sound("Audio/death.wav");

        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);//, ref m_canvas);
        }

        public void MoveForward(float a_value, List<int> a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            if (m_movementForce.sqrMagnitude > 0) m_movementForce.Normalize();
            m_movementForce.y -= a_value;
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.magnitude = m_speed;
        }

        public void MoveRight(float a_value, List<int> a_controllerIDs)
        {
            if (m_hp.current <= 0)
                return;

            if (m_movementForce.sqrMagnitude > 0) m_movementForce.Normalize();
            m_movementForce.x += a_value;
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.magnitude = m_speed;
        }

        public void FaceForward(float a_value, List<int> a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            m_direction.y -= a_value;
        }

        public void FaceRight(float a_value, List<int> a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            m_direction.x += a_value;
        }

        public void Shoot(bool a_pressed, int a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            m_guns[m_currentGun].Shoot(a_pressed);
        }

        public void Reload(bool a_pressed, int a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            if (!a_pressed && !m_guns[m_currentGun].IsReloading)
                m_guns[m_currentGun].Reload();
        }

        public void Dodge(bool a_pressed, int a_controllerID)
        {
            if (!a_pressed && !m_dodgeCooldown)
            {
                m_dodgeCooldown = true;
                m_dodgeTimeBuffer = 0;
                Vector2 fwd = new Vector2(rotation);
                m_dodgeForce -= fwd * 800;
            }
        }

        public void SwitchWeapon(bool a_pressed, int a_controllerID)
        {
            if (m_hp.current <= 0)
                return;

            if (!a_pressed)
            {
                m_guns[m_currentGun].SetActive(false);
                m_currentGun = (m_currentGun + 1) % m_guns.Count;
                m_guns[m_currentGun].SetActive(true);
                System.Console.WriteLine("switched");
            }

        }

        public void OnCollision(GameObject other, Vector2 a_mtv)
        {
            if (m_hp.current <= 0)
                return;

            if (other is Bullet)
            {
                if (((Bullet)other).m_owner.GetType().Equals(typeof(Enemy)))
                {
                    other.Destroy();
                    m_hp.current -= 5f;
                }
            }
            else if (other is PickUp)
            {
                other.Destroy();

                if (!m_tags.Contains(other.name))
                    m_tags.Add(other.name);
            }
            else if (!HasChild(other))
            {
                position += a_mtv;
            }
        }

        void Update(float a_dt)
        {
            if (m_hp.current <= 0)
            {
                if (m_deathSoundChannel == null)
                {
                    m_deathSoundChannel = m_deathSound.Play();
                    m_sprite.Destroy();
                }

                if (!m_deathSoundChannel.IsPlaying)
                    game.End();
            }

            m_dodgeTimeBuffer += a_dt;
            if(m_dodgeTimeBuffer > m_dodgeCooldownTime)
            {
                m_dodgeCooldown = false;
                m_dodgeTimeBuffer = m_dodgeCooldownTime;
            }

            if (m_direction.sqrMagnitude > 0)
                rotation = m_direction.angle;
            m_direction *= 0;

            m_velocity = m_dodgeForce + m_movementForce;

            position += m_velocity * a_dt;
            m_dodgeForce *= 0.9f;

            if (m_hp.current > 0)
                m_movementForce *= 0;

            m_animTimeBuffer += a_dt;
            if (m_animTimeBuffer >= m_animFrameTime)
            {
                m_animTimeBuffer -= m_animFrameTime;
                m_sprite.NextFrame();
            }
        }

        protected override void RenderSelf(GLContext glContext)
        {
            if (m_hp.current > 0)
            {
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

                percentage = m_guns[m_currentGun].Clip / (float)m_guns[m_currentGun].ClipSize * m_hp.max;
                m_canvas.Stroke(System.Drawing.Color.Aqua);
                m_canvas.StrokeWeight(5);
                m_canvas.Line(screenPosition.x - percentage / 2, screenPosition.y + m_sprite.height / 2 + 8, screenPosition.x + percentage / 2, screenPosition.y + m_sprite.height / 2 + 8);
            }
        }
    }
}
