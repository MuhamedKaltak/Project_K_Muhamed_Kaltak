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
        public async Task<string> HashPassword(string password,string salt)
        {
            int memorySize = 65536; 
            int iterations = 4; 

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
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

        public string GenerateVerificationCode()
        {
            Byte[] bytes;
            String bytesBase64Url; //Detta är Base64Url-Encoded och inte Base64-encoded, vilket innebär att det är säkert att använda i en url, se bara till att konvertera det till Base64 först när man decodar det.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {

                bytes = new Byte[12]; //En multipel av 3 ex(3,6,12..) används för att undvika outputs med trailing padding.
                rng.GetBytes(bytes);


                //Konverterar det först till Base64 och därefter till Base64Url string.
                bytesBase64Url = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
            }

            return bytesBase64Url;
        }

        public async Task<bool> VerifyPassword(User user, string password)
        {
            string hashedPassword = await HashPassword(password, user.Salt);

            return hashedPassword == user.Password;
        }
    }
}
