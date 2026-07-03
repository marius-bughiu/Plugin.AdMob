using System.Reflection;

namespace Foo.Bar.SampleApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var mauiVersion = typeof(Shell).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion.Split('+')[0];

            MainShellContent.Title = $"MAUI {mauiVersion}";
        }
    }
}
