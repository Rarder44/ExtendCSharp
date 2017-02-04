﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendCSharp.Interfaces
{
    public interface ISerializer
    {
        String Serialize();
        void Deserialize(String data);
    }
}
