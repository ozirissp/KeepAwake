using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace KeepAwake
{
    public class SerializationService
    {
        private readonly string filePath;

        public SerializationService(string filePath)
        {
            this.filePath = filePath;
        }

        public void Save(List<WindowState> windowStates)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<WindowState>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, windowStates);
            }
        }

        public List<WindowState> Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<WindowState>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (List<WindowState>)serializer.Deserialize(reader);
            }
        }
    }
}
