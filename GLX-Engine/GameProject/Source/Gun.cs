using GLXEngine.Core;
using GLXEngine;

namespace GameProject
{
    enum ReloadStyle
    {
        COMPLETE_CLIP,
        SHOT_BY_SHOT
    }

    class Gun : GameObject
    {
        float m_shotTimeBuffer = 0;
        float m_shotTime;

        float m_reloadTime;
        float m_reloadTimeBuffer;
        bool m_reloading = false;

        int m_clipSize = 10;
        int m_clip;

        bool m_active = false;

        Sound m_reloadSound;
        Sound m_shotSound;
        SoundChannel m_reloadSoundChannel;

        GameObject m_owner;

        protected ReloadStyle m_reloadStyle;

        Sprite m_sprite = new Sprite("Textures/gun.png");

        public Gun(Scene a_scene, ReloadStyle a_reloadStyle, GameObject a_owner) : base(a_scene)
        {
            m_reloadStyle = a_reloadStyle;

            if (m_reloadStyle == ReloadStyle.COMPLETE_CLIP)
            {
                m_reloadTime = 2.1f;
                m_shotTime = 1f / 10f;
                m_reloadSound = new Sound("Audio/reload_mag.wav");
                m_shotSound = new Sound("Audio/gun_shot.wav");
            }
            else
            {
                m_reloadTime = 4f;
                m_shotTime = 1f;
                m_reloadSound = new Sound("Audio/reload_shell.wav");
                m_shotSound = new Sound("Audio/shotgun_shot.wav");
            }

            y += 10;
            x -= 10;
            m_reloadTimeBuffer = m_reloadTime;
            m_clip = m_clipSize;

            m_owner = a_owner;
            m_sprite.SetOrigin(0, m_sprite.height / 2f);
            AddChild(m_sprite);
        }

        public float CoolDown { get { return Mathf.Clamp((m_reloadTimeBuffer / m_reloadTime), 0f, 1f); } }
        public int Clip { get { return m_clip; } set { m_clip = value < 0 ? 0 : (value > m_clipSize ? m_clipSize : value); } }

        public int ClipSize { get { return m_clipSize; } }

        public void Update(float a_dt)
        {
            if (m_active)
            {
                if (m_clip <= 0 && m_reloading == false)
                {
                    Reload();
                }

                HandleReload(a_dt);

                if (m_shotTimeBuffer >= m_shotTime)
                    m_shotTimeBuffer = m_shotTime;

                if (!m_reloading)
                    m_shotTimeBuffer += a_dt;
            }
        }

        public void SetActive(bool a_active)
        {
            m_active = a_active;
            if (a_active && m_reloading && m_reloadStyle == ReloadStyle.COMPLETE_CLIP)
            {
                m_clip = 0;
                m_reloadTimeBuffer = 0;
                m_reloading = false;
            }
            else if (!a_active && m_reloadSoundChannel != null)
            {
                m_reloadSoundChannel.Stop();
            }
        }

        public void HandleReload(float a_dt)
        {
            if (m_active)
            {
                if (m_reloadTimeBuffer >= m_reloadTime)
                {
                    m_clip = m_clipSize;
                    m_reloading = false;
                    m_reloadTimeBuffer = m_reloadTime;
                }

                if (m_reloading)
                {
                    m_reloadTimeBuffer += a_dt;

                    if (m_reloadStyle == ReloadStyle.SHOT_BY_SHOT)
                    {
                        int temp = m_clip;
                        m_clip = Mathf.Floor(m_reloadTimeBuffer / m_reloadTime * m_clipSize);
                        if (temp != m_clip)
                            m_reloadSound.Play();
                    }
                }
            }

        }

        public void Reload()
        {
            if (m_reloadStyle == ReloadStyle.COMPLETE_CLIP)
                m_reloadSoundChannel = m_reloadSound.Play();
            m_reloading = true;
        }

        public void Shoot()
        {
            while (m_shotTimeBuffer >= m_shotTime)
            {
                m_shotTimeBuffer -= m_shotTime;
                m_reloadTimeBuffer -= m_reloadTime / m_clipSize;
                m_clip--;

                if (m_reloadStyle == ReloadStyle.COMPLETE_CLIP)
                    CreateBullet(2);
                else
                    for (int i = -3; i < 6; i++)
                        CreateBullet(i);

                m_shotSound.Play();
            }
        }

        void CreateBullet(float a_angleOffset = 0f)
        {
            Bullet bullet = new Bullet(m_scene, m_owner);
            bullet.SetScaleXY(4);
            ((Overworld)m_scene).bullets.Add(bullet);
            Vector2 fwd = new Vector2(m_owner.rotation);
            bullet.position = m_scene.InverseTransformPoint(screenPosition) + fwd * m_sprite.width;
            bullet.m_velocity = Vector2.RandomFromAngle(m_owner.rotation - a_angleOffset, m_owner.rotation + a_angleOffset).SetMagnitude(800);

            m_scene.AddChild(bullet);
        }
    }
}
