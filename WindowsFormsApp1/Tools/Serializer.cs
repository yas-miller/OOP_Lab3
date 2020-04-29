using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace WindowsFormsApp1
{
    class Serializer: Basic
    {

        public void ConvertToJson()
        {
            if ((this.Object == null) || (this.Type == null)) throw new NullReferenceException("Поле \"Object\" или(-и) поле \"Type\" не заполнено(-ы)");
            
            Program.form.textBox3.Text = this.Type.Name;
            
            String str = JsonConvert.SerializeObject(this.Object, Formatting.Indented);
            Program.ToTextBox(str);
            
        }

    }
}
