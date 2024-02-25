namespace Numbers;

public struct Fraction
{
    public int P { get; private set; }
    public int Q { get; private set; }

    public Fraction(int p, int q = 1, bool normalize = true)
    {
        P = p;
        Q = q;
        if (q == 0) throw new DivideByZeroException("Denominator cannot be zero.");
        if (q < 0)
        {
            // ensures that Q > 0
            P = -P;
            Q = -Q;
        }
        if (normalize) this = Normalize();
    }

    /// <summary>
    /// Creates a Fraction from a string in the form "p/q" or "p".
    /// </summary>
    public static Fraction Parse(string s, bool normalize = true)
    {
        if (!s.Contains('/')) return Parse(double.Parse(s));
        if (s.Split('/').Length > 2) throw new Exception();
        return new Fraction(int.Parse(s.Split('/')[0]), int.Parse(s.Split('/')[1]), normalize);
    }
    public static Fraction Parse(double n)
    {
        int i = 1;
        while (n % 1 != 0)
        {
            n *= 10;
            i *= 10;
        }
        return new Fraction((int)n, i);
    }
    public static Fraction Parse(int n) => new Fraction(n, 1, false);

    public Fraction Normalize()
    {
        if (P == 0)
        {
            Q = 1;
            return this;
        }
        int nod = NOD(Math.Abs(P), Q);
        return new Fraction(P / nod, Q / nod, false);
    }

    public Fraction Invert()
    {
        if (P < 0) return new Fraction(-Q, -P, false);
        return new Fraction(Q, P, false);
    }

    public override string ToString()
    {
        if (Q == 1) return P.ToString();
        return P + "/" + Q;
    }

    public static bool operator ==(Fraction a, Fraction b)
    {
        a = a.Normalize();
        b = b.Normalize();
        return a.P == b.P && a.Q == b.Q;
    }
    public static bool operator ==(Fraction a, double b) => a == Parse(b);
    public static bool operator ==(Fraction a, int b) => a.P == b && a.Q == 1;

    public static bool operator !=(Fraction a, Fraction b) => !(a == b);
    public static bool operator !=(Fraction a, double b) => !(a == b);
    public static bool operator !=(Fraction a, int b) => !(a == b);

    public static bool operator >(Fraction a, Fraction b) => a.P * b.Q > b.P * a.Q;
    public static bool operator >(Fraction a, double b) => a > Parse(b);
    public static bool operator >(Fraction a, int b) => a.P > b * a.Q;

    public static bool operator <(Fraction a, Fraction b) => a.P * b.Q < b.P * a.Q;
    public static bool operator <(Fraction a, double b) => a < Parse(b);
    public static bool operator <(Fraction a, int b) => a.P < b * a.Q;

    public static bool operator >=(Fraction a, Fraction b) => a > b || a == b;
    public static bool operator >=(Fraction a, double b) => a >= Parse(b);
    public static bool operator >=(Fraction a, int b) => a > b || a == b;

    public static bool operator <=(Fraction a, Fraction b) => a < b || a == b;
    public static bool operator <=(Fraction a, double b) => a <= Parse(b);
    public static bool operator <=(Fraction a, int b) => a < b || a == b;

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int nok = NOK(a.Q, b.Q);
        return new Fraction(a.P * nok / a.Q + b.P * nok / b.Q, nok);
    }
    public static Fraction operator +(Fraction a, double b) => a + Parse(b);
    public static Fraction operator +(Fraction a, int b) => new Fraction(a.P + b * a.Q, a.Q);

    public static Fraction operator -(Fraction a, Fraction b)
    {
        int nok = NOK(a.Q, b.Q);
        return new Fraction(a.P * nok / a.Q - b.P * nok / b.Q, nok);
    }
    public static Fraction operator -(Fraction a, double b) => a - Parse(b);
    public static Fraction operator -(Fraction a, int b) => new Fraction(a.P - b * a.Q, a.Q);

    public static Fraction operator -(Fraction a) => new Fraction(-a.P, a.Q, false);

    public static Fraction operator *(Fraction a, Fraction b) => new Fraction(a.P * b.P, a.Q * b.Q);
    public static Fraction operator *(Fraction a, double b) => a * Parse(b);
    public static Fraction operator *(Fraction a, int b) => new Fraction(a.P * b, a.Q);

    public static Fraction operator /(Fraction a, Fraction b) => new Fraction(a.P * b.Q, a.Q * b.P);
    public static Fraction operator /(Fraction a, double b) => a / Parse(b);
    public static Fraction operator /(Fraction a, int b) => new Fraction(a.P, a.Q * b);

    public static Fraction Pow(Fraction f, int power)
    {
        Fraction res = Parse(1);
        for (int i = 0; i < power; i++)
            res *= f;
        return res;
    }

    public static explicit operator float(Fraction f) => (float)f.P / f.Q;
    public static explicit operator double(Fraction f) => (double)f.P / f.Q;
    public static explicit operator int(Fraction f) => f.P / f.Q;

    public static int NOD(int a, int b)
    {
        while (b != 0)
        {
            int oldB = b;
            b = a % b;
            a = oldB;
        }
        return a;
    }
    public static int NOK(int a, int b)
    {
        return a * b / NOD(a, b);
    }
    public static int NOD(params int[] numbers)
    {
        bool isNOD = false;
        for (int i = numbers.Min(); i >= 1; i--)
        {
            isNOD = true;
            for (int j = 0; j < numbers.Length; j++)
            {
                if (numbers[j] % i != 0)
                {
                    isNOD = false;
                    break;
                }
            }
            if (isNOD) return i;
        }
        throw new Exception();
    }
    public static int NOK(params int[] numbers)
    {
        bool isNOK = false;
        for (int i = numbers.Max(); i <= int.MaxValue; i++)
        {
            isNOK = true;
            for (int j = 0; j < numbers.Length; j++)
            {
                if (i % numbers[j] != 0)
                {
                    isNOK = false;
                    break;
                }
            }
            if (isNOK) return i;
        }
        throw new Exception();
    }
    public static Fraction Abs(Fraction fraction)
    {
        if (fraction.P >= 0) return fraction;
        return -fraction;
    }
    public static Root Sqrt(Fraction fraction)
    {
        return new Root(1, fraction.P * fraction.Q, fraction.Q);
    }
}