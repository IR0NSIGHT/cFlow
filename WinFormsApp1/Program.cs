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

            var guiChannel = new GuiEventChannel();
            var backend = new Backend();
            //create backend and gui
            var gui = new MainWindow();
            var backendChannel = new BackendEventChannel(gui);

            gui.Populate(guiChannel, backendChannel);
            backend.Populate(guiChannel, backendChannel);

            //start gui thread
            new Thread(p => Application.Run(gui)).Start();

            //load heightmap
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            backend.OnHeightmapPathSelected(null, new FileEventArgs(path + file));

        }




    }
}