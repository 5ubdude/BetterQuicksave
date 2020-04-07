using System;
using System.IO;
using System.Xml.Serialization;

namespace BetterQuicksave.Utils
{
    public static class XmlSerialization
    {
        public static void Serialize<T>(string fileName, T objectToSerialize) where T : class
        {
            var xmlSerializer = new XmlSerializer(objectToSerialize.GetType());
            using (StreamWriter streamWriter = File.CreateText(fileName))
            {
                xmlSerializer.Serialize(streamWriter, objectToSerialize);
            }
        }

        public static T Deserialize<T>(string fileName) where T : class
        {
            T result;
            if (!File.Exists(fileName))
            {
                result = default(T);
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                try
                {
                    using (StreamReader streamReader = File.OpenText(fileName))
                    {
                        result = (T) xmlSerializer.Deserialize(streamReader);
                    }
                }
                catch (Exception)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    result = default(T);
                }
            }

            return result;
        }
    }
}
