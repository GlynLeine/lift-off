
namespace GLXEngine.Core
{
    public abstract class Shape
    {
        public Vector2 position = new Vector2();

        public float x { get { return position.x; } set { position.x = value; } }
        public float y { get { return position.y; } set { position.y = value; } }

        public abstract bool Contains(Vector2 a_point);

        public abstract bool Overlaps(Rectangle a_other);
    }
}
