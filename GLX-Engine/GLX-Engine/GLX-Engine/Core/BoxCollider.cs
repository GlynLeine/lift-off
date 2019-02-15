using System;
using System.Collections.Generic;

namespace GLXEngine.Core
{
    public class BoxCollider : Collider
    {
        private Sprite _owner;
        private List<Type> _ignore;

        //------------------------------------------------------------------------------------------------------------------------
        //														BoxCollider()
        //------------------------------------------------------------------------------------------------------------------------		
        public BoxCollider(Sprite owner, Type[] ignore = null)
        {
            _owner = owner;
            if (ignore != null)
                _ignore = new List<Type>(ignore);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool HitTest(Collider other)
        {
            if (other is BoxCollider)
            {
                if (_owner.parent == null)
                    return false;

                if (((BoxCollider)other)._owner.parent == null)
                    return false;

                if (_ignore != null)
                    if (_ignore.Contains(((BoxCollider)other)._owner.parent.GetType()))
                        return false;

                if (((BoxCollider)other)._ignore != null)
                    if (((BoxCollider)other)._ignore.Contains(_owner.parent.GetType()))
                        return false;

                Vector2[] extendsA = _owner.GetExtents();
                if (extendsA == null) return false;

                Vector2[] extendsB = ((BoxCollider)other)._owner.GetExtents();
                if (extendsB == null) return false;

                Vector2 positionA = _owner.screenPosition;
                Vector2[] hullA = new Vector2[extendsA.Length];
                for (int i = 0; i < extendsA.Length; i++)
                {
                    hullA[i] = new Vector2(extendsA[i]);// - positionA;
                }

                Vector2 positionB = ((BoxCollider)other)._owner.screenPosition;
                Vector2[] hullB = new Vector2[extendsB.Length];
                for (int i = 0; i < extendsB.Length; i++)
                {
                    hullB[i] = new Vector2(extendsB[i]);// - positionB;
                }

                //if(areaOverlap(hullA, hullB))
                //    return true;
                //return areaOverlap(hullB, hullA);

                if (CircleBroadPhase(hullA, positionA, _owner.parent.m_velocity, hullB, positionB, ((BoxCollider)other)._owner.parent.m_velocity))
                    return SATNarrowPhase(hullA, positionA, hullB, positionB);

                return false;
            }
            else
            {
                return false;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool HitTestPoint(float x, float y)
        {
            Vector2[] c = _owner.GetExtents();
            if (c == null) return false;
            Vector2 p = new Vector2(x, y);
            return pointOverlapsArea(p, c);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														CircleNarrowPhase()
        //------------------------------------------------------------------------------------------------------------------------
        private bool CircleBroadPhase(Vector2[] hullA, Vector2 positionA, Vector2 velocityA, Vector2[] hullB, Vector2 positionB, Vector2 velocityB)
        {
            float extremeA = 0, extremeB = 0;

            foreach (Vector2 point in hullA)
            {
                if (Mathf.Abs(point.x) > extremeA)
                    extremeA = point.x;
                if (Mathf.Abs(point.y) > extremeA)
                    extremeA = point.y;
            }

            foreach (Vector2 point in hullB)
            {
                if (Mathf.Abs(point.x) > extremeB)
                    extremeB = point.x;
                if (Mathf.Abs(point.y) > extremeB)
                    extremeB = point.y;
            }

            float deltaTime = Time.deltaTime / 1000f;
            float radiusA = velocityA.magnitude * deltaTime * 0.5f + extremeA;
            float radiusB = velocityB.magnitude * deltaTime * 0.5f + extremeB;

            Vector2 midPointA = positionA - velocityA * deltaTime * 0.5f;
            Vector2 midPointB = positionB - velocityB * deltaTime * 0.5f;
            return Vector2.Distance(midPointA, midPointB) <= (radiusA + radiusB);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SATNarrowPhase()
        //------------------------------------------------------------------------------------------------------------------------
        private bool SATNarrowPhase(Vector2[] hullA, Vector2 positionA, Vector2[] hullB, Vector2 positionB)
        {
            List<Vector2> axes = new List<Vector2>();

            for (int i = 0; i < hullA.Length; i++)
            {
                Vector2 a = hullA[i];
                Vector2 b = hullA[(i + 1) % hullA.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);
                if (normal.angle > 180)
                    normal.angle -= 180;
                if (!axes.Contains(normal))
                    axes.Add(normal);
            }
            for (int i = 0; i < hullB.Length; i++)
            {
                Vector2 a = hullB[i];
                Vector2 b = hullB[(i + 1) % hullB.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);
                if (normal.angle > 180)
                    normal.angle -= 180;
                if (!axes.Contains(normal))
                    axes.Add(normal.Normalize());
            }

            for (int i = 0; i < axes.Count; i++)
            {
                float minA = float.MaxValue, maxA = float.MinValue, minB = float.MaxValue, maxB = float.MinValue;
                for (int j = 0; j < hullA.Length; j++)
                {
                    float projection = hullA[j].Dot(axes[i]);
                    if (projection < minA)
                        minA = projection;
                    if (projection > maxA)
                        maxA = projection;
                }
                for (int j = 0; j < hullB.Length; j++)
                {
                    float projection = hullB[j].Dot(axes[i]);
                    if (projection < minB)
                        minB = projection;
                    if (projection > maxB)
                        maxB = projection;
                }
                if (!(minA <= maxB && maxA >= minB))
                    return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														areaOverlap()
        //------------------------------------------------------------------------------------------------------------------------
        private bool areaOverlap(Vector2[] c, Vector2[] d)
        {

            float dx = c[1].x - c[0].x;
            float dy = c[1].y - c[0].y;
            float lengthSQ = (dy * dy + dx * dx);

            if (lengthSQ == 0.0f) lengthSQ = 1.0f;

            float t, minT, maxT;

            t = ((d[0].x - c[0].x) * dx + (d[0].y - c[0].y) * dy) / lengthSQ;
            maxT = t; minT = t;

            t = ((d[1].x - c[0].x) * dx + (d[1].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            t = ((d[2].x - c[0].x) * dx + (d[2].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            t = ((d[3].x - c[0].x) * dx + (d[3].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            if ((minT >= 1) || (maxT < 0)) return false;

            dx = c[3].x - c[0].x;
            dy = c[3].y - c[0].y;
            lengthSQ = (dy * dy + dx * dx);

            if (lengthSQ == 0.0f) lengthSQ = 1.0f;

            t = ((d[0].x - c[0].x) * dx + (d[0].y - c[0].y) * dy) / lengthSQ;
            maxT = t; minT = t;

            t = ((d[1].x - c[0].x) * dx + (d[1].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            t = ((d[2].x - c[0].x) * dx + (d[2].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            t = ((d[3].x - c[0].x) * dx + (d[3].y - c[0].y) * dy) / lengthSQ;
            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

            if ((minT >= 1) || (maxT < 0)) return false;

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														pointOverlapsArea()
        //------------------------------------------------------------------------------------------------------------------------
        //ie. for hittestpoint and mousedown/up/out/over
        private bool pointOverlapsArea(Vector2 p, Vector2[] c)
        {

            float dx = c[1].x - c[0].x;
            float dy = c[1].y - c[0].y;
            float lengthSQ = (dy * dy + dx * dx);

            float t;

            t = ((p.x - c[0].x) * dx + (p.y - c[0].y) * dy) / lengthSQ;

            if ((t > 1) || (t < 0)) return false;

            dx = c[3].x - c[0].x;
            dy = c[3].y - c[0].y;
            lengthSQ = (dy * dy + dx * dx);

            t = ((p.x - c[0].x) * dx + (p.y - c[0].y) * dy) / lengthSQ;

            if ((t > 1) || (t < 0)) return false;

            return true;
        }
    }
}


