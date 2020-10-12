using Doge.Model.InitialData;
using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Doge.Model.Calculation
{

    public class Calculator : INotifyPropertyChanged
    {
        public List<Repair> Repairs { get; set; }
        public string Rg2FileName { get; set; }
        public string Ut2FileName { get; set; }
        public string SchFileName { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Calculator()
        {

        }
        public Calculator(InitialDataCreator initialDataCreator)
        {
            Repairs = new List<Repair>();
            Rg2FileName = initialDataCreator.Rg2FileName;
            Ut2FileName = initialDataCreator.Ut2FileName;
            SchFileName = initialDataCreator.SchFileName;
            foreach (var taskRepair in initialDataCreator.TaskRepairs)
            {
                var repair = new Repair();
                repair.Name = taskRepair.Name;
                repair.SwitchedElements = taskRepair.SwitchedElements;
                repair.DeltaP = taskRepair.deltaP ?? initialDataCreator.deltaP;
                var firstAccident = new Accident();
                firstAccident.Name = @"";
                foreach (var observableBranch in initialDataCreator.ObservableBranches)
                {
                    var ip = observableBranch.BranchElement.Ip;
                    var iq = observableBranch.BranchElement.Iq;
                    var np = observableBranch.BranchElement.Np;
                    var name = observableBranch.Name;
                    var newObservableBranch = new ObservableBranch(ip, iq, np, name);
                    firstAccident.ObservableBranches.Add(newObservableBranch);
                }
                repair.Accidents.Add(firstAccident);
                foreach (var taskAccident in initialDataCreator.TaskAccidents)
                {
                    if (taskRepair.ExcludeAccidentsLabels.Contains(taskAccident.Label))
                        continue;
                    if (taskAccident.IsContainsInRepair(taskRepair))
                        continue;
                    if ((taskAccident.Label ?? @"").StartsWith(@"$") && !taskRepair.IncludeAccidentsLabels.Contains(taskAccident.Label))
                        continue;

                    var accident = new Accident();
                    accident.SwitchedElements = taskAccident.SwitchedElements;
                    accident.Name = taskAccident.Name;
                    foreach (var observableBranch in initialDataCreator.ObservableBranches)
                    {
                        var ip = observableBranch.BranchElement.Ip;
                        var iq = observableBranch.BranchElement.Iq;
                        var np = observableBranch.BranchElement.Np;
                        var name = observableBranch.Name;
                        var newObservableBranch = new ObservableBranch(ip, iq, np, name);
                        accident.ObservableBranches.Add(newObservableBranch);
                    }
                    repair.Accidents.Add(accident);
                }
                Repairs.Add(repair);
            }

        }

        public void Run(object sender, DoWorkEventArgs e)
        {
            var counter = 0;
            foreach (var repair in Repairs)
                foreach (var accident in repair.Accidents)
                    counter++;
            var progress = 0.0;
            var progressStep =100.0 / (double)counter;
            foreach (var repair in Repairs)
            {
                foreach (var accident in repair.Accidents)
                {

                    var rastr = new RastrOperations();
                    rastr.Load(Rg2FileName, Ut2FileName, SchFileName);
                    foreach (var switchedElement in repair.SwitchedElements)
                        rastr.ApplyRastrElement(switchedElement);
                    foreach (var switchedElement in accident.SwitchedElements)
                        rastr.ApplyRastrElement(switchedElement);

                    

                    var code = rastr.InitializeLoading();
                    while (code)
                    {
                        accident.AddValuesFromCurrentStep(rastr);
                        code = rastr.PerformStep();
                    }
                    accident.AddValuesFromCurrentStep(rastr);
                    var result = $@"{repair.Name} {accident.Name}";
                    progress += progressStep;
                    (sender as BackgroundWorker).ReportProgress((int)progress);
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                };
                var jsonString = JsonSerializer.Serialize(this, options);
                File.WriteAllText(Rg2FileName.Replace(@".rg2", @"_calc.txt"), jsonString, Encoding.Default);

            }

        }

        public void Check(object sender, DoWorkEventArgs e)
        {
            var list = new List<string>();
            var counter = 0;
            foreach (var repair in Repairs)
                foreach (var accident in repair.Accidents)
                    counter++;
            var progress = 0.0;
            var progressStep = 100.0 / (double)counter;
            foreach (var repair in Repairs)
            {
                foreach (var accident in repair.Accidents)
                {

                    var rastr = new RastrOperations();
                    rastr.Load(Rg2FileName, Ut2FileName, SchFileName);
                    foreach (var switchedElement in repair.SwitchedElements)
                        rastr.ApplyRastrElement(switchedElement);
                    foreach (var switchedElement in accident.SwitchedElements)
                        rastr.ApplyRastrElement(switchedElement);
                    var code = rastr.InitializeLoading();
                    if (code==false)
                    {
                        list.Add($@"Неустойчиво {repair.Name} {accident.Name}");
                    }
                    progress += progressStep;
                    (sender as BackgroundWorker).ReportProgress((int)progress);
                }
                File.WriteAllLines(Rg2FileName.Replace(@".rg2", @"ошибки.txt"), list, Encoding.Default);
            }

        }
    }
}
