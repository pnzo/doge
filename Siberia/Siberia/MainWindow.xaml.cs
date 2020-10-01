using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace Siberia
{
    public class Generator
    {
        public string Name;
        public string Node;
        public string Unom;
        public double Pnom;
        public double P;
        public double Kef;
        public bool Vkl = false;

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
            var generators = new List<Generator>();
            for (int i = 0; i < dta.Length; i++)
            {
                if (dta[i].Contains(@"M2      "))
                {
                    var line = dta[i + 1];
                    var generator = new Generator();
                    generator.Name = line.Substring(0, 8).Trim();
                    generator.Node =line.Substring(9, 8).Trim();
                    generator.Unom = line.Substring(27, 8).Trim();
                    var line2 = dta[i + 4];
                    generator.Pnom = double.Parse(line2.Substring(63, 8).Trim().TrimEnd('.'));
                    generators.Add(generator);
                }
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets.Add(@"1111");

                var counter = 1;
                foreach (var generator in generators)
                {
                    firstWorksheet.Cells[counter, 1].Value = generator.Name;
                    firstWorksheet.Cells[counter, 2].Value = generator.Node;
                    firstWorksheet.Cells[counter, 3].Value = generator.Unom;
                    firstWorksheet.Cells[counter, 4].Value = generator.Pnom;
                    counter++;
                }
                excelPackage.SaveAs(new FileInfo($@"c:\Eurostag45\Siberia\res.xlsx"));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[0];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\01_2023zmax.dta", dta, Encoding.Default);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[1];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\02_2023lmax.dta", dta, Encoding.Default);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[2];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\03_2023lmin.dta", dta, Encoding.Default);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[3];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\04_2028zmax.dta", dta, Encoding.Default);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[4];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\05_2023lmax.dta", dta, Encoding.Default);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo($@"c:\Eurostag45\Siberia\исх.xlsx")))
            {
                var firstWorksheet = excelPackage.Workbook.Worksheets[5];
                var generators = new List<Generator>();
                var counter = 0;
                while (firstWorksheet.Cells[counter + 1, 1].Value != null)
                {
                    counter++;
                    if (firstWorksheet.Cells[counter, 5].Value == null)
                        continue;
                    var generator = new Generator();
                    generator.Name = firstWorksheet.Cells[counter, 5].Value.ToString();
                    generator.Pnom = double.Parse(firstWorksheet.Cells[counter, 6].Value.ToString());
                    generator.P = double.Parse(firstWorksheet.Cells[counter, 4].Value.ToString());
                    generator.Node = firstWorksheet.Cells[counter, 3].Value.ToString();
                    if (firstWorksheet.Cells[counter, 1].Value.ToString() == @"False")
                        generator.Vkl = true;
                    if (generator.Vkl == false)
                    {
                        generator.P = 0;
                    }
                    generators.Add(generator);
                }


                var nodes = new List<string>();
                foreach (var generator in generators)
                {
                    if (!nodes.Contains(generator.Node))
                        nodes.Add(generator.Node);
                }

                foreach (var node in nodes)
                {
                    var subgens = generators.Where(k => k.Node == node).ToList();
                    if (subgens.Count == 1)
                        subgens[0].Kef = 1;
                    if (subgens.Count > 1)
                    {
                        var sum = subgens.Sum(k => k.P);
                        if (sum == 0)
                            sum = 1;
                        foreach (var subgen in subgens)
                        {
                            subgen.Kef = subgen.P / sum;
                        }
                    }
                }
                var dta = File.ReadAllLines(@"c:\Eurostag45\Siberia\DTA_korr_3.dta", Encoding.Default);
                foreach (var generator in generators)
                {
                    var line = @"";
                    for (int i = 0; i < dta.Length; i++)
                    {
                        if (dta[i].Contains(generator.Name) && !dta[i].Contains(@"R "))
                        {
                            line = dta[i];
                            var l1 = line.Substring(0, 36);
                            var l2 = line.Substring(54);
                            var k1 = generator.Kef.ToString("f3").PadRight(9);
                            dta[i] = l1 + k1 + k1 + l2;
                        }
                    }
                }
                File.WriteAllLines($@"c:\Eurostag45\Siberia\06_2028lmin.dta", dta, Encoding.Default);
            }
        }
    }
}
