using GLXEngine.Core;
using GLXEngine;

namespace GameProject
{
    class Gun : GameObject
    {
        float m_shotTimeBuffer = 0;
        float m_reloadTime = 1f / 10f;

        float m_cooldownTimeBuffer = 2f;
        float m_cooldownTime = 2f;
        bool m_cooldown = false;

        Sound m_rechargeSound;
        Sound m_shotSound;

        public Gun(Scene a_scene) : base(a_scene)
        {
            m_shotSound = new Sound("Audio/shot.wav");
            m_rechargeSound = new Sound("Audio/recharge.wav");
        }

        public float CoolDown { get { return Mathf.Clamp((m_cooldownTimeBuffer/m_cooldownTime), 0f, 1f); } }

        public void Update(float a_dt)
        {
            if (m_cooldownTimeBuffer <= 0 && m_cooldown == false)
            {
                m_rechargeSound.Play();
                m_cooldown = true;
            }

            if (m_cooldownTimeBuffer >= m_cooldownTime)
            {
                m_cooldown = false;
                m_cooldownTimeBuffer = m_cooldownTime;
            }

            if (m_cooldown)
                m_cooldownTimeBuffer += a_dt;

            if (m_shotTimeBuffer >= m_reloadTime)
                m_shotTimeBuffer = m_reloadTime;

            if (!m_cooldown)
                m_shotTimeBuffer += a_dt;
        }

        public void Shoot(bool a_pressed)
        {
            while (m_shotTimeBuffer >= m_reloadTime)
            {
                m_shotTimeBuffer -= m_reloadTime;
                m_cooldownTimeBuffer -= m_cooldownTime * 0.1f;

                Bullet bullet = new Bullet(m_scene);
                ((Overworld)m_scene).bullets.Add(bullet);
                bullet.position = position;
                bullet.m_velocity = new Vector2(rotation).SetMagnitude(1000);

                m_scene.AddChild(bullet);
                m_shotSound.Play();
            }
        }
    }
}
