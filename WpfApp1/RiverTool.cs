using WpfApp1.components;

namespace WpfApp1;

public partial class MainWindow
{
    public class RiverTool
    {
        private bool _isEnabled;

        public void OnToggleToolClicked(object sender, EventArgs e)
        {
            _isEnabled = !_isEnabled;
            if (sender is ToolButton button)
            {
                button.SetActive(_isEnabled);
            }
        }
    }
}