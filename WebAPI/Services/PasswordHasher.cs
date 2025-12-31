using WebAPI.Interfaces;

namespace WebAPI.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt with default work factor (11)
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            // BCrypt.Net handles salt generation automatically
            // Work factor of 11 provides good security/performance balance
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        /// <summary>
        /// Verifies a password against a BCrypt hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="passwordHash">The hashed password to verify against</param>
        /// <returns>True if password matches, false otherwise</returns>
        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                // If hash is invalid format, return false
                return false;
            }
        }
    }
}
