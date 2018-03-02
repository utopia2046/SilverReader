using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;

namespace SilverReaderApp
{
    class TextSpeaker
    {
        private Prompt activePrompt { get; set; }
        private SpeechSynthesizer reader { get; set; }

        public bool IsReading { get; set; }
        public bool IsPaused { get; set; }
        private int rate;  // -10 ~ 10
        public int Rate
        {
            get
            {
                return rate;
            }
            set
            {
                if (value >= -10 && value <= 10)
                {
                    reader.Rate = value;
                }
            }
        }

        private int volumn; // 0 ~ 100
        public int Volumn
        {
            get
            {
                return volumn;
            }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    reader.Volume = value;
                }
            }
        }

        public TextSpeaker()
        {
            reader = new SpeechSynthesizer();
            IsReading = false;
            IsPaused = false;
            rate = 0;
            volumn = 50;
        }

        public string[] GetAvailableVoiceNames()
        {
            List<string> names = new List<string>();

            foreach (var voice in reader.GetInstalledVoices())
            {
                if (voice.Enabled)
                {
                    names.Add(voice.VoiceInfo.Name);
                }
            }

            return names.ToArray();
        }

        public void SetVoice(string voiceName)
        {
            reader.SelectVoice(voiceName);
            reader.SetOutputToDefaultAudioDevice();
        }

        public void SetVoiceByHint(string hint)
        {
            reader.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, System.Globalization.CultureInfo.CurrentCulture);
        }

        public void Read(string message)
        {
            IsReading = true;
            // Get the prompt that is being read out right now; for stopping it later.
            activePrompt = reader.SpeakAsync(message);

            // Attach the handler
            reader.SpeakCompleted += (sander, ev) =>
            {
                IsReading = false;
            };
        }

        public void Stop()
        {
            if (IsReading)
            {
                reader.SpeakAsyncCancel(activePrompt);
                IsReading = false;
                IsPaused = false;
            }
        }

        public void Pause()
        {
            if (IsReading && !IsPaused)
            {
                reader.Pause();
                IsPaused = true;
            }
        }

        public void Resume()
        {
            if (IsReading && IsPaused)
            {
                reader.Resume();
                IsPaused = false;
            }
        }

        public void SaveToFile(string message, string fileName)
        {
            // Store the audio
            if (!IsReading)
            {
                reader.SetOutputToWaveFile(fileName);
                reader.SpeakAsync(message);
            }
        }
    }
}
