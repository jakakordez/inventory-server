using System.Security.Cryptography;
using System.Text;
using InventoryServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer;

public class AuthService
{
    private readonly DatabaseContext db;

    public AuthService(DatabaseContext db)
    {
        this.db = db;
    }

    public async Task<User?> Authenticate(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return null;

        // Validate password using the custom hashing mechanism
        if (VerifyPassword(password, user.Password, user.Salt))
        {
            return user;
        }

        return null;
    }

    private bool VerifyPassword(string password, string storedHash, string salt)
    {
        string salted = password + "{" + salt + "}";
        byte[] saltedBytes = Encoding.UTF8.GetBytes(salted);

        // Initial SHA-512 hash (binary output)
        byte[] digest = ComputeSHA512(saltedBytes);

        return storedHash == Convert.ToBase64String(digest);
    }

    private static byte[] ComputeSHA512(byte[] input)
    {
        using (SHA512 sha512 = SHA512.Create())
        {
            return sha512.ComputeHash(input);
        }
    }
}
