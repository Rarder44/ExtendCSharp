﻿using ExtendCSharp.Interfaces;


namespace ExtendCSharp.Services
{
    public class _TestService : IService
    {

        public void Test()
        {

#if (NETFX4_6_1)
            Log.Log.AddLog("v4_6_1 was set");
#endif
#if (NETFX4_5)
            Log.Log.AddLog("v4_5 was set");
#endif

#if (NETFX4_0)
        Log.Log.AddLog("v4_0 was set");
#endif
#if (NETFX4_0_3)
        Log.Log.AddLog("v4_0_3 was set");
#endif
#if (NETFX3_5)

        Log.Log.AddLog("v3_5 was set");

#endif


        }
    }
}
