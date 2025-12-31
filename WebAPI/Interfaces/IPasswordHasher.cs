namespace WebAPI.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="passwordHash">The hashed password to verify against</param>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
