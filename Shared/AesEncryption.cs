using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Shared.Security;

public static class AesEncryption
{

    public static readonly byte[] Key =
        Encoding.UTF8.GetBytes(
        "12345678901234567890123456789012");


    public static (string Data, string IV) Encrypt(
        string text)
    {

        using var aes = Aes.Create();

        aes.Key = Key;

        aes.GenerateIV();


        using var encryptor =
            aes.CreateEncryptor();


        byte[] input =
            Encoding.UTF8.GetBytes(text);


        byte[] output =
            encryptor.TransformFinalBlock(
                input,
                0,
                input.Length);


        return
        (
            Convert.ToBase64String(output),
            Convert.ToBase64String(aes.IV)
        );

    }



    public static string Decrypt(
        string data,
        string iv)
    {

        using var aes = Aes.Create();


        aes.Key = Key;

        aes.IV =
            Convert.FromBase64String(iv);



        using var decryptor =
            aes.CreateDecryptor();


        byte[] encrypted =
            Convert.FromBase64String(data);



        byte[] result =
            decryptor.TransformFinalBlock(
                encrypted,
                0,
                encrypted.Length);



        return Encoding.UTF8.GetString(result);

    }

}