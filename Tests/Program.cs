using DeepLearningFramework;
using DeepLearningFramework.Core;
using DeepLearningFramework.Core.Optimizers;
using DeepLearningFramework.Operators.Layers;
using PerformanceWork;
using PerformanceWork.OptimizedNumerics;
using PerformanceWork.OptimizedNumerics.Pool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Index = PerformanceWork.OptimizedNumerics.Index;
using Terms = DeepLearningFramework.Operators.Terms;
using static System.Console;

namespace Tests
{
    class Program
    {
        public static void PrintPools()
        {
            Console.WriteLine("TensorPool.UnreturnedArrayCount: " + TensorPool.Host.UnreturnedArrayCount);
        }

        public static float[] LoadCurrentImage()
        {
            string file = @"C:\Users\faruk\source\repos\MNISTDemo\MNISTDemo\bin\Debug\img.txt";
            int l2 = 28 * 28;
            float[] data = new float[l2];

            StreamReader sr = new StreamReader(file);
            string a = sr.ReadToEnd();
            sr.Close();

            if (a.Length != l2)
                throw new Exception("length");

            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 28; j++)
                    data[i * 28 + j] = a[i * 28 + j] - '0';
            return data;
        }
        public static unsafe (float[,], float[,]) LoadMNISTDataSet()
        {
            int l1 = 42000;
            int l2 = 28 * 28;
            int l3 = 10;

            float[,] data = new float[l1, l2];
            float[,] labels = new float[l1, l3];
            //return (data, labels);

            int digitcount = 10;
            string path = @"C:\Users\faruk\source\repos\MNISTDemo\MNISTDemo\trainingSet\";

            for (int i = 0; i < digitcount; i++)
            {
                int maxval = 0;
                string[] files = Directory.GetFiles(path + i.ToString());
                foreach (var file in files)
                {
                    Bitmap bmp = new Bitmap(file);
                    string filename = Path.GetFileNameWithoutExtension(file);
                    int c = int.Parse(filename.Substring(4, filename.Length - 4));
                    {
                        BitmapData bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                        byte* scan0 = (byte*)bData.Scan0.ToPointer();

                        for (int x = 0; x < bData.Height; ++x)
                        {
                            for (int y = 0; y < bData.Width; ++y)
                            {
                                byte* p = scan0 + (x * bData.Stride + y);
                                if (*p >= 128)
                                    data[c, (y * bData.Stride + x)] = 1;
                                else
                                    data[c, (y * bData.Stride + x)] = 0;
                            }
                        }

                        bmp.UnlockBits(bData);
                    }
                    bmp.Dispose();

                    for (int j = 0; j < 10; j++)
                        labels[c, j] = 0;
                    labels[c, i] = 1;
                    maxval = Math.Max(maxval, c);
                }
                Console.WriteLine("okundu " + i + " max -> " + maxval);
            }
            return (data, labels);
        }

        public static unsafe int MaxId(float* pp)
        {
            int maxid = -1;
            float max = 0;

            for (int i = 0; i < 10; i++)
                if (pp[i] > max)
                {
                    max = pp[i];
                    maxid = i;
                }
            return maxid;
        }

