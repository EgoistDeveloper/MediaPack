using System.Windows;
using System.Windows.Media.Effects;

namespace MediaPack.Helpers
{
    public static class WindowEffect
    {
        public static void ApplyBlurEffect(Window window, bool apply)
        {
            if (window != null)
            {
                window.Effect = apply ? new BlurEffect() { Radius = 7 } : null;
            }
        }
    }
}
