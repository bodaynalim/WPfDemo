using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using HomeWork.Model;
using HomeWork.ViewModel;

namespace HomeWork.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = Colors.LightSkyBlue;

            var s = (value as FileTree)?.ChangeType;

            //  Add code here to pick a color or generate RGB values for one
            switch (s)
            {
                case WatcherChangeTypes.Created:
                    color = Colors.PaleGreen;
                    break;
                case WatcherChangeTypes.Deleted:
                    color = Colors.PaleVioletRed;
                    break;
                case WatcherChangeTypes.Changed:
                    color = Colors.YellowGreen;
                    break;
                case WatcherChangeTypes.Renamed:
                    color = Colors.Orange;
                    break;
                case WatcherChangeTypes.All:
                    break;
                case null:

                    return null;
            }

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
