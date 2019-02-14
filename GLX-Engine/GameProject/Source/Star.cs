using GLXEngine.Core;
using GLXEngine;
using static GLXEngine.Utils;
using System.Drawing;

namespace GameProject
{
    class Star : GameObject
    {
        Vector2 m_prevScreenPosition;
        float m_size;
        float m_lastAngle = 0;
        int m_greyScale = 255;
        EasyDraw m_canvas;
        bool m_allowRespawn = false;
        Vector2 m_line;
        float m_blinkSpeed;
        int m_blinkStep = 0;
        float m_blinkTimeBuffer = 0;

        public Star(Scene a_scene, EasyDraw a_canvas, Player a_player) : base(a_scene)
        {
            a_scene.AddChild(this);
            screenPosition = Vector2.Random(game.RenderRange.left, game.RenderRange.right, game.RenderRange.top, game.RenderRange.bottom);
            m_prevScreenPosition = new Vector2(screenPosition);

            m_size = Random(1f, 3f);

            m_line = new Vector2(m_lastAngle);
            m_line.magnitude = m_size;

            m_blinkSpeed = Random(0.3f, 1.3f);

            m_canvas = a_canvas;
        }


        public void Update(float a_dt)
        {
            if (!m_scene.InView(new float[] { position.x, position.y, position.x + m_line.x, position.y + m_line.y }))
            {
                if (m_allowRespawn)
                {
                    Vector2 topLeft = new Vector2(m_scene.RenderRange.left, m_scene.RenderRange.top);
                    Vector2 bottomRight = new Vector2(m_scene.RenderRange.right, m_scene.RenderRange.bottom);

                    if (position.x < topLeft.x)
                    {
                        x = bottomRight.x;
                        y = Random(topLeft.y, bottomRight.y);
                    }

                    if (position.x > bottomRight.x)
                    {
                        x = topLeft.x;
                        y = Random(topLeft.y, bottomRight.y);
                    }

                    if (position.y < topLeft.y)
                    {
                        y = bottomRight.y;
                        x = Random(topLeft.x, bottomRight.x);
                    }

                    if (position.y > bottomRight.y)
                    {
                        y = topLeft.y;
                        x = Random(topLeft.x, bottomRight.x);
                    }

                    m_prevScreenPosition = screenPosition;
                    m_size = Random(1f, 3f);
                    m_allowRespawn = false;
                }
            }
            else
                m_allowRespawn = true;


            m_blinkTimeBuffer += a_dt;
            while (m_blinkTimeBuffer >= m_blinkSpeed)
            {
                m_blinkTimeBuffer -= m_blinkSpeed;
                if (m_blinkStep == 0)
                {
                    m_greyScale = 255;
                }
                else
                {
                    m_greyScale = 127;
                }
                m_blinkStep = ++m_blinkStep % 2;
            }

        }

        protected override void RenderSelf(GLContext glContext)
        {
            m_line = (m_prevScreenPosition != screenPosition) ? (m_prevScreenPosition - screenPosition) : new Vector2(m_lastAngle);
            if (m_prevScreenPosition != screenPosition)
                position += m_line * (1 - (m_size / 4));

            if (m_line.magnitude <= m_size)
                m_line.magnitude = m_size;

            m_canvas.StrokeWeight(m_size);
            m_canvas.Stroke((int)(m_greyScale * (m_size / 3)));
            m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_line.x, screenPosition.y + m_line.y);
            m_prevScreenPosition = new Vector2(screenPosition);

            //Vector2 lineEnd = new Vector2(1, 0);
            //drawCanvas.Line(0, 0, lineEnd.x, lineEnd.y);

            //if (m_scene.InView(new float[] { position.x, position.y, m_prevPos.x, m_prevPos.y }))
            //{
            //    glContext.SetColor((byte)((m_color >> 16) & 0xFF),
            //                       (byte)((m_color >> 8) & 0xFF),
            //                       (byte)(m_color & 0xFF),
            //                       0xFF);
            //    glContext.DrawLine(new float[] { 0f,        0f,
            //                                     lineEnd.x, lineEnd.y}, m_size);
            //    glContext.SetColor(1, 1, 1, 1);
            //}

            //m_prevPos = position;
        }
    }
}
