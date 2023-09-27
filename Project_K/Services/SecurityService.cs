using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Project_K.Model;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;

namespace Project_K.Services
{
    public class SecurityService
    {
        public async Task<string> Hash(string value,string salt)
        {
            int memorySize = 65536; 
            int iterations = 4; 

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(value)))
            {
                argon2.Salt = Encoding.UTF8.GetBytes(salt);
                argon2.DegreeOfParallelism = 16; 

                //(i KB)
                argon2.MemorySize = memorySize;

                
                argon2.Iterations = iterations;

                byte[] hashBytes = await Task.Run(() => argon2.GetBytes(32)); // 32 bytes = 256 bits
                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<string> GenerateRandomSalt(int size)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[size];
                await Task.Run(() => rng.GetBytes(saltBytes));
                return Convert.ToBase64String(saltBytes);
            }
        }

        public async Task<string> GenerateToken(User user)
        {
            byte[] bytes;
            string bytesBase64Url;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                bytes = new byte[12];
                await Task.Run(() => rng.GetBytes(bytes)); // Use Task.Run to run the synchronous operation asynchronously.

                bytesBase64Url = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
            }

            return bytesBase64Url;
        }

        public async Task<bool> VerifyPassword(User user, string password)
        {
            string hashedPassword = await Hash(password, user.Salt);

            return hashedPassword == user.Password;
        }

        public async Task<bool> VerifyToken(User user, string enteredToken)
        {
            TimeSpan timeSpan = DateTime.Now - user.PasswordResetDate;

            if (user.PasswordResetToken == await Hash(enteredToken, user.Salt) && timeSpan.Minutes <= 5)
            {
                return true;
            }

            return false;
        }
    }
}
