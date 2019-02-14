using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using GLXEngine.ECS;


namespace GLXEngine.Physics
{
    public class PhysicsSystem : ECSSystem
    {
        public PhysicsSystem(Scene a_scene) : base(new System.Type[] { typeof(MeshComponent2D), typeof(TransformComponent), typeof(PhysicsComponent) })
        {
            a_scene.objectList.ForEach(gameObject => { Add(gameObject); });
        }

        public override void Update(float a_dt)
        {
            Dictionary<int, ECSComponentHandle<MeshComponent2D>> meshComponents = GetComponents<MeshComponent2D>();
            Dictionary<int, ECSComponentHandle<TransformComponent>> transformComponents = GetComponents<TransformComponent>();

            foreach (int entityID in GetEntityList())
            {
                MeshComponent2D meshComponent = meshComponents[entityID].m_value;
                TransformComponent transformComponent = transformComponents[entityID].m_value;

                Vector2 origin = new Vector2();

                List<Point> points = meshComponent.points;
                for(int i = 0; i < points.Count; i++)
                {
                    Point point = points[i];
                    Vector2 velocity = point.m_position - point.m_previousPosition;
                    point.m_position += velocity;
                    point.m_previousPosition = point.m_position;
                    origin += point.m_position;
                }

                origin /= points.Count;
                transformComponent.position += origin;
                for(int i = 0; i < points.Count; i++)
                {
                    points[i].m_position -= origin;
                    points[i].m_previousPosition -= origin;
                }

                transform.rotation = (points[0].m_position.angle + points[1].m_position.angle)/2f;

            }
        }

    }
}
