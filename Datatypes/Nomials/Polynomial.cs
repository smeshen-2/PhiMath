using PhiMath;

namespace Nomials;

public class Polynomial
{
    public int Count => Monomials.Count;
    public List<Monomial> Monomials { get; private set; } = new List<Monomial>();
    public Polynomial() { Monomials = new List<Monomial>(); }

    // normalizes unless told otherwise
    public Polynomial(List<Monomial> monos, bool normalize = true)
    {
        foreach (var item in monos)
        {
            Monomials.Add(item);
        }
        if (normalize) Normalize();
    }

    // normalizes unless told otherwise
    public static Polynomial Parse(string s, bool normalize = true)
    {
        s = s.Replace(" ", "").Replace("-", "+-").Replace("^+-", "^-").Trim('+');

        List<Monomial> listMonos = new List<Monomial>();
        string[] stringMonos = s.Split('+');

        foreach (var item in stringMonos) { listMonos.Add(Monomial.Parse(item)); }
        return new Polynomial(listMonos, normalize);
    }

    // extend polynomial
    private Polynomial Add(Monomial m)
    {
        Monomials.Add(m);
        return this;
    }
    private Polynomial Add(Polynomial p)
    {
        foreach (var item in p.Monomials)
        {
            this.Add(item);
        }
        return this;
    }

    public Polynomial Copy()
    {
        List<Monomial> monos = new List<Monomial>();
        foreach (var item in Monomials)
        {
            monos.Add(item.Copy());
        }
        return new Polynomial(monos, false);
    }

    public Polynomial Sort()
    {
        Monomial temp;
        for (int write = 0; write < Count; write++)
        {
            for (int sort = 0; sort < Count - 1; sort++)
            {
                if (Monomials[sort].Power < Monomials[sort + 1].Power)
                {
                    temp = Monomials[sort + 1];
                    Monomials[sort + 1] = Monomials[sort];
                    Monomials[sort] = temp;
                }
            }
        }
        return this;
    }

    // sorts polynomial and does all possible additions and subtractions
    public Polynomial Normalize()
    {
        Polynomial p = this.Copy();
        p.Sort();
        Monomials.Clear();
        double coeff = 0;
        int pow = p.Count == 0 ? 0 : p.Monomials[0].Power;
        foreach (var item in p.Monomials)
        {
            if (item.Power == pow) coeff += item.Coefficient;
            else
            {
                Add(new Monomial(coeff, pow));
                coeff = item.Coefficient;
                pow = item.Power;
            }
        }
        Add(new Monomial(coeff, pow));
        Monomials.RemoveAll(x => x.Coefficient == 0);
        if (Monomials.Count == 0) Monomials.Add(new Monomial(0, 0));
        return this;
    }

