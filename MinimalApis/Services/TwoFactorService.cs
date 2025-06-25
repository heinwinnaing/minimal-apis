using Microsoft.AspNetCore.DataProtection;
using OtpNet;
using QRCoder;
using static QRCoder.PayloadGenerator;

namespace MinimalApis.Services;

public interface ITwoFactorService
{
    (string secretKey, byte[] qrImage) Generate(string email, string issuer = "Tagu");
    bool Verify(string secret, string code);
}
public class TwoFactorService : ITwoFactorService
{
    public (string secretKey, byte[] qrImage) Generate(string email, string issuer = "Tagu")
    {
        string secret = GenerateSecretKey();
        //string uri = $"otpauth://{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={secret}&issuer=TaguTech&digits=6";
        
        string uri = new OtpUri(
            schema: OtpType.Totp,
            secret: secret,
            user: email,
            issuer: issuer,
            digits: 6,
            period: 30).ToString();
        using QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
        using QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        return (secret, qrCode.GetGraphic(20));
    }

    public bool Verify(string secret, string code)
    {
        try 
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            bool result = totp.VerifyTotp(code, out _, new VerificationWindow(0, 0));
            return result;
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }
}
