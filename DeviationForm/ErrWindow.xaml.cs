using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeviationForm
{
    /// <summary>
    /// Interaction logic for ErrWindow.xaml
    /// </summary>
    public partial class ErrWindow : Window
    {
        private Exception _ourException;
        
        /// <summary>
        /// Instantiates our window
        /// </summary>
        public ErrWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instantiates our window
        /// </summary>
        /// <param name="ex">A System.Exception object</param>
        public ErrWindow(Exception ex)
        {
            InitializeComponent();
            _ourException = ex;
            this.ShowErr();
        }

        /// <summary>
        /// Fills the error textbox
        /// </summary>
        private void ShowErr()
        {
            System.Media.SystemSounds.Beep.Play();
            this.Title = "Error in " + _ourException.TargetSite;
            string msg = string.Empty;

            msg += string.Format("HRESULT: {0:x}\r\nMessage: {1}\r\n\r\nStack Trace:\r\n{2}\r\n\r\n", _ourException.HResult, _ourException.Message,_ourException.StackTrace);

            if (_ourException.InnerException != null)
            {
                msg += CatchInnerExceptions(_ourException);
            }

            this.tbErrMsg.Text = msg;
        }

        /// <summary>
        /// Takes an exception, pulls out its inner exception, and builds an error message
        /// </summary>
        /// <param name="ex">A System.Exception object</param>
        /// <returns>Returns an nicely constructed error message in a string</returns>
        private string CatchInnerExceptions(Exception ex)
        {
            string msg = string.Empty;
            Exception inEx = ex.InnerException;
            msg += "Inner exception:\r\n";
            msg += string.Format("HRESULT: {0:x}\r\nMessage: {1}\r\n\r\nStack Trace:\r\n{2}\r\n\r\n", inEx.HResult, inEx.Message, inEx.StackTrace);

            //Recursive! Watch out!
            if (inEx.InnerException != null)
                msg += CatchInnerExceptions(inEx);

            return msg;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string mailToLink = "mailto:" + Properties.Settings.Default.devEmail +
                "?subject=" + 
                Properties.Settings.Default.errMailSub +
                "&body=" + tbErrMsg.Text;
            System.Diagnostics.Process.Start(mailToLink.Replace("\r\n", "%0A"));
            this.Close();
        }
    }
}
