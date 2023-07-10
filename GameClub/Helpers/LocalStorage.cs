using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Helpers
{
    static public class LocalStorage
    {
        static public object _obj { get; set; } = null;
        static private Dictionary<string, object> _dict = new Dictionary<string, object>(); 
        static public object getObjectByName(string name)
        {
            if (_dict.ContainsKey(name))
                return _dict[name];
            else return null;
        }

        static public void addItem(object obj, string key)
        {
            if(_dict.ContainsKey(key))
            {
                _dict[key] = obj;
            }
            else
            {
                _dict.Add(key, obj);
            }
        }
    }
}
