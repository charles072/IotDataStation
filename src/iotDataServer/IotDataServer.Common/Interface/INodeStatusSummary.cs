using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDataServer.Common.Interface
{
    public interface INodeStatusSummary
    {
        string Path { get; }
        int PathDepth { get; }
        string Name { get; }
        int TotalCount { get; }
        KeyValuePair<string, int>[] StatusSummaries { get; }
    }
}
