using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDataServer.Interface.DataModel
{
    public abstract class IotNode
    {
        protected IotNode(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public abstract string Group { get; }
        public string Name { get; }
        public IotNodeStatus Status { get; }

    }
}
