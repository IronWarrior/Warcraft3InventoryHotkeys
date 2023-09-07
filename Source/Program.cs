using InventoryHotkeys;
using Warcraft3InventoryHotkeys;

class Program
{
    private static InterceptKeys interceptor;
    private static StatusWindow statusWindow;

    static void Main()
    {
        interceptor = new();
        interceptor.OnPress += OnKeyPressed;
        interceptor.Register();

        statusWindow = new();
        statusWindow.IsEnabled = true;

        new Task(Monitor).Start();

        Application.Run(statusWindow);
        interceptor.Dispose();
    }

    static async void Monitor()
    {
        while (true)
        {
            statusWindow.IsPolling = WarcraftMonitor.IsPlaying();

            await Task.Delay(100);
        }
    }

    private static readonly Dictionary<int, VirtualNumpad.Numpad> bindings = new()
    {
        {  84,  VirtualNumpad.Numpad._7 }, // t
        {  89,  VirtualNumpad.Numpad._8 }, // y
        {  71,  VirtualNumpad.Numpad._4 }, // g
        {  72,  VirtualNumpad.Numpad._5 }, // h
        {  66,  VirtualNumpad.Numpad._1 }, // b
        {  78,  VirtualNumpad.Numpad._2 }, // n
    };

    private static void OnKeyPressed(int vCode)
    {
        // Home key.
        if (vCode == 36)
        {
            statusWindow.IsEnabled = !statusWindow.IsEnabled;
        }

        if (statusWindow.IsPolling)
        {
            if (bindings.ContainsKey(vCode))
            {
                VirtualNumpad.PressDown(bindings[vCode]);
            }
        }
    }
}