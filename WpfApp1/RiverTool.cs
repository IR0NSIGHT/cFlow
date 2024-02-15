using System.Windows.Input;
using cFlowForms;
using WpfApp1.components;

namespace WpfApp1;

public partial class MainWindow
{
    public class RiverTool
    {
        private bool _isEnabled;
        private GuiEventChannel _channel;
        public RiverTool(GuiEventChannel channel)
        {
            this._channel = channel;
        }

        public void OnToggleToolClicked(object sender, bool newState)
        {
            _isEnabled = newState;
        }

        public void OnMapClicked(object sender, ((int x, int y) pos, MouseEventArgs e) args)
        {
            if (!_isEnabled || args.e.LeftButton != MouseButtonState.Pressed) 
                return;

            _channel.RequestRiverChange(new GuiEvents.RiverChangeRequestEventArgs(){ pos = args.pos , splitEveryXBlocks = -1});

        }
    }
}