using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Interfaces
{
    public interface IUserProvider
    {
        User GetUser();
    }
}