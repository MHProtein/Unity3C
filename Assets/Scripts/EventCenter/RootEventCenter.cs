using System;
using System.Collections.Generic;

namespace Unity3C.EventCenter
{
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class RootEventCenter : EventCenter<RootEventCenter, string, FunctionWithADict>
    {
        
    }
}