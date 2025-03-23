namespace DATNBACK_END.Helpers;
public static class PasswordHasher{
    public static void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt){
        using (var hac = new System.Security.Cryptography.HMACSHA512()){
            PasswordSalt = hac.Key;
            PasswordHash = hac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
            //System.Text.Encoding.UTF8.GetBytes(password) chuyen hoa password dang string sang dang byte theo ma hoa utf8
        }
    }
    public static bool VerifyPasswordHash(string Password, byte[] storedHash, byte[] storedSalt){
        using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt)){
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}