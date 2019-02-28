using GLXEngine;

namespace GameProject
{
    class TommyGun : Gun
    {
        public TommyGun(Scene a_scene, GameObject a_owner, GameObject a_player) : base(a_scene, ReloadStyle.COMPLETE_CLIP, a_owner, a_player, new Sprite("Textures/thompson.png"))
        {
            m_automatic = true;

            m_reloadTime = 2.1f;

            m_shotTime = 1f / 10f;

            m_reloadSound = new Sound("Audio/reload_mag.wav");
            m_shotSound = new Sound("Audio/gun_shot.wav");

            m_clipSize = 30;

            m_damage = 25;
            m_inaccuracy = 2;
            m_spread = 0;
            m_bulletsPerShot = 1;

            Setup();
        }
    }
}
