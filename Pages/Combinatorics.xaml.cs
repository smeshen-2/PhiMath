using CombinatoricsN;

namespace PhiMath.Pages;

public partial class Combinatorics : ContentPage
{
    public Combinatorics()
    {
        InitializeComponent();
    }

    private void Calculate_Clicked(object sender, EventArgs e)
    {
        int n, k;
        int[] a;
        switch (Picker.SelectedItem)
        {
            case "P":
                if (!int.TryParse(P_entry.Text, out n)) return;
                comb_output.Text = CombinatoricsC.P(n).ToString();
                return;
            case "V":
                if (!int.TryParse(k_entry.Text, out k)) return;
                if (!int.TryParse(n_entry.Text, out n)) return;
                comb_output.Text = CombinatoricsC.V(k, n).ToString();
                return;
            case "C":
                if (!int.TryParse(k_entry.Text, out k)) return;
                if (!int.TryParse(n_entry.Text, out n)) return;
                comb_output.Text = CombinatoricsC.C(k, n).ToString();
                return;
            case "P̃":
                if (!int.TryParse(Pn_entry.Text, out n)) return;
                try { a = Pa_entry.Text.Replace(" ", "").Split(',').Select(int.Parse).ToArray(); }
                catch { return; }
                try { comb_output.Text = CombinatoricsC.RepeatP(n, a).ToString(); }
                catch { comb_output.Text = "Invalid input"; }
                return;
            case "Ṽ":
                if (!int.TryParse(k_entry.Text, out k)) return;
                if (!int.TryParse(n_entry.Text, out n)) return;
                comb_output.Text = CombinatoricsC.RepeatV(k, n).ToString();
                return;
            case "C̃":
                if (!int.TryParse(k_entry.Text, out k)) return;
                if (!int.TryParse(n_entry.Text, out n)) return;
                comb_output.Text = CombinatoricsC.RepeatC(k, n).ToString();
                return;
        }
    }
    private void Picker_Changed(object sender, EventArgs e)
    {
        Picker p = (Picker)sender;
        if (p.SelectedIndex == 0)
        {
            P_entry.IsVisible = true;
            k_entry.IsVisible = false;
            n_entry.IsVisible = false;
            Pn_entry.IsVisible = false;
            Pa_entry.IsVisible = false;
            return;
        }
        if (p.SelectedIndex == 3)
        {
            P_entry.IsVisible = false;
            k_entry.IsVisible = false;
            n_entry.IsVisible = false;
            Pn_entry.IsVisible = true;
            Pa_entry.IsVisible = true;
            return;
        }
        P_entry.IsVisible = false;
        k_entry.IsVisible = true;
        n_entry.IsVisible = true;
        Pn_entry.IsVisible = false;
        Pa_entry.IsVisible = false;
    }
}