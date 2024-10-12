using System.Text.RegularExpressions;

namespace Numbers;

/// <summary>
/// A number in the form A0√B0 + A1√B1 + A2√B2...
/// </summary>
public class RootSum
{
    public List<Root> Roots { get; private set; } = new List<Root>();

    public RootSum(params Root[] roots)
    {
        Roots = roots.ToList();
    }

    public RootSum(Root root)
    {
        Roots = new List<Root>() { root };
    }

    /// <summary>
    /// Creates a number from a string in the form "R0 + R1 + R2 + ... + Rn", where R0, R1, ..., Rn are valid string representations of Roots.
    /// </summary>
    public RootSum(string s)
    {
        s = s.Replace(" ", "");
        if (s[0] == '-') s = "0" + s;
        s = s.Replace("-", "+-");
        Roots = s.Split('+').Select(r => new Root(r)).ToList();
    }

    public void Simplify()
    {
        var rootMap = new Dictionary<int, Fraction>();
        foreach (var root in Roots)
        {
            Root simplified = root.Simplified();
            if (simplified.A * simplified.B == 0) continue;
            if(rootMap.ContainsKey(simplified.B))
            {
                rootMap[simplified.B] += simplified.A;
                if (rootMap[simplified.B] == Fraction.Parse("0")) rootMap.Remove(simplified.B);
            }
            else
            {
                rootMap.Add(simplified.B, simplified.A);
            }    
        }
        Roots = rootMap.Select(pair => new Root(pair.Value, pair.Key)).ToList();
    }
    
    public static RootSum operator +(RootSum a, RootSum b)
    {
        RootSum result = new RootSum();
        result.Roots.AddRange(a.Roots);
        result.Roots.AddRange(b.Roots);
        result.Simplify();
        return result;
    }

    public static RootSum operator +(RootSum a, Root b)
    {
        RootSum result = new RootSum();
        result.Roots.AddRange(a.Roots);
        result.Roots.Add(b);
        result.Simplify();
        return result;
    }

    public static RootSum operator -(RootSum real)
    {
        return new RootSum(real.Roots.Select(r => -r).ToArray());
    }

    public static RootSum operator -(RootSum a, RootSum b)
    {
        return a + (-b);
    }

    public static RootSum operator -(RootSum a, Root b)
    {
        return a + (-b);
    }

    public static RootSum operator *(RootSum a, Root b)
    {
        RootSum result = new RootSum();
        result.Roots.AddRange(a.Roots.Select(r => r * b));
        return result;
    }

    public static RootSum operator *(RootSum a, RootSum b)
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
        RootSum result = new RootSum(roots);
        result.Simplify();
        return result;
    }

    public RootSum Squared()
    {
        return this * this;
    }

    public RootSum Copy()
    {
        var roots = new List<Root>();
        roots.AddRange(Roots);
        return new RootSum(roots.ToArray());
    }

    public static RootSum operator /(RootSum a, Root b)
    {
        RootSum result = new RootSum(a.Roots.Select(r => r / b).ToArray());
        result.Simplify();
        return result;
    }

    public static string operator /(RootSum a, RootSum b)
    {
        if (b.Roots.Count == 1)
        {
            var real = a / b.Roots[0];
            real.Simplify();
            return real.ToString();
        }
        var allRoots = new List<Root>();
        allRoots.AddRange(a.Roots);
        allRoots.AddRange(b.Roots);
        int nod = Fraction.NOD(allRoots.Select(r => r.A.P).ToArray());
        string result = (a.Copy() / new Root(nod, 1)).ToString()
            + "/(" + (b.Copy() / new Root(nod, 1)).ToString() + ")";
        return Regex.Replace(result, @"\((\d+)\)", m => m.Groups[1].Value);
    }

    public override string ToString()
    {
        if (Roots.Count == 0) return "0";
        return string.Join("+", Roots).Replace("+-", "-");
    }

    public static implicit operator RootSum(string s)
    {
        return new RootSum(s);
    }
}
