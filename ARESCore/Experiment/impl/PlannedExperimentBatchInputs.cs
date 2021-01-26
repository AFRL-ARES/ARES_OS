using ARESCore.DisposePatternHelpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARESCore.Experiment.impl
{
  public class PlannedExperimentBatchInputs : ReactiveSubscriber, IPlannedExperimentBatchInputs
  {
    private List<IPlannedExperimentInputs> _inputs = new List<IPlannedExperimentInputs>();

    public int Count() { return _inputs.Count; }
    public bool HasInputs() { return (_inputs != null && _inputs.Count > 0); }
    public virtual double GetInput(string desc, int expInd)
    {
      // Make sure the inputs exists
      if (_inputs.Count - 1 < expInd)
        throw new Exception("Inputs for the experiment number " + expInd + " does not exist!");

      var experimentInputs = _inputs[expInd - 1];

      // If the description does not match return
      if (!experimentInputs.Inputs.ContainsKey(desc))
        throw new Exception("Column description " + desc + " does not exist!");

      return (experimentInputs.Inputs[desc]);
    }


    public IPlannedExperimentInputs GetExperimentInputs(int expNum)
    {
      if (!HasInputs())
        throw new Exception("There are no planned experiment inputs");

      var index = expNum - 1;
      if (index > _inputs.Count)
        index = _inputs.Count - 1;

      return _inputs[index];
    }

    public virtual void SetExperimentBatchInputs(List<string> dataDesc, List<List<double>> data)
    {
      // Ensure that the dataDesc are valid
      if (dataDesc == null || dataDesc.Count == 0)
        throw new Exception("Inputs descriptions cannot be null or empty!");

      // Ensure that the inputs is valid
      if (data == null || data.Count == 0)
        throw new Exception("Inputs cannot be null or empty!");

      // Ensure there is a valid inputs piece for each column for each experiment
      data.ForEach(anExpData =>
     {
       if (anExpData.Count != dataDesc.Count)
         throw new Exception("Every experiment must have a inputs entry for each column!");

       anExpData.ForEach(aData =>
       {
         if (double.IsInfinity(aData) || double.IsNaN(aData))
           throw new Exception("Cannot have NaN or an Infinity inputs value!");
       });
     });

      if (HasInputs()) // add planned inputs to current batch Inputs
      {
        for (var index = 0; index < data.Count; index++)
        {
          PlannedInputs[index].SetInputs(dataDesc, data[index]);
        }
      }
      else // add experiments
      {
        foreach (var experimentData in data)
        {
          PlannedInputs.Add(new PlannedExperimentInputs(dataDesc, experimentData));
        }

      }
    }



    public List<IPlannedExperimentInputs> PlannedInputs
    {
      get => _inputs;
      protected set => this.RaiseAndSetIfChanged(ref _inputs, value);
    }

    public virtual void LoadInputsFromFile(string fileName, char delim = ',')
    {
      // Read all lines from the data file
      List<string> dataFileLines;
      try
      { dataFileLines = File.ReadAllLines(fileName).ToList(); }
      catch (Exception ex) { throw new Exception("Error reading experiment data file!", ex); }

      // Make sure the data file has data!
      if (dataFileLines == null || dataFileLines.Count < 2)
        throw new Exception("Could not read any data from file!");

      // Create a useful Func to split lines
      Func<string, List<string>> TokenizeLine = new Func<string, List<string>>((line) =>
     {
       return line.Trim().Split(new[] { delim }, StringSplitOptions.RemoveEmptyEntries).ToList();
     });

      // Tokenize the first line of the file
      List<string> firstLineTokens = TokenizeLine(dataFileLines.First());

      // Ensure the validity of the data descriptions then remove them from the list
      firstLineTokens.ForEach(desc =>
     {
       if (desc == null || desc.Trim().Equals(""))
         throw new Exception("Data descriptions in file cannot be null or empty!");
     });
      dataFileLines.RemoveAt(0);

      // Tokenize each line and parse to doubles
      int expNum = 1; // 1 based index
      List<List<double>> data = new List<List<double>>();
      dataFileLines.ForEach(expDataLine =>
     {
       // Tokenize the line and check the validity of it
       expNum += 1;
       List<string> expLineTokens = TokenizeLine(expDataLine);
       if (expLineTokens == null || expLineTokens.Count != firstLineTokens.Count)
         throw new Exception("Experiment " + expNum + " does not have enough tokens in line!");

       // Parse the tokens to double and check the validities
       int tokenNum = 0; // 1 based index
       List<double> expData = new List<double>();
       expLineTokens.ForEach(dataToken =>
       {
         tokenNum += 1;
         if (dataToken == null || dataToken.Trim().Equals(""))
           throw new Exception("Experiment " + expNum + ", column " + tokenNum + " data is null or empty!");

         double dataVal;
         try
         { dataVal = Convert.ToDouble(dataToken); }
         catch (Exception)
         {
           throw new Exception("Experiment " + expNum + ", column " + tokenNum + " data cannot be parsed to a double!");
         }

         if (double.IsInfinity(dataVal) || double.IsNaN(dataVal))
           throw new Exception("Experiment " + expNum + ", column " + tokenNum + " data cannot be NaN or an Infinity!");

         // This token is good data
         expData.Add(dataVal);
       });

       // This line is a good experiment
       data.Add(expData);
     });
      var descs = firstLineTokens;

      for (int i = 0; i < data.Count; i++)
      {
        var input = new PlannedExperimentInputs();
        for (var index = 0; index < descs.Count; index++)
        {
          var desc = descs[index];
          input.Inputs[desc] = data[i][index];
        }
        PlannedInputs.Add(input);
      }
    }
  }
}
