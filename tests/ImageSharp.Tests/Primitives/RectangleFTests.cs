// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Globalization;

namespace SixLabors.ImageSharp.Tests;

/// <summary>
/// Tests the <see cref="RectangleF"/> struct.
/// </summary>
public class RectangleFTests
{
    [Fact]
    public void DefaultConstructorTest()
    {
        Assert.Equal(default, RectangleF.Empty);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void NonDefaultConstructorTest(float x, float y, float width, float height)
    {
        RectangleF rect1 = new(x, y, width, height);
        PointF p = new(x, y);
        SizeF s = new(width, height);
        RectangleF rect2 = new(p, s);

        Assert.Equal(rect1, rect2);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void FromLTRBTest(float left, float top, float right, float bottom)
    {
        RectangleF expected = new(left, top, right - left, bottom - top);
        RectangleF actual = RectangleF.FromLTRB(left, top, right, bottom);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void DimensionsTest(float x, float y, float width, float height)
    {
        RectangleF rect = new(x, y, width, height);
        PointF p = new(x, y);
        SizeF s = new(width, height);

        Assert.Equal(p, rect.Location);
        Assert.Equal(s, rect.Size);
        Assert.Equal(x, rect.X);
        Assert.Equal(y, rect.Y);
        Assert.Equal(width, rect.Width);
        Assert.Equal(height, rect.Height);
        Assert.Equal(x, rect.Left);
        Assert.Equal(y, rect.Top);
        Assert.Equal(x + width, rect.Right);
        Assert.Equal(y + height, rect.Bottom);
    }

    [Fact]
    public void IsEmptyTest()
    {
        Assert.True(RectangleF.Empty.IsEmpty);
        Assert.True(default(RectangleF).IsEmpty);
        Assert.True(new RectangleF(1, -2, -10, 10).IsEmpty);
        Assert.True(new RectangleF(1, -2, 10, -10).IsEmpty);
        Assert.True(new RectangleF(1, -2, 0, 0).IsEmpty);

        Assert.False(new RectangleF(0, 0, 10, 10).IsEmpty);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(float.MaxValue, float.MinValue)]
    public void LocationSetTest(float x, float y)
    {
        PointF point = new(x, y);
        RectangleF rect = new(10, 10, 10, 10) { Location = point };
        Assert.Equal(point, rect.Location);
        Assert.Equal(point.X, rect.X);
        Assert.Equal(point.Y, rect.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(float.MaxValue, float.MinValue)]
    public void SizeSetTest(float x, float y)
    {
        SizeF size = new(x, y);
        RectangleF rect = new(10, 10, 10, 10) { Size = size };
        Assert.Equal(size, rect.Size);
        Assert.Equal(size.Width, rect.Width);
        Assert.Equal(size.Height, rect.Height);
    }

    [Theory]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void EqualityTest(float x, float y, float width, float height)
    {
        RectangleF rect1 = new(x, y, width, height);
        RectangleF rect2 = new(width, height, x, y);

        Assert.True(rect1 != rect2);
        Assert.False(rect1 == rect2);
        Assert.False(rect1.Equals(rect2));
        Assert.False(rect1.Equals((object)rect2));
    }

    [Fact]
    public void EqualityTestNotRectangleF()
    {
        RectangleF rectangle = new(0, 0, 0, 0);
        Assert.False(rectangle.Equals(null));
        Assert.False(rectangle.Equals(0));

        // If RectangleF implements IEquatable<RectangleF> (e.g. in .NET Core), then classes that are implicitly
        // convertible to RectangleF can potentially be equal.
        // See https://github.com/dotnet/corefx/issues/5255.
        bool expectsImplicitCastToRectangleF = typeof(IEquatable<RectangleF>).IsAssignableFrom(rectangle.GetType());
        Assert.Equal(expectsImplicitCastToRectangleF, rectangle.Equals(new Rectangle(0, 0, 0, 0)));

        Assert.False(rectangle.Equals((object)new Rectangle(0, 0, 0, 0))); // No implicit cast
    }

    [Fact]
    public void GetHashCodeTest()
    {
        RectangleF rect1 = new(10, 10, 10, 10);
        RectangleF rect2 = new(10, 10, 10, 10);
        Assert.Equal(rect1.GetHashCode(), rect2.GetHashCode());
        Assert.NotEqual(rect1.GetHashCode(), new RectangleF(20, 10, 10, 10).GetHashCode());
        Assert.NotEqual(rect1.GetHashCode(), new RectangleF(10, 20, 10, 10).GetHashCode());
        Assert.NotEqual(rect1.GetHashCode(), new RectangleF(10, 10, 20, 10).GetHashCode());
        Assert.NotEqual(rect1.GetHashCode(), new RectangleF(10, 10, 10, 20).GetHashCode());
    }

    [Theory]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void ContainsTest(float x, float y, float width, float height)
    {
        RectangleF rect = new(x, y, width, height);
        float x1 = (x + width) / 2;
        float y1 = (y + height) / 2;
        PointF p = new(x1, y1);
        RectangleF r = new(x1, y1, width / 2, height / 2);

        Assert.False(rect.Contains(x1, y1));
        Assert.False(rect.Contains(p));
        Assert.False(rect.Contains(r));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue / 2, float.MinValue / 2, float.MinValue / 2, float.MaxValue / 2)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void InflateTest(float x, float y, float width, float height)
    {
        RectangleF rect = new(x, y, width, height);
        RectangleF inflatedRect = new(x - width, y - height, width + (2 * width), height + (2 * height));

        rect.Inflate(width, height);
        Assert.Equal(inflatedRect, rect);

        SizeF s = new(x, y);
        inflatedRect = RectangleF.Inflate(rect, x, y);

        rect.Inflate(s);
        Assert.Equal(inflatedRect, rect);
    }

    [Theory]
    [InlineData(float.MaxValue, float.MinValue, float.MaxValue / 2, float.MinValue / 2)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void IntersectTest(float x, float y, float width, float height)
    {
        RectangleF rect1 = new(x, y, width, height);
        RectangleF rect2 = new(y, x, width, height);
        RectangleF expectedRect = RectangleF.Intersect(rect1, rect2);
        rect1.Intersect(rect2);
        Assert.Equal(expectedRect, rect1);
        Assert.False(rect1.IntersectsWith(expectedRect));
    }

    [Fact]
    public void IntersectIntersectingRectsTest()
    {
        RectangleF rect1 = new(0, 0, 5, 5);
        RectangleF rect2 = new(1, 1, 3, 3);
        RectangleF expected = new(1, 1, 3, 3);

        Assert.Equal(expected, RectangleF.Intersect(rect1, rect2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void UnionTest(float x, float y, float width, float height)
    {
        RectangleF a = new(x, y, width, height);
        RectangleF b = new(width, height, x, y);

        float x1 = Math.Min(a.X, b.X);
        float x2 = Math.Max(a.X + a.Width, b.X + b.Width);
        float y1 = Math.Min(a.Y, b.Y);
        float y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

        RectangleF expectedRectangle = new(x1, y1, x2 - x1, y2 - y1);

        Assert.Equal(expectedRectangle, RectangleF.Union(a, b));
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, 0, 0, float.MaxValue)]
    [InlineData(0, float.MinValue, float.MaxValue, 0)]
    public void OffsetTest(float x, float y, float width, float height)
    {
        RectangleF r1 = new(x, y, width, height);
        RectangleF expectedRect = new(x + width, y + height, width, height);
        PointF p = new(width, height);

        r1.Offset(p);
        Assert.Equal(expectedRect, r1);

        expectedRect.Offset(p);
        r1.Offset(width, height);
        Assert.Equal(expectedRect, r1);
    }

    [Fact]
    public void ToStringTest()
    {
        RectangleF r = new(5, 5.1F, 1.3F, 1);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, "RectangleF [ X={0}, Y={1}, Width={2}, Height={3} ]", r.X, r.Y, r.Width, r.Height), r.ToString());
    }

    [Theory]
    [InlineData(float.MinValue, float.MaxValue, float.MaxValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MaxValue, float.MaxValue, float.MinValue)]
    [InlineData(float.MinValue, float.MaxValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MaxValue, float.MinValue, float.MinValue)]
    [InlineData(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MinValue, float.MaxValue, float.MinValue)]
    [InlineData(float.MinValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MinValue, float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue)]
    [InlineData(float.MaxValue, float.MaxValue, float.MaxValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MinValue, float.MaxValue, float.MaxValue)]
    [InlineData(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MaxValue)]
    [InlineData(float.MaxValue, float.MinValue, float.MinValue, float.MinValue)]
    [InlineData(0, 0, 0, 0)]
    public void DeconstructTest(float x, float y, float width, float height)
    {
        RectangleF r = new(x, y, width, height);

        (float dx, float dy, float dw, float dh) = r;

        Assert.Equal(x, dx);
        Assert.Equal(y, dy);
        Assert.Equal(width, dw);
        Assert.Equal(height, dh);
    }
}
