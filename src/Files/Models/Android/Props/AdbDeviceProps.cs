using System.Collections.Generic;

namespace Files.Models.Android.Props
{
    public class AdbDeviceProps
    {
        private Dictionary<string, string> _props;

        public AdbDeviceProps()
        {
            _props = new Dictionary<string, string>();
        }
        
        public void AddOrUpdate(string key, string value)
        {
            if (_props.ContainsKey(key))
            {
                _props[key] = value;
            }
            else
            {
                _props.Add(key, value);
            }
        }
        
        public void AddOrUpdate(KeyValuePair<string, string> pair)
        {
            AddOrUpdate(pair.Key, pair.Value);
        }
        
        public void AddOrUpdate(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            foreach (var pair in pairs)
            {
                AddOrUpdate(pair);
            }
        }
        
        // Unnecessary method, but it's here for completeness on other libraries
        // yep on my own adb wrapper library :P
        public IDictionary<string, bool> GetRunningServices()
        {
            var services = new Dictionary<string, bool>();
            foreach (var service in _props.Keys)
            {
                if (service.StartsWith("init.svc."))
                {
                    services.Add(service[9..], _props[service] == "running");
                }
            }
            return services;
        }
        
        public int? GetSdkVersion()
        {
            if(_props.TryGetValue("ro.build.version.sdk", out var sdkVersion))
            {
                return int.Parse(sdkVersion);
            }
            return null;
        }
        
        public string? GetBrand() => _props.TryGetValue("ro.product.brand", out var brand) ? brand : null;
        public string? GetModel() => _props.TryGetValue("ro.product.model", out var model) ? model : null;
        
        public string? GetBrandModel()
        {
            return $"{GetBrand()} {GetModel()}";
        }
        
    }
}