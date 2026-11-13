using System.Numerics;

namespace Object3;

internal static class MatrixMath
{

    // алгоритм Барейса (без використання чисел із плаваючою комою)
    internal static BigInteger Determinant(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        BigInteger[,] a = new BigInteger[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i, j] = matrix[i, j];
            }
        }

        BigInteger previousPivot = 1;
        int sign = 1;

        for (int k = 0; k < n - 1; k++)
        {
            int pivotRow = k;

            while (pivotRow < n && a[pivotRow, k] == 0)
            {
                pivotRow++;
            }

            if (pivotRow == n)
            {
                return BigInteger.Zero;
            }

            if (pivotRow != k)
            {
                for (int j = 0; j < n; j++)
                {
                    (a[k, j], a[pivotRow, j]) =
                        (a[pivotRow, j], a[k, j]);
                }

                sign = -sign;
            }

            BigInteger pivot = a[k, k];

            for (int i = k + 1; i < n; i++)
            {
                for (int j = k + 1; j < n; j++)
                {
                    a[i, j] =
                        (a[i, j] * pivot - a[i, k] * a[k, j])
                        / previousPivot;
                }
            }

            for (int i = k + 1; i < n; i++)
            {
                a[i, k] = 0;
            }

            previousPivot = pivot;
        }

        return sign * a[n - 1, n - 1];
    }
}