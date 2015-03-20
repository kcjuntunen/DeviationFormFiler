using System;
using System.Collections.Generic;
using System.Linq;
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
        public ErrWindow()
        {
            InitializeComponent();
        }

        public ErrWindow(Exception ex)
        {
            InitializeComponent();
            _ourException = ex;
            this.ShowErr();
        }

        private void ShowErr()
        {
            System.Media.SystemSounds.Beep.Play();
            this.Title = "Error in " + _ourException.TargetSite;
            string msg = string.Empty;

            msg += string.Format("HRESULT: {0:x}\nMessage: {1}\n\nStack Trace:\n{2}\n\n", _ourException.HResult, _ourException.Message,_ourException.StackTrace);

            if (_ourException.InnerException != null)
            {
                msg += CatchInnerExceptions(_ourException.InnerException);
            }

            this.tbErrMsg.Text = msg;
        }

        private string CatchInnerExceptions(Exception ex)
        {
            string msg = string.Empty;
            Exception inEx = _ourException.InnerException;
            msg += "Inner exception:\n";
            msg += string.Format("HRESULT: {0:x}\nMessage: {1}\n\nStack Trace:\n{2}\n\n", inEx.HResult, inEx.Message, inEx.StackTrace);

            //Recursive! Watch out!
            if (inEx.InnerException != null)
                msg += CatchInnerExceptions(ex);

            return msg;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
