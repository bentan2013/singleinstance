using System;
using System.Threading;
using System.Text;
using System.Windows;

namespace app
{
    class Program
    {
        public static Mutex Mutex;
        public static bool bUseSingleInstance = true;

        #region Might need change according to your requirment

        public static string AppPipeName = "SingleInstanceAppChannel";
        public static string AppName = @"Global\SingleInstanceApp";
        
        public static void ExitThisApp()
        {
            Environment.Exit(0);
        }

        #endregion


        static void Main(string[] args)
        {
            bool isFirstOne = SingleInstanceCheck();
            if(isFirstOne)
            {
                StartServer();
            }
            Console.WriteLine("This SingleInstanceApp Started!");
        }

        public static void HandleNamedPipe_OpenRequest(string filesToOpen)
        {
            Console.WriteLine(filesToOpen);
        }

        public static void StartServer()
        {
            if (bUseSingleInstance)
            {
                NamedPipeManager PipeManager = new NamedPipeManager(AppPipeName);
                PipeManager.StartServer();
                PipeManager.ReceiveString += HandleNamedPipe_OpenRequest;
            }
        }

        public static bool SingleInstanceCheck()
        {
            bool isFirstOne = true;
            if (bUseSingleInstance)
            {
                bool isOnlyInstance = false;
                Mutex = new Mutex(true, AppName, out isOnlyInstance);
                if (!isOnlyInstance)
                {
                    isFirstOne = false;
                    string filesToOpen = " ";
                    var args = Environment.GetCommandLineArgs();
                    if (args != null && args.Length > 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 1; i < args.Length; i++)
                        {
                            sb.AppendLine(args[i]);
                        }
                        filesToOpen = sb.ToString();
                    }

                    var manager = new NamedPipeManager(AppPipeName);

                    // send the message
                    manager.Write(filesToOpen);

                    ExitThisApp();
                }
            }
            return isFirstOne;
        }

    }
}
