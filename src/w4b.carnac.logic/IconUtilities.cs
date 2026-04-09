using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Carnac.Logic {
    internal static class IconUtilities {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private static readonly ConcurrentDictionary<string, ImageSource> icons = new();

        private static Icon GetProcessIcon(string processFileName) {
            Icon icon = Icon.ExtractAssociatedIcon(processFileName);
            return icon;
        }

        private static ImageSource IconToImageSource(Icon icon) {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap)) throw new Win32Exception();
            wpfBitmap.Freeze();
            return wpfBitmap;
        }

        public static ImageSource GetProcessIconAsImageSource(string processFileName) {
            return icons.GetOrAdd(processFileName, key => {
                Icon icon = GetProcessIcon(key);
                return IconToImageSource(icon);
            });
        }
    }
}