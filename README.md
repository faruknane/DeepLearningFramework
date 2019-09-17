# DeepLearningFramework
Every expression is a term whose derivative can be calculated by the DeepLearningFramework automatically. Every Layer is consist of Terms which corresponds to sequential input. An example of XOR model is shown below.

### Hyperparameters
Global, independent from model.
```csharp
Hyperparameters.LearningRate = 0.2f;
Hyperparameters.Optimizer = new SGD();
```

### Inputs/Arguments of Model
```csharp
var x = new Input(2);
var y = new Input(1);
```
### Creating a Model
```csharp
var l1 = Layer.Dense(2, x, "sigmoid"); 
var model = Layer.Dense(1, l1, "sigmoid"); 
```

### The loss/error function
```csharp
var loss = Layer.SquaredError(model, y);
```

### Training Process
Very simple If you manually assign inputs! Dot Minimize or Maximize whichever you want to use. 
```csharp
for (int epoch = 0; epoch < 1000; epoch++)
{
    x.SetSequenceLength(1);
    y.SetSequenceLength(1);
    x.SetInput(0, new float[2, 4] { { 1, 1, 0, 0 }, { 1, 0, 1, 0 } } );
    y.SetInput(0, new float[1, 4] { { 0, 1, 1, 0 } } );
    
    loss.Minimize();
    //Console.WriteLine("loss: " + loss.GetTerm(0).GetResult()[0]);
}
```

### Print Results
```csharp
loss.DeleteTerms();
var result = model.GetTerm(0).GetResult();
Console.WriteLine("Results: " + result[0] + ", " + result[1] + ", " + result[2] + ", " + result[3]);
```

To support me: [My Patreon](https://www.patreon.com/afaruknane)
