using DeepLearningFramework.Core;
using DeepLearningFramework.Core.Optimizers;
using DeepLearningFramework.Operators.Layers;
using PerformanceWork;
using PerformanceWork.OptimizedNumerics;
using PerformanceWork.OptimizedNumerics.Pool;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Index = PerformanceWork.OptimizedNumerics.Index;
using Terms = DeepLearningFramework.Operators.Terms;

namespace Tests
{
    class Program
    {
        public static void PrintPools()
        {
            Console.WriteLine("TensorPool.UnreturnedArrayCount: " + TensorPool.Host.UnreturnedArrayCount);
            Console.WriteLine("ShapeArrayPool.UnreturnedArrayCount: " + Shape.ArrayPool.UnreturnedArrayCount);
            Console.WriteLine("ShapeObjectPool.UnreturnedArrayCount: " + Shape.ObjectPool.UnreturnedCount);
            Console.WriteLine("IndexArrayPool.UnreturnedArrayCount: " + Index.ArrayPool.UnreturnedArrayCount);
            Console.WriteLine("IndexObjectPool.UnreturnedArrayCount: " + Index.ObjectPool.UnreturnedCount);
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
            float[, ] labels = new float[l1, l3];
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
            Hyperparameters.LearningRate = 0.0001f;
            Hyperparameters.Optimizer = new SGD();


            //Model Creation
            var x = new Input(784);
            var model = Layer.Dense(500, x, "relu");
            model = Layer.Dense(200, model, "relu");
            model = Layer.Dense(100, model, "relu");
            model = Layer.Dense(10, model, "softmax");


            //Loss Function Creation
            var y = new Input(10);
            var loss = Layer.SquaredError(model, y);


            //Data preparation
            (float[,] traindata, float[, ] labels) = LoadMNISTDataSet();
            int mnistsize = 42000;

            Tensor x_train, y_train;

            fixed (float* xptr = traindata, yptr = labels)
            {
               x_train = Tensor.LoadFloatArrayToTensorHost(xptr, 0, mnistsize * 784, Shape.NewShape(mnistsize, 784));
               y_train = Tensor.LoadFloatArrayToTensorHost(yptr, 0, mnistsize * 10, Shape.NewShape(mnistsize, 10));
            }

            //Training
            int batchsize = 100;
            Shape shapebatchx = Shape.NewShape(1, batchsize, 784);
            Shape shapebatchy = Shape.NewShape(1, batchsize, 10);

            int trainl = 35000;

            Stopwatch s = new Stopwatch();

            for (int epoch = 0; epoch < 15; epoch++)
            {
                float l = 0;
                float val = 0;

                s.Restart();
                Console.WriteLine("Epoch " + epoch + " başladı.");
                for (int batch = 0; batch < trainl / batchsize; batch++)
                {
                    Tensor batchx = Tensor.Cut(x_train, batch * (batchsize * 784), (batch + 1) * (batchsize * 784), shapebatchx);
                    Tensor batchy = Tensor.Cut(y_train, batch * (batchsize * 10), (batch + 1) * (batchsize * 10), shapebatchy);
                    
                    x.SetInput(batchx);
                    y.SetInput(batchy);

                    loss.Minimize();

                    Index zero = Index.NewIndex(loss.OuterShape);
                    zero.SetZero();

                    Tensor res = loss.GetTerm(zero).GetResult();
                    float* pp = (float*)res.Array;

                    for (int i = 0; i < res.Shape.TotalSize; i++)
                        l += pp[i];

                    Index.Return(zero);

                }

                for (int batch = trainl / batchsize; batch < mnistsize / batchsize; batch++)
                {
                    Tensor batchx = Tensor.Cut(x_train, batch * (batchsize * 784), (batch + 1) * (batchsize * 784), shapebatchx);
                    Tensor batchy = Tensor.Cut(y_train, batch * (batchsize * 10), (batch + 1) * (batchsize * 10), shapebatchy);

                    model.DeleteTerms();

                    x.SetInput(batchx);
                    y.SetInput(batchy);

                    Index zero = Index.NewIndex(model.OuterShape);
                    zero.SetZero();
                    model.PreCheck();
                    Tensor res = model.GetTerm(zero).GetResult();
                    Index.Return(zero);

                    for(int i = 0; i < batchsize; i++)
                    {
                        int myans = MaxId((float*)res.Array + i*10);
                        int correctres = MaxId((float*)batchy.Array + i*10);
                        val += (myans == correctres ? 1 : 0);
                    }
                }
                s.Stop();

                Console.WriteLine("Epoch " + epoch + " biti.");
                Console.WriteLine("Loss: " + l/trainl);
                Console.WriteLine("Validation: " + val/(mnistsize - trainl));
                Console.WriteLine("Time: " + s.ElapsedMilliseconds  + "ms");
            }

            PrintPools();

            shapebatchx.Dispose();
            shapebatchy.Dispose();


            Shape testx = Shape.NewShape(1, 1, 784);

            while (true)
            {
                try
                {
                    float[] data = LoadCurrentImage();
                    Tensor x_test;

                    fixed (float* ptr = data)
                        x_test = Tensor.LoadFloatArrayToTensorHost(ptr, 0, 784, testx);

                    model.DeleteTerms();

                    x.SetInput(x_test);

                    Index zero = Index.NewIndex(model.OuterShape);
                    zero.SetZero();
                    model.PreCheck();
                    Tensor res = model.GetTerm(zero).GetResult();
                    Index.Return(zero);

                   

                    Console.WriteLine("Result: " + res);
                    Console.WriteLine("Digit Prediction: " + MaxId((float*)res.Array));
                    Console.WriteLine("-----------");
                }
                catch (Exception)
                {

                }
                Thread.Sleep(500);
            }
            Shape.Return(testx);
        }

