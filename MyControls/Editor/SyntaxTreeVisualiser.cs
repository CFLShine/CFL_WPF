using CFL_1.CFL_System.Compil;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Windows.Controls;
using System.Windows.Media;
using ShLayouts;

namespace MyControls.Editor
{
    public class SyntaxTreeVisualiser : VBoxLayout
    {
        public SyntaxTreeVisualiser()
        {
            init();
        }

        public void showSyntaxTree(SyntaxTree _syntaxTree)
        {
            __treeView.Items.Clear();

            SyntaxNode _root = _syntaxTree.GetRoot();
            TreeViewItem _rootItem = syntaxNodeItem(_root);
            append(_rootItem, _root);
        }

        private void append(TreeViewItem _rootItem, SyntaxNode _node)
        {
            if(__treeView.Items.Count == 0)
            {
                __treeView.Items.Add(_rootItem);
            }

            ChildSyntaxList _children = _node.ChildNodesAndTokens();
            ChildSyntaxList.Enumerator _enumerator = _children.GetEnumerator();
            while(_enumerator.MoveNext())
            {
                SyntaxNodeOrToken _syntaxElement = _enumerator.Current;
                if(_syntaxElement.IsNode)
                {
                    SyntaxNode _childNode = _syntaxElement.AsNode();
                    TreeViewItem _childNodeItem = syntaxNodeItem(_childNode);
                    _rootItem.Items.Add(_childNodeItem);
                    append(_childNodeItem, _childNode);
                }
                else
                if(_syntaxElement.IsToken)
                {
                    SyntaxToken _token = _syntaxElement.AsToken();
                    _rootItem.Items.Add(syntaxTokenItem(_token));
                }
            }
        }

        private TreeViewItem syntaxNodeItem(SyntaxNode _syntaxNode)
        {
            TreeViewItem _item = new TreeViewItem();
            string _header = SyntaxAnalyzeHelper.kindStr(_syntaxNode.Kind());
            _item.Header = _header;
            _item.Background = Brushes.Aquamarine;
            _item.IsExpanded = true;
            return _item;
        }

        private TreeViewItem syntaxTokenItem(SyntaxToken _token)
        {
            TreeViewItem _item = new TreeViewItem();
            _item.Header = SyntaxAnalyzeHelper.kindStr(_token.Kind()) + " " + SyntaxAnalyzeHelper.text(_token);
            _item.Background = Brushes.Beige;
            return _item;
        }

        private void init()
        {
            Add(__treeView);
        }

        private TreeView __treeView = new TreeView();
    }
}
