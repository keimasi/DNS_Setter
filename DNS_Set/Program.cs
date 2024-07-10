using System.Management;

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Hello my friend\n");
Console.ResetColor();

string[] dnsServers_403 = { "10.202.10.202", "10.202.10.102" };
string[] dnsServers_Shekan = { "178.22.122.100", "185.51.200.2" };

while (true)
{
    Console.Write("Enter 1 to select '403' DNS or 2 to select 'Shecan': ");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            SetDnsServers(dnsServers_403);
            break;
        case "2":
            SetDnsServers(dnsServers_Shekan);
            break;
        default:
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("\nInvalid input. Please enter 1 or 2.");
            Console.ResetColor();
            continue;
    }
    break;
}

AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

Console.WriteLine("Press Enter to exit and clear DNS settings...");
Console.ReadLine();

static void SetDnsServers(string[] dnsServers)
{
    try
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();

        foreach (ManagementObject mo in moc)
        {
            if ((bool)mo["IPEnabled"])
            {
                ManagementBaseObject newDns = mo.GetMethodParameters("SetDNSServerSearchOrder");
                newDns["DNSServerSearchOrder"] = dnsServers;
                ManagementBaseObject setDns = mo.InvokeMethod("SetDNSServerSearchOrder", newDns, null);

                uint result = (uint)setDns["returnValue"];
                if (result == 0)
                {
                    Console.ForegroundColor= ConsoleColor.Green;
                    Console.WriteLine("\nDNS servers set successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to set DNS servers. Error code: {result}");
                    Console.ResetColor();
                }
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error occurred: {e.Message}");
    }
}

static void OnProcessExit(object sender, EventArgs e)
{
    try
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();

        foreach (ManagementObject mo in moc)
        {
            if ((bool)mo["IPEnabled"])
            {
                ManagementBaseObject newDns = mo.GetMethodParameters("SetDNSServerSearchOrder");
                newDns["DNSServerSearchOrder"] = null;
                ManagementBaseObject setDns = mo.InvokeMethod("SetDNSServerSearchOrder", newDns, null);

                uint result = (uint)setDns["returnValue"];
                if (result == 0)
                {
                    Console.WriteLine("DNS servers cleared successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to clear DNS servers. Error code: {result}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while clearing DNS: {ex.Message}");
    }
}