using Doge.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Doge.Model.InitialData;
using OfficeOpenXml;
using Doge.Model.RastrPart;
using Doge.Model.Calculation;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Doge.Model.Result;
using System.ComponentModel;

namespace Doge
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public InitialDataCreator InitialDataCreator { get; set; }
        public Calculator Calculator { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            InitialDataCreator = new InitialDataCreator();
            Calculator = new Calculator();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var g = new ObservableBranch();
            //g.Name = @"NAme11111";
            //g.BranchNumber = new BranchElement(1, 2, 3);
            //g.CurrentValues = new List<double>
            //{
            //    1231.23, 123213.43,4312431.4
            //};
            //var jsonString = JsonSerializer.Serialize(g);
            //File.WriteAllText(@"D:\1.txt", jsonString, Encoding.Default);
        }

        
        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void Window_PreviewDrop(object sender, DragEventArgs e)
        {
            var s = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (s == null) return;
            var rg2FileName = s.FirstOrDefault(k => Path.GetExtension(k) == @".rg2");
            var schFileName = s.FirstOrDefault(k => Path.GetExtension(k) == @".sch");
            var ut2FileName = s.FirstOrDefault(k => Path.GetExtension(k) == @".ut2");
            var xlsFileNames = s.Where(k => Path.GetExtension(k) == @".xlsx").ToList();

            if (rg2FileName != null)
                InitialDataCreator.Rg2FileName = rg2FileName;
            if (schFileName != null)
                InitialDataCreator.SchFileName = schFileName;
            if (ut2FileName != null)
                InitialDataCreator.Ut2FileName = ut2FileName;

            if (xlsFileNames.Count>0)
            {
                bool firstIsTask;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(xlsFileNames[0])))
                {
                    var sheet = package.Workbook.Worksheets[0];
                    var header = (sheet.Cells[2, 1].Value ?? @"").ToString();
                    firstIsTask = header.ToLower().Contains("ремонт");
                }
                if (firstIsTask)
                    InitialDataCreator.TaskFileName = xlsFileNames[0];
                else
                    InitialDataCreator.CurrentsFileName = xlsFileNames[0];
                if (xlsFileNames.Count == 1)
                    return;
                if (firstIsTask)
                    InitialDataCreator.CurrentsFileName = xlsFileNames[1];
                else
                    InitialDataCreator.TaskFileName = xlsFileNames[1];
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //InitialDataCreator.GetTaskInformation();
            //var calculator = new Calculator(InitialDataCreator);
            //calculator.Run();
            //InitialDataCreator.GetBranchElementsWithCurrentLimits();
            //var result = new Result(calculator, InitialDataCreator.BranchElementsWithCurrentLimits);
            //var options = new JsonSerializerOptions
            //{
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //    WriteIndented = true
            //};
            //var jsonString = JsonSerializer.Serialize(result,options);
            //File.WriteAllText(@"g:\Code\doge\Тестовый расчет\2\result.txt", jsonString, Encoding.Default);
            //jsonString = JsonSerializer.Serialize<Calculator>(calculator, options);
            //File.WriteAllText(@"g:\Code\doge\Тестовый расчет\2\calc.txt", jsonString, Encoding.Default);
            //result.SaveToExcel(@"g:\Code\doge\Тестовый расчет\2\calc.xlsx");
            //var calc = JsonSerializer.Deserialize<Calculator>(jsonString, options);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            InitialDataCreator.GetTaskInformation();
            Calculator = new Calculator(InitialDataCreator);

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Calculator.Run;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            TB1.Text = $@"{e.ProgressPercentage}%";
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitialDataCreator.GetBranchElementsWithCurrentLimits();
            var result = new Result(Calculator, InitialDataCreator.BranchElementsWithCurrentLimits);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
            var file = Calculator.Rg2FileName.Replace(@".rg2",@".xlsx");
            result.SaveToExcel(file);
            var jsonString = JsonSerializer.Serialize(result, options);
            File.WriteAllText(Calculator.Rg2FileName.Replace(@".rg2", @".txt"), jsonString, Encoding.Default);
            jsonString = JsonSerializer.Serialize(Calculator, options);
            File.WriteAllText(Calculator.Rg2FileName.Replace(@".rg2", @"2.txt"), jsonString, Encoding.Default);
            //var calc = JsonSerializer.Deserialize<Calculator>(jsonString, options);
            MessageBox.Show("!!!!");
        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var remonts = new List<Remont>();
            var file = @"g:\Code\doge\Тестовый расчет\3\Ремонты.xlsx";
            var file2 = @"g:\Code\doge\Тестовый расчет\3\Ремонты2.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(file)))
            {
                var sheet = package.Workbook.Worksheets[0];
                var counter = 1;

                while (sheet.Cells[counter,1].Value !=null)
                {
                    var remont = new Remont();
                    remont.Name = sheet.Cells[counter, 1].Value.ToString();
                    remont.Options = (sheet.Cells[counter, 2].Value ?? @"").ToString();
                    remont.Ip = sheet.Cells[counter, 3].Value.ToString();
                    remont.Iq = sheet.Cells[counter, 4].Value.ToString();
                    remont.Np = sheet.Cells[counter, 5].Value.ToString();
                    counter++;
                    remonts.Add(remont);
                }
                counter = 1;
                var sheet2 = package.Workbook.Worksheets[1];
                for (int i = 0; i < remonts.Count; i++)
                {
                    for (int j = i+1; j < remonts.Count; j++)
                    {
                        sheet2.Cells[counter, 1].Value = $@"Ремонт {remonts[i].Name} и {remonts[j].Name}";
                        sheet2.Cells[counter, 2].Value = $@"{remonts[i].Options} {remonts[j].Options}";
                        sheet2.Cells[counter, 3].Value = remonts[i].Ip;
                        sheet2.Cells[counter, 4].Value = remonts[i].Iq;
                        sheet2.Cells[counter, 5].Value = remonts[i].Np;
                        sheet2.Cells[counter+1, 3].Value = remonts[j].Ip;
                        sheet2.Cells[counter + 1, 4].Value = remonts[j].Iq;
                        sheet2.Cells[counter + 1, 5].Value = remonts[j].Np;
                        counter +=2;
                    }
                }
                package.SaveAs(new FileInfo(file2));
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            InitialDataCreator.GetTaskInformation();
            Calculator = new Calculator(InitialDataCreator);

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Calculator.Check;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }
    }

    public class  Remont
    {
        public string Name;
        public string Options;
        public string Ip;
        public string Iq;
        public string Np;
    }

}
