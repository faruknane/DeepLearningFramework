# DeepLearningFramework
Every expression is a term whose derivative can be calculated by the DeepLearningLibrary automatically. 

### Hyperparameters
Global, independent from model.
```csharp
Hyperparameters.LearningRate = 0.2f;
Hyperparameters.Optimizer = new SGD();
```

### Inputs/Arguments of Model
```csharp
PlaceHolder x = new PlaceHolder(2);
PlaceHolder y = new PlaceHolder(1);
```
### Creating a Model
```csharp
Term l1 = Layer(x, 2, true);
Term model = Layer(l1, 1, true);
```

### The loss/error function
```csharp
Term lossdiscrete = new Power(new Minus(model, y), 2);
Term loss = new ShrinkByAdding(lossdiscrete, lossdiscrete.D1, lossdiscrete.D2);
```
### Assign Input Values
```csharp
x.SetVariable(new Variable(2, 4)
{
    Weights = new float[2, 4]
    {{ 1, 1, 0, 0},
    { 1, 0, 1, 0}},
    Trainable = false
});

y.SetVariable(new Variable(1, 4)
{
    Weights = new float[1, 4] { { 0, 1, 1, 0 } },
    Trainable = false
});
```

### Training Process
Very simple If you manually assign inputs! Dot Minimize or Maximize whichever you want to use. 
```csharp
for (int epoch = 0; epoch < 1000; epoch++)
{
    loss.Minimize();
    Console.WriteLine("loss: " + loss.GetResult()[0]);
}
```

### Print Results
```csharp
loss.DeleteResults();
Console.WriteLine("Results: " + model.GetResult()[0] + ", " + model.GetResult()[1] + ", " + model.GetResult()[2] + ", " + model.GetResult()[3]);
```
