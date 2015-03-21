using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace DeviationForm
{
    class DeviationSearchHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ini">An IniParser object</param>
        public DeviationSearchHandler (IniParser ini)
	    {
            this.Ini = ini;
            this.DbProvider = ini.GetSetting("db", "provider");
            this.DbLocation = ini.GetSetting("db", "location");
	    }

	    
	/// <summary>
	/// Constructs an SQL query, and runs the specified search
	/// </summary>
	/// <param name="searchTerms">Our search terms. Really only one term; we're not that smart yet</param>
	/// <param name="chked">A SearchOptions value. I don't know why, but we're looking for an int</param>
	/// <returns>Returns a <see cref="System.Data.DataTable"> object</returns>
        public System.Data.DataTable RunSearch(string searchTerms, int chked)
        {
            StringBuilder sb = new StringBuilder("SELECT dev_reqs.supplier_name AS Supplier, dev_reqs.reqr_name AS Requestor, " +
                "dev_reqs.reqr_email AS Email, dev_reqs.po as PO, dev_reqs.descr AS Description, dev_reqs.dev_desc AS DeviationDescription, " +
                "dev_reqs.file_loc AS Location FROM dev_reqs ");
            //sb.Append(" (");
            string connectionString = "Provider=" + DbProvider + ";Data Source=" + DbLocation;
            System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            System.Data.OleDb.OleDbCommand com = con.CreateCommand();

            if ((chked & (int)DeviationFormHandler.SearchOptions.Supplier) == (int)DeviationFormHandler.SearchOptions.Supplier)
            {
                if (!(sb.ToString().EndsWith("dev_reqs ")))
                    sb.Append(" OR ");
                else
                    sb.Append(" WHERE ");

                sb.Append("dev_reqs.supplier_name Like '" + searchTerms.Replace("'", "''") + "' ");
            }

            if ((chked & (int)DeviationFormHandler.SearchOptions.Requestor) == (int)DeviationFormHandler.SearchOptions.Requestor)
            {
                if (!sb.ToString().EndsWith("dev_reqs "))
                    sb.Append(" OR ");
                else
                    sb.Append(" WHERE ");

                sb.Append("dev_reqs.reqr_name Like '" + searchTerms.Replace("'", "''") + "' ");
            }

            if ((chked & (int)DeviationFormHandler.SearchOptions.PO) == (int)DeviationFormHandler.SearchOptions.PO)
            {
                if (!sb.ToString().EndsWith("dev_reqs "))
                    sb.Append(" OR ");
                else
                    sb.Append(" WHERE ");

                sb.Append("dev_reqs.po Like '" + searchTerms.Replace("'", "''") + "' ");
            }

            if ((chked & (int)DeviationFormHandler.SearchOptions.Parts) == (int)DeviationFormHandler.SearchOptions.Parts)
            {
                if (!sb.ToString().EndsWith("dev_reqs "))
                    sb.Append(" OR ");
                else
                    sb.Append(" WHERE ");

                sb.Append("dev_reqs.parts Like '" + searchTerms.Replace("'", "''") + "' ");
            }

            if ((chked & (int)DeviationFormHandler.SearchOptions.Description) == (int)DeviationFormHandler.SearchOptions.Description)
            {
                if (!sb.ToString().EndsWith("dev_reqs "))
                    sb.Append(" OR ");
                else
                    sb.Append(" WHERE ");

                sb.Append(" dev_reqs.descr Like '" + searchTerms.Replace("'", "''") + "' ");
                sb.Append(" OR dev_reqs.dev_desc Like '" + searchTerms.Replace("'", "''") + "' ");
                sb.Append(" OR dev_reqs.dev_reas Like '" + searchTerms.Replace("'", "''") + "' ");
                sb.Append(" OR dev_reqs.comm Like '" + searchTerms.Replace("'", "''") + "' ");
            }

            //sb.Append(";");
            //System.Windows.MessageBox.Show(sb.ToString());
            //System.Windows.Clipboard.SetText(sb.ToString());

            com.Connection = con;

            try
            {
                con.Open();
            }
            catch (InvalidOperationException ioEx)
            {
                throw new DeviationDataHandlerException("The connection is already open.", ioEx);
            }
            catch (OleDbException odEx)
            {
                throw new DeviationDataHandlerException("A connection-level error occurred while opening the connection.", odEx);
            }

            com.CommandText = sb.ToString();
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                System.Data.Common.DbDataReader dbR = com.ExecuteReader();
                dt.Load(dbR);
            }
            catch (InvalidOperationException ioEx)
            {
                throw new DeviationDataHandlerException("Cannot execute a command within a transaction context that differs " +
                    "from the context in which the connection was originally enlisted.", ioEx);
            }
            //System.Data.DataTableReader dtr = new System.Data.DataTable().CreateDataReader();
            return dt;
        }

        //public void SearchForms(string searchTerms, int flags)
        //{
        //    DeviationDataHandler ddh = new DeviationDataHandler(CurrentForm, Ini.GetSetting("DB", "provider"), Ini.GetSetting("DB", "location"));
        //    ddh.RunSearch(searchTerms, flags);
        //}

        private IniParser Ini { get; set; }
        private string DbProvider { get; set; }
        private string DbLocation { get; set; }

    }
}
