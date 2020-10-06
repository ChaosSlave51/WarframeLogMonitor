using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WarframeLogMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public override void EndInit()
        {
            base.EndInit();
            DisplayArea.Document.Blocks.Clear();
            //InitialFileRead();
            //WatchFile();


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        long _position = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            //DisplayArea.Document.Blocks.Clear();
            var _fileStream = new FileStream(GetPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var _reader = new StreamReader(_fileStream))
            {
                _reader.BaseStream.Seek(_position, SeekOrigin.Begin);
                while (_reader.Peek() >= 0)
                {
                    var line = _reader.ReadLine();
                    ProcessLine(line);

                }
                _position = _reader.BaseStream.Position;
            }
        }

        private void AddLine(string s)
        {
            ColorAnimation animation = new ColorAnimation(Colors.Red, Colors.Black,new Duration(TimeSpan.FromSeconds(5)));
            var run = new Run(s);
            var paragraph = new Paragraph(run);
            paragraph.Foreground = new SolidColorBrush(Colors.Black);
            DisplayArea.Document.Blocks.Add(paragraph);
            paragraph.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, animation.CreateClock());

            DisplayArea.ScrollToEnd();

        }
       // private static FileStream _fileStream;
        ///private static StreamReader _reader;
        private void InitialFileRead()
        {
            var _fileStream = new FileStream(GetPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var _reader=new StreamReader(_fileStream))
            {
                while (_reader.Peek() >= 0)
                {
                    var line = _reader.ReadLine();
                    ProcessLine(line);

                }
            }
        }
        private static FileSystemWatcher watcher;
        private void WatchFile()
        {

            watcher = new FileSystemWatcher();
            watcher.Path = $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Warframe\\";
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;               ;
            watcher.Filter = "ee.log";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnFileChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            DisplayArea.Document.Blocks.Clear();
            var _fileStream = new FileStream(GetPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var _reader = new StreamReader(_fileStream))
            {
                while (_reader.Peek() >= 0)
                {
                    var line = _reader.ReadLine();
                    ProcessLine(line);

                }
            }
        }

        private void ProcessLine(string line)
        {
            if (line.Contains("killed by")|| line.Contains("downed by"))
            {
                AddLine(line);
            }
        }

        private string GetPath()
        {
            var userName = Environment.UserName;

            return $"C:\\Users\\{userName}\\AppData\\Local\\Warframe\\ee.log";
        } 
    }
}
