using Numbers;

namespace GeometryTools;

public struct Line
{
    public Fraction A { get; private set; }
    public Fraction B { get; private set; }
    public Fraction C { get; private set; }

    public Fraction Angle { get { return -A / B; } }
    public bool IsHorizontal { get { return A == 0; } }
    public bool IsVertical { get { return B == 0; } }

    public static Line Ox { get { return new Line(0, 1, 0); } }
    public static Line Oy { get { return new Line(1, 0, 0); } }

    public Line(Vector2 point1, Vector2 point2)
    {
        A = point2.Y - point1.Y;
        B = point1.X - point2.X;
        C = - A * point1.X - B * point1.Y;
    }

    public Line(Vector2 point, Line parallel)
    {
        A = parallel.A;
        B = parallel.B;
        C = -A * point.X - B * point.Y;
    }

    public Line(Fraction A, Fraction B, Fraction C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
    }

    public Line(float A, float B, float C)
    {
        this.A = new Fraction(A);
        this.B = new Fraction(B);
        this.C = new Fraction(C);
    }

    /// <summary>
    /// Returns a copy of the line with whole numbers for A, B and C; A >= 0.
    /// </summary>
    public Line Normalized()
    {
        int NOK = Fraction.NOK(A.Q, B.Q, C.Q);
        Line copy = new Line(A.P * NOK / A.Q, B.P * NOK / B.Q, C.P * NOK / C.Q);
        if(copy.A.P < 0)
        {
            copy.A *= -1;
            copy.B *= -1;
            copy.C *= -1;
        }
        return copy;
    }

    public override string ToString()
    {
        string s = $"{A}x+{B}y+{C}=0";
        return s.Replace("+-", "-");
    }

    public static bool AreParallel(Line line1, Line line2)
    {
        return line1.A * line2.B == line1.B * line2.A;
    }

    public static bool operator == (Line line1, Line line2)
    {
        return AreParallel(line1, line2) && (line1.B * line2.C == line1.C * line2.B);
    }

    public static bool operator != (Line line1, Line line2)
    {
        return !(line1 == line2);
    }

    public static Vector2 GetIntersection(Line line1, Line line2)
    {
        if (AreParallel(line1, line2)) throw new Exception("Parallel lines do not have an intersection point.");
        return new Vector2((line1.B * line2.C - line2.B * line1.C) / (line1.A * line2.B - line2.A * line1.B),
            (line1.C * line2.A - line2.C * line1.A) / (line1.A * line2.B - line2.A * line1.B));
    }

    public static double GetAngle(Line line1, Line line2)
    {
        double result = Math.Atan(((line2.A * line1.B - line1.A * line2.B) / (line1.A * line2.A + line1.B * line2.B)).ToFloat()) * 180 / Math.PI;
        return Math.Abs(result);
    }

    /// <summary>
    /// Returns a new Line through a point and perpendicular to the specified line.
    /// </summary>
    public static Line GetPerpendicular(Line line, Vector2 point)
    {
        return new Line(point, new Line(-line.B, line.A, new Fraction(0)));
    }

    public bool ContainsPoint(Vector2 point)
    {
        return A * point.X + B * point.Y + C == 0;
    }
}
