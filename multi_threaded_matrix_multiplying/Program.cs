using System;
using System.Diagnostics;
using System.IO;
using MyMath;

namespace multi_threaded_matrix_multiplying {
    internal static class Program {

        private static readonly Random rd = new Random();

        private static void Main(string[] args) {
            int size = 3; // matrixin n*x lik boyutu
            int maxValueOfNumbers = 10; // 0 ile bu deger arasinda rastgele degerler atamasi icin
            Matrix<int> m1 = new Matrix<int>(size, size);
            Matrix<int> m2 = new Matrix<int>(size, size);

            //random olmayan 10x10 luk bir matrix
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

            //matrixlere random degerler atanmasi
            m1.Values = Matrix<int>.RandomSquareArray(size, maxValueOfNumbers);
            m2.Values = Matrix<int>.RandomSquareArray(size, maxValueOfNumbers);

            /* Hesaplama suresini en iyi sekilde karsilastirmak icin
             çarpma işlemini birden fazla kez çalıştırıp en iyi ve 
             en kötü sürelere bakılmıştır.
             */

            //en iyi süre değişkeni
            long best = long.MaxValue;
            //en kötü süre değişkeni
            long worst = long.MinValue;
            //deneme sayısı
            int testCount = 10;

            for (int i = 0; i < testCount; i++) {
                var currentTicks = MeasureMultiplyRunTime(m1, m2).Ticks;
                Console.WriteLine($"{i + 1}. {currentTicks}");
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
                var currentTicks = MeasureMultiplyRunTimeAsync(m1, m2).Ticks;
                Console.WriteLine($"{i + 1}. {currentTicks}");
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
            
            Console.WriteLine("-- RESULT --");
            Console.WriteLine(m1.Multiply(m2));
            Console.WriteLine("------------");
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

        /// <summary>
        /// iki boyutlu integer degerlerden olusan kare bir matrix olusturur
        /// </summary>
        /// <param name="size">matrixin boyutu</param>
        /// <param name="max">matrix icindeki degerlerin alabilecegi maksimum deger</param>
        /// <returns></returns>


        private static void End() {
            Console.WriteLine("#-- End of Program --#");
            Console.ReadKey(true);
        }

        /// <summary>
        /// threadsiz olarak çarpma işleminin süresini hesaplama
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private static TimeSpan MeasureMultiplyRunTime(Matrix<int> m1, Matrix<int> m2) {
            var stopwatch = Stopwatch.StartNew();
            m1.Multiply(m2);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// threadli olarak matrix çarpmasının süresini hesaplama
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private static TimeSpan MeasureMultiplyRunTimeAsync(Matrix<int> m1, Matrix<int> m2) {
            var stopwatch = Stopwatch.StartNew();
            m1.MultiplyAsync(m2);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

    }
}