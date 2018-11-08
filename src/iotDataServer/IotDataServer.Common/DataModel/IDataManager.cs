using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDataServer.Common.DataModel
{
    public interface IDataManager
    {
        INode[] GetNodes(string path);
        INode GetNode(string path, string id);
        bool SetNode(string path, INode node);

        IFolder GetFolder(string path, int depth, bool includeNodes);
    }
}
