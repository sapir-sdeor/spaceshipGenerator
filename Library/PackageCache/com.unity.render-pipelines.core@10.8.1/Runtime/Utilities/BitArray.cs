using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// IBitArray interface.
    /// </summary>
    public interface IBitArray
    {
        /// <summary>Gets the capacity of this BitArray. This is the number of bits that are usable.</summary>
        uint capacity { get; }
        /// <summary>Return `true` if all the bits of this BitArray are set to 0. Returns `false` otherwise.</summary>
        bool allFalse { get; }
        /// <summary>Return `true` if all the bits of this BitArray are set to 1. Returns `false` otherwise.</summary>
        bool allTrue { get; }
        /// <summary>
        /// An indexer that allows access to the bit at a given index. This provides both read and write access.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        bool this[uint index] { get; set; }
        /// <summary>Writes the bits in the array in a human-readable form. This is as a string of 0s and 1s packed by 8 bits. This is useful for debugging.</summary>
        string humanizedData { get; }

        /// <summary>
        /// Perform an AND bitwise operation between this BitArray and the one you pass into the function and return the result. Both BitArrays must have the same capacity. This will not change current BitArray values.
        /// </summary>
        /// <param name="other">BitArray with which to the And operation.</param>
        /// <returns>The resulting bit array.</returns>
        IBitArray BitAnd(IBitArray other);
        /// <summary>
        /// Perform an OR bitwise operation between this BitArray and the one you pass into the function and return the result. Both BitArrays must have the same capacity. This will not change current BitArray values.
        /// </summary>
        /// <param name="other">BitArray with which to the Or operation.</param>
        /// <returns>The resulting bit array.</returns>
        IBitArray BitOr(IBitArray other);
        /// <summary>
        /// Return the BitArray with every bit inverted.
        /// </summary>
        /// <returns></returns>
        IBitArray BitNot();
    }

    // /!\ Important for serialization:
    // Serialization helper will rely on the name of the struct type.
    // In order to work, it must be BitArrayN where N is the capacity without suffix.

    /// <summary>
    /// Bit array of size 8.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray8 : IBitArray
    {
        [SerializeField]
        byte data;

        /// <summary>Number of elements in the bit array.</summary>
        public uint capacity => 8u;
        /// <summary>True if all bits are 0.</summary>
        public bool allFalse => data == 0u;
        /// <summary>True if all bits are 1.</summary>
        public bool allTrue => data == byte.MaxValue;
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string humanizedData => String.Format("{0, " + capacity + "}", Convert.ToString(data, 2)).Replace(' ', '0');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get8(index, data);
            set => BitArrayUtilities.Set8(index, ref data, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray8(byte initValue) => data = initValue;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray8(IEnumerable<uint> bitIndexTrue)
        {
            data = (byte)0u;
            if (bitIndexTrue == null)
                return;
            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= capacity) continue;
                data |= (byte)(1u << (int)bitIndex);
            }
        }

        /// <summary>
        /// Bit-wise Not operator
        /// </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator ~(BitArray8 a) => new BitArray8((byte)~a.data);
        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator |(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data | b.data));
        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator &(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data & b.data));

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitAnd(IBitArray other) => this & (BitArray8)other;
        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitOr(IBitArray other) => this | (BitArray8)other;
        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray8 a, BitArray8 b) => a.data == b.data;
        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray8 a, BitArray8 b) => a.data != b.data;
        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray8 && ((BitArray8)obj).data == data;
        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 16.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray16 : IBitArray
    {
        [SerializeField]
        ushort data;

        /// <summary>Number of elements in the bit array.</summary>
        public uint capacity => 16u;
        /// <summary>True if all bits are 0.</summary>
        public bool allFalse => data == 0u;
        /// <summary>True if all bits are 1.</summary>
        public bool allTrue => data == ushort.MaxValue;
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string humanizedData => System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + capacity + "}", Convert.ToString(data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get16(index, data);
            set => BitArrayUtilities.Set16(index, ref data, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray16(ushort initValue) => data = initValue;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>

        public BitArray16(IEnumerable<uint> bitIndexTrue)
        {
            data = (ushort)0u;
            if (bitIndexTrue == null)
                return;
            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= capacity) continue;
                data |= (ushort)(1u << (int)bitIndex);
            }
        }

        /// <summary>
        /// Bit-wise Not operator
        /// </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator ~(BitArray16 a) => new BitArray16((ushort)~a.data);
        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator |(BitArray16 a, BitArray16 b) => new BitArray16((ushort)(a.data | b.data));
        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator &(BitArray16 a, BitArray16 b) => new BitArray16((ushort)(a.data & b.data));

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitAnd(IBitArray other) => this & (BitArray16)other;
        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitOr(IBitArray other) => this | (BitArray16)other;
        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray16 a, BitArray16 b) => a.data == b.data;
        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray16 a, BitArray16 b) => a.data != b.data;
        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray16 && ((BitArray16)obj).data == data;
        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 32.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray32 : IBitArray
    {
        [SerializeField]
        uint data;

        /// <summary>Number of elements in the bit array.</summary>
        public uint capacity => 32u;
        /// <summary>True if all bits are 0.</summary>
        public bool allFalse => data == 0u;
        /// <summary>True if all bits are 1.</summary>
        public bool allTrue => data == uint.MaxValue;
        string humanizedVersion => Convert.ToString(data, 2);
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string humanizedData => System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + capacity + "}", Convert.ToString(data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get32(index, data);
            set => BitArrayUtilities.Set32(index, ref data, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray32(uint initValue) => data = initValue;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>

        public BitArray32(IEnumerable<uint> bitIndexTrue)
        {
            data = 0u;
            if (bitIndexTrue == null)
                return;
            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= capacity) continue;
                data |= 1u << (int)bitIndex;
            }
        }

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitAnd(IBitArray other) => this & (BitArray32)other;
        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitOr(IBitArray other) => this | (BitArray32)other;
        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitNot() => ~this;

        /// <summary>
        /// Bit-wise Not operator
        /// </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator ~(BitArray32 a) => new BitArray32(~a.data);
        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator |(BitArray32 a, BitArray32 b) => new BitArray32(a.data | b.data);
        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator &(BitArray32 a, BitArray32 b) => new BitArray32(a.data & b.data);

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray32 a, BitArray32 b) => a.data == b.data;
        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray32 a, BitArray32 b) => a.data != b.data;
        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray32 && ((BitArray32)obj).data == data;
        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 64.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray64 : IBitArray
    {
        [SerializeField]
        ulong data;

        /// <summary>Number of elements in the bit array.</summary>
        public uint capacity => 64u;
        /// <summary>True if all bits are 0.</summary>
        public bool allFalse => data == 0uL;
        /// <summary>True if all bits are 1.</summary>
        public bool allTrue => data == ulong.MaxValue;
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string humanizedData => System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + capacity + "}", Convert.ToString((long)data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get64(index, data);
            set => BitArrayUtilities.Set64(index, ref data, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray64(ulong initValue) => data = initValue;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>

        public BitArray64(IEnumerable<uint> bitIndexTrue)
        {
            data = 0L;
            if (bitIndexTrue == null)
                return;
            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= capacity) continue;
                data |= 1uL << (int)bitIndex;
            }
        }

        /// <summary>
        /// Bit-wise Not operator
        /// </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator ~(BitArray64 a) => new BitArray64(~a.data);
        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator |(BitArray64 a, BitArray64 b) => new BitArray64(a.data | b.data);
        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator &(BitArray64 a, BitArray64 b) => new BitArray64(a.data & b.data);

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitAnd(IBitArray other) => this & (BitArray64)other;
        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitOr(IBitArray other) => this | (BitArray64)other;
        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray64 a, BitArray64 b) => a.data == b.data;
        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray64 a, BitArray64 b) => a.data != b.data;
        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray64 && ((BitArray64)obj).data == data;
        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 128.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray128 : IBitArray
    {
        [SerializeField]
        ulong data1;
        [SerializeField]
        ulong data2;

        /// <summary>Number of elements in the bit array.</summary>
        public uint capacity => 128u;
        /// <summary>True if all bits are 0.</summary>
        public bool allFalse => data1 == 0uL && data2 == 0uL;
        /// <summary>True if all bits are 1.</summary>
        public bool allTrue => data1 == ulong.MaxValue && data2 == ulong.MaxValue;
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string humanizedData =>
            System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data2, 2)).Replace(' ', '0'), ".{8}", "$0.")
            + System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uin