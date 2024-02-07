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

            var channel = new GuiEventChannel();
            var backendChannel = new BackendEventChannel();
            var backend = new Backend();
            //create backend and gui
            var gui = new MainWindow();
            gui.Populate(channel, backend, backendChannel);
            backend.Populate(channel, backendChannel, gui);

            //start gui thread
            new Thread(p => Application.Run(gui)).Start();

            //load heightmap
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            backend.OnHeightmapPathSelected(null, new FileEventArgs(path + file));

        }




    }
}