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
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Doge
{
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
            var g = new ObservableBranch();
            g.Name = @"NAme11111";
            g.BranchNumber = new BranchNumber(1, 2, 3);
            g.CurrentValues = new List<double>
            {
                1231.23, 123213.43,4312431.4
            };
            var jsonString = JsonSerializer.Serialize(g);
            File.WriteAllText(@"D:\1.txt", jsonString, Encoding.Default);
        }
    }
}
