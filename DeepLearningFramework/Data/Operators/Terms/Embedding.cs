//using System;
//using System.Collections.Generic;
//using System.Text;
//using DeepLearningFramework.Core;
//using PerformanceWork.OptimizedNumerics;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Terms
//{
//    public class Embedding : Term, Trainable
//    {
//        public int VocabSize { get; private set; }
//        public int VectorSize { get; private set; }
//        public float LearningRateMultiplier { get; set; } = 1;
//        public Matrix Weights { get => Vocabulary; }

//        public bool Trainable { get; set; } = true;

//        public Matrix Vocabulary { get; set; }

//        public int UniqueId { get; set; }

//        public Embedding(Term v1, int VectorSize, int VocabSize)
//        {
//            Type = TermType.Embedding;
//            Terms = new Term[1] { v1 };
//            this.VectorSize = VectorSize;
//            D1 = VectorSize;
//            if (this.Terms[0].D1 != 1)
//                throw new Exception("Embedding Term takes (1) number indicating a word, and gives a vector");
//            D2 = this.Terms[0].D2;
//            this.VocabSize = VocabSize;
//            Vocabulary = new Matrix(VectorSize, VocabSize);
//            unsafe
//            {
//                Layers.Randomiz.Randomize(Vocabulary.Array, Vocabulary.D1 * Vocabulary.D2);
//            }
//            UniqueId = Variable.UniqueIdAssigner;
//            Variable.UniqueIdAssigner++;

//            if (this.D1 <= 0 || this.D2 <= 0)
//                throw new Exception("Dimensions should have an exact value!");
//        }

//        public override void CalculateDerivate(MMDerivative s)
//        {

//            //S.D1 = 1, S.D2 = 1 && S.D3 = this.D1, S.D4 = this.D2

//            Matrix wordindices = Terms[0].GetResult();

//            MMDerivative combined = new MMDerivative(s.D1, s.D2, Vocabulary.D1, Vocabulary.D2, true);
//            combined.Negative = s.Negative;
//            for (int i6 = 0; i6 < this.D2; i6++)
//            {
//                int i4 = (int)wordindices[0, i6];
//                for (int i1 = 0; i1 < s.D1; i1++)
//                    for (int i2 = 0; i2 < s.D2; i2++)
//                        for (int i3 = 0; i3 < Vocabulary.D1; i3++)
//                        //for (int i4 = 0; i4 < Vocabulary.D2; i4++)
//                        {

//                            combined[i1, i2, i3, i4] += s[i1, i2, i3, i6];
//                            // i5,i6,i3,i4;
//                            //this.result[i5,i6] / vocab[i3][i4]
//                            //wordindex of this.result[i5,i6] = wordindices[0,i6]
//                            //if wordindex = i4 and i5 = i3, it is 1
//                        }
//            }

//            if (this.Trainable)
//            {
//                Hyperparameters.Optimizer.UpdateWeights(this, combined);
//            }
//        }

//        internal override Matrix CalculateResult()
//        {
//            Matrix wordindices = Terms[0].GetResult();

//            Matrix res = new Matrix(VectorSize, D2);
//            for(int i = 0; i < wordindices.D2; i++)
//            {
//                int wordindex = (int)wordindices[0, i];
//                for (int j = 0; j < VectorSize; j++)
//                    res[j, i] = Vocabulary[j, wordindex];
//            }

//            return res;
//        }
//    }
//}
