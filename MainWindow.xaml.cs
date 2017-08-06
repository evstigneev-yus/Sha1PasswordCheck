using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using Microsoft.Win32;

namespace sha1CheckSum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _dictionaryPath = "";
        string[] _originals;
        string[] _originalsHashes;
        List<string> _foundedList;
        
        private void SetProgressBarText(string str)
        {
            ProgressBarText.Text = str;
        }
        public MainWindow()
        {
            InitializeComponent();
            _foundedList=new List<string>();
            ProgressBar.Visibility=Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Open
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FileTextBox.Text = openFileDialog.FileName;
                _originals=File.ReadAllLines(openFileDialog.FileName);
                _originalsHashes = _originals.Select(item => GetHash(item).ToUpper()).ToArray();
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string GetHash(string original)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(original);
            var encodedBytes = sha1.ComputeHash(originalBytes);
            return System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(encodedBytes), "-", "").ToLower();
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Find
            ProgressBar.Visibility = Visibility.Visible;
            IsEnabled = false;
            await Task.Factory.StartNew(ReadTxtFile);
            ProgressBar.Visibility=Visibility.Hidden;
            IsEnabled = true;
            FoundenOriginals.ItemsSource = _foundedList;
        }
        private void ReadTxtFile()
        {
            
            if (string.IsNullOrEmpty(_dictionaryPath)) 
                return;
            using (var sr = new StreamReader(_dictionaryPath))
            {
                string line;
                //uint cnt = 0;
                while ((line = sr.ReadLine()) != null)
                {

                    if (_originalsHashes.Contains(line))
                    {
                        //SetProgressBarText(String.Format("Обработано паролей: {0}",cnt));
                        var i = _originalsHashes.ToList().IndexOf(line);
                        _foundedList.Add(_originals[i]);
                        
                        if (_foundedList.Count==_originals.Length)
                        {
                            return;
                        }
                        //cnt++;
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //OpenDictionary
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _dictionaryPath = openFileDialog.FileName;
                dictionaryPathTextBob.Text = _dictionaryPath;
            }
        }
    }
}
