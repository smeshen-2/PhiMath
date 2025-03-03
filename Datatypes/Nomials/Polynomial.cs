using Exceptions;
using Numbers;
using WordEvolution;

namespace Nomials;

public class Polynomial
{
	/// <summary>
	/// The monomials that make up the polynomial.
	/// </summary>
	public List<Monomial> Monomials { get; private set; } = new List<Monomial>();
	/// <summary>
	/// The number of monomials that make up the polynomial.
	/// </summary>
	public int Count => Monomials.Count;
	/// <summary>
	/// The power of the first monomial in the polynomial, which is NOT the highest power if NOT sorted.
	/// </summary>
	public int Power => Monomials[0].Power;
	public Polynomial() { Monomials = new List<Monomial>(); }

	/// <summary>
	/// Creates instance of a polynomial, based on a list of monomials, which it normalizes unless told otherwise.
	/// </summary>
	/// <param name="monos"></param>
	/// <param name="normalize"></param>
	public Polynomial(List<Monomial> monos, bool normalize = true)
	{
		foreach (var item in monos)
		{
			Monomials.Add(item);
		}
		if (normalize) Normalize();
	}
	public Polynomial(Monomial mono) { Monomials.Add(mono); }

	/// <summary>
	/// Parses a polynomial and normalizes it unless told otherwise.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="normalize"></param>
	public static Polynomial Parse(string s, bool normalize = true)
	{
		s = s.Replace(" ", "").Replace("-", "+-").Replace("^+-", "^-").Trim('+');

		List<Monomial> listMonos = new List<Monomial>();
		string[] stringMonos = s.Split('+');

		foreach (var item in stringMonos) { listMonos.Add(Monomial.Parse(item)); }
		return new Polynomial(listMonos, normalize);
	}

	/// <summary>
	/// Exends polynomial with a monomial.
	/// </summary>
	/// <param name="m"></param>
	private void Add(Monomial m)
	{
		Monomials.Add(m);
	}
	/// <summary>
	/// Exends polynomial with a polynomial.
	/// </summary>
	/// <param name="p"></param>
	private void Add(Polynomial p)
	{
		foreach (var item in p.Monomials)
		{
			Monomials.Add(item);
		}
	}

	/// <summary>
	/// Gives a copy of polynomial.
	/// </summary>
	public Polynomial Copy()
	{
		List<Monomial> monos = new List<Monomial>();
		foreach (var item in Monomials)
		{
			monos.Add(item);
		}
		return new Polynomial(monos, false);
	}

