using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Iot.Common.IO
{
    enum TextFieldReaderParingMode
    {
        BeforeData,
        StartedData,
        InQuotationMark,
        AfterQuotationMark
    }

    public class TextFieldReader : IDisposable
    {
        public string CommentTokens { get; set; }
        public string Delimiters { get; set; }
        public bool EndOfData { get; private set; }
        public string ErrorLine { get; private set; }
        public int ErrorLineNumber { get; private set; }
        public int LineNumber { get; private set; }
        public bool TrimWhiteSpace { get; private set; }

        private TextReader _reader = null;

        protected  void Init()
        {
            CommentTokens = "";
            Delimiters = ",";

            EndOfData = false;
            ErrorLine = "";
            ErrorLineNumber = 0;
            LineNumber = 0;
            TrimWhiteSpace = true;
        }

        public TextFieldReader(string path, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            _reader = new StreamReader(path, encoding);
            Init();
        }

        public TextFieldReader(Stream stream, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            _reader = new StreamReader(stream, encoding);
            Init();
        }

        public TextFieldReader(TextReader reader)
        {
            _reader = reader;
            Init();
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
                _reader = null;
            }
        }

        public void Close()
        {
            _reader.Close();
        }

        public string[] ReadFields()
        {
            List<string> fieldList = new List<string>();
            string currData = null;
            string readLine = null;
            char[] delimiterChars = Delimiters.ToCharArray();

            // Skip comments line
            while ((readLine = _reader.ReadLine()) != null)
            {
                LineNumber++;
                if (!string.IsNullOrEmpty(CommentTokens) && (readLine.StartsWith(CommentTokens)))
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(readLine.Trim()))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (readLine == null)
            {
                return null;
            }

            currData = "";
            TextFieldReaderParingMode parsingMode = TextFieldReaderParingMode.BeforeData;
            bool parsingDone = false;
            do
            {
                int charIndex = 0;
                char currChar = '\0';
                while (charIndex < readLine.Length)
                {
                    currChar = readLine[charIndex];
                    switch (parsingMode)
                    {
                        case TextFieldReaderParingMode.BeforeData:
                            if (currChar == '"')
                            {
                                parsingMode = TextFieldReaderParingMode.InQuotationMark;
                            }
                            else if (Delimiters.IndexOf(currChar) >= 0)
                            {
                                fieldList.Add(currData);
                                currData = "";
                                parsingMode = TextFieldReaderParingMode.BeforeData;
                            }
                            else if (Char.IsWhiteSpace(currChar))
                            {
                                currData += currChar;
                            }
                            else
                            {
                                currData += currChar;
                                parsingMode = TextFieldReaderParingMode.StartedData;
                            }
                            break;

                        case TextFieldReaderParingMode.StartedData:
                            if (currChar == '"')
                            {
                                ErrorLineNumber = LineNumber;
                                ErrorLine = readLine;
                                throw new ApplicationException(string.Format("'{0}' is wrong.", currChar));
                            }
                            
                            if (Delimiters.IndexOf(currChar) >= 0)
                            {
                                fieldList.Add(currData);
                                currData = "";
                                parsingMode = TextFieldReaderParingMode.BeforeData;
                            }
                            else
                            {
                                currData += currChar;
                            }
                            break;

                        case TextFieldReaderParingMode.InQuotationMark:
                            if (currChar == '"')
                            {
                                if (readLine.Length > (charIndex + 1))
                                {
                                    char nextChar = readLine[charIndex + 1];
                                    if (nextChar == '"')
                                    {
                                        currData += '"';
                                        charIndex++;
                                    }
                                    else
                                    {
                                        fieldList.Add(currData);
                                        currData = "";
                                        parsingMode = TextFieldReaderParingMode.AfterQuotationMark;
                                    }
                                }
                                else
                                {
                                    fieldList.Add(currData);
                                    currData = "";
                                    parsingMode = TextFieldReaderParingMode.AfterQuotationMark;
                                }
                            }
                            else
                            {
                                currData += currChar;
                            }
                            break;

                        case TextFieldReaderParingMode.AfterQuotationMark:
                            if (Delimiters.IndexOf(currChar) >= 0)
                            {
                                parsingMode = TextFieldReaderParingMode.BeforeData;
                            }
                            else if (!Char.IsWhiteSpace(currChar))
                            {
                                ErrorLineNumber = LineNumber;
                                ErrorLine = readLine;
                                throw new ApplicationException(string.Format("'{0}' is wrong.", currChar));
                            }
                            break;

                        default:
                            throw new NotSupportedException(parsingMode.ToString());
                    }
                    charIndex++;
                }

                switch (parsingMode)
                {
                    case TextFieldReaderParingMode.BeforeData:
                    case TextFieldReaderParingMode.StartedData:
                        fieldList.Add(currData);
                        currData = "";
                        parsingDone = true;
                        break;

                    case TextFieldReaderParingMode.AfterQuotationMark:
                        currData = "";
                        parsingDone = true;
                        break;
                }

                if (parsingDone)
                {
                    break;
                }
                else
                {
                    currData += "\r\n";
                    LineNumber++;
                }

            } while ((readLine = _reader.ReadLine()) != null);

            return fieldList.ToArray();
        }
    }

}