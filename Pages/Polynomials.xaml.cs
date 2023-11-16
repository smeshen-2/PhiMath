using Nomials;
using Exceptions;

namespace PhiMath.Pages;

public partial class Polynomials : ContentPage
{
    public Polynomials()
    {
        InitializeComponent();
    }
    private void Simplify_Clicked(object sender, EventArgs e)
    {
        try
        {
            string expr = polynomials_entry.Text
                .Replace("⁰", "^0")
                .Replace("¹", "^1")
                .Replace("²", "^2")
                .Replace("³", "^3")
                .Replace("⁴", "^4")
                .Replace("⁵", "^5")
                .Replace("⁶", "^6")
                .Replace("⁷", "^7")
                .Replace("⁸", "^8")
                .Replace("⁹", "^9");
            polynomials_output.Text = Polynomial.Simplify(expr).ToString();
        }
        catch
        {
            polynomials_output.Text = "Invalid expression";
            return;
        }
    }

    private void Solve_Clicked(object sender, EventArgs e)
    {
        polynomials_output.Text = "";
        try
        {
            string expr = polynomials_entry.Text
                .Replace("⁰", "^0")
                .Replace("¹", "^1")
                .Replace("²", "^2")
                .Replace("³", "^3")
                .Replace("⁴", "^4")
                .Replace("⁵", "^5")
                .Replace("⁶", "^6")
                .Replace("⁷", "^7")
                .Replace("⁸", "^8")
                .Replace("⁹", "^9");
            polynomials_output.Text = GetSolveOutput(Polynomial.Simplify(expr), Polynomial.Solve(expr));
        }
        catch (AxeQException)
        {
            polynomials_output.Text = "∀x∈ℚ";
        }
        catch (xeOException)
        {
            polynomials_output.Text = "x∈∅";
        }
        catch
        {
            polynomials_output.Text = "Invalid expression";
            try
            {
                polynomials_output.Text = "";

                string expr = polynomials_entry.Text
               .Replace("⁰", "^0")
               .Replace("¹", "^1")
               .Replace("²", "^2")
               .Replace("³", "^3")
               .Replace("⁴", "^4")
               .Replace("⁵", "^5")
               .Replace("⁶", "^6")
               .Replace("⁷", "^7")
               .Replace("⁸", "^8")
               .Replace("⁹", "^9");
                List<double> res = Polynomial.Solve(expr);

                foreach (var item in res)
                {
                    polynomials_output.Text += item + "; ";
                }
            }
            catch
            {
                polynomials_output.Text = "Invalid";
            }
            return;
        }
    }
    static string GetSolveOutput(Polynomial p, List<double> res)
    {
        res.Sort();
        double x;
        if (res.Count == 1)
        {
            x = Math.Round(res[0], 5);
            if (p.Power > 2) return "x1 " + (x == res[0] ? "= " : "≈ ") + x + "...";
            return "x " + (x == res[0] ? "= " : "≈ ") + x;
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
            x = Math.Round(item.Key, 5);
            output += "x";
            for(int i = 0; i < item.Value; i++)
            {
                output += counter + ",";
                counter++;
            }
            output = output.Substring(0, output.Length - 1);
            //output += (res[counter - 1] == x ? " = " : " ≈ ") + x + "; ";
            output += " = " + x + "; ";
        }
        output = output.Substring(0, output.Length - 2);
        if (res.Count != p.Power) output += "...";
        return output;
    }
}