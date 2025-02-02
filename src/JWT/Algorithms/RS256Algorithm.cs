﻿using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace JWT.Algorithms
{
    /// <summary>
    /// RSASSA-PKCS1-v1_5 using SHA-256
    /// </summary>
    public sealed class RS256Algorithm : IJwtAlgorithm
    {
        private readonly RSA _publicKey;
        private readonly RSA _privateKey;

        /// <summary>
        /// Creates an instance of <see cref="RS256Algorithm" /> using the provided pair of public and private keys.
        /// </summary>
        /// <param name="publicKey">The public key for verifying the data.</param>
        /// <param name="privateKey">The private key for signing the data.</param>
        public RS256Algorithm(RSA publicKey, RSA privateKey)
        {
            _publicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        }

        /// <summary>
        /// Creates an instance of <see cref="RS256Algorithm" /> using the provided public key only.
        /// </summary>
        /// <remarks>
        /// An instance created using this constructor can only be used for verifying the data, not for signing it.
        /// </remarks>
        /// <param name="publicKey">The public key for verifying the data.</param>
        public RS256Algorithm(RSA publicKey)
        {
            _publicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            _privateKey = null;
        }

        /// <summary>
        /// Creates an instance using the provided certificate.
        /// </summary>
        /// <param name="cert">The certificate having both public and private keys.</param>
        public RS256Algorithm(X509Certificate2 cert)
            : this(GetPublicKey(cert), GetPrivateKey(cert))
        {
        }

        /// <inheritdoc />
        public string Name => JwtHashAlgorithm.RS256.ToString();

        /// <inheritdoc />
        public bool IsAsymmetric => true;

        /// <inheritdoc />
        public byte[] Sign(byte[] key, byte[] bytesToSign) =>
            _privateKey is object ? Sign(bytesToSign) : throw new InvalidOperationException("Can't sign data without private key");

        /// <summary>
        /// Signs the provided bytes.
        /// </summary>
        /// <param name="bytesToSign">The bytes to sign.</param>
        /// <returns>The signed bytes.</returns>
        public byte[] Sign(byte[] bytesToSign) =>
            _privateKey.SignData(bytesToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        /// <summary>
        /// Verifies provided byte array with provided signature.
        /// </summary>
        /// <param name="bytesToSign">The data to verify</param>
        /// <param name="signature">The signature to verify with</param>
        public bool Verify(byte[] bytesToSign, byte[] signature) =>
            _publicKey.VerifyData(bytesToSign, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        private static RSA GetPrivateKey(X509Certificate2 cert)
        {
#if NETSTANDARD1_3
            return cert.GetRSAPrivateKey();
#else
            return (RSA)cert.PrivateKey;
#endif
        }

        private static RSA GetPublicKey(X509Certificate2 cert)
        {
#if NETSTANDARD1_3
            return cert.GetRSAPublicKey();
#else
            return (RSA)cert.PublicKey.Key;
#endif
        }
    }
}
