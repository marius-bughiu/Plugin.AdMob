namespace Plugin.AdMob;

/// <summary>
/// This class describes a reward credited to a user for interacting with a rewarded ad.
/// </summary>
/// <param name="Amount">The reward amount.</param>
/// <param name="Type">The type of the reward.</param>
public sealed record RewardItem(int Amount, string Type);