namespace Foo.Bar.SampleApp.Services;

internal static class ServiceProvider
{
    public static TService? GetService<TService>()
        => Current.GetService<TService>();

    public static TService GetRequiredService<TService>() where TService : notnull
        => Current.GetRequiredService<TService>();

    public static IServiceProvider Current =>
        (IPlatformApplication.Current ?? throw new InvalidOperationException("Cannot resolve current application.")).Services;
}
