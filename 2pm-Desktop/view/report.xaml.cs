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

namespace _2pm_Desktop.view
{
    /// <summary>
    /// Interaction logic for report.xaml
    /// </summary>
    public partial class report : UserControl
    {
        private MainWindow window;

        public string id { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string input { get; set; }

        public report(MainWindow win)
        {
            InitializeComponent();
            this.window = win;
            DataContext = this;

        }

        private void getDataSave(object sender, MouseEventArgs e)
        {

            if (datainput.Text.Length > 50)
            {
                this.input = datainput.Text;
            }

            System.Diagnostics.Debug.WriteLine(datainput.Text.Length);
            System.Diagnostics.Debug.WriteLine(this.input);

        }

        private void getClear(object sender, MouseEventArgs e)
        {
            window.reportlabel.Content = this.title.ToString();
            if (datainput.Text.Equals("type here")) { datainput.Clear(); }
        }
    }
}
