using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Newtonsoft.Json;

namespace Electricity.Infrastructure.Services
{
    public class FileUserSource : IUserSource
    {
        public User[] ListUsers()
        {
            string users = File.ReadAllText("users.json");

            return JsonConvert.DeserializeObject<User[]>(users);
        }
    }
}