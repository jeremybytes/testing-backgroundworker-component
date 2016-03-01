using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TestHelpers
{
    public class PropertyChangeTracker : IDisposable
    {
        private List<string> notifications = new List<string>();
        INotifyPropertyChanged changeObject;

        public PropertyChangeTracker(INotifyPropertyChanged changer)
        {
            changeObject = changer;
            changeObject.PropertyChanged += TrackProperties;
        }

        private void TrackProperties(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
                notifications.Add("**ALL**");
            else
                notifications.Add(e.PropertyName);
        }

        public string[] ChangedProperties
        {
            get { return notifications.ToArray(); }
        }

        public int ChangeCount(string propertyName)
        {
            return notifications.Count(p => p == propertyName);
        }

        public bool WaitForChange(string propertyName, int maxWaitMilliseconds)
        {
            var startTime = DateTime.UtcNow;
            while (!notifications.Contains(propertyName) &&
                   !notifications.Contains("**ALL**"))
            {
                if (startTime.AddMilliseconds(maxWaitMilliseconds) < DateTime.UtcNow)
                    return false;
            }
            return true;
        }

        public bool WaitForChange(string propertyName, TimeSpan maxWait)
        {
            var startTime = DateTime.UtcNow;
            while (!notifications.Contains(propertyName) &&
                   !notifications.Contains("**ALL**"))
            {
                if (startTime + maxWait < DateTime.UtcNow)
                    return false;
            }
            return true;
        }

        public void Reset()
        {
            notifications.Clear();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    changeObject.PropertyChanged -= TrackProperties;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
