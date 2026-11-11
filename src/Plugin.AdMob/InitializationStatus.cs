namespace Plugin.AdMob;

/// <summary>
/// The initialization state of a mediation adapter.
/// </summary>
public enum AdapterState
{
    /// <summary>
    /// The adapter isn't ready to serve ads: it isn't in the app, failed to initialize or timed out.
    /// </summary>
    NotReady = 0,

    /// <summary>
    /// The adapter is initialized and ready to serve ads.
    /// </summary>
    Ready = 1
}

/// <summary>
/// The status of a mediation adapter once the Google Mobile Ads SDK has started.
/// </summary>
public interface IAdapterStatus
{
    /// <summary>
    /// The adapter's class name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether the adapter is ready to serve ads.
    /// </summary>
    AdapterState State { get; }

    /// <summary>
    /// A description of the adapter's state, for example why it isn't ready.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// How long the adapter took to initialize.
    /// </summary>
    TimeSpan Latency { get; }
}

/// <summary>
/// The status of the Google Mobile Ads SDK once it has started.
/// </summary>
public interface IInitializationStatus
{
    /// <summary>
    /// The status of each mediation adapter.
    /// </summary>
    IReadOnlyList<IAdapterStatus> AdapterStatuses { get; }
}

internal class AdapterStatus(string name, AdapterState state, string description, TimeSpan latency)
    : IAdapterStatus
{
    public string Name { get; } = name;

    public AdapterState State { get; } = state;

    public string Description { get; } = description;

    public TimeSpan Latency { get; } = latency;
}

internal class InitializationStatus(IReadOnlyList<IAdapterStatus> adapterStatuses)
    : IInitializationStatus
{
    public IReadOnlyList<IAdapterStatus> AdapterStatuses { get; } = adapterStatuses;
}
