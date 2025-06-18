using Akka.Actor;
using Serilog;
using Akka.Logger.Serilog;

namespace AkkaNetApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        // Create the actor system
        using var system = ActorSystem.Create("DeviceSystem");

        // Create the DeviceManager actor, ensuring Props() is called correctly
        var deviceManager = system.ActorOf(DeviceManagerActor.Props(), "device-manager");

        // Register some test devices
        deviceManager.Tell(new RegisterDevice("device1", true));  // Online device
        deviceManager.Tell(new RegisterDevice("device2", false)); // Offline device
        deviceManager.Tell(new RegisterDevice("device3", true));  // Online device

        // Wait briefly to ensure actors process messages
        await Task.Delay(1000);

        // Request status for devices
        var statusRequest1 = new RequestDeviceStatus("device1");
        var statusRequest2 = new RequestDeviceStatus("device2");
        var statusRequest3 = new RequestDeviceStatus("device3");

        // Use Ask pattern to get status responses
        var status1 = await deviceManager.Ask<DeviceStatusResponse>(statusRequest1, TimeSpan.FromSeconds(2));
        var status2 = await deviceManager.Ask<DeviceStatusResponse>(statusRequest2, TimeSpan.FromSeconds(2));
        var status3 = await deviceManager.Ask<DeviceStatusResponse>(statusRequest3, TimeSpan.FromSeconds(2));

        // Print status responses
        Console.WriteLine($"Status for {status1.DeviceId}: Online={status1.IsOnline}, Status={status1.Status}");
        Console.WriteLine($"Status for {status2.DeviceId}: Online={status2.IsOnline}, Status={status2.Status}");
        Console.WriteLine($"Status for {status3.DeviceId}: Online={status3.IsOnline}, Status={status3.Status}");
    }
}