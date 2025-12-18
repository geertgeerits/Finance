namespace Finance
{
    public sealed partial class App : Application
    {
        public App()
        {
//#if WINDOWS
//            /*
//            On Windows, apps using WebView2-based controls that are installed to the Program Files directory may fail
//            to render content properly. This occurs because WebView2 attempts to write its cache and user data files
//            to the app's installation directory, which has restricted write permissions in Program Files.
//            To resolve this issue, set the WEBVIEW2_USER_DATA_FOLDER environment variable before any WebView control
//            is initialized. */
//            var userDataFolder = Path.Combine(FileSystem.AppDataDirectory, "WebView2");
//            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", userDataFolder);
//#endif
            InitializeComponent();
        }

        /// <summary>
        /// Window dimensions and location for desktop apps
        /// </summary>
        /// <param name="activationState"></param>
        /// <returns></returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            //return new Window(new AppShell())
            return new Window(new NavigationPage(new MainPage()))
            {
                X = 200,
                Y = 50,
                Height = 900,
                Width = 900,
                MinimumHeight = 800,
                MinimumWidth = 900,
                MaximumHeight = 1100,
                MaximumWidth = 900
            };
        }
    }
}
