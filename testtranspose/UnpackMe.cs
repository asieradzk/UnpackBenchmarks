// See https://aka.ms/new-console-template for more information
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Runtime.CompilerServices;
using IEnumerableUnpacker;


[Unpackable]
public class UnpackMe<Titem, Titem2, UselessGeneric>
{
    [Unpack("MyItegersOut")]
    public int[] myIntegers;
    [Unpack("MyIntegerOut")]
    public int myInteger;
    [Unpack("MyFloatsOut")]
    public float[] myFloats;
    [Unpack("MyGenericOut")]
    public Titem[] myGeneric;
    [Unpack("MyGeneric2Out")]
    public Titem2 myGeneric2;

    public UnpackMe(int[] integers, int integer, float[] floats, Titem[] generic, Titem2 generic2)
    {
        myIntegers = integers;
        myInteger = integer;
        myFloats = floats;
        myGeneric = generic;
        myGeneric2 = generic2;
    }

    public static List<UnpackMe<string, string, string>> CreateUnpackMe(int lengthIntegers, int lengthFloats, int lengthGenerics, int numberOfItems)
    {
        Random random = new Random();
        var result = new List<UnpackMe<string, string, string>>();

        for (int n = 0; n < numberOfItems; n++)
        {
            int[] myIntegers = new int[lengthIntegers];
            float[] myFloats = new float[lengthFloats];
            string[] myGeneric = new string[lengthGenerics];

            for (int i = 0; i < lengthIntegers; i++)
            {
                myIntegers[i] = random.Next(); // Generate a random integer
            }

            for (int i = 0; i < lengthFloats; i++)
            {
                myFloats[i] = (float)(random.NextDouble() * 100); // Generate a random float
            }

            for (int i = 0; i < lengthGenerics; i++)
            {
                myGeneric[i] = "Item " + random.Next(); // Generate a random string item
            }

            var transposeMe = new UnpackMe<string, string, string>(
                myIntegers,
                random.Next(), // Random integer value
                myFloats,
                myGeneric,
                "Generic2 String" // Random string value
            );

            result.Add(transposeMe);
        }

        return result;
    }
}

public static class FLattenMeExtensions2
{



    public static unsafe void UnpackMeNormalLoop<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> transposeMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var transposeMeArray = transposeMe.ToArray();
        int length = transposeMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = transposeMeArray[0].myIntegers.Length;
        var floatLength = transposeMeArray[0].myFloats.Length;
        var genericLength = transposeMeArray[0].myGeneric.Length;

        myIntegersOut = new int[length, integerLength];
        myIntegerOut = new int[length];
        myFloatsOut = new float[length, floatLength];
        myGenericOut = new Titem[length, genericLength];
        myGeneric2Out = new Titem2[length];

