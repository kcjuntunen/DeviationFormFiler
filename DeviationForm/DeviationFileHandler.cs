using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeviationForm
{
    class DeviationFileHandler
    {
        public DeviationFileHandler(string formDir, string origPdf)
        {
            this.FormDirectory = formDir + @"\";
            this.OriginalPdf = origPdf;
        }

        public string FilePdf()
        {
            return this.MoveForm();
        }

        public string GetNextFileName()
        {
            string result = string.Empty;

            if (this.NewFileName == null)
            {
                DirectoryInfo di = new DirectoryInfo(FormDirectory);
                IEnumerable<FileInfo> fiIE;
                try
                {
                    fiIE = di.EnumerateFiles("?????.pdf", SearchOption.TopDirectoryOnly);
                }
                catch (ArgumentNullException anEx) { throw new DeviationFileHandlerException(@"searchPattern is null.", anEx); }
                catch (ArgumentOutOfRangeException aoorEx) { throw new DeviationFileHandlerException(@"searchOption is not a valid SearchOption value.", aoorEx); }
                catch (DirectoryNotFoundException dnfEx) { throw new DeviationFileHandlerException(@"This directory doesn't exist. Check 'C:\Optimize\Import\dev_req.ini'", dnfEx); }
                catch (System.Security.SecurityException sEx) { throw new DeviationFileHandlerException(@"You don't have permission to access '" + this.FormDirectory + "'.", sEx); }

                List<FileInfo> fil = new List<FileInfo>();
                int nextName = 0;
                bool bParseResult = false;

                foreach (FileInfo item in fiIE)
                    fil.Add(item);

                //fil.Sort();

                //System.Windows.MessageBox.Show(fil.Count.ToString());
                int i = fil.Count;
                if (fil.Count > 0)
                    while (!bParseResult && i > 0)
                        bParseResult = int.TryParse(Path.GetFileNameWithoutExtension(fil[--i].Name), out nextName);

                if (bParseResult)
                    result = string.Format("{0:00000}.pdf", nextName + 1);
                else
                    result = "00000.pdf";

                this.NewFileName = result;
            }

            return this.FormDirectory + this.NewFileName;
        }

        public string MoveForm()
        {
            string FullPathName = this.FormDirectory + this.NewFileName;
            //System.Windows.MessageBox.Show(FullPathName);

            if (!File.Exists(FullPathName))
            {
                try { File.Move(this.OriginalPdf, FullPathName); }
                catch (ArgumentNullException anEx) { throw new DeviationFileHandlerException(string.Format("'FullPathName' or 'this.OriginalPdf' is null."), anEx); }
                catch (ArgumentException aEx)
                {
                    char[] invalid = Path.GetInvalidPathChars();
                    string msg = "The path name cannot be zero-length, nor contain any of these:\n";
                    foreach (char ch in invalid)
                        msg += ch.ToString() + " ";
                    throw new DeviationFileHandlerException(msg, aEx);
                }
                catch (UnauthorizedAccessException sEx) { throw new DeviationFileHandlerException(@"You don't have permission to access '" + this.FormDirectory + "'.", sEx); }
                catch (PathTooLongException ptlEx) { throw new DeviationFileHandlerException(@"The path is more than 247 characters.", ptlEx); }
                catch (DirectoryNotFoundException dnfEx) { throw new DeviationFileHandlerException(@"This directory doesn't exist. Check 'C:\Optimize\Import\dev_req.ini'", dnfEx); }
            }
            else
                throw new DeviationFileHandlerException(string.Format("'{0}' exists. I'm screwing something up somehow.", FullPathName));

            return FullPathName;
        }

        public string NewFileName { get; set; }
        public string FormDirectory { get; set; }
        public string OriginalPdf { get; set; }
    }
}
