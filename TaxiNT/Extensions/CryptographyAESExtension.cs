using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TaxiNT.Extensions;
public static class CryptographyAESExtension
{
    private const string SecretKey = "NamThangTaxi";

    private static byte[] GetKeyBytes()
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(SecretKey));
    }

    public static string Encrypt(string plainText)
    {
        byte[] key = GetKeyBytes();

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();

        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        var base64 = Convert.ToBase64String(ms.ToArray());
        return ToBase64Url(base64);
    }

    public static string Decrypt(string cipherTextBase64Url)
    {
        var base64 = FromBase64Url(cipherTextBase64Url);
        byte[] fullCipher = Convert.FromBase64String(base64);
        byte[] key = GetKeyBytes();

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, 16);
        aes.IV = iv;

        byte[] cipher = new byte[fullCipher.Length - 16];
        Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);

        return sr.ReadToEnd();
    }

    private static string ToBase64Url(string base64)
    {
        return base64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static string FromBase64Url(string base64Url)
    {
        string base64 = base64Url
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return base64;
    }
}

/* Chuẩn hóa key tìm kiếm tự do, Ví dụ sử dụng:
    string token = request.Token;
    string rawKeyword = SearchCrypto.Decrypt(token);
    // ví dụ: "Nguyễn   Văn A - NV001"
    string keyword = SearchNormalizer.Normalize(rawKeyword);
    // => "NGUYỄN VĂN A - NV001"
*/
public static class SearchNormalizer
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string s = input.Trim();
        s = Regex.Replace(s, @"\s+", " ");
        s = s.ToUpperInvariant();

        return s;
    }
}

