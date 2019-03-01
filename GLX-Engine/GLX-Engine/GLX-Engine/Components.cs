using GLXEngine.Core;
using System.Collections.Generic;
using GLXEngine.ECS;

namespace GLXEngine
{
    public class TransformComponent : ECSComponent
    {
        public float[] m_matrix = new float[16] {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f };

        float m_rotation = 0.0f;
        
        public Vector2 m_scale = new Vector2(1f, 1f);

        public Vector2 position {
			get { return new Vector2(m_matrix[12], m_matrix[13]);}
			set { m_matrix[12] = value.x; m_matrix[13] = value.y;}
		}

        public float rotation {
			get { return m_rotation; }
			set {
				m_rotation = value;
				float rotation = m_rotation * Mathf.PI / 180.0f;
				float cosine = Mathf.Cos (rotation);
				float sine = Mathf.Sin (rotation);
				m_matrix[0] = cosine;
				m_matrix[1] = sine;
				m_matrix[4] = -sine;
				m_matrix[5] = cosine;
			}
		}
    }

    public class PhysPoint
    {
        public Vector2 m_position = new Vector2();
        public Vector2 m_previousPosition = new Vector2();

        public PhysPoint(Vector2 a_position, Vector2 a_previousPosition)    {   m_position = a_position; m_previousPosition = a_previousPosition;   }
        public PhysPoint(PhysPoint a_source) {  m_position = a_source.m_position; m_previousPosition = a_source.m_previousPosition; }
    }

    public class MeshComponent2D : ECSComponent
    {
        public List<PhysPoint> points = new List<PhysPoint>();
    }

    public class PhysicsComponent : ECSComponent
    { }
}
