/*using Numbers;

namespace GeometryTools;

public struct Rectangle
{
    public Vector2 StartPoint { get; private set; }
    public Vector2 EndPoint { get; private set; }

    public Fraction Width { get { return Fraction.Abs(StartPoint.X - EndPoint.X); } }
    public Fraction Height { get { return Fraction.Abs(StartPoint.Y - EndPoint.Y); } }
    public Fraction Perimeter { get { return (Width + Height) * 2; } }
    public Fraction S { get { return Width * Height; } }
    public Segment Top
    {
        get
        {
            Fraction maxY = Fraction.Max(StartPoint.Y, EndPoint.Y);
            return new Segment(new Vector2(StartPoint.X, maxY), new Vector2(EndPoint.X, maxY));
        }
    }
    public Segment Bottom
    {
        get
        {
            Fraction minY = Fraction.Min(StartPoint.Y, EndPoint.Y);
            return new Segment(new Vector2(StartPoint.X, minY), new Vector2(EndPoint.X, minY));
        }
    }
    public Segment Left
    {
        get
        {
            Fraction minX = Fraction.Min(StartPoint.X, EndPoint.X);
            return new Segment(new Vector2(minX, StartPoint.Y), new Vector2(minX, EndPoint.Y));
        }
    }
    public Segment Right
    {
        get
        {
            Fraction maxX = Fraction.Max(StartPoint.X, EndPoint.X);
            return new Segment(new Vector2(maxX, StartPoint.Y), new Vector2(maxX, EndPoint.Y));
        }
    }
    public Vector2 TopLeft { get { return Segment.GetIntersection(Top, Left); } }
    public Vector2 TopRight { get { return Segment.GetIntersection(Top, Right); } }
    public Vector2 BottomLeft { get { return Segment.GetIntersection(Bottom, Left); } }
    public Vector2 BottomRight { get { return Segment.GetIntersection(Bottom, Right); } }

    public Rectangle(Vector2 startPoint, Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public Rectangle(Vector2 bottomLeftPoint, Fraction width, Fraction height)
    {
        this = new Rectangle(bottomLeftPoint, bottomLeftPoint + new Vector2(width, height));
    }

    public bool ContainsPoint(Vector2 point)
    {
        return point.X >= Fraction.Min(StartPoint.X, EndPoint.X)
            && point.X <= Fraction.Max(StartPoint.X, EndPoint.X)
            && point.Y >= Fraction.Min(StartPoint.Y, EndPoint.Y)
            && point.Y <= Fraction.Max(StartPoint.Y, EndPoint.Y);
    }

    public static List<Vector2> GetIntersection(Rectangle r, Line l)
    {
        var ans = new List<Vector2>();
        Segment[] sides = { r.Top, r.Bottom, r.Left, r.Right };
        foreach (var side in sides)
        {
            if (Segment.AreIntersecting(side, l)) ans.Add(Segment.GetIntersection(side, l));
        }
        return ans;
    }
}
*/