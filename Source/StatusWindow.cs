using Newtonsoft.Json.Linq;

namespace Warcraft3InventoryHotkeys
{
    class StatusWindow : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public bool IsEnabled
        {
            get => isEnabled;

            set
            {
                isEnabled = value;

                UpdateView();
            }
        }

        public bool IsPolling
        {
            get => isPolling;

            set
            {
                isPolling = value;

                UpdateView();
            }
        }

        private bool isEnabled, isPolling;

        public StatusWindow()
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1, 1);
            Size = new Size(32, 32);

            MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void UpdateView()
        {
            if (IsEnabled)
            {
                BackColor = IsPolling ? Color.FromArgb(96, 156, 73) : Color.FromArgb(173, 49, 49);
            }
            else
            {
                BackColor = Color.FromArgb(122, 122, 122);
            }
        }
    }
}
