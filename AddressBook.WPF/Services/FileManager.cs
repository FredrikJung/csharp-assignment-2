using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBook.WPF.Services
{
    public interface IFileManager
    {
        // Interface som visar vilka metoder min FileManager har.
        public void Save(string filePath, string json);
        public string Read(string filePath);
    }
    public class FileManager : IFileManager
    {
        //Metod för att läsa in min fil från sökvägen
        public string Read(string filePath)
        {
            using var sr = new StreamReader(filePath);
            return  sr.ReadToEnd();
        }

        //Metod för att spara min fil till sökvägen
        public void Save(string filePath, string json)
        {
            using var sw = new StreamWriter(filePath);
            sw.WriteLine(json);
        }
    }
}
