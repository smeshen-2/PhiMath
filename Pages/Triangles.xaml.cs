using Microsoft.Maui.Graphics;
using Numbers;

namespace PhiMath.Pages;

public partial class Triangles : ContentPage
{
    private static Dictionary<int, Root> cos;
    private static Color defaultColor;
    private static Color uneditedColor;

    public Triangles()
    {
        InitializeComponent();
        cos = new Dictionary<int, Root>
        {
            { 30, "1/2V3" },
            { 45, "1/2V2" },
            { 60, "1/2" },
            { 90, "0" },
            { 120, "-1/2" },
            { 135, "-1/2V2" },
            { 150, "-1/2V3" }
        };

        if (App.Current.RequestedTheme == AppTheme.Light) defaultColor = Colors.Black;
        else defaultColor = Colors.White;

        if (App.Current.Resources.TryGetValue("Secondary", out var colorvalue)) uneditedColor = (Color)colorvalue;
    }

    private void Solve_Clicked(object sender, EventArgs e)
    {
        Real[] sides = new Real[3];
        int[] angles = new int[3];

        if (a.Text != null && a.Text != "" && a.TextColor == defaultColor) sides[0] = new Real(a.Text);
        else sides[0] = null;
        if (b.Text != null && b.Text != "" && b.TextColor == defaultColor) sides[1] = new Real(b.Text);
        else sides[1] = null;
        if (c.Text != null && c.Text != "" && c.TextColor == defaultColor) sides[2] = new Real(c.Text);
        else sides[2] = null;

        if (alpha.Text != null && alpha.Text != "" && alpha.TextColor == defaultColor) angles[0] = int.Parse(alpha.Text);
        else angles[0] = 0;
        if (beta.Text != null && beta.Text != "" && beta.TextColor == defaultColor) angles[1] = int.Parse(beta.Text);
        else angles[1] = 0;
        if (gamma.Text != null && gamma.Text != "" && gamma.TextColor == defaultColor) angles[2] = int.Parse(gamma.Text);
        else angles[2] = 0;

        var knownSideIndexes = new SortedSet<int>();
        var knownAngleIndexes = new SortedSet<int>();
        for (int i = 0; i < 3; i++)
        {
            if (sides[i] != null) knownSideIndexes.Add(i);
            if (angles[i] != 0) knownAngleIndexes.Add(i);
        }

        if (knownAngleIndexes.Count == 2)
        {
            // figuring out which angle is unknown
            Entry unknownAngle = alpha;
            if (angles[1] == 0) unknownAngle = beta;
            else if (angles[2] == 0) unknownAngle = gamma;

            angles[3 - knownAngleIndexes.Sum()] = 180 - angles.Sum();
            unknownAngle.Text = angles[3 - knownAngleIndexes.Sum()].ToString();
            knownAngleIndexes.Add(3 - knownAngleIndexes.Sum());
            unknownAngle.TextColor = uneditedColor;
        }

        if (knownSideIndexes.Count == 3)
        {
            var perimeter = sides[0] + sides[1] + sides[2];
            perimeter.Simplify();
            P.Text = perimeter.ToString();
        }
        else if (knownSideIndexes.Count == 2 && knownAngleIndexes.Count > 0)
        {
            // figuring out which side is unknown
            Entry unknownSide = a;
            if (sides[1] == null) unknownSide = b;
            if (sides[2] == null) unknownSide = c;

            // if it is an angle between two known sides
            if (knownAngleIndexes.Contains(3 - knownSideIndexes.Sum()))
            {
                // cos theorem
                Real side1 = sides[knownSideIndexes.First()];
                Real side2 = sides[knownSideIndexes.Skip(1).First()];
                int angle = angles[3 - knownSideIndexes.Sum()];
                Real side3 = side1.Squared() + side2.Squared() - ("2" * side1 * side2 * cos[angle]);

                if (side3.Roots.Count == 1)
                    unknownSide.Text = Fraction.Sqrt(side3.Roots[0].A).Simplified().ToString();
                else
                    unknownSide.Text = "√(" + side3.ToString() + ")";

                unknownSide.TextColor = uneditedColor;
            }
        }
        else if (knownSideIndexes.Count == 1 && knownAngleIndexes.Count == 3)
        {

        }
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = (Entry)sender;
        entry.TextColor = defaultColor;
    }
}