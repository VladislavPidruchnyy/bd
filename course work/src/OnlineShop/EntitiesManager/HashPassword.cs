using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace EntitiesManager
{
    public static class HashPassword
    {
        public static string HashCode(string pass)
        {
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, pass);
            sha256Hash.Dispose();
            return hash;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}