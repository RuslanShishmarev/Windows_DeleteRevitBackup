using System.Windows;

namespace DeleteRevitBackup
{
    /// <summary>
    /// Логика взаимодействия для Rename.xaml
    /// </summary>
    public partial class Rename : Window
    {
        public string new_file_name;
        string old_name;
        public Rename(double t, double l, double h, double w, string name_text)
        {            
            InitializeComponent();
            this.Top = t + h/2 - this.Height/2;
            this.Left = l + w/2 - this.Width/2;
            new_name.Text = name_text;
            old_name = name_text;
        }
        private void ok_b_Click(object sender, RoutedEventArgs e)
        {
            new_file_name = new_name.Text;
            this.Close();            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new_file_name = new_name.Text;
        }
    }
}
