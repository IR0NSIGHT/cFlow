using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for ToolButton.xaml
    /// </summary>
    public partial class ToolButton : UserControl
    {
        public static DependencyProperty ToolNameProperty =
            DependencyProperty.Register(
                name: "ToolNameProperty",
                propertyType: typeof(string),
                ownerType: typeof(ToolButton)
            );

        public static DependencyProperty StatelessProperty =
            DependencyProperty.Register(
                name: "Stateless",
                propertyType: typeof(bool),
                ownerType: typeof(ToolButton),
                new PropertyMetadata(false)
            );

        public bool Stateless
        {
            get => (bool)GetValue(StatelessProperty);
            set => SetValue(StatelessProperty, value);
        }

        // Declare a read-write property wrapper.
        public string ToolName
        {
            get => (string)GetValue(ToolNameProperty);
            set => SetValue(ToolNameProperty, value);
        }

        public ToolButton()
        {
            InitializeComponent();
            SetActive(false);
            MyButton.Click += OnButtonClick;
            Loaded += ToolButton_Loaded; // Subscribe to the Loaded event
        }

        private void ToolButton_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button has ToolName property: " + ToolName);
        }

        public EventHandler<bool> OnToggledEventHandler;

        private bool active;

        private void OnButtonClick(object? sender, EventArgs args)
        {
            OnToggledEventHandler?.Invoke(this, !active);
            SetActive(!active);
        }

        public bool isActive()
        {
            return active;
        }
        public void SetStateless(bool stateless)
        {
            this.Stateless = stateless;
        }

        public void SetActive(bool active)
        {
            if (!Stateless)
            {
                this.active = active;
                this.MyButton.Background = active ? Brushes.SlateGray : Brushes.DarkSlateGray;
            }
        }
    }
}
