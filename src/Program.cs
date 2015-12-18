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

        private TextWriter _textWriter;

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
        }

        /// <summary>
        /// Where to write the results.
        /// </summary>
        public TextWriter TextWriter
        {
            get { return _textWriter; }
            private set
            {
                DisposeTextWriter();
                _textWriter = value;
            }
        }

        /// <summary>
        /// Name of this executable program file.
        /// </summary>
        public string ProgramName { get; private set; }

        /// <summary>
        /// Parse command line arguments and set the inner variables. Set default values.
        /// </summary>
        public void ParseArgs(string[] args)
        {
            ParseArgsInner(args);

            // default settings
            if (TextWriter == null)
                SetTmpTextWriter();
        }

        /// <summary>
        /// Parse command line arguments and set the inner variables.
        /// </summary>
        private void ParseArgsInner(string[] args)
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

                if (argLower == "-tmp")
                {
                    SetTmpTextWriter();
                    continue;
                }

                if (argLower == "-out")
                {
                    TextWriter = Console.Out;
                    _exitWait = true;
                    _exitOpenFileName = null;
                    continue;
                }

                if (argLower == "-o")
                {
                    if (i >= args.Length - 1)
                        HelpAndExit();
                    i++;

                    TextWriter = new StreamWriter(args[i]);
                    _exitWait = false;
                    _exitOpenFileName = null;
                    continue;
                }
            }
        }

        /// <summary>
        /// Set TextWriter to the temporary .txt file.
        /// </summary>
        private void SetTmpTextWriter()
        {
            // create temp .txt file
            string tmpFileName = Path.GetTempFileName();
            _exitOpenFileName = Path.ChangeExtension(tmpFileName, "txt");
            File.Move(tmpFileName, _exitOpenFileName);

            TextWriter = new StreamWriter(_exitOpenFileName);
            _exitWait = false;
        }

        /// <summary>
        /// Write help and exit.
        /// </summary>
        private void HelpAndExit()
        {
            Console.WriteLine(string.Format("Usage: {0} [-h] [-tmp] [-out] [-o filename]", ProgramName));
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
            DisposeTextWriter();
        }

        private void DisposeTextWriter()
        {
            if (TextWriter == null || TextWriter == Console.Out)
                return;
            TextWriter.Dispose();
        }
    }
}