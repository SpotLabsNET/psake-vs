namespace PSake.TaskRunner.Helpers.TaskRunner
{
    using System;
    using System.Text;

    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;

    internal class VsTextViewTextUtil : ITextUtil
    {
        private int _currentLineLength;
        private int _lineNumber;
        private IVsTextView _view;

        public VsTextViewTextUtil(IVsTextView view)
        {
            this._view = view;
        }

        public Range CurrentLineRange
        {
            get { return new Range { LineNumber = this._lineNumber, LineRange = new LineRange { Start = 0, Length = this._currentLineLength } }; }
        }

        public bool Delete(Range range)
        {
            try
            {
                this.GetEditPointForRange(range)?.Delete(range.LineRange.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Insert(Range position, string text, bool addNewline)
        {
            try
            {
                this.GetEditPointForRange(position)?.Insert(text + (addNewline ? Environment.NewLine : string.Empty));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryReadLine(out string line)
        {
            IVsTextLines textLines;
            int hr = this._view.GetBuffer(out textLines);

            if (hr != VSConstants.S_OK || textLines == null)
            {
                line = null;
                return false;
            }

            int lineCount;
            hr = textLines.GetLineCount(out lineCount);

            if (hr != VSConstants.S_OK || this._lineNumber == lineCount)
            {
                line = null;
                return false;
            }

            int lineNumber = this._lineNumber++;
            hr = textLines.GetLengthOfLine(lineNumber, out this._currentLineLength);

            if(hr != VSConstants.S_OK)
            {
                line = null;
                return false;
            }

            hr = textLines.GetLineText(lineNumber, 0, lineNumber, this._currentLineLength, out line);

            if (hr != VSConstants.S_OK)
            {
                line = null;
                return false;
            }

            LINEDATA[] lineData = new LINEDATA[1];
            textLines.GetLineData(lineNumber, lineData, null);
            if (lineData[0].iEolType != EOLTYPE.eolNONE)
            {
                line += "\n";
            }

            return true;
        }

        public string ReadAllText()
        {
            StringBuilder text = new StringBuilder();
            string line;
            while (this.TryReadLine(out line))
            {
                text.Append(line);
            }
            return text.ToString();
        }

        public void Reset()
        {
            this._currentLineLength = 0;
            this._lineNumber = 0;
        }

        private EditPoint GetEditPointForRange(Range range)
        {
            IVsTextLines textLines;
            int hr = this._view.GetBuffer(out textLines);

            if (hr != VSConstants.S_OK || textLines == null)
            {
                return null;
            }

            object editPointObject;
            hr = textLines.CreateEditPoint(range.LineNumber, range.LineRange.Start, out editPointObject);
            EditPoint editPoint = editPointObject as EditPoint;

            if (hr != VSConstants.S_OK || editPoint == null)
            {
                return null;
            }

            return editPoint;
        }

        public void FormatRange(LineRange range)
        {
            this.Reset();
            int startLine, startLineOffset, endLine, endLineOffset;
            this.GetExtentInfo(range.Start, range.Length, out startLine, out startLineOffset, out endLine, out endLineOffset);

            int oldStartLine, oldStartLineOffset, oldEndLine, oldEndLineOffset;
            this._view.GetSelection(out oldStartLine, out oldStartLineOffset, out oldEndLine, out oldEndLineOffset);
            this._view.SetSelection(startLine, startLineOffset, endLine, endLineOffset);
            IOleCommandTarget target = (IOleCommandTarget) ServiceProvider.GlobalProvider.GetService(typeof (SUIHostCommandDispatcher));
            Guid cmdid = VSConstants.VSStd2K;
            int hr = this._view.SendExplicitFocus();
            hr = target.Exec(ref cmdid, (uint) VSConstants.VSStd2KCmdID.FORMATSELECTION, 0, IntPtr.Zero, IntPtr.Zero);
            this._view.SetSelection(oldStartLine, oldStartLineOffset, oldEndLine, oldEndLineOffset);
        }
    }
}
