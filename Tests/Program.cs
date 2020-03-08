using DeepLearningFramework.Core;
using DeepLearningFramework.Core.Optimizers;

using DeepLearningFramework.Operators.Layers;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Index = PerformanceWork.OptimizedNumerics.Index;
using Terms = DeepLearningFramework.Operators.Terms;

namespace Tests
{
    class Program
    {
        //public static void deneme()
        //{
        //    Hyperparameters.LearningRate = 1f;
        //    Hyperparameters.Optimizer = new SGD();

        //    var x = new Input(2);
        //    var y = new Input(1);

        //    var model = Layer.Dense(2, x, "sigmoid");
        //    model = Layer.Dense(1, model, "sigmoid");

        //    var loss = Layer.SquaredError(model, y);

        //    Stopwatch s = new Stopwatch();
        //    s.Start();
        //    //Console.WriteLine("Pool.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);
        //    for (int epoch = 0; epoch < 2000; epoch++)
        //    {
        //        x.SetSequenceLength(1);
        //        y.SetSequenceLength(1);
        //        x.SetInput(0, new float[2, 4] { { 1, 1, 0, 0 }, { 1, 0, 1, 0 } });
        //        y.SetInput(0, new float[1, 4] { { 0, 1, 1, 0 } });

        //        loss.Minimize();
        //        //Console.WriteLine("loss: " + loss.GetTerm(0).GetResult()[0]);
        //    }
        //    model.DeleteTerms();
        //    //Console.WriteLine("Pool.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);
        //    var result = model.GetTerm(0).GetResult();
        //    Console.WriteLine("Results: " + result[0] + ", " + result[1] + ", " + result[2] + ", " + result[3]);
        //    s.Stop();
        //    Console.WriteLine(s.ElapsedMilliseconds);
        //}

        //public static unsafe void deneme2()
        //{
        //    LoadData();
        //    //Load training data.

        //    Hyperparameters.LearningRate = 0.001f;

        //    var x = new Input(784);
        //    var y = new Input(10);

        //    var l1 = Layer.Dense(500, x, "sigmoid");
        //    var model = Layer.Dense(10, l1, "softmax");

        //    var loss = Layer.SquaredError(model, y);

        //    int batchsize = 100;
        //    //Console.WriteLine("Pool.UnreturnedArrayCount:  " + Matrix.Pool.UnreturnedArrayCount);
        //    //Console.WriteLine("MMPool.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    int run = 10;
        //    for (int ss = 0; ss < run; ss++)
        //    {
        //        float err = 0;
        //        for (int i = 0; i < d.Length / batchsize; i++)
        //        {
        //            sw.Stop();
        //            Matrix f = new Matrix(784, batchsize);
        //            Matrix l = new Matrix(10, batchsize);

        //            for (int j = 0; j < batchsize; j++)
        //            {
        //                for (int k = 0; k < 784; k++)
        //                    f[k, j] = d[i * batchsize + j].Key[k];
        //                for (int k = 0; k < 10; k++)
        //                    l[k, j] = d[i * batchsize + j].Value[k];
        //            }
        //            sw.Start();
        //            //training procedure
        //            //1 - set input sequence length
        //            //2 - assign values to input
        //            //3 - minimize loss function
        //            //add evaluate!
        //            x.SetSequenceLength(1);
        //            y.SetSequenceLength(1);
        //            x.SetInput(0, f);
        //            y.SetInput(0, l);
        //            loss.Minimize();
        //            err += loss.GetTerm(0).GetResult().Array[0];
        //            f.Dispose();
        //            l.Dispose();
        //        }

        //        Console.WriteLine($"Hyperparameters.LearningRate -> {Hyperparameters.LearningRate}");
        //        Console.WriteLine(err / (d.Length / batchsize * batchsize));
        //    }
        //    sw.Stop();
        //    Console.WriteLine($"TotalTimeElapsed: {sw.ElapsedMilliseconds}, Run {run} times!");
        //    model.DeleteTerms();
        //    //Console.WriteLine("Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
        //    //Console.WriteLine("MMPool.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);

        //    while (false)
        //    {
        //        Thread.Sleep(1000);
        //        string path = @"C:\Users\Faruk\OneDrive\Faruk Nane\Kod\Eski\handwritten digit\NeuralNetwork temiz\NeuralNetwork temiz\NeuralNetwork\bin\Debug";
        //        StreamReader sr = new StreamReader(path + "\\resim.txt");
        //        string s = sr.ReadToEnd();
        //        sr.Close();
        //        string[] data = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        float[,] features = new float[784, 1];

