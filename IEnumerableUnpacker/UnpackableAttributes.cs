using System;
namespace IEnumerableUnpacker;
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UnpackableAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class UnpackAttribute : Attribute
{
    public string OutputName { get; }

    public UnpackAttribute(string outputName)
    {
        OutputName = outputName;
    }
}
