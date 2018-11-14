using IotDataStation.Common.DataGetter;
using IotDataStation.Common.DataModel;

namespace SampleConsoleApp
{
    public class TestReporter : PollingDataReporterBase
    {
        protected override void DoWorkTick(bool isFirstTick, bool isTestMode)
        {
            if (isFirstTick)
            {
                NodeItems testNodeItems = new NodeItems();
                testNodeItems.SetItem("item1", "value1");
                testNodeItems.SetItem("아이템2", "값2");
                DataRepository.SetNode("/camera/basic", new Node("c0001", "카메라1", NodeStatus.Normal, "Camera", items:testNodeItems));
                DataRepository.SetNode("/camera/basic", new Node("c0002", "카메라2", NodeStatus.Normal, "Camera"));
                DataRepository.SetNode("/camera/basic", new Node("c0003", "카메라3", NodeStatus.Normal, "Camera", items: testNodeItems));
                DataRepository.SetNode("/camera/composite", new Node("c0002-1", "카메라2-1", NodeStatus.Normal, "Camera", items: testNodeItems));
                DataRepository.SetNode("/camera/composite/leaf", new Node("c0002-1-1", "카메라12-1-1", NodeStatus.Normal, "Camera", items: testNodeItems));

                DataRepository.SetNode("/sensor/basic", new Node("s0001", "sensor1", NodeStatus.Normal, "Sensor"));
                DataRepository.SetNode("/sensor/basic", new Node("s0002", "sensor2", NodeStatus.Warn, "Sensor", items: testNodeItems));
                DataRepository.SetNode("/sensor/basic", new Node("s0003", "sensor3", NodeStatus.Alarm, "Sensor", items: testNodeItems));
                DataRepository.SetNode("/sensor/composite", new Node("s0002-1", "sensor2-1", NodeStatus.Alarm, "Sensor"));
                DataRepository.SetNode("/sensor/composite/leaf", new Node("s0002-1-1", "sensor12-1-1", NodeStatus.Normal, "Sensor", items: testNodeItems));
            }
        }
    }
}