        //        for (int j = 0; j < 784; j++)
        //            features[j, 0] = float.Parse(data[j]);
        //        x.SetSequenceLength(1);
        //        x.SetInput(0, features);
        //        model.DeleteTerms();
        //        Matrix res = model.GetTerm(0).GetResult();
        //        float max = -1;
        //        int index = -1;
        //        for (int i = 0; i < res.D1; i++)
        //        {
        //            if (max < res[i])
        //            { max = res[i]; index = i; }
        //        }
        //        Console.WriteLine(index);
        //    }

        //}


        //public static KeyValuePair<float[], float[]>[] d;

        //public static void LoadData()
        //{
        //    d = new KeyValuePair<float[], float[]>[60000];
        //    StreamReader rs = new StreamReader("mnistdata.txt");
        //    string s = rs.ReadToEnd();
        //    rs.Close();
        //    string[] str = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //    for (int i = 0; i < str.Length; i++)
        //    {
        //        string[] data = str[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //        float[] features = new float[784];
        //        for (int j = 0; j < 784; j++)
        //            if (j < data.Length)
        //                features[j] = float.Parse(data[j]);
        //            else
        //                features[j] = 0;

        //        float[] labels = new float[10];
        //        for (int j = 784; j < 784 + 10; j++)
        //            if (j < data.Length)
        //                labels[j - 784] = float.Parse(data[j]);
        //            else
        //                labels[j - 784] = 0;
        //        d[i] = (new KeyValuePair<float[], float[]>(features, labels));
        //    }
        //}


        //public static void deneme3()
        //{
        //    Hyperparameters.LearningRate = 0.0001f;
        //    Hyperparameters.Optimizer = new SGD();

        //    var x = new Input(1);
        //    var y = new Input(1);


        //    Layer l1 = new StaticRecurrent(10, x.SequenceLength, x.BatchSize, new[] { x },
        //        (Layer h, Layer[] x) =>
        //        {
        //            var WH = new Variable(h.D1, h.D1, x[0].SequenceLength, setzero: true); // bu bir kere işleniyor bende, terms ile yaparsam her defasında işlenen bir sistem ile daha manuellik sağlayabilirim.

        //            return WH * h + Layer.Dense(h.D1, x[0], "");
        //        }
        //    );



        //    var model = Layer.Dense(1, l1, "");
        //    var loss = Layer.SquaredError(model, y);

        //    int seqlength = 10;
        //    Random r = new Random();
        //    for (int epoch = 0; epoch < 15000; epoch++)
        //    {
        //        x.SetSequenceLength(seqlength);
        //        y.SetSequenceLength(seqlength);

        //        float sum = 0;
        //        float sum2 = 0;
        //        for (int i = 0; i < seqlength; i++)
        //        {
        //            float num = (float)r.NextDouble() * 1;
        //            float num2 = (float)r.NextDouble() * 1;
        //            x.SetInput(i, new float[1, 2] { { num, num2 } });
        //            sum += num;
        //            sum2 += num2;
        //            y.SetInput(i, new float[1, 2] { { sum, sum2 } });
        //        }
        //        //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
        //        //Console.WriteLine("MMDerivative.Pool2.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);
        //        loss.Minimize();
        //        //Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);
        //        //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
        //        //Console.WriteLine("MMDerivative.Pool2.UnreturnedArrayCount: " + MMDerivative.Pool2.UnreturnedArrayCount);
        //    }
        //    Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);


        //    for (int d = 0; d < 10; d++)
        //    {
        //        if (d%2 == 0)
        //        {
        //            x.SetSequenceLength(seqlength);

        //            float sum = 0;
        //            for (int i = 0; i < seqlength; i++)
        //            {
        //                float num = (float)r.NextDouble() * 1;
        //                x.SetInput(i, new float[1, 1] { { num } });
        //                sum += num;
        //                Console.Write(sum + ", ");
        //            }

        //            Console.WriteLine();

        //            loss.DeleteTerms();

        //            for (int i = 0; i < seqlength; i++)
        //            {
        //                model.GetTerm(i);
        //                Console.Write(model.GetTerm(i).GetResult()[0] + ", ");
        //            }
        //            Console.WriteLine();
        //            Console.WriteLine();
        //            Console.WriteLine();
        //            //Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);
        //        }
        //        else
        //        {
        //            x.SetSequenceLength(seqlength);

