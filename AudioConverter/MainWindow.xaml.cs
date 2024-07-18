using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Lame;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using MathNet.Numerics.IntegralTransforms;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AudioConverter
{
    public partial class MainWindow
    {
        private string _inputFilePath = String.Empty;
        private string _outputFilePath = String.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _inputFilePath = openFileDialog.FileName;
                if (File.Exists(_inputFilePath))
                {
                    Converter converterWindow = new Converter(_inputFilePath);
                    converterWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
