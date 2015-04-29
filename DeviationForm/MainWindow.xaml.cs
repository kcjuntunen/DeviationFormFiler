﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviationForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private IniParser ini;
        private DeviationFormHandler dfh;
        private List<FormEntry> CurrentForm;

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
	        this.init();
        }
        
        private void init()
        {
            this.tbFormDir.Text = Properties.Settings.Default.FormDir;
            this.tbTableMap.Text = Properties.Settings.Default.Tables;
            this.tbDataLoc.Text = Properties.Settings.Default.dbLocation;

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;

            if (Properties.Settings.Default.Maximized)
                this.SourceInitialized += (s, a) => this.WindowState = WindowState.Maximized;
        }
        
        private void OpenFile(string fName)
        {
            this.lblEditing.Content = string.Format("Processing \"{0}\"...", fName);

            try
            {
                dfh = new DeviationFormHandler(fName);
                CurrentForm = dfh.GetFormData();
                this.dg1.ItemsSource = CurrentForm;
            }
            catch (DeviationFormHandlerException dEx)
            {
                System.Media.SystemSounds.Beep.Play();
                this.lblEditing.Content = dEx.Message;
            }
            catch (Exception ex)
            {
                ErrWindow err = new ErrWindow(ex);
                err.ShowDialog();
                this.Close();
            }
        }

        private void dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "Portable Document Format (*.pdf)|*.pdf";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                this.OpenFile(dlg.FileName);
            }
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnFileForm_Click(object sender, RoutedEventArgs e)
        {
            if (dfh != null)
                //dfh.InsertData();
                dfh.FilePdf();
            else
                System.Windows.MessageBox.Show("Open a deviation form (*.pdf) first.", "Try again", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void dg1_AutoGeneratedColumns(object sender, EventArgs e)
        {
	    /// The last column just has an object with type info in it. Let's hide it.
            if (this.dg1.Columns.Count > 2)
                this.dg1.Columns[dg1.Columns.Count - 1].Visibility = System.Windows.Visibility.Hidden;
        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;

            if (this.WindowState == System.Windows.WindowState.Maximized)
                Properties.Settings.Default.Maximized = true;
            else
                Properties.Settings.Default.Maximized = false;

            if (!this.btnSaveConfig.IsEnabled)
            {
                Properties.Settings.Default.FormDir = this.tbFormDir.Text;
                Properties.Settings.Default.Tables = this.tbTableMap.Text;
                Properties.Settings.Default.dbLocation = this.tbDataLoc.Text;
            }
            Properties.Settings.Default.Save();
            
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] f = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                this.OpenFile(f[0]);
                //string msg = string.Format("{0}", f[0]);
                //System.Windows.MessageBox.Show(msg);
            }
            else
                System.Windows.MessageBox.Show("I got nothing.");

        }

        private void FileDeviationFormGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void MainGrid_Drop(object sender, DragEventArgs e)
        {
        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            
             System.Windows.MessageBoxResult mssRes = System.Windows.MessageBox.Show(
                "The new configuration will be saved on close. Are you sure?", 
                "Be sure.", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

             if (mssRes == System.Windows.MessageBoxResult.Yes)
             {
                 this.btnSaveConfig.IsEnabled = false;   
             }
        }

        

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchString = "%" + this.tbSearchTerms.Text + "%";
            int checkedBoxes = 0;

            if (this.chSupplier.IsChecked.Value)
                checkedBoxes += (int)DeviationFormHandler.SearchOptions.Supplier;

            if (this.chRequestor.IsChecked.Value)
                checkedBoxes += (int)DeviationFormHandler.SearchOptions.Requestor;

            if (this.chPO.IsChecked.Value)
                checkedBoxes += (int)DeviationFormHandler.SearchOptions.PO;

            if (this.chParts.IsChecked.Value)
                checkedBoxes += (int)DeviationFormHandler.SearchOptions.Parts;

            if (this.chDesc.IsChecked.Value)
                checkedBoxes += (int)DeviationFormHandler.SearchOptions.Description;

            DeviationSearchHandler dsh = new DeviationSearchHandler();

            //if (!dbR.IsClosed)
            //{
            //    while (dbR.Read())
            //    {
            //        lv.Items.Add(dbR.GetOrdinal("supplier_name"));
            //    }

            //    dbR.Close();
            //}
            this.expRes.IsExpanded = true;
            this.btnOpenSelected.Visibility = System.Windows.Visibility.Visible;
            this.dgSearch.ItemsSource = dsh.RunSearch(searchString, checkedBoxes).DefaultView;
           
            //this.dgSearch.Items = dsh.RunSearch(searchString, checkedBoxes);
        }

        private void dgSearch_AutoGeneratedColumns(object sender, EventArgs e)
        {
            //if (this.dgSearch.Columns.Count > 2)
            //    this.dgSearch.Columns[dgSearch.Columns.Count - 1].Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnOpenSelected_Click(object sender, RoutedEventArgs e)
        {
	    // This took a while to figure out
            System.Data.DataRow dr = SelectedRow.Row;
            string procToStart = dr["Location"].ToString();
            //System.Windows.MessageBox.Show(procToStart);
            System.Diagnostics.Process.Start(procToStart);
        }

        private System.Data.DataRowView SelectedRow { get; set; }
	
        private void dgSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
	    // Turns out that, if the DataGrid is not active, its selection is null.
	    // This just remembers the last row selected.
            this.SelectedRow = (System.Data.DataRowView)this.dgSearch.CurrentItem;
        }
    }
}
