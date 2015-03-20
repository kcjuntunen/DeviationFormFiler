using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationForm
{
    class FormEntry
    {
        public FormEntry(string Key, string Value, FormFieldTranslation formFieldTranslation)
        {
            this.Key = Key;
            this.Value = Value;
            this.FormField = formFieldTranslation;
        }

        private string BoolParse(string b)
        {
            switch (b.ToLower())
            {
                case "yes":
                    return "True";
                case "no":
                    return "False";
                case "on":
                    return "True";
                case "off":
                    return "False";
                case "true":
                    return "True";
                case "false":
                    return "False";
                default:
                    return "False";
            }
        }

        public string Key { get; set; }

        private string _value;
        public string Value
        {
            get
            {
                switch (this.FormField.DataFieldType)
                {
                    case "Boolean":
                        if (this._value != string.Empty)
                            return this.BoolParse(this._value);
                        else
                            return "False";
                    case "Text":
                        if (this._value != string.Empty)
                            return string.Format("'{0}'", this._value.Replace("'", "''"));
                        else
                            return string.Format("Null");
                    case "Number":
                        if (this._value != string.Empty)
                            return string.Format("{0}", this._value.Replace("'", "''"));
                        else
                            return string.Format("Null");
                    case "Date":
                        if (this._value != string.Empty)
                            return string.Format("'{0}'", this._value.Replace("'", "''"));
                        else
                            return string.Format("Null");
                    default:
                        if (this._value != string.Empty)
                            return string.Format("'{0}'", this._value.Replace("'", "''"));
                        else
                            return string.Format("Null");
                }
            }
            set
            {
                this._value = value;
            }
        }
        public FormFieldTranslation FormField { get; set; }

        //public string FormFieldName {
        //    get
        //    { return FormField.FormFieldName; }

        //    set
        //    { }
        //}
        //public string DataFieldName
        //{
        //    get
        //    { return FormField.DataFieldName; }

        //    set
        //    { }
        //}
        //public string DataFieldType
        //{
        //    get
        //    { return FormField.DataFieldType; }

        //    set
        //    { }
        //}
    }
}
