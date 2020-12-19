using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACOperationalTestLib
{
    public class OperationalTestResultFile
    {
        public OperationalTestResultFile()
        {
            Name = String.Empty;
            Type = String.Empty;
            eContent = String.Empty;
        }
        public string Type { get; set; }
        public string Name { get; set; }
        public string eContent { get; set; }
    }
}
