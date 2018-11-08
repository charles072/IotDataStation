using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot.Common.DataModel;

namespace IotDataServer.Common.DataModel
{
    public class Node : NodeBaseImpl, INode
    {
        public bool IsNullNode => string.IsNullOrWhiteSpace(Id);

        public Node(string id, string name = "", NodeStatus status = NodeStatus.None, string groupName = "", PinObject pin = null, NodeAttributes attributes = null, NodeItems items = null, DateTime? updatedTime = null) : base(id, name, status, groupName, pin, attributes, items, updatedTime)
        {
        }

        public static INode CreateNullNode()
        {
            return new Node("", "", NodeStatus.None);
        }
    }
}
