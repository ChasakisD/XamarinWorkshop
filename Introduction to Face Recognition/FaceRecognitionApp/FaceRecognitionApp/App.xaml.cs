using Xamarin.Forms;

namespace FaceRecognitionApp
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

#if DEBUG
            /* If the application is not running on previewer, listen to port */
            if (!DesignMode.IsDesignModeEnabled)
            {
                HotReloader.Current.Start(this);
            }
#endif

            MainPage = new NavigationPage(new ReadyMainPage());
        }
    }
}
