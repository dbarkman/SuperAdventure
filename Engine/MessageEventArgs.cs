using System;

namespace Engine
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public bool AddExtraNewLine { get; private set; }
        public bool ClearTextBox { get; private set; }

        public MessageEventArgs(string message, bool addExtraNewLine, bool clearTextBox)
        {
            Message = message;
            AddExtraNewLine = addExtraNewLine;
            ClearTextBox = clearTextBox;
        }
    }
}
