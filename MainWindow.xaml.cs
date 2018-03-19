using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SilverReaderApp
{
    using Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OverlayMessage overlayMessage;
        private FlowDocHandler docHandler;
        private TextSpeaker reader = new TextSpeaker();
        private Action<Paragraph> SetParagraphMargin = (p) => { p.Margin = new Thickness(Settings.Default.ParagraphMargin); };
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
            Trace.TraceInformation("Main windows initialized.");
            overlayMessage = new OverlayMessage(this.overlayText, this.roundBorder);
            docHandler = new FlowDocHandler(this.flowDoc);

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

        private void GoToStart()
        {
            try
            {
                NavigationCommands.FirstPage.Execute(null, this.flowDoc);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error when navigating to start. Detail: " + ex.Message);
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
                    case Key.P:  // Play or Pause selection text
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
                    case Key.S:  // Stop reading
                        reader.Stop();
                        Trace.TraceInformation("Reader stopped.");
                        overlayMessage.ShowMessage("Stopped");
                        break;
                    case Key.V:  // Replace document content with clipboard text
                        if (Clipboard.ContainsText() && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        {
                            string text = Clipboard.GetText(TextDataFormat.Text);
                            docHandler.UpdateFlowDocumentFromString(text, SetParagraphMargin);
                            GoToStart();
                            Trace.TraceInformation("Document updated with clipboard text.");
                        }
                        break;
                    case Key.U:  // Reload file using utf-8
                        LoadFile(fileName, Encoding.UTF8);
                        Trace.TraceInformation("Reload document using UTF-8.");
                        break;
                    case Key.L:  // Reload file using code page number specified in config file
                        LoadFile(fileName, Encoding.GetEncoding(Settings.Default.CodePage));
                        Trace.TraceInformation("Reload document using code page.");
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
                fileName = files[0];
                Trace.TraceInformation("Drop file " + fileName);
                LoadFile(fileName, Encoding.Default);
            }
        }

        private void LoadFile(string file, Encoding encoding)
        {
            try
            {
                string[] lines = FileHandler.ReadFile(file, encoding);
                docHandler.UpdateFlowDocumentFromString(lines, SetParagraphMargin);
                GoToStart();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error when reading file {0}.", file);
                Trace.TraceError("Detail: " + ex.Message);
            }
        }

        private void flowDocReader_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
