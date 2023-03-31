using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace MyUtils {
    public static class Crypto {

        private static Logger Logger { get; } = new(new() { Name = "crypto" });

        /// <summary>
        /// A way to convert plain text to cipher text
        /// </summary>
        /// <param name="text">The plain text to encrypt</param>
        /// <returns>The key, nonce, tag, and encrypted text as base64</returns>
        public static (string key, string nonce, string tag, string cipherText) Encrypt(string text) {
            byte[] key = Encoding.ASCII.GetBytes(new MyRandom(32).AlphaNumSpecial(true));
            byte[] nonce = Encoding.ASCII.GetBytes(new MyRandom(AesGcm.NonceByteSizes.MaxSize).AlphaNumSpecial(true));
            byte[] plainText = Encoding.ASCII.GetBytes(text);
            byte[] cipherText = new byte[plainText.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
            using AesGcm aes = new(key);
            aes.Encrypt(nonce, plainText, cipherText, tag);
            return (Convert.ToBase64String(key), Convert.ToBase64String(nonce), Convert.ToBase64String(tag), Convert.ToBase64String(cipherText));
        }

        /// <summary>
        /// A way to convert plain text to cipher text
        /// </summary>
        /// <param name="key">The key to use (must be 16, 24, or 32 characters long)</param>
        /// <param name="text">The plain text to encrypt</param>
        /// <returns>The key, nonce, tag, and encrypted text as base64</returns>
        public static (string key, string nonce, string tag, string cipherText) Encrypt(string key, string text) {
            byte[] _key = Encoding.ASCII.GetBytes(key);
            byte[] nonce = Encoding.ASCII.GetBytes(new MyRandom(AesGcm.NonceByteSizes.MaxSize).AlphaNumSpecial(true));
            byte[] plainText = Encoding.ASCII.GetBytes(text);
            byte[] cipherText = new byte[plainText.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
            using AesGcm aes = new(_key);
            aes.Encrypt(nonce, plainText, cipherText, tag);
            return (Convert.ToBase64String(_key), Convert.ToBase64String(nonce), Convert.ToBase64String(tag), Convert.ToBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts ciphertext to plaintext
        /// </summary>
        /// <param name="key">The key in base64</param>
        /// <param name="nonce">The nonce to use</param>
        /// <param name="tag">The authenication tag associated with the cipher</param>
        /// <param name="cipherText">the cipher text to convert</param>
        /// <returns>The original plaintext</returns>
        public static string Decrypt(string key, string nonce, string tag, string cipherText) {
            byte[] encrypted = Convert.FromBase64String(cipherText);
            byte[] plainText = new byte[encrypted.Length];
            using AesGcm aes = new(Convert.FromBase64String(key));
            aes.Decrypt(Convert.FromBase64String(nonce), encrypted, Convert.FromBase64String(tag), plainText);
            return Encoding.ASCII.GetString(plainText);
        }

        public static X509Certificate2 CreateCert(string commonName, string path, string name = "cert.pfx") {
            var basePath = Path.GetFullPath(path);
            if (!Directory.Exists(basePath)) {
                try {
                    Directory.CreateDirectory(basePath);
                }
                catch (Exception e) {
                    Logger.Fatal(e, "Error creating Cert Directory");
                }
            }
            /* using (ECDsa ec = ECDsaOpenSsl.Create()) {
                logger.Log($"Creating Certificate to {basePath}");
                // ec.GenerateKey(new());
                var cert = new CertificateRequest($"cn={commonName}", ec, new("SHA256")).CreateSelfSigned(DateTimeOffset.UtcNow.AddSeconds(1), DateTimeOffset.UtcNow.AddSeconds(1).AddYears(2));
                // cert.Export(X509ContentType.Cert);
                // var p = Path.GetFullPath("cert.pem", basePath);
                File.WriteAllBytes(Path.GetFullPath("cert.pfx", basePath), cert.Export(X509ContentType.Pfx));
                // File.WriteAllText(Path.GetFullPath("cert.pem", basePath), new String(PemEncoding.Write("CERTIFICATE", cert.RawData)));
                // AsymmetricAlgorithm key = cert.GetECDsaPrivateKey();
                // key = key ?? cert.GetRSAPrivateKey();
                // key = key ?? cert.GetECDiffieHellmanPrivateKey();
                // key = key ?? cert.GetDSAPrivateKey();
                // File.WriteAllText(Path.GetFullPath("pubkey.pem", basePath), new String(PemEncoding.Write("PUBLIC KEY", ec.ExportECPrivateKey())));
                // File.WriteAllText(Path.GetFullPath("key.pem", basePath), new String(PemEncoding.Write("PRIVATE KEY", key.ExportPkcs8PrivateKey())));
                // new X509Certificate2(p);
                logger.Log("Done");
                return cert;
            } */
            using var rsa = RSA.Create();
            Logger.Log($"Creating Certificate to {basePath}");
            // new CertificateRequest()
            var cert = new CertificateRequest($"cn={commonName}", rsa, new("SHA512"), RSASignaturePadding.Pkcs1).CreateSelfSigned(DateTimeOffset.UtcNow.AddSeconds(1), DateTimeOffset.UtcNow.AddSeconds(1).AddYears(2));
            // cert.Export(X509ContentType.Cert);
            // var p = Path.GetFullPath("cert.pem", basePath);
            File.WriteAllBytes(Path.GetFullPath(name, basePath), cert.Export(X509ContentType.Pfx));
            // File.WriteAllText(Path.GetFullPath("cert.pem", basePath), new String(PemEncoding.Write("CERTIFICATE", cert.RawData)));
            // AsymmetricAlgorithm key = cert.GetRSAPrivateKey();
            // key = key ?? cert.GetECDsaPrivateKey();
            // key = key ?? cert.GetECDiffieHellmanPrivateKey();
            // key = key ?? cert.GetDSAPrivateKey();
            // File.WriteAllText(Path.GetFullPath("pubkey.pem", basePath), new String(PemEncoding.Write("PUBLIC KEY", ec.ExportECPrivateKey())));
            // File.WriteAllText(Path.GetFullPath("key.pem", basePath), new String(PemEncoding.Write("PRIVATE KEY", key.ExportPkcs8PrivateKey())));
            // new X509Certificate2(p);
            Logger.Log("Done");
            return cert;
        }
    }
}
