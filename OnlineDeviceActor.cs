// The OnlineDeviceActor handles online-specific actions for devices that are online.

using Akka.Actor;

namespace AkkaNetApp;

public class OnlineDeviceActor : ReceiveActor
{
    private readonly string _deviceId;

    public OnlineDeviceActor(string deviceId)
    {
        _deviceId = deviceId;

        Receive<PerformOnlineAction>(msg =>
        {
            if (msg.DeviceId == _deviceId)
            {
                Console.WriteLine($"OnlineDeviceActor {_deviceId}: Performing action '{msg.Action}'");
                // Simulate an online-specific action
                Sender.Tell($"Action '{msg.Action}' completed for {_deviceId}");
            }
        });
    }
}