using System;
using System.Diagnostics;
using IotDataStation.Common.DataGetter;
using IotDataStation.Common.DataModel;

namespace SampleConsoleApp
{
    public class TestReporter : PollingDataReporterBase
    {
        private Random _random = new Random();
        private Stopwatch _stopwatch = new Stopwatch();
        protected override void DoWorkTick(bool isFirstTick, bool isTestMode)
        {
            if (isFirstTick || _stopwatch.Elapsed.TotalSeconds > 1)
            {
                _stopwatch.Restart();

                if (isFirstTick)
                {
                    NodeItems testNodeItems = new NodeItems();
                    testNodeItems.SetItem("item1", "value1");
                    testNodeItems.SetItem("아이템2", "값2");
                    DataRepository.SetNode("/camera/basic", new Node("c0001", "카메라1", NodeStatus.Normal, "Camera", items: testNodeItems));
                    DataRepository.SetNode("/camera/basic", new Node("c0002", "카메라2", NodeStatus.Normal, "Camera"));
                    DataRepository.SetNode("/camera/basic", new Node("c0003", "카메라3", NodeStatus.Normal, "Camera", items: testNodeItems));
                    DataRepository.SetNode("/camera/composite", new Node("c0002-1", "카메라2-1", NodeStatus.Normal, "Camera", items: testNodeItems));
                    DataRepository.SetNode("/camera/composite/leaf", new Node("c0002-1-1", "카메라12-1-1", NodeStatus.Normal, "Camera", items: testNodeItems));

                    DataRepository.SetNode("/sensor/composite", new Node("s0002-1", "sensor2-1", NodeStatus.Alarm, "Sensor"));
                    DataRepository.SetNode("/sensor/composite/leaf", new Node("s0002-1-1", "sensor12-1-1", NodeStatus.Normal, "Sensor", items: testNodeItems));
                }

                var sen001_1 = new Node("sen001_1", "1층 출입문 센서");
                sen001_1.SetItem("위치", "1층 현관 출입문");
                sen001_1.SetItem("상태", "닫침");
                DataRepository.SetNode("sensor/basic", sen001_1);
                NodePoint point = new NodePoint(37.1235, 127.2222);
                var sen001_2 = new Node("sen001_2", "1층 화제 센서", NodeStatus.Normal, point: point);
                double temperature = Math.Round(_random.NextDouble() * 30.0, 1);
                sen001_2.Attributes["Temperature"] = $"{temperature}";
                sen001_2.SetItem("온도", $"{temperature}도");
                DataRepository.SetNode("sensor/basic", sen001_2);

                temperature = Math.Round(_random.NextDouble() * 30.0, 1);
                var sen001_3 = new Node("sen001_3", "1층 열화상 카메라", NodeStatus.Alarm);
                sen001_3.Attributes["Temperature"] = $"{temperature}";
                sen001_3.SetItem("최고온도", $"{temperature}도", "Alarm");
                sen001_3.SetItem("평균온도", "30.1도", "Normal");
                sen001_3.SetItem("최저온도", "26.5도", "Normal");
                DataRepository.SetNode("sensor/basic", sen001_3);

                _stopwatch.Restart();
            }
        }
    }
}