	/// <summary>
	/// Sorts polynomial.
	/// </summary>
	public void Sort()
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
	}

	/// <summary>
	/// Sorts polynomial and sums its monomials with the same power.
	/// </summary>
	public void Normalize()
	{
		Polynomial p = Copy();
		p.Sort();
		Monomials.Clear();
		Fraction coeff = Fraction.Parse(0);
		int pow = p.Power;
		foreach (var item in p.Monomials)
		{
			if (item.Power == pow) coeff += item.Coefficient;
			else
			{
				Add(new Monomial(coeff, pow));
				coeff = item.Coefficient.Normalize();
				pow = item.Power;
			}
		}
		Add(new Monomial(coeff, pow));
		Monomials.RemoveAll(x => x.Coefficient == 0);
		if (Monomials.Count == 0) Monomials.Add(new Monomial(0));
	}

	/// <summary>
	/// Translates an expression into reverse polish notation.
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public static string ToRPN(string expr)
	{
		string s = expr;
		// gets rid of superscript
		bool inPower = false;
		int offset = 0;
		for (int i = 0; i < expr.Length; i++)
		{
			if ("⁰¹²³⁴⁵⁶⁷⁸⁹".Contains(expr[i]))
			{
				if (!inPower)
				{
					s = s.Insert(i + offset, "^");
					offset++;
				}
				inPower = true;
				s = s.Insert(i + offset, Monomial.FromSuperscript(expr[i].ToString()).ToString() ?? "").Remove(i + offset + 1, 1);
			}
			else inPower = false;
			if (!"(+*^-/)x,._0123456789⁰¹²³⁴⁵⁶⁷⁸⁹ ".Contains(expr[i])) throw new ArgumentException();
		}

		expr = s;
		expr = Evolver.Evolve(expr, "|*|N_("); // adds * between monomial and (
		expr = Evolver.Evolve(expr, "|*|)_N"); // adds * between ) and monomial
		expr = Evolver.Evolve(expr, "|*|)_("); // adds * between ) and (
		expr = Evolver.Evolve(expr, "|*|x_N"); // adds * between x and monomial
		expr = Evolver.Evolve(expr, "|*|N_x"); // adds * between monomial and x
		expr = Evolver.Evolve(expr, "| |_O"); // adds space before [+-*/()^]
		expr = Evolver.Evolve(expr, "| |O_"); // adds space after [+-*/()^]
		expr = "( " + expr + " )";
		while (expr.Contains("  ")) expr = expr.Replace("  ", " ");
		expr = expr.Replace("( -", "( 0 -");

		string[] arrExpr = expr.Split();
		Stack<char> operators = new Stack<char>();
		string res = "";

		foreach (var item in arrExpr)
		{
			switch (item)
			{
				case "(":
					operators.Push('(');
					continue;
				case "+":
				case "-":
					while (operators.Peek() != '(')
						res += operators.Pop() + " ";
					operators.Push(item[0]);
					continue;
				case "*":
				case "/":
					while (operators.Peek() != '(' && operators.Peek() != '+' && operators.Peek() != '-')
						res += operators.Pop() + " ";
					operators.Push(item[0]);
					continue;
				case "^":
					operators.Push('^');
					continue;
				case ")":
					while (operators.Count > 0 && operators.Peek() != '(')
					{
						res += operators.Pop() + " ";
					}
					if (operators.Count > 0) operators.Pop();
					continue;
				default:
					res += item + " ";
					continue;
			}
		}
		while (operators.Count > 0) res += operators.Pop() + " ";

		return res.Trim();
	}

	/// <summary>
	/// Turns an expression into a single, normalized polynomial.
	/// </summary>
	/// <exception cref="DivideByPolynomialException"></exception>
	/// <exception cref="DivideByZeroException"></exception>
	public static Polynomial ParseExpression(string expr)
	{
		string[] arrExpr = ToRPN(expr).Split();
		Stack<Polynomial> operands = new Stack<Polynomial>();
		foreach (var item in arrExpr)
		{
			switch (item)
			{
				case "+":
					operands.Push(operands.Pop() + operands.Pop());
					continue;
				case "-":
					operands.Push(-operands.Pop() + operands.Pop());
					continue;
				case "*":
					operands.Push(operands.Pop() * operands.Pop());
					continue;
				case "/":
					Polynomial divisor = operands.Pop();
					if (divisor.Monomials[0].Coefficient == 0) throw new DivideByZeroException();
					operands.Push(operands.Pop() / divisor);
					continue;
				case "^":
					int pow = int.Parse(operands.Pop().ToString()); // TODO: turn into Fraction
					Polynomial p = operands.Pop();
					if (pow < 0)
					{
						if (p.Count > 1) throw new DivideByPolynomialException();
						Monomial m = p.Monomials[0];
						Fraction coeff = new Fraction(1);
						for (int i = 0; i > pow; i--)
							coeff /= m.Coefficient;
						operands.Push(new Polynomial(new Monomial(coeff, m.Power * pow)));
						continue;
					}
					Polynomial res = new Polynomial(new Monomial(1));
					for (int i = 0; i < pow; i++)
						res *= p;
					operands.Push(res);
					continue;
				default:
					operands.Push(Parse(item, false));
					continue;
			}
		}
		Polynomial sorted = operands.Pop();
		sorted.Sort();
		return sorted;
	}

	/// <summary>
	/// Finds all rational (and possibly some real) roots of polynomial equation.
	/// </summary>
	/// <param name="expr"></param>
	/// <returns>List of real roots that cannot be empty.</returns>
	public static List<double> Solve(string expr) => Solve(ParseEquation(expr));
	/// <summary>
	/// Finds all rational (and possibly some real) roots of polynomial.
	/// </summary>
	/// <param name="polynomial"></param>
	/// <returns>List of real roots that cannot be empty.</returns>
	public static List<double> Solve(Polynomial polynomial)
	{
		Polynomial p = polynomial.Copy();
		p.Normalize();
		List<double> res = new List<double>();

		while (p.Monomials[p.Count - 1].Power < 0) // multiply by x if there's a negative power
			p *= new Monomial(1, 1);
		while (p.Monomials[p.Count - 1].Power > 0) // divide by x if there's no x^0, x=0
		{
			p /= new Monomial(1, 1);
			res.Add(0);
		}
		// if not binomial and power > 2, do horner and check again
		if (p.Count != 2 && p.Power > 2) res.AddRange(SolveHorner(p).Select(f => (double)f));
		if (p.Count == 2 || p.Power <= 2)
			try { res.AddRange(SimpleSolve(p)); } // if a solution is found but SimpleSolve returns xeO, ignore
			catch (NoRealRootsException) { if (res.Count == 0) throw; }
		res.Sort();
		return res;
	}
	/// <summary>
	/// Finds all real roots of polynomials of at most 2nd power.
	/// </summary>
	/// <param name="p"></param>
	/// <returns>List of all real roots that cannot be empty.</returns>
	/// <exception cref="NoRealRootsException"></exception>
	private static List<double> SimpleSolve(Polynomial p)
	{
		List<double> res = new List<double>();

		if (p.Power == 0) throw p.Monomials[0].Coefficient == 0 ? new AllRealRootsException() : new NoRealRootsException();
		if (p.Power == 1) { res.Add((double)(-p.GetCoefficientByPower(0) / p.GetCoefficientByPower(1))); return res; }
		if (p.Power == 2) return SolveQuadratic(p);

		if (p.Count == 2) // ax^n + b = 0 -> x = (+- if even power)nth root of -b/a
		{
			double t = (double)(-p.Monomials[1].Coefficient / p.Monomials[0].Coefficient); // t = -b/a
			if (t > 0)
			{
				if (p.Power % 2 == 0) res.Add(-Math.Pow(t, 1 / (double)p.Power));
				res.Add(Math.Pow(t, 1 / (double)p.Power));
				return res;
			}
			if (p.Power % 2 == 1)
			{
				t = -t;
				res.Add(-Math.Pow(t, 1 / (double)p.Power));
				return res;
			}
			throw new NoRealRootsException();
		}
		return res;
	}
	/// <summary>
	/// Finds both roots of quadratic polynomial.
	/// </summary>
	/// <param name="p"></param>
	/// <returns>List of both real roots.</returns>
	/// <exception cref="NoRealRootsException"></exception>
	private static List<double> SolveQuadratic(Polynomial p)
	{
		List<double> res = new List<double>();
		double a = (double)p.GetCoefficientByPower(2);
		double b = (double)p.GetCoefficientByPower(1);
		double c = (double)p.GetCoefficientByPower(0);
		double discriminant = b * b - 4 * a * c;
		if (discriminant < 0) throw new NoRealRootsException();
		res.Add((-b + Math.Sqrt(discriminant)) / (2 * a));
		res.Add((-b - Math.Sqrt(discriminant)) / (2 * a));
		return res;
	}
	/// <summary>
	/// Finds all rational roots of polynomial, using Horner's method.
	/// </summary>
	/// <param name="p"></param>
	/// <returns>List of all rational roots that cannot be empty.</returns>
	/// <exception cref="NoRationalRootsException"></exception>
	private static List<Fraction> SolveHorner(Polynomial p)
	{
		List<Fraction> res = new List<Fraction>();
		IntifyCoefficients(p);
		int first = Math.Abs(p.GetCoefficientByPower(p.Power).P);
		int last = Math.Abs(p.GetCoefficientByPower(0).P);

		HashSet<int> divisorsFirst = new HashSet<int>() { 1 };
		HashSet<int> divisorsLast = new HashSet<int>() { 1, -1 };

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
				Fraction possibleSolution = new Fraction(dividend, divisor);
				while (p.WhereXEquals(possibleSolution) == 0)
				{
					string solution = "x" + (possibleSolution < 0 ? " + " + -possibleSolution : " - " + possibleSolution);
					p /= Parse(solution, false);
					res.Add(possibleSolution);
				}
			}

		if (res.Count == 0) throw new NoRationalRootsException();
		return res;
	}
	/// <summary>
	/// Multiplies polynomial by numbers repeatedly, until all coefficients of the polynomial become integers.
	/// </summary>
	/// <param name="p"></param>
	public static void IntifyCoefficients(Polynomial p)
	{
		bool done = false;
		while (!done)
		{
			done = true;
			foreach (Monomial mono in p.Monomials)
				if (mono.Coefficient.Q != 1)
				{
					p *= new Monomial(mono.Coefficient.Q);
					done = false;
					break;
				}
		}
	}

	/// <summary>
	/// Turns polynomial into a product represented by a list of the smallest polynomials that comprise it.
	/// The element at index 0 is always a monomial, even if it is 1.
	/// </summary>
	/// <param name="p"></param>
	public static List<Polynomial> Factorize(Polynomial p)
	{
		List<Polynomial> res = new List<Polynomial>();
		Polynomial remainder = p.Copy();

		// divide polynomial so that the leading coefficient is 1 and there is a free term
		Monomial factor = new Monomial(remainder.Monomials[0].Coefficient, remainder.Monomials[remainder.Count - 1].Power);
		res.Add(new Polynomial(new Monomial(1)));
		remainder /= factor;

		List<double> solutions;
		try { solutions = Solve(remainder); }
		catch (NoRootsException) { res.Add(remainder); return res; }

		foreach (var solution in solutions)
		{
			Polynomial binomial = ParseExpression("x - (" + solution + ")");
			res.Add(binomial);
			// PROBLEM! BINOMIAL CONTAINS FLOAT, WHICH IS NOT PRECISE!
			remainder /= binomial;
		}
		if (remainder.Count != 1) res.Add(remainder);
		res.Sort((p1, p2) => p1.Power - p2.Power);
		res[0] = new Polynomial(factor);
		return res;
	}

	/// <summary>
	/// Turns an equation into a single, normalized polynomial by subtracting the right side from the left side.
	/// </summary>
	/// <param name="expr"></param>
	/// <exception cref="ArgumentException"></exception>
	public static Polynomial ParseEquation(string expr)
	{
		expr = expr.Replace(" ", "");
		if (expr.Count(c => c == '=') > 1) throw new ArgumentException();
		if (expr.Contains('=')) expr = expr.Split('=')[0] + "-(" + expr.Split('=')[1] + ")";
		return ParseExpression(expr);
	}

	/// <summary>
	/// Finds the result when replacing the variable in the polynomial with a given value.
	/// </summary>
	/// <param name="x"></param>
	public double WhereXEquals(double x)
	{
		double res = 0;
		foreach (var item in Monomials)
		{
			res += item.WhereXEquals(x);
		}
		return res;
	}
	/// <summary>
	/// Finds the result when replacing the variable in the polynomial with a given value.
	/// </summary>
	/// <param name="x"></param>
	public Fraction WhereXEquals(Fraction x)
	{
		Fraction res = Fraction.Parse(0);
		foreach (var item in Monomials)
		{
			res += item.WhereXEquals(x);
		}
		return res;
	}

	/// <summary>
	/// Finds the coefficient of the FIRST monomial with a specified power.
	/// </summary>
	/// <param name="power"></param>
	public Fraction GetCoefficientByPower(int power)
	{
		Fraction coeff = Monomials.Find(n => n.Power == power).Coefficient;
		if (coeff.P == 0) return new Fraction(0);
		return coeff;
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
		while (p.Power >= p2.Power && p.ToString() != "0")
		{
			res.Add(p.Monomials[0] / p2.Monomials[0]);
			Polynomial t = res.Monomials[res.Count - 1] * p2;
			p -= t;
		}
		if (p.ToString() != "0") throw new DivideByPolynomialException();
		return res;
	}
	public static Polynomial operator %(Polynomial p1, Polynomial p2)
	{
		return p1 - p1 / p2 * p2;
	}

	// OPERATORS WITH ONE POLYNOMIAL
	public static Polynomial operator -(Polynomial p)
	{
		Polynomial res = p.Copy();
		for (int i = 0; i < res.Count; i++)
			res.Monomials[i] = -res.Monomials[i];
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
	// TODO: implement static Polynomial Pow(Polynomial p, Fraction(?) pow)
}