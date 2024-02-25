/*using Numbers;

namespace GeometryTools;

public struct Circle
{
    public Vector2 Center { get; private set; }
    public Root Radius { get; private set; }

    public Circle(Vector2 center, Root radius)
    {
        Center = center;
        Radius = radius;
    }

    public Circle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        Line s1 = new Segment(point1, point2).GetBisector();
        Line s2 = new Segment(point2, point3).GetBisector();
        Center = Line.GetIntersection(s1, s2);
        Radius = Vector2.Distance(Center, point1).Simplified();
    }

    public override string ToString()
    {
        return $"(x:{Center.X}, y:{Center.Y}, r:{Radius})";
    }

    /// <summary>
    /// Returns true if the point is inside the circle.
    /// </summary>
    public bool ContainsPoint(Vector2 point)
    {
        return Vector2.Distance(point, Center) < Radius;
    }
}
*/