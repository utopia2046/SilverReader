using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SilverReaderApp
{
    using Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OverlayMessage overlayMessage;
        private TextSpeaker reader = new TextSpeaker();
        private Action<Paragraph> SetParagraphMargin = (p) => { p.Margin = new Thickness(Settings.Default.ParagraphMargin); };

        public MainWindow()
        {
            InitializeComponent();
            Trace.TraceInformation("Main windows initialized.");
            overlayMessage = new OverlayMessage(this.overlayText);

            try
            {
                //reader.GetAvailableVoiceNames();
                reader.SetVoice(Settings.Default.VoiceName);
                reader.Volumn = Settings.Default.VoiceVolumn;
                reader.Rate = Settings.Default.VoiceRate;
                Trace.TraceInformation("Voice initialized.");
                overlayMessage.ShowMessage("Current voice: " + Settings.Default.VoiceName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception while initializing speech engine. Detail: " + ex.Message);
                var voiceNames = reader.GetAvailableVoiceNames();
                Trace.TraceInformation("Available voice names:");
                foreach (var voice in voiceNames)
                {
                    Trace.TraceInformation(voice);
                }
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error when saving configuration. Detail: " + ex.Message);
            }
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.P:
                        if (reader.IsReading && !reader.IsPaused)
                        {
                            reader.Pause();
                            Trace.TraceInformation("Reader paused.");
                            overlayMessage.ShowMessage("Pause");
                        }
                        else if (reader.IsReading && reader.IsPaused)
                        {
                            reader.Resume();
                            Trace.TraceInformation("Reader resumed.");
                            overlayMessage.ShowMessage("Resume");
                        }
                        else
                        {
                            reader.Read(flowDocReader.Selection.Text);
                            Trace.TraceInformation("Reader started.");
                            overlayMessage.ShowMessage("Reading");
                        }
                        break;
                    case Key.S:
                        reader.Stop();
                        Trace.TraceInformation("Reader stopped.");
                        overlayMessage.ShowMessage("Stopped");
                        break;
                    case Key.V:
                        if (Clipboard.ContainsText() && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        {
                            string text = Clipboard.GetText(TextDataFormat.Text);
                            FlowDocHandler.UpdateFlowDocumentFromString(flowDoc, text, SetParagraphMargin);
                            Trace.TraceInformation("Document updated with clipboard text.");
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error when handling key event. Detail: " + ex.Message);
            }
        }

        private void flowDocReader_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files[0];
                Trace.TraceInformation("Drop file " + file);
                try
                {
                    string[] lines = FileHandler.ReadFile(file, Encoding.Default);
                    FlowDocHandler.UpdateFlowDocumentFromString(flowDoc, lines, SetParagraphMargin);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error when reading file {0}.", file);
                    Trace.TraceError("Detail: " + ex.Message);
                }
            }
        }

        private void flowDocReader_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void flowDocReader_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            flowDoc.Blocks.First().BringIntoView();
        }
    }
}
