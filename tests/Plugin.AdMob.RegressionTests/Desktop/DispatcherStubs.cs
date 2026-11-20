internal class Application
{
    public static Application? Current => null;
    public Dispatcher? Dispatcher => null;
}

internal class Dispatcher
{
    public bool Dispatch(Action action) => false;
}
