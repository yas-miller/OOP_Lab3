using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WindowsFormsApp1
{
    partial class Deserializer: Basic
    {

        public void ConvertToObject()
        {
            if (this.Type == null) throw new NullReferenceException("Поле \"Type\" не заполнено");

            Program.form.flowLayoutPanel2.Enabled = true;

            String str = Program.FromTextBox();

            /*if (this.Type.Name == "_Base") this.Object = JsonConvert.DeserializeObject<Figures._Base>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Circle") this.Object = JsonConvert.DeserializeObject<Figures.Circle>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Ellipse") this.Object = JsonConvert.DeserializeObject<Figures.Ellipse>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Line") this.Object = JsonConvert.DeserializeObject(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Rect") this.Object = JsonConvert.DeserializeObject<Figures.Rect>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Square") this.Object = JsonConvert.DeserializeObject<Figures.Square>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            else if (this.Type.Name == "Triangle") this.Object = JsonConvert.DeserializeObject<Figures.Triangle>(str, new JsonSerializerSettings() { Formatting = Formatting.Indented });*/
            this.Object = JsonConvert.DeserializeObject(str, this.Type, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            Program.form.textBox3.Clear();
        }
    }
}
