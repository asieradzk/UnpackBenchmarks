using IEnumerableUnpacker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnpackBench
{
    public class SomeStuff
    {
        public string myString;
        public int myInt;

        public SomeStuff()
        {
            this.myString = "420";
            this.myInt = 69;
        }
    }

    [Unpackable]
    public class UnpackMe2
    {
        [Unpack("MyItegersOut")]
        public int[] myIntegers;
        
        [Unpack("MyIntegerOut")]
        public int myInteger;
        [Unpack("MyFloatsOut")]
        public float[] myFloats;
        [Unpack("MyObjsOut")]
        public SomeStuff[] myObj;
        [Unpack("MyObjOut")]
        public SomeStuff myObjs;
        public SomeStuff myObjDontUnpack;
        

        public UnpackMe2(int[] integers, int integer, float[] floats)
        {
            myIntegers = integers;
            myInteger = integer;
            myFloats = floats;
            myObj = new SomeStuff[5];
            for (int i = 0; i < 5; i++)
            {
                myObj[i] = new SomeStuff();
            }
            myObjs = new SomeStuff();
            myObjDontUnpack = new SomeStuff();
        }

        public static List<UnpackMe2> CreateUnpackMe2(int lengthIntegers, int lengthFloats, int lengthGenerics, int numberOfItems)
        {
            Random random = new Random();
            var result = new List<UnpackMe2>();

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

                var transposeMe = new UnpackMe2(
                    myIntegers,
                    random.Next(), // Random integer value
                    myFloats
                );

                result.Add(transposeMe);
            }

            return result;
        }
    }
}
