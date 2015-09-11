using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConvertToXNAContent
{
    class Program
    {
        static string dir = @"D:\Tech\WorkArea\Inlumino\Content\MainLevels\";
        static void Main(string[] args)
        {        
            Console.WriteLine("Run the script: (y/n)");
            if (Console.ReadLine() == "y")
            {
                foreach (string filename in Directory.GetFiles(dir))
                {
                    if (Path.GetFileName(filename).StartsWith("S_"))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(string));
                        Stream s = File.OpenRead(filename);
                        string temp = (string)xs.Deserialize(s);
                        s.Close(); s.Dispose();
                        using (XmlWriter writer = XmlWriter.Create(Path.GetDirectoryName(filename)+"\\ML"+Path.GetFileName(filename)))
                        {
                            IntermediateSerializer.Serialize<string>(writer, temp, null);
                        }
                        Console.WriteLine("File converted: " + filename);
                        if (!Directory.Exists(dir + "old")) Directory.CreateDirectory(dir + "old");
                        if (File.Exists(dir + "old\\" + Path.GetFileName(filename))) File.Delete(dir + "old\\" + Path.GetFileName(filename));
                        File.Move(filename, dir + "old\\" + Path.GetFileName(filename));
                    }
                    else
                        Console.WriteLine("File skipped: " + Path.GetFileName(filename));
                }            
            }
            Console.WriteLine("Terminated.");
            Console.Read();
        }
    }
}
