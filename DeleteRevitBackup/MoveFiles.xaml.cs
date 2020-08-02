using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace DeleteRevitBackup
{
    /// <summary>
    /// Логика взаимодействия для MoveFiles.xaml
    /// </summary>
    public partial class MoveFiles : Window
    {
        
        string[] files;
        string pathway;
        public MoveFiles(string main_path, string[] all_files, double t, double l, double h, double w)
        {
            InitializeComponent();
            this.Top = t + h / 2 - this.Height / 2;
            this.Left = l + w / 2 - this.Width / 2;
            files = all_files;
            pathway = main_path;

            List<string> formats = new List<string>();

            foreach (String f in files)
            {
                string format_name = Path.GetExtension(f).Replace(".", string.Empty).ToUpper();

                if (!formats.Contains(format_name)) formats.Add(format_name);
            }
             
            CheckBox box;
            foreach(string form in formats)
            {
                box = new CheckBox();
                box.Content = form;
                box.Margin = new System.Windows.Thickness { Left = 50, Top = 20, Right = 0, Bottom = 0 };
                scroll_items.Children.Add(box);
            }
            
        }
        private void ok_Click(object sender, RoutedEventArgs e)
        {
            //get all items in window
            var all_items = scroll_items.Children;
            //create the list for moved files
            string moved_files = "";
            foreach(CheckBox item in all_items)
            {                
                if (item.IsChecked == true)
                {
                    string newpath = pathway + "\\" + item.Content;

                    DirectoryInfo dirInfo = new DirectoryInfo(newpath);
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }

                    foreach (String f in files)
                    {
                        //get file's extension as capital letters
                        string format_file = Path.GetExtension(f).Replace(".", string.Empty).ToUpper();

                        //check all files. If file's extension = checked item, then move
                        if (item.Content.ToString() == format_file)
                        {
                            string file_name = Path.GetFileName(f);

                            File.Move(f, newpath + "\\" + file_name);
                            moved_files+=file_name + "\n";
                        }
                    }
                }
            }
            
            this.Close();
            MessageBox.Show("Готово");
        }
    }
}
