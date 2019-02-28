using GLXEngine;

namespace GameProject
{
    class Carbine : Gun
    {
        public Carbine(Scene a_scene, GameObject a_owner, GameObject a_player) : base(a_scene, ReloadStyle.COMPLETE_CLIP, a_owner, a_player, new Sprite("Textures/carbine.png"))
        {
            m_reloadTime = 2.1f;

            m_shotTime = 0.2f;

            m_reloadSound = new Sound("Audio/reload_mag.wav");
            m_shotSound = new Sound("Audio/gun_shot.wav");

            m_clipSize = 15;

            m_damage = 50;
            m_inaccuracy = 2;
            m_spread = 0;
            m_bulletsPerShot = 1;

            Setup();
        }
    }
}
