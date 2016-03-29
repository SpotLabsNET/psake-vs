namespace PSake.TaskRunner.Helpers.TaskRunner
{
    using System;
    using System.IO;

    internal class FileTextUtil : ITextUtil
    {
        private int _currentLineLength;
        private readonly string _filename;
        private int _lineNumber;

        public FileTextUtil(string filename)
        {
            this._filename = filename;
        }

        public Range CurrentLineRange => new Range { LineNumber = this._lineNumber, LineRange = new LineRange { Start = 0, Length = this._currentLineLength } };

        public bool Delete(Range range)
        {
            if (range.LineRange.Length == 0)
            {
                return true;
            }

            string fileContents = File.ReadAllText(this._filename);

            using (StringReader reader = new StringReader(fileContents))
            using (TextWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Create)))
            {
                int remainingCharacters = range.LineRange.Length;
                int currentStart = range.LineRange.Start;
                string lineText;
                while (remainingCharacters > 0 && this.SeekTo(reader, writer, range, out lineText))
                {
                    int trimFromLine = Math.Min(lineText.Length - currentStart, remainingCharacters);
                    writer.WriteLine(lineText.Substring(0, currentStart) + lineText.Substring(currentStart + trimFromLine));
                    remainingCharacters -= trimFromLine;
                    range.LineNumber = 0;
                    range.LineRange.Start = 0;
                    range.LineRange.Length = remainingCharacters;
                    currentStart = 0;
                }

                lineText = reader.ReadLine();

                while (lineText != null)
                {
                    writer.WriteLine(lineText);
                    lineText = reader.ReadLine();
                }
            }

            return true;
        }

        public bool Insert(Range range, string text, bool addNewline)
        {
            if (text.Length == 0)
            {
                return true;
            }

            string fileContents = File.ReadAllText(this._filename);

            using (StringReader reader = new StringReader(fileContents))
            using (TextWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Create)))
            {
                string lineText;
                if (this.SeekTo(reader, writer, range, out lineText))
                {
                    writer.WriteLine(lineText.Substring(0, range.LineRange.Start) + text + (addNewline ? Environment.NewLine : string.Empty) + lineText.Substring(range.LineRange.Start));
                }

                lineText = reader.ReadLine();

                while (lineText != null)
                {
                    writer.WriteLine(lineText);
                    lineText = reader.ReadLine();
                }
            }

            return true;
        }

        public bool TryReadLine(out string line)
        {
            line = null;
            Stream stream = File.OpenRead(this._filename);
            using (TextReader reader = new StreamReader(stream))
            {
                int lineCount = this._lineNumber;
                for (int i = 0; i < lineCount + 1; ++i)
                {
                    line = reader.ReadLine();
                }

                if (line != null)
                {
                    this._currentLineLength = line.Length;
                    ++this._lineNumber;
                    return true;
                }

                this._currentLineLength = 0;
                return false;
            }
        }

        public string ReadAllText()
        {
            return File.ReadAllText(this._filename).Replace("\r", "").Replace("\n", "");
        }

        public void Reset()
        {
            this._lineNumber = 0;
        }

        private bool SeekTo(StringReader reader, TextWriter writer, Range range, out string lineText)
        {
            bool success = true;

            for (int lineNumber = 0; lineNumber < range.LineNumber; ++lineNumber)
            {
                string line = reader.ReadLine();

                if (line != null)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    success = false;
                    break;
                }
            }

            lineText = reader.ReadLine();

            if (success)
            {
                if (lineText != null)
                {
                    if (lineText.Length < range.LineRange.Start)
                    {
                        success = false;
                        writer.WriteLine(lineText);
                    }
                }
            }

            return success;
        }

        public void FormatRange(LineRange range)
        {
        }
    }
}
