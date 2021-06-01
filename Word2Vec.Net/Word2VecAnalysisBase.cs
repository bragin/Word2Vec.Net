using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Word2Vec.Net
{
    public class Word2VecAnalysisBase
    {
        public const long max_size = 2000;         // max length of strings
        public const long N = 40;                  // number of closest words that will be shown
        public const long max_w = 50;              // max length of vocabulary entries
        private long size;
        private string file_name;
        private long words;
        private char[] vocab;
        private float[] m;
        protected char[] Vocab
        {
            get
            {
                return vocab;
            }

            set
            {
                vocab = value;
            }
        }

        protected long Words
        {
            get
            {
                return words;
            }

            set
            {
                words = value;
            }
        }

        protected long Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        protected float[] M
        {
            get
            {
                return m;
            }

            set
            {
                m = value;
            }
        }

        /// <summary>
        /// Basic class for analysis algorithms( distnace, analogy, commpute-accuracy)
        /// </summary>
        /// <param name="fileName"></param>
        public Word2VecAnalysisBase(string fileName)
        {
            file_name = fileName;           //bestw = new string[N];



            InitVocub();
        }

        private void InitVocub()
        {
            // file_name is an output file, not vocab
            using (var fs = File.Open(file_name, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs))
            {
                var header = reader.ReadLine().Split(' ');
                if (header.Length != 2) return;
                Words = int.Parse(header[0]);
                Size = int.Parse(header[1]);

                M = new float[Words * Size];
                Vocab = new char[Words * max_w];
                for (int b = 0; b < Words; b++)
                {
                    int a = 0;

                    var line = reader.ReadLine()?.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (line == null) continue;

                    string word = line[0];
                    foreach (char ch in word)
                    {
                        Vocab[b * max_w + a] = ch;
                        if ((a < max_w) && (vocab[b * max_w + a] != '\n')) a++;
                    }
                    Vocab[b * max_w + a] = '\0';

                    // line[1] is base64 encoded
                    var bytesFloats = Convert.FromBase64String(line[1]);
                    if (bytesFloats.Length == 0)
                        continue;

                    for (a = 0; a < Size; a++)
                        M[a + b * Size] = BitConverter.ToSingle(bytesFloats, a * sizeof(float));

                    float len = 0;
                    for (a = 0; a < Size; a++) len += M[a + b * Size] * M[a + b * Size];
                    len = (float)Math.Sqrt(len);
                    for (a = 0; a < Size; a++) M[a + b * Size] = M[a + b * Size] / len;

                    if (reader.EndOfStream) break;
                }
            }
        }

    }
}
