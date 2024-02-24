using System.Text.RegularExpressions;

namespace WordEvolution;

class Evolver
{
    private static Dictionary<char, HashSet<char>> groups = new Dictionary<char, HashSet<char>>();
    public static string Separator = "/";
    public static void AddGroup(char name, IEnumerable<char> group)
    {
        groups.Add(name, group.ToHashSet());
    }
    private static string GroupEvolve(string word, string rule) // one group evolves into another
    {
        string[] strings = rule.Split(Separator);
        string beforeGroup = string.Join(null, groups[strings[0][0]]);
        string afterGroup = string.Join(null, groups[strings[1][0]]);
        string condition = strings.Length == 2 ? "_" : strings[2];
        string pattern = Escape(condition.Replace("_", strings[0]));

        foreach (char letter in pattern)
            if (groups.ContainsKey(letter)) pattern = pattern.Replace(letter.ToString(), "[" + Escape(string.Join("", groups[letter])) + "]");

        MatchCollection matches = Regex.Matches(word, pattern);
        foreach (Match match in matches)
        {
            string oldValue = match.Value;
            int index = condition.IndexOf('_');
            char after = afterGroup[beforeGroup.IndexOf(oldValue[index])];
            string newValue = oldValue.Substring(0, index) + after + oldValue.Substring(index + 1);
            word = word.Replace(oldValue, newValue);
        }
        return word;
    }
    public static string Evolve(string word, string rule) // any evolution that doesn't have a group as 'after'
    {
        string[] strings = rule.Split(Separator);
        string before = strings[0];
        string after = strings[1];
        if (after.Length > 0 && groups.ContainsKey(after[0])) return GroupEvolve(word, rule);
        string condition = strings.Length == 2 ? "_" : strings[2];
        string pattern = Escape(condition.Replace("_", before));

        foreach (char letter in pattern)
            if (groups.ContainsKey(letter)) pattern = pattern.Replace(letter.ToString(), "[" + Escape(string.Join("", groups[letter])) + "]");

        MatchCollection matches = Regex.Matches(word, pattern);
        foreach (Match match in matches)
        {
            string oldValue = match.Value;
            int index = condition.IndexOf('_');
            string newValue = oldValue.Substring(0, index) + after + oldValue.Substring(index + before.Length);
            word = word.Replace(oldValue, newValue);
        }
        return word;
    }
    public static IEnumerable<string> GroupEvolve(IEnumerable<string> words, string rule)
    {
        return words.Select(w => w = GroupEvolve(w, rule));
    }
    public static IEnumerable<string> Evolve(IEnumerable<string> words, string rule)
    {
        return words.Select(w => w = Evolve(w, rule));
    }

    public static string Escape(string expr)
    {
        return Regex.Escape(expr).Replace("-", "\\-");
    }
}