using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace SampleOpcUaServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 启动OPC UA服务器
            ApplicationInstance application = new ApplicationInstance();
            application.ConfigSectionName = "OpcUaServer";
            application.LoadApplicationConfiguration(false).Wait();
            application.CheckApplicationInstanceCertificate(false, 0).Wait();

            var server = new StandardServer();
            var nodeManagerFactory = new NodeManagerFactory();
            server.AddNodeManager(nodeManagerFactory);
            application.Start(server).Wait();

            // 模拟数据
            var nodeManager = nodeManagerFactory.NodeManager;
            var simulationTimer = new System.Timers.Timer(1000);
            var random = new Random();
            simulationTimer.Elapsed += (sender, EventArgs) =>
            {
                nodeManager?.UpdateValue("ns=2;s=Root_Test", random.NextInt64());
            };
            simulationTimer.Start();

            // 输出OPC UA Endpoint
            Console.WriteLine("Endpoints:");
            foreach (var endpoint in server.GetEndpoints().DistinctBy(x => x.EndpointUrl))
            {
                Console.WriteLine(endpoint.EndpointUrl);
            }

            Console.WriteLine("按Enter添加新变量");
            Console.ReadLine();

            // 添加新变量
            nodeManager?.AddVariable("ns=2;s=Root", "Test2", (int)BuiltInType.Int16, ValueRanks.Scalar);
            Console.WriteLine("已添加变量");
            Console.ReadLine();
        }
    }
}
