using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace DeviationForm
{
    class DeviationDataHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="form">A <see cref="List<>"/> of <see cref="FormEntry"/>s.</param>
        /// <param name="provider">The "provider" part of a connection string.</param>
        /// <param name="location">The "data source" part of a connection string.</param>
        public DeviationDataHandler(List<FormEntry> form, string provider, string location)
        {
            this.CurrentForm = form;
            this.DbProvider = provider;
            this.DbLocation = location;
        }

        public void InsertData()
        {
            StringBuilder sb = new StringBuilder("INSERT INTO dev_reqs ");
            sb.Append(" (");
            string connectionString = "Provider=" + DbProvider + ";Data Source=" + DbLocation;
            System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            System.Data.OleDb.OleDbCommand com = con.CreateCommand();

            for (int i = 0; i < CurrentForm.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(CurrentForm[i].FormField.DataFieldName);
            }

            sb.Append(") ");
            sb.Append("VALUES (");

            for (int i = 0; i < CurrentForm.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(CurrentForm[i].Value);
            }

            sb.Append(");");

            //if (System.Windows.MessageBox.Show(sb.ToString(), "I'm gonna run this.", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question)
            //    == System.Windows.MessageBoxResult.OK)
            //{
            try
            {
                con.Open();
            }
            catch (InvalidOperationException ioEx)
            {
                throw new DeviationDataHandlerException("The connection is already open.", ioEx);
            }
            catch   (OleDbException odEx)
            {
                throw new DeviationDataHandlerException("A connection-level error occurred while opening the connection.", odEx);
            }

            com.CommandText = sb.ToString();

            try
            {
                int aff = com.ExecuteNonQuery();
                //System.Windows.MessageBox.Show(string.Format("We affected {0} rows.", aff));
            }
            catch (InvalidOperationException ioEx)
            {
                throw new DeviationDataHandlerException("The connection is not open or attempted to execute a " +
                    "command within a transaction context that differs from the context in which the connection was originally enlisted.", ioEx);
            }

            con.Close(); // throws no exceptions
            //}
        }

        public string DbProvider { get; set; }
        public string DbLocation { get; set; }
        public List<FormEntry> CurrentForm { get; set; }
    }
}
