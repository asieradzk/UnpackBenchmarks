// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using IEnumerableUnpacker;

/*

List<UnpackMe<string, string, string>> inspectThis = UnpackMe<string, string, string>.CreateUnpackMe(3, 3, 3, 2);
//print all the values

inspectThis.PrintToConsole();

inspectThis.UnpackMeSIMD(out int[,] myIntegers, out int[] myInteger, out float[,] myFloats, out string[,] myGeneric, out string[] myGeneric2);

foreach (var item in myIntegers)
{
    Console.WriteLine(string.Join(", ", item));
}

Console.WriteLine(string.Join(", ", myInteger));

foreach (var item in myFloats)
{
    Console.WriteLine(string.Join(", ", item));
}
foreach (var item in myGeneric)
{
    Console.WriteLine(string.Join(", ", item));
}
*/


//UnpackMeExtensions.UnpackUnpackMe();




var summary = BenchmarkRunner.Run<UnpackBenchmarks>();


