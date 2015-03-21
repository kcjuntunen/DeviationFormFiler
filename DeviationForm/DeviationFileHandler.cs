using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeviationForm
{
    class DeviationFileHandler
    {
        /// <summary>
        /// Takes a target dir, and a target file to operate on.
        /// </summary>
        /// <param name="formDir">The directory in which the processed file will end up.</param>
        /// <param name="origPdf">The file we're going to move.</param>
        public DeviationFileHandler(string formDir, string origPdf)
        {
            this.FormDirectory = formDir + @"\";
            this.OriginalPdf = origPdf;
        }

        /// <summary>
        /// Why is this here? I can't remember.
        /// </summary>
        public string FilePdf()
        {
            return this.MoveForm();
        }

        /// <summary>
        /// Populates vars only once. Just returns the rest of the time.
        /// </summary>
        /// <returns>A string of the full path of the new file.</returns>
        public string GetNextFileName()
        {
            string result = string.Empty;
            
            // This block will run only once and populate this.NewFileName.
            if (this.NewFileName == null)
            {
                DirectoryInfo di = new DirectoryInfo(FormDirectory);
                IEnumerable<FileInfo> fiIE;
                try
                {
                    // It would be better to run .EnumerateFiles with a regex (this would find 'aj3rk.pdf' and try to parse it).
                    // I don't know how to do that (yet?).
                    fiIE = di.EnumerateFiles("?????.pdf", SearchOption.TopDirectoryOnly);
                }
                catch (ArgumentNullException anEx) { throw new DeviationFileHandlerException(@"searchPattern is null.", anEx); }
                catch (ArgumentOutOfRangeException aoorEx) { throw new DeviationFileHandlerException(@"searchOption is not a valid SearchOption value.", aoorEx); }
                catch (DirectoryNotFoundException dnfEx) { throw new DeviationFileHandlerException(@"This directory doesn't exist. Check 'C:\Optimize\Import\dev_req.ini'", dnfEx); }
                catch (System.Security.SecurityException sEx) { throw new DeviationFileHandlerException(@"You don't have permission to access '" + this.FormDirectory + "'.", sEx); }

                List<FileInfo> fil = new List<FileInfo>();
                int nextName = 0;
                bool bParseResult = false;
                
                // Are these alphabetical? I hope so.
                foreach (FileInfo item in fiIE)
                    fil.Add(item);

                //System.Windows.MessageBox.Show(fil.Count.ToString());
                int i = fil.Count;
                if (fil.Count > 0)
                    while (!bParseResult && i > 0)
                        bParseResult = int.TryParse(Path.GetFileNameWithoutExtension(fil[--i].Name), out nextName);

                if (bParseResult)
                    result = string.Format("{0:00000}.pdf", nextName + 1);
                else
                    result = "00000.pdf"; // Couldn't find any files matching our pattern. May as well start at the beginning.

                this.NewFileName = result;
            }

            return this.FormDirectory + this.NewFileName;
        }
        
        /// <summary>
        /// 'Nuff said, I suppose.
        /// </summary>
        /// <returns>A string of the full path of the new file.</returns>
        public string MoveForm()
        {
            string FullPathName = this.GetNextFileName();
            //System.Windows.MessageBox.Show(FullPathName);

            if (!File.Exists(FullPathName))
            {
                try { File.Move(this.OriginalPdf, FullPathName); } // Let's give it a go.
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
            {
                throw new DeviationFileHandlerException(string.Format("'{0}' exists. I'm screwing something up somehow." +
                    "Or, probably, the '{1}' has files that don't belong there.", FullPathName, this.FormDirectory));
            }

            return FullPathName;
        }

        /// <summary>
        /// Gets or sets the new file name. Hmm... I should probably only allow getting.
        /// </summary>
        public string NewFileName { get; set; }
        
        /// <summary>
        /// Gets or sets our target directory.
        /// </summary>
        public string FormDirectory { get; set; }
        
        /// <summary>
        /// Gets or sets the full path of the pdf we're working on.
        /// </summary>
        public string OriginalPdf { get; set; }
    }
}
