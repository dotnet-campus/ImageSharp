// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using SixLabors.ImageSharp.ColorProfiles;

namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255.
/// The color components are stored in red, green, blue, and alpha order (least significant to most significant byte).
/// <para>
/// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
/// </para>
/// </summary>
/// <remarks>
/// This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
/// as it avoids the need to create new values for modification operations.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public partial struct Rgba32 : IPixel<Rgba32>, IPackedVector<uint>
{
    /// <summary>
    /// Gets or sets the red component.
    /// </summary>
    public byte R;

    /// <summary>
    /// Gets or sets the green component.
    /// </summary>
    public byte G;

    /// <summary>
    /// Gets or sets the blue component.
    /// </summary>
    public byte B;

    /// <summary>
    /// Gets or sets the alpha component.
    /// </summary>
    public byte A;

    private static readonly Vector4 MaxBytes = Vector128.Create(255f).AsVector4();
    private static readonly Vector4 Half = Vector128.Create(.5f).AsVector4();

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = byte.MaxValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(byte r, byte g, byte b, byte a)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(float r, float g, float b, float a = 1)
        : this(new Vector4(r, g, b, a))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="vector">
    /// The vector containing the components for the packed vector.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(Vector3 vector)
        : this(new Vector4(vector, 1f))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="vector">
    /// The vector containing the components for the packed vector.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(Vector4 vector)
        : this() => this = Pack(vector);

    /// <summary>
    /// Initializes a new instance of the <see cref="Rgba32"/> struct.
    /// </summary>
    /// <param name="packed">
    /// The packed value.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(uint packed)
        : this() => this.Rgba = packed;

    /// <summary>
    /// Gets or sets the packed representation of the Rgba32 struct.
    /// </summary>
    public uint Rgba
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Unsafe.As<Rgba32, uint>(ref Unsafe.AsRef(in this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Unsafe.As<Rgba32, uint>(ref this) = value;
    }

    /// <summary>
    /// Gets or sets the RGB components of this struct as <see cref="Rgb24"/>
    /// </summary>
    public Rgb24 Rgb
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => new(this.R, this.G, this.B);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            this.R = value.R;
            this.G = value.G;
            this.B = value.B;
        }
    }

    /// <summary>
    /// Gets or sets the RGB components of this struct as <see cref="Bgr24"/> reverting the component order.
    /// </summary>
    public Bgr24 Bgr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => new(this.R, this.G, this.B);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            this.R = value.R;
            this.G = value.G;
            this.B = value.B;
        }
    }

    /// <inheritdoc/>
    public uint PackedValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => this.Rgba;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.Rgba = value;
    }

    /// <summary>
    /// Allows the implicit conversion of an instance of <see cref="Rgb"/> to a
    /// <see cref="Rgba32"/>.
    /// </summary>
    /// <param name="color">The instance of <see cref="Rgb"/> to convert.</param>
    /// <returns>An instance of <see cref="Rgba32"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Rgba32(Rgb color) => FromScaledVector4(new Vector4(color.ToScaledVector3(), 1F));

    /// <summary>
    /// Compares two <see cref="Rgba32"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba32"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba32"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Rgba32 left, Rgba32 right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Rgba32"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba32"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba32"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Rgba32 left, Rgba32 right) => !left.Equals(right);

    /// <summary>
    /// Creates a new instance of the <see cref="Rgba32"/> struct
    /// from the given hexadecimal string.
    /// </summary>
    /// <param name="hex">
    /// The hexadecimal representation of the combined color components arranged
    /// in rgb, rgba, rrggbb, or rrggbbaa format to match web syntax.
    /// </param>
    /// <returns>
    /// The <see cref="Rgba32"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Hexadecimal string is not in the correct format.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 ParseHex(string hex)
    {
        Guard.NotNull(hex, nameof(hex));

        if (!TryParseHex(hex, out Rgba32 rgba))
        {
            throw new ArgumentException("Hexadecimal string is not in the correct format.", nameof(hex));
        }

        return rgba;
    }

    /// <summary>
    /// Attempts to creates a new instance of the <see cref="Rgba32"/> struct
    /// from the given hexadecimal string.
    /// </summary>
    /// <param name="hex">
    /// The hexadecimal representation of the combined color components arranged
    /// in rgb, rgba, rrggbb, or rrggbbaa format to match web syntax.
    /// </param>
    /// <param name="result">When this method returns, contains the <see cref="Rgba32"/> equivalent of the hexadecimal input.</param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseHex(string? hex, out Rgba32 result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(hex))
        {
            return false;
        }

        hex = ToRgbaHex(hex);

        if (hex is null || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint packedValue))
        {
            return false;
        }

        packedValue = BinaryPrimitives.ReverseEndianness(packedValue);
        result = Unsafe.As<uint, Rgba32>(ref packedValue);
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Rgba32 ToRgba32() => this;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToScaledVector4() => this.ToVector4();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToVector4() => new Vector4(this.R, this.G, this.B, this.A) / MaxBytes;

    /// <inheritdoc />
    public static PixelTypeInfo GetPixelTypeInfo()
        => PixelTypeInfo.Create<Rgba32>(
            PixelComponentInfo.Create<Rgba32>(4, 8, 8, 8, 8),
            PixelColorType.RGB | PixelColorType.Alpha,
            PixelAlphaRepresentation.Unassociated);

    /// <inheritdoc />
    public static PixelOperations<Rgba32> CreatePixelOperations() => new PixelOperations();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromScaledVector4(Vector4 source) => FromVector4(source);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromVector4(Vector4 source) => Pack(source);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromAbgr32(Abgr32 source) => new(source.R, source.G, source.B, source.A);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromArgb32(Argb32 source) => new(source.R, source.G, source.B, source.A);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromBgr24(Bgr24 source) => new(source.R, source.G, source.B);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromBgra32(Bgra32 source) => new(source.R, source.G, source.B, source.A);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromL8(L8 source) => new(source.PackedValue, source.PackedValue, source.PackedValue);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromL16(L16 source)
    {
        byte rgb = ColorNumerics.From16BitTo8Bit(source.PackedValue);
        return new Rgba32(rgb, rgb, rgb);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromLa16(La16 source) => new(source.L, source.L, source.L, source.A);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromLa32(La32 source)
    {
        byte rgb = ColorNumerics.From16BitTo8Bit(source.L);
        return new Rgba32(rgb, rgb, rgb, ColorNumerics.From16BitTo8Bit(source.A));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromRgb24(Rgb24 source) => new(source.R, source.G, source.B);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromRgba32(Rgba32 source) => new() { PackedValue = source.PackedValue };

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromRgb48(Rgb48 source)
        => new()
        {
            R = ColorNumerics.From16BitTo8Bit(source.R),
            G = ColorNumerics.From16BitTo8Bit(source.G),
            B = ColorNumerics.From16BitTo8Bit(source.B),
            A = byte.MaxValue
        };

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba32 FromRgba64(Rgba64 source)
        => new()
        {
            R = ColorNumerics.From16BitTo8Bit(source.R),
            G = ColorNumerics.From16BitTo8Bit(source.G),
            B = ColorNumerics.From16BitTo8Bit(source.B),
            A = ColorNumerics.From16BitTo8Bit(source.A)
        };

    /// <summary>
    /// Converts the value of this instance to a hexadecimal string.
    /// </summary>
    /// <returns>A hexadecimal string representation of the value.</returns>
    public readonly string ToHex()
    {
        uint hexOrder = (uint)((this.A << 0) | (this.B << 8) | (this.G << 16) | (this.R << 24));
        return hexOrder.ToString("X8", CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is Rgba32 rgba32 && this.Equals(rgba32);

    /// <inheritdoc/>
    public readonly bool Equals(Rgba32 other) => this.Rgba.Equals(other.Rgba);

    /// <inheritdoc/>
    public override readonly string ToString() => $"Rgba32({this.R}, {this.G}, {this.B}, {this.A})";

    /// <inheritdoc/>
    public override readonly int GetHashCode() => this.Rgba.GetHashCode();

    /// <summary>
    /// Packs a <see cref="Vector4"/> into a color.
    /// </summary>
    /// <param name="vector">The vector containing the values to pack.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Rgba32 Pack(Vector4 vector)
    {
        vector *= MaxBytes;
        vector += Half;
        vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);

        Vector128<byte> result = Vector128.ConvertToInt32(vector.AsVector128()).AsByte();
        return new Rgba32(result.GetElement(0), result.GetElement(4), result.GetElement(8), result.GetElement(12));
    }

    /// <summary>
    /// Converts the specified hex value to an rrggbbaa hex value.
    /// </summary>
    /// <param name="hex">The hex value to convert.</param>
    /// <returns>
    /// A rrggbbaa hex value.
    /// </returns>
    private static string? ToRgbaHex(string hex)
    {
        if (hex[0] == '#')
        {
            hex = hex[1..];
        }

        if (hex.Length == 8)
        {
            return hex;
        }

        if (hex.Length == 6)
        {
            return hex + "FF";
        }

        if (hex.Length is < 3 or > 4)
        {
            return null;
        }

        char a = hex.Length == 3 ? 'F' : hex[3];
        char b = hex[2];
        char g = hex[1];
        char r = hex[0];

        return new string(new[] { r, r, g, g, b, b, a, a });
    }
}
