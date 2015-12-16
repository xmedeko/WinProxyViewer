using System;
using System.Diagnostics;
using System.IO;

namespace WinProxyViewer
{
    public class Program : IDisposable
    {
        private static void Main(string[] args)
        {
            Program program = new Program();
            program.ParseArgs(args);

            ProxySettingsViewer viewer = new ProxySettingsViewer(program.TextWriter);
            viewer.PrintProxyConfiguration();
            program.Exit();
        }

        /// <summary>
        /// Wait for a key when exiting.
        /// </summary>
        private bool _exitWait;

        /// <summary>
        /// Open specified file name on exit.
        /// </summary>
        private string _exitOpenFileName;

        public Program()
        {
            ProgramName = System.AppDomain.CurrentDomain.FriendlyName;
            ProgramName = Path.GetFileNameWithoutExtension(ProgramName);

            // output to console by default
            TextWriter = Console.Out;
            _exitWait = true;
        }

        /// <summary>
        /// Where to write the results.
        /// </summary>
        public TextWriter TextWriter { get; private set; }

        /// <summary>
        /// Name of this executable program file.
        /// </summary>
        public string ProgramName { get; private set; }

        /// <summary>
        /// Parce command line arguments and set the inner variables.
        /// </summary>
        private void ParseArgs(string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg))
                    continue;

                string argLower = arg.ToLower();
                if (argLower == "-h")
                    HelpAndExit();

                if (argLower == "-o")
                {
                    if (i >= args.Length - 1)
                        HelpAndExit();
                    i++;

                    TextWriter = new StreamWriter(args[i]);
                    _exitWait = false;
                    continue;
                }

                if (argLower == "-tmp")
                {
                    string tmp = Path.GetTempFileName();
                    _exitOpenFileName = Path.ChangeExtension(tmp, "txt");
                    File.Move(tmp, _exitOpenFileName);

                    TextWriter = new StreamWriter(_exitOpenFileName);
                    _exitWait = false;
                    continue;
                }
            }
        }

        /// <summary>
        /// Write help and exit.
        /// </summary>
        private void HelpAndExit()
        {
            Console.WriteLine(string.Format("Usage: {0} [-h] [-tmp] [-o filename]", ProgramName));
            _exitWait = true;
            Exit();
        }

        /// <summary>
        /// Do exit with some actions.
        /// </summary>
        private void Exit()
        {
            Dispose();
            if (_exitWait)
            {
                Console.WriteLine("Press any key.");
                Console.ReadKey();
            }

            if (_exitOpenFileName != null)
                OpenFile(_exitOpenFileName);

            Environment.Exit(0);
        }

        /// <summary>
        /// Open specified file.
        /// </summary>
        private void OpenFile(string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot open " + fileName);
            }
        }

        public void Dispose()
        {
            if (TextWriter == null || TextWriter == Console.Out)
                return;
            TextWriter.Dispose();
        }
    }
}