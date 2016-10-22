using System;
using System.Diagnostics;
using System.IO;
using MyMath;

namespace multi_threaded_matrix_multiplying {
    internal static class Program {

        private static readonly Random rd = new Random();

        private static void Main(string[] args) {
            int size = 100;
            int max = 10;
            Matrix<int> m1 = new Matrix<int>(size, size);
            Matrix<int> m2 = new Matrix<int>(size, size);
            //matrix.Values = new[,] {
            //    {1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
            //    {2, 3, 4, 5, 6, 7, 8, 9, 10, 1},
            //    {3, 4, 5, 6, 7, 8, 9, 10, 1, 2},
            //    {4, 5, 6, 7, 8, 9, 10, 1, 2, 3},
            //    {5, 6, 7, 8, 9, 10, 1, 2, 3, 4},
            //    {6, 7, 8, 9, 10, 1, 2, 3, 4, 5},
            //    {7, 8, 9, 10, 1, 2, 3, 4, 5, 6},
            //    {8, 9, 10, 1, 2, 3, 4, 5, 6, 7},
            //    {9, 10, 1, 2, 3, 4, 5, 6, 7, 8},
            //    {10, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            //};

            m1.Values = RandomSquareArray(size, max);
            m2.Values = RandomSquareArray(size, max);

            long best = long.MaxValue;
            long worst = long.MinValue;
            int testCount = 100;

            for (int i = 0; i < testCount; i++) {
                var currentTicks = MeasureFunctionRunTime(m1, m2).Ticks;
                if (currentTicks < best) {
                    best = currentTicks;
                }
                if (currentTicks > worst) {
                    worst = currentTicks;
                }
            }

            Console.WriteLine("- Without Threads");
            Console.WriteLine($"Best Time  :{best,10}");
            Console.WriteLine($"Worst Time :{worst,10}\n");

            best = long.MaxValue;
            worst = long.MinValue;
            for (int i = 0; i < testCount; i++) {
                var currentTicks = MeasureFunctionRunTimeAsync(m1, m2).Ticks;
                if (currentTicks < best) {
                    best = currentTicks;
                }
                if (currentTicks > worst) {
                    worst = currentTicks;
                }
            }
            Console.WriteLine("- With Threads");
            Console.WriteLine($"Best Time  :{best,10}");
            Console.WriteLine($"Worst Time :{worst,10}");



            //            Console.WriteLine(matrix);
            //            Console.WriteLine();

            //            var m1 = matrix.Multiply(matrix);
            //            Console.WriteLine(m1);
            //            Console.WriteLine();

            //            var m2 = matrix.MultiplyAsync(matrix);
            //            Console.WriteLine(m2);
            //            Console.WriteLine();

            End();
        }
        

        private static int[,] RandomSquareArray(int size, int max) {
            var array = new int[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    array[i, j] = rd.Next(max);
                }
            }
            return array;
        }

        private static void End() {
            Console.WriteLine("End of Program");
            Console.ReadKey(true);
        }

        private static TimeSpan MeasureFunctionRunTime(Matrix<int> m1, Matrix<int> m2) {
            var stopwatch = Stopwatch.StartNew();
            m1.Multiply(m2);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan MeasureFunctionRunTimeAsync(Matrix<int> m1, Matrix<int> m2)
        {
            var stopwatch = Stopwatch.StartNew();
            m1.MultiplyAsync(m2);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

    }
}