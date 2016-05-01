using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ID3;
using TreeUI;
using System.Threading;

namespace ID3
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            double n = 14.0;
            double f = 5.0 / n;
            double z = 0.69;
            double delt = z * Math.Sqrt(f * (1.0 - f) / n);
            double up = f + delt;
            double down = f - delt;

            //DataSet set = new DataSet();
            //DataSet set1 = new DataSet();
            //DataSet set2 = new DataSet();
            //DataSet set3 = new DataSet();

            //Item item;
            //for (int i = 0; i < 4; i++)
            //{
            //    item = new Item();
            //    item.Attributes.Add("1", new MyAttribute("1", i % 2));
            //    item.Attributes.Add("split", new MyAttribute("split", 1));
            //    set.Items.Add(item);
            //}

            //for (int i = 0; i < 4; i++)
            //{
            //    item = new Item();
            //    item.Attributes.Add("1", new MyAttribute("1", i % 2));
            //    item.Attributes.Add("split", new MyAttribute("split", 2));
            //    set.Items.Add(item);
            //}


            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Sunny"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set1.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Sunny"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set1.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Sunny"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set1.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Sunny"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 0)); set1.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Sunny"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 0)); set1.Items.Add(item);


            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Overcast"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set2.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Overcast"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set2.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Overcast"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set2.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Overcast"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set2.Items.Add(item);

            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Rainy"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set3.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Rainy"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 1)); set3.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Rainy"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 0)); set3.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Rainy"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 0)); set3.Items.Add(item);
            //item = new Item(); item.Attributes.Add("Outlook", new MyAttribute("Outlook", "Rainy"));
            //item.Attributes.Add("Win", new MyAttribute("Win", 0)); set3.Items.Add(item);

            //List<DataSet> ds = new List<DataSet>() { set1, set2, set3 };

            //var split = Utility.SplitIDiscrete(set, "split");
            //double ent_init = Utility.Entropy(set, "1");
            //double ent_split = Utility.Entropy(split, "1");
            //double ent1 = Utility.Entropy(ds, "Win");

            //var gain = Utility.Gain(ds, "Win");

            //var ttt = Utility.SplitContinuous(set, "1", 0);

            ////int _cnt = 0;
            ////foreach (var dset in ttt)
            ////{
            ////    ++_cnt;
            ////    foreach (var ditem in dset.Items)
            ////    {
            ////        Console.WriteLine(_cnt + ": " + ditem.Attributes["1"].RowValue);
            ////    }
            ////    Console.WriteLine();
            ////}




            //var lines = File.ReadLines("input.txt").ToList();

            //var columns = lines[0].Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.ToLower()).ToList();
            //var map = new HashSet<string>[columns.Count];
            //for (int i = 0; i < columns.Count; i++)
            //{
            //    map[i] = new HashSet<string>();
            //}

            //DataSet resSet = new DataSet();
            //for (int i = 1; i < lines.Count; i++)
            //{
            //    var values = lines[i].Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.ToLower()).ToList();

            //    if (values.Count != columns.Count)
            //    {
            //        throw new ArgumentException("Count of values in row are not equal to columns count");
            //    }

            //    Item _item = new Item();
            //    for (int j = 0; j < values.Count; j++)
            //    {
            //        var attr = new MyAttribute(columns[j], values[j]);
            //        _item.Attributes.Add(attr.Name, attr);
            //    }
            //    resSet.Items.Add(_item);
            //    for (int j = 0; j < values.Count; j++)
            //    {
            //        map[j].Add(values[j]);
            //    }
            //}

            ////Console.WriteLine(Utility.Entropy(resSet, "decision(category)"));

            //Tree tree = new Tree();
            //tree.Data = resSet;
            ////tree.Data = set;
            //tree.ClassificationAttributeName = "decision(category)";
            //// tree.ClassificationAttributeName = "1";
            //tree.Build();



            //foreach (string line in lines.Skip(1))
            //{
            //    Console.WriteLine(line);
            //}

            //{

            //    Node root = new Node(null, 0);

            //    Node node1 = new Node(null, 0);
            //    //Node node2 = new Node(null, 0);

            //    root.Nodes.Add(node1);
            //   // root.Nodes.Add(node2);

            //   // Node node3 = new Node(null, 0);
            //    Node node4 = new Node(null, 0);

            //    //node1.Nodes.Add(node3);
            //    node1.Nodes.Add(node4);

            //    Node node5 = new Node(null, 0); 
            //    Node node6 = new Node(null, 0);
            //    Node node7 = new Node(null, 0);

            //    node4.Nodes.Add(node5);
            //    node4.Nodes.Add(node6);
            //    node4.Nodes.Add(node7);


            //    node5.RightValidate = 4; node5.WrongValidate = 2;
            //    node6.RightValidate = 1; node6.WrongValidate = 1;
            //    node7.RightValidate = 4; node7.WrongValidate = 2;

            //   // root.Prunning(0.69);
            //}

            DataSet set = Parse("input.txt");
            //set.Print(8);

            int mid = set.Items.Count / 2;
            //mid = 3;
            DataSet set1 = new DataSet();
            set1.Items = set.Items.Skip(mid).ToList();
            set.Items.RemoveRange(mid, set.Items.Count - mid);

            Tree tree = new Tree();
            tree.Data = set;
            tree.ClassificationAttributeName = "GENDER";
            tree.Build();

            Thread myThread = new Thread(new ThreadStart(() =>
            {
                var ss = new MainWindow(tree);
                ss.ShowDialog();
            }));
            myThread.SetApartmentState(ApartmentState.STA);
