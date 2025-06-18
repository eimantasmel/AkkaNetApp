using Akka.Actor;

namespace AkkaNetApp;

public class BaseDeviceActor : ReceiveActor
{
    private readonly string _deviceId;
    private bool _isOnline;

    public override void AroundPreStart()
    {
        base.AroundPreStart();
    }

    public BaseDeviceActor(string deviceId, bool isOnline)
    {
        _deviceId = deviceId;
        _isOnline = isOnline;

        Receive<RegisterDevice>(msg =>
        {
            if (msg.DeviceId == _deviceId)
            {
                _isOnline = msg.IsOnline;
                Console.WriteLine($"BaseDeviceActor {_deviceId}: Updated status to Online={_isOnline}");
            }
        });

        Receive<RequestDeviceStatus>(msg =>
        {
            if (msg.DeviceId == _deviceId)
            {
                Sender.Tell(new DeviceStatusResponse(_deviceId, _isOnline, "Operational"));
            }
        });
    }
}