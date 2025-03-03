using Nomials;
using Exceptions;
using WordEvolution;

namespace PhiMath.Pages;

public partial class Algebra : ContentPage
{
	private static Color uneditedColor, editedColor;
	public Algebra()
	{
		InitializeComponent();
		algebra_entry.Text = "";
		Evolver.AddGroup('N', "0123456789x");
		Evolver.AddGroup('O', "+-*/()^");
		Evolver.separator = "|";

		if (App.Current.RequestedTheme == AppTheme.Light) uneditedColor = Colors.Black;
		else uneditedColor = Colors.White;

		if (App.Current.Resources.TryGetValue("Secondary", out var colorvalue)) editedColor = (Color)colorvalue;
	}
	private void Factor_Clicked(object sender, EventArgs e)
	{
		algebra_output.TextColor = uneditedColor;
		string entry = algebra_entry.Text;
		if (entry == "") { algebra_output.Text = "Input an integer, polynomial or expression"; return; }

		try
		{
			int n = int.Parse(entry);
			if (n == 0 || n == 1 || n == -1) { algebra_output.Text = n.ToString(); return; }
			if (IsPrime(n)) { algebra_output.Text = "Prime"; return; }
			algebra_output.Text = string.Join(", ", GetFactors(n));
		}
		catch
		{
			try
			{
				Polynomial p = Polynomial.ParseExpression(entry);
				if (p.Power == 1) { algebra_output.Text = p.ToString(); return; }
				if (p.Power == 0)
				{
					if (p.Monomials[0].Coefficient.Q != 1) { algebra_output.Text = "Fraction"; return; }
					int n = p.Monomials[0].Coefficient.P;
					if (n == 0 || n == 1 || n == -1) { algebra_output.Text = n.ToString(); return; }
					if (IsPrime(n)) { algebra_output.Text = "Prime"; return; }
					algebra_output.Text = string.Join(", ", GetFactors(n));
					return;
				}
				List<Polynomial> res = Polynomial.Factorize(p);

				if (res[0].ToString() == "-1")
				{
					algebra_output.Text = "-";
					res.RemoveAt(0);
				}
				else if (res[0].ToString() == "1")
				{
					algebra_output.Text = "";
					res.RemoveAt(0);
				}
				else
				{
					algebra_output.Text = res[0].ToString();
					res.RemoveAt(0);
				}
				int pow = 1;
				Polynomial current = res[0];
				for (int i = 1; i < res.Count; i++)
				{
					if (res[i].ToString() == current.ToString()) pow++;
					else
					{
						algebra_output.Text += "(" + current.ToString() + ")";
						if(pow != 1) algebra_output.Text += Monomial.ToSuperscript(pow);
						pow = 1;
						current = res[i];
					}
				}
				algebra_output.Text += "(" + current.ToString() + ")";
				if (pow != 1) algebra_output.Text += Monomial.ToSuperscript(pow);
			}
			catch (ArgumentException) { algebra_output.Text = "Invalid input"; }
			catch { algebra_output.Text = "Something went wrong"; }
		}
	}
	private void Normalize_Clicked(object sender, EventArgs e)
	{
		algebra_output.TextColor = uneditedColor;
		string entry = algebra_entry.Text;
		if (entry == "") { algebra_output.Text = "Input a polynomial or expression"; return; }

		try { algebra_output.Text = Polynomial.ParseExpression(entry).ToString(); }
		catch (DivideByPolynomialException ex) { algebra_output.Text = ex.Message; }
		catch (ArgumentException) { algebra_output.Text = "Invalid input"; }
		catch { algebra_output.Text = "Something went wrong"; }
	}
	private void Sqrt_Clicked(object sender, EventArgs e)
	{
		algebra_output.TextColor = uneditedColor;
		string entry = algebra_entry.Text;
		if (entry == "") { algebra_output.Text = "Input an integer"; return; }

		int n;
		try
		{
			n = int.Parse(algebra_entry.Text);
			if (n < 0) throw new Exception();
		}
		catch { algebra_output.Text = "Invalid number"; return; }
		if (n == 0 || n == 1) { algebra_output.Text = n.ToString(); return; }
		int coefficient = 1;
		int root = n;
		List<int> factors = GetFactors(n);
		for (int i = 0; i < factors.Count - 1; i++)
		{
			if (factors[i] == factors[i + 1])
			{
				coefficient *= factors[i];
				root /= (factors[i] * factors[i]);
				i++;
			}
		}
		algebra_output.Text = coefficient == 1 ? "" : coefficient.ToString();
		if (root != 1)
		{
			algebra_output.Text += "√" + root.ToString();
			algebra_output.Text += " ≈ " + Math.Round(Math.Sqrt(n), 5);
		}
	}
	private void Solve_Clicked(object sender, EventArgs e)
	{
		algebra_output.TextColor = uneditedColor;
		string entry = algebra_entry.Text;
		if (entry == "") { algebra_output.Text = "Input a polynomial or expression"; return; }

		try { algebra_output.Text = GetSolveOutput(Polynomial.Solve(entry), 4); }
		catch (xException ex) { algebra_output.Text = ex.Message; }
		catch (DivideByPolynomialException ex) { algebra_output.Text = ex.Message; }
		catch (ArgumentException) { algebra_output.Text = "Invalid input"; }
		catch { algebra_output.Text = "Something went wrong"; }
	}

	private static List<int> GetFactors(int n)
	{
		List<int> factors = new List<int>();
		for (int i = 2; n > 1; i++)
		{
			if (n % i == 0)
			{
				factors.Add(i);
				n /= i;
				i--;
			}
		}
		return factors;
	}
	private static bool IsPrime(int n)
	{
		if (n < 0) return false;
		if (n == 2) return true;
		for (int i = 2; i <= Math.Sqrt(n); i++)
		{
			if (n % i == 0) return false;
		}
		return true;
	}
	static string GetSolveOutput(List<double> res, int precision)
	{
		double x;
		if (res.Count == 1)
		{
			x = Math.Round(res[0], precision);
			return "x" + (Math.Round(res[0], 7) == x ? " = " : " ≈ ") + x;
		}
		Dictionary<double, int> solutions = new Dictionary<double, int>();
		for (int i = 0; i < res.Count; i++)
		{
			if (res[i] == -0) res[i] = 0;
			if (!solutions.ContainsKey(res[i])) solutions.Add(res[i], 1);
			else solutions[res[i]]++;
		}
		string output = "";
		int counter = 1;
		foreach (var item in solutions)
		{
			x = Math.Round(item.Key, precision);
			output += "x";
			for (int i = 0; i < item.Value; i++)
			{
				output += counter + ",";
				counter++;
			}
			output = output.Substring(0, output.Length - 1);
			output += (Math.Round(res[counter - 2], 7) == x ? " = " : " ≈ ") + x + "; ";
		}
		output = output.Substring(0, output.Length - 2);
		return output;
	}

	private void Differentiate_Clicked(object sender, EventArgs e)
	{

    }

	private void Integrate_Clicked(object sender, EventArgs e)
	{

	}

	private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
		algebra_output.TextColor = editedColor;
    }
}