using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IEnumerableUnpacker;
using UnpackBench;

public class UnpackBenchmarks
{
    private readonly List<UnpackMe<string, string, string>> _data;
    private readonly List<UnpackMe<string, string, string>> _data2;
    private readonly List<UnpackMe2> _data3;
    

    public UnpackBenchmarks()
    {
        _data = UnpackMe<string, string, string>.CreateUnpackMe(40, 10, 5, 20000);
        _data2 = UnpackMe<string, string, string>.CreateUnpackMe(400, 100, 5, 20000);
        _data3 = UnpackMe2.CreateUnpackMe2(400, 100, 5, 20000);
    }

    [Benchmark]
    public void UnpackMeNormalForLoop()
    {
        _data.UnpackMeNormalLoop(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeSimd()
    {
        _data.UnpackMeSIMD(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeCopyUnaligned()
    {
        _data.UnpackMeCopyUnaligned(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeCopyUnalignedParallelFor()
    {
        _data.UnpackMeCopyUnalignedParallelFor(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeCopyUnalignedTaskScheduler()
    {
        _data.UnpackMeCopyUnalignedTaskScheduler(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeParallelFor()
    {
        _data.UnpackMeParallelFor(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeParallelForNested()
    {
        _data.UnpackMeParallelForNested(out _, out _, out _, out _, out _);
    }
    
    [Benchmark]
    public void UnpackMeGenerated()
    {
        _data.UnpackUnpackMe(out _, out _, out _, out _, out _);
    }

    [Benchmark]
    public void UnpackMeParallelForMoreData()
    {
        _data2.UnpackMeParallelFor(out _, out _, out _, out _, out _);
    }
    [Benchmark]
    public void UnpackMeGeneratedMoreData()
    {
        _data2.UnpackUnpackMe(out _, out _, out _, out _, out _);
    }
    [Benchmark]
    public void UnpackMe2Generated()
    {
        _data3.UnpackUnpackMe2(out _, out _, out _, out _, out _);
    }
}