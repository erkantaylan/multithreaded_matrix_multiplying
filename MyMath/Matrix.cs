﻿using System;
using System.Text;
using System.Threading;

namespace MyMath {
    /// <summary>
    /// Matrix islemeri icin generic class
    /// </summary>
    /// <typeparam name="T">Sadece sayisal degiskenler olabilir</typeparam>
    public class Matrix<T> where T : new() {

        private int rowCount;
        private int columnCount;
        static Random rd;


        public T[,] Values;

        public Matrix(int rowCount, int columnCount) {
            if (!IsNumericType(typeof(T))) {
                throw new UnexpectedValueTypeException($"{typeof(T)} is not valid type. Only numeric types are allowed!");
            }

            RowCount = rowCount;
            ColumnCount = columnCount;

            Values = new T[rowCount, columnCount];

            //rd static oldugu icin baska bir obje tarafindan atama yapilmamissa atamasinin gerceklestirilmesi
            //random isleminin daha guveneilir olamsi icin tek bir random olusturup onun Next degerleri alinmistir
            if (rd == null) {
                rd = new Random();
            }
        }

        public int ColumnCount {
            get { return columnCount; }
            set {
                if (value > 0) {
                    columnCount = value;
                }
                else {
                    throw new MatrixSizeError(MatrixSizeError.NotPossitiveMessage);
                }
            }
        }

        public int RowCount {
            get { return rowCount; }
            set {
                if (value > 0) {
                    rowCount = value;
                }
                else {
                    throw new MatrixSizeError(MatrixSizeError.NotPossitiveMessage);
                }
            }
        }

        public Matrix<T> Multiply(Matrix<T> matrix) {
            var result = new Matrix<T>(columnCount, matrix.rowCount);
            for (int row = 0; row < rowCount; row++) {
                for (int column = 0; column < columnCount; column++) {
                    result.Values[row, column] = new T();
                    for (int i = 0; i < matrix.rowCount; i++) {
                        result.Values[row, column] = Add(result.Values[row, column], this.Values[row, i], matrix.Values[i, column]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Martixin toplama islemi
        /// A x B = C olsun
        /// </summary>
        /// <param name="current">C[x,y]</param>
        /// <param name="y">A[x,i]</param>
        /// <param name="z">B[i,y]</param>
        /// <returns></returns>
        private static T Add(T current, T y, T z) {
            dynamic tx = current, ty = y, tz = z;
            return tx + (ty * tz);
        }

        /// <summary>
        /// Threadler kullanarak carpma islemi
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix<T> MultiplyAsync(Matrix<T> matrix) {
            var result = new Matrix<T>(columnCount, matrix.rowCount);
            for (int row = 0; row < rowCount; row++) {
                for (int column = 0; column < columnCount; column++) {
                    var r = row;
                    var c = column;
                    var t = new Thread(() => MultiplyAsync(matrix, result, r, c));
                    t.Start();
                }
            }
            return result;
        }

        private void MultiplyAsync(Matrix<T> matrix, Matrix<T> result, int row, int column) {
            result.Values[row, column] = new T();
            T sum = new T();
            for (int i = 0; i < matrix.rowCount; i++) {
                 sum = Add(sum, this.Values[row, i], matrix.Values[i, column]);
            }
            result.Values[row, column] = sum;
        }


        public bool IsSquare() {
            return rowCount == columnCount;
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// http://stackoverflow.com/a/5182747
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        private static bool IsNumericType(Type type) {
            while (true) {
                if (type == null) {
                    return false;
                }

                switch (Type.GetTypeCode(type)) {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                    case TypeCode.Object:
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                            type = Nullable.GetUnderlyingType(type);
                            continue;
                        }
                        return false;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        break;
                    case TypeCode.Char:
                        break;
                    case TypeCode.DateTime:
                        break;
                    case TypeCode.String:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return false;
            }
        }


        public override string ToString() {
            var builder = new StringBuilder();
            for (int i = 0; i < rowCount; i++) {
                for (int j = 0; j < columnCount; j++) {
                    builder.Append($"{Values[i, j],5:##00.####} ");
                }
                if (i + 1 != rowCount) {
                    builder.Append(Environment.NewLine);
                }
            }
            return builder.ToString();
        }

        public static int[,] RandomSquareArray(int size, int max)
        {
            var array = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++) {
                    array[i, j] = rd.Next(max + 1);
                }
            }
            return array;
        }
    }

    public class UnexpectedValueTypeException : Exception {
        public UnexpectedValueTypeException() {}

        public UnexpectedValueTypeException(string message) : base(message) {}

        public UnexpectedValueTypeException(string message, Exception inner) : base(message, inner) {}
    }

    public class MatrixSizeError : Exception {

        public static string NotPossitiveMessage = "Matrix column size cannot be negative or zero!";

        public MatrixSizeError() {}

        public MatrixSizeError(string message) : base(message) {}

        public MatrixSizeError(string message, Exception inner) : base(message, inner) {}
    }
}