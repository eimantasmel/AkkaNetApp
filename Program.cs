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

        // Log status responses using Serilog logger
        Log.Logger.Information("Status for {DeviceId}: Online={IsOnline}, Status={Status}", status1.DeviceId, status1.IsOnline, status1.Status);
        Log.Logger.Information("Status for {DeviceId}: Online={IsOnline}, Status={Status}", status2.DeviceId, status2.IsOnline, status2.Status);
        Log.Logger.Information("Status for {DeviceId}: Online={IsOnline}, Status={Status}", status3.DeviceId, status3.IsOnline, status3.Status);
    }
}