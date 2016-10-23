# Threadli/threadsiz matris çarpımı

## Thread sınıfı

Bir adet generic `Matrix<T> where T : new()` sınıfı oluşturdum. 

Bu matrix sınıfının constructorı matrixin boyutlarını parametre olarak almaktadır. `int rowCount, int columnCount`

Matrix sınıfı 'değer' olarak C# dilinin desteklediği bütün değerleri kabul etmektedir. 

Boyut olarak bütün boyutlarda matrix oluşturulabilir ama sadece kare matrixlerin çarpımlarının testini gerçekleştirdim.

Matrixin public/private fieldları
```C#
private int rowCount;
private int columnCount;
static Random rd;
public T[,] Values;
```

Metodlar

```C#
1. public Matrix(int rowCount, int columnCount)
2. public Matrix<T> Multiply(Matrix<T> matrix)
3. public Matrix<T> MultiplyAsync(Matrix<T> matrix)
4. private void MultiplyAsync(Matrix<T> matrix, Matrix<T> result, int row, int column)
5. private static T Add(T current, T y, T z)
6. public bool IsSquare()
7. private static bool IsNumericType(Type type)
8. public override string ToString()
9. public static int[,] RandomSquareArray(int size, int max)
```
2 -> Threadli ve threadsiz sonuçların sürelerini karşılaştırabilmek için threadsiz çarpım versiyonu.
4 -> Metodun threadlere bölündüğü kısmı
5 -> Satır ve sütun çarpıp toplama işleminin toplamasının yapıldığı fonksiyon
7. Sınıf oluşturulurken girilen T tipinin sayısal bir tip olup olmadığını kontrol eder


Threadli matrix çarpımı işlemi
```C#
1 public Matrix<T> MultiplyAsync(Matrix<T> matrix) {
2     var result = new Matrix<T>(columnCount, matrix.rowCount);
3     for (int row = 0; row < rowCount; row++) {
4         for (int column = 0; column < columnCount; column++) {
5             var r = row;
6             var c = column;
7             var t = new Thread(() => MultiplyAsync(matrix, result, r, c));
8             t.Start();
9         }
10    }
11    return result;
12}

private void MultiplyAsync(Matrix<T> matrix, Matrix<T> result, int row, int column) {
    result.Values[row, column] = new T();
    T sum = new T();
    for (int i = 0; i < matrix.rowCount; i++) {
         sum = Add(sum, this.Values[row, i], matrix.Values[i, column]);
    }
    result.Values[row, column] = sum;
}
```


## Program.cs

Süreleri daha gerçekçi bir şekilde karşılaştırabilmek için `MultiplyAsync` ve `Multiply` fonksiyonlarını `int testCount = 10;` kadar çağırıp çalışma zamanlarının en kısa(`long best`) ve en uzun(`long worst`) sürelerini hesapladım. (Satır 40 - 76 )

Süre hesaplaması için kullandığım fonksiyonlardan thread için olanı

```C#
private static TimeSpan MeasureMultiplyRunTimeAsync(Matrix<int> m1, Matrix<int> m2) {
    var stopwatch = Stopwatch.StartNew();
    m1.MultiplyAsync(m2);
    stopwatch.Stop();
    return stopwatch.Elapsed;
}
```

Fonksiyon sadece geriye süre döndürdüğü için satır 79 da baştan hesaplatıp ekrana sonucu yazdırdım.


## Örnek ekran çıktısı

![alt text](https://ryzyyw.dm2304.livefilestore.com/y3m2ilGzPLF_u-MB3PW8A07BPsMnllXKcM3R8hrIenvaXG4xBVEvKBqyGDHooo7I5K-D3w5wCWSG8eKsvhrl_he25GaWds2AnkLmtB8fk4VFCyfgNuBF58cTYBFq_UBkc07evatomAclZ337hzx6JLaVDBGNC0p7Sim6T7lCngV6eQ/matrix_carpim.png?psid=1)

## Sonuç

İlginç bir şekilde threadli olan hesaplama daha yavaş sürmekte. Biraz araştırınca bu sonuca ulaştım:
```
After some more exploration into performance profiling, I have discovered that using a Stopwatch is not an accurate way to measure the performance of a particular task

Reasons a stopwatch are not accurate:
1. Measurements are calculated in elapsed time in milliseconds, not CPU time.
2. Measurements can be influenced by background "noise" and thread intensive processes.
3. Measurements do not take into account JIT compilation and overhead.
```
http://stackoverflow.com/questions/12629032/performance-profiling-in-net
