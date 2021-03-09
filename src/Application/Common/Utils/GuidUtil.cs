using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Utils
{
    public class GuidUtil
    {
        public static Guid IntToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}