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
using Inlumino_SHARED;

namespace ConvertToXNAContent
{
    class Program
    {
        //static string dir = @"D:\Tech\WorkArea\Inlumino\Content\MainLevels\";
        static string dir = @"D:\Tech\WorkArea\levels\Final\";
        static void Main(string[] args)
        {
        command:
            string cmd = Console.ReadLine();
            if (cmd == "conv") goto convert;
            else if (cmd == "ren") goto rename;
            else if (cmd == "hash") goto hash;

            convert:
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
                                /*
                                string[] data = temp.Split('$');
                                temp = data[0] + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[1])) + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[2])) + "|" + Encoding.ASCII.GetString(Convert.FromBase64String(data[3]));
                                temp = temp.Replace("#", ",");
                                temp = Inlumino_SHARED.SecurityProvider.Encrypt(temp);
                                */
                                using (XmlWriter writer = XmlWriter.Create(Path.GetDirectoryName(filename) + "\\ML" + Path.GetFileName(filename)))
                                {
                                    //xs.Serialize(writer, temp);
                                    IntermediateSerializer.Serialize<string>(writer, temp, null);
                                }
                                Console.WriteLine("File converted: " + filename);
                                if (!Directory.Exists(sub + "\\old")) Directory.CreateDirectory(sub + "\\old");
                                if (File.Exists(sub + "\\old\\" + Path.GetFileName(filename))) File.Delete(sub + "\\old\\" + Path.GetFileName(filename));
                                File.Move(filename, sub + "\\old\\" + Path.GetFileName(filename));
                            }
                            catch (Exception e) { if (s != null) s.Close(); Console.WriteLine("Error with " + filename + "\n" + e.Message); }
                        }
                    }
            }
            Console.WriteLine("Terminated.");
        rename:
            int c = 0;
            foreach (string sub in Directory.GetDirectories(dir))
            {
                Console.WriteLine("Init for: " + sub);
                string init = Console.ReadLine();
                string ar = "";
                foreach (string filename in Directory.GetFiles(sub))
                {
                    if (Path.GetFileName(filename).StartsWith("S_"))
                    {
                        string name = init + getstring(c);
                        Directory.CreateDirectory(Path.GetDirectoryName(filename) + "\\renamed");
                        File.Copy(filename, Path.GetDirectoryName(filename) + "\\renamed\\" + "S_" + name + ".xml", true);
                        ar += "\"" + name + "\",";
                        Console.WriteLine(Path.GetFileNameWithoutExtension(filename) + " ==> " + name);
                        c++;
                    }
                }
                StreamWriter sw = new StreamWriter(sub + "\\ar.txt");
                sw.Write(ar);
                sw.Close();
            }
            Console.WriteLine("Terminated.");
            goto command;
        hash:
            foreach (string sub in Directory.GetDirectories(dir))
            {
                string hashes = "";
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
                            string h = SecurityProvider.GetMD5Hash(temp);
                            hashes += "\"" + h + "\",";
                            Console.WriteLine(">>File: " + Path.GetFileName(filename) + "\nData: " + temp + "\nHash: " + h);
                        }
                        catch { Console.WriteLine("Failed with: " + filename); }
                    }
                }
                StreamWriter sw = new StreamWriter(sub + "\\hashes.txt");
                sw.WriteLine(hashes);
                sw.Close();
            }
            Console.WriteLine("Terminated.");
            goto command;
        }

        private static string getstring(int c)
        {
            string output = "";
            while (c >= 25)
            { c -= 25; output += "Z"; }
            output += (char)(65 + c);
            return output;
        }
    }
}
