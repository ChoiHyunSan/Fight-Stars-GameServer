
using System.Text;

public static class PasswordUtil
{
    public static string CreatePassword()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 20);
    }

    public static string Encode(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        byte[] bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(bytes);
    }
    
    public static bool Verify(string originalPassword, string encodedPassword)
    {
        if (string.IsNullOrEmpty(originalPassword) || string.IsNullOrEmpty(encodedPassword))
            return false;

        try
        {
            // 원본 패스워드를 인코딩하여 비교
            string encodedOriginal = Encode(originalPassword);
            return encodedOriginal.Equals(encodedPassword);
        }
        catch
        {
            return false;
        }
    }
}