        public unsafe static void MNISTExample()
        {
            //Hyperparameters
            Hyperparameters.LearningRate = 0.001f;
            Hyperparameters.Optimizer = new SGD();


            //Model Creation
            var x = new Input(784);
            //var dropout = new Dropout(x, 0.1f);
            //var model = Layer.Dense(500, x, "relu");
            var model = LayerBuilder.Dense(100, x, "relu");
            model = LayerBuilder.Dense(400, model, "relu");
            model = LayerBuilder.Dense(200, model, "relu");
            model = LayerBuilder.Dense(100, model, "relu");
            model = LayerBuilder.Dense(10, model, "softmax");


            //Loss Function Creation
            var y = new Input(10);
            var loss = LayerBuilder.SquaredError(model, y);


            //Data preparation
            (float[,] traindata, float[,] labels) = LoadMNISTDataSet();
            int mnistsize = 42000;

            Tensor x_train = Tensor.LoadFloatArray(traindata, new Shape((mnistsize, 784)));
            Tensor y_train = Tensor.LoadFloatArray(labels, new Shape((mnistsize, 10)));

            //Training
            int batchsize = 100;
            int trainl = 41000;

            Stopwatch s = new Stopwatch();

            for (int epoch = 0; epoch < 35; epoch++)
            {
                float l = 0;
                float val = 0;

                s.Restart();
                Console.WriteLine("Epoch " + epoch + " başladı.");
                for (int batch = 0; batch < trainl / batchsize; batch++)
                {
                    Tensor batchx = Tensor.Cut(x_train, batch * (batchsize * 784), (batch + 1) * (batchsize * 784), new Shape((1, batchsize, 784)));
                    Tensor batchy = Tensor.Cut(y_train, batch * (batchsize * 10), (batch + 1) * (batchsize * 10), new Shape((1, batchsize, 10)));

                    x.SetInput(batchx);
                    y.SetInput(batchy);

                    loss.Minimize();

                    Index zero = new Index(loss.OuterShape);
                    zero.SetZero();

                    Tensor res = loss.GetTerm(zero).GetResult();
                    float* pp = (float*)res.Array;

                    for (int i = 0; i < res.Shape.TotalSize; i++)
                        l += pp[i];

                }

                for (int batch = trainl / batchsize; batch < mnistsize / batchsize; batch++)
                {
                    Tensor batchx = Tensor.Cut(x_train, batch * (batchsize * 784), (batch + 1) * (batchsize * 784), new Shape((1, batchsize, 784)));
                    Tensor batchy = Tensor.Cut(y_train, batch * (batchsize * 10), (batch + 1) * (batchsize * 10), new Shape((1, batchsize, 10)));

                    model.DeleteTerms();

                    x.SetInput(batchx);
                    y.SetInput(batchy);

                    Index zero = new Index(model.OuterShape);
                    zero.SetZero();
                    model.PreCheck();
                    Tensor res = model.GetTerm(zero).GetResult();

                    for (int i = 0; i < batchsize; i++)
                    {
                        int myans = MaxId((float*)res.Array + i * 10);
                        int correctres = MaxId((float*)batchy.Array + i * 10);
                        val += (myans == correctres ? 1 : 0);
                    }

                }
                s.Stop();

                Console.WriteLine("Epoch " + epoch + " biti.");
                Console.WriteLine("Loss: " + l / trainl);
                Console.WriteLine("Validation: " + val / (mnistsize - trainl));
                Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            }

            PrintPools();

            while (true)
            {
                try
                {
                    float[] data = LoadCurrentImage();
                    Tensor x_test = Tensor.LoadFloatArray(data, new Shape((1, 1, 784)));

                    model.DeleteTerms();

                    x.SetInput(x_test);

                    Index zero = new Index(model.OuterShape);
                    zero.SetZero();
                    model.PreCheck();
                    Tensor res = model.GetTerm(zero).GetResult();



                    Console.WriteLine("Result: " + res);
                    Console.WriteLine("Digit Prediction: " + MaxId((float*)res.Array));
                    Console.WriteLine("-----------");
                }
                catch (Exception)
                {

                }
                Thread.Sleep(500);
            }
        }

