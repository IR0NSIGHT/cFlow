using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WpfApp1;

public class MapView : Image
{
    private float scale = 1;
    private float ratio = 1;
    private (int x, int y) position = (0, 0);

   
}