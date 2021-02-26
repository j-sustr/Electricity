using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Electricity.Application.Common.Extensions
{
    public static class ObjectExtensions
    {
        // extends any class that's been marked as [Serializable] with a DeepClone method
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}