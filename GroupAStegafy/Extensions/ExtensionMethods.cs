using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Model.FileDescriptions;

namespace GroupAStegafy.Extensions
{
    /// <summary>
    ///     Contains useful extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Data members

        private const int ByteSize = 8;

        #endregion

        #region Methods

        /// <summary>
        ///     Converts to this array of bytes into a WriteableBitmap with the specified width and height.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="imageData">The byte array to be converted into a bitmap.</param>
        /// <param name="width">The width of the final image.</param>
        /// <param name="height">The height of the final image.</param>
        /// <returns>A WriteableBitmap with the specified width and height.</returns>
        public static async Task<WriteableBitmap> ToWriteableBitmapAsync(this byte[] imageData, int width, int height)
        {
            var bitmap = new WriteableBitmap(width, height);
            using (var writeStream = bitmap.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(imageData, 0, imageData.Length);
            }

            return bitmap;
        }

        /// <summary>
        ///     Determines whether this StorageFile has an extension that matches one of the target extensions.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="file">This file to be checked.</param>
        /// <param name="targetExtensions">The target extensions to be used to check.</param>
        /// <returns>
        ///     <c>true</c> if this StorageFile has an extension that matches one of the target extensions; otherwise, <c>false</c>
        /// </returns>
        public static bool HasTargetExtension(this StorageFile file, IEnumerable<FileExtension> targetExtensions)
        {
            var hasValidExtension = false;
            foreach (var targetExtension in targetExtensions ?? Enumerable.Empty<FileExtension>())
            {
                if (targetExtension.ExtensionEquals(file.FileType))
                {
                    hasValidExtension = true;
                }
            }

            return hasValidExtension;
        }

        /// <summary>
        ///     Gets the specific bit in this byte.
        ///     Precondition: bit is less than the number of bits in a byte.
        ///     Postcondition: none
        /// </summary>
        /// <param name="b">The byte to pull the bit out of.</param>
        /// <param name="bit">The index of the bit to be returned.</param>
        /// <returns>
        ///     true if the specified bit is 1, false otherwise
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">If the specified bit is greater than or equal to the size of a byte.</exception>
        public static bool GetBit(this byte b, int bit)
        {
            if (bit >= ByteSize)
            {
                throw new IndexOutOfRangeException();
            }

            return (b & (1 << bit)) != 0;
        }

        /// <summary>
        ///     Sets the specified bit to the specified value. True = 1, False = 0.
        ///     Precondition: bit is less than the number of bits in a byte.
        ///     Postcondition: the bit at the specified index of the byte is replaced with the passed in value.
        /// </summary>
        /// <param name="sourceByte">The source byte.</param>
        /// <param name="bit">The index to be replaced.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>The newly changed byte.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static byte SetBit(this byte sourceByte, int bit, bool value)
        {
            var booleanArray = sourceByte.ToBooleanArray();
            if (bit >= ByteSize)
            {
                throw new IndexOutOfRangeException();
            }

            booleanArray[bit] = value;

            return booleanArray.ToByte();
        }

        /// <summary>
        ///     Creates an array from this byte in which the 0 index in
        ///     the array corresponds with the least significant bit of the byte.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="sourceByte">The source byte.</param>
        /// <returns>The array representation of this byte.</returns>
        public static bool[] ToBooleanArray(this byte sourceByte)
        {
            var boolArray = new bool[ByteSize];
            for (var i = 0; i < ByteSize; i++)
            {
                boolArray[i] = sourceByte.GetBit(i);
            }

            return boolArray;
        }

        /// <summary>
        ///     Converts this boolean array to a byte.
        ///     Precondition: boolArray.Length less than or equal to the number of bits in a byte.
        ///     Postcondition: none
        /// </summary>
        /// <param name="boolArray">The bool array.</param>
        /// <returns>The byte representation of this boolean array.</returns>
        /// <exception cref="ArgumentException">If the length of this array is greater than or equal to the size of a byte.</exception>
        public static byte ToByte(this bool[] boolArray)
        {
            if (boolArray.Length > ByteSize)
            {
                throw new ArgumentException();
            }

            var result = (byte) 0;
            var index = boolArray.Length - 1;

            foreach (var condition in boolArray)
            {
                if (condition)
                {
                    result |= (byte) (1 << (7 - index));
                }

                index--;
            }

            return result;
        }

        /// <summary>
        ///     Splits the elements the according to the SplitElementsBy array extension method;
        ///     however, this method does it in reverse order.
        ///     Ex: array = [1, 2, 3, 4, 5], then this method will return
        ///     array.ReverseSplitElementsBy(2) -&gt; [[5, 4], [3, 2], [1]]
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <typeparam name="T">The data type of the contents of the array.</typeparam>
        /// <param name="array">The array to be reversed.</param>
        /// <param name="splitFactor">The split factor.</param>
        /// <returns>
        ///     The newly split array with splits determined by the splitFactor in reverse.
        /// </returns>
        public static T[][] ReverseSplitElementsBy<T>(this T[] array, int splitFactor)
        {
            var reversedArray = array.Reverse().ToArray();

            return reversedArray.SplitElementsBy(splitFactor);
        }

        /// <summary>
        ///     Splits the elements by the specified splitFactor. If the split factor does not
        ///     evenly divide into the length of the array, then the last element in the
        ///     returned array will be of length array.Length % splitFactor.
        ///     Ex: array = [1, 2, 3, 4, 5], then this method will return
        ///     array.SplitElementsBy(2) -&gt; [[1, 2], [3, 4], [5]]
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <typeparam name="T">The data type of the contents of the array.</typeparam>
        /// <param name="array">The array to be split.</param>
        /// <param name="splitFactor">The split factor.</param>
        /// <returns>The newly split array with splits determined by the splitFactor</returns>
        public static T[][] SplitElementsBy<T>(this T[] array, int splitFactor)
        {
            var container = new List<T>(array);

            var size = array.Length / splitFactor;
            size = array.Length % splitFactor != 0 ? size + 1 : size;

            var splitContainer = new List<T[]>();
            var amountToTake = splitFactor;
            for (var i = 0; i < size; i++)
            {
                if (container.Count < splitFactor)
                {
                    amountToTake = container.Count;
                }

                splitContainer.Add(container.Take(amountToTake).ToArray());
                container.RemoveRange(0, amountToTake);
            }

            return splitContainer.ToArray();
        }

        /// <summary>
        ///     Combines the contents of the 2d array into a 1 dimensional array.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="array">The two dimensional array to be combined into a one dimensional array.</param>
        /// <returns>A one dimensional array containing the contents of this 2d array.</returns>
        public static T[] CombineContents<T>(this T[][] array)
        {
            var combinedArray = new List<T>();

            foreach (var elements in array)
            {
                combinedArray.AddRange(elements);
            }

            return combinedArray.ToArray();
        }

        #endregion
    }
}