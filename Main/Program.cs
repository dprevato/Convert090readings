using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Main
{
    class Program
    {
        public static List<Reading> InputCollection { get; set; } = new List<Reading>();
        public static List<Reading> OutputCollection { get; set; } = new List<Reading>();
        public static List<Reading> RejectedCollection { get; set; } = new List<Reading>();

        static void Main(string[] args)
        {
            InputCollection = JsonConvert.DeserializeObject<List<Reading>>(File.ReadAllText(@"Reading.json"), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Parse();
            File.WriteAllText(@"rejected.json", JsonConvert.SerializeObject(RejectedCollection));
            File.WriteAllText(@"newitems.json", JsonConvert.SerializeObject(OutputCollection));
        }

        public static void Parse()
        {

            foreach (var item in InputCollection)
            {
                var cmt = item.Comment;
                // cmt deve contenere un numero dispari di caratteri pipe '|'. Se i caratteri sono pari, è perché ne manca uno, presumibilmente tra il valore dell'invaso e la label successiva,
                // che potrebbe essere "Apertura cm" oppure "Chiusura cm". Generalizzando, devo trovare un numero pari di pipe, e un punto dove un carattere numerico è immediatamente
                // seguito da un carattere alfabetico (A o C)
                if (!cmt.StartsWith("Invaso"))
                {
                    RejectedCollection.Add(item);
                    continue;
                }

                if (cmt.StartsWith(@"|"))
                {
                    cmt = cmt.Length > 1 ? cmt[1..] : cmt; // elimino il carattere iniziale
                }

                while (cmt.EndsWith("|"))
                {
                    cmt = cmt.Chop();  // elimino il carattere finale
                }

                var pipes = Regex.Matches(cmt, @"\|");
                if (pipes.Count % 2 == 0)
                {
                    // I pipes sono pari. Devo trovare un punto in cui immediatamente dopo un carattere numerico si trova un carattere alfabetico.
                    // Ce ne deve essere soltanto uno.
                    cmt = Regex.Replace(cmt, @"(.*\d{2})([a-zA-z]{2}.*)", @"$1|$2");
                }

                var splittedComment = cmt.Split("|", StringSplitOptions.None);
                // A questo punto, splittedComment contiene una serie di etichette alternate ai valori corrispondenti. La prima ad esempio, contiene "Invaso", la seconda contiene l'invaso del
                // giorno e ora della misura, ecc. Da un singolo record in input, devo creare una serie di record in output (fino a 8) normalizzati.

                // Se rValue è diverso da -999, significa che devo creare un'istanza della classe Reading per ogni gruppo di due posizioni nel vettore splittedComment. Se rValue contiene
                // un valore validabile, devo creare un'istanza Reading per contenere il valore, che si riferisce ad un tempo di apertura/chiusura dell'organo idraulico.

                for (var idx = 0; idx < splittedComment.Length / 2 * 2; idx += 2)
                {
                    CreateItem(item, splittedComment[idx], splittedComment[idx+1]);
                }

            }

        }

        public static void CreateItem(Reading item, string label, string value)
        {
            // Primo: modificare il record originale, che contiene un tempo di apertura o di chiusura - lo devo fare sempre, per ogni item
            Reading record;
            if (item.PdmId.LastChar() == "0" && item.rValue != -999)
            {
                // Tempo di apertura
                record = new Reading
                {
                    PdmId = item.PdmId.Chop() + "3",
                    ReadingDateTime = item.ReadingDateTime,
                    rValue = item.rValue,
                    taken = item.taken,
                    valid = item.valid,
                    ValidationDate = item.ValidationDate
                };
                OutputCollection.Add(record);
            }

            // Secondo: se il record originale contiene una chiusura, devo modificarlo
            if (item.PdmId.LastChar() == "1" && item.rValue != -999)
            {
                // Tempo di chiusura
                record = new Reading
                {
                    PdmId = item.PdmId.Chop() + "4",
                    ReadingDateTime = item.ReadingDateTime,
                    rValue = item.rValue,
                    taken = item.taken,
                    valid = item.valid,
                    ValidationDate = item.ValidationDate
                };
                OutputCollection.Add(record);
            }

            // Terzo: creo un nuovo record per salvare i dati label e value
            string channel;
            switch (label)
            {
                case "Invaso":
                    channel = "0";
                    break;
                case "Apertura cm":
                    channel = "1";
                    break;
                case "Chiusura cm":
                    channel = "2";
                    break;
                case "Pos. Iniziale":
                    channel = "5";
                    break;
                case "Pompa Ap":
                    channel = "6";
                    break;
                case "Pompa Ch":
                    channel = "7";
                    break;
                default:
                    channel = "X";
                    break;
            }

            if (channel == "X")
            {
                // questo è un caso speciale, vanno aggiunti due record, uno per l'apertura e uno per la chiusura
                record = new Reading
                {
                    PdmId = item.PdmId.Chop() + "6",
                    ReadingDateTime = item.ReadingDateTime,
                    Comment = value,
                    taken = item.taken,
                    valid = item.valid,
                    ValidationDate = item.ValidationDate
                };
                OutputCollection.Add(record);

                record = new Reading
                {
                    PdmId = item.PdmId.Chop() + "7",
                    ReadingDateTime = item.ReadingDateTime,
                    Comment = value,
                    taken = item.taken,
                    valid = item.valid,
                    ValidationDate = item.ValidationDate
                };
                OutputCollection.Add(record);
            }
            else
            {
                record = new Reading
                {
                    PdmId = item.PdmId.Chop() + channel,
                    ReadingDateTime = item.ReadingDateTime,
                    Comment = value,
                    taken = item.taken,
                    valid = item.valid,
                    ValidationDate = item.ValidationDate
                };
                OutputCollection.Add(record);
            }
        }
    }
}
