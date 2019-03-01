using System;
using System.Reflection;
using System.Collections.Generic;
using GLXEngine.Core;

namespace GLXEngine
{
    //------------------------------------------------------------------------------------------------------------------------
    //														CollisionManager
    //------------------------------------------------------------------------------------------------------------------------
    public class CollisionManager
    {
        private delegate void CollisionDelegate(GameObject a_gameObject, Vector2 a_minimumTranslationVec);

        //------------------------------------------------------------------------------------------------------------------------
        //														ColliderInfo
        //------------------------------------------------------------------------------------------------------------------------
        private struct ColliderInfo
        {
            public GameObject m_gameObject;
            public CollisionDelegate m_onCollision;

            //------------------------------------------------------------------------------------------------------------------------
            //														ColliderInfo()
            //------------------------------------------------------------------------------------------------------------------------
            public ColliderInfo(GameObject a_gameObject, CollisionDelegate a_onCollision)
            {
                m_gameObject = a_gameObject;
                m_onCollision = a_onCollision;
            }
        }

        private QuadTree m_colliderTree;
        private List<GameObject> colliderList = new List<GameObject>();
        private List<ColliderInfo> activeColliderList = new List<ColliderInfo>();
        private Dictionary<GameObject, ColliderInfo> _collisionReferences = new Dictionary<GameObject, ColliderInfo>();

        //------------------------------------------------------------------------------------------------------------------------
        //														CollisionManager()
        //------------------------------------------------------------------------------------------------------------------------
        public CollisionManager(Rectangle a_bounds, int a_cellCapacity = 4)
        {
            m_colliderTree = new QuadTree(a_bounds, a_cellCapacity);
        }
        public CollisionManager(CollisionManager a_masterCollisionManager)
        {
            colliderList = new List<GameObject>(a_masterCollisionManager.colliderList);
            activeColliderList = new List<ColliderInfo>(a_masterCollisionManager.activeColliderList);
            _collisionReferences = new Dictionary<GameObject, ColliderInfo>(a_masterCollisionManager._collisionReferences);
            m_colliderTree = new QuadTree(a_masterCollisionManager.m_colliderTree);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public void Step()
        {
            m_colliderTree = new QuadTree(m_colliderTree.m_boundary, m_colliderTree.m_capacity);

            Console.Write(m_colliderTree.Count + " ");

            for (int i = 0; i < colliderList.Count; i++)
            {
                GameObject gameObject = colliderList[i];
                m_colliderTree.Insert(new QuadTree.Point(gameObject.screenPosition, gameObject));
            }

            Console.Write(colliderList.Count + " ");
            Console.WriteLine(m_colliderTree.Count);

            for (int i = activeColliderList.Count - 1; i >= 0; i--)
            {
                ColliderInfo info = activeColliderList[i];

                List<QuadTree.Point> foundColliders = new List<QuadTree.Point>();

                m_colliderTree.Query(BroadPhaseRectangle(info.m_gameObject), ref foundColliders);

                for (int j = foundColliders.Count - 1; j >= 0; j--)
                {

                    if (j >= foundColliders.Count) continue; //fix for removal in loop

                    GameObject other = foundColliders[j].data as GameObject;
                    if (info.m_gameObject != other)
                    {
                        if (info.m_gameObject.HitTest(ref other))
                        {
                            if (info.m_onCollision != null)
                                info.m_onCollision(other, info.m_gameObject.collider.m_minimumTranslationVec);

                            info.m_gameObject.collider.m_minimumTranslationVec = new Vector2();
                        }

                    }
                }
            }
        }

        private Circle BroadPhaseCircle(GameObject gameObject)
        {
            Vector2 position = gameObject.screenPosition;
            Vector2[] hullA = (gameObject.collider as BoxCollider).m_owner.GetHull();

            Vector2 velocityA = gameObject.m_velocity;

            float extendA = 0;

            foreach (Vector2 point in hullA)
            {
                if (Mathf.Abs(point.x) > extendA)
                    extendA = point.x;
                if (Mathf.Abs(point.y) > extendA)
                    extendA = point.y;
            }

            float deltaTime = Time.deltaTime;
            float radius = velocityA.magnitude * deltaTime * 0.5f + extendA;

            Vector2 center = position - velocityA * deltaTime * 0.5f;

            return new Circle(position.x, position.y, 500);
        }

        private Rectangle BroadPhaseRectangle(GameObject gameObject)
        {
            Vector2 position = gameObject.screenPosition;
            Vector2[] hullA = (gameObject.collider as BoxCollider).m_owner.GetHull();

            Vector2 velocityA = gameObject.m_velocity;

            float extendA = 0;

            foreach (Vector2 point in hullA)
            {
                if (Mathf.Abs(point.x) > extendA)
                    extendA = point.x;
                if (Mathf.Abs(point.y) > extendA)
                    extendA = point.y;
            }

            float deltaTime = Time.deltaTime;
            float radius = velocityA.magnitude * deltaTime * 0.5f + extendA;

            Vector2 center = position - velocityA * deltaTime * 0.5f;

            return new Rectangle(position.x - 250, position.y - 250, 500, 500);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //												 GetCurrentCollisions()
        //------------------------------------------------------------------------------------------------------------------------
        public GameObject[] GetCurrentCollisions(GameObject gameObject)
        {
            List<GameObject> list = new List<GameObject>();
            for (int j = colliderList.Count - 1; j >= 0; j--)
            {

                if (j >= colliderList.Count) continue; //fix for removal in loop

                GameObject other = colliderList[j];
                if (gameObject != other)
                    if (gameObject.HitTest(ref other))
                        list.Add(other);

            }
            return list.ToArray();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(ref GameObject gameObject)
        {
            if (gameObject.collider != null && !colliderList.Contains(gameObject))
            {
                colliderList.Add(gameObject);
            }

            MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (info != null)
            {

                CollisionDelegate onCollision = (CollisionDelegate)Delegate.CreateDelegate(typeof(CollisionDelegate), gameObject, info, false);
                if (onCollision != null && !_collisionReferences.ContainsKey(gameObject))
                {
                    ColliderInfo colliderInfo = new ColliderInfo(gameObject, onCollision);
                    _collisionReferences[gameObject] = colliderInfo;
                    activeColliderList.Add(colliderInfo);
                }

            }
            else
            {
                validateCase(gameObject);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														validateCase()
        //------------------------------------------------------------------------------------------------------------------------
        private void validateCase(GameObject gameObject)
        {
            MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (info != null)
            {
                throw new Exception("'OnCollision' function was not binded. Please check its case (capital O?)");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(GameObject gameObject)
        {
            colliderList.Remove(gameObject);
            if (_collisionReferences.ContainsKey(gameObject))
            {
                ColliderInfo colliderInfo = _collisionReferences[gameObject];
                activeColliderList.Remove(colliderInfo);
                _collisionReferences.Remove(gameObject);
            }
        }

        public string GetDiagnostics()
        {
            string output = "";
            output += "Number of colliders: " + colliderList.Count + '\n';
            output += "Number of active colliders: " + activeColliderList.Count + '\n';
            return output;
        }
    }
}

