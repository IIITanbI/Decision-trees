using Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBC
{
    public partial class NBCTree
    {
        public DataSet Parse(string file)
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

                for (int i = 0; i < values.Length; i++)
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

        private IComparable ConvertToObject(string value, string type)
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
        private AttributeType GetType(string attributeType)
        {
            attributeType = attributeType.ToUpper();

            AttributeType type;
            switch (attributeType)
            {
                default:
                    type = AttributeType.Discrete;
                    break;
            }
            return type;
        }
    }
        
}
