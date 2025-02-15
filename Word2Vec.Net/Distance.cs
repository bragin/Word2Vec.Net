﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Distance.cs" >
//   Create on 19:43:33 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the Distance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Word2Vec.Net
{
    public class Distance : Word2VecAnalysisBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">path to binary file created by Word2Vec</param>
        public Distance(string fileName) : base(fileName)
        {

        }

        /// <summary>
        /// Add the m variable of two Distances, putting the answer into this copy's of m
        /// </summary>
        /// <param name="that"></param>
        private void AddVector(Distance that)
        {
            int vectorLength1 = this.M.Length;
            int vectorLength2 = that.M.Length;
            if (vectorLength1 != vectorLength2)
            {
                return;
            }
            for (int index = 0; index < vectorLength1; index++)
            {
                this.M[index] += that.M[index];
            }
        }

        /// <summary>
        /// Subtracts the m variable of that from this, putting the answer into this copy's of m
        /// </summary>
        /// <param name="that"></param>
        private void SubtractVector(Distance that)
        {
            int vectorLength1 = this.M.Length;
            int vectorLength2 = that.M.Length;
            if (vectorLength1 != vectorLength2)
            {
                return;
            }
            for (int index = 0; index < vectorLength1; index++)
            {
                this.M[index] -= that.M[index];
            }
        }

        /// <summary>
        /// Avergas the m variable of that and this, putting the answer into this copy's of m
        /// </summary>
        /// <param name="that"></param>
        private void AverageVectors(Distance that)
        {
            int vectorLength1 = this.M.Length;
            int vectorLength2 = that.M.Length;
            if (vectorLength1 != vectorLength2)
            {
                return;
            }
            for (int index = 0; index < vectorLength1; index++)
            {
                this.M[index] = 0.5f * (this.M[index] + that.M[index]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[] GetWordVector()
        {
            return this.M;
        }

        /// <summary>
        /// search nearest words to <param name="intext"></param>
        /// </summary>
        /// <returns>nearest words</returns>
        public BestWord[] Search(string intext)
        {
            BestWord[] bestWords = new BestWord[N];
            long[] bi = new long[100];
            float[] vec = new float[max_size];
            string[] st = intext.Split(' ');
            int cn = st.Length;
            long b = -1;
            for (long a = 0; a < cn; a++)
            {

                for (b = 0; b < Words; b++)
                {
                    string word = new string(Vocab, (int)(b * max_w), (int)max_w).Replace("\0", string.Empty);
                    if (word.Equals(st[a])) break;
                }
                if (b == Words) b = -1;
                bi[a] = b;
                Console.Write("\nWord: {0}  Position in vocabulary: {1}\n", st[a], bi[a]);
                if (b == -1)
                {
                    Console.Write("Out of dictionary word!\n");
                    break;
                }
            }
            if (b == -1) return new BestWord[0];

            for (long a = 0; a < Size; a++) vec[a] = 0;
            for (b = 0; b < cn; b++)
            {
                if (bi[b] == -1) continue;
                for (long a = 0; a < Size; a++) vec[a] += M[a + bi[b] * Size];
            }
            float len = 0;
            for (long a = 0; a < Size; a++) len += vec[a] * vec[a];
            len = (float)Math.Sqrt(len);
            for (long a = 0; a < Size; a++) vec[a] /= len;
            for (long c = 0; c < Words; c++)
            {
                long a = 0;
                for (b = 0; b < cn; b++) if (bi[b] == c) a = 1;
                if (a == 1) continue;
                float dist = 0;
                for (a = 0; a < Size; a++) dist += vec[a] * M[a + c * Size];
                for (a = 0; a < N; a++)
                {
                    if (dist > bestWords[a].Distance)
                    {
                        for (long d = N - 1; d > a; d--)
                        {
                            bestWords[d] = bestWords[d - 1];
                        }
                        bestWords[a].Distance = dist;
                        bestWords[a].Word = new string(Vocab, (int)(max_w * c), (int)max_w).Replace("\0", String.Empty).Trim();
                        break;
                    }
                }
            }
            return bestWords;
        }




    }

    public struct BestWord
    {
        public string Word { get; set; }
        public float Distance { get; set; }
    }
}