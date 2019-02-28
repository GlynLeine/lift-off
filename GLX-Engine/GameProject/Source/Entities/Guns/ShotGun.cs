using GLXEngine;

namespace GameProject
{
    class ShotGun : Gun
    {
        public ShotGun(Scene a_scene, GameObject a_owner, GameObject a_player) : base(a_scene, ReloadStyle.SHOT_BY_SHOT, a_owner, a_player, new Sprite("Textures/shotGun.png"))
        {
            m_reloadTime = 2.1f;

            m_shotTime = 1f;

            m_reloadSound = new Sound("Audio/reload_shell.wav");
            m_shotSound = new Sound("Audio/shotgun_shot.wav");

            m_clipSize = 4;

            m_damage = 30;
            m_inaccuracy = 2;
            m_spread = 1;
            m_bulletsPerShot = 6;

            Setup();
        }
    }
}
