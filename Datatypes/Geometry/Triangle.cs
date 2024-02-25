/*using Numbers;

namespace GeometryTools;

public struct Triangle
{
    private Vector2 A, B, C;
    private Line a, b, c;

    public Segment AB { get { return new Segment(A, B); } }
    public Segment BC { get { return new Segment(B, C); } }
    public Segment AC { get { return new Segment(A, C); } }

    public Triangle(Vector2 A, Vector2 B, Vector2 C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
        a = new Line(B, C);
        b = new Line(A, C);
        c = new Line(A, B);
    }

    public Triangle(Line a, Line b, Line c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        A = Line.GetIntersection(b, c);
        B = Line.GetIntersection(a, c);
        C = Line.GetIntersection(a, b);
    }

    public Vector2 GetMedicenter()
    {
        return (A + B + C) / 3;
    }

    public bool ContainsPoint(Vector2 point)
    {
       return SameSide(point, A, B, C) && SameSide(point, B, C, A) && SameSide(point, C, A, B);
    }

    private bool SameSide(Vector2 p1, Vector2 p2, Vector2 a, Vector2 b)
    {
        var cross1 = Vector2.Cross(b - a, p1 - a);
        var cross2 = Vector2.Cross(b - a, p2 - a);
        return cross1 * cross2 >= 0;
    }
}
*/