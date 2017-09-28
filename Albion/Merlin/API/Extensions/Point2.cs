namespace Merlin.API.Direct
{
    public partial struct Point2
    {
        public static float Distance(Point2 a, Point2 b)
        {
            return akp.c(a, b).k();
        }
    }
}