        public unsafe static void XORExample()
        {
            //Hyperparameters
            Hyperparameters.LearningRate = 0.01f;
            Hyperparameters.Optimizer = new SGD();

            //Model Creation
            var x = new Input(2);
            var model = Layer.Dense(100, x, "relu");
            model = Layer.Dense(1, model, "sigmoid");


            //Loss Function Creation
            var y = new Input(1);
            var loss = Layer.SquaredError(model, y);


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
            Index a = Index.NewIndex(model.OuterShape);
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

        }
       
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

        public static unsafe void Print(Tensor x)
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
            Variable w1 = new Variable(new Dimension[] { 20, 10 }, Shape.NewShape(500, 300)); w1.Name = "w1";
            Variable w2 = new Variable(new Dimension[] { 20, 10 }, Shape.NewShape(500, 300)); w2.Name = "w2";
            Variable w3 = new Variable(new Dimension[] { 20, 10 }, Shape.NewShape(300, 400)); w3.Name = "w3";
            Variable w4 = new Variable(new Dimension[] { 20, 10 }, Shape.NewShape(500, 400)); w4.Name = "w4";

            var d1 = new Subtract(w1, w2); d1.Name = "d1";
            var d2 = new MatrixMultiply(d1, w3); d2.Name = "d2";
            var sum = new Power(new Subtract(d2,w4),2); sum.Name = "sum";

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
            Variable w1 = new Variable(new Dimension[] { 2, 10 }, Shape.NewShape(1000, 3000)); w1.Name = "w1";
            Variable w2 = new Variable(new Dimension[] { 2, 10 }, Shape.NewShape(1000, 3000)); w2.Name = "w2";
            Variable w4 = new Variable(new Dimension[] { 2, 10 }, Shape.NewShape(1000, 3000)); w4.Name = "w4";

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
            Variable w1 = new Variable(new Dimension[] { 10 }, Shape.NewShape(3, 4)); w1.Name = "w1";

            var sum = new Add(w1, x); sum.Name = "sum";


            Stopwatch c = new Stopwatch();
            c.Start();

            for (int i2 = 0; i2 < 100; i2++)
            {
                Tensor data = new Tensor((10, 3, 4), DataType.Type.Float, DeviceIndicator.Host());

                for (int i = 0; i < data.Shape.TotalSize; i++)
                    ((float*)data.Array)[i] = i / 12;
                x.SetInput(data);

                x.PreCheck();
                
                Index a = Index.NewIndex(x.OuterShape);
                a.SetZero();
                
                for (int i = 0; i < x.OuterShape.TotalSize; i++, a.Add(1))
                {
                    Console.WriteLine("Term " + i + ":" + x.GetTerm(a).GetResult());
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
            Variable v = new Variable(new[] { new Dimension(3) }, Shape.NewShape(10));

            v.PreCheck();

            Index a = Index.NewIndex(v.OuterShape);
            a.SetZero();

            Terms.ReLU r = new Terms.ReLU(v.GetTerm(a));

            Console.WriteLine(v.GetTerm(a).GetResult());
            Console.WriteLine(r.GetResult());

            r.Dispose();

            ((Terms.Variable)v.GetTerm(a)).Clean();

            Index.Return(a);
            v.Dispose();
        }

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


            int s1 = Shape.ArrayPool.UnreturnedArrayCount;

            for (int i = 0; i < 1; i++)
            {
                bb4();
            }

            s1 = Shape.ArrayPool.UnreturnedArrayCount;
            Console.WriteLine(Shape.ObjectPool.UnreturnedCount);
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
