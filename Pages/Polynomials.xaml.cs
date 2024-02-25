using Nomials;
using Exceptions;
using WordEvolution;

namespace PhiMath.Pages;

public partial class Polynomials : ContentPage
{
    public Polynomials()
    {
        InitializeComponent();
        Evolver.AddGroup('N', "0123456789x");
        Evolver.AddGroup('O', "+-*/()^");
        Evolver.Separator = "|";
    }
    private void Normalize_Clicked(object sender, EventArgs e)
    {
        string expr;
        try { expr = polynomials_entry.Text; if (expr == "") throw new Exception(); }
        catch { polynomials_output.Text = "Invalid input"; return; }

        try { polynomials_output.Text = Polynomial.Normalize(expr).ToString(); }
        catch (DivideByPolynomialException ex) { polynomials_output.Text = ex.Message; }
        catch (ArgumentException) { polynomials_output.Text = "Invalid input"; }
        catch { polynomials_output.Text = "Something went wrong..."; }
    }

    private void Solve_Clicked(object sender, EventArgs e)
    {
        polynomials_output.Text = "";
        string expr;
        try { expr = polynomials_entry.Text; if (expr == "") throw new Exception(); }
        catch { polynomials_output.Text = "Invalid input"; return; }

        try { polynomials_output.Text = GetSolveOutput(Polynomial.ParseEquation(expr), Polynomial.Solve(expr), 4); }
        catch (xException ex) { polynomials_output.Text = ex.Message; }
        catch (DivideByPolynomialException ex) { polynomials_output.Text = ex.Message; }
        catch (ArgumentException) { polynomials_output.Text = "Invalid input"; }
        catch { polynomials_output.Text = "Something went wrong..."; }
    }
    static string GetSolveOutput(Polynomial p, List<double> res, int precision)
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
}