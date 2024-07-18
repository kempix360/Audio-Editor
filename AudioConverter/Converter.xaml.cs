using Microsoft.Win32;
using NAudio.Wave;
//using ScottPlot;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenTK.Graphics.OpenGL;

namespace AudioConverter
{
    public partial class Converter : Window
    {
        private string _inputFilePath;
        private double[] _audioValues;
        private string _outputFilePath = string.Empty;
        private AudioFileReader _audioFileReader;

        public Converter(string inputFilePath)
        {
            InitializeComponent();
            _inputFilePath = inputFilePath;
            LoadAudioFile(_inputFilePath);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _inputFilePath = openFileDialog.FileName;
                LoadAudioFile(_inputFilePath);
            }
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                _outputFilePath = saveFileDialog.FileName;
            }
        }

        private void LoadAudioFile(string filePath)
        {
            _audioFileReader = new AudioFileReader(filePath);

            var samples = _audioFileReader.Length / (_audioFileReader.WaveFormat.Channels * _audioFileReader.WaveFormat.BitsPerSample / 8);
            double f = 0.0f;
            double max = 0.0f;
            // waveform will be a maximum of 4000 pixels wide:
            int batch = (int)Math.Max(40, samples / 4000);
            int mid = 100;
            int yScale = 100;
            float[] buffer = new float[batch];
            int read;
            int xPos = 0;
            
            while((read = _audioFileReader.Read(buffer,0,batch)) == batch)
            {
                for(int n = 0; n < read; n++)
                {
                    max = Math.Max(Math.Abs(buffer[n]), max);
                }
                var line = new Line();
                line.X1 = xPos;
                line.X2 = xPos;
                line.Y1 = mid + (max * yScale); 
                line.Y2 = mid - (max * yScale);
                line.StrokeThickness = 1.5;
                line.Stroke = Brushes.GreenYellow;
                FrequencyCanvas.Children.Add(line);
                max = 0;
                xPos++;
            }
            FrequencyCanvas.Width = xPos;
            FrequencyCanvas.Height = mid * 2 + 40; // 40 is a margin for x axis labels
            DrawAxisX(samples / (double)_audioFileReader.WaveFormat.SampleRate);
        }

        private void DrawAxisX(double totalDurationSeconds)
        {
            int bottomMargin = 15;
            int tickIntervalSeconds = 10;
            // Draw X axis (time)
            var xAxis = new Line
            {
                X1 = 0,
                Y1 = FrequencyCanvas.Height - bottomMargin - 20,
                X2 = FrequencyCanvas.Width,
                Y2 = FrequencyCanvas.Height - bottomMargin - 20,
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            FrequencyCanvas.Children.Add(xAxis);
            
            int numberOfLabels = (int)(totalDurationSeconds / tickIntervalSeconds);
            // Add time labels on X axis
            for (int i = 0; i < numberOfLabels; i++)
            {
                double xPos = i * FrequencyCanvas.Width / numberOfLabels;
                double time = i * tickIntervalSeconds;
                string timeLabel = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
                
                double xTickPos;
                if (i == 0)
                    xTickPos = xPos - 22.5;
                else if (i == numberOfLabels - 1)
                    xTickPos = xPos - 12.5;
                else
                    xTickPos = xPos;
                
                var textBlock = new TextBlock
                {
                    Text = timeLabel,
                    Foreground = Brushes.White
                };
                Canvas.SetLeft(textBlock, xTickPos);
                Canvas.SetTop(textBlock, FrequencyCanvas.Height - bottomMargin - 10); // Position below the X axis
                FrequencyCanvas.Children.Add(textBlock);
                
                var minorLine = new Line
                {
                    X1 = xPos,
                    Y1 = FrequencyCanvas.Height - 20 - bottomMargin + 5,
                    X2 = xPos,
                    Y2 = FrequencyCanvas.Height - 20 - bottomMargin - 5,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.5
                };
                FrequencyCanvas.Children.Add(minorLine);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }
        

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
