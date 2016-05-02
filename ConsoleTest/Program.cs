using C4_5;
using ID3;
using KNN;
using Main;
using NBC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NDesk.Options;
using System.IO;

namespace ConsoleTest
{
    public class Test
    {
        public ITree Tree { get; set; }

        public DataSet Full { get; set; } = new DataSet();

        public DataSet Init { get; set; } = new DataSet();
        public DataSet CrossValidate { get; set; } = new DataSet();
        public DataSet Controll { get; set; } = new DataSet();

        public Dictionary<string, double> Results = new Dictionary<string, double>();

        public void Prepare(string file)
        {
            Results.Clear();

            Full = Tree.Parse(file);
            //Full.Items = Full.Items.Take(6).ToList();
            Init = new DataSet();
            CrossValidate = new DataSet();
            Controll = new DataSet();

            int partCnt = Full.Items.Count / 3;
            this.Init.Items = Full.Items.Take(partCnt).ToList();
            this.CrossValidate.Items = Full.Items.Skip(partCnt).Take(partCnt).ToList();
            this.Controll.Items = Full.Items.Skip(partCnt + partCnt).ToList();
        }
    }


    class Program
    {
        static object ConsoleWriterLock = new object();

        static void Main(string[] args)
        {
            List<string> _files = new List<string>();
            List<string> _parametres = new List<string>();
            bool _isPruning = false;
            bool _crossValidate = false;
            bool _allAtttibutes = false;
            List<string> _attributesList = new List<string>();
            string _attribute = null;


            string currentParameter = null;
            OptionSet options = new OptionSet()
            {
                {"f|file", "a list of files" , v => {
                    currentParameter = "f";
                }},
                {"a", "attributes", v => {
                    currentParameter = "a";
                }},
                {"p", "pruning", v => {
                    _isPruning = true;
                }},
                {"c", "cross validate", v => {
                    _crossValidate = true;
                }},
                {"aa", "all attribute", v => {
                    _allAtttibutes = true;
                }},
                { "<>", v => {
                    switch(currentParameter) {
                        case "a":
                            _attributesList.Add(v.ToLower());
                            break;
                        case "f":
                            _files.Add(v);
                            break;
                    }
                }}
            };
            options.Parse(args);

            Console.ReadLine();
            var files = _files;

            var sw = Stopwatch.StartNew();


            int start = Console.CursorTop + 1;
            List<Task> tasks = new List<Task>();

            #region
            foreach (var fileName in files)
            {
                var tests = new List<Test>();
                //tests.Add(new Test() { Tree = new C4_5Tree() });
                tests.Add(new Test() { Tree = new ID3Tree() });
                //tests.Add(new Test() { Tree = new KNNTree() });
                //tests.Add(new Test() { Tree = new NBCTree() });

                Console.WriteLine($"File: {fileName}");

                tests.ForEach(t => t.Prepare(fileName));

                List<string> fullList = new List<string>();

                double total = 0;
                double cur = 0;

                foreach (var test in tests)
                {
                    test.Prepare(fileName);
                    var _attributes = test.Full.GetAttributeDict();
                    fullList = fullList.Union(_attributes.Keys).ToList();

                    total += _attributes.Count;
                }

                int curR = start;
                Console.CursorTop = curR + 1;

                string[][] matrix = Clear(fullList, tests);
                Print(matrix);
                Console.WriteLine();

                #region
                foreach (var test in tests)
                {
                    Task task = new Task(() =>
                    {
                        List<string> _attributes = new List<string>();
                        List<string> _allAttributes = test.Full.GetAttributeList();
                        List<string> _errorAttributes = new List<string>();

                        if (_allAtttibutes)
                        {
                            _attributes = _allAttributes;
                        }
                        else
                        {
                            foreach (var att in _attributesList)
                            {
                                if (_allAttributes.Contains(att))
                                {
                                    _attributes.Add(att);
                                }
                                else
                                {
                                    _errorAttributes.Add(att);
                                }
                            }
                        }

                        foreach (var name in _attributes)
                        {
                            var tree = test.Tree;
                            tree.Data = test.Init;
                            tree.ClassificationAttributeName = name;
                            tree.Build();

                            if (_isPruning)
                                tree.Prunning(test.Controll, 1.96);

                            double res = tree.GetRightProbability(test.Controll);
                            test.Results[name] = res;

                            cur++;

                            int _i = fullList.IndexOf(name);
                            int _j = tests.IndexOf(test);

                            lock (ConsoleWriterLock)
                            {
                                Console.CursorTop = curR;
                                Console.CursorLeft = 0;
                                Console.Write(string.Format("{0:0.00}", 100.0 * cur / total));
                                Console.WriteLine();

                                matrix[_i + 1][_j + 1] = string.Format("{0:0.00}", test.Results[name]);
                                Print(matrix);
                            }
                        }

                        if (_errorAttributes.Count > 0)
                        {
                            Console.Write("Wrong attributes: ");
                            foreach (var att in _errorAttributes)
                                Console.Write(" " + att);
                            Console.WriteLine();
                        }

                    });
                    tasks.Add(task);
                }
                #endregion
                start += fullList.Count + 5;
            }
            #endregion

            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());

            Console.CursorTop = start;
            Console.WriteLine("All OK " + sw.ElapsedMilliseconds);
            Console.WriteLine("OK");
            Console.ReadLine();
        }
        static void Print(string[][] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[i].GetLength(0); j++)
                {
                    Console.Write(matrix[i][j]?.PadRight(15) ?? " ".PadRight(15));
                }
                Console.WriteLine();
            }
        }
        static string[][] Clear(List<string> fullList, List<Test> tests)
        {
            string[][] matrix = new string[fullList.Count + 1][];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix[i] = new string[tests.Count + 1];
            }

            for (int i = 0; i < fullList.Count; i++)
            {
                matrix[i + 1][0] = fullList[i];
            }

            for (int i = 0; i < tests.Count; i++)
            {
                matrix[0][i + 1] = tests[i].Tree.Name;
            }

            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                for (int j = 1; j < matrix[i].GetLength(0); j++)
                {
                    matrix[i][j] = string.Format("{0:0.00}", double.NaN);
                }
            }

            return matrix;
        }
    }


}
