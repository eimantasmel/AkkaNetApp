// Define the message types for actor communication. These are immutable records that actors send and receive.



namespace AkkaNetApp;

// Message to register a new device with the DeviceManager
public record RegisterDevice(string DeviceId, bool IsOnline);

// Message to request device status
public record RequestDeviceStatus(string DeviceId);

// Response with device status
public record DeviceStatusResponse(string DeviceId, bool IsOnline, string Status);

// Message for online-specific actions
public record PerformOnlineAction(string DeviceId, string Action);