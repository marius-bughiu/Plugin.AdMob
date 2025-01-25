namespace Plugin.AdMob.Configuration;

/// <summary>
/// Debug settings to hardcode in consent requests during testing.
/// </summary>
public class ConsentDebugSettings
{
    internal static ConsentDebugSettings Current { get; set; }

    /// <summary>
    /// The debug geography used for testing purposes.
    /// </summary>
    public ConsentDebugGeography Geography { get; set; }

    /// <summary>
    /// The hashed device IDs that should be considered test device.
    /// </summary>
    public IList<string> TestDeviceHashedIds { get; set; } = [];

    /// <summary>
    /// When true, the consent information will be reset every time.
    /// </summary>
    public bool Reset { get; set; }

    /// <summary>
    /// Registers a device as a test device.
    /// </summary>
    /// <param name="hashedDeviceId">The hashed device ID that should be considered a test device.</param>
    public void AddTestDeviceHashedId(string hashedDeviceId)
    {
        TestDeviceHashedIds.Add(hashedDeviceId);
    }
}

/// <summary>
/// Debug values for testing geography.
/// </summary>
public enum ConsentDebugGeography
{
    /// <summary>
    /// Debug geography disabled for debug devices.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Geography appears as in EEA for debug devices.
    /// </summary>
    Eea = 1,

    /// <summary>
    /// Geography appears as in a regulated US State for debug devices.
    /// </summary>
    RegulatedUsState = 3,

    /// <summary>
    /// Geography appears as in a region with no regulation in force for debug device.
    /// </summary>
    Other = 4,
}
