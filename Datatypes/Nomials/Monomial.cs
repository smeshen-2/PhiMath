using System.Text.RegularExpressions;
using Numbers;

namespace Nomials;

public struct Monomial
{
    public Fraction Coefficient { get; set; }
    public int Power { get; set; }

    public Monomial(Fraction coefficient, int power = 0)
    {
        Coefficient = coefficient;
        Power = power;
    }
    public Monomial(double coefficient, int power = 0)
    {
        Coefficient = Fraction.Parse(coefficient);
        Power = power;
    }
    public Monomial(int coefficient, int power = 0)
    {
        Coefficient = Fraction.Parse(coefficient);
        Power = power;
    }

    public override string ToString()
    {
        if (Power == 0) return Coefficient.ToString();
        if (Coefficient == 0) return "0";

        string res = "";
        if (Coefficient == 1) res += "x";
        else if (Coefficient == -1) res += "-x";
        else if (Coefficient.Q == 1) res += Coefficient + "x";
        else res += "(" + Coefficient + ")x";

        if (Power == 1) return res;
        if (Power < 0) return res + $"^({Power})";
        return res + $"{ToSuperscript(Power)}";
    }

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
        s = s.Replace(" ", "").Replace("(", "").Replace(")", "");
        foreach (char c in s)
        {
            if (!"/^-x,._0123456789⁰¹²³⁴⁵⁶⁷⁸⁹".Contains(c)) throw new ArgumentException();
        }
        Fraction coeff = new Fraction(1, 1);
        int power = 1;
        if (!s.Contains('x')) return new Monomial(Fraction.Parse(s));
        if (s[0] == '-')
        {
            coeff *= -1;
            s = s.Remove(0, 1);
        }
        if (!(s[0] == 'x'))
            coeff *= Fraction.Parse(s.Split('x')[0]);
        if (s.Contains('^'))
        {
            string pow = s.Split('x')[1];
            pow = pow.Substring(1, pow.Length - 1);
            if (Regex.IsMatch(pow, @"\(.*\)")) pow = pow.Substring(1, pow.Length - 2);
            power = int.Parse(pow);
        }
        else if (s.Split('x')[1] != "") power = FromSuperscript(s.Split('x')[1]) ?? throw new ArgumentException();
        return new Monomial(coeff, power);
    }

    public double WhereXEquals(double x)
    { return (double)Coefficient * Math.Pow(x, Power); }
    public Fraction WhereXEquals(Fraction x)
    { return Coefficient * Fraction.Pow(x, Power); }

    public static Polynomial operator +(Monomial m1, Monomial m2)
    { return new Polynomial(new List<Monomial>() { m1, m2 }); }

    public static Polynomial operator -(Monomial m1, Monomial m2)
    { return new Polynomial(new List<Monomial>() { m1, new Monomial(-m1.Coefficient, m1.Power) }); }

    public static Monomial operator *(Monomial m1, Monomial m2)
    { return new Monomial(m1.Coefficient * m2.Coefficient, m1.Power + m2.Power); }

    public static Monomial operator /(Monomial m1, Monomial m2)
    { return new Monomial(m1.Coefficient / m2.Coefficient, m1.Power - m2.Power); }

    public static Monomial operator -(Monomial m) => new Monomial(-m.Coefficient, m.Power);

    // TODO: implement static Monomial Pow(Monomial m, Fraction pow)
}