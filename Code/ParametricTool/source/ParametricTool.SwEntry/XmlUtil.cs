using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParametricTool.SwEntry
{
    public static class XmlUtil
    {
        /// <summary>
        /// 将对象保存至xml文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void Ex_SaveToXML(this object obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string content = string.Empty;
            //serialize
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                content = writer.ToString();
            }
            //save to file
            using (StreamWriter stream_writer = new StreamWriter(path))
            {
                stream_writer.Write(content);
            }
        }


        /// <summary>
        /// 从xml文件中反序列化到实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Ex_SerializeFile<T>(this string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(path))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
