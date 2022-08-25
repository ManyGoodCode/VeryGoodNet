namespace SharpPak
{
    class Program
    {
        static void Main(string[] args)
        {
            SharpPakApp sharpPak = new SharpPakApp();
            sharpPak.ParseArguments(args);
            sharpPak.Run();
        }
    }
}
