using System.Collections.Generic;
using GLXEngine.Core;
using GLXEngine;

namespace GameProject
{
    enum ReloadStyle
    {
        COMPLETE_CLIP,
        SHOT_BY_SHOT
    }

    abstract class Gun : BoundsObject
    {
        protected bool m_automatic = false;

        protected float m_shotTimeBuffer = 0;
        protected float m_shotTime;

        protected float m_barrelLength = 0;
        protected float m_damage = 10;
        protected float m_bulletsPerShot = 1;
        protected float m_inaccuracy = 0;
        protected float m_spread = 0;

        protected float m_reloadTime;
        protected float m_reloadTimeBuffer;

        protected bool m_reloading = false;

        protected int m_clipSize = 10;
        protected int m_clip;

        protected bool m_active = false;

        protected Sound m_reloadSound;
        protected Sound m_shotSound;
        protected SoundChannel m_reloadSoundChannel;

        protected GameObject m_owner;
        protected GameObject m_player;

        protected ReloadStyle m_reloadStyle;

        protected Sprite m_sprite;
        protected Texture2D m_bulletTexture;
        private List<Sprite> m_reloadBar = new List<Sprite>();

        public Gun(Scene a_scene, ReloadStyle a_reloadStyle, GameObject a_owner, GameObject a_player, Sprite a_sprite) : base(a_scene, a_sprite.width, a_sprite.height)
        {
            visible = false;

            m_reloadStyle = a_reloadStyle;

            m_owner = a_owner;
            m_player = a_player;

            m_sprite = a_sprite;
            AddChild(m_sprite);
            m_sprite.SetOrigin(m_sprite.width / 2f, m_sprite.height / 2f);

            SetOrigin(width / 2, height / 2);

            m_barrelLength = width / 2f;
        }

        protected void Setup()
        {
            m_reloadTimeBuffer = m_reloadTime;
            m_clip = m_clipSize;

            if (m_bulletTexture != null)
                for (int i = 0; i < m_clipSize; i++)
                {
                    Sprite sprite = new Sprite(m_bulletTexture);
                    sprite.x = i * sprite.width;
                    sprite.visible = false;
                    m_reloadBar.Add(sprite);
                }
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(this, new System.Type[] { GetType() });
        }

        public void OnCollision(GameObject other, Vector2 a_mtv)
        {
            if (other is Bullet)
            {
                if (!((Bullet)other).m_owner.GetType().Equals(m_owner.GetType()))
                {
                    other.Destroy();
                    if (!m_owner.GetType().Equals(typeof(Player)))
                    {
                        (game as Program).score += 1;
                        Destroy();
                    }
                }
            }
        }

        public float CoolDown { get { return Mathf.Clamp((m_reloadTimeBuffer / m_reloadTime), 0f, 1f); } }
        public int Clip { get { return m_clip; } set { m_clip = value < 0 ? 0 : (value > m_clipSize ? m_clipSize : value); } }
        public bool IsReloading { get { return m_reloading; } }
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
            visible = a_active;
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
            if (!(m_clip == m_clipSize))
            {
                if (m_reloadStyle == ReloadStyle.COMPLETE_CLIP)
                    m_reloadSoundChannel = m_reloadSound.Play();

                m_reloading = true;
            }
        }

        public virtual void Shoot(bool a_pressed)
        {
            if (a_pressed && !m_automatic)
                return;

            while (m_shotTimeBuffer >= m_shotTime)
            {
                m_shotTimeBuffer -= m_shotTime;
                m_reloadTimeBuffer -= m_reloadTime / m_clipSize;
                m_clip--;

                for (int i = 0; i < m_bulletsPerShot; i++)
                    CreateBullet(m_inaccuracy, (i - (m_bulletsPerShot - 1) / 2) * m_spread);

                m_shotSound.Play();
            }
        }

        void CreateBullet(float a_inaccuracy = 0f, float a_angleOffset = 1f)
        {
            Bullet bullet = new Bullet(m_scene, m_owner, m_player, m_damage);
            bullet.SetScaleXY(4);
            ((Overworld)m_scene).bullets.Add(bullet);

            Vector2 fwd = new Vector2(m_owner.rotation);
            bullet.position = m_scene.InverseTransformPoint(screenPosition) + fwd * m_barrelLength;

            bullet.m_velocity = Vector2.RandomFromAngle(m_owner.rotation + a_angleOffset - a_inaccuracy, m_owner.rotation + a_angleOffset + a_inaccuracy).SetMagnitude(1000);

            m_scene.AddChild(bullet);
        }
    }
}
