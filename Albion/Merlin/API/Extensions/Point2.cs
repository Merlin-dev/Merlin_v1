namespace Merlin.API.Direct
{
    public partial struct Point2
    {
        public static float Distance(Point2 a, Point2 b)
        {
            return ajg.c(a, b).k();
        }

        public static float Distance(Point3 a, Point3 b)
        {
            return Distance(a.ToPoint2(), b.ToPoint2());
        }
    }
}
