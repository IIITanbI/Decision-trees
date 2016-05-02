using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class Classification
    {
        //public List<ClassificationItem> Result { get; set; } = new List<ClassificationItem>();
        public SortedSet<ClassificationItem> Result { get; set; } = new SortedSet<ClassificationItem>();

        public override string ToString()
        {
            string res = "";

            foreach (var citem in Result)
            {
                res += citem.ToString() + "\n";
            }
            return res;
        }
    }
    public class ClassificationItem : IComparable<ClassificationItem>
    {
        public object Value { get; set; }
        public double Percent { get; set; }
        public double Count { get; set; }

        public int CompareTo(ClassificationItem other)
        {
            if (this.Percent == other.Percent)
            {
                if (this.Count == other.Count)
                {
                    return 1;
                }
                else return this.Count.CompareTo(other.Count);
            }
            else return -1*this.Percent.CompareTo(other.Percent);
        }
        public override string ToString()
        {
            return $"{Value} : {Percent}%";
            //return $"{Value} : {Percent}% : {Count}";
        }
    }

    public enum AttributeType
    {
        Discrete,
        Continuous
    }
    public interface IItemAttribute
    {
        string Name { get; set; }
        AttributeType AttributeType { get; }
        IComparable Value { get; set; }
    }

    public interface INode
    {
        DataSet Data { get; set; }
    }

    public class Item
    {
        public Dictionary<string, IItemAttribute> Attributes { get; set; } = new Dictionary<string, IItemAttribute>();

        public override string ToString()
        {
            string res = "";

            int pad = 10;
            foreach (var attr in Attributes.Values)
            {
                res += attr.Name.PadRight(pad);
            }
            res += Environment.NewLine;

            foreach (var attr in Attributes.Values)
            {
                res += attr.Value.ToString().PadRight(pad);
            }

            return res;
        }
    }
    public class DataSet
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public Dictionary<string, AttributeType> GetAttributeDict()
        {
            var dict = new Dictionary<string, AttributeType>();
            foreach (var item in Items)
            {
                foreach (var pair in item.Attributes)
                {
                    string name = pair.Key;

                    if (!dict.ContainsKey(name))
                    {
                        dict[name] = pair.Value.AttributeType;
                    }
                }
            }
            return dict;
        }

        public List<string> GetAttributeList()
        {
            var list = new List<string>();
            foreach (var item in Items)
            {
                foreach (var name in item.Attributes.Keys)
                {
                    if (!list.Contains(name))
                    {
                        list.Add(name);
                    }
                }
            }
            return list;
        }
        public void Print(int pad)
        {
            foreach (var attribute in Items[0].Attributes.Values)
            {
                Console.Write(attribute.Name.PadRight(pad));
            }
            Console.WriteLine();
            foreach (Item item in Items)
            {
                foreach (var attribute in item.Attributes.Values)
                {
                    Console.Write(attribute.Value?.ToString().PadRight(pad));
                }
                Console.WriteLine();
            }
        }
    }

    public interface ITree
    {
        string Name { get; }
        string ClassificationAttributeName { get; set; }
        DataSet Data { get; set; }

        void Build();

        void Prunning(DataSet set, double z);
        double GetRightProbability(DataSet set);
        Classification Classify(Item item);

        DataSet Parse(string file);
        Item GetItem(Dictionary<string, string> itemAttributes);
    }
}
