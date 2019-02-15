using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    public class Player : GameObject
    {
        const float m_speed = 600f;
        const float m_angularAcceleration = 5f;

        Vector2 m_direction = new Vector2();

        Vector2 m_dodgeForce = new Vector2();
        Vector2 m_movementForce = new Vector2();

        Sprite m_sprite = new Sprite("Textures/player.png");

        public Hp m_hp = new Hp();
        EasyDraw m_canvas;

        Sound m_deathSound;
        SoundChannel m_deathSoundChannel;

        List<Gun> m_guns;
        int m_currentGun = 0;

        public Player(Scene a_scene, EasyDraw a_canvas) : base(a_scene)
        {
            m_guns = new List<Gun> { new Gun(a_scene, ReloadStyle.COMPLETE_CLIP, this), new Gun(a_scene, ReloadStyle.SHOT_BY_SHOT, this) };
            foreach (Gun gun in m_guns)
                AddChild(gun);
            m_guns[0].SetActive(true);

            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            m_sprite.rotation = 45;
            AddChild(m_sprite);

            m_canvas = a_canvas;

            m_deathSound = new Sound("Audio/death.wav");

        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);
        }

        public void MoveForward(float a_value)
        {
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.Normalize();
            m_movementForce.y -= a_value;
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.magnitude = m_speed;
        }

        public void MoveRight(float a_value)
        {
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.Normalize();
            m_movementForce.x += a_value;
            if (m_movementForce.sqrMagnitude > 0) m_movementForce.magnitude = m_speed;
        }

        public void FaceForward(float a_value)
        {
            m_direction.y -= a_value;
        }

        public void FaceRight(float a_value)
        {
            m_direction.x += a_value;
        }

        public void Shoot(bool a_pressed)
        {
            if (a_pressed)
                m_guns[m_currentGun].Shoot();
        }

        public void Reload(bool a_pressed)
        {
            if (a_pressed)
                m_guns[m_currentGun].Reload();
        }

        public void Dodge(bool a_pressed)
        {
            if (!a_pressed)
            {
                Vector2 fwd = new Vector2(rotation);
                    m_dodgeForce -= fwd*1500;
            }
        }

        public void SwitchWeapon(bool a_pressed)
        {
            if (!a_pressed)
            {
                m_guns[m_currentGun].SetActive(false);
                m_currentGun = (m_currentGun + 1) % m_guns.Count;
                m_guns[m_currentGun].SetActive(true);
                System.Console.WriteLine("switched");
            }

        }

        public void OnCollision(GameObject other)
        {
            //if (other is Bullet)
            //{
            //    if (((Bullet)other).m_owner.GetType().Equals(typeof(Enemy)))
            //    {
            //        other.Destroy();
            //        m_hp.current -= 5f;
            //    }
            //}
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
                    ((Program)game).Restart();
            }

            if (m_direction.sqrMagnitude > 0)
                rotation = m_direction.angle;
            m_direction *= 0;

            m_velocity = m_dodgeForce + m_movementForce;

            position += m_velocity * a_dt;
            m_dodgeForce *= 0.9f;
            m_movementForce *= 0;

        }

        protected override void RenderSelf(GLContext glContext)
        {
            if (m_hp.current > 0)
            {
                float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
                Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);

                m_canvas.Stroke(System.Drawing.Color.FromArgb(color.x, color.y, 0));
                m_canvas.StrokeWeight(5);
                m_canvas.Line(screenPosition.x - m_hp.current / 2, screenPosition.y + m_sprite.height / 2, screenPosition.x + m_hp.current / 2, screenPosition.y + m_sprite.height / 2);

                percentage = m_guns[m_currentGun].Clip / (float)m_guns[m_currentGun].ClipSize * m_hp.max;
                m_canvas.Stroke(System.Drawing.Color.Aqua);
                m_canvas.Line(screenPosition.x - percentage / 2, screenPosition.y + m_sprite.height / 2 + 5, screenPosition.x + percentage / 2, screenPosition.y + m_sprite.height / 2 + 5);
            }
        }
    }
}
