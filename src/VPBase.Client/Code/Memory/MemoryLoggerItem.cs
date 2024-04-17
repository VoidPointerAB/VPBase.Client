using System;
using System.Collections.Generic;

namespace VPBase.Client.Code.Memory
{
    public class MemoryLoggerItem
    {
        internal MemoryLoggerItem(int maxLogsCount)
        {
            logList = new List<Tuple<DateTime, string>>(maxLogsCount);
        }

        internal int currentLogIndex = 0;

        internal List<Tuple<DateTime, string>> logList;
    }
}