

using CFL_1.CFL_System.Compil;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Documents;
using Microsoft.CodeAnalysis;
using System.Windows;
using System.Collections.Generic;
using CFL_1.CFL_System.Compil_and_script;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace MyControls.Editor

{
    public class ctrl_codeEditor : RichTextBox
    {
        public ctrl_codeEditor()
        { init(); }

        public ctrl_codeEditor(Compiler _compiler)
        { 
            init(_compiler);
            init(); 
        }

        public void init(CodeCompiler _compiler)
        { 
            __compiler = _compiler ; 
        }

        public void init(CodeCompiler _compiler, SyntaxTreeVisualiser _treeVisualizer)
        {
            init(_compiler);
            __syntaxTreeVisualizer = _treeVisualizer;
        }

        public void init(CodeCompiler _compiler, SyntaxTreeVisualiser _treeVisualizer, ctrl_userCommunication _ctrl_userCommunication)
        {
            init(_compiler, _treeVisualizer);
            __ctrl_userCommunication = _ctrl_userCommunication;
        }

        public void completion(string _insert)
        {
            CaretPosition = CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            CaretPosition.InsertTextInRun(_insert);
            textChanged();
        }

        private void textChanged()
        {
            __compiler.compil(text);

            if(__syntaxTreeVisualizer != null)
                __syntaxTreeVisualizer.showSyntaxTree(__compiler.syntaxTree);

            hilight();

            compilationErrors();
            showErrors();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            textChanged();
            if(lastTyped() == '.')
                suggestMembers();
            else
            {
                if(__ctrl_userCommunication != null)
                    __ctrl_userCommunication.clearCompletions();
            }
        }

        #region highlight

        public void hilightFromList(List<Hilight> _hilights)
        {
            foreach(Hilight _h in _hilights)
            {
                TextRange _range = textRange(_h.pos, _h.length);
                switch (_h.type)
                {
                    case HilightType.COMMENT:
                        hilightRange(_range, Brushes.Gray);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.Gray); }); 
                        break;
                    case HilightType.KEYWORD:
                        hilightRange(_range, Brushes.Blue);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.Blue); }); 
                        break;
                    case HilightType.CLASSNAME:
                        hilightRange(_range, Brushes.Magenta);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.Magenta); }); 
                        break;
                    case HilightType.MEMBER:
                        hilightRange(_range, Brushes.DarkGreen);
                        Dispatcher.Invoke(() => { hilightRange(_range, Brushes.DarkGreen); }); 
                        break;
                    case HilightType.STRING:
                        hilightRange(_range, Brushes.DarkSalmon);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.DarkSalmon); }); 
                        break;
                    case HilightType.NUMBER:
                        hilightRange(_range, Brushes.DarkOliveGreen);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.DarkOliveGreen); }); 
                        break;
                    case HilightType.NOTHING:
                        hilightRange(_range, Brushes.Black);
                        //Dispatcher.Invoke(() => { hilightRange(_range, Brushes.Black); }); 
                        break;
                    default:
                        break;
                }
            }
        }

        private void clearBackGround()
        {
            highlightRangeBackGround(textRange(0, text.Length), Background);
        }
        
        private void hilight()
        {
            __hilighter = new Hilighter(__compiler, hilightFromList);
            __hilighter.highlight();

            //__hiLightTask = new Task( new Action(__hilighter.highlight));
            //__hiLightTask.Start();
        }

        private void highlightErrors()
        {
            foreach(Error _error in __errors)
                highlightRangeBackGround(textRange(_error.pos, _error.length), Brushes.Red);
        }

        private void hilightRange(TextRange _range, Brush _color)
        {
            _range.ApplyPropertyValue(TextElement.ForegroundProperty, _color);
        }

        private void highlightRangeBackGround(TextRange _range, Brush _color)
        {
            _range.ApplyPropertyValue(TextElement.BackgroundProperty, _color);
        }

        TextRange textRange(int _start, int _length)
        {
            if(_start + _length >= text.Length)
                _length = text.Length - _start - 1;
            TextPointer navigator = Document.ContentStart; 
            TextPointer _startPos  = GetTextPointerAtOffset(_start, navigator);
            TextPointer _endPos = GetTextPointerAtOffset(_start + _length, navigator);

            return new TextRange(_startPos, _endPos);
        }

        /// <summary>
        /// Gets the text pointer at the given character offset.
        /// Each line break will count as 2 chars.
        /// </summary>
        public TextPointer GetTextPointerAtOffset(int offset, TextPointer navigator)
        {
            
            int cnt = 0;
            
            while (navigator.CompareTo(Document.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        // TODO: Find out what to do here?
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }

        #endregion highlight

        #region analyze

        char lastTyped()
        {
            string _text =  CaretPosition.GetTextInRun(LogicalDirection.Backward);
            if(!string.IsNullOrEmpty(_text))
                return _text.Last();
            return new char();
        }

        int currentPosition
        {
            get { return new TextRange(Document.ContentStart, CaretPosition).Text.Length; }
        }

        private void suggestMembers()
        {
            if(__ctrl_userCommunication == null)
                return;

            List<string> _suggestions = new List<string>();

            SyntaxToken _token = __compiler.syntaxTree.GetRoot().FindToken(currentPosition);
            SyntaxNode _parent = _token.Parent;

            ExpressionSyntax _identifier = null;
            if(_parent is MemberAccessExpressionSyntax)
            {
                _identifier = _parent.ChildNodes().FirstOrDefault() as ExpressionSyntax;
                TypeInfo _semanticInfo = __compiler.semanticModel.GetTypeInfo(_identifier);
                ITypeSymbol _type = _semanticInfo.Type;
                foreach(var _symbol in _type.GetMembers())
                {
                    if(_symbol.CanBeReferencedByName
                    && _symbol.DeclaredAccessibility == Accessibility.Public
                    && !_symbol.IsStatic)
                        _suggestions.Add(_symbol.Name);
                }
            }
                
            if(_identifier == null)
            {
                _identifier = _parent as LiteralExpressionSyntax;
                if(_identifier == null)
                {
                    _identifier = _parent as ParenthesizedLambdaExpressionSyntax;
                    if(_identifier == null)
                    {
                        _identifier = _parent as InvocationExpressionSyntax;
                        if(_identifier == null)
                            _identifier = _parent as ObjectCreationExpressionSyntax;
                    }
                }
                if(_identifier == null)
                    return;
            }
            
            __ctrl_userCommunication.completions(_suggestions);
            

            /*
            IEnumerable<SyntaxNode> _nodes = __compiler.syntaxTree.GetRoot().DescendantNodes(_span);
            if(_nodes.Count() == 0)
                return;
            MemberAccessExpressionSyntax memberAccessNode = (MemberAccessExpressionSyntax)_nodes.Last();
            var _type = __compiler.semanticModel.GetTypeInfo(memberAccessNode.Expression).Type;

            

            foreach(var _symbol in _type.GetMembers())
            {
                if(_symbol.CanBeReferencedByName
                && _symbol.DeclaredAccessibility == Accessibility.Public
                && !_symbol.IsStatic)
                    _suggestions.Add(_symbol.Name);
            }
            
            */
        }

        #region errors

        private void compilationErrors()
        {
            __errors.Clear();
            errors(__compiler.compilation.GetDiagnostics());
        }

        private void errors(IEnumerable<Diagnostic> _diagnostics)
        {
            IEnumerator<Diagnostic> _it = _diagnostics.GetEnumerator();
            while(_it.MoveNext())
            {
                Diagnostic _d = _it.Current;
                FileLinePositionSpan _lineSpan = _d.Location.GetLineSpan();
                string _message = "Line " 
                                + _lineSpan.StartLinePosition.Line.ToString() 
                                ;
                _message += " erreur " + _d.Id + " " + _d.GetMessage();
                __errors.Add(new Error(_lineSpan.Span.Start.Character, 
                                       _lineSpan.Span.End.Character,
                                       _message));
            }
        }

        private void showErrors()
        {
            if(__ctrl_userCommunication == null)
                return;

            __ctrl_userCommunication.clearErrors();

            foreach(Error _error in __errors)
                __ctrl_userCommunication.error(_error.message) ;
        }

        List<Error> __errors = new List<Error>();

        struct Error
        {
            public Error(int _pos, int _length, string _message)
            {
                pos = _pos;
                length = _length;
                message = _message;
            }

            public int pos;
            public int length;
            public string message;
        }
        
        #endregion errors

        #endregion analyze

        public string text
        {
            get
            {
                TextRange textRange = new TextRange(Document.ContentStart,
                                                    Document.ContentEnd);
                return textRange.Text;
            }

            set 
            { 
               
                FlowDocument _flowDoc = new FlowDocument();

                string[] _paragraphs = value.Split('\n');

                foreach(string _paragraph in _paragraphs)
                {
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Run(_paragraph));
                    _flowDoc.Blocks.Add(para);
                }
                Document = _flowDoc;
                
                hilight();
            }
        }

        private void init()
        {
            AcceptsReturn = true;
            AcceptsTab = true;
            
            Background = Brushes.White;

            // évite que les lignes soient espacées (comportement par défaut du RichTextBox)
            Style noSpaceStyle = new Style(typeof(Paragraph));
            noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            Resources.Add(typeof(Paragraph), noSpaceStyle);

            
        }

        private CodeCompiler __compiler;
        private SyntaxTreeVisualiser __syntaxTreeVisualizer;
        private ctrl_userCommunication __ctrl_userCommunication;

        private Hilighter __hilighter;
#pragma warning disable CS0169 // Le champ 'ctrl_codeEditor.__hiLightTask' n'est jamais utilisé
        private Task __hiLightTask;
#pragma warning restore CS0169 // Le champ 'ctrl_codeEditor.__hiLightTask' n'est jamais utilisé
    }

    
}
