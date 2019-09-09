using DeepLearningFramework.Core;
using DeepLearningFramework.Data.Operators.Layers;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Tests
{
    class Program
    {
        public static float[,] Randomize(float[,] a)
        {
            Random r = new Random();
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = (float)(r.NextDouble()*2-1);
                }
            }
            return a;
        }

        public static void deneme()
        {
            Hyperparameters.LearningRate = 0.2f;
            Hyperparameters.Optimizer = new SGD();

            var x = new Input(2);
            var y = new Input(1);

            var l1 = new Dense(2, "sigmoid"); l1.PreviousLayer = x;
            var model = new Dense(1, "sigmoid"); model.PreviousLayer = l1;
            var loss = new SquaredError(model, y);

           


            Stopwatch s = new Stopwatch();
            s.Start();
            Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
            for (int epoch = 0; epoch < 5000; epoch++)
            {
                x.SetInput(0, new float[2, 4]
                  {{ 1, 1, 0, 0},
                { 1, 0, 1, 0}});

                y.SetInput(0, new float[1, 4]
                     {{ 0, 1, 1, 0 }});
                loss.GetTerm(0).Minimize();
                //Console.WriteLine("loss: " + loss.GetResult()[0]);
            }

            loss.GetTerm(0).DeleteResults();
            Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
            Console.WriteLine("Results: " + model.GetTerm(0).GetResult()[0] + ", " + model.GetTerm(0).GetResult()[1] + ", " + model.GetTerm(0).GetResult()[2] + ", " + model.GetTerm(0).GetResult()[3]);
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
        }
        public static Term Layer(Term x, int size, bool sigmoid) //Demo Dense Layer
        {
            //Initializers will be in Layer classes, not in variable class!.
            Variable w0 = new Variable(size, x.D1)
            {
                Weights = Randomize(new float[size, x.D1]),
                Name = "w0",
                Trainable = true
            };


            Variable b0 = new Variable(size, 1)
            {
                Weights = Randomize(new float[size, 1]),
                Name = "b0",
                Trainable = true
            };
            Term h = new Plus(new MatrixMultiply(w0, x), new ExpandWithSame(b0, 1, x.D2));

            if (sigmoid)
                return new Sigmoid(h);
            return h;
        }
 
        public static Func<Term, Term> Layer(int size, bool sigmoid) //Demo Dense Layer
        {
            //Initializers will be in Layer classes, not in variable class!.

            Term M(Term x)
            {
                Variable w0 = new Variable(size, x.D1)
                {
                    Weights = Randomize(new float[size, x.D1]),
                    Name = "w0",
                    Trainable = true
                };

                Variable b0 = new Variable(size, 1)
                {
                    Weights = Randomize(new float[size, 1]),
                    Name = "b0",
                    Trainable = true
                };
                Term h = new Plus(new MatrixMultiply(w0, x), new ExpandWithSame(b0, 1, x.D2));

                if (sigmoid)
                    return new Sigmoid(h);
                return h;
            }

            return M;

        }

        
        public static void deneme2()
        {
            PlaceHolder x = new PlaceHolder(256);
            PlaceHolder y = new PlaceHolder(10);

            Term l1 = Layer(x, 32, true);
            Term l3 = Layer(l1, 10, true);

            Term diff = new Minus(l3, y);

            Term lossdiscrete = new Power(diff, 2);
            Term loss = new ShrinkByAdding(lossdiscrete, lossdiscrete.D1, lossdiscrete.D2);

            int batchsize = 32;
            Hyperparameters.LearningRate = 0.1f;
            for (int ss = 0; ss < 100; ss++)
            {
                float err = 0;
                for (int i = 0; i < d.Count/batchsize; i++)
                {
                    float[,] f = new float[256, batchsize];
                    float[,] l = new float[10, batchsize];
                    for (int j = 0; j < batchsize; j++)
                    {
                        for (int k = 0; k < 256; k++)
                            f[k, j] = d[i * batchsize + j].Key[k];
                        for (int k = 0; k < 10; k++)
                            l[k, j] = d[i * batchsize + j].Value[k];
                    }

                    x.SetVariable(new Variable(256, batchsize) { Weights = f, Trainable = false });
                    y.SetVariable(new Variable(10, batchsize) { Weights = l, Trainable = false });
                    loss.Minimize();
                    err += loss.GetResult().Array[0];
                }
                Console.WriteLine(err / (d.Count / batchsize*batchsize));
            }


            //
            while (true)
            {
                Thread.Sleep(1000);
                string path = @"C:\Users\Faruk\OneDrive\Faruk Nane\Kod\Eski\handwritten digit\NeuralNetwork temiz\NeuralNetwork temiz\NeuralNetwork\bin\Debug";
                StreamReader sr = new StreamReader(path + "\\resim.txt");
                string s = sr.ReadToEnd();
                sr.Close();
                string[] data = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                float[,] features = new float[256, 1];
               
                for (int j = 0; j < 256; j++)
                    features[j,0] = float.Parse(data[j]);
                x.SetVariable(new Variable(256, 1) { Weights = features, Trainable = false });
                l3.DeleteResults();
                Matrix res = l3.GetResult();
                float max = -1;
                int index = -1;
                for (int i = 0; i < res.D1; i++)
                {
                    if(max < res[i])
                    { max = res[i]; index = i; }
                }
                Console.WriteLine(index);
            }
            //
        }

        public static List<KeyValuePair<float[], float[]>> d;
        public static void LoadData()
        {
            d = new List<KeyValuePair<float[], float[]>>();
            StreamReader rs = new StreamReader("mnistdata.txt");
            string s = rs.ReadToEnd();
            rs.Close();
            string[] str = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < str.Length; i++)
            {
                string[] data = str[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                float[] features = new float[256];
                for (int j = 0; j < 256; j++)
                    features[j] = float.Parse(data[j]);

                float[] labels = new float[10];
                for (int j = 256; j < 266; j++)
                    labels[j-256] = float.Parse(data[j]);

                d.Add(new KeyValuePair<float[], float[]>(features, labels));
            }
        }

        public static void deneme3()
        {
            Hyperparameters.LearningRate = 0.00001f;
            Hyperparameters.Optimizer = new SGD();

            var x = new Input(1);
            var y = new Input(1);

            var l1 = new SimpleRNN(10); l1.PreviousLayer = x;
            var model = new Dense(1); model.PreviousLayer = l1;
            var loss = new SquaredError(model, y);

            int seqlength = 10;
            Random r = new Random();
            for (int epoch = 0; epoch < 2000; epoch++)
            {
                float sum = 0;
                for (int i = 0; i < seqlength; i++)
                {
                    float num = (float)r.NextDouble() * 1;
                    x.SetInput(i, new float[1, 1]
                    {{ num}});
                    sum += num;
                    y.SetInput(i, new float[1, 1]
                       {{ sum } });
                }
                //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
                loss.GetTerm(seqlength - 1).Minimize();
                //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
                Console.WriteLine("Loss: " + loss.GetTerm(seqlength - 1).GetResult()[0]);
            }
            while (true)
            {
                float sum = 0;
                for (int i = 0; i < seqlength; i++)
                {
                    float num = (float)r.NextDouble() * 1;
                    x.SetInput(i, new float[1, 1]
                    {{ num}});
                    sum += num;
                    Console.Write(sum + ", ");
                    y.SetInput(i, new float[1, 1]
                       {{ sum } });
                    model.GetTerm(i).DeleteResults();
                }

                Console.WriteLine();

                for (int i = 0; i < seqlength; i++)
                {
                    Console.Write(model.GetTerm(i).GetResult()[0] + ", ");
                }
                Console.WriteLine();

                break;
            }
        }


        static void Main(string[] args)
        {
            
            Stopwatch s = new Stopwatch();
            s.Start();
            deneme3();
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);

            Console.WriteLine("Hello World!");
        }

    }
}
