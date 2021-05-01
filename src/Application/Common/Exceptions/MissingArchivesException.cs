using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Exceptions
{
    public class MissingArchivesException : Exception
    {
        public MissingArchivesException()
            : base()
        {
        }

        public MissingArchivesException(Arch archive)
            : base($"Archive \"{archive}\" is missing.")
        {
        }

        public MissingArchivesException(Arch[] archives)
            : base($"Archives \"{String.Join(", ", archives)}\" are missing.")
        {
        }
    }
}
