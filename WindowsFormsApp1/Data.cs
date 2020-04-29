using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace WindowsFormsApp1
{
    public static class Data
    {

        public static List<object> objects = new List<object>();

        public static void Add(object _object)
        {
            objects.Add(_object);
            Program.form.listBox2.Items.Add(_object.GetType().ToString());
        }
        public static void Delete(int index)
        {
            Program.form.listBox2.Items.RemoveAt(index);
            objects.RemoveAt(index);
        }
        public static object GetObject(int index)
        {
            return objects[index];
            //foreach (var _object in objects) if (_object.GetType() == object_type) return _object;
            //return null;
        }
        public static void SetObject(object _object, int index)
        {
            objects[index] = _object;
            //for (int i = 0; i < objects.Count; i++)
            //    if (objects[i].GetType() == object_type) objects[i] = _object;
            
        }
    }
}
