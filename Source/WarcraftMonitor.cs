using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InventoryHotkeys
{
    /// <summary>
    /// Checks whether Warcraft III is open and whether the user is in normal gameplay.
    /// </summary>
    class WarcraftMonitor
    {
        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr srchDc, int srcX, int srcY, int srcW, int srcH,
            IntPtr desthDc, int destX, int destY, int op);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        // TODO: Need to check if we're actually in a game. Currently,
        // this will return false if the chat input field is open, otherwise true.
        public static bool IsPlaying()
        {
            if (SearchForWarcraft(out Process warcraftProcess))
            {
                return IsWarcraftGameplay(warcraftProcess);
            }

            return false;
        }

        private static bool SearchForWarcraft(out Process process)
        {
            var processes = Process.GetProcessesByName("Warcraft III");

            if (processes.Length > 0)
            {
                process = processes[0];
                return true;
            }

            process = null;
            return false;
        }

        // We only want to forward key presses when the user is in normal gameplay;
        // otherwise, they will not be able to correctly type chat messages.

        // Checks if the message input field is open by comparing pixel values.
        // We can tell that the window is open when the yellow opaque border is present
        // on the screen.

        // TODO: Probably doesn't work for anything other than 1920x1080, and maybe issues
        // with different gamma settings (does Reforged even have a gamma slider?).
        private static bool IsWarcraftGameplay(Process warcraftProcess)
        {
            IntPtr handle = warcraftProcess.MainWindowHandle;
            IntPtr foreground = GetForegroundWindow();

            // Check if Warcraft is focused.
            if (handle != foreground)
                return false;

            using Bitmap gold = GetPixelsFromWindow(handle, 1087, 12, 1, 1);
            using Bitmap lumber = GetPixelsFromWindow(handle, 1238, 12, 1, 1);

            // *Both* must fail the test to ensure we are not in standard gameplay,
            // as the mouse cursor can cover one of the icons.
            if (lumber.GetPixel(0, 0) != Color.FromArgb(52, 146, 38) && gold.GetPixel(0, 0) != Color.FromArgb(238, 214, 98))
                return false;

            using Bitmap messageBoxBitmap = GetPixelsFromWindow(handle, 620, 841, 512, 1);

            // Count how many pixels in the area match. Some may be incorrect due to the mouse cursor
            // hovering the message box; to account for this, we assume if more than 64 are incorrect
            // the message box is closed.
            Color a = Color.FromArgb(255, 245, 194, 37), b = Color.FromArgb(255, 246, 194, 37);

            int count = 0;

            for (int i = 0; i < messageBoxBitmap.Width; i++)
            {
                Color pixel = messageBoxBitmap.GetPixel(i, 0);

                if (pixel != a && pixel != b)
                {
                    count++;
                }
            }

            return count > 64;
        }

        private static Bitmap GetPixelsFromWindow(IntPtr windowHandle, int x, int y, int width, int height)
        {
            var bitmap = new Bitmap(width, height);

            using Graphics gdest = Graphics.FromImage(bitmap);
            using Graphics gsrc = Graphics.FromHwnd(windowHandle);

            IntPtr hsrcdc = gsrc.GetHdc();
            IntPtr hdc = gdest.GetHdc();

            BitBlt(hdc, 0, 0, width, height, hsrcdc, x, y, (int)CopyPixelOperation.SourceCopy);

            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();

            return bitmap;
        }
    }
}
