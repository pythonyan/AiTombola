using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using iTombola.Models;
using iTombola.Services;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;


namespace iTombola
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private readonly VideoCapture capture;
        private readonly CascadeClassifier cascadeClassifier;

        private readonly BackgroundWorker bkgWorker;

        private readonly ITombolaService tombolaService;

        private readonly DialectsService dialectsService;

        public MainWindow(ITombolaService tombolaService, DialectsService dialectsService)
        {
            InitializeComponent();

            capture = new VideoCapture();
            cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

            bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            bkgWorker.DoWork += Worker_DoWork;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            this.tombolaService = tombolaService;

            this.dialectsService = dialectsService;
        }


        private async void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            capture.Open(0, VideoCaptureAPIs.ANY);
            if (!capture.IsOpened())
            {
                Close();
                return;
            }

            bkgWorker.RunWorkerAsync();
            txtStatus.Text = string.Empty;

            cmbLanguage.ItemsSource = await dialectsService.LoadAsync();
            cmbLanguage.SelectedIndex = 0;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bkgWorker.CancelAsync();

            capture.Dispose();
            cascadeClassifier.Dispose();
        }

        private int SecondsBetweenImageRetrieve = 10;

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                using (var frameMat = capture.RetrieveMat())
                {
                    Dispatcher.Invoke(() =>
                    {
                        FrameImage.Source = frameMat.ToWriteableBitmap();
                    });
                }
                await Task.Delay(30);
            }
        }

        private SoundPlayer soundPlayer = new SoundPlayer();

        private async Task AnalyzeImageAsync()
        {
            var dialect = (Dialect)cmbLanguage.SelectedItem;

            txtStatus.Text = "Analisi immagine in corso";
            using var memStream = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)FrameImage.Source));
            encoder.Save(memStream);

            memStream.Position = 0;

            var result = await this.tombolaService.ExtractNumberFromStream(memStream,
                dialect.Culture, dialect.DialectName, dialect.VoiceName, 0.90);

            if (result.Numbers.Any())
            {

                var number = result.Numbers.First();
                txtStatus.Text = $"Ultimo numero riconosciuto: {number.Number}";
                try
                {
                    PlayWavFile(number.NumberAudioFilePath);
                    PlayWavFile(number.DescriptionAudioFilePath);
                }
                catch (System.Exception ex)
                {

                }
            }
            else
            {
                txtStatus.Text = $"Nessun numero riconosciuto!";
            }
        }

        private void PlayWavFile(string filePath)
        {
            using (var audioFile = File.OpenRead(filePath))
            {
                soundPlayer.Stream = audioFile;
                soundPlayer.PlaySync();
            }
        }

        private async void btnScatta_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await AnalyzeImageAsync();
        }
    }
}
