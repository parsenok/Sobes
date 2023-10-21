using System;
using System.IO;

namespace Sobes
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher watcher;
        public Form1()
        {
            InitializeComponent();
            InitializeFileWatcher();
        }

        private void InitializeFileWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = @"E:\Задачки к собесам\Sobes\Sobes";
            watcher.Filter = "test.txt";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;
        }
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            string filePath = Path.Combine(watcher.Path, watcher.Filter);

            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                UpdateTextBox(fileContents);
            }
        }
        private void UpdateTextBox(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action(() => textBox1.Text = text));
            }
            else
            {
                textBox1.Text = text;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}