        //            float sum = 0;
        //            float sum2 = 0;
        //            for (int i = 0; i < seqlength; i++)
        //            {
        //                float num = (float)r.NextDouble() * 1;
        //                float num2 = (float)r.NextDouble() * 1;
        //                x.SetInput(i, new float[1, 2] { { num, num2 } });
        //                sum += num;
        //                sum2 += num2;
        //                Console.Write(sum + ", ");
        //                Console.Write(sum2 + ", ");
        //            }

        //            Console.WriteLine();
        //            loss.DeleteTerms();
        //            for (int i = 0; i < seqlength; i++)
        //            {
        //                model.GetTerm(i);
        //                Console.Write(model.GetTerm(i).GetResult()[0] + ", ");
        //                Console.Write(model.GetTerm(i).GetResult()[1] + ", ");
        //            }
        //            Console.WriteLine();
        //            Console.WriteLine();
        //            Console.WriteLine();
        //            //Console.WriteLine("Loss: " + loss.GetTerm(0).GetResult()[0]);
        //        }
        //    }
        //}


        //public static void TestShiftingTime()
        //{
        //    Hyperparameters.LearningRate = 0.2f;
        //    Hyperparameters.Optimizer = new SGD();

        //    var x = new Input(2);
        //    var futurevalue = new ShiftTime(x, -1);

        //    { 
        //        x.SetSequenceLength(3);

        //        x.SetInput(0, new float[2, 4] { { 1, 1, 0, 0 }, { 1, 0, 1, 0 } });
        //        x.SetInput(1, new float[2, 4] { { 2, 2, 0, 0 }, { 2, 0, 2, 0 } });
        //        x.SetInput(2, new float[2, 4] { { 3, 3, 0, 0 }, { 3, 0, 3, 0 } });

        //        x.DeleteTerms(); //use before evaluating

        //        for (int i = 0; i < futurevalue.SequenceLength; i++)
        //        {
        //            for (int j = 0; j < futurevalue.GetTerm(i).D1 * futurevalue.GetTerm(i).D2; j++)
        //                Console.Write(futurevalue.GetTerm(i).GetResult()[j]);
        //            Console.WriteLine();
        //        }

        //    }
        //}
        //public static void TestSoftMax()
        //{
        //    Terms.Variable v1 = new Terms.Variable(3, 2)
        //    {
        //        Weights = new float[3, 2] {
        //        { 1, 1 },
        //        { 3, 2 },
        //        { 5, 1 }}
        //    };


        //    var s = new Terms.SoftMax(v1);
        //    var softres = s.GetResult();

        //    Console.WriteLine(softres[0, 0]);
        //    Console.WriteLine(softres[1, 0]);
        //    Console.WriteLine(softres[2, 0]);
        //    Console.WriteLine("-----");
        //    Console.WriteLine(softres[0, 1]);
        //    Console.WriteLine(softres[1, 1]);
        //    Console.WriteLine(softres[2, 1]);
        //}

        //public static void deneme4()
        //{
        //    var x = new Input(1);

        //    Layer l1 = new StaticRecurrent(1, new[] { x },
        //        (Layer h, Layer[] x) =>
        //        {
        //            //dont use foreign layer inside StaticRecurrent. beacuse the terms of all layers connected to model has to be deleted.
        //            return h + x[0];
        //        }
        //    );

        //    if (true)
        //    {
        //        l1 = new DynamicRecurrent(1, x.SequenceLength, x.BatchSize, new[] { x },
        //            (Layer h, Layer[] x, int t) =>
        //            {
        //                //bunun içinde termlerle ilgili istediğin operasyonu yapabilirsin çünkü o termler erişilemeyince GC direk siliyor.

        //                if (t % 2 == 0)
        //                    return new Terms.Plus(h.GetTerm(t - 1), x[0].GetTerm(t));
        //                else
        //                    return h.GetTerm(t - 1);
        //            }
        //        );
        //    }
        //    else
        //    {
        //        l1 = new DynamicRecurrent(1, x.SequenceLength / new Dimension(2), x.BatchSize, new[] { x },
        //            (Layer h, Layer[] x, int t) =>
        //            {
        //                return new Terms.Plus(h.GetTerm(t - 1), x[0].GetTerm(2*t), x[0].GetTerm(2*t+1));
        //            }
        //        );
        //    }
        //    int seqlength = 10;
        //    Random r = new Random();
        //    {
        //        x.SetSequenceLength(seqlength);
        //        Console.Write("\nReal Results: ");
        //        float sum = 0;
        //        for (int i = 0; i < seqlength; i++) 
        //        {
        //            float num = (float)r.NextDouble() * 1;
        //            x.SetInput(i, new float[1, 1]
        //            {{ num}});
        //            sum += num;
        //            Console.Write(sum + (i == seqlength - 1 ? "" : ", "));
        //        }
        //        //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
        //        Console.Write("\n\nExperiment Results: ");
        //        l1.DeleteTerms();
        //        for (int i = 0; i < l1.SequenceLength.Value; i++)
        //            Console.Write(l1.GetTerm(i).GetResult()[0] + (i == seqlength - 1 ? "" : ", "));
        //        Console.WriteLine();
        //        //Console.WriteLine("Matrix.Pool.UnreturnedArrayCount: " + Matrix.Pool.UnreturnedArrayCount);
        //    }

