using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace SilverReaderApp
{
    class FlowDocHandler
    {
        private List<Paragraph> paragraphs = new List<Paragraph>();
        private FlowDocument doc;

        public FlowDocHandler(FlowDocument doc)
        {
            this.doc = doc;
        }

        public void UpdateFlowDocumentFromString(string text, Action<Paragraph> funcForParagraph = null)
        {
            string[] lineSeperator = new string[] { System.Environment.NewLine };
            string[] lines = text.Split(lineSeperator, StringSplitOptions.RemoveEmptyEntries);

            UpdateFlowDocumentFromString(lines, funcForParagraph);
        }

        public void UpdateFlowDocumentFromString(string[] lines, Action<Paragraph> funcForParagraph = null)
        {
            doc.Blocks.Clear();
            foreach (var line in lines)
            {
                var p = new Paragraph(new Run(line));
                funcForParagraph?.Invoke(p);
                doc.Blocks.Add(p);
            }
        }

        private void ScrollToTop(FlowDocument doc)
        {
            doc.Blocks.FirstBlock.BringIntoView();
            //var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            //object operation = null;
            //
            //range.Changed += (obj, e) =>
            //{
            //    if (operation == null) {
            //        operation = Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            //        {
            //            operation = null;
            //
            //            var scrollViewer = FindFirstVisualDescendantOfType<ScrollViewer>(doc);
            //            scrollViewer.ScrollToBottom();
            //        });
            //    }
            //};
        }
    }
}
