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
        if (Power == 1)
        {
            if (Coefficient == 1) return "x";
            if (Coefficient == -1) return "-x";
            return Coefficient + "x";
        }
        if (Coefficient == 1) return "x^" + Power;
        if (Coefficient == -1) return "-x^" + Power;
        return $"{Coefficient}x^{Power}";
    }

    public static Monomial Parse(string s)
    {
        foreach (var item in s)
        {
            if (!"^x0123456789 ".Contains(item)) throw new Exception("Invalid symbols.");
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
            power = int.Parse(s.Split('^')[1]);
        return new Monomial(coeff, power);
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