using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using iTextSharp;
using iTextSharp.text.pdf;

namespace DeviationForm
{
    class DeviationFormHandler
    {
        private string _formDir;
        private string _pdfPath;

        /// <summary>
        /// Possible checkbox options.
        /// </summary>
        /// <remarks>Oh, boy! Oh, boy! I get to use the bitwise flags trick!</remarks>
        [Flags]
        public enum SearchOptions
        {
            None        = 0x00,
            Supplier    = 0x01,
            Requestor   = 0x02,
            PO          = 0x04,
            Parts       = 0x08,
            Description = 0x10
        }

        /// <summary>
        /// Our constructor
        /// </summary>
        /// <param name="pdfPath">Our pdf file</param>
        /// <param name="ini">An IniParser object</param>
        public DeviationFormHandler(string pdfPath)
        {
            this._formDir = Properties.Settings.Default.FormDir;
            this._pdfPath = pdfPath;
        }

        /// <summary>
        /// Our constructor
        /// </summary>
        /// <param name="pdfPath">Our pdf file</param>
        /// <param name="ini">An IniParser object</param>
        public DeviationFormHandler(string pdfPath, IniParser ini)
        {
            this.Ini = ini;
            this._formDir = this.Ini.GetSetting("filelocations", "formdir");
            this._pdfPath = pdfPath;
        }
        
        /// <summary>
        /// Our constructor
        /// </summary>
        /// <param name="pdfPath">Our pdf file</param>
        /// <param name="fDir">The target form directory</param>
        public DeviationFormHandler(string pdfPath, string fDir)
        {
            this._formDir = fDir;
            this._pdfPath = pdfPath;
        }

        /// <summary>
        /// Sucks all the data from a pdf form
        /// </summary>
        /// <returns>Returns a <see cref="List<>"> of <see cref="FormEntry"> objects</returns>
        public List<FormEntry> GetFormData()
        {
            string jsonLocation = Ini.GetSetting("FileLocations", "tables");
            string json = string.Empty;
            try
            {
                json = System.IO.File.ReadAllText(jsonLocation);
            }
            catch (ArgumentException aEx)
            {
                throw new DeviationFormHandlerException(string.Format("'{0}' is a zero-length string, contains only white space, " +
                    "or contains one or more invalid characters as defined by InvalidPathChars.", jsonLocation), aEx);
            }
            catch (System.IO.PathTooLongException ptlEx)
            {
                throw new DeviationFormHandlerException("TableMap path is too long.", ptlEx);
            }
            catch (System.IO.DirectoryNotFoundException dnfEx)
            {
                throw new DeviationFormHandlerException(string.Format("'{0}' has an invalid path.", jsonLocation), dnfEx);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                throw new DeviationFormHandlerException(string.Format("You do not have permission to access '{0}'.", jsonLocation), uaEx);
            }
            catch (System.IO.FileNotFoundException fnfEx)
            {
                throw new DeviationFormHandlerException(string.Format("Could not find '{0}'.", jsonLocation), fnfEx);
            }
            catch (System.Security.SecurityException sEx)
            {
                throw new DeviationFormHandlerException(string.Format("You do not have permission to access '{0}'.", jsonLocation), sEx);
            }
            catch (Exception ex)
            {
                throw new DeviationFormHandlerException("Error opening table map.", ex);
            }

            // I hope there are no unhandled exceptions in here.
            fft = Newtonsoft.Json.JsonConvert.DeserializeObject<FormFieldTranslation[]>(json);

            PdfReader pr = default(PdfReader);

            try
            {
                pr = new PdfReader(this.PdfPath);
            }
            catch (System.IO.IOException ioEx)
            {
                throw new DeviationFormHandlerException(string.Format("Couldn't open '{0}'", this.PdfPath), ioEx);
            }
            catch (Exception ex)
            {
                throw new DeviationFormHandlerException(string.Format("Error opening PdfReader on '{0}'", this.PdfPath), ex);
            }

            List<FormEntry> result = new List<FormEntry>();
            IDictionary<string, AcroFields.Item> d = pr.AcroFields.Fields;
            ICollection<string> k = d.Keys;
            ICollection<AcroFields.Item> v = d.Values;

            if (this.ValidateForm(k))
            {
                foreach (KeyValuePair<string, AcroFields.Item> item in d)
                    for (int i = 0; i < fft.Length; i++)
                        if (item.Key == fft[i].FormFieldName)
                        {
                            result.Add(new FormEntry(item.Key,
                                pr.AcroFields.GetField(item.Key),
                                fft[i]));
                        }
            }
            else
            {
                result.Add(new FormEntry("Err", "Err", fft[0]));
                throw new DeviationFormHandlerException("Form data doesn't match the scheme provided in '" + jsonLocation + "'.");
            }

            pr.Close();
            this.CurrentForm = result;
            return result;
        }

        private void InsertData()
        {
        }