        public unsafe static void XORExample()
        {
            //Hyperparameters
            Hyperparameters.LearningRate = 0.1f;
            Hyperparameters.Optimizer = new SGD();

            //Model Creation
            var l1 = LayerBuilder.Dense(16, "sigmoid");
            var l2 = LayerBuilder.Dense(1, "sigmoid")[l1];


            var x = new Input(2);
            Layer model = l2[x];

            //Loss Function Creation
            var y = new Input(1);
            var loss = LayerBuilder.SquaredError(model, y);


            //Data preparation
            Tensor x_train = new Tensor((1, 4, 2), DataType.Type.Float, DeviceIndicator.Host());
            Tensor y_train = new Tensor((1, 4, 1), DataType.Type.Float, DeviceIndicator.Host());

            float* xt = (float*)x_train.Array;
            float* yt = (float*)y_train.Array;

            // 1,1 = 0
            // 1,0 = 1
            // 0,1 = 1
            // 0,0 = 0

            xt[0] = 1; xt[1] = 1;
            xt[2] = 1; xt[3] = 0;
            xt[4] = 0; xt[5] = 1;
            xt[6] = 0; xt[7] = 0;

            yt[0] = 0;
            yt[1] = 1;
            yt[2] = 1;
            yt[3] = 0;

            //Give data to the model
            x.SetInput(x_train);
            y.SetInput(y_train);

            Stopwatch s = new Stopwatch();
            s.Start();
            //Minimizing
            loss.PreCheck();
            Index a = new Index(model.OuterShape);
            a.SetZero();

            for (int epoch = 0; epoch < 100000; epoch++)
            {
                loss.Minimize();
                if (epoch % 5000 == 0)
                {
                    float res = ((float*)loss.GetTerm(a).GetResult().Array)[0];
                    res += ((float*)loss.GetTerm(a).GetResult().Array)[1];
                    res += ((float*)loss.GetTerm(a).GetResult().Array)[2];
                    res += ((float*)loss.GetTerm(a).GetResult().Array)[3];
                    Console.WriteLine(res);
                }
            }
            s.Stop();
            Console.WriteLine("Time Elapsed: " + s.ElapsedMilliseconds);

            //Print Pools
            PrintPools();

            //Print the results

            var result = model.GetTerm(a).GetResult();
            Console.WriteLine("Results: " + result);


            //Print the results of clone model
            Input x2 = new Input(2);
            x2.SetInput(x_train);
            var clonemodel = l2[x2];
            clonemodel.PreCheck();
            var result2 = clonemodel.GetTerm(a).GetResult();
            Console.WriteLine("Results: " + result2);

            clonemodel.DeleteTerms();
            model.DeleteTerms();
        }

        public static unsafe void Print(Tensor x)
        {
            float* ptr = (float*)x.Array;
            for (int i = 0; i < x.Shape.TotalSize; i++)
                Console.Write(ptr[i] + ", ");
            Console.WriteLine();
        }

        public static unsafe void bb()
        {
            Terms.Variable w = new Terms.Variable(new Shape((1, 3)));
            w.SetValue(new float[1, 3] {
                    { 2, 3, 1 }
                });

            Terms.Variable w2 = new Terms.Variable(new Shape((1, 3)));
            w2.SetValue(new float[1, 3] {
                    { 1, 1, 1 }
                });

            Terms.Add s = new Terms.Add(w, w2);


            Hyperparameters.LearningRate = 1;
            Console.WriteLine(s.GetResult());
            for (int i = 0; i < 1000; i++)
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
            Variable w1 = new Variable(new Dimension[] { 20, 10 }, new Shape((500, 300))); w1.Name = "w1";
            Variable w2 = new Variable(new Dimension[] { 20, 10 }, new Shape((500, 300))); w2.Name = "w2";
            Variable w3 = new Variable(new Dimension[] { 20, 10 }, new Shape((300, 400))); w3.Name = "w3";
            Variable w4 = new Variable(new Dimension[] { 20, 10 }, new Shape((500, 400))); w4.Name = "w4";

            var d1 = new Subtract(w1, w2); d1.Name = "d1";
            var d2 = new MatrixMultiply(d1, w3); d2.Name = "d2";
            var sum = new Power(new Subtract(d2, w4), 2); sum.Name = "sum";

            //Shape s = Shape.NewShape(2, 10);
            //Index a = Index.NewIndex(s);

            sum.PreCheck();
            //a.SetZero();
            //Console.WriteLine("w: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(w.GetTerm(a).GetResult());

            //a.SetZero();
            //Console.WriteLine("w2: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(w2.GetTerm(a).GetResult());

            //a.SetZero();
            //Console.WriteLine("sum: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(sum.GetTerm(a).GetResult());

            Hyperparameters.LearningRate = 0.02f;

            Stopwatch c = new Stopwatch();
            for (int i2 = 0; i2 < 10; i2++)
            {
                c.Restart();
                sum.Minimize();
                c.Stop();
                Console.WriteLine($"{i2} took {c.ElapsedMilliseconds}ms");
            }

            //sum.PreCheck();
            //a.SetZero();
            //Console.WriteLine("w: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(w.GetTerm(a).GetResult());

            //a.SetZero();
            //Console.WriteLine("w2: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(w2.GetTerm(a).GetResult());

            //a.SetZero();
            //Console.WriteLine("sum: ");
            //for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            //    Console.WriteLine(sum.GetTerm(a).GetResult());

            //Index.Return(a);
            //Shape.Return(s);
        }

