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
using Doge.Model.Rastr;

namespace Doge
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public InitialDataCreator InitialDataCreator { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            InitialDataCreator = new InitialDataCreator();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var g = new ObservableBranch();
            g.Name = @"NAme11111";
            g.BranchNumber = new BranchElement(1, 2, 3);
            g.CurrentValues = new List<double>
            {
                1231.23, 123213.43,4312431.4
            };
            var jsonString = JsonSerializer.Serialize(g);
            File.WriteAllText(@"D:\1.txt", jsonString, Encoding.Default);
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
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
            InitialDataCreator.GetTaskInformation();
        }
    }
}
