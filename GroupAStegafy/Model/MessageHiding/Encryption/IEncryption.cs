namespace GroupAStegafy.Model.MessageHiding.Encryption
{
    /// <summary>
    ///     Defines behavior for a class to encrypt and decrypt data.
    /// </summary>
    /// <typeparam name="T">The type of data to encrypt/decrypt.</typeparam>
    public interface IEncryption<T>
    {
        #region Methods

        /// <summary>
        ///     Encrypts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The encrypted item.</returns>
        T Encrypt(T item);

        /// <summary>
        ///     Decrypts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The decrypted item.</returns>
        T Decrypt(T item);

        #endregion
    }
}