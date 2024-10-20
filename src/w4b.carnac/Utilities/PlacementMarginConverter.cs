﻿using Carnac.Logic.Native;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Carnac.Utilities {
    public class PlacementMarginConverter: IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter,
            CultureInfo culture) {
            if ((bool)values[2] == false || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) {
                return new Thickness(0);
            }

            Thickness or = (Thickness)values[0];
            DetailedScreen sc = (DetailedScreen)values[1];

            Thickness th = new Thickness {
                Top = or.Top * (sc.RelativeHeight / sc.Height),
                Bottom = or.Bottom * (sc.RelativeHeight / sc.Height),
                Left = or.Left * (sc.RelativeWidth / sc.Width),
                Right = or.Right * (sc.RelativeWidth / sc.Width)
            };

            return th;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}