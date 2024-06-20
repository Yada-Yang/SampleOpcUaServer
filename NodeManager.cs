using Opc.Ua;
using Opc.Ua.Server;

namespace SampleOpcUaServer
{
    internal class NodeManager : CustomNodeManager2
    {
        public NodeManager(IServerInternal server, params string[] namespaceUris)
            : base(server, namespaceUris)
        {
        }

        public NodeManager(IServerInternal server, ApplicationConfiguration configuration, params string[] namespaceUris)
            : base(server, configuration, namespaceUris)
        {
        }

        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            FolderState root = CreateFolder(null, null, "Root");
            root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder); // 将节点添加到服务器根节点
            root.EventNotifier = EventNotifiers.SubscribeToEvents;
            AddRootNotifier(root);

            CreateVariable(root, null, "Test", BuiltInType.Int64, ValueRanks.Scalar);

            return new NodeStateCollection(new List<NodeState> { root });
        }

        protected virtual FolderState CreateFolder(NodeState? parent, string? path, string name)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = parent?.NodeId.Identifier is string id ? id + "_" + name : name;

            FolderState folder = new FolderState(parent);
            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = new NodeId(path, NamespaceIndex);
            folder.BrowseName = new QualifiedName(path, NamespaceIndex);
            folder.DisplayName = new LocalizedText("en", name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        protected virtual BaseDataVariableState CreateVariable(NodeState? parent, string? path, string name, BuiltInType dataType, int valueRank)
        {
            return CreateVariable(parent, path, name, (uint)dataType, valueRank);
        }

        protected virtual BaseDataVariableState CreateVariable(NodeState? parent, string? path, string name, NodeId dataType, int valueRank)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = parent?.NodeId.Identifier is string id ? id + "_" + name : name;

            BaseDataVariableState variable = new BaseDataVariableState(parent);
            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.None;
            variable.UserWriteMask = AttributeWriteMask.None;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = Opc.Ua.TypeInfo.GetDefaultValue(dataType, valueRank, Server.TypeTree);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        public void UpdateValue(NodeId nodeId, object value)
        {
            var variable = (BaseDataVariableState)FindPredefinedNode(nodeId, typeof(BaseDataVariableState));
            if (variable != null)
            {
                variable.Value = value;
                variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(SystemContext, false);
            }
        }

        public NodeId AddFolder(NodeId parentId, string? path, string name)
        {
            var node = Find(parentId);
            var newNode = CreateFolder(node, path, name);
            AddPredefinedNode(SystemContext, node);
            return newNode.NodeId;
        }

        public NodeId AddVariable(NodeId parentId, string? path, string name, BuiltInType dataType, int valueRank)
        {
            return AddVariable(parentId, path, name, (uint)dataType, valueRank);
        }

        public NodeId AddVariable(NodeId parentId, string? path, string name, NodeId dataType, int valueRank)
        {
            var node = Find(parentId);
            var newNode = CreateVariable(node, path, name, dataType, valueRank);
            AddPredefinedNode(SystemContext, node);
            return newNode.NodeId;
        }
    }
}