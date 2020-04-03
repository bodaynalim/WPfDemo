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
    public class ElementTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = value as FileTree;

            //  Add code here to pick a color or generate RGB values for one
            switch (element?.Type)
            {
                case ElemetnType.File:
                    return "File";
                case ElemetnType.Directory:
                    return "Folder";
                default:

                    if (element?.RealPath == null)
                        return "";

                    var count = element?.RealPath.Split('\\').Length;

                    return count <= 1 ? "Inbox" : "Folder";

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
