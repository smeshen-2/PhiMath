/*using Numbers;

namespace GeometryTools;

public struct Vector2
{
    public Fraction X { get; private set; }
    public Fraction Y { get; private set; }

    public Root Length { get { return Fraction.Sqrt(X * X + Y * Y); } }

    public Vector2(Fraction x, Fraction y)
    {
        X = x;
        Y = y;
    }

    public Vector2(float x, float y)
    {
        X = new Fraction(x);
        Y = new Fraction(y);
    }

    public static bool operator == (Vector2 a, Vector2 b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator != (Vector2 a, Vector2 b)
    {
        return !(a == b);
    }

    public static Vector2 operator + (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 operator - (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2 operator *(Vector2 a, Fraction k)
    {
        return new Vector2(a.X * k, a.Y * k);
    }

    public static Vector2 operator * (Vector2 a, float k)
    {
        return a * new Fraction(k);
    }

    public static Vector2 operator /(Vector2 a, Fraction k)
    {
        return new Vector2(a.X / k, a.Y / k);
    }

    public static Vector2 operator /(Vector2 a, float k)
    {
        return a / new Fraction(k);
    }

    public static Root Distance(Vector2 a, Vector2 b)
    {
        return (b - a).Length;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public Vector2 ToVector()
    {
        return new Vector2(X, Y);
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float x)
    {
        return a + (b - a) * x;
    }

    /// <summary>
    /// Returns the dot product of two vectors.
    /// </summary>
    public static Fraction Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    /// <summary>
    /// Returns the length of the cross product of two vectors.
    /// </summary>
    public static Fraction Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - b.X * a.Y;
    }
}
*/