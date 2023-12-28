using InventoryHotkeys;
using Warcraft3InventoryHotkeys;
using Warcraft3InventoryHotkeys.Source;

class Program
{
    private static InterceptKeys interceptor;
    private static StatusWindow statusWindow;
    private static Config config;

    private static bool hotkeysEnabled, hotkeysPolling;

    static void Main()
    {
        config = Config.Load();

        interceptor = new();
        interceptor.OnPress += OnKeyPressed;
        interceptor.Register();

        hotkeysEnabled = true;

        statusWindow = new(config.WindowLocation);
        statusWindow.OnMoved += StatusWindow_OnMoved;

        new Task(Monitor).Start();

        Application.Run(statusWindow);

        interceptor.Dispose();
    }

    private static void StatusWindow_OnMoved()
    {
        config.WindowLocation = statusWindow.Location;

        Config.Save(config);
    }

    static async void Monitor()
    {
        while (true)
        {
            hotkeysPolling = WarcraftMonitor.IsPlaying();

            StatusWindow.IndicatorStatus indicator;

            if (!hotkeysEnabled)
                indicator = StatusWindow.IndicatorStatus.Disabled;
            else if (hotkeysPolling)
                indicator = StatusWindow.IndicatorStatus.Polling;
            else
                indicator = StatusWindow.IndicatorStatus.Idle;

            statusWindow.SetIndicator(indicator);

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
            hotkeysEnabled = !hotkeysEnabled;
        }

        if (hotkeysEnabled && hotkeysPolling)
        {
            if (bindings.ContainsKey(vCode))
            {
                VirtualNumpad.PressDown(bindings[vCode]);
            }
        }
    }
}