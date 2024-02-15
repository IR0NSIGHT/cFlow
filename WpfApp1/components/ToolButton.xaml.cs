using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for ToolButton.xaml
    /// </summary>
    public partial class ToolButton : UserControl
    {
        public ToolButton()
        {
            InitializeComponent();
            SetActive(false);
            MyButton.Click += OnButtonClick;
        }

        public EventHandler<bool> OnToggledEventHandler;

        private bool active;
        private bool stateless = false;

        private void OnButtonClick(object? sender, EventArgs args)
        {
            OnToggledEventHandler?.Invoke(this, !active);
            SetActive(!active);
        }

        public void SetStateless(bool stateless)
        {
            this.stateless = stateless;
        }

        public void SetActive(bool active)
        {
            if (!stateless)
            {
                this.active = active;
                this.MyButton.Background = active ? Brushes.SlateGray : Brushes.DarkSlateGray;
            }
        }
    }
}
