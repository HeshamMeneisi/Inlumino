using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Input;
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
        //static string dir = @"D:\Tech\WorkArea\Inlumino\Content\MainLevels\";
        static string dir = @"D:\Tech\WorkArea\levels\Inlumino1\";
        static void Main(string[] args)
        {
            Console.WriteLine("Run the script: (y/n)");
            if (Console.ReadLine() == "y")
            {
                foreach (string sub in Directory.GetDirectories(dir))
                    foreach (string filename in Directory.GetFiles(sub))
                    {
                        if (Path.GetFileName(filename).StartsWith("S_"))
                        {
                            Stream s = null;
                            try
                            {
                                s = File.OpenRead(filename);
                                XmlSerializer xs = new XmlSerializer(typeof(string));
                                string temp = (string)xs.Deserialize(s);
                                s.Close(); s.Dispose();
                                ///
                                string[] data = temp.Split('$');
                                temp = data[0] + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[1])) + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[2])) + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[3]));
                                temp = temp.Replace("#", ",");
                                temp = Inlumino_SHARED.SecurityProvider.Encrypt(temp);
                                ///
                                using (XmlWriter writer = XmlWriter.Create(Path.GetDirectoryName(filename) + "\\ML" + Path.GetFileName(filename)))
                                {
                                    xs.Serialize(writer, temp);
                                    //IntermediateSerializer.Serialize<string>(writer, temp, null);
                                }
                                Console.WriteLine("File converted: " + filename);
                                if (!Directory.Exists(sub + "\\old")) Directory.CreateDirectory(sub + "\\old");
                                if (File.Exists(sub + "\\old\\" + Path.GetFileName(filename))) File.Delete(sub + "\\old\\" + Path.GetFileName(filename));
                                File.Move(filename, sub + "\\old\\" + Path.GetFileName(filename));
                            }
                            catch (Exception e) { if (s != null) s.Close(); Console.WriteLine("Error with " + filename + "\n" + e.Message); }
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