        /// <summary>
        /// Inserts the data, and moves the file
        /// </summary>
        public void FilePdf()
        {
            DeviationFileHandler dFih = new DeviationFileHandler(Ini.GetSetting("FileLocations", "FormDir"), this.PdfPath);
            FormFieldTranslation fft = new FormFieldTranslation();

            fft.FormFieldName = "File Location";
            fft.DataFieldName = "file_loc";
            fft.DataFieldType = "Text";

            string newFile = dFih.GetNextFileName();
            CurrentForm.Add(new FormEntry("File Location", newFile, fft));
            string msg = string.Format("Move '{0}' to '{1}'?", this.PdfPath, newFile );
            if (System.Windows.MessageBox.Show(msg, "Really?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question)
                == System.Windows.MessageBoxResult.Yes)
            {
                DeviationDataHandler ddh = new DeviationDataHandler(CurrentForm, Ini.GetSetting("DB", "provider"), Ini.GetSetting("DB", "location"));
                try
                {
                    dFih.FilePdf();
                }
                catch (DeviationFileHandlerException dfhEx)
                {
                    throw new DeviationFormHandlerException("Filing error.", dfhEx);
                }
                catch (Exception ex)
                {
                    throw new DeviationFormHandlerException("Filing error.", ex);
                }

                try
                {
                    ddh.InsertData();
                }
                catch (DeviationDataHandlerException ddhEx)
                {
                    throw new DeviationDataHandlerException("Database Error.", ddhEx);
                }
                catch (Exception ex)
                {
                    throw new DeviationDataHandlerException("Database Error.", ex);
                }
            }
        }

        private void InsertData2()
        {
            string provider = Ini.GetSetting("DB", "provider");
            string dbPath = Ini.GetSetting("DB", "location");
            StringBuilder sb = new StringBuilder("INSERT INTO dev_reqs ");
            sb.Append(" (");
            string connectionString = "Provider=" + provider + ";Data Source=" + dbPath;
            System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            System.Data.OleDb.OleDbCommand com = con.CreateCommand();

            foreach (FormEntry item in CurrentForm)
                for (int i = 0; i < fft.Length; i++)
                    if (item.Key == fft[i].FormFieldName)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        sb.Append(fft[i].DataFieldName);
                    }

            sb.Append(") ");
            sb.Append("VALUES (");

            foreach (FormEntry item in CurrentForm)
                for (int i = 0; i < fft.Length; i++)
                    if (item.Key == fft[i].FormFieldName)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        sb.Append(item.Value);
                    }
            sb.Append(");");

            if (System.Windows.MessageBox.Show(sb.ToString(), "I'm gonna run this.", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question) 
                == System.Windows.MessageBoxResult.OK)
            {
                con.Open();
                com.CommandText = sb.ToString();
                int aff = com.ExecuteNonQuery();
                System.Windows.MessageBox.Show(string.Format("We affected {0} rows.", aff));
                con.Close();
            }
        }

        /// <summary>
        /// Alphabetizes, then hashes a list of all the fields on the form, comparing the resultant
        /// hash to a saved hash. Makes sure we're using a form that has all the same fields.
        /// </summary>
        /// <param name="k">Takes a <see cref="System.Collections.Generic.ICollection<>"> 
        /// of <see cref="System.String">s</param>
        /// <returns>Returns true if hashes match.</returns>
        private bool ValidateForm(ICollection<string> k)
        {
            string fieldList = string.Empty;

            List<string> l = new List<string>(k);
            l.Sort();

            foreach (string item in l)
                fieldList += item;
             
            byte[] encodedFieldList = new UTF8Encoding().GetBytes(fieldList);
            byte[] hash = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("SHA1")).ComputeHash(encodedFieldList);

            string b = Ini.GetSetting("FileLocations", "TableHash");
            string hashString1 = string.Empty;

            foreach (byte x in hash)
                hashString1 += string.Format("{0:x2}", x);

            //System.Windows.Clipboard.SetText(hashString1);

            return (hashString1.ToLower().Equals(b.ToLower()));
        }

        private void SetPdfPath(string pdfPath)
        {
            if (pdfPath.ToLower().Contains(this._formDir.ToLower()))
                throw new DeviationFormHandlerException("You cannot operate on a file that exists in the Processed Form Directery.");
            else
                this.PdfPath = pdfPath;
        }

        /// <summary>
        /// Gets or sets the file path of the pdf we're working on
        /// </summary>
        public string PdfPath
        {
	        set
	        {
		        if (value.ToLower().Contains(this._formDir.ToLower()))
		            throw new DeviationFormHandlerException("You cannot operate on a file that exists in the Processed Form Directery.");
		        else
		            this._pdfPath = value;
            }    
	        get { return _pdfPath; }
        }
	
	/// <summary>
        /// Gets or sets an IniParser object
        /// </summary>
        public IniParser Ini { get; set; }
        private List<FormEntry> CurrentForm { get; set; }
        private FormFieldTranslation[] fft { get; set; }
    }
}
