using cFlowForms;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var channel = new EventChannel();

            var backend = new Backend(channel);
            //create backend and gui
            var gui = new MainWindow(channel, backend);

            //load heightmap
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            backend.OnHeightmapPathSelected(null, new FileEventArgs(path+file));

            //start gui thread
            new Thread(p => Application.Run(gui)).Start();
        }




    }
}