        //}

        //public static void TestEmbeddings()
        //{
        //    Terms.PlaceHolder x = new Terms.PlaceHolder(1);
        //    Terms.Embedding a = new Terms.Embedding(x, 10, 3);
        //    x.SetVariable(new Terms.Variable(new float[1, 6] { { 0, 1, 2, 1, 0, 2 } }));
        //    Matrix res = a.GetResult();

        //    for (int j = 0; j < res.D2; j++)
        //    {
        //        for (int i = 0; i < res.D1; i++)
        //            Console.Write(res[i, j] + ", ");
        //        Console.WriteLine();
        //    }

        //    a.Minimize();
        //}


        public static unsafe void Print(Tensor<float> x)
        {
            float* ptr = (float*)x.Array;
            for (int i = 0; i < x.Shape.TotalSize; i++)
                Console.Write(ptr[i] + ", ");
            Console.WriteLine();
        }

        public static unsafe void bb()
        {
            Terms.Variable w = new Terms.Variable(Shape.NewShape(1, 3));
            w.SetValue(new float[1, 3] {
                    { 2, 3, 1 }
                });

            Terms.Variable w2 = new Terms.Variable(Shape.NewShape(1, 3));
            w2.SetValue(new float[1, 3] {
                    { 1, 1, 1 }
                });

            Terms.Plus s = new Terms.Plus(w, w2);


            Hyperparameters.LearningRate = 1;
            Console.WriteLine(s.GetResult());
            for (int i = 0; i < 1; i++)
            {
                s.Minimize();
            }
            s.DeleteResults();
            Console.WriteLine(s.GetResult());
            Console.WriteLine(w.GetResult());

            w.Clean();
            w2.Clean();
            s.Dispose();
            //Variables should be cleaned manually and disposed manually! All other terms should call dispose method. 
        }

        public static unsafe void bb2()
        {
            Variable w = new Variable(Shape.NewShape(2, 3), new Dimension[] { 1, 1 });
            Variable w2 = new Variable(Shape.NewShape(2, 3), new Dimension[] { 1, 1 });
            ExpandWithSame sum = new ExpandWithSame(new Plus(w, w2), new Dimension[] { 2, 3 });

            Shape s = Shape.NewShape(1, 1);
            Index a = Index.NewIndex(s);

            sum.PreCheck();
            a.SetZero();
            Console.WriteLine("w: ");
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(w.GetTerm(a).GetResult());
            
            a.SetZero();
            Console.WriteLine("w2: ");
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(w2.GetTerm(a).GetResult());

            a.SetZero();
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(sum.GetTerm(a).GetResult());
            
            Hyperparameters.LearningRate = 0.02f;

            for (int i = 0; i < 10000; i++)
                sum.Minimize();

            sum.PreCheck();
            a.SetZero();
            Console.WriteLine("w: ");
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(w.GetTerm(a).GetResult());

            a.SetZero();
            Console.WriteLine("w2: ");
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(w2.GetTerm(a).GetResult());

            a.SetZero();
            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
                Console.WriteLine(sum.GetTerm(a).GetResult());

            Index.Return(a);
            Shape.Return(s);
        }


        public static unsafe void Main(string[] args)
        {

            int a = 5;

            int s1 = Shape.ShapePool.UnreturnedArrayCount;

            for (int i = 0; i < 1; i++)
            {
                bb2();
            }

            s1 = Shape.ShapePool.UnreturnedArrayCount;
            Console.WriteLine(Shape.ObjectPool.Count);
            //Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //Stopwatch s = new Stopwatch();
            //s.Start();
            //deneme2();
            //s.Stop();
            //Console.WriteLine(s.ElapsedMilliseconds);
            //Console.WriteLine("Hello World!");
        }

    }
}
