/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.IO;
using log4net;
using Sportradar.MTS.SDK.DemoProject.Example;

namespace Sportradar.MTS.SDK.DemoProject
{
    public class Start
    {
        private static ILog _log;

        private static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            SdkLoggerFactory.Configure(new FileInfo("log4net.sdk.config"));
            _log = LogManager.GetLogger(typeof(Start));
            if (!SdkLoggerFactory.CheckAllLoggersExists())
            {
                _log.Warn("Loggers are not set correctly!");
            }

            var key = 'y';
            while (key.Equals('y'))
            {
                DoExampleSelection();

                Console.WriteLine(string.Empty);
                Console.Write("Want to run another example? (y|n): ");
                key = Console.ReadKey().KeyChar;
                Console.WriteLine(string.Empty);
            }
        }

        private static void DoExampleSelection()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine(" Select which example you want to run:");
            Console.WriteLine(" 1 - Basic \t\t\t(normal sending ticket and receiving response via event handler)");
            Console.WriteLine(" 2 - Blocking \t\t\t(sending ticket and receiving response with blocking mode)");
            Console.WriteLine(" 3 - Reoffer \t\t\t(sending reoffer to the declined ticket and receiving response)");
            Console.WriteLine(" 4 - Cashout \t\t\t(creating and sending cashout ticket)");
            Console.WriteLine(" 5 - NonSrSettle \t\t\t(creating and sending ticket fo settling non-sportradar ticket)");
            Console.WriteLine(" 6 - Examples \t\t\t(creating and sending ticket examples from MTS integration guide)");
            Console.Write(" Enter number: ");
            var k = Console.ReadKey();

            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);

            switch (k.KeyChar)
            {
                case '1':
                    {
                        new Basic(_log).Run();
                        break;
                    }
                case '2':
                    {
                        new Blocking(_log).Run();
                        break;
                    }
                case '3':
                    {
                        new Reoffer(_log).Run();
                        break;
                    }
                case '4':
                {
                    new Cashout(_log).Run();
                    break;
                }
                case '5':
                    {
                        new NonSrSettle(_log).Run();
                        break;
                    }
                case '6':
                    {
                        new Examples(_log).Run();
                        break;
                    }
                default:
                    {
                        DoExampleSelection();
                        break;
                    }
            }
        }
    }
}
