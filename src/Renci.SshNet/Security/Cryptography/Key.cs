﻿using System;
using System.Collections.Generic;

using Renci.SshNet.Common;
using Renci.SshNet.Security.Cryptography;

namespace Renci.SshNet.Security
{
    /// <summary>
    /// Base class for asymmetric cipher algorithms.
    /// </summary>
    public abstract class Key
    {
        /// <summary>
        /// Specifies array of big integers that represent private key.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        protected BigInteger[] _privateKey;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Gets the default digital signature implementation for this key.
        /// </summary>
        protected internal abstract DigitalSignature DigitalSignature { get; }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        /// <value>
        /// The public.
        /// </value>
#pragma warning disable CA1716 // Identifiers should not match keywords
        public abstract BigInteger[] Public { get; set; }
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Gets the length of the key.
        /// </summary>
        /// <value>
        /// The length of the key.
        /// </value>
        public abstract int KeyLength { get; }

        /// <summary>
        /// Gets or sets the key comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="data">DER encoded private key data.</param>
        protected Key(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var der = new DerData(data);
            _ = der.ReadBigInteger(); // skip version

            var keys = new List<BigInteger>();
            while (!der.IsEndOfData)
            {
                keys.Add(der.ReadBigInteger());
            }

            _privateKey = keys.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        protected Key()
        {
        }

        /// <summary>
        /// Signs the specified data with the key.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <returns>
        /// Signed data.
        /// </returns>
        public byte[] Sign(byte[] data)
        {
            return DigitalSignature.Sign(data);
        }

        /// <summary>
        /// Verifies the signature.
        /// </summary>
        /// <param name="data">The data to verify.</param>
        /// <param name="signature">The signature to verify against.</param>
        /// <returns><see langword="true"/> is signature was successfully verifies; otherwise <see langword="false"/>.</returns>
        public bool VerifySignature(byte[] data, byte[] signature)
        {
            return DigitalSignature.Verify(data, signature);
        }
    }
}
