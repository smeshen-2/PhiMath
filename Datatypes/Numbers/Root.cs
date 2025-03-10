﻿using System.Text.RegularExpressions;

namespace Numbers;

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
    /// Creates a Root in the form "a√b"
    /// </summary>
    public Root(int a, int b = 1) : this(new Fraction(a), b)
    {

    }

    /// <summary>
    /// Creates a Root in the form "p√b/q"
    /// </summary>
    public Root(int p, int b, int q) : this(new Fraction(p, q), b)
    {

    }

    /// <summary>
    /// Creates a Root from a string in the form "p√b/q", "√b/q", "p/q", "a√b", "√b" or "a". "V" can be used instead of "√".
    /// </summary>
    public Root(string s)
    {
        s = s.Replace(" ", "").Replace("V", "√");
        Match match = Regex.Match(s, @"(-?\d+)√(\d+)\/(\d+)");
        if (match.Success)
        {
            A = new Fraction(int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[3].Value));
            B = int.Parse(match.Groups[2].Value);
            return;
        }
        match = Regex.Match(s, @"^-?√(\d+)\/(\d+)$");
        if (match.Success)
        {
            A = new Fraction(s[0] == '-' ? -1 : 1, int.Parse(match.Groups[2].Value));
            B = int.Parse(match.Groups[1].Value);
            return;
        }
        match = Regex.Match(s, @"^-?\d+\/\d+$");
        if (match.Success)
        {
            A = Fraction.Parse(s);
            B = 1;
            return;
        }
        match = Regex.Match(s, @"^(-?\d+)√(\d+)$");
        if (match.Success)
        {
            A = new Fraction(int.Parse(match.Groups[1].Value));
            B = int.Parse(match.Groups[2].Value);
            return;
        }
        match = Regex.Match(s, @"^-?√(\d+)$");
        if (match.Success)
        {
            A = new Fraction(s[0] == '-' ? -1 : 1, 1);
            B = int.Parse(match.Groups[1].Value);
            return;
        }
        match = Regex.Match(s, @"^-?\d+$");
        if (match.Success)
        {
            A = Fraction.Parse(match.Value);
            B = 1;
            return;
        }
        throw new Exception("Invalid string format");
    }

    public Root Simplified()
    {
        int sqrt = (int)Math.Sqrt(B);
        for (int i = sqrt; i > 1; i--)
        {
            if (B % (i * i) == 0)
            {
                return new Root(A * i, B / (i * i));
            }
        }
        return new Root(A.Normalize(), B);
    }

    public override string ToString()
    {
        if (A.P * B == 0) return "0";
        if (B == 1) return A.ToString();
        if (A == 1) return "√" + B;
        if (A == -1) return "-√" + B;
        return (A.P != 1 ? A.P.ToString() : "") + $"√{B}" + (A.Q > 1 ? "/" + A.Q : "");
    }

    public float ToFloat()
    {
        return (float)(A * Math.Sqrt(B));
    }

    public Root Inverted()
    {
        return new Root(A.Invert() / B, B);
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
        return (a * b.Inverted()).Simplified();
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

    public static bool operator >=(Root a, Root b)
    {
        return !(a < b);
    }

    public static bool operator <=(Root a, Root b)
    {
        return !(a > b);
    }

    public static RootSum operator +(Root a, Root b)
    {
        return new RootSum(a, b);
    }

    public static RootSum operator -(Root a, Root b)
    {
        return new RootSum(a, -b);
    }

    public static implicit operator Root(string s)
    {
        return new Root(s);
    }
}
