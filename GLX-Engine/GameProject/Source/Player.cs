using GLXEngine.Core;
using GLXEngine;

namespace GameProject
{
    public class Player : GameObject
    {
        const float m_speed = 600f;
        const float m_angularAcceleration = 5f;

        Vector2 m_direction = new Vector2(0f);

        Sprite m_sprite = new Sprite("Textures/player.png");

        public Hp m_hp = new Hp();
        EasyDraw m_canvas;

        Sound m_deathSound;
        SoundChannel m_deathSoundChannel;

        Gun m_gun;

        public Player(Scene a_scene, EasyDraw a_canvas) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);
            m_gun = new Gun(a_scene);
            AddChild(m_gun);

            m_canvas = a_canvas;

            m_deathSound = new Sound("Audio/death.wav");
        }

        protected override Collider createCollider()
        {
            return new BoxCollider(m_sprite);
        }

        public void MoveForward(float a_value)
        {
            if(m_velocity.sqrMagnitude > 0) m_velocity.Normalize();
            m_velocity.y -= a_value;
            if(m_velocity.sqrMagnitude > 0) m_velocity.magnitude = m_speed;
        }

        public void MoveRight(float a_value)
        {
            if(m_velocity.sqrMagnitude > 0) m_velocity.Normalize();
            m_velocity.x += a_value;
            if(m_velocity.sqrMagnitude > 0) m_velocity.magnitude = m_speed;
        }

        public void FaceForward(float a_value)
        {
            m_direction.y -= a_value;
            m_direction.Normalize();
            rotation = m_direction.angle;
        }

        public void FaceRight(float a_value)
        {
            m_direction.x += a_value;
            m_direction.Normalize();
            rotation = m_direction.angle;
        }

        public void OnCollision(GameObject other)
        {
            if (other is Enemy)
            {
                //other.Destroy();
                //m_hp.current -= 10f;
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

            //if(m_velocity.sqrMagnitude > 0)
                //rotation++;// = m_velocity.angle;
            position += m_velocity * a_dt;
            m_velocity *= 0;

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

                percentage = m_gun.CoolDown*m_hp.max;
                m_canvas.Stroke(System.Drawing.Color.Aqua);
                m_canvas.Line(screenPosition.x - percentage/2, screenPosition.y + m_sprite.height/2 + 5, screenPosition.x + percentage/2, screenPosition.y + m_sprite.height/2 + 5);
            }
        }
    }
}
