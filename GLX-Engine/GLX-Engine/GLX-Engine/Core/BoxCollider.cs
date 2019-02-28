using System;
using System.Collections.Generic;

namespace GLXEngine.Core
{
    public class BoxCollider : Collider
    {
        private BoundsObject m_owner;
        private List<Type> _ignore;
        //public EasyDraw m_canvas;

        //------------------------------------------------------------------------------------------------------------------------
        //														BoxCollider()
        //------------------------------------------------------------------------------------------------------------------------		
        public BoxCollider(BoundsObject owner, Type[] ignore = null)
        {
            m_owner = owner;
            if (ignore != null)
                _ignore = new List<Type>(ignore);
            //m_canvas = a_canvas;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool HitTest(ref Collider other)
        {
            if (other is BoxCollider)
            {
                BoxCollider otherCollider = (other as BoxCollider);

                if (m_owner.parent == null)
                    return false;

                if (otherCollider.m_owner.parent == null)
                    return false;

                if (_ignore != null)
                    if (_ignore.Contains(otherCollider.m_owner.parent.GetType()))
                        return false;

                if (otherCollider._ignore != null)
                    if (otherCollider._ignore.Contains(m_owner.parent.GetType()))
                        return false;

                Vector2[] extendsA = m_owner.GetExtents();
                if (extendsA == null) return false;

                Vector2[] extendsB = otherCollider.m_owner.GetExtents();
                if (extendsB == null) return false;

                Vector2 positionA = m_owner.screenPosition;
                Vector2[] hullA = m_owner.GetHull();

                Vector2 positionB = otherCollider.m_owner.screenPosition;
                Vector2[] hullB = otherCollider.m_owner.GetHull();

                float extendA = 0, extendB = 0;

                foreach (Vector2 point in hullA)
                {
                    if (Mathf.Abs(point.x) > extendA)
                        extendA = point.x;
                    if (Mathf.Abs(point.y) > extendA)
                        extendA = point.y;
                }

                foreach (Vector2 point in hullB)
                {
                    if (Mathf.Abs(point.x) > extendB)
                        extendB = point.x;
                    if (Mathf.Abs(point.y) > extendB)
                        extendB = point.y;
                }

                Vector2 velocityA = m_owner.parent.m_velocity * Time.deltaTime / 1000f;
                Vector2 velocityB = otherCollider.m_owner.parent.m_velocity * Time.deltaTime / 1000f;
                float speedAsqr = velocityA.sqrMagnitude;
                float speedBsqr = velocityB.sqrMagnitude;

                //if (m_canvas != null)
                //{
                //    m_canvas.Quad(extendsA[0].x, extendsA[0].y, extendsA[1].x, extendsA[1].y, extendsA[2].x, extendsA[2].y, extendsA[3].x, extendsA[3].y);

                //    m_canvas.Fill(255, 0, 0);
                //    m_canvas.Ellipse(positionA.x, positionA.y, 10, 10);
                //    m_canvas.Fill(0);
                //}

                if (CircleBroadPhase(hullA, positionA, velocityA, extendA, hullB, positionB, velocityB, extendB))
                {
                    //if (speedAsqr >= extendB * extendB && speedBsqr >= extendA * extendA)
                    //{
                    //    if (LSINarrowPhase(hullA, positionA, velocityA, hullB, positionB))
                    //        return true;
                    //    if (LSINarrowPhase(hullB, positionB, velocityB, hullA, positionA))
                    //        return true;
                    //}
                    //else if (speedAsqr >= extendB * extendB)
                    //{
                    //    if (LSINarrowPhase(hullA, positionA, velocityA, hullB, positionB))
                    //        return true;
                    //}
                    //else
                    //{
                    //    if (LSINarrowPhase(hullB, positionB, velocityB, hullA, positionA))
                    //        return true;
                    //}

                    if (SATNarrowPhase(extendsA, positionA, extendsB, positionB, ref other))
                        return true;
                }
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
            Vector2[] c = m_owner.GetExtents();
            if (c == null) return false;
            Vector2 p = new Vector2(x, y);
            return pointOverlapsArea(p, c);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														CircleBroadPhase()
        //------------------------------------------------------------------------------------------------------------------------
        private bool CircleBroadPhase(Vector2[] hullA, Vector2 positionA, Vector2 velocityA, float extendA, Vector2[] hullB, Vector2 positionB, Vector2 velocityB, float extendB)
        {
            float deltaTime = Time.deltaTime / 1000f;
            float radiusA = velocityA.magnitude * deltaTime * 0.5f + extendA;
            float radiusB = velocityB.magnitude * deltaTime * 0.5f + extendB;

            Vector2 midPointA = positionA - velocityA * deltaTime * 0.5f;
            Vector2 midPointB = positionB - velocityB * deltaTime * 0.5f;
            return Vector2.Distance(midPointA, midPointB) <= (radiusA + radiusB);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														LSINarrowPhase()
        //------------------------------------------------------------------------------------------------------------------------
        private bool LSINarrowPhase(Vector2[] hullA, Vector2 positionA, Vector2 velocityA, Vector2[] hullB, Vector2 positionB)
        {
            Vector2 closestPoint = null;
            foreach (Vector2 point in hullA)
                if (closestPoint == null)
                    closestPoint = point;
                else if (Vector2.Distance(point, positionB) < Vector2.Distance(closestPoint, positionB))
                    closestPoint = point;

            Vector2 lineEnd = closestPoint + velocityA;



            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SATNarrowPhase()
        //------------------------------------------------------------------------------------------------------------------------
        private bool SATNarrowPhase(Vector2[] hullA, Vector2 positionA, Vector2[] hullB, Vector2 positionB, ref Collider other)
        {
            List<Vector2> axes = new List<Vector2>();

            #region Get Axes
            for (int i = 0; i < hullA.Length; i++)
            {
                Vector2 a = hullA[i];
                Vector2 b = hullA[(i + 1) % hullA.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);

                //if (m_canvas != null)
                //{
                //    m_canvas.Stroke(0, 255, 0);
                //    m_canvas.Line(positionA.x, positionA.y, positionA.x + normal.x * 50, positionA.y + normal.y * 50);
                //    m_canvas.Stroke(0);
                //}

                if (!axes.Contains(normal) && !axes.Contains(-normal))
                    axes.Add(normal);
            }
            for (int i = 0; i < hullB.Length; i++)
            {
                Vector2 a = hullB[i];
                Vector2 b = hullB[(i + 1) % hullB.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);

                if (!axes.Contains(normal) && !axes.Contains(-normal))
                    axes.Add(normal.Normalize());
            }
            #endregion

            Vector2 mtv = new Vector2(float.MaxValue, float.MaxValue);

            for (int i = 0; i < axes.Count; i++)
            {
                float minA = float.MaxValue, maxA = float.MinValue, minB = float.MaxValue, maxB = float.MinValue;
                for (int j = 0; j < hullA.Length; j++)
                {
                    float projection = (hullA[j]).Dot(axes[i]);
                    if (projection < minA)
                        minA = projection;
                    if (projection > maxA)
                        maxA = projection;
                }
                for (int j = 0; j < hullB.Length; j++)
                {
                    float projection = (hullB[j]).Dot(axes[i]);
                    if (projection < minB)
                        minB = projection;
                    if (projection > maxB)
                        maxB = projection;
                }

                if (!(minA <= maxB && maxA >= minB))
                {
                    m_minimumTranslationVec = new Vector2();
                    return false;
                }

                float overlap = maxA < maxB ? -(maxA - minB) : (maxB - minA);
                if (Mathf.Abs(overlap) < mtv.magnitude)
                {
                    mtv = axes[i] * overlap;
                }

            }

            m_minimumTranslationVec = (m_minimumTranslationVec + mtv) / 2;

            //if (m_canvas != null)
            //{
            //    m_canvas.Stroke(0, 0, 255);
            //    m_canvas.Line(positionA.x, positionA.y, positionA.x + mtv.x, positionA.y + mtv.y);
            //    m_canvas.Stroke(0);
            //}

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


