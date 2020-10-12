using Doge.Model.Calculation;
using Doge.Model.RastrPart;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.InitialData
{
    public class InitialDataCreator : INotifyPropertyChanged
    {
        private string rg2FileName;
        private string schFileName;
        private string ut2FileName;
        private string taskFileName;
        private string currentsFileName;


        public string Rg2FileName
        {
            get { return rg2FileName; }
            set
            {
                rg2FileName = value;
                OnPropertyChanged("Rg2FileName");
            }
        }
        public string SchFileName
        {
            get { return schFileName; }
            set
            {
                schFileName = value;
                OnPropertyChanged("SchFileName");
            }
        }
        public string Ut2FileName
        {
            get { return ut2FileName; }
            set
            {
                ut2FileName = value;
                OnPropertyChanged("Ut2FileName");
            }
        }
        public string TaskFileName
        {
            get { return taskFileName; }
            set
            {
                taskFileName = value;
                OnPropertyChanged("TaskFileName");
            }
        }
        public string CurrentsFileName
        {
            get { return currentsFileName; }
            set
            {
                currentsFileName = value;
                OnPropertyChanged("CurrentsFileName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public List<ObservableBranch> ObservableBranches { get; set; }
        public List<TaskRepair> TaskRepairs { get; set; }
        public List<TaskAccident> TaskAccidents { get; set; }
        public List<BranchElementWithCurrentLimits> BranchElementsWithCurrentLimits { get; set; }
        public string OptionsString { get; set; }
        public double deltaP
        {
            get
            {
                var parts = OptionsString.Split(' ').Where(k => k.StartsWith(@"p")).ToList();
                if (parts.Count() == 0)
                    return 0;
                return double.Parse(parts.FirstOrDefault().TrimStart('p'));
            }
        }

        public InitialDataCreator()
        {
            TaskRepairs = new List<TaskRepair>();
            TaskAccidents = new List<TaskAccident>();
            ObservableBranches = new List<ObservableBranch>();
            BranchElementsWithCurrentLimits = new List<BranchElementWithCurrentLimits>();
        }

        public void GetTaskInformation()
        {
            TaskRepairs = new List<TaskRepair>();
            TaskAccidents = new List<TaskAccident>();
            ObservableBranches = new List<ObservableBranch>();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(taskFileName)))
            {
                var sheet = package.Workbook.Worksheets[0];
                var counter = 4;

                OptionsString = (sheet.Cells[1, 1].Value ?? @"").ToString();
                // ремонты
                TaskRepair taskRepair = null;
                while (sheet.Cells[counter, 3].Value != null)
                {
                    if (sheet.Cells[counter, 1].Value != null)
                    {
                        if (taskRepair != null)
                            TaskRepairs.Add(taskRepair);
                        taskRepair = new TaskRepair();
                        taskRepair.Name = sheet.Cells[counter, 1].Value.ToString();
                        taskRepair.OptionsString = (sheet.Cells[counter, 2].Value ?? @"").ToString();
                    }
                    RastrElement rastrElement;
                    if (sheet.Cells[counter, 4].Value != null)
                    {
                        var ip = int.Parse(sheet.Cells[counter, 3].Value.ToString());
                        var iq = int.Parse(sheet.Cells[counter, 4].Value.ToString());
                        var np = int.Parse((sheet.Cells[counter, 5].Value ?? @"0").ToString());
                        rastrElement = new RastrElement(ip, iq, np);
                    }
                    else
                    {
                        var ny = int.Parse(sheet.Cells[counter, 3].Value.ToString());
                        rastrElement = new RastrElement(ny);
                    }
                    rastrElement.State = (sheet.Cells[counter, 6].Value ?? @"0").ToString().Trim() != @"0"; // если 0 или пусто то false, если что-то не 0 то true
                    taskRepair.SwitchedElements.Add(rastrElement);
                    counter++;
                }
                if (taskRepair != null)
                    TaskRepairs.Add(taskRepair);

                // ПАРы
                counter = 4;
                TaskAccident taskAccident = null;
                while (sheet.Cells[counter, 9].Value != null)
                {
                    if (sheet.Cells[counter, 8].Value != null)
                    {
                        if (taskAccident != null)
                            TaskAccidents.Add(taskAccident);
                        taskAccident = new TaskAccident();
                        taskAccident.Name = sheet.Cells[counter, 8].Value.ToString();
                        if (sheet.Cells[counter, 7].Value == null)
                            taskAccident.Label = null;
                        else
                            taskAccident.Label = sheet.Cells[counter, 7].Value.ToString();
                    }
                    RastrElement rastrElement;
                    if (sheet.Cells[counter, 10].Value != null)
                    {
                        var ip = int.Parse(sheet.Cells[counter, 9].Value.ToString());
                        var iq = int.Parse(sheet.Cells[counter, 10].Value.ToString());
                        var np = int.Parse((sheet.Cells[counter, 11].Value ?? @"0").ToString());
                        rastrElement = new RastrElement(ip, iq, np);
                    }
                    else
                    {
                        var ny = int.Parse(sheet.Cells[counter, 9].Value.ToString());
                        rastrElement = new RastrElement(ny);
                    }
                    rastrElement.State = (sheet.Cells[counter, 12].Value ?? @"0").ToString().Trim() != @"0"; // если 0 или пусто то false, если что-то не 0 то true
                    taskAccident.SwitchedElements.Add(rastrElement);
                    counter++;
                }
                if (taskAccident != null)
                    TaskAccidents.Add(taskAccident);

            }

            using (var package = new ExcelPackage(new FileInfo(currentsFileName)))
            {
                var sheet = package.Workbook.Worksheets[0];
                var counter = 3;
                ObservableBranch observableBranch = null;
                while (sheet.Cells[counter, 5].Value != null)
                {
                    if (sheet.Cells[counter, 1].Value != null)
                    {
                        if (observableBranch != null)
                            ObservableBranches.Add(observableBranch);
                        var name = sheet.Cells[counter, 1].Value.ToString();
                        var ip = int.Parse(sheet.Cells[counter, 2].Value.ToString());
                        var iq = int.Parse(sheet.Cells[counter, 3].Value.ToString());
                        var np = int.Parse((sheet.Cells[counter, 4].Value ?? @"0").ToString());
                        observableBranch = new ObservableBranch(ip, iq, np, name);
                    }
                    counter++;
                }
                if (observableBranch != null)
                    ObservableBranches.Add(observableBranch);
            }
        }

        public void GetBranchElementsWithCurrentLimits()
        {
            BranchElementsWithCurrentLimits = new List<BranchElementWithCurrentLimits>();
            using (var package = new ExcelPackage(new FileInfo(currentsFileName)))
            {
                var sheet = package.Workbook.Worksheets[0];
                var counter = 3;
                while (sheet.Cells[counter, 5].Value != null)
                {
                    if (sheet.Cells[counter, 1].Value != null)
                    {
                        var ip = int.Parse(sheet.Cells[counter, 2].Value.ToString());
                        var iq = int.Parse(sheet.Cells[counter, 3].Value.ToString());
                        var np = int.Parse((sheet.Cells[counter, 4].Value ?? @"0").ToString());
                        var name = sheet.Cells[counter, 1].Value.ToString();
                        var branchElementWithCurrentLimit = new BranchElementWithCurrentLimits(ip, iq, np);
                        branchElementWithCurrentLimit.Name = name;
                        var columnCounter = 5;
                        while (sheet.Cells[counter, columnCounter].Value != null)
                        {
                            var currentLimit = new CurrentLimit();
                            currentLimit.Temperature = int.Parse(sheet.Cells[2, columnCounter].Value.ToString());
                            currentLimit.LongTermCurrentLimit = double.Parse(sheet.Cells[counter, columnCounter].Value.ToString());
                            currentLimit.ShortTermCurrentLimit = double.Parse(sheet.Cells[counter + 1, columnCounter].Value.ToString());
                            branchElementWithCurrentLimit.CurrentLimits.Add(currentLimit);
                            columnCounter++;
                        }
                        BranchElementsWithCurrentLimits.Add(branchElementWithCurrentLimit);
                    }
                    counter++;
                }
            }
        }
    }
}
