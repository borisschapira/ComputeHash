using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Diagnostics;

namespace ComputeHash
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Use a StopWatch for execution monitoring
            Stopwatch sw = Stopwatch.StartNew();

            // File extensions to look for in the directory
            string lookfor = "*.gif;*.jpg;*.jpeg;*.png";
            string[] extensions = lookfor.Split(new char[] { ';' });

            // Number of duplicate files in the directory
            int duplicate = 0;

            // Definition of the folder
            DirectoryInfo di = null;
            if (args.Length > 0)
            {
                try
                {
                    di = new DirectoryInfo(args[0]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid folder URI");
                    sw.Stop();
                    Console.WriteLine("Time used (rounded): {0} ms", sw.ElapsedMilliseconds);
                    return;
                }

            }

            if (di == null)
                di = new DirectoryInfo(Directory.GetCurrentDirectory());

            List<FileInfo> rgFiles;
            Dictionary<String, String> hashDic;
            Dictionary<String, List<String>> bilan = new Dictionary<string, List<string>>();


            rgFiles = new List<FileInfo>();
            hashDic = new Dictionary<string, string>();
            rgFiles.Clear();

            foreach (string currentExt in extensions)
            {
                rgFiles.AddRange(di.GetFiles(currentExt, SearchOption.AllDirectories));

                using (HashAlgorithm hashAlg = new SHA1Managed())
                {
                    foreach (FileInfo currentFile in rgFiles)
                    {
                        using (Stream file = new FileStream(currentFile.FullName, FileMode.Open, FileAccess.Read))
                        {
                            string hash = BitConverter.ToString(hashAlg.ComputeHash(file));

                            if (!hashDic.ContainsKey(hash))
                            {
                                hashDic.Add(hash, currentFile.Name);
                                //Console.WriteLine(currentFile.Name + " : " + hash);
                            }
                            else
                            {
                                if (bilan.ContainsKey(hashDic[hash]))
                                {
                                    bilan[hashDic[hash]].Add(currentFile.Name);
                                }
                                else
                                {
                                    bilan.Add(hashDic[hash], new List<string>() {currentFile.Name});
                                }
                            }
                        }
                    }
                }
            }


            foreach (var itemBilan in bilan)
            {
                Debug.WriteLine(itemBilan.Key + " : " + itemBilan.Value.Count + " occurence(s)");
                duplicate += itemBilan.Value.Count;
                foreach (var itemOccurence in itemBilan.Value)
                {
                    Debug.WriteLine("\t=> " + itemOccurence);
                }
            }

            if (duplicate > 0)
            {
                Console.WriteLine("===> " + duplicate + " duplicates out of " + rgFiles.Count + " images");
                Debug.WriteLine("===> " + duplicate + " duplicates out of " + rgFiles.Count + " images");
            }
            else
            {
                Console.WriteLine("===> No duplicates");
                Debug.WriteLine("===> No duplicates");
                Console.WriteLine();
            }
            
            sw.Stop();
            Console.WriteLine("Time used (rounded): {0} ms", sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }

    public static class DictionnaryExtensions
    {
        public static TKey FindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value)) return pair.Key;

            throw new Exception("The value is not found in the dictionary");
        }
    }
}