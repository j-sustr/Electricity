using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Utils
{
    public static class ArchiveUtils
    {
        public static string CreateMissingArchivesMessage(bool missingMain, bool missingEM)
        {
            string message = $"Missing archives: ";
            message += missingMain ? (nameof(Arch.Main) + ", ") : "";
            message += missingEM ? (nameof(Arch.ElectricityMeter) + ", ") : "";
            return message;
        }
    }
}
