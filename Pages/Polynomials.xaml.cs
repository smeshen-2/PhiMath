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
        try
        {
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
        string expr = polynomials_entry.Text ?? "";
        expr = expr
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
        try
        {
            List<double> res = Polynomial.Solve(expr);
            if (res.Count == 2)
            {
                double x1 = Math.Round(res[0], 5);
                double x2 = Math.Round(res[1], 5);
                polynomials_output.Text = "x1 " + (x1 == res[0] ? "= " : "≈ ") + x1 +
                    ", x2 " + (x2 == res[1] ? "= " : "≈ ") + x2;
                return;
            }
            else if (res.Count == 1)
            {
                double x = Math.Round(res[0], 5);
                polynomials_output.Text = "x " + (x == res[0] ? "= " : "≈ ") + x;
                return;
            }
        }
        catch(AxeQException)
        {
            polynomials_output.Text = "∀x∈ℚ";
        }
        catch(xeOException)
        {
            polynomials_output.Text = "x∈∅";
        }
        catch
        {
            polynomials_output.Text = "Invalid expression";
            return;
        }
    }
}