﻿namespace Numbers;

/// <summary>
/// A real number.
/// </summary>
public class Real
{
    public List<Root> Roots { get; private set; } = new List<Root>();

    public Real(params Root[] roots)
    {
        Roots = roots.ToList();
    }

    public Real(Root root)
    {
        Roots = new List<Root>() { root };
    }

    /// <summary>
    /// Creates a Real number from a string in the form "R0 + R1 + R2 + ... + Rn", where R0, R1, ..., Rn are valid string representations of roots.
    /// </summary>
    public Real(string s)
    {
        int i = s.IndexOf('+');
        if(i > 0)
        {
            s = s.Replace("-", "+-");
            Roots = s.Split('+').Select(r => new Root(r)).ToList();
        }
        else
        {
            Roots = new List<Root>() { new Root(s) };
        }
    }

    public void Simplify()
    {
        Roots = Roots.Select(r => r.Simplified()).OrderByDescending(k => k.B).ToList();
        var result = new Root?[Roots.Count];
        result[0] = Roots[0];
        int currentB = Roots[0].B;
        int currentIndex = 0;
        for (int i = 1; i < Roots.Count; i++)
        {
            if (currentB != Roots[i].B)
            {
                currentB = Roots[i].B;
                currentIndex++;
                result[currentIndex] = Roots[i];
            }
            else
            {
                result[currentIndex] = new Root(((Root)result[currentIndex]).A + Roots[i].A, currentB);
            }
        }
        Roots = new List<Root>();
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] is null) break;
            Root r = (Root)result[i];
            if (r.A * r.B != 0) Roots.Add(r);
        }
    }

    public static Real operator +(Real a, Root b)
    {
        a.Roots.Add(b);
        return a;
    }

    public static Real operator -(Real a, Root b)
    {
        a.Roots.Add(-b);
        return a;
    }

    public static Real operator *(Real a, Root b)
    {
        var roots = a.Roots.ToArray();
        for (int i = 0; i < roots.Length; i++)
        {
            roots[i] *= b;
        }
        a.Roots = roots.ToList();
        return a;
    }

    public static Real operator *(Real a, Real b)
    {
        int x = a.Roots.Count, y = b.Roots.Count, k = 0;
        var roots = new Root[x * y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                roots[k] = a.Roots[i] * b.Roots[j];
                k++;
            }
        }
        return new Real(roots);
    }

    public override string ToString()
    {
        return string.Join("+", Roots).Replace("+-", "-");
    }

    public static implicit operator Real(string s)
    {
        return new Real(s);
    }
}