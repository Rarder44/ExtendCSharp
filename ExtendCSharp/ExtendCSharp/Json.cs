﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ExtendCSharp
{
    public class Json
    {
        public static T Deserialize<T>(String s)
        {
            MemoryStream stream = new MemoryStream();
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


            return t;
        }
        public static T Deserialize<T>(Stream s)
        {
            DataContractJsonSerializer d = new DataContractJsonSerializer(typeof(T));
            return (T)d.ReadObject(s);
        }


    }
}
