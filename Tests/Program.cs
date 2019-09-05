using DeepLearningFramework.Core;
using DeepLearningFramework.Data;
using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

            x.SetInput(0, new float[2, 4]
                {{ 1, 1, 0, 0},
                { 1, 0, 1, 0}});

            y.SetInput(0, new float[1, 4]
                 {{ 0, 1, 1, 0 }});


            Stopwatch s = new Stopwatch();
            s.Start();
            for (int epoch = 0; epoch < 5000; epoch++)
            {
                loss.GetTerm(0).Minimize();
                //Console.WriteLine("loss: " + loss.GetResult()[0]);
            }

            loss.GetTerm(0).DeleteResults();
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
            
            Term l1 = Layer(x, 128, true);
            Term l2 = Layer(l1, 64, true);
            Term l3 = Layer(l2, 10, true);
            Term lossdiscrete = new Power(new Minus(l3, y), 2);
            Term loss = new ShrinkByAdding(lossdiscrete, lossdiscrete.D1, lossdiscrete.D2);

            for (int ss = 0; ss < 20; ss++)
            {
                float err = 0;
                for (int i = 0; i < 100; i++)
                {
                    float[,] f = new float[256, 10];
                    float[,] l = new float[10, 10];
                    for (int j = 0; j < 10; j++)
                    {
                        for (int k = 0; k < 256; k++)
                            f[k, j] = d[i * 10 + j].Key[k];
                        for (int k = 0; k < 10; k++)
                            l[k, j] = d[i * 10 + j].Value[k];
                    }

                    x.SetVariable(new Variable(256, 10) { Weights = f, Trainable = false });
                    y.SetVariable(new Variable(10, 10) { Weights = l, Trainable = false });
                    loss.Minimize();
                    err += loss.GetResult().Array[0];
                }
                Console.WriteLine(err / 1000);
            }
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

        
        static void Main(string[] args)
        {
            
            LoadData();
            deneme();
           
            Console.WriteLine("Hello World!");
        }
    }
}
