using Exceptions;
using System.Runtime.CompilerServices;

namespace Nomials;

public class Polynomial
{
    public int Count => Monomials.Count;
    public int Power => Monomials[0].Power;
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
        Polynomial p = Copy();
        p.Sort();
        Monomials.Clear();
        double coeff = 0;
        int pow = p.Count == 0 ? 0 : p.Power;
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
    public static Polynomial Normalize(string expr)
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
                    operands.Push(pow == 0 ? Parse("1", false) : p);
                    break;
                default:
                    operands.Push(Parse(item, false));
                    break;
            }
        }
        return operands.Pop().Sort();
    }

    public static List<double> Solve(string expr) => Solve(ParseEquation(expr));
    public static List<double> Solve(Polynomial polynomial)
    {
        Polynomial p = polynomial.Copy();
        p.Normalize();
        List<double> res = new List<double>();

        if (p.Monomials[p.Count - 1].Power != 0)
        {
            p /= new Monomial(1, 1);
            res.Add(0);
            try { res.AddRange(Solve(p)); } catch (xeOException) { }
            res.Sort();
            return res;
        }

        if (p.Power == 0) throw p.Monomials[0].Coefficient == 0 ? new AxeQException() : new xeOException();
        if (p.Power == 1) { res.Add(-p.GetCoefficientByPower(0) / p.GetCoefficientByPower(1)); return res; }
        if (p.Power == 2) return SolveQuadratic(p);

        if (p.Count == 2) // ax^n + b = 0 -> x = nth root of -b/a
        {
            double t = -p.Monomials[1].Coefficient / p.Monomials[0].Coefficient;
            if (t > 0)
            {
                res.Add(Math.Pow(t, 1 / (float)p.Power));
                return res;
            }
            if (p.Power % 2 == 1)
            {
                t = -t;
                res.Add(-Math.Pow(t, 1 / (float)p.Power));
                return res;
            }
            throw new xeOException();
        }

        int first = (int)p.GetCoefficientByPower(p.Power);
        int last = (int)p.GetCoefficientByPower(0);
        HashSet<int> divisorsFirst = new HashSet<int>() { 1 };
        HashSet<int> divisorsLast = new HashSet<int>() { 1, -1 };

        if (first < 0) first = -first;
        if (last < 0) last = -last;
        for (int i = 2; i <= first; i++)
            if (first % i == 0)
                divisorsFirst.Add(i);
        for (int i = 2; i <= last; i++)
            if (last % i == 0)
            {
                divisorsLast.Add(i);
                divisorsLast.Add(-i);
            }
        foreach (var dividend in divisorsLast)
            foreach (var divisor in divisorsFirst)
            {
                float possibleSolution = (dividend / (float)divisor);
                while (p.WhereXEquals(possibleSolution) == 0)
                {
                    string solution = "x" + (possibleSolution < 0 ? " + " : " - ") + Math.Abs(possibleSolution);
                    p /= Parse(solution, false);
                    res.Add(possibleSolution);
                }
            }
        // finishes the job if there is a quadratic equation left
        // allows the quadratic equation to have no solutions since the overall equation has at least one
        if (p.Power == 2)
        {
            try { res.AddRange(SolveQuadratic(p)); }
            catch (xeOException) { }
        }
        res.Sort();
        return res;
    }

    private static List<double> SolveQuadratic(Polynomial p)
    {
        List<double> res = new List<double>();
        double a = p.GetCoefficientByPower(2);
        double b = p.GetCoefficientByPower(1);
        double c = p.GetCoefficientByPower(0);
        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0) throw new xeOException();
        res.Add((-b + Math.Sqrt(discriminant)) / (2 * a));
        res.Add((-b - Math.Sqrt(discriminant)) / (2 * a));
        return res;
    }

    public static Polynomial ParseEquation(string expr)
    {
        expr = expr.Replace(" ", "");
        if (expr.Count(c => c == '=') > 1) throw new Exception();
        if (expr.Contains('=')) expr = expr.Split('=')[0] + "-(" + expr.Split('=')[1] + ")";
        return Normalize(expr);
    }

    public double WhereXEquals(double x)
    {
        double res = 0;
        foreach (var item in Monomials)
        {
            res += item.WhereXEquals(x);
        }
        return res;
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
        while (p.Power >= p2.Power)
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
        while (p.Power >= p2.Power)
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