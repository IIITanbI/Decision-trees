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
using TreeUI;

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

            Init = new DataSet();
            CrossValidate = new DataSet();
            Controll = new DataSet();

            int partCnt = Full.Items.Count / 3;
            this.Init.Items = Full.Items.Take(partCnt).ToList();
            this.CrossValidate.Items = Full.Items.Skip(partCnt).Take(partCnt).ToList();
            this.Controll.Items = Full.Items.Skip(partCnt + partCnt).ToList();
        }

        public void PrepareRelease(string file, bool pruning)
        {
            Results.Clear();

            Full = Tree.Parse(file);

            Init = new DataSet();
            CrossValidate = new DataSet();
            Controll = new DataSet();

            Full.Items = Full.Items.Take(20).ToList();
            if (pruning)
            {
                int partCnt = Full.Items.Count / 2;
                this.Init.Items = Full.Items.Take(partCnt).ToList();
                this.CrossValidate.Items = Full.Items.Skip(partCnt).Take(partCnt).ToList();
            }
            else
            {
                this.Init.Items = Full.Items.ToList();
            }
        }
    }


    class Program
    {
        static object ConsoleWriterLock = new object();

        static Dictionary<string, ITree> TreeMap = new Dictionary<string, ITree>()
         {
            {"C45", new C4_5Tree() },
            {"ID3", new ID3Tree() },
            {"KNN", new KNNTree() },
            {"NBC", new NBCTree() }
          };
        [STAThread]
        static void Main(string[] args)
        {
            List<string> _files = new List<string>();
            List<string> _parametres = new List<string>();
            bool _pruning = false;
            bool _allAtttibutes = false;
            bool _guiMode = false;
            string _treeName = "C45".ToUpper();
            string _attribute = null;
            var _attributesList = new List<string>();
            Dictionary<string, string> _newItemAttributes = null;
            //Tree file pruning crossvalidation attribute newItem attr:value
            //Tree file pruning crossvalidation  attribute gui
            //Tree file pruning crossvalidation  all_attributes

            string currentParameter = null;
            OptionSet options = new OptionSet()
            {
                {"t", "tree" , v => {
                    currentParameter = "t";
                }},
                {"f", "file" , v => {
                    currentParameter = "f";
                }},
                {"p", "pruning enable", v => {
                    _pruning = true;
                }},
                {"a", "attribute", v => {
                    currentParameter = "a";
                }},
                {"aa", "all attribute", v => {
                    _attributesList.Clear();
                    _allAtttibutes = true;
                }},
                {"item", "new item", v => {
                    _newItemAttributes = new  Dictionary<string, string>();
                    currentParameter = "item";
                }},
                {"gui", "gui mode", v => {
                    _guiMode = true;
                }},
                { "<>", v => {
                    switch(currentParameter) {
                        case "a":
                            _attributesList.Add(v);
                            break;
                        case "f":
                            _files.Add(v);
                            break;
                        case "t":
                            _treeName = v.ToUpper();
                            break;
                        case "item":
                            string[] temp = v.Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                            string name = temp[0].ToLower();
                            string value = temp[1].ToLower();

                            _newItemAttributes[name] = value;
                            break;
                    }
                }}
            };
            options.Parse(args);


            if (_allAtttibutes)
                Super(_files, _pruning);
            else
            {
                _attribute = _attributesList[0];

                var file = _files[0];
                var tree = TreeMap[_treeName];

                Test test = new Test();
                test.Tree = tree;
                test.PrepareRelease(file, _pruning);

                List<string> fullList = test.Full.GetAttributeList();

                tree.Data = test.Init; ;
                tree.ClassificationAttributeName = _attribute;
                tree.Build();

                if (_pruning)
                    tree.Prunning(test.CrossValidate, 1.96);

                if (_guiMode)
                {
                    MainWindow mw = new MainWindow(tree as C4_5Tree);
                    mw.ShowDialog();
                }
                else
                {
                    Item item = tree.GetItem(_newItemAttributes);
                    var res = tree.Classify(item);
                    Console.WriteLine("Result: \n" + res);
                }
            }
        }

        public static void Super(List<string> _files, bool _isPruning)
        {
            var files = _files;

            var sw = Stopwatch.StartNew();

            int start = Console.CursorTop + 1;
            List<Task> tasks = new List<Task>();

            #region
            foreach (var fileName in files)
            {
                var tests = new List<Test>();
                tests.Add(new Test() { Tree = new C4_5Tree() });
                tests.Add(new Test() { Tree = new ID3Tree() });
                tests.Add(new Test() { Tree = new KNNTree() });
                tests.Add(new Test() { Tree = new NBCTree() });

                Console.WriteLine($"File: {fileName}");

                tests.ForEach(t => t.Prepare(fileName));

                List<string> fullList = new List<string>();

                double total = 0;
                double cur = 0;

                foreach (var test in tests)
                {
                    test.Prepare(fileName);
                    var _attributes = test.Full.GetAttributeList();
                    fullList = fullList.Union(_attributes).ToList();

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
                        List<string> _attributes = test.Full.GetAttributeList();

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
        static string[][] Clear(List<string> fullList, ITree tree)
        {
            string[][] matrix = new string[fullList.Count + 1][];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix[i] = new string[2];
            }

            for (int i = 0; i < fullList.Count; i++)
            {
                matrix[i + 1][0] = fullList[i];
            }

            for (int i = 0; i < 1; i++)
            {
                matrix[0][i + 1] = tree.Name;
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
