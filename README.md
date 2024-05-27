
# Benchmarking Unpacking Methods for `IEnumerable<T>`

This project compares the performance of various methods for unpacking `IEnumerable<T>` objects into 1D and 2D arrays. The goal is to find the fastest and most efficient way to flatten and extract the data.

## Methods Compared

1. `UnpackMeNormalForLoop`: Simple for loop that copies the data to the output arrays.
2. `UnpackMeSimd`: Utilizes SIMD instructions for parallelization.
3. `UnpackMeCopyUnaligned`: Uses `Unsafe.CopyBlockUnaligned` for unaligned memory copy of blittable types.
4. `UnpackMeCopyUnalignedParallelFor`: Combines `Unsafe.CopyBlockUnaligned` with parallel processing using `Parallel.For`.
5. `UnpackMeCopyUnalignedTaskScheduler`: Similar to `UnpackMeCopyUnalignedParallelFor`, but uses a custom task scheduler.
6. `UnpackMeParallelFor`: Utilizes `Parallel.For` for parallelization without `Unsafe.CopyBlockUnaligned`.
7. `UnpackMeParallelForNested`: Uses nested `Parallel.For` loops for parallelization.
8. `UnpackMeGenerated`: Generated method using source generators.
9. `UnpackMeParallelForMoreData`: `UnpackMeParallelFor` with a larger dataset.
10. `UnpackMeGeneratedMoreData`: `UnpackMeGenerated` with a larger dataset.
11. `UnpackMe2Generated`: Another variation of the generated method.

## Benchmark Results

| Method                            | Mean      | Error     | StdDev    | Median    |
|-----------------------------------|----------:|----------:|----------:|----------:|
| UnpackMeNormalForLoop             | 5.939 ms  | 0.0656 ms | 0.0581 ms | 5.944 ms  |
| UnpackMeSimd                      | 4.065 ms  | 0.0789 ms | 0.0811 ms | 4.062 ms  |
| UnpackMeCopyUnaligned             | 4.173 ms  | 0.0736 ms | 0.0688 ms | 4.163 ms  |
| UnpackMeCopyUnalignedParallelFor  | 2.833 ms  | 0.0309 ms | 0.0289 ms | 2.836 ms  |
| UnpackMeCopyUnalignedTaskScheduler| 17.016 ms | 0.2761 ms | 0.2583 ms | 16.920 ms |
| UnpackMeParallelFor               | 2.942 ms  | 0.0523 ms | 0.0463 ms | 2.931 ms  |
| UnpackMeParallelForNested         | 26.351 ms | 0.5210 ms | 0.8560 ms | 26.280 ms |
| UnpackMeGenerated                 | 2.847 ms  | 0.0216 ms | 0.0202 ms | 2.852 ms  |
| UnpackMeParallelForMoreData       | 6.440 ms  | 0.2149 ms | 0.6338 ms | 6.340 ms  |
| UnpackMeGeneratedMoreData         | 7.105 ms  | 0.1412 ms | 0.3960 ms | 7.194 ms  |
| UnpackMe2Generated                | 7.112 ms  | 0.1413 ms | 0.4100 ms | 7.225 ms  |

The fastest method is `UnpackMeCopyUnalignedParallelFor`, which combines parallel processing with unaligned memory copy for blittable types. It achieves a mean execution time of 2.833 ms.

## Fastest Method: `UnpackMeCopyUnalignedParallelFor`

The `UnpackMeCopyUnalignedParallelFor` method achieves the best performance by:

1. Using `Parallel.For` for parallel processing.
2. Utilizing `Unsafe.CopyBlockUnaligned` for fast unaligned memory copy of blittable types (`int` and `float`).
3. Employing unsafe code and pointers for direct memory access.

The method converts the input `IEnumerable<T>` to an array, creates output arrays, and uses `Parallel.For` to process each element. It copies scalar members directly and uses `Unsafe.CopyBlockUnaligned` for array members of blittable types. Non-blittable types are copied element-wise.

## Conclusion

The `UnpackMeCopyUnalignedParallelFor` method emerges as the fastest approach for unpacking `IEnumerable<T>` objects into arrays. However, consider factors such as data size, memory usage, and maintainability when choosing an unpacking method for your specific application.
