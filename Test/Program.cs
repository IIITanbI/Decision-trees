//using C4_5;
//using ID3;
//using KNN;
//using Main;
//using NBC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Test
//{
//    public class Test
//    {
//        public ITree Tree { get; set; }

//        public DataSet Full { get; set; } = new DataSet();

//        public DataSet Init { get; set; } = new DataSet();
//        public DataSet CrossValidate { get; set; } = new DataSet();
//        public DataSet Controll { get; set; } = new DataSet();

//        public Dictionary<string, double> Results = new Dictionary<string, double>();

//        public void Prepare(string file)
//        {
//            Results.Clear();

//            Full = Tree.Parse(file);
//            //Full.Items = Full.Items.Take(6).ToList();
//            Init = new DataSet();
//            CrossValidate = new DataSet();
//            Controll = new DataSet();

//            int partCnt = Full.Items.Count / 3;
//            this.Init.Items = Full.Items.Take(partCnt).ToList();
//            this.CrossValidate.Items = Full.Items.Skip(partCnt).Take(partCnt).ToList();
//            this.Controll.Items = Full.Items.Skip(partCnt + partCnt).ToList();
//        }
//    }
//    public class Program
//    {
//        static void Main(string[] args)
//        {
//            var files = new List<string>();
//            files.Add("input.txt");

//            var tests = new List<Test>();
//            tests.Add(new Test() { Tree = new C4_5Tree() });
//            tests.Add(new Test() { Tree = new ID3Tree() });
//            tests.Add(new Test() { Tree = new KNNTree() });
//            tests.Add(new Test() { Tree = new NBCTree() });




//            foreach (var fileName in files)
//            {
//                Console.WriteLine($"File: {fileName}");

//                tests.ForEach(t => t.Prepare(fileName));
//                List<string> fullList = new List<string>();

              
//                foreach (var test in tests)
//                {
//                    Console.WriteLine($"Test: {test}");
//                    test.Prepare(fileName);
//                    var _attributes = test.Full.GetAttributeList();
//                    fullList = fullList.Union(_attributes.Keys).ToList();

//                    foreach (var attribute in _attributes)
//                    {
//                        Console.WriteLine($"Attribute: {attribute}");
//                        string name = attribute.Key;
//                        var tree = test.Tree;
//                        tree.Data = test.Init;
//                        tree.ClassificationAttributeName = name;
//                        tree.Build();

//                        double pre = tree.GetRightProbability(test.Controll);
//                        tree.Prunning(test.Controll, 1.96);
//                        double post = tree.GetRightProbability(test.Controll);
//                        Console.WriteLine(pre + " vs "  + post);
//                        double best = Math.Max(pre, post);
//                        test.Results[name] = best;
//                        test.Results[name] = post;
//                    }
//                }

//                string[][] matrix = new string[fullList.Count + 1][];


//                for (int i = 0; i < matrix.GetLength(0); i++)
//                {
//                    matrix[i] = new string[tests.Count + 1];
//                }

//                for (int i = 0; i < fullList.Count; i++)
//                {
//                    matrix[i + 1][0] = fullList[i];
//                }

//                for (int i = 0; i < tests.Count; i++)
//                {
//                    matrix[0][i + 1] = tests[i].Tree.Name;
//                }


//                for (int i = 0; i < fullList.Count; i++)
//                {
//                    for (int j = 0; j < tests.Count; j++)
//                    {
//                        string attribute = fullList[i];
//                        var test = tests[j];


//                        if (test.Results.ContainsKey(attribute))
//                        {

//                            matrix[i + 1][j + 1] = string.Format("{0:0.00}", test.Results[attribute]);
//                        }
//                        else
//                        {
//                            matrix[i + 1][j + 1] = double.NaN.ToString();
//                        }
//                    }
//                }

//                for (int i = 0; i < matrix.GetLength(0); i++)
//                {
//                    for (int j = 0; j < matrix[i].GetLength(0); j++)
//                    {
//                        Console.Write(matrix[i][j]?.PadRight(15) ?? " ".PadRight(15));
//                    }
//                    Console.WriteLine();
//                }
//            }




//            //C4_5Tree _tree = new C4_5Tree();
//            //_tree.Data = set;
//            //_tree.ClassificationAttributeName = "VIOL";
//            //_tree.Build();

//            //Console.WriteLine(_tree.GetRightProbability(set1));

//            //_tree.Prunning(set1, 1.69);

//            //Console.WriteLine(_tree.GetRightProbability(set1));

//            Console.WriteLine("OK");
//            Console.ReadLine();
//        }
//    }
//}
