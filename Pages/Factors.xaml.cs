namespace PhiMath.Pages;

public partial class Factors : ContentPage
{
	public Factors()
	{
		InitializeComponent();
	}
    private void Factor_Clicked(object sender, EventArgs e)
    {
        int n;
        try
        {
            n = int.Parse(factor_entry.Text);
        }
        catch
        {
            factor_output.Text = "Invalid number";
            return;
        }
        if (n == 0 || n == 1)
        {
            factor_output.Text = "Няма пък да ти кажа";
            return;
        }
        if (IsPrime(n))
        {
            factor_output.Text = "Prime";
            return;
        }
        factor_output.Text = string.Join(", ", GetFactors(n));
    }
    private void Sqrt_Clicked(object sender, EventArgs e)
    {
        int n;
        try
        {
            n = int.Parse(factor_entry.Text);
            if (n < 0) throw new Exception();
        }
        catch
        {
            factor_output.Text = "Invalid number";
            return;
        }
        if (n == 0 || n == 1)
        {
            factor_output.Text = "Няма пък да ти кажа";
            return;
        }
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
        factor_output.Text = coefficient == 1 ? "" : coefficient.ToString();
        if (root != 1)
        {
            factor_output.Text += "√" + root.ToString();
            factor_output.Text += " ≈ " + Math.Round(Math.Sqrt(n), 5);
        }
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
        if (n == 2) return true;
        for (int i = 2; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0) return false;
        }
        return true;
    }
}