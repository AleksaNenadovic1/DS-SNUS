using System;
using System.Text;
using System.Security.Cryptography;

namespace Shared.Security;

public static class EcdsaSignature
{

    private static readonly ECDsa Key;


    static EcdsaSignature()
    {
        Key = ECDsa.Create();

        Key.ImportFromPem(
        """
        -----BEGIN EC PRIVATE KEY-----
        MHcCAQEEIBcLFLpd7w3A23Dqdiuey2Nr12SBJrQmONbJxfnJmEc1oAoGCCqGSM49
        AwEHoUQDQgAEeAjeyx2TBm4y/B32pboi9tzuTbCjWmdS7+BXcqR56hqNXMA+69kD
        qWQvYL7e/GtNqWovSAOKnuUsXjY9nv97uA==
        -----END EC PRIVATE KEY-----
        """);
    }



    public static string Sign(string text)
    {

        byte[] data =
            Encoding.UTF8.GetBytes(text);


        byte[] signature =
            Key.SignData(
                data,
                HashAlgorithmName.SHA256);


        return Convert.ToBase64String(signature);

    }



    public static bool Verify(
        string text,
        string signature)
    {

        byte[] data =
            Encoding.UTF8.GetBytes(text);



        return Key.VerifyData(
            data,
            Convert.FromBase64String(signature),
            HashAlgorithmName.SHA256);

    }

}