        for (int i = 0; i < length; i++)
        {
            var item = transposeMeArray[i];
            myIntegerOut[i] = item.myInteger;
            myGeneric2Out[i] = item.myGeneric2;

            for (int j = 0; j < integerLength; j++)
            {
                myIntegersOut[i, j] = item.myIntegers[j];
            }

            for (int j = 0; j < floatLength; j++)
            {
                myFloatsOut[i, j] = item.myFloats[j];
            }

            for (int j = 0; j < genericLength; j++)
            {
                myGenericOut[i, j] = item.myGeneric[j];
            }
        }
    }
    public static unsafe void UnpackMeSIMD<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> transposeMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var transposeMeArray = transposeMe.ToArray();
        int length = transposeMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = transposeMeArray[0].myIntegers.Length;
        var floatLength = transposeMeArray[0].myFloats.Length;
        var genericLength = transposeMeArray[0].myGeneric.Length;

        myIntegersOut = new int[length, integerLength];
        myIntegerOut = new int[length];
        myFloatsOut = new float[length, floatLength];
        myGenericOut = new Titem[length, genericLength];
        myGeneric2Out = new Titem2[length];

        fixed (int* pMyIntegersOut = myIntegersOut)
        fixed (int* pMyIntegerOut = myIntegerOut)
        fixed (float* pMyFloatsOut = myFloatsOut)
        {
            for (int i = 0; i < length; i++)
            {
                var item = transposeMeArray[i];
                pMyIntegerOut[i] = item.myInteger;
                myGeneric2Out[i] = item.myGeneric2;

                int* pIntegersDest = pMyIntegersOut + i * integerLength;
                float* pFloatsDest = pMyFloatsOut + i * floatLength;

                fixed (int* pIntegersSrc = item.myIntegers)
                fixed (float* pFloatsSrc = item.myFloats)
                {
                    int integersRemaining = integerLength;
                    int floatsRemaining = floatLength;

                    int* pIntegersCur = pIntegersSrc;
                    float* pFloatsCur = pFloatsSrc;

                    while (integersRemaining >= 8)
                    {
                        Vector256<int> integers = Avx.LoadVector256(pIntegersCur);
                        Avx.Store(pIntegersDest, integers);
                        pIntegersCur += 8;
                        pIntegersDest += 8;
                        integersRemaining -= 8;
                    }

                    while (floatsRemaining >= 8)
                    {
                        Vector256<float> floats = Avx.LoadVector256(pFloatsCur);
                        Avx.Store(pFloatsDest, floats);
                        pFloatsCur += 8;
                        pFloatsDest += 8;
                        floatsRemaining -= 8;
                    }

                    for (int j = 0; j < integersRemaining; j++)
                    {
                        pIntegersDest[j] = pIntegersCur[j];
                    }

                    for (int j = 0; j < floatsRemaining; j++)
                    {
                        pFloatsDest[j] = pFloatsCur[j];
                    }
                }

                for (int j = 0; j < genericLength; j++)
                {
                    myGenericOut[i, j] = item.myGeneric[j];
                }
            }
        }
    }
    public static unsafe void UnpackMeCopyUnaligned<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> flattenMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var flattenMeArray = flattenMe.ToArray();
        int length = flattenMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = flattenMeArray[0].myIntegers.Length;
        var floatLength = flattenMeArray[0].myFloats.Length;
        var genericLength = flattenMeArray[0].myGeneric.Length;

        myIntegersOut = new int[length, integerLength];
        myIntegerOut = new int[length];
        myFloatsOut = new float[length, floatLength];
        myGenericOut = new Titem[length, genericLength];
        myGeneric2Out = new Titem2[length];

        fixed (int* pMyIntegersOut = myIntegersOut)
        fixed (int* pMyIntegerOut = myIntegerOut)
        fixed (float* pMyFloatsOut = myFloatsOut)
        {
            for (int i = 0; i < length; i++)
            {
                var item = flattenMeArray[i];
                pMyIntegerOut[i] = item.myInteger;
                myGeneric2Out[i] = item.myGeneric2;

                int* pIntegersDest = pMyIntegersOut + i * integerLength;
                float* pFloatsDest = pMyFloatsOut + i * floatLength;

                fixed (int* pIntegersSrc = item.myIntegers)
                fixed (float* pFloatsSrc = item.myFloats)
                {
                    int integersByteLength = integerLength * sizeof(int);
                    int floatsByteLength = floatLength * sizeof(float);

                    Unsafe.CopyBlockUnaligned(pIntegersDest, pIntegersSrc, (uint)integersByteLength);
                    Unsafe.CopyBlockUnaligned(pFloatsDest, pFloatsSrc, (uint)floatsByteLength);
                }

                for (int j = 0; j < genericLength; j++)
                {
                    myGenericOut[i, j] = item.myGeneric[j];
                }
            }
        }
    }
    public static unsafe void UnpackMeCopyUnalignedParallelFor<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> flattenMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var flattenMeArray = flattenMe.ToArray();
        int length = flattenMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = flattenMeArray[0].myIntegers.Length;
        var floatLength = flattenMeArray[0].myFloats.Length;
        var genericLength = flattenMeArray[0].myGeneric.Length;

        myIntegersOut = new int[length, integerLength];
        myIntegerOut = new int[length];
        myFloatsOut = new float[length, floatLength];
        myGenericOut = new Titem[length, genericLength];
        myGeneric2Out = new Titem2[length];

        fixed (int* pMyIntegersOut = myIntegersOut)
        fixed (int* pMyIntegerOut = myIntegerOut)
        fixed (float* pMyFloatsOut = myFloatsOut)
        {
            var state = new ParallelState<Titem, Titem2, UselesGeneric>
            {
                FlattenMeArray = flattenMeArray,
                PMyIntegersOut = pMyIntegersOut,
                PMyIntegerOut = pMyIntegerOut,
                PMyFloatsOut = pMyFloatsOut,
                MyGenericOut = myGenericOut,
                MyGeneric2Out = myGeneric2Out,
                IntegerLength = integerLength,
                FloatLength = floatLength,
                GenericLength = genericLength
            };

            Parallel.For(0, length, i => ProcessUnpackMe(state, i));
        }
    }
    private static unsafe void ProcessUnpackMe<Titem, Titem2, UselesGeneric>(ParallelState<Titem, Titem2, UselesGeneric> state, int i)
    {
        var item = state.FlattenMeArray[i];
        state.PMyIntegerOut[i] = item.myInteger;
        state.MyGeneric2Out[i] = item.myGeneric2;

        int* pIntegersDest = state.PMyIntegersOut + i * state.IntegerLength;
        float* pFloatsDest = state.PMyFloatsOut + i * state.FloatLength;

        fixed (int* pIntegersSrc = item.myIntegers)
        fixed (float* pFloatsSrc = item.myFloats)
        {
            int integersByteLength = state.IntegerLength * sizeof(int);
            int floatsByteLength = state.FloatLength * sizeof(float);

            Unsafe.CopyBlockUnaligned(pIntegersDest, pIntegersSrc, (uint)integersByteLength);
            Unsafe.CopyBlockUnaligned(pFloatsDest, pFloatsSrc, (uint)floatsByteLength);
        }

        for (int j = 0; j < state.GenericLength; j++)
        {
            state.MyGenericOut[i, j] = item.myGeneric[j];
        }
    }
    private unsafe struct ParallelState<Titem, Titem2, UselesGeneric>
    {
        public UnpackMe<Titem, Titem2, UselesGeneric>[] FlattenMeArray;
        public int* PMyIntegersOut;
        public int* PMyIntegerOut;
        public float* PMyFloatsOut;
        public Titem[,] MyGenericOut;
        public Titem2[] MyGeneric2Out;
        public int IntegerLength;
        public int FloatLength;
        public int GenericLength;
    }
    public static unsafe void UnpackMeCopyUnalignedTaskScheduler<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> flattenMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var flattenMeArray = flattenMe.ToArray();
        int length = flattenMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = flattenMeArray[0].myIntegers.Length;
        var floatLength = flattenMeArray[0].myFloats.Length;
        var genericLength = flattenMeArray[0].myGeneric.Length;

        myIntegersOut = new int[length, integerLength];
        myIntegerOut = new int[length];
        myFloatsOut = new float[length, floatLength];
        myGenericOut = new Titem[length, genericLength];
        myGeneric2Out = new Titem2[length];

        fixed (int* pMyIntegersOut = myIntegersOut)
        fixed (int* pMyIntegerOut = myIntegerOut)
        fixed (float* pMyFloatsOut = myFloatsOut)
        {
            var state = new ParallelState<Titem, Titem2, UselesGeneric>
            {
                FlattenMeArray = flattenMeArray,
                PMyIntegersOut = pMyIntegersOut,
                PMyIntegerOut = pMyIntegerOut,
                PMyFloatsOut = pMyFloatsOut,
                MyGenericOut = myGenericOut,
                MyGeneric2Out = myGeneric2Out,
                IntegerLength = integerLength,
                FloatLength = floatLength,
                GenericLength = genericLength
            };

            var tasks = new List<Task>();

            for (int i = 0; i < length; i++)
            {
                int index = i; // Avoid closure issues
                tasks.Add(Task.Factory.StartNew(() => ProcessUnpackMe(state, index)));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
    public static void UnpackMeParallelFor<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> transposeMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var transposeMeArray = transposeMe.ToArray();
        int length = transposeMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = transposeMeArray[0].myIntegers.Length;
        var floatLength = transposeMeArray[0].myFloats.Length;
        var genericLength = transposeMeArray[0].myGeneric.Length;

        int[,] integersTemp = new int[length, integerLength];
        int[] integerTemp = new int[length];
        float[,] floatsTemp = new float[length, floatLength];
        Titem[,] genericTemp = new Titem[length, genericLength];
        Titem2[] generic2Temp = new Titem2[length];

        Parallel.For(0, length, i =>
        {
            var item = transposeMeArray[i];
            integerTemp[i] = item.myInteger;
            generic2Temp[i] = item.myGeneric2;

            for (int j = 0; j < integerLength; j++)
            {
                integersTemp[i, j] = item.myIntegers[j];
            }

            for (int j = 0; j < floatLength; j++)
            {
                floatsTemp[i, j] = item.myFloats[j];
            }

            for (int j = 0; j < genericLength; j++)
            {
                genericTemp[i, j] = item.myGeneric[j];
            }
        });

        myIntegersOut = integersTemp;
        myIntegerOut = integerTemp;
        myFloatsOut = floatsTemp;
        myGenericOut = genericTemp;
        myGeneric2Out = generic2Temp;
    }
    public static void UnpackMeParallelForNested<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> flattenMe, out int[,] myIntegersOut, out int[] myIntegerOut, out float[,] myFloatsOut, out Titem[,] myGenericOut, out Titem2[] myGeneric2Out)
    {
        var flattenMeArray = flattenMe.ToArray();
        int length = flattenMeArray.Length;
        if (length == 0)
        {
            myIntegersOut = new int[0, 0];
            myIntegerOut = new int[0];
            myFloatsOut = new float[0, 0];
            myGenericOut = new Titem[0, 0];
            myGeneric2Out = new Titem2[0];
            return;
        }

        var integerLength = flattenMeArray[0].myIntegers.Length;
        var floatLength = flattenMeArray[0].myFloats.Length;
        var genericLength = flattenMeArray[0].myGeneric.Length;

        int[,] integersTemp = new int[length, integerLength];
        int[] integerTemp = new int[length];
        float[,] floatsTemp = new float[length, floatLength];
        Titem[,] genericTemp = new Titem[length, genericLength];
        Titem2[] generic2Temp = new Titem2[length];

        Parallel.For(0, length, i =>
        {
            var item = flattenMeArray[i];
            integerTemp[i] = item.myInteger;
            generic2Temp[i] = item.myGeneric2;

            Parallel.For(0, integerLength, j =>
            {
                integersTemp[i, j] = item.myIntegers[j];
            });

            Parallel.For(0, floatLength, j =>
            {
                floatsTemp[i, j] = item.myFloats[j];
            });

            Parallel.For(0, genericLength, j =>
            {
                genericTemp[i, j] = item.myGeneric[j];
            });
        });

        myIntegersOut = integersTemp;
        myIntegerOut = integerTemp;
        myFloatsOut = floatsTemp;
        myGenericOut = genericTemp;
        myGeneric2Out = generic2Temp;
    }
    public static void PrintToConsole<Titem, Titem2, UselesGeneric>(this IEnumerable<UnpackMe<Titem, Titem2, UselesGeneric>> collection)
    {
        foreach (var item in collection)
        {
            Console.WriteLine("myIntegers: " + string.Join(", ", item.myIntegers));
            Console.WriteLine("myInteger: " + item.myInteger);
            Console.WriteLine("myFloats: " + string.Join(", ", item.myFloats));
            Console.WriteLine("myGeneric: " + string.Join(", ", item.myGeneric));
            Console.WriteLine("myGeneric2: " + item.myGeneric2);
            Console.WriteLine(new string('-', 40)); // Separator for each item
        }
    }
}