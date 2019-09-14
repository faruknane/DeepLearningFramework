using DeepLearningFramework.Core;
using DeepLearningFramework.Data;
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

        public static void deneme()
        {
            Hyperparameters.LearningRate = 0.2f;
            Hyperparameters.Optimizer = new SGD();

            var x = new Input(2);
            var y = new Input(1);
            //var l1 = Layer.Dense(2, x, "sigmoid"); 

            var l1 = Layer.Dense(2, x, "sigmoid"); 
            var model = Layer.Dense(1, l1, "sigmoid"); 
            var loss = Layer.SquaredError(model, y);


            Stopwatch s = new Stopwatch();
            s.Start();
            Console.WriteLine("Pool.UnreturnedArrayCount: " + MMDerivative.Pool.UnreturnedArrayCount);
            for (int epoch = 0; epoch < 20000; epoch++)
            {
                x.SetSequenceLength(1);
                y.SetSequenceLength(1);
                x.SetInput(0, new float[2, 4] { { 1, 1, 0, 0 }, { 1, 0, 1, 0 } } );
                y.SetInput(0, new float[1, 4] { { 0, 1, 1, 0 } } );

                loss.Minimize();
                //Console.WriteLine("loss: " + loss.GetResult()[0]);
            }
            loss.GetTerm(0).DeleteResults();
            Console.WriteLine("Pool.UnreturnedArrayCount: " + MMDerivative.Pool.UnreturnedArrayCount);
            Console.WriteLine("Results: " + model.GetTerm(0).GetResult()[0] + ", " + model.GetTerm(0).GetResult()[1] + ", " + model.GetTerm(0).GetResult()[2] + ", " + model.GetTerm(0).GetResult()[3]);
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
        }

        public static void deneme2()
        {
            var x = new Input(256);
            var y = new Input(10);
            var l1 = Layer.Dense(32, x, "sigmoid");
            var model = Layer.Dense(10, l1, "sigmoid");
            var loss = Layer.SquaredError(model, y);


            int batchsize = 32;
            Hyperparameters.LearningRate = 0.1f;
            Console.WriteLine("Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
            for (int ss = 0; ss < 20; ss++)
            {
                float err = 0;
                for (int i = 0; i < d.Count / batchsize; i++)
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

                    //training procedure
                    //1 - set input sequence length
                    //2 - assign values to input
                    //3 - minimize loss function
                    //add evaluate!
                    x.SetSequenceLength(1);
                    y.SetSequenceLength(1); 
                    x.SetInput(0, f);
                    y.SetInput(0, l);
                    loss.Minimize();
                    err += loss.GetTerm(0).GetResult().Array[0];
                }
                Console.WriteLine(err / (d.Count / batchsize * batchsize));
            }
            loss.DeleteTerms();
            Console.WriteLine("Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);

            //while (true)
            //{
            //    Thread.Sleep(1000);
            //    string path = @"C:\Users\Faruk\OneDrive\Faruk Nane\Kod\Eski\handwritten digit\NeuralNetwork temiz\NeuralNetwork temiz\NeuralNetwork\bin\Debug";
            //    StreamReader sr = new StreamReader(path + "\\resim.txt");
            //    string s = sr.ReadToEnd();
            //    sr.Close();
            //    string[] data = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    float[,] features = new float[256, 1];

            //    for (int j = 0; j < 256; j++)
            //        features[j, 0] = float.Parse(data[j]);
            //    x.SetVariable(new Variable(256, 1) { Weights = features, Trainable = false });
            //    l3.DeleteResults();
            //    Matrix res = l3.GetResult();
            //    float max = -1;
            //    int index = -1;
            //    for (int i = 0; i < res.D1; i++)
            //    {
            //        if (max < res[i])
            //        { max = res[i]; index = i; }
            //    }
            //    Console.WriteLine(index);
            //}

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
                    labels[j - 256] = float.Parse(data[j]);

                d.Add(new KeyValuePair<float[], float[]>(features, labels));
            }
        }


        public static void deneme3()
        {
            Hyperparameters.LearningRate = 0.0001f;
            Hyperparameters.Optimizer = new SGD();

            var x = new Input(1);
            var y = new Input(1);

            var l1 = Layer.SimpleRNNDemo(10, x);
            var model = Layer.Dense(1, l1, "");
            var loss = Layer.SquaredError(model, y);
            int seqlength = 10;
            Random r = new Random();

            for (int epoch = 0; epoch < 15000; epoch++)
            {
                x.SetSequenceLength(seqlength);
                y.SetSequenceLength(seqlength);

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
                loss.Minimize();
                //Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);
                //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
            }
            Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);
            while (true)
            {
                x.SetSequenceLength(seqlength);
                y.SetSequenceLength(seqlength);

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
                }

                Console.WriteLine();

                loss.DeleteTerms();
                for (int i = 0; i < seqlength; i++)
                {
                    Console.Write(model.GetTerm(i).GetResult()[0] + ", ");
                }
                Console.WriteLine();
                Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);

                break;
            }
        }

      
        public static void deneme4()
        {
            Hyperparameters.LearningRate = 0.2f;
            Hyperparameters.Optimizer = new SGD();

            var x = new Input(2);
            var futurevalue = new ShiftTime(x, -1);

            {
                x.SetSequenceLength(3);

                x.SetInput(0, new float[2, 4] { { 1, 1, 0, 0 }, { 1, 0, 1, 0 } });
                x.SetInput(1, new float[2, 4] { { 2, 2, 0, 0 }, { 2, 0, 2, 0 } });
                x.SetInput(2, new float[2, 4] { { 3, 3, 0, 0 }, { 3, 0, 3, 0 } });

                x.DeleteTerms();//use before evaluating

                for(int i = 0; i < futurevalue.SequenceLength; i++)
                {
                    for (int j = 0; j < futurevalue.GetTerm(i).D1 * futurevalue.GetTerm(i).D2; j++)
                        Console.Write(futurevalue.GetTerm(i).GetResult()[j]);
                    Console.WriteLine();
                }

            }
        }

        static void Main(string[] args)
        {

            LoadData();
            Stopwatch s = new Stopwatch();
            s.Start();
            deneme();
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);

            Console.WriteLine("Hello World!");
        }

    }
}
