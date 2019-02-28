using System;
using System.Collections.Generic;
using System.Linq;
using GLXEngine.Core;

namespace GLXEngine
{
    public class QuadTree
    {
        public struct Point
        {
            public Vector2 position;
            public object data;

            public float x { get { return position.x; } set { position.x = value; } }
            public float y { get { return position.y; } set { position.y = value; } }

            Point(Vector2 a_pos, object a_data)
            {
                position = a_pos;
                data = a_data;
            }
        }

        List<Point> m_points;

        Rectangle m_boundary;
        int m_capacity;
        bool m_divided;

        public int Count()
        {
            int count = m_points.Count;
            if (m_divided)
            {
                count += m_northeast.Count();
                count += m_northwest.Count();
                count += m_southeast.Count();
                count += m_southwest.Count();
            }
            return count;

        }

        QuadTree m_northeast;
        QuadTree m_northwest;
        QuadTree m_southeast;
        QuadTree m_southwest;

        public QuadTree(Rectangle a_boundary, int a_capacity)
        {
            m_capacity = a_capacity;
            m_boundary = a_boundary;

            m_points = new List<Point>();
            m_divided = false;
        }

        public void SubDivide()
        {
            float w = m_boundary.width / 2;
            float h = m_boundary.height / 2;
            float x = m_boundary.x + w;
            float y = m_boundary.y + h;

            Rectangle ne = new Rectangle(x + w, y - h, w, h);
            m_northeast = new QuadTree(ne, m_capacity);
            Rectangle nw = new Rectangle(x - w, y - h, w, h);
            m_northwest = new QuadTree(nw, m_capacity);
            Rectangle se = new Rectangle(x + w, y + h, w, h);
            m_southeast = new QuadTree(se, m_capacity);
            Rectangle sw = new Rectangle(x - w, y + h, w, h);
            m_southwest = new QuadTree(sw, m_capacity);

            m_divided = true;
        }

        public bool Insert(Point a_point)
        {
            if (!m_boundary.Contains(a_point.position))
            {
                return false;
            }

            if (m_points.Count < m_capacity)
            {
                m_points.AddRange(m_points);
                return true;
            }

            if (!m_divided)
            {
                SubDivide();
            }

            return (m_northeast.Insert(a_point) || m_northwest.Insert(a_point) ||
              m_southeast.Insert(a_point) || m_southwest.Insert(a_point));
        }

        public List<Point> Query(Shape range, ref List<Point> found)
        {
            if (!range.Overlaps(m_boundary))
            {
                return found;
            }

            foreach (Point point in m_points)
            {
                if (range.Contains(point.position))
                {
                    found.Add(point);
                }
            }
            if (m_divided)
            {
                m_northwest.Query(range, ref found);
                m_northeast.Query(range, ref found);
                m_southwest.Query(range, ref found);
                m_southeast.Query(range, ref found);
            }

            return found;
        }

        public List<Point> closest(Point a_point, int a_count = 1, float a_startingSize = 1)
        {
            // Limit to number of points in this QuadTree
            if (Count() == 0)
            {
                return new List<Point>();
            }
            if (Count() < a_count)
            {
                return m_points;
            }

            // optimized, expanding binary search
            // start with a small circle, rapidly expand, slowly shrink
            float radius = a_startingSize;
            float limit = 16;
            while (true)
            {
                Circle range = new Circle(a_point.x, a_point.y, radius);
                List<Point> points = new List<Point>();
                points = Query(range, ref points);
                if (points.Count == a_count)
                {
                    return points; // Return when we hit the right size
                }
                else if (points.Count < a_count)
                {
                    radius *= 2;
                }
                else if (limit-- <= 0)
                {
                    return new List<Point>(points.Take(a_count));
                }
                else
                {
                    radius /= 3;
                }
            }
        }

        public void ForEach(Action<Point> fn)
        {
            m_points.ForEach(fn);
            if (m_divided)
            {
                m_northeast.ForEach(fn);
                m_northwest.ForEach(fn);
                m_southeast.ForEach(fn);
                m_southwest.ForEach(fn);
            }
        }


    }
}