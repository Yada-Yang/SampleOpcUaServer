using Opc.Ua;
using Opc.Ua.Server;

namespace SampleOpcUaServer
{
    internal class NodeManagerFactory : INodeManagerFactory
    {
        public NodeManager? NodeManager { get; private set; }
        public StringCollection NamespacesUris => new StringCollection() { "http://opcfoundation.org/OpcUaServer" };

        public INodeManager Create(IServerInternal server, ApplicationConfiguration configuration)
        {
            if (NodeManager != null)
                return NodeManager;

            NodeManager = new NodeManager(server, configuration, NamespacesUris.ToArray());
            return NodeManager;
        }
    }
}
