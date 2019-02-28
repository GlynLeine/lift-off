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

        private List<GameObject> colliderList = new List<GameObject>();
        private List<ColliderInfo> activeColliderList = new List<ColliderInfo>();
        private Dictionary<GameObject, ColliderInfo> _collisionReferences = new Dictionary<GameObject, ColliderInfo>();

        //------------------------------------------------------------------------------------------------------------------------
        //														CollisionManager()
        //------------------------------------------------------------------------------------------------------------------------
        public CollisionManager()
        {
        }
        public CollisionManager(CollisionManager a_masterCollisionManager)
        {
            List<GameObject> colliderList = new List<GameObject>(a_masterCollisionManager.colliderList);
            List<ColliderInfo> activeColliderList = new List<ColliderInfo>(a_masterCollisionManager.activeColliderList);
            Dictionary<GameObject, ColliderInfo> _collisionReferences = new Dictionary<GameObject, ColliderInfo>(a_masterCollisionManager._collisionReferences);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public void Step()
        {
            for (int i = activeColliderList.Count - 1; i >= 0; i--)
            {
                ColliderInfo info = activeColliderList[i];

                for (int j = colliderList.Count - 1; j >= 0; j--)
                {

                    if (j >= colliderList.Count) continue; //fix for removal in loop

                    GameObject other = colliderList[j];
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

