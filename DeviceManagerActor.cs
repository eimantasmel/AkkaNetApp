using Akka.Actor;

namespace AkkaNetApp;

public class DeviceManagerActor : ReceiveActor
{
    private readonly Dictionary<string, IActorRef> _deviceActors = new();

    public DeviceManagerActor()
    {
        Receive<RegisterDevice>(msg =>
        {
            if (msg == null || string.IsNullOrEmpty(msg.DeviceId))
            {
                Console.WriteLine("DeviceManager: Invalid RegisterDevice message received.");
                return;
            }

            // Create or update BaseDeviceActor for the device
            if (!_deviceActors.ContainsKey(msg.DeviceId))
            {
                var baseDeviceActor = Context.ActorOf(Props.Create(() => new BaseDeviceActor(msg.DeviceId, msg.IsOnline)), $"base-device-{msg.DeviceId}");
                _deviceActors[msg.DeviceId] = baseDeviceActor;
                Console.WriteLine($"DeviceManager: Created BaseDeviceActor for {msg.DeviceId} (Online: {msg.IsOnline})");
            }
            else
            {
                // Update existing BaseDeviceActor's status
                _deviceActors[msg.DeviceId].Tell(msg);
            }

            // If the device is online, create or ensure OnlineDeviceActor exists
            if (msg.IsOnline)
            {
                var onlineDeviceActor = Context.ActorOf(Props.Create(() => new OnlineDeviceActor(msg.DeviceId)), $"online-device-{msg.DeviceId}");
                Console.WriteLine($"DeviceManager: Created OnlineDeviceActor for {msg.DeviceId}");
                // Forward online-specific messages to OnlineDeviceActor
                onlineDeviceActor.Tell(new PerformOnlineAction(msg.DeviceId, "Initialize"));
            }
        });

        Receive<RequestDeviceStatus>(msg =>
        {
            if (msg == null || string.IsNullOrEmpty(msg.DeviceId))
            {
                Sender.Tell(new DeviceStatusResponse("", false, "Invalid status request"));
                return;
            }

            if (_deviceActors.TryGetValue(msg.DeviceId, out var actor))
            {
                // Forward status request to BaseDeviceActor
                actor.Forward(msg);
            }
            else
            {
                Sender.Tell(new DeviceStatusResponse(msg.DeviceId, false, "Device not found"));
            }
        });
    }

    // Static Props method to create DeviceManagerActor instances
    public static Props Props() => Akka.Actor.Props.Create(() => new DeviceManagerActor());
}