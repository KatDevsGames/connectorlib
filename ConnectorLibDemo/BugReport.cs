using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace ConnectorLibDemo
{
    /// <summary>
    /// The bug report class. This class handles both the bug submission dialog
    /// and the exception log.
    /// </summary>
    internal partial class BugReport : Form
    {
        /// <summary>
        /// The spacer for the bug reports.
        /// </summary>
        [NotNull]
        private static readonly string _spacer = Environment.NewLine + Environment.NewLine +
                                                 "================================================================" +
                                                 Environment.NewLine + Environment.NewLine;

        [NotNull]
        private static string _exception_log = "ConnectorLibDemo Diagnostic Log" + _spacer;

        /// <summary>
        /// FileStream for the exception log.
        /// </summary>
        [NotNull] private static readonly FileStream _log_file = new FileStream("ExceptionLog", FileMode.Create);

        /// <summary>
        /// The current exception (for the bug report dialog).
        /// </summary>
        private readonly Exception _ex;

        private static bool IsOpen => Application.OpenForms.OfType<BugReport>().Any();

        /// <summary>
        /// Initializes an empty bug report dialog.
        /// This only displays the comments section.
        /// </summary>
        public BugReport()
        {
            InitializeComponent();
            _ex = null;
        }

        /// <summary>
        /// Initializes a full bug report dialog.
        /// </summary>
        /// <param name="e"></param>
        public BugReport([CanBeNull] Exception e)
        {
            InitializeComponent();
            _ex = e;
        }

        /// <summary>
        /// Initializes and launches a bug report dialog.
        /// </summary>
        /// <param name="e">The exception to report.</param>
        public static void ReportBug([CanBeNull] Exception e)
        {
            if (IsOpen) { return; }
            BugReport br = new BugReport(e);
            br.ShowDialog();
        }

        /// <summary>
        /// Logs a message to the exception log.
        /// </summary>
        /// <param name="message">The message to send to the exception log.</param>
        public static void LogMessage([CanBeNull] string message)
        {
            _exception_log += message + _spacer;
            byte[] bytes = Encoding.UTF8.GetBytes(message + _spacer);
            _log_file.Write(bytes, 0, bytes.Length);
            _log_file.Flush(true);
        }

        /// <summary>
        /// Logs an exception to the exception log.
        /// </summary>
        /// <param name="e">The exception to send to the exception log.</param>
        public static void LogBug([CanBeNull] Exception e)
        {
            if (e == null) { return; }
            _exception_log += ExceptionText(e);
            byte[] bytes = Encoding.UTF8.GetBytes(ExceptionText(e));
            try
            {
                _log_file.Write(bytes, 0, bytes.Length);
                _log_file.Flush(true);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        /// <summary>
        /// Load event handler.
        /// </summary>
        /// <param name="sender">This parameter is ignored.</param>
        /// <param name="e">This parameter is ignored.</param>
        private void BugReport_Load([CanBeNull] object sender, [NotNull] EventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (_ex != null) { textState.Text = ExceptionText(_ex); }

            // ReSharper disable once PossibleNullReferenceException
            textState.Text += _spacer.TrimEnd() + Environment.NewLine + _spacer.Trim() + Environment.NewLine + _spacer.TrimStart();
            textState.Text += _exception_log;
        }

        /// <summary>
        /// Shown event handler.
        /// </summary>
        /// <param name="sender">This parameter is ignored.</param>
        /// <param name="e">This parameter is ignored.</param>
        private void BugReport_Shown([CanBeNull] object sender, [NotNull] EventArgs e) { BringToFront(); }

        /// <summary>
        /// Formats an exception for easy reading by a developer.
        /// </summary>
        /// <param name="e">The exception to format.</param>
        /// <returns>The exception information in a easier-to-read format.</returns>
        [NotNull]
        private static string ExceptionText([NotNull] Exception e)
        {
            string retString = string.Empty;
            if (Task.Run((() =>
            {
                //try to force the exception text into english
                //works about half the time - jpech
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                try { retString = "ConnectorLibDemo Diagnostic Log" + _spacer + e.GetType().FullName + Environment.NewLine + e.Message + _spacer + e.Source + _spacer + e.TargetSite + _spacer + CleanStackTrace(e) + _spacer; }
                catch
                {
                    retString = e.Message + _spacer + e.Source + _spacer + e.TargetSite + _spacer + e.StackTrace + _spacer;
                }
                if (e.InnerException != null) { retString += ExceptionText(e.InnerException); }
            })).Wait(250)) { return retString; }
            return "ConnectorLibDemo Diagnostic Log" + _spacer + e.GetType().FullName + Environment.NewLine + e.Message + _spacer + e.Source + _spacer + e.TargetSite + _spacer + CleanStackTrace(e) + _spacer;
        }

        [NotNull]
        private static string CleanStackTrace([NotNull] Exception e) => e.StackTrace.Replace("C:\\Users\\Development\\Documents\\Visual Studio 2017\\Projects\\ConnectorLibDemo", string.Empty, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// ButtonExit Click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click([CanBeNull] object sender, [CanBeNull] EventArgs e) { Close(); }

        private void buttonCopy_Click([CanBeNull]object sender, [CanBeNull] EventArgs e) => Clipboard.SetText(textState?.Text ?? string.Empty);
    }
}