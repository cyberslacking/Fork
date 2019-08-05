using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GMap.NET;


namespace MapToolkit
{
    public class MapManagerLoader
    {
        private static readonly MapManagerLoader _instance = new MapManagerLoader();
        private bool _isLoaded;

        public static MapManagerLoader Instance
        {
            get { return _instance; }
        }

        private MapManagerLoader()
        {
        }

        public bool Load(string fileName)
        {
            if (!_isLoaded)
            {
                new Thread(() => GMaps.Instance.ImportFromGMDB(fileName)).Start();
                _isLoaded = true;
            }
            return _isLoaded;
        }

        public bool Save(string fileName)
        {
            if (_isLoaded)
            {
                new Thread(() => GMaps.Instance.ExportToGMDB(fileName)).Start();
                return true;
            }
            return false;
        }
    }
}