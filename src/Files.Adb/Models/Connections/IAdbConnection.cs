using System;

namespace Files.Adb.Models.Connections
{
    public interface IAdbConnection
    {
        Uri GetConnectionUri();
        string GetAdbConnectionString();
    }
}