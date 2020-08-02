using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using System.Media;

namespace DeleteRevitBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //create dictionary for differents formats
        Dictionary<string, string[]> formats = new Dictionary<string, string[]>(3);
        //format arrays
        string[] revit_formats = { ".rvt", ".rfa", ".rte", ".rft" };
        string[] autocad_formats = { ".dwg", ".bak", ".dxf" };
        string[] dmax_formats = { ".max", ".3ds", ".dwf" };
        string[] office_formats = { ".txt", ".doc", ".docx", "xls", ".xlsx", ".csv", ".pdf" };
        string[] dynamo_formats = {".dyn" };
        string[] all_formats = { };
        //default path
        static string pathway = @"C:\";
        //name for selected item in combobox
        string s_i = "";
        String[] arrayfiles = Directory.GetFiles(pathway);
        //array for all files 
        string[] files = new string[0];
        //
        string[] selected_format = new string[0];

        public MainWindow()
        {
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            
            InitializeComponent();
            format_method(formats);
        }
        
        private void format_method(Dictionary<string, string[]> d)
        {
            formats.Add("ALL", all_formats);
            formats.Add("Revit", revit_formats);
            formats.Add("AutoCAD", autocad_formats);
            formats.Add("Dynamo", dynamo_formats);
            formats.Add("3DsMax", dmax_formats);
            formats.Add("Office", office_formats);
            
            FormatsList.ItemsSource = d.Keys;
        }
        private void look_files(string[] files, Dictionary<string, string[]> dict, string[] sel_f)
        {
            foreach (String f in files)
            {
                string format = Path.GetExtension(f);
                if (dict.Keys.Contains(s_i) && sel_f.Count() != 0) 
                {
                    if (sel_f.Contains(format))
                        ViewBox.Items.Add(Path.GetFileName(f));
                }
                if (dict.Keys.Contains(s_i) && sel_f.Count() == 0)
                    ViewBox.Items.Add(Path.GetFileName(f));
            }
        }
        private string[] Selected_f(Dictionary<string, string[]> dict,string item)
        {
            string[] sel_format = new string[0];
            if (FormatsList.SelectedItem != null)
            {
                foreach (string k in dict.Keys)
                {
                    if (item == k)
                        sel_format = dict[k];
                }
            }
            return sel_format;
        }
        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            ViewBox.Items.Clear();
            if(FormatsList.SelectedItem != null)
                s_i = FormatsList.SelectedItem.ToString();

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = pathway;
            DialogResult result = fbd.ShowDialog();
            pathway = fbd.SelectedPath;

            if (!string.IsNullOrWhiteSpace(pathway))
            {
                files = Directory.GetFiles(pathway);
                PathName.Text = pathway;
            }
            if (s_i != "")
            {
                selected_format = Selected_f(formats, s_i);
                look_files(files, formats, selected_format);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> del_list = new List<string>();
            
            foreach (var item in ViewBox.SelectedItems)
            {
                del_list.Add(item.ToString());
            }
            foreach(string f in files)
            {
                string name = Path.GetFileName(f);
                if (del_list.Contains(name))
                    FileSystem.DeleteFile(f, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            ViewBox.Items.Clear();
            files = Directory.GetFiles(pathway);
            look_files(files, formats, selected_format);
            if (del_list.Count() != 0)
            {
                SystemSounds.Beep.Play();
                System.Windows.MessageBox.Show("Файлы удалены. Всего: " + del_list.Count());
            }
        }

        private void FormatsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewBox.Items.Clear();
            s_i = FormatsList.SelectedItem.ToString();
            selected_format = Selected_f(formats, s_i);
            files = Directory.GetFiles(pathway);
            look_files(files, formats, selected_format);            
        }
        private void FindBackRevit_Click(object sender, RoutedEventArgs e)
        {
            List<string> items_string = new List<string>();
            List<string> items_last = new List<string>();
            foreach (string l in ViewBox.Items)
            {
                if (revit_formats.Contains(Path.GetExtension(l)))
                {
                    //search the file name with 0000.revit
                    String[] words = l.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 2)
                    {
                        var last_word = words[words.Length - 2];
                        Regex reg = new Regex(@"\d");
                        int c = 0;
                        foreach (char w in last_word)
                        {
                            Match m = reg.Match(w.ToString());
                            if (m.Success)
                                c++;
                        }
                        if (c == 4 && last_word.Length == 4)
                        {
                            items_string.Add(l);
                            items_last.Add(last_word);
                            //select this items
                            ViewBox.SelectedItems.Add(l);
                        }
                    }
                }               
            }
            if (items_string.Count() == 0)
                System.Windows.MessageBox.Show("Нет резервных копий Revit");
        }
        private void RenameFile_Click(object sender, RoutedEventArgs e)
        {
            double windowTop = this.Top;
            double windowLeft = this.Left;
            double wH = this.Height;
            double wW = this.Width;
            if (ViewBox.SelectedItems.Count == 1)
            {
                string _name_t = ViewBox.SelectedItem.ToString();
                string name_t = Path.GetFileNameWithoutExtension(pathway + "\\" + _name_t);
                int i;
                Rename rn = new Rename(windowTop, windowLeft, wH, wW, name_t);
                rn.ShowDialog();
                if (ViewBox.SelectedIndex != -1)
                {
                    string file_n = ViewBox.SelectedItem.ToString();
                    char[] invalidFileChars = Path.GetInvalidFileNameChars();
                    
                    string new_name = rn.new_file_name + Path.GetExtension(file_n);
                    int index;
                    while ((index = new_name.IndexOfAny(Path.GetInvalidFileNameChars())) != -1)
                        new_name = new_name.Remove(index, 1);
                    if (file_n != new_name)
                    {
                        if (files.Contains(pathway + "\\" + new_name) == false)
                        {
                            i = ViewBox.SelectedIndex;
                            ViewBox.Items.RemoveAt(i);
                            ViewBox.Items.Insert(i, new_name);
                            File.Move(pathway + "\\" + file_n, pathway + "\\" + new_name);
                        }
                        else
                            System.Windows.MessageBox.Show("Имя занято. Придумайте новое");
                    }
                }
            }
        }
        private void MoveFiles_Click(object sender, RoutedEventArgs e)
        {
            double windowTop = this.Top;
            double windowLeft = this.Left;
            double wH = this.Height;
            double wW = this.Width;
            
            MoveFiles move = new MoveFiles(pathway, files, windowTop, windowLeft, wH, wW);
            move.ShowDialog();
        }
        private void Url_way(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe","http://infobim.ru/");
        }
    }
}
