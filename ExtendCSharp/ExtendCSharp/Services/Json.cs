﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ExtendCSharp.Services
{
    public class Json
    {
        public static T Deserialize<T>(String s)
        {
            /* MemoryStream stream = new MemoryStream();
             StreamWriter writer = new StreamWriter(stream);
             writer.Write(s);
             writer.Flush();
             stream.Position = 0;
             DataContractJsonSerializer d = new DataContractJsonSerializer(typeof(T));
             T t = (T)d.ReadObject(stream);
             writer.Close();
             writer.Dispose();
             stream.Close();
             stream.Dispose();


             return t;*/

            try
            {
                return JsonConvert.DeserializeObject<T>(s);
            }catch(Exception ex) { return default(T); }
        }
        public static T Deserialize<T>(Stream s)
        {
            /*DataContractJsonSerializer d = new DataContractJsonSerializer(typeof(T));
            return (T)d.ReadObject(s);*/
            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                }
            }
            catch (Exception ex) { return default(T); }
        }

        public static String Serialize(object o)
        {
            try
            {
                return JsonConvert.SerializeObject(o);
            }
            catch (Exception ex) { return default(String); }

            /*MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(o.GetType());
            ser.WriteObject(stream, o);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            String s = sr.ReadToEnd();
            stream.Close();
            stream.Dispose();
            return s;*/
        }
    }
}