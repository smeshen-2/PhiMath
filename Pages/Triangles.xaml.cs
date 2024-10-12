using Microsoft.Maui.Graphics;
using Numbers;

namespace PhiMath.Pages;

public partial class Triangles : ContentPage
{
    private static Dictionary<int, Root> sin, cos;
    private static Color defaultColor, uneditedColor;

    public Triangles()
    {
        InitializeComponent();
        sin = new Dictionary<int, Root>
        {
            { 30, "1/2" },
            { 45, "V2/2" },
            { 60, "V3/2" },
            { 90, "1" },
            { 120, "V3/2" },
            { 135, "V2/2" },
            { 150, "1/2" }
        };

        cos = new Dictionary<int, Root>
        {
            { 30, "V3/2" },
            { 45, "V2/2" },
            { 60, "1/2" },
            { 90, "0" },
            { 120, "-1/2" },
            { 135, "-V2/2" },
            { 150, "-V3/2" }
        };

        if (App.Current.RequestedTheme == AppTheme.Light) defaultColor = Colors.Black;
        else defaultColor = Colors.White;

        if (App.Current.Resources.TryGetValue("Secondary", out var colorvalue)) uneditedColor = (Color)colorvalue;
    }

    private void Solve_Clicked(object sender, EventArgs e)
    {
        try
        {
            RootSum[] sides = new RootSum[3];
            int[] angles = new int[3];

            Entry[] entries = { a, b, c, alpha, beta, gamma, S, P, R, r };
            entries.Where(entry => IsGiven(entry) == false).ToList()
                .ForEach(entry => entry.Text = "");

            if (IsGiven(a)) sides[0] = new RootSum(a.Text);
            else sides[0] = null;
            if (IsGiven(b)) sides[1] = new RootSum(b.Text);
            else sides[1] = null;
            if (IsGiven(c)) sides[2] = new RootSum(c.Text);
            else sides[2] = null;

            if (IsGiven(alpha)) angles[0] = int.Parse(alpha.Text);
            else angles[0] = 0;
            if (IsGiven(beta)) angles[1] = int.Parse(beta.Text);
            else angles[1] = 0;
            if (IsGiven(gamma)) angles[2] = int.Parse(gamma.Text);
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
                CalculatePerimeter(sides);
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
                    RootSum side1 = sides[knownSideIndexes.First()];
                    RootSum side2 = sides[knownSideIndexes.Skip(1).First()];
                    int angle = angles[3 - knownSideIndexes.Sum()];
                    if (cos.ContainsKey(angle) == false) return;
                    RootSum side3Squared = side1.Squared() + side2.Squared() - ("2" * side1 * side2 * cos[angle]);

                    
                    RootSum area = CalculateArea(side1, side2, angle);

                    if (side3Squared.Roots.Count == 1)
                    {
                        RootSum side3 = new RootSum(Fraction.Sqrt(side3Squared.Roots[0].A).Simplified());
                        unknownSide.Text = side3.ToString();

                        // getting P
                        RootSum perimeter = CalculatePerimeter(side1, side2, side3);

                        CalculateBigRadius(angle, side3.ToString());

                        // S = p * r (to get r)
                        if (perimeter.Roots.Count == 1)
                        {
                            RootSum smallRadius = "2" * area / perimeter.Roots[0];
                            r.Text = smallRadius.ToString();
                        }
                        else
                        {
                            r.Text = "2" * area / perimeter;
                        }
                    }
                    else
                    {
                        string side3 = "√(" + side3Squared.ToString() + ")";
                        unknownSide.Text = side3;

                        // getting P
                        RootSum perimeter = side1 + side2;
                        P.Text = perimeter.ToString() + "+" + side3;

                        CalculateBigRadius(angle, side3);

                        // S = p * r (to get r)
                        var t = "2" * area;
                        r.Text = t.ToString() + "/(" + P.Text + ")";
                    }
                    unknownSide.TextColor = uneditedColor;
                    P.TextColor = uneditedColor;
                    R.TextColor = uneditedColor;
                    r.TextColor = uneditedColor;
                }
                else
                {
                    RootSum oppositeSide = sides[knownAngleIndexes.First()];
                    RootSum knownAdjacentSide = sides[knownSideIndexes.First(i => i != knownAngleIndexes.First())];
                    int angle = angles[knownAngleIndexes.First()];
                    if (cos.ContainsKey(angle) == false) return;
                    RootSum discriminant = "4" * knownAdjacentSide.Squared() * new Root(cos[angle].Squared()) - ("4" * (knownAdjacentSide.Squared() - oppositeSide.Squared()));
                    discriminant.Simplify();
                    if (discriminant.Roots.Count == 1 && discriminant.Roots[0].B <= 1 && discriminant.Roots[0] >= new Root(0))
                    {
                        RootSum side3 = "1/2" * ("2" * knownAdjacentSide * cos[angle] + new Root("V" + discriminant.ToString()));
                        side3.Simplify();
                        unknownSide.Text = side3.ToString();
                        unknownSide.TextColor = uneditedColor;
                        for (int i = 0; i < 3; i++)
                        {
                            if (sides[i] == null)
                            {
                                sides[i] = side3;
                                break;
                            }
                        }
                        CalculatePerimeter(sides);
                        CalculateArea(knownAdjacentSide, side3, angle);
                        CalculateBigRadius(angle, oppositeSide.ToString());
                    }

                }
            }
            else if (knownSideIndexes.Count == 1 && knownAngleIndexes.Count == 3)
            {
                CalculateBigRadius(angles[knownSideIndexes.First()], sides[knownSideIndexes.First()].ToString());
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Calculates R using the sine theorem
    /// </summary>
    private RootSum CalculateBigRadius(int angle, string side)
    {
        RootSum radiusValue = "1";
        Root d = sin[angle] * "2";
        try
        {
            radiusValue  = new RootSum(side) / d;
        }
        catch
        {
            string bigRadius = d == "1" ? side : side + "/" + d.ToString();
            R.Text = bigRadius;
            R.TextColor = uneditedColor;
            return null;
        }
        radiusValue.Simplify();
        R.Text = radiusValue.ToString();
        R.TextColor = uneditedColor;
        return radiusValue;
    }

    /// <summary>
    /// Calculates the are using the formula: S = 1/2 * a * b * sin(angle)
    /// </summary>
    /// <returns>s</returns>
    private RootSum CalculateArea(RootSum side1, RootSum side2, int angle)
    {
        RootSum area = "1/2" * side1 * side2 * sin[angle];
        S.Text = area.ToString();
        S.TextColor = uneditedColor;
        return area;
    }

    private RootSum CalculatePerimeter(params RootSum[] sides)
    {
        RootSum perimeter = sides[0] + sides[1] + sides[2];
        P.Text = perimeter.ToString();
        P.TextColor = uneditedColor;
        return perimeter;
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = (Entry)sender;
        entry.TextColor = defaultColor;
    }

    private static bool IsGiven(Entry e)
    {
        return e.Text != null && e.Text != "" && e.TextColor == defaultColor;
    }
}