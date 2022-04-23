using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace Files.Adb.Models
{
    public class AdbDeviceModel
    {
        private AdbConnection _connection;
        private string _status;
        private IReadOnlyDictionary<string, string> _properties;

        public AdbConnection Connection => _connection;
        
        public string Model => _properties["model"];
        public bool IsReady => _status == "device";
        public bool IsUnauthorized => _status == "unauthorized";

        // ReSharper disable once MemberCanBePrivate.Global
        public static AdbDeviceModel Parse(string line)
        {
            using (var reader = new StringReader(line))
            {
                string? serialOrIpAddress = null;
                string? status = null;

                string? propertyName = null;
                string? propertyValue = null;
                
                var properties = new Dictionary<string, string>();

                var catchMode = 0;
                
                var startCatching = false;
                
                var builder = new StringBuilder();

                void SetNoneMode()
                {
                    catchMode = 0;
                    startCatching = false;
                }

                void SetCatchSerialMode()
                {
                    catchMode = 1;
                    startCatching = false;
                }

                void SetCatchStatusMode()
                {
                    catchMode = 2;
                    startCatching = false;
                }

                void SetCatchPropNameMode()
                {
                    catchMode = 3;
                    startCatching = false;
                }

                void SetCatchPropValueMode()
                {
                    catchMode = 4;
                    startCatching = false;
                }

                void Append(char? c)
                {
                    builder.Append(c);
                    startCatching = true;
                }

                string GetString()
                {
                    var str = builder.ToString();
                    builder.Clear();
                    return str;
                }

                bool GetNextChar(out char? c)
                {
                    c = null;
                    
                    var r = reader.Read();

                    if (r < 0)
                        return false;

                    c = (char) r;
                    return true;
                }

                SetCatchSerialMode();

                while (GetNextChar(out var c))
                {
                    switch (catchMode)
                    {
                        case 0: // in none mode
                        {
                            if(c == ' ')
                                if(!startCatching)
                                    continue;
                            
                            SetCatchPropNameMode();
                            Append(c);
                        } break;

                        case 1: // in catch serial or ip address mode, append serial or ip to builder
                        {
                            if(c == ' ')
                            {
                                if(!startCatching)
                                    continue;

                                serialOrIpAddress = GetString();
                                SetCatchStatusMode();
                                continue;
                            }
                            
                            Append(c);
                        } break;
                        
                        case 2: // in catch status mode, append status to builder and set none mode when reach to space
                        {
                            if(c == ' ')
                            {
                                if(!startCatching)
                                    continue;
                                
                                status = GetString();
                                SetNoneMode();
                                continue;
                            }
                            
                            Append(c);
                        } break;
                        
                        case 3: // in catch property name mode, append property name to builder
                        {
                            if(c == ':')
                            {
                                if(!startCatching)
                                    continue;
                                
                                SetCatchPropValueMode();
                                propertyName = GetString().ToLowerInvariant();
                                continue;
                            }
                            
                            Append(c);
                        } break;
                        case 4: // in catch property value mode, append value to builder
                        {
                            if(c == ' ')
                            {
                                if(!startCatching)
                                    continue;
                                
                                SetNoneMode();
                                propertyValue = GetString();
                                properties.Add(propertyName!, propertyValue);
                                continue;
                            }
                            
                            Append(c);
                        } break;
                    }
                }

                // Possible to have a line without a space at the end
                // ReSharper disable once InvertIf
                if (builder.Length > 0)
                {
                    propertyValue = GetString();
                    properties.Add(propertyName!, propertyValue);
                }

                return new AdbDeviceModel
                {
                    _properties = properties.ToImmutableDictionary(),
                    _connection = new AdbConnection(serialOrIpAddress!),
                    _status = status?.ToLowerInvariant()
                };
            }
        }
        
        public static bool TryParse(string line, out AdbDeviceModel? deviceInfo)
        {
            deviceInfo = null;
            
            if (string.IsNullOrWhiteSpace(line))
                return false;
            
            try
            {
                deviceInfo = Parse(line);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}