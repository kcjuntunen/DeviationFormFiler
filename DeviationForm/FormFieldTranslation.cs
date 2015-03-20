using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationForm
{
    class FormFieldTranslation
    {
        public FormFieldTranslation()
        {

        }

        public FormFieldTranslation(string ffn, string dfn, string dft)
        {
            this.FormFieldName = ffn;
            this.DataFieldName = dfn;
            this.DataFieldType = dft;
        }

        public string FormFieldName { get; set; }
        public string DataFieldName { get; set; }
        public string DataFieldType { get; set; }
    }
}
