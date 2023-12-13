﻿using System;
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
            return await Task.Run(() =>
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

                    byte[] hashBytes = argon2.GetBytes(32); // 32 bytes = 256 bits
                    return Convert.ToBase64String(hashBytes);
                }
            });

            
        }

        public async Task<string> GenerateRandomSalt(int size)
        {
            return await Task.Run(() =>
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] saltBytes = new byte[size];
                    rng.GetBytes(saltBytes);
                    return Convert.ToBase64String(saltBytes);
                }
            });

            
        }

        public async Task<string> GenerateToken()
        {
            return await Task.Run(() =>
            {
                byte[] bytes;
                string bytesBase64Url;

                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    bytes = new byte[12];
                    rng.GetBytes(bytes); // Use Task.Run to run the synchronous operation asynchronously.

                    bytesBase64Url = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
                }

                return bytesBase64Url;
            });
            
        }

        public async Task<bool> VerifyPassword(UserOld user, string password)
        {
            string hashedPassword = await Hash(password, user.Salt);

            return hashedPassword == user.Password;
        }

        public async Task<bool> VerifyToken(UserOld user, string enteredToken)
        {
            TimeSpan timeSpan = DateTime.Now - user.ResetDate;

            if (user.ResetToken == await Hash(enteredToken, user.Salt) && timeSpan.Minutes <= 5)
            {
                return true;
            }

            return false;
        }
    }
}
