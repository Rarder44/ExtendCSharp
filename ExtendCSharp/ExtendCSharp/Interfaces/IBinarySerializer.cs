﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendCSharp.Interfaces
{
    public interface IBinarySerializer
    {
        byte[] Serialize();
        void Deserialize(byte[] data);
    }
}