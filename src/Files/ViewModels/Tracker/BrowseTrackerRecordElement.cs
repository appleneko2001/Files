using System;

namespace Files.ViewModels.Tracker
{
    public class BrowseTrackerRecordElement
    {
        private Uri _uri;
        public Uri Uri => _uri;

        public BrowseTrackerRecordElement(Uri uri)
        {
            _uri = uri;
        }

        public override string ToString()
        {
            return _uri.ToString();
        }
    }
}