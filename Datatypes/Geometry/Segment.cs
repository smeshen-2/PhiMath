/*using Numbers;

namespace GeometryTools;

public struct Segment
{
    public Vector2 point1 { get; private set; }
    public Vector2 point2 { get; private set; }
    public Line line { get; private set; }
    public Root Length { get { return Vector2.Distance(point1, point2); } }

    public Segment(Vector2 point1, Vector2 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
        line = new Line(point1, point2);
    }

    public Vector2 GetMiddlePoint()
    {
        return (point1 + point2) * 0.5f;
    }

    /// <summary>
    /// Returns a line, perpendicular to the segment and passing through its middle.
    /// </summary>
    public Line GetBisector()
    {
        return Line.GetPerpendicular(line, GetMiddlePoint());
    }

    public static bool AreIntersecting(Segment a, Segment b)
    {
        if (Line.AreParallel(a.line, b.line)) return false;
        Vector2 intersection = Line.GetIntersection(a.line, b.line);
        return a.ContainsPoint(intersection) && b.ContainsPoint(intersection);
    }

    public static bool AreIntersecting(Segment a, Line b)
    {
        if (Line.AreParallel(a.line, b)) return false;
        Vector2 intersection = Line.GetIntersection(a.line, b);
        return a.ContainsPoint(intersection);
    }

    public static Vector2 GetIntersection(Segment a, Line b)
    {
        if (AreIntersecting(a, b) == false) throw new Exception("The two elements do not intersect. Check AreIntersecting() first.");
        return Line.GetIntersection(a.line, b);
    }

    public static Vector2 GetIntersection(Segment a, Segment b)
    {
        if (AreIntersecting(a, b) == false) throw new Exception("The two segments do not intersect. Check AreIntersecting() first.");
        return Line.GetIntersection(a.line, b.line);
    }

    public bool ContainsPoint(Vector2 point)
    {
        return line.ContainsPoint(point) && point.X >= Fraction.Min(point1.X, point2.X) && point.X <= Fraction.Max(point1.X, point2.X);
    }
}
*/