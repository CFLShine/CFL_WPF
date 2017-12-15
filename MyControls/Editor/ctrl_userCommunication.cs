
using CFL_1.CFLGraphics.PM_grid;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using ShLayouts;

namespace MyControls.Editor
{
    public class ctrl_userCommunication : HBoxLayout
    {
        public ctrl_userCommunication(ctrl_codeEditor _codeEditor)
        { 
            __codeEditor = _codeEditor;
            init() ; 
        }

        public void clearErrors()
        { __textBlockErrors.Text = "" ; }

        public void clearMessages()
        { __textBlocMessages.Text = "" ; }

        public void clearCompletions()
        {
            __gridCompletion.rowCount = 0;
            __gridCompletion.Visibility = Visibility.Hidden;
        }

        public void error(string _error)
        { __textBlockErrors.Text += _error + '\n' ; }

        public void message(string _message)
        { __textBlocMessages.Text += _message + '\n' ; }

        public void completions(List<string> _completions)
        { 
            showCompletions(_completions);
        }

        #region completion

        private void showCompletions(List<string> _completions)
        {
            __gridCompletion.rowCount = _completions.Count;
            __gridCompletion.rowCount = _completions.Count;
            for(int _i = 0 ; _i < _completions.Count ; _i++)
                __gridCompletion.value(0, _i, _completions[_i]);
            __gridCompletion.Visibility = Visibility.Visible;
        }

        private void gridCompletionClicked(int _column, int _row)
        { 
            __codeEditor.completion(__gridCompletion.value(_column, _row).ToString());
            __gridCompletion.rowCount = 0;
        }

        #endregion completion

        void init()
        {
            __itemErrors.Content = __textBlockErrors;
            __itemMessages.Content = __textBlocMessages;
            __tabControl.Items.Add(__itemErrors);
            __tabControl.Items.Add(__itemMessages);
            
            Add(__tabControl);

            __gridCompletion.setColumn_string(0, 100);
            __gridCompletion.cellClicked += gridCompletionClicked;
            Add(__gridCompletion);
        }

        private ctrl_codeEditor __codeEditor;

        private TabControl __tabControl = new TabControl();

        private TabItem __itemErrors = new TabItem() { Header = "Erreures" };
        private TabItem __itemMessages = new TabItem();

        private TextBlock __textBlockErrors = new TextBlock()
        { TextWrapping = TextWrapping.WrapWithOverflow };
        private TextBlock __textBlocMessages = new TextBlock()
        { TextWrapping = TextWrapping.WrapWithOverflow };

        PM_Grid __gridCompletion = new PM_Grid() { columnCount = 1 };
    }
}
