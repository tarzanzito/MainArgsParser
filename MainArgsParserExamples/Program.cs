
#define LNX
//#define PWSH

using System;

namespace Candal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
               new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Start();

#if (DEBUG) && (PWSH)
            args = new string[] { "-EncodedCommand", "-name-item", "alfaBeta", "-go", "my_parm1", "my_parm2" };
            //args = new string[] { "--help" };
#endif

#if (PWSH)
           new PwshParserExample1().Execute(args);
#endif

#if (DEBUG) && (LNX)
            // -c, /c or --create
            // -n=anyContent, /n=anyContent or --name=anyContent
            // -n:anyContent, /n:anyContent or --name:anyContent
            // "-r+, --refresh+ 
            args = new string[] { "--create=xml", "-d=letterD", "-f", "--goal", "--kapa+", "my_argument1", "my_argument2" };
            //args = new string[] { "--help" };
#endif

#if (LNX)
            new LnxParserExample1().Execute(args);
#endif

            Console.WriteLine("Hello World!");
        }

        private static void Start()
        {
            string nameApp = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string nameVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            Console.WriteLine(nameApp + " (ver: " + nameVersion + ")");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("ERROR: [CurrentDomain_UnhandledException");
            Console.WriteLine((e.ExceptionObject as Exception).Message);
            Environment.Exit(1);
        }
    }
}

