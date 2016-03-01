using SlowLibrary;
using System.ComponentModel;

namespace DataProcessor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BackgroundWorker backgroundWorker;

        #region Bindable Properties

        private int interation = 50;
        public int Iterations
        {
            get { return interation; }
            set
            {
                interation = value;
                RaisePropertyChanged(nameof(Iterations));
            }
        }

        private int progressPercentage = 0;
        public int ProgressPercentage
        {
            get { return progressPercentage; }
            set
            {
                progressPercentage = value;
                RaisePropertyChanged(nameof(ProgressPercentage));
            }
        }

        private string output = string.Empty;
        public string Output
        {
            get { return output; }
            set
            {
                output = value;
                RaisePropertyChanged(nameof(Output));
            }
        }

        private bool startEnabled = true;
        public bool StartEnabled
        {
            get { return startEnabled; }
            set
            {
                startEnabled = value;
                RaisePropertyChanged(nameof(StartEnabled));
            }
        }

        private bool cancelEnabled = false;
        public bool CancelEnabled
        {
            get { return cancelEnabled; }
            set
            {
                cancelEnabled = value;
                RaisePropertyChanged(nameof(CancelEnabled));
            }
        }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            backgroundWorker = new BackgroundWorker();

            // Background Process
            backgroundWorker.DoWork += worker_DoWork;
            backgroundWorker.RunWorkerCompleted += worker_RunWorkerCompleted;

            // Progress
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += worker_ProgressChanged;

            // Cancellation
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        #endregion

        #region Methods

        internal void StartProcess()
        {
            if (!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync(Iterations);

            StartEnabled = !backgroundWorker.IsBusy;
            CancelEnabled = backgroundWorker.IsBusy;
            Output = string.Empty;
        }

        internal void CancelProcess()
        {
            backgroundWorker.CancelAsync();
        }

        #endregion

        #region BackgroundWorker Events

        // Runs on Background Thread
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Exception in the background to see behavior
            // throw new Exception("Something bad happened");

            int result = 0;
            int iterations = (int)e.Argument;

            SlowProcessor processor = new SlowProcessor(iterations);
            foreach (var current in processor)
            {
                if (worker != null)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    if (worker.WorkerReportsProgress)
                    {
                        int percentageComplete =
                            (int)((float)current / (float)iterations * 100);
                        string progressMessage =
                            string.Format("Iteration {0} of {1}", current, iterations);
                        worker.ReportProgress(percentageComplete, progressMessage);
                    }
                }
                result = current;
            }

            e.Result = result;
        }

        // Runs on UI Thread
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Output = e.Error.Message;
            }
            else if (e.Cancelled)
            {
                Output = "Canceled";
            }
            else
            {
                Output = e.Result.ToString();
                ProgressPercentage = 0;
            }
            StartEnabled = !backgroundWorker.IsBusy;
            CancelEnabled = backgroundWorker.IsBusy;
        }

        // Runs on UI Thread
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
            Output = (string)e.UserState;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
