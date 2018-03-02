using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SilverReaderApp
{
    class FlowDocHandler
    {
        private List<Paragraph> paragraphs = new List<Paragraph>();

        public static FlowDocument UpdateFlowDocumentFromString(FlowDocument doc, string text, Action<Paragraph> funcForParagraph = null)
        {
            string[] lineSeperator = new string[] { System.Environment.NewLine };
            string[] lines = text.Split(lineSeperator, StringSplitOptions.RemoveEmptyEntries);

            return UpdateFlowDocumentFromString(doc, lines, funcForParagraph);
        }

        public static FlowDocument UpdateFlowDocumentFromString(FlowDocument doc, string[] lines, Action<Paragraph> funcForParagraph = null)
        {
            doc.Blocks.Clear();
            foreach (var line in lines)
            {
                var p = new Paragraph(new Run(line));
                funcForParagraph?.Invoke(p);
                doc.Blocks.Add(p);
            }

            return doc;
        }
    }
}
