using InventoryHotkeys;

class Program
{
    private static WarcraftMonitor monitor;
    private static InterceptKeys interceptor;

    static void Main()
    {
        monitor = new();
        monitor.StartMonitoring();

        interceptor = new();
        interceptor.OnPress += OnKeyPressed;
        interceptor.Register();

        Application.Run();

        interceptor.Dispose();
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
        if (monitor.IsPlaying)
        {
            if (bindings.ContainsKey(vCode))
            {
                VirtualNumpad.PressDown(bindings[vCode]);
            }
        }
    }
}