using System.Text.RegularExpressions;

namespace Nomials;

public class Monomial
{
    public double Coefficient { get; set; }
    public int Power { get; set; }

    public Monomial(double coefficient, int power)
    {
        Coefficient = coefficient;
        Power = power;
    }

    public Monomial Copy()
    {
        return new Monomial(Coefficient, Power);
    }

    public override string ToString()
    {
        if (Power == 0) return Coefficient.ToString();
        if (Coefficient == 0) return "0";

        string res = "";
        if (Coefficient == 1) res += "x";
        else if (Coefficient == -1) res += "-x";
        else res += Coefficient + "x";

        if (Power == 1) return res;
        if (Power < 0) return res + $"^({Power})";
        return res + $"{ToSuperscript(Power)}";
    }

    public static string? ToSuperscript(string s) => ToSuperscript(int.Parse(s));
    public static string? ToSuperscript(int n)
    {
        if (n < 0) return null;
        string s = n.ToString();
        s = s.Replace('1', '¹').Replace('2', '²').Replace('3', '³');
        foreach (char c in s)
            if (c == '0' || (c >= '4' && c <= '9'))
                s = s.Replace(c, (char)(c + 8256));
        return s;
    }
    public static int? FromSuperscript(string s)
    {
        foreach (char c in s)
        {
            if (!"⁰¹²³⁴⁵⁶⁷⁸⁹".Contains(c)) return null;
            if (c == '⁰' || (c >= '⁴' && c <= '⁹'))
                s = s.Replace(c, (char)(c - 8256));
        }
        s = s.Replace('¹', '1').Replace('²', '2').Replace('³', '3');
        return int.Parse(s);
    }

    public static Monomial Parse(string s)
    {
        foreach (char c in s)
        {
            if (!"^-x,._0123456789⁰¹²³⁴⁵⁶⁷⁸⁹ ".Contains(c)) throw new ArgumentException();
        }
        double coeff = 1;
        int power = 1;
        s.Replace(" ", "");
        if (!s.Contains('x')) return new Monomial(double.Parse(s), 0);
        if (s[0] == '-')
        {
            coeff = -1;
            s = s.Remove(0, 1);
        }
        if (!(s[0] == 'x'))
            coeff *= double.Parse(s.Split('x')[0]);
        if (s.Contains('^'))
        {
            string pow = s.Split('x')[1];
            pow = pow.Substring(1, pow.Length - 1);
            if (Regex.Match(pow, @"\(-?\d+\)").Success) pow = pow.Substring(1, pow.Length - 2);
            power = int.Parse(pow);
        }
        else if (s.Split('x')[1] != "") power = FromSuperscript(s.Split('x')[1]) ?? throw new ArgumentException();
        return new Monomial(coeff, power);
    }

    public double WhereXEquals(double x)
    {
        return Coefficient * Math.Pow(x, Power);
    }

    public static Polynomial operator +(Monomial m1, Monomial m2)
    {
        return new Polynomial(new List<Monomial>() { m1, m2 });
    }
    public static Polynomial operator -(Monomial m1, Monomial m2)
    {
        return new Polynomial(new List<Monomial>() { m1, new Monomial(-m1.Coefficient, m1.Power) });
    }
    public static Monomial operator *(Monomial m1, Monomial m2)
    {
        return new Monomial(m1.Coefficient * m2.Coefficient, m1.Power + m2.Power);
    }
    public static Monomial operator /(Monomial m1, Monomial m2)
    {
        return new Monomial(m1.Coefficient / m2.Coefficient, m1.Power - m2.Power);
    }
}