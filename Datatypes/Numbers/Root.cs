﻿namespace Numbers;

/// <summary>
/// A number in the form A√B, where A is a Fraction, B is a whole number, B >= 0.
/// </summary>
public struct Root
{
    /// <summary>
    /// The rational part of the number.
    /// </summary>
    public Fraction A { get; private set; }

    /// <summary>
    /// The irrational part of the number.
    /// </summary>
    public int B { get; private set; }

    public Root(Fraction rational, int irrational = 1)
    {
        if (irrational < 0) throw new Exception("Argument \"b\" cannot be less than zero.");
        A = rational;
        B = irrational;
    }

    /// <summary>
    /// Creates a Root int the form "a√b"
    /// </summary>
    public Root(int a, int b = 1) : this(new Fraction(a), b)
    {
        
    }

    /// <summary>
    /// Creates a Root int the form "p/q√b"
    /// </summary>
    public Root(int p, int q, int b = 1) : this(new Fraction(p, q), b)
    {

    }

    /// <summary>
    /// Creates a Root from a string in the form "p/q√b", "p/q", "a√b", "√b" or "a". "V" can be used instead of "√".
    /// </summary>
    public Root(string s)
    {
        s = s.Replace(" ", "").Replace("V", "√");
        int i = s.IndexOf('√');
        if(i > -1)
        {
            if (i == 0)
            {
                s = "1" + s;
                i = 1;
            }
            A = new Fraction(s.Substring(0, i));
            B = int.Parse(s.Substring(i + 1));
            if (B < 0) throw new Exception("Argument b in a√b cannot be less than zero.");
        }
        else
        {
            A = new Fraction(s);
            B = 1;
        }
    }

    public Root Simplified()
    {
        int sqrt = (int)Math.Sqrt(B);
        for (int i = sqrt; i > 1; i--)
        {
            if(B % (i * i) == 0)
            {
                return new Root(A * i, B / (i * i));
            }
        }
        return new Root(A, B);
    }

    public override string ToString()
    {
        if (A.P * B == 0) return "0";
        if (B == 1) return A.ToString();
        if (A == 1) return "√" + B;
        return (A.P != 1 ? A.P.ToString() : "") + $"√{B}" + (A.Q > 1 ? "/" + A.Q : "");
    }

    public float ToFloat()
    {
        return (float)(A.ToFloat() * Math.Sqrt(B));
    }

    public Root Inverted()
    {
        return new Root(A.Inverted() / B, B);
    }

    public Fraction Squared()
    {
        return A * A * B;
    }

    public static Root operator -(Root a)
    {
        return new Root(-a.A, a.B);
    }

    public static Root operator *(Root a, Root b)
    {
        return new Root(a.A * b.A, a.B * b.B).Simplified();
    }

    public static Root operator /(Root a, Root b)
    {
        return a * b.Inverted();
    }

    public static bool operator ==(Root a, Root b)
    {
        return a.A == b.A && a.B == b.B;
    }

    public static bool operator !=(Root a, Root b)
    {
        return !(a == b);
    }

    public static bool operator >(Root a, Root b)
    {
        return a.Squared() > b.Squared();
    }

    public static bool operator <(Root a, Root b)
    {
        return a.Squared() < b.Squared();
    }

    public static Real operator +(Root a, Root b)
    {
        return new Real(a, b);
    }

    public static Real operator -(Root a, Root b)
    {
        return new Real(a, -b);
    }

    public static implicit operator Root(string s)
    {
        return new Root(s);
    }
}