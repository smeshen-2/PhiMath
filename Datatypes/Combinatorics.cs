namespace CombinatoricsN;

class CombinatoricsC
{
    public static int P(int n)
    {
        int res = 1;
        for (int i = n; i > 1; i--) res *= i;
        return res;
    }
    public static int V(int k, int n) => P(n) / P(n - k);
    public static int C(int k, int n) => V(k, n) / P(k);
    public static int RepeatP(int n, params int[] arr)
    {
        if (arr.Sum() != n) throw new ArgumentException();
        int res = P(n);
        foreach (var item in arr)
            res /= P(item);
        return res;
    }
    public static int RepeatV(int k, int n) => (int)Math.Pow(n, k);
    public static int RepeatC(int k, int n) => C(k, n + k - 1);
}