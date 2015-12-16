using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace WinProxyViewer
{
    /// <summary>
    /// Main class to print proxy settings.
    /// See http://stackoverflow.com/a/11750887/254109
    /// </summary>
    public class ProxySettingsViewer
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WinhttpCurrentUserIeProxyConfig
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool AutoDetect;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string AutoConfigUrl;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Proxy;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ProxyBypass;
        }

        [DllImport("winhttp.dll", SetLastError = true)]
        private static extern bool WinHttpGetIEProxyConfigForCurrentUser(ref WinhttpCurrentUserIeProxyConfig pProxyConfig);

        private TextWriter _w;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="w">TextWriter where will be written the output.</param>
        public ProxySettingsViewer(TextWriter w)
        {
            _w = w;
        }

        /// <summary>
        /// Start reading proxy config and write it.
        /// </summary>
        public void PrintProxyConfiguration()
        {
            try
            {
                var config = new WinhttpCurrentUserIeProxyConfig();
                WinHttpGetIEProxyConfigForCurrentUser(ref config);

                _w.WriteLine("Autodetect: " + config.AutoDetect);
                _w.WriteLine("Proxy: " + config.Proxy);
                _w.WriteLine("Proxy bypass: " + config.ProxyBypass);
                _w.WriteLine("Auto config URL: " + config.AutoConfigUrl);

                if (!string.IsNullOrEmpty(config.AutoConfigUrl))
                {
                    _w.WriteLine("----- Auto config content -----");

                    Uri configUrl = new Uri(config.AutoConfigUrl);

                    // I do not know if the
                    PrintUrlContent(configUrl);
                }
            }
            catch (Exception e)
            {
                _w.WriteLine("Cannot read proxy config.", e);
            }
        }

        /// <summary>
        /// Prints the content of the URL. Do not use proxy.
        /// </summary>
        private void PrintUrlContent(Uri url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                req.Proxy = new WebProxy(); // no proxy

                using (WebResponse response = req.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    _w.Write(reader.ReadToEnd());
            }
            catch (Exception e)
            {
                Error("Reading Auto config.", e);
            }
        }

        /// <summary>
        /// Prints error message.
        /// </summary>
        /// <param name="msg"></param>
        private void Error(string msg, Exception e = null)
        {
            _w.WriteLine("ERROR " + msg + (e == null ? "" : e.Message));
        }
    }
}