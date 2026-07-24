namespace Plugin.AdMob.DeviceTests;

public class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new HarnessPage());
}
