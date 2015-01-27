using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataServices
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(string jsonData) where T : class
        {
            try
            {
                T result = null;
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    result = serializer.ReadObject(stream) as T;
                }

                return result;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error in UserInfoResponse.Deserialize: {0}", ex.Message));
                return default(T);
            }
        }

        public static string Serialize<T>(T jsonData) where T : class
        {
            try
            {
                string result = "";
                
                using (MemoryStream stream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(stream, jsonData);
                    stream.Position = 0;
                    StreamReader sr = new StreamReader(stream);
                    result = sr.ReadToEnd();
                }

                return result;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error in UserInfoResponse.Deserialize: {0}", ex.Message));
                return null;
            }
        }


        public static T Deserialize<T>(Stream jsonDataStream) where T : class
        {
            try
            {
                T result = null;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                result = serializer.ReadObject(jsonDataStream) as T;

                return result;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error in UserInfoResponse.Deserialize: {0}", ex.Message));
                return default(T);
            }
        }

        //public static string Serialize<T>(T t) where T : class
        //{
        //    try
        //    {
        //        string result = null;
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        //            serializer.WriteObject(stream, t);

        //            int length = (int)stream.Length;
        //            byte[] dataBytes = new byte[length];

        //            stream.Position = 0;
        //            stream.Read(dataBytes, 0, length);
        //            result = Encoding.UTF8.GetString(dataBytes, 0, length);
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(String.Format("Error in UserInfoResponse.Deserialize: {0}", ex.Message));
        //        return default(String);
        //    }
        //}
    }
}
