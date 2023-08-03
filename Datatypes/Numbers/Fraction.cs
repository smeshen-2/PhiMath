namespace Numbers;

public struct Fraction
{
    public int P { get; private set; }
    public int Q { get; private set; }

    public Fraction(int p, int q)
    {
        P = p;
        Q = q;
        if (q == 0) throw new DivideByZeroException("Denominator cannot be zero.");
        if (q < 0)
        {
            //Ensures that Q > 0
            P = -P;
            Q = -Q;
        }
    }

    public Fraction(int n)
    {
        P = n;
        Q = 1;
    }

    /// <summary>
    /// Creates a Fraction from a string in the form "p/q" or "p".
    /// </summary>
    public Fraction(string s)
    {
        int i = s.IndexOf('/');
        if (i > -1)
        {
            P = int.Parse(s.Substring(0, i));
            Q = int.Parse(s.Substring(i + 1));
            if(Q == 0) throw new DivideByZeroException("Denominator cannot be zero.");
        }
        else
        {
            P = int.Parse(s);
            Q = 1;
        }
    }

    public Fraction(float n)
    {
        if (n < 0)
        {
            this = -new Fraction(-n);
            return;
        }
        string s = n.ToString();
        P = int.Parse(s.Replace(".", ""));
        int i = s.IndexOf('.');
        Q = i > -1 ? (int)Math.Pow(10, s.Length - i - 1) : 1;
    }

    public Fraction Simplified()
    {
        if (P == 0) return new Fraction(0);
        int nod = NOD(Math.Abs(P), Q);
        return new Fraction(P / nod, Q / nod);
    }

    public Fraction Inverted()
    {
        if (P < 0) return new Fraction(-Q, -P);
        return new Fraction(Q, P);
    }

    public override string ToString()
    {
        this = Simplified();
        if (Q == 1) return P.ToString();
        return P + "/" + Q;
    }

    public float ToFloat()
    {
        return (float)P / Q;
    }

    public Fraction Squared()
    {
        return new Fraction(P * P, Q * Q).Simplified();
    }

    public static bool operator ==(Fraction a, Fraction b)
    {
        a = a.Simplified();
        b = b.Simplified();
        return a.P == b.P && a.Q == b.Q;
    }

    public static bool operator ==(Fraction a, float b)
    {
        return a == new Fraction(b);
    }

    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }

    public static bool operator !=(Fraction a, float b)
    {
        return !(a == b);
    }

    public static bool operator >(Fraction a, Fraction b)
    {
        return (a - b).P > 0;
    }

    public static bool operator >(Fraction a, float b)
    {
        return a > new Fraction(b);
    }

    public static bool operator <(Fraction a, Fraction b)
    {
        return (a - b).P < 0;
    }

    public static bool operator <(Fraction a, float b)
    {
        return a < new Fraction(b);
    }

    public static bool operator >=(Fraction a, Fraction b)
    {
        return a > b || a == b;
    }

    public static bool operator >=(Fraction a, float b)
    {
        return a > new Fraction(b) || a == new Fraction(b);
    }

    public static bool operator <=(Fraction a, Fraction b)
    {
        return a < b || a == b;
    }

    public static bool operator <=(Fraction a, float b)
    {
        return a < new Fraction(b) || a == new Fraction(b);
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int nok = NOK(a.Q, b.Q);
        return new Fraction(a.P * nok / a.Q + b.P * nok / b.Q, nok).Simplified();
    }

    public static Fraction operator +(Fraction a, float b)
    {
        return (a + new Fraction(b)).Simplified();
    }

    public static Fraction operator -(Fraction a, Fraction b)
    {
        int nok = NOK(a.Q, b.Q);
        return new Fraction(a.P * nok / a.Q - b.P * nok / b.Q, nok).Simplified();
    }

    public static Fraction operator -(Fraction a, float b)
    {
        return (a - new Fraction(b)).Simplified();
    }

    public static Fraction operator -(Fraction a)
    {
        return new Fraction(-a.P, a.Q);
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        return new Fraction(a.P * b.P, a.Q * b.Q).Simplified();
    }

    public static Fraction operator *(Fraction a, float b)
    {
        return a * new Fraction(b);
    }

    public static Fraction operator /(Fraction a, Fraction b)
    {
        return a * b.Inverted();
    }

    public static Fraction operator /(Fraction a, float b)
    {
        return a / new Fraction(b);
    }

    private static int NOD(int a, int b)
    {
        //if (a * b == 0) return 0;
        if (a == b) return a;
        if (a > b) return NOD(a - b, b);
        return NOD(a, b - a);
    }

    private static int NOK(int a, int b)
    {
        return a * b / NOD(a, b);
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

    public static Fraction Min(Fraction a, Fraction b)
    {
        return a < b ? a : b;
    }

    public static Fraction Max(Fraction a, Fraction b)
    {
        return a > b ? a : b;
    }

    public static Fraction Clamp(Fraction fraction, Fraction min, Fraction max)
    {
        if (fraction < min) return min;
        if (fraction > max) return max;
        return fraction;
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

    public static implicit operator Fraction(string s)
    {
        return new Fraction(s);
    }
}
