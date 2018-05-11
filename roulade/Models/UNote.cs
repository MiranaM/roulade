using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using roulade.Model;
>>>>>>> 05153b0ab210e62f7eb25f76b291b577bac66d9b

namespace roulade.Models
{

    public class UNote
    {
        static public string[] Parameters =
        {
            "Length",
            "Lyric",
            "NoteNum",
            "Velocity",
            "Intensity",
            "Modulation",
            "Flags",
            "Envelope",
            "PBW",
            "PBS"
        };
        private Dictionary<string, dynamic> OtherParameters = new Dictionary<string, dynamic> { };
        private Dictionary<string, dynamic> AliasParameters = new Dictionary<string, dynamic> { };
        public int Length;
        public string Lyric;
        public int NoteNum;
        public int Velocity;
        public int Intensity;
        public int Modulation;
        public string Flags;
        public string Envelope;
        public string PBW;
        public string PBS;
        public string UNumber;
        public long AbsoluteTime;
        public int Volume = 80;
        public UOto UOto;

        private List<string> GotParameters = new List<string> { };

        public void Set(string parameter, dynamic value)
        {
            if (value is "") return;
            if (value is IEnumerable<string>)
            {
                value = String.Join(",", value);
            }

            Console.WriteLine($"\t{parameter}={value}");
            switch (parameter)
            {
                case "Length":
                case "NoteNum":
                case "Velocity":
                case "Intensity":
                case "Modulation":
                    this[parameter] = int.Parse(value);
                    break;
                case "Lyric":
                    if (value == "rr")
                    {
                        this[parameter] = "r";
                    }
                    else if (value == "R")
                    {
                        this[parameter] = "";
                    }
                    else this[parameter] = value;
                    break;
                case "Flags":
                case "PBW":
                case "PBS":
                case "Envelope":
                    this[parameter] = value;
                    break;
                default:
                    if (parameter[0] == '@') AliasParameters[parameter] = value;
                    else OtherParameters[parameter] = value;
                    break;
            }
            GotParameters.Add(parameter);
        }

        public string[] ToStrings()
        {
            if (GotParameters.Count == 0)
            {
                return new string[] { "" };
            }
            string[] text = new string[GotParameters.Count];
            for (int i = 0; i < GotParameters.Count; i++)
            {
                string parameter = GotParameters[i];
                string value;
                switch (parameter)
                {
                    case "Lyric":
                        if (this[parameter].ToString() == "r") value = "rr";
                        else value = this[parameter].ToString();
                        break;
                    default:
                        value = this[parameter].ToString();
                        break;
                }
                text[i] = $"{parameter}={value}";
            }
            return text;
        }

        // Для обращения через имя параметра
        public object this[string fieldName]
        {
            get
            {
                if (GotParameters.Contains(fieldName))
                {
                    if (UNote.Parameters.Contains(fieldName))
                    {
                        var field = this.GetType().GetField(fieldName);
                        return field.GetValue(this);
                    }
                    if (OtherParameters.ContainsKey(fieldName))
                    {
                        return OtherParameters[fieldName];
                    }
                    if (AliasParameters.ContainsKey(fieldName))
                    {
                        return AliasParameters[fieldName];
                    }
                }
                throw new KeyNotFoundException($"Parameter {fieldName} is not set");
            }
            set
            {
                if (UNote.Parameters.Contains(fieldName))
                {
                    var field = this.GetType().GetField(fieldName);
                    field.SetValue(this, value);
                    return;
                }
                if (fieldName[0] == '@')
                {
                    AliasParameters[fieldName] = value;
                }
                else
                {
                    OtherParameters[fieldName] = value;
                }
                if (!GotParameters.Contains(fieldName))
                {
                    GotParameters.Add(fieldName);
                }
                return;
            }
        }

        public bool IsSet(string parameter)
        {
            return GotParameters.Contains(parameter);
        }

        public UNote Copy()
        {
            // Copy only main parameters
            UNote NewNote = new UNote();
            List<string> NewParameters = new List<string> { };
            foreach(string parameter in GotParameters)
            {
                if (Parameters.Contains(parameter))
                {
                    NewNote[parameter] = this[parameter];
                    NewParameters.Add(parameter);
                }
                //else
                //{
                //    NewNote.SetDefault(parameter);
                //    NewParameters.Add(parameter);
                //}
            }
            NewNote.GotParameters = NewParameters;
            return NewNote;
        }

        public void SetDefault(string parameter)
        {
            switch (parameter)
            {
                case "Flags":
                    this[parameter] = "";
                    break;
                case "Envelope":
                    this[parameter] = new string[] { };
                    break;
                default:
                    Set(parameter, "");
                    break;
            }

        }

        public UNote CopyWhole()
        {
            // Copy all parameters
            UNote NewNote = new UNote();
            foreach(string parameter in GotParameters)
            {
                NewNote[parameter] = this[parameter];
            }
            NewNote.GotParameters = GotParameters;
            return NewNote;
        }

        public void SetLyric(string lyric)
        {
            Lyric = lyric;
        }

        public void SetDefaultNoteSettings()
        {
            // We will apply this to "r" note which we won't consider Rest
            this["Intensity"] = Ust.uDefaultNote.Intensity;
            this["Modulation"] = Ust.uDefaultNote.Modulation;
            this["Envelope"] = Ust.uDefaultNote.Envelope;
            this["PBS"] = Ust.uDefaultNote.PBS;
            this["PBW"] = Ust.uDefaultNote.PBW;
        }

        public void ResetAlias()
        {
            foreach (string parameter in AliasParameters.Keys)
            {
                GotParameters.Remove(parameter);
            }
            AliasParameters = new Dictionary<string, dynamic> { };
        }

        public void SendToResampler()
        {
    //        string request = string.Format(
    //            "{0} {1:D} {2} {3} {4:D} {5} {6} {7:D} {8:D} {9} {10}",
    //            NoteNum,
    //            Velocity,
    //            Flags,
    //            Oto.Offset,
    //            RequiredLength,
    //            Oto.Consonant,
    //            Oto.Cutoff,
    //            Volume,
    //            0,
				//Tempo,
    //            String.Join(",",PitchData));


            //RenderItem item = new RenderItem()
            //{
            //    // For resampler
            //    RawFile = rawfile,
            //    NoteNum = phoneme.Parent.NoteNum,
            //    Velocity = (int)phoneme.Parent.Expressions["velocity"].Data,
            //    Volume = (int)phoneme.Parent.Expressions["volume"].Data,
            //    StrFlags = phoneme.Parent.GetResamplerFlags(),
            //    PitchData = BuildPitchData(phoneme, part, project),
            //    RequiredLength = (int)requiredLength,
            //    Oto = phoneme.Oto,
            //    Tempo = project.BPM,

            //    // For connector
            //    SkipOver = phoneme.Oto.Preutter * strechRatio - phoneme.Preutter,
            //    PosMs = project.TickToMillisecond(part.PosTick + phoneme.Parent.PosTick + phoneme.PosTick) - phoneme.Preutter,
            //    DurMs = project.TickToMillisecond(phoneme.DurTick) + lengthAdjustment,
            //    Envelope = phoneme.Envelope.Points
            //};
        }

        public void SendToWavtool()
        {

        }
    }
}
