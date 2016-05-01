using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DT_Algorithm;
using System.IO;
using NBc;
using static NBc.NBC;

namespace CART
{
    class Program
    {
        static void Main(string[] args)
        {
            Tuple<SortedDictionary<char, double>, SortedDictionary<MyPair<char, char>, double>> classifier;
            var nbc = new NBC();

            {
                List<Tuple<string, char>> names = new List<Tuple<string, char>>();

                var lines = File.ReadAllLines("name.txt");
                foreach (var line in lines)
                {
                    var tt = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    names.Add(new Tuple<string, char>(tt[0], tt[1][0]));
                }

                classifier = nbc.Train(names);
            }
            {
                List<Tuple<string, char>> names = new List<Tuple<string, char>>();

                var lines = File.ReadAllLines("name2.txt");
                foreach (var line in lines)
                {
                    var tt = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    names.Add(new Tuple<string, char>(tt[0], tt[1][0]));
                }

                int total = names.Count;
                int right = 0;
                foreach (var pp in names)
                {
                    string init = pp.Item1;
                    char cls = pp.Item2;

                    char res = nbc.Classify(classifier, nbc.GetFeatures(init));
                    if (res == cls)
                        right++;
                    Console.WriteLine($"{init} : {res} vs {cls}");
                }
                Console.WriteLine($"RESULT: {100.0 * right/total}");

                Console.ReadLine();
            }

            {
                Tree tree = new Tree();
                DataSet dataSet = new DataSet();


                var lines = File.ReadAllLines("input.txt");

                var ht = lines[0].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var hn = lines[1].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);


                if (hn.Length != ht.Length)
                {
                    throw new ArgumentException("azaza");
                }

                foreach (var line in lines.Skip(2))
                {
                    var item = new Item();

                    var values = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != ht.Length)
                    {
                        throw new ArgumentException("azaza2");
                    }

                    for (int i = 0; i < values.Length; i++)
                    {
                        string name = hn[i];
                        string value = values[i];
                        AttributeType type;
                        if (ht[i] == "c") type = AttributeType.CategoricalNominal;
                        else type = AttributeType.Numerical;

                        item.Attributes[name] = new MyAttribute(name, value, type);
                    }

                    dataSet.Items.Add(item);
                }

                tree.Data = dataSet;
                tree.ClassificationAttributeName = hn.Last();
                tree.Build();
                //var gini = Utility.Gini(dataSet, hn.Last());

                Console.WriteLine();
            }
        }
    }
}
