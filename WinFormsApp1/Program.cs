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

            //create backend and gui
            var backend = new Backend();
            var gui = new MainWindow();

            //connect both via events
            backend.FlowmapChanged += gui.OnFlowmapChanged;
            backend.HeightmapChanged += gui.OnHeightmapChanged;
            backend.RivermapChanged += gui.OnRivermapChanged;
            backend.MessageRaised += gui.OnMessageRaised;
            backend.LoadingStateChanged += gui.OnLoadingStateChanged;

            gui.RiverChangeRequestHandler += backend.OnRiverChangeRequested;
            gui.FlowCalculationRequestHandler += backend.OnFlowGenerationRequested;

            //load heightmap
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            backend.OnHeightmapPathSelected(null, new FileEventArgs(path+file));

            //start gui thread
            new Thread(p => Application.Run(gui)).Start();
        }




    }
}