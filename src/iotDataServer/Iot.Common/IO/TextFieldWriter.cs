using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Iot.Common.IO
{
    public class TextFieldWriter : IDisposable
    {
        public string CommentTokens { get; set; }
        public string Delimiters { get; set; }
        public bool TrimWhiteSpace { get; private set; }

        private TextWriter _writer = null;

        protected void Init()
        {
            CommentTokens = "'";
            Delimiters = ",";
            TrimWhiteSpace = true;
        }

        public TextFieldWriter(string path, bool append = false, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            _writer = new StreamWriter(path, append, encoding);
            Init();
        }

        public TextFieldWriter(Stream stream, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            _writer = new StreamWriter(stream, encoding);
            Init();
        }

        public TextFieldWriter(TextWriter writer)
        {
            _writer = writer;
            Init();
        }

        public Encoding Encoding
        {
            get
            {
                return _writer.Encoding;
            }
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _writer = null;
            }
        }

        public void Close()
        {
            Flush();
            _writer.Close();
        }

        public void WriteFields(IEnumerable fields)
        {
            WriteFields(fields, false);
        }

        private static Regex _regexNumber = new Regex(@"^[0-9.]*$");
        public void WriteFields(IEnumerable fields, bool bFlush)
        {
            string writeLine = "";

            foreach (var field in fields)
            {
                string fieldString = field.ToString();
                bool hasSpecialChar = false;

                if (fieldString.IndexOf(CommentTokens) >= 0)
                {
                    hasSpecialChar = true;
                }
                else if (fieldString.IndexOf(Delimiters) >= 0)
                {
                    hasSpecialChar = true;
                }
                var match = _regexNumber.Match(fieldString);
                if (!match.Success)
                {
                    hasSpecialChar = true;
                }

                if (fieldString.IndexOf("\"") >= 0)
                {
                    fieldString = fieldString.Replace("\"", "\"\"");
                    hasSpecialChar = true;
                }

                if (writeLine != "")
                {
                    writeLine += ",";
                }

                if (hasSpecialChar)
                {
                    writeLine += string.Format("\"{0}\"", fieldString);
                }
                else
                {
                    writeLine += fieldString;
                }
            }

            _writer.WriteLine(writeLine);
            if (bFlush)
            {
                Flush();
            }
        }
    }
}