//            myThread.Start();

            tree.EnableCrossValidate = true; 
    

            int right = 0;
            foreach(var item in set1.Items)
            {
                var res = tree.Classify(item);
                var top = res.Result.FirstOrDefault().Value;
                var expected = item.Attributes[tree.ClassificationAttributeName].Value;
                if (expected.CompareTo(top) == 0)
                    right++;
                else
                {

                }
                //else
                //{
                //    Console.WriteLine($"{expected} vs {top}");
                //}
                //if (res.Result.Count != 1)
                //{
                //    Console.WriteLine(item.ToString());
                //}
            }


            double perc = 100.0 * right / set1.Items.Count;
            Console.WriteLine(perc);

            tree.Prunning(1.69);
            right = 0;
            foreach (var item in set1.Items)
            {
                var res = tree.Classify(item);
                var top = res.Result.FirstOrDefault().Value;
                var expected = item.Attributes[tree.ClassificationAttributeName].Value;
                if (expected.CompareTo(top) == 0)
                    right++;
                //else
                //{
                //    Console.WriteLine($"{expected} vs {top}");
                //}
                //if (res.Result.Count != 1)
                //{
                //    Console.WriteLine(item.ToString());
                //}
            }
            Console.WriteLine();
            perc = 100.0 * right / set1.Items.Count;
            Console.WriteLine(perc);


            //var res = tree.Classify(cls);
            //Console.WriteLine(res.ToString());
            Console.WriteLine();
            //Console.ReadLine();







            return;
            tree.Prunning(0.69);

            Thread myThread1 = new Thread(new ThreadStart(() =>
            {
                var ss = new MainWindow(tree);
                ss.ShowDialog();
            }));
            myThread1.SetApartmentState(ApartmentState.STA);
            myThread1.Start();
         }


        /*
            1-st line   - Headers
            2-nd line   - Attribute type (D/C) (Discrete/Co)
            3-rd line   - value type (s/n)
            Next lines  - Items
        */
        public static DataSet Parse(string file)
        {
            DataSet set = new DataSet();

            string[] lines = File.ReadAllLines(file);

            var headerNames = lines[0].Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            var attributeTypes = lines[1].Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            var valueTypes = lines[2].Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);


            if (attributeTypes.Length != headerNames.Length)
                throw new ArgumentException($"Count of Headers {headerNames.Length} not equal to count of attribute types {attributeTypes.Length}");
            if (valueTypes.Length != headerNames.Length)
                throw new ArgumentException($"Count of Headers {headerNames.Length} not equal to count of value types {attributeTypes.Length}");

            foreach (var line in lines.Skip(3))
            {
                string[] values = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != headerNames.Length)
                    throw new ArgumentException($"Count of Headers {headerNames.Length} not equal to count of values {values.Length}");

                Item item = new Item();
                
                for(int i = 0; i < values.Length; i++)
                {
                    string attributeName = headerNames[i].ToLower();
                    string attributeType = attributeTypes[i];
                    string valueType = valueTypes[i];

                    IComparable value = ConvertToObject(values[i], valueType);
                    AttributeType type = GetType(attributeType);
                    item.Attributes[attributeName] = new MyAttribute(attributeName, value, type);
                }

                set.Items.Add(item);
            }
            return set;
        }

        public static IComparable ConvertToObject(string value, string type)
        {
            if (value == "?")
                return null;

            type = type.ToUpper();
            Type _type = null;
            switch (type)
            {
                case "N":
                    _type = typeof(double);
                    break;
                case "D":
                    _type = typeof(DateTime);
                    break;
                case "S":
                default:
                    _type = typeof(string);
                    break;
            }

            object obj = Convert.ChangeType(value, _type);
            return (IComparable)obj;
        }
        public static AttributeType GetType(string attributeType)
        {
            attributeType = attributeType.ToUpper();

            AttributeType type;
            switch (attributeType)
            {
                case "C":
                    type = AttributeType.Continuous;
                    break;
                case "D":
                default:
                    type = AttributeType.Discrete;
                    break;
            }
            return type;
        }
    }
}