    // turns an expression into a RPN one
    public static string ToRPN(string expr)
    {
        foreach (var item in expr)
        {
            if (!"(+*^-/)x,._0123456789 ".Contains(item)) throw new Exception("Invalid symbols.");
        }
        expr = "( " + expr.Replace("+", " + ").Replace("-", " - ")
            .Replace("*", " * ").Replace("/", " / ")
            .Replace("(", " ( ").Replace(")", " ) ")
            .Replace("^", " ^ ") + " )";
        while (expr.Contains("  ")) expr = expr.Replace("  ", " ");
        expr = expr.Replace("( -", "( 0 -");

        string[] arrExpr = expr.Split();
        Stack<char> operators = new Stack<char>();
        string res = "";

        foreach (var item in arrExpr)
        {
            try
            {
                Monomial t = Monomial.Parse(item);
                if (t.Coefficient == 0) { res += "0 "; continue; }
                if (t.Power == 0) { res += t.Coefficient + " "; continue; }
                if (t.Coefficient == 1) { res += "x "; continue; }
                res += t.Coefficient + " x ";
                operators.Push('*');
                continue;
            }
            catch { }

            if (item == "(") operators.Push('(');

            if (item == "+" || item == "-")
            {
                while (operators.Peek() != '(')
                {
                    res += operators.Pop() + " ";
                }
                operators.Push(item[0]);
            }

            if (item == "*" || item == "/")
            {
                while (operators.Peek() != '(' && operators.Peek() != '+' && operators.Peek() != '-')
                {
                    res += operators.Pop() + " ";
                }
                operators.Push(item[0]);
            }

            if (item == "^")
            {
                operators.Push('^');
            }

            if (item == ")")
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    res += operators.Pop() + " ";
                }
                if (operators.Count > 0) operators.Pop();
            }
        }
        while (operators.Count > 0) res += operators.Pop() + " ";

        return res.Trim();
    }

    // turns an expression into a single, normalized polynomial
    public static Polynomial Simplify(string expr)
    {
        string[] arrExpr = ToRPN(expr).Split();
        Stack<Polynomial> operands = new Stack<Polynomial>();
        foreach (var item in arrExpr)
        {
            switch (item)
            {
                case "+":
                    operands.Push(operands.Pop() + operands.Pop());
                    break;
                case "-":
                    operands.Push(-operands.Pop() + operands.Pop());
                    break;
                case "*":
                    operands.Push(operands.Pop() * operands.Pop());
                    break;
                case "/":
                    Monomial t;
                    try
                    {
                        t = Monomial.Parse(operands.Pop().ToString());
                    }
                    catch
                    {
                        throw new Exception("Divisor cannot be a polynomial.");
                    }
                    if (t.Coefficient == 0) throw new Exception("Cannot divide by 0.");
                    operands.Push(operands.Pop() / t);
                    break;
                case "^":
                    int pow = int.Parse(operands.Pop().ToString());
                    Polynomial p = operands.Pop();
                    Polynomial pCopy = p.Copy();
                    while (pow > 1)
                    {
                        p *= pCopy;
                        pow--;
                    }
                    operands.Push(pow == 0 ? Polynomial.Parse("1", false) : p);
                    break;
                default:
                    operands.Push(Polynomial.Parse(item, false));
                    break;
            }
        }
        return operands.Pop().Sort();
    }

    // gets the values of x where the expression would equal 0, can also solve equations (power <= 2)
    public static List<double> Solve(string expr)
    {
        expr = expr.Replace(" ", "");
        if (expr.Count(c => c == '=') > 1) throw new Exception();
        if (expr.Contains('=')) expr = expr.Split('=')[0] + "-(" + expr.Split('=')[1] + ")";
        Polynomial p = Simplify(expr);
        List<double> res = new List<double>();
        if (p.Monomials[0].Power > 2) throw new Exception("Cannot solve equation.");
        if (p.Monomials[0].Power == 2)
        {
            double a = p.GetCoefficientByPower(2);
            double b = p.GetCoefficientByPower(1);
            double c = p.GetCoefficientByPower(0);
            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0) throw new Exception("Cannot solve equation.");
            res.Add((-b + Math.Sqrt(discriminant)) / (2 * a));
            res.Add((-b - Math.Sqrt(discriminant)) / (2 * a));
            if (res[0] == res[1]) res.Remove(res[0]);
            return res;
        }
        if (p.Monomials[0].Power == 1)
        {
            if (p.Count == 1) res.Add(0);
            else res.Add(-p.GetCoefficientByPower(0) / p.GetCoefficientByPower(1));
            return res;
        }
        if (p.Monomials[0].Coefficient == 0) throw new AxeQException();
        else throw new xeOException();
    }

    public double GetCoefficientByPower(int power)
    {
        Monomial m = Monomials.Find(n => n.Power == power);
        return m is null ? 0 : m.Coefficient;
    }

    public override string ToString()
    {
        string s = "";
        foreach (var item in Monomials)
        {
            s = s + " + " + item;
        }
        s = s.Replace("+ -", "- ").Trim();
        if (s.Substring(0, 2) == "+ ") return s.Substring(2);
        if (s.Substring(0, 2) == "- ") return "-" + s.Substring(2);
        return s;
    }

    // OPERATORS WITH 2 POLYNOMIALS
    public static Polynomial operator +(Polynomial p1, Polynomial p2)
    {
        Polynomial res = p1.Copy();
        res.Add(p2);
        res.Normalize();
        return res;
    }
    public static Polynomial operator -(Polynomial p1, Polynomial p2)
    {
        return p1 + -p2;
    }
    public static Polynomial operator *(Polynomial p1, Polynomial p2)
    {
        Polynomial res = new Polynomial();
        foreach (var first in p1.Monomials)
        {
            foreach (var second in p2.Monomials)
            {
                res.Add(first * second);
            }
        }
        res.Normalize();
        return res;
    }
    public static Polynomial operator /(Polynomial p1, Polynomial p2)
    {
        Polynomial res = new Polynomial();
        Polynomial p = p1.Copy();
        Polynomial t = new Polynomial();
        while (p.Monomials[0].Power >= p2.Monomials[0].Power)
        {
            res.Add(p.Monomials[0] / p2.Monomials[0]);
            t = res.Monomials[res.Count - 1] * p2;
            p -= t;
        }
        return res;
    }
    public static Polynomial operator %(Polynomial p1, Polynomial p2)
    {
        Polynomial p = p1.Copy();
        Polynomial t = new Polynomial();
        while (p.Monomials[0].Power >= p2.Monomials[0].Power)
        {
            t = (p.Monomials[0] / p2.Monomials[0]) * p2;
            p -= t;
        }
        return p;
    }

    // OPERATORS WITH ONE POLYNOMIAL
    public static Polynomial operator -(Polynomial p)
    {
        Polynomial res = p.Copy();
        foreach (var item in res.Monomials)
        {
            item.Coefficient = -item.Coefficient;
        }
        return res;
    }
    public static Polynomial operator *(Polynomial p, Monomial m)
    {
        Polynomial res = new Polynomial();
        foreach (var mono in p.Monomials)
        {
            res.Add(mono * m);
        }
        res.Normalize();
        return res;
    }
    public static Polynomial operator *(Monomial m, Polynomial p)
    {
        Polynomial res = new Polynomial();
        foreach (var mono in p.Monomials)
        {
            res.Add(mono * m);
        }
        res.Normalize();
        return res;
    }
    public static Polynomial operator /(Polynomial p, Monomial m)
    {
        Polynomial res = new Polynomial();
        foreach (var mono in p.Monomials)
        {
            res.Add(mono / m);
        }
        res.Normalize();
        return res;
    }
}