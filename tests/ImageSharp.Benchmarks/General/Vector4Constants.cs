// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace SixLabors.ImageSharp.Benchmarks.General;

/// <summary>
/// Has it any effect on performance to store SIMD constants as static readonly fields? Is it OK to always inline them?
/// Spoiler: the difference seems to be statistically insignificant!
/// </summary>
public class Vector4Constants
{
    private static readonly Vector4 A = new(1.2f);
    private static readonly Vector4 B = new(3.4f);
    private static readonly Vector4 C = new(5.6f);
    private static readonly Vector4 D = new(7.8f);

    private Random random;

    private Vector4 parameter;

    [GlobalSetup]
    public void Setup()
    {
        this.random = new Random(42);
        this.parameter = new Vector4(
            this.GetRandomFloat(),
            this.GetRandomFloat(),
            this.GetRandomFloat(),
            this.GetRandomFloat());
    }

    [Benchmark(Baseline = true)]
    public Vector4 Static()
    {
        Vector4 p = this.parameter;

        Vector4 x = (p * A / B) + (p * C / D);
        Vector4 y = (p / A * B) + (p / C * D);
        Vector4 z = Vector4.Min(p, A);
        Vector4 w = Vector4.Max(p, B);
        return x + y + z + w;
    }

    [Benchmark]
    public Vector4 Inlined()
    {
        Vector4 p = this.parameter;

        Vector4 x = (p * new Vector4(1.2f) / new Vector4(2.3f)) + (p * new Vector4(4.5f) / new Vector4(6.7f));
        Vector4 y = (p / new Vector4(1.2f) * new Vector4(2.3f)) + (p / new Vector4(4.5f) * new Vector4(6.7f));
        Vector4 z = Vector4.Min(p, new Vector4(1.2f));
        Vector4 w = Vector4.Max(p, new Vector4(2.3f));
        return x + y + z + w;
    }

    private float GetRandomFloat() => (float)this.random.NextDouble();
}