        public static unsafe void bb3()
        {
            Variable w1 = new Variable(new Dimension[] { 2, 10 }, new Shape((1000, 3000))); w1.Name = "w1";
            Variable w2 = new Variable(new Dimension[] { 2, 10 }, new Shape((1000, 3000))); w2.Name = "w2";
            Variable w4 = new Variable(new Dimension[] { 2, 10 }, new Shape((1000, 3000))); w4.Name = "w4";

            var d1 = new Add(w1, w2); d1.Name = "d1";
            var sum = new Power(new Add(d1, w4), 2); sum.Name = "sum";


            Hyperparameters.LearningRate = 0.00f;

            Stopwatch c = new Stopwatch();
            for (int i2 = 0; i2 < 10; i2++)
            {
                c.Restart();
                sum.Minimize();
                c.Stop();
                Console.WriteLine($"{i2} took {c.ElapsedMilliseconds}ms");
            }
        }
        public static unsafe void bb4()
        {
            Input x = new Input(4, 2, 1);

            var sum = new ShiftTime(x, new Dimension[] { -1 }); sum.Name = "sum";


            Stopwatch c = new Stopwatch();
            c.Start();

            for (int i2 = 0; i2 < 1; i2++)
            {
                Tensor data = new Tensor((10, 3, 4), DataType.Type.Float, DeviceIndicator.Host());

                for (int i = 0; i < data.Shape.TotalSize; i++)
                    ((float*)data.Array)[i] = i / 12;
                x.SetInput(data);

                sum.PreCheck();

                Index a = new Index(x.OuterShape);
                a.SetZero();

                for (int i = 0; i < x.OuterShape.TotalSize; i++, a.Increase(1))
                {
                    Console.WriteLine("Term " + i + ":" + sum.GetTerm(a).GetResult());
                }

                c.Restart();
                sum.Minimize();
                c.Stop();
                data.Dispose();
                Console.WriteLine($"{i2} took {c.ElapsedMilliseconds}ms");
            }
        }

        public static unsafe void bb5()
        {
            Variable v = new Variable(new[] { new Dimension(3) }, new Shape((10, true)));

            v.PreCheck();

            Index a = new Index(v.OuterShape);
            a.SetZero();

            Terms.ReLU r = new Terms.ReLU(v.GetTerm(a));

            Console.WriteLine(v.GetTerm(a).GetResult());
            Console.WriteLine(r.GetResult());

            r.Dispose();

            ((Terms.Variable)v.GetTerm(a)).Clean();

            v.Dispose();
        }

        public static unsafe void bb6()
        {
            Variable v = new Variable(new Dimension[] { 6 }, new Shape((3,true)));

            DynamicRecurrent r = new DynamicRecurrent(v.OuterDimensions, v.InnerDimensions, new[] { v },
                (Layer me, List<Layer> x, Index t) =>
                {
                    if (t[0] % 2 == 1)
                        return new Terms.Add(x[0].GetTerm(t), me.GetTerm(t - 1));
                    else
                        return x[0].GetTerm(t);
                });

            r.PreCheck();


            Index a = new Index(r.OuterShape);
            a.SetZero();

            for (int i = 0; i < r.OuterShape.TotalSize; i++, a.Increase(1))
                Console.WriteLine(r.GetTerm(a).GetResult());
        }
        //todo term + layer gibi işlemlere izin vermelisin mi? Serialization sıkıntı çıkarır mı??  ? ? ?   ? ?   ? ? ? ?? ? 



        public static unsafe void Main(string[] args)
        {
            MNISTExample();
            //XORExample();
            return;
            //Thread t = new Thread(() => 
            //{ 
            //    while (true)
            //    { 
            //        Console.WriteLine(Process.GetCurrentProcess().Threads.Count); 
            //        Thread.Sleep(200);
            //    } 
            //});
            //t.IsBackground = true;
            //t.Start();


            
            for (int i = 0; i < 1; i++)
            {
                bb4();
            }

            
            Console.WriteLine(TensorPool.Host.UnreturnedArrayCount);
            //Thread.Sleep(10000);
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
