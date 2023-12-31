﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Salt { get; set; }
        public string? ResetToken { get; set; }
        public DateTime ResetDate { get; set; }
        public byte[]? ProfilePicture { get; set; }


    }
}
