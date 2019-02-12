using GLXEngine.Core;
using GLXEngine;

namespace GameProject
{
    public class Player : GameObject
    {
        const float m_linearAcceleration = 6f;
        const float m_angularAcceleration = 5f;

        float m_shotTimeBuffer = 0;
        float m_reloadTime = 1f/10f;

        float m_cooldownTimeBuffer = 2f;
        float m_cooldownTime = 2f;
        bool m_cooldown = false;

        Sprite m_sprite = new Sprite("Textures/shooter.png");

        public Hp m_hp = new Hp();
        EasyDraw m_canvas;

        Sound m_shotSound;
        Sound m_deathSound;
        SoundChannel m_deathSoundChannel;
        Sound m_rechargeSound;

        public Player(Scene a_scene, EasyDraw a_canvas) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);
            m_canvas = a_canvas;

            m_shotSound = new Sound("Audio/shot.wav");
            m_deathSound = new Sound("Audio/death.wav");
            m_rechargeSound = new Sound("Audio/recharge.wav");
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);
        }

        public void MoveForward(float a_value)
        {
            Vector2 forward = new Vector2(rotation);
            m_velocity += forward * (a_value * m_linearAcceleration);
        }

        public void MoveRight(float a_value)
        {
            Vector2 right = new Vector2(rotation + 90f);
            m_velocity += right * (a_value * m_linearAcceleration);
        }

        public void RotateRight(float a_value)
        {
            rotation += m_angularAcceleration * a_value;
        }

        public void Shoot(bool a_pressed)
        {
            while(m_shotTimeBuffer >= m_reloadTime)
            {
                m_shotTimeBuffer -= m_reloadTime;
                m_cooldownTimeBuffer -= m_cooldownTime*0.1f;
                Bullet bullet = new Bullet(m_scene);
                ((Overworld)m_scene).bullets.Add(bullet);
                bullet.position = position;
                bullet.m_velocity = new Vector2(rotation) * (m_velocity.magnitude + 400f);
                m_scene.AddChild(bullet);
                m_shotSound.Play();
            }
        }

        public void OnCollision(GameObject other)
        {
            if (other is Enemy)
            {
                other.Destroy();
                m_hp.current -= 10f;
            }
        }

        void Update(float a_dt)
        {
            if(m_hp.current <= 0)
            {
                if(m_deathSoundChannel == null)
                {
                    m_deathSoundChannel = m_deathSound.Play();
                    m_sprite.Destroy();
                }

                if(!m_deathSoundChannel.IsPlaying)
                    ((Program)game).Restart();
            }

            position += m_velocity * a_dt;
            m_velocity *= Mathf.Pow(0.99f, a_dt / (1f/60f));

            if(m_cooldownTimeBuffer <= 0 && m_cooldown == false)
            {
                m_rechargeSound.Play();
                m_cooldown = true;
            }

            if(m_cooldownTimeBuffer >= m_cooldownTime)
            {
                m_cooldown = false;
                m_cooldownTimeBuffer = m_cooldownTime;
            }

            if(m_shotTimeBuffer >= m_reloadTime)
                m_shotTimeBuffer = m_reloadTime;

            if(!m_cooldown)
                m_shotTimeBuffer += a_dt;
            
            m_cooldownTimeBuffer += a_dt;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            if(m_hp.current > 0)
            {
                float percentage = Mathf.Clamp(m_hp.current/m_hp.max, 0f, 1f);
                Vector2i color = new Vector2i(Mathf.Round(255 * (1-percentage)), Mathf.Round(255*percentage)).SetMagnitude(255);

                m_canvas.Stroke(System.Drawing.Color.FromArgb(color.x, color.y, 0));
                m_canvas.StrokeWeight(5);
                m_canvas.Line(screenPosition.x - m_hp.current/2, screenPosition.y + m_sprite.height/2, screenPosition.x + m_hp.current/2, screenPosition.y + m_sprite.height/2);

                percentage = Mathf.Clamp((m_cooldownTimeBuffer/m_cooldownTime), 0f, 1f)*m_hp.max;
                m_canvas.Stroke(System.Drawing.Color.Aqua);
                m_canvas.Line(screenPosition.x - percentage/2, screenPosition.y + m_sprite.height/2 + 5, screenPosition.x + percentage/2, screenPosition.y + m_sprite.height/2 + 5);
            }
        }
    }
}
