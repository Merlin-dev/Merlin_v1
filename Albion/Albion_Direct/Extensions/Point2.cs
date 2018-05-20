namespace Albion_Direct
{
    public partial struct Point2
    {
        public static float Distance(Point2 a, Point2 b)
        {
            return ao6.c(a, b).c();
        }

        public float GetX()
        {
            return _internal.e;
        }

        public float GetY()
        {
            return _internal.f;
        }
    }
}