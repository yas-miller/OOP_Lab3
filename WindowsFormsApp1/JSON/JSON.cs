﻿using System;
using System.Collections;
using System.Collections.Generic;
#if ! SILVERLIGHT
	using System.Data;
#endif
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Xml;
using WindowsFormsApp1;

namespace JSON
	{
	public delegate string Serialize(object data);
	public delegate object Deserialize(string data);

	#region klinkenbecker
	// Based on the original by Mehdi Gholam - http://www.codeproject.com/KB/IP/fastJSON.aspx
	// Which in turn was derived from...??
	//
	// I tweeked Mehdi's code for a utilty fix (http://www.codeproject.com/Messages/4027380/Sharing-JSON-objects-between-assembly.aspx)
	// and even more speed (http://www.codeproject.com/Messages/4029697/How-to-pick-up-another-10-speed.aspx).
	//
	// For JSON spec see: www.json.org
	//
	// A JSON string defines either;
	//   a JSON object, a collection of name/value pairs where 'name' is a string and 'value' is any JSON value, OR
	//   a JSON array, an ordered list of JSON values
	//
	// There are 7 defined JSON values: string, number, true, false, null, object, array
	// Of these, string, number, true, false, null may be considered as 'basic values' and object and array as 'compound values'
	//
	// Compound values are a structured collection of JSON values:
	//     { "name" : "this is the value" }     - a named object with a and a string value
	//     { "name" : 123.45 }                  - a named object with a number value
	//     { "name" : { "object" : 123 } }      - a named object with an object (named "object") value
	//     [ "string", 1234, true, false, null, { "object" : 123 } ]
	//                                          - an array of values
	//     { "name" : [ "string", 1234, true, false, null, { "object" : 123 } ] }
	//                                          - a named object with an array of values value
	//
	// The original JSON converted all basic values to type 'string' and the original type info was lost
	// When serializing/deserializing, it may be important not to loose track of the basic value type
	// Thus JSON has been extended to retain the original value type information. Doing so comes at a
	// very slight loss of throughput in order to create the derived objects.
	//

	public class jsonObject : Dictionary<string, jsonObject>
		{
		public static implicit operator jsonObject(String s)
			{
			return new jsonObject(/* s */);
			}

		public static implicit operator jsonObject(jsonNumber n)
			{
			return (jsonObject) ((object) n);
			}
		}

	public class jsonArray
		{
		}

	public class jsonValue
		{
		public static implicit operator jsonValue(string n)
			{
			return n.ToString();
			}

		public static implicit operator string(jsonValue n)
			{
			return n.ToString();
			}

		public new virtual string ToString()
			{
			return "null";
			}

		public virtual string Emit()
			{
			return ToString();
			}
		}

	public class jsonString : jsonValue
		{
		private String value;

		public jsonString(String s)
			{
			value = s;
			}

		public static implicit operator jsonString(String s)
			{
			return new jsonString(s);
			}

		public static implicit operator string(jsonString n)
			{
			return n.ToString();
			}

		public override string ToString()
			{
			return value;
			}

		public override string Emit()
			{
			return "\"" + value + "\"";
			}
		}

	public class jsonBoolean : jsonValue
		{
		private Boolean value;

		public jsonBoolean(String s)
			{
			value = s.ToLower() == "true" ? true : false;
			}

		public jsonBoolean(Boolean s)
			{
			value = s;
			}

		public static implicit operator jsonBoolean(Boolean s)
			{
			return new jsonBoolean(s);
			}

		public static implicit operator jsonBoolean(String s)
			{
			return new jsonBoolean(s);
			}

		public static implicit operator string(jsonBoolean n)
			{
			return n.ToString();
			}

		public override string ToString()
			{
			return value ? "true" : "false";
			}

		public override string Emit()
			{
			return ToString();
			}
		}

	public abstract class jsonNumber : jsonValue
		{
		public static jsonNumber Parse(String s)
			{
			if (s.IndexOf('.') == -1)
				return new jsonInteger(s);
			else
				return new jsonReal(s);
			}

		public static implicit operator jsonNumber(string s)
			{
			return jsonNumber.Parse(s);
			}

		public static implicit operator jsonNumber(int i)
			{
			return new jsonInteger(i);
			}

		public static implicit operator jsonNumber(double r)
			{
			return new jsonReal(r);
			}

		public static implicit operator string(jsonNumber n)
			{
			return n.ToString();
			}
		}

	public class jsonInteger : jsonNumber
		{
		private int value;

		public jsonInteger(string s)
			{
			if (s == "") value = 0;
			else
				try
				{
					value = int.Parse(s, CultureInfo.InvariantCulture);
				}
				catch(System.FormatException)
				{
					MessageBox.Show("Неверный ввод: ожидался тип \"int\"",
					"Ошибка",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
					JSONEditorForm.JSONForm.valueTextBox.Text = JSONEditorForm.JSONForm.valueTextBox.Text.Remove(JSONEditorForm.JSONForm.valueTextBox.Text.Length - 1);
				}
			}

		public jsonInteger(int i)
			{
			value = i;
			}

		public static implicit operator jsonInteger(string s)
			{
			return new jsonInteger(s);
			}

		public static implicit operator jsonInteger(int i)
			{
			return new jsonInteger(i);
			}

		public static implicit operator string(jsonInteger n)
			{
			return n.ToString();
			}

		public override string ToString()
			{
			return value.ToString();
			}

		public override string Emit()
			{
			return ToString();
			}
		}

	public class jsonReal : jsonNumber
		{
		private double value;

		public jsonReal(string s)
			{
			value = double.Parse(s);
			}

		public jsonReal(double r)
			{
			value = r;
			}

		public static implicit operator jsonReal(string s)
			{
			return new jsonReal(s);
			}

		public static implicit operator jsonReal(double r)
			{
			return new jsonReal(r);
			}

		public static implicit operator string(jsonReal n)
			{
			return n.ToString();
			}

		public override string ToString()
			{
			return value.ToString();
			}

		public override string Emit()
			{
			return ToString();
			}
		}

#if false
			jsonInteger a = 1;
			jsonInteger b = "1";
			jsonNumber c = "1";
			jsonNumber d = 1;
			jsonNumber e = 1.234;
			jsonNumber f = "1.234";
			jsonReal g = 1.5;
			jsonReal h = "1.6";
			string s = (string) a;
			string s1 = (string) d;
			string s2 = (string) g;
#endif
	#endregion

	public class JSON
		{
		public readonly static JSON Instance = new JSON();

		private JSON()
			{
			}
		public bool UseOptimizedDatasetSchema = true;
		public bool UseFastGuid = false;
		public bool UseSerializerExtension = true;
		public bool IndentOutput = false;
		public bool SerializeNullValues = true;

		public string ToJSON(object obj)
			{
			return ToJSON(obj, UseSerializerExtension, UseFastGuid, UseOptimizedDatasetSchema, SerializeNullValues);
			}

		public string ToJSON(object obj,
									bool enableSerializerExtensions)
			{
			return ToJSON(obj, enableSerializerExtensions, UseFastGuid, UseOptimizedDatasetSchema, SerializeNullValues);
			}

		public string ToJSON(object obj,
									bool enableSerializerExtensions,
									bool enableFastGuid)
			{
			return ToJSON(obj, enableSerializerExtensions, enableFastGuid, UseOptimizedDatasetSchema, SerializeNullValues);
			}

		public string ToJSON(object obj,
									bool enableSerializerExtensions,
									bool enableFastGuid,
									bool enableOptimizedDatasetSchema,
									bool serializeNullValues)
			{
			return new JSONSerializer(enableOptimizedDatasetSchema, enableFastGuid, enableSerializerExtensions, serializeNullValues, IndentOutput).ConvertToJSON(obj);
			}

		public static object Parse(string json)
			{
			return new JsonParser(json).Decode();
			}

		public T ToObject<T>(string json)
			{
			return (T)ToObject(json, typeof(T), typeof(T).Assembly);
			}

		public object ToObject(string json)
			{
			return ToObject(json, null, null);
			}

		public object ToObject(string json, Type type)
			{
			return ToObject(json, type, null);
			}

		public object ToObject(string json, Type type, Assembly asm)
			{
			Dictionary<string, object> ht = new JsonParser(json).Decode() as Dictionary<string, object>;
			if (ht == null) return null;

			return ParseDictionary(ht, type, asm);
			}

#if CUSTOMTYPE
        internal SafeDictionary<Type, Serialize> _customSerializer = new SafeDictionary<Type, Serialize>();
        internal SafeDictionary<Type, Deserialize> _customDeserializer = new SafeDictionary<Type, Deserialize>();

        public void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
            if (type != null && serializer != null && deserializer != null)
            {
                _customSerializer.Add(type, serializer);
                _customDeserializer.Add(type, deserializer);
                // reset property cache
                _propertycache = new SafeDictionary<string, SafeDictionary<string, myPropInfo>>();
            }
        }

        internal bool IsTypeRegistered(Type t)
        {
            Serialize s;
            return _customSerializer.TryGetValue(t, out s);
        }
#endif

		#region [   PROPERTY GET SET CACHE   ]
		SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
		internal string GetTypeAssemblyName(Type t)
			{
			string val = "";
			if (_tyname.TryGetValue(t, out val))
				return val;
			else
				{
				string s = t.AssemblyQualifiedName;
				_tyname.Add(t, s);
				return s;
				}
			}

		SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
		private Type GetTypeFromCache(string typename)
			{
			Type val = null;
			if (_typecache.TryGetValue(typename, out val))
				return val;
			else
				{
				Type t = Type.GetType(typename);
				_typecache.Add(typename, t);
				return t;
				}
			}

		SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
		private delegate object CreateObject();
		private object FastCreateInstance(Type objtype)
			{
			try
				{
				CreateObject c = null;
				if (_constrcache.TryGetValue(objtype, out c))
					{
					return c();
					}
				else
					{
					DynamicMethod dynMethod = new DynamicMethod("_", objtype, null);
					ILGenerator ilGen = dynMethod.GetILGenerator();

					ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
					ilGen.Emit(OpCodes.Ret);
					c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
					_constrcache.Add(objtype, c);
					return c();
					}
				}
			catch (Exception exc)
				{
				throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assemebly '{1}'",
					 objtype.FullName, objtype.AssemblyQualifiedName), exc);
				}
			}

		private struct myPropInfo
			{
			public bool filled;
			public Type pt;
			public Type bt;
			public Type changeType;
			public bool isDictionary;
			public bool isValueType;
			public bool isGenericType;
			public bool isArray;
			public bool isByteArray;
			public bool isGuid;
#if !SILVERLIGHT
			public bool isDataSet;
			public bool isDataTable;
			public bool isHashtable;
#endif
			public GenericSetter setter;
			public bool isEnum;
			public bool isDateTime;
			public Type[] GenericTypes;
			public bool isInt;
			public bool isLong;
			public bool isString;
			public bool isBool;
			public bool isClass;
			public GenericGetter getter;
			public bool isStringDictionary;
			public string Name;
			public bool isCustomType;
			}

		SafeDictionary<string, SafeDictionary<string, myPropInfo>> _propertycache = new SafeDictionary<string, SafeDictionary<string, myPropInfo>>();
		private SafeDictionary<string, myPropInfo> Getproperties(Type type, string typename)
			{
			SafeDictionary<string, myPropInfo> sd = null;
			if (_propertycache.TryGetValue(typename, out sd))
				{
				return sd;
				}
			else
				{
				sd = new SafeDictionary<string, myPropInfo>();
				PropertyInfo[] pr = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo p in pr)
					{
					myPropInfo d = CreateMyProp(p.PropertyType, p.Name);
					d.setter = CreateSetMethod(p);
					d.getter = CreateGetMethod(p);
					sd.Add(p.Name, d);
					}
				FieldInfo[] fi = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo f in fi)
					{
					myPropInfo d = CreateMyProp(f.FieldType, f.Name);
					d.setter = CreateSetField(type, f);
					d.getter = CreateGetField(type, f);
					sd.Add(f.Name, d);
					}

				_propertycache.Add(typename, sd);
				return sd;
				}
			}

		private myPropInfo CreateMyProp(Type t, string name)
			{
			myPropInfo d = new myPropInfo();
			d.filled = true;
			d.pt = t;
			d.Name = name;
			d.isDictionary = t.Name.Contains("Dictionary");
			if (d.isDictionary)
				d.GenericTypes = t.GetGenericArguments();
			d.isValueType = t.IsValueType;
			d.isGenericType = t.IsGenericType;
			d.isArray = t.IsArray;
			if (d.isArray)
				d.bt = t.GetElementType();
			if (d.isGenericType)
				d.bt = t.GetGenericArguments()[0];
			d.isByteArray = t == typeof(byte[]);
			d.isGuid = (t == typeof(Guid) || t == typeof(Guid?));
#if !SILVERLIGHT
			d.isHashtable = t == typeof(Hashtable);
			d.isDataSet = t == typeof(DataSet);
			d.isDataTable = t == typeof(DataTable);
#endif

			d.changeType = GetChangeType(t);
			d.isEnum = t.IsEnum;
			d.isDateTime = t == typeof(DateTime) || t == typeof(DateTime?);
			d.isInt = t == typeof(int) || t == typeof(int?);
			d.isLong = t == typeof(long) || t == typeof(long?);
			d.isString = t == typeof(string);
			d.isBool = t == typeof(bool) || t == typeof(bool?);
			d.isClass = t.IsClass;

			if (d.isDictionary && d.GenericTypes[0] == typeof(string) && d.GenericTypes[1] == typeof(string))
				d.isStringDictionary = true;

#if CUSTOMTYPE
                    if (IsTypeRegistered(t))
                        d.isCustomType = true;
#endif
			return d;
			}

		private delegate void GenericSetter(object target, object value);

		private static GenericSetter CreateSetMethod(PropertyInfo propertyInfo)
			{
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			if (setMethod == null)
				return null;

			Type[] arguments = new Type[2];
			arguments[0] = arguments[1] = typeof(object);

			DynamicMethod setter = new DynamicMethod("_", typeof(void), arguments);
			ILGenerator il = setter.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			il.Emit(OpCodes.Ldarg_1);

			if (propertyInfo.PropertyType.IsClass)
				il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
			else
				il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

			il.EmitCall(OpCodes.Callvirt, setMethod, null);
			il.Emit(OpCodes.Ret);

			return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
			}

		internal delegate object GenericGetter(object obj);

		private static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
			{
			DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type, true);
			ILGenerator il = dynamicGet.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldInfo);
			if (fieldInfo.FieldType.IsValueType)
				il.Emit(OpCodes.Box, fieldInfo.FieldType);
			il.Emit(OpCodes.Ret);

			return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
			}

		private static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
			{
			Type[] arguments = new Type[2];
			arguments[0] = arguments[1] = typeof(object);

			DynamicMethod dynamicSet = new DynamicMethod("_", typeof(void), arguments, type, true);
			ILGenerator il = dynamicSet.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			if (fieldInfo.FieldType.IsValueType)
				il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
			il.Emit(OpCodes.Stfld, fieldInfo);
			il.Emit(OpCodes.Ret);

			return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
			}

		private GenericGetter CreateGetMethod(PropertyInfo propertyInfo)
			{
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			if (getMethod == null)
				return null;

			Type[] arguments = new Type[1];
			arguments[0] = typeof(object);

			DynamicMethod getter = new DynamicMethod("_", typeof(object), arguments);
			ILGenerator il = getter.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			il.EmitCall(OpCodes.Callvirt, getMethod, null);

			if (!propertyInfo.PropertyType.IsClass)
				il.Emit(OpCodes.Box, propertyInfo.PropertyType);

			il.Emit(OpCodes.Ret);

			return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
			}

		readonly SafeDictionary<Type, List<Getters>> _getterscache = new SafeDictionary<Type, List<Getters>>();
		internal List<Getters> GetGetters(Type type)
			{
			List<Getters> val = null;
			if (_getterscache.TryGetValue(type, out val))
				return val;

			PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			List<Getters> getters = new List<Getters>();
			foreach (PropertyInfo p in props)
				{
				if (!p.CanWrite) continue;

				object[] att = p.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
				if (att != null && att.Length > 0)
					continue;

				JSON.GenericGetter g = CreateGetMethod(p);
				if (g != null)
					{
					Getters gg = new Getters();
					gg.Name = p.Name;
					gg.Getter = g;
					gg.propertyType = p.PropertyType;
					getters.Add(gg);
					}
				}

			FieldInfo[] fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (var f in fi)
				{
				object[] att = f.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
				if (att != null && att.Length > 0)
					continue;

				JSON.GenericGetter g = CreateGetField(type, f);
				if (g != null)
					{
					Getters gg = new Getters();
					gg.Name = f.Name;
					gg.Getter = g;
					gg.propertyType = f.FieldType;
					getters.Add(gg);
					}
				}

			_getterscache.Add(type, getters);
			return getters;
			}

		private object ChangeType(object value, Type conversionType)
			{
			if (conversionType == typeof(int))
				return (int)CreateLong((string)value);

			else if (conversionType == typeof(long))
				return CreateLong((string)value);

			else if (conversionType == typeof(string))
				return (string)value;

			else if (conversionType == typeof(Guid))
				return CreateGuid((string)value);

			return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
			}
		#endregion


		private object ParseDictionary(Dictionary<string, object> d, Type type)
			{
			return ParseDictionary(d, type, null);
			}

		private object ParseDictionary(Dictionary<string, object> d, Type type, Assembly asm)
			{
			object tn = "";
			bool found = d.TryGetValue("$type", out tn);
#if !SILVERLIGHT
			if (found == false && type == typeof(System.Object))
				{
				return CreateDataset(d);
				}
#endif
			if (found && asm != null)
				type = GetTypeFromCache((string)tn + ", " + asm.FullName);

			if (type == null)
				throw new Exception("Cannot determine type");

			string typename = type.Name;
			object o = FastCreateInstance(type);
			SafeDictionary<string, myPropInfo> props = Getproperties(type, typename);
			foreach (string name in d.Keys)
				{
				if (name == "$map")
					{
					ProcessMap(o, props, (Dictionary<string, object>)d[name]);
					continue;
					}
				myPropInfo pi;
				if (props.TryGetValue(name, out pi) == false)
					continue;
				if (pi.filled == true)
					{
					object v = d[name];

					if (v != null)
						{
						object oset = null;

						if (pi.isInt)
							oset = (int)CreateLong((string)v);
#if CUSTOMTYPE
                        else if (pi.isCustomType)
                            oset = CreateCustom((string)v, pi.pt);
#endif
						else if (pi.isLong)
							oset = CreateLong((string)v);

						else if (pi.isString)
							oset = (string)v;

						else if (pi.isBool)
							oset = (bool)v;

						else if (pi.isGenericType && pi.isValueType == false && pi.isDictionary == false)
#if SILVERLIGHT
                            oset = CreateGenericList((List<object>)v, pi.pt, pi.bt);
#else
							oset = CreateGenericList((ArrayList)v, pi.pt, pi.bt);
#endif
						else if (pi.isByteArray)
							oset = Convert.FromBase64String((string)v);

						else if (pi.isArray && pi.isValueType == false)
#if SILVERLIGHT
                            oset = CreateArray((List<object>)v, pi.pt, pi.bt);
#else
							oset = CreateArray((ArrayList)v, pi.pt, pi.bt);
#endif
						else if (pi.isGuid)
							oset = CreateGuid((string)v);
#if !SILVERLIGHT
						else if (pi.isDataSet)
							oset = CreateDataset((Dictionary<string, object>)v);

						else if (pi.isDataTable)
							oset = this.CreateDataTable((Dictionary<string, object>)v);
#endif

						else if (pi.isStringDictionary)
							oset = CreateStringKeyDictionary((Dictionary<string, object>)v, pi.pt, pi.GenericTypes);

#if !SILVERLIGHT
						else if (pi.isDictionary || pi.isHashtable)
							oset = CreateDictionary((ArrayList)v, pi.pt, pi.GenericTypes);
#else 
                        else if (pi.isDictionary)
                            oset = CreateDictionary((List<object>)v, pi.pt, pi.GenericTypes);
#endif

						else if (pi.isEnum)
							oset = CreateEnum(pi.pt, (string)v);

						else if (pi.isDateTime)
							oset = CreateDateTime((string)v);

						else if (pi.isClass && v is Dictionary<string, object>)
							oset = ParseDictionary((Dictionary<string, object>)v, pi.pt);

						else if (pi.isValueType)
							oset = ChangeType(v, pi.changeType);

#if SILVERLIGHT
                        else if (v is List<object>)
                            oset = CreateArray((List<object>)v, pi.pt, typeof(object));
#else
						else if (v is ArrayList)
							oset = CreateArray((ArrayList)v, pi.pt, typeof(object));
#endif
						else
							oset = v;

						pi.setter(o, oset);
						}
					}
				}
			return o;
			}

#if CUSTOMTYPE
        private object CreateCustom(string v, Type type)
        {
            Deserialize d;
            _customDeserializer.TryGetValue(type, out d);
            return d(v);
        }
#endif

		private void ProcessMap(object obj, SafeDictionary<string, JSON.myPropInfo> props, Dictionary<string, object> dic)
			{
			foreach (KeyValuePair<string, object> kv in dic)
				{
				myPropInfo p = props[kv.Key];
				object o = p.getter(obj);
				Type t = Type.GetType((string)kv.Value);
				if (t == typeof(Guid))
					p.setter(obj, CreateGuid((string)o));
				}
			}

		private long CreateLong(string s)
			{
			long num = 0;
			bool neg = false;
			foreach (char cc in s)
				{
				if (cc == '-')
					neg = true;
				else if (cc == '+')
					neg = false;
				else
					{
					num *= 10;
					num += (int)(cc - '0');
					}
				}

			return neg ? -num : num;
			}

		private object CreateEnum(Type pt, string v)
			{
			// TODO : optimize create enum
#if !SILVERLIGHT
			return Enum.Parse(pt, v);
#else
            return Enum.Parse(pt, v, true);
#endif
			}

		private Guid CreateGuid(string s)
			{
			if (s.Length > 30)
				return new Guid(s);
			else
				return new Guid(Convert.FromBase64String(s));
			}

		private DateTime CreateDateTime(string value)
			{
			//                   0123456789012345678
			// datetime format = yyyy-MM-dd HH:mm:ss
			int year = (int)CreateLong(value.Substring(0, 4));
			int month = (int)CreateLong(value.Substring(5, 2));
			int day = (int)CreateLong(value.Substring(8, 2));
			int hour = (int)CreateLong(value.Substring(11, 2));
			int min = (int)CreateLong(value.Substring(14, 2));
			int sec = (int)CreateLong(value.Substring(17, 2));
			return new DateTime(year, month, day, hour, min, sec);
			}

#if SILVERLIGHT
        private object CreateArray(List<object> data, Type pt, Type bt)
        {
            Array col = Array.CreateInstance(bt, data.Count);
            // create an array of objects
            for (int i = 0; i < data.Count; i++)// each (object ob in data)
            {
                object ob = data[i];
                if (ob is IDictionary)
                    col.SetValue(ParseDictionary((Dictionary<string, object>)ob, bt), i);
                else
                    col.SetValue(ChangeType(ob, bt), i);
            }

            return col;
        }
#else
		private object CreateArray(ArrayList data, Type pt, Type bt)
			{
			ArrayList col = new ArrayList();
			// create an array of objects
			foreach (object ob in data)
				{
				if (ob is IDictionary)
					col.Add(ParseDictionary((Dictionary<string, object>)ob, bt));
				else
					col.Add(ChangeType(ob, bt));
				}
			return col.ToArray(bt);
			}
#endif


#if SILVERLIGHT
        private object CreateGenericList(List<object> data, Type pt, Type bt)
#else
		private object CreateGenericList(ArrayList data, Type pt, Type bt)
#endif
			{
			IList col = (IList)FastCreateInstance(pt);
			// create an array of objects
			foreach (object ob in data)
				{
				if (ob is IDictionary)
					col.Add(ParseDictionary((Dictionary<string, object>)ob, bt));
#if SILVERLIGHT
                else if (ob is List<object>)
                    col.Add(((List<object>)ob).ToArray());
#else
				else if (ob is ArrayList)
					col.Add(((ArrayList)ob).ToArray());
#endif
				else
					col.Add(ChangeType(ob, bt));
				}
			return col;
			}

		private object CreateStringKeyDictionary(Dictionary<string, object> reader, Type pt, Type[] types)
			{
			var col = (IDictionary)FastCreateInstance(pt);
			Type t1 = null;
			Type t2 = null;
			if (types != null)
				{
				t1 = types[0];
				t2 = types[1];
				}

			foreach (KeyValuePair<string, object> values in reader)
				{
				var key = values.Key;//ChangeType(values.Key, t1);
				var val = ChangeType(values.Value, t2);
				col.Add(key, val);
				}

			return col;
			}

#if SILVERLIGHT
        private object CreateDictionary(List<object> reader, Type pt, Type[] types)
#else
		private object CreateDictionary(ArrayList reader, Type pt, Type[] types)
#endif
			{
			IDictionary col = (IDictionary)FastCreateInstance(pt);
			Type t1 = null;
			Type t2 = null;
			if (types != null)
				{
				t1 = types[0];
				t2 = types[1];
				}

			foreach (Dictionary<string, object> values in reader)
				{
				object key = values["k"];
				object val = values["v"];

				if (key is Dictionary<string, object>)
					key = ParseDictionary((Dictionary<string, object>)key, t1);
				else
					key = ChangeType(key, t1);

				if (val is Dictionary<string, object>)
					val = ParseDictionary((Dictionary<string, object>)val, t2);
				else
					val = ChangeType(val, t2);

				col.Add(key, val);
				}

			return col;
			}

		private Type GetChangeType(Type conversionType)
			{
			if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				return conversionType.GetGenericArguments()[0];

			return conversionType;
			}
#if !SILVERLIGHT
		private DataSet CreateDataset(Dictionary<string, object> reader)
			{
			DataSet ds = new DataSet();
			ds.EnforceConstraints = false;
			ds.BeginInit();

			// read dataset schema here
			ReadSchema(reader, ds);

			foreach (KeyValuePair<string, object> pair in reader)
				{
				if (pair.Key == "$type" || pair.Key == "$schema") continue;

				ArrayList rows = (ArrayList)pair.Value;
				if (rows == null) continue;

				DataTable dt = ds.Tables[pair.Key];
				ReadDataTable(rows, dt);
				}

			ds.EndInit();

			return ds;
			}

		private void ReadSchema(Dictionary<string, object> reader, DataSet ds)
			{
			var schema = reader["$schema"];

			if (schema is string)
				{
				TextReader tr = new StringReader((string)schema);
				ds.ReadXmlSchema(tr);
				}
			else
				{
				DatasetSchema ms = (DatasetSchema)ParseDictionary((Dictionary<string, object>)schema, typeof(DatasetSchema));
				ds.DataSetName = ms.Name;
				for (int i = 0; i < ms.Info.Count; i += 3)
					{
					if (ds.Tables.Contains(ms.Info[i]) == false)
						ds.Tables.Add(ms.Info[i]);
					ds.Tables[ms.Info[i]].Columns.Add(ms.Info[i + 1], Type.GetType(ms.Info[i + 2]));
					}
				}
			}

		private static void ReadDataTable(ArrayList rows, DataTable dt)
			{
			dt.BeginInit();
			dt.BeginLoadData();
			List<int> guidcols = new List<int>();

			foreach (DataColumn c in dt.Columns)
				if (c.DataType == typeof(Guid) || c.DataType == typeof(Guid?))
					guidcols.Add(c.Ordinal);

			foreach (ArrayList row in rows)
				{
				object[] v = new object[row.Count];
				row.CopyTo(v, 0);
				foreach (int i in guidcols)
					{
					string s = (string)v[i];
					if (s != null && s.Length < 36)
						v[i] = new Guid(Convert.FromBase64String(s));
					}
				dt.Rows.Add(v);
				}

			dt.EndLoadData();
			dt.EndInit();
			}

		DataTable CreateDataTable(Dictionary<string, object> reader)
			{
			var dt = new DataTable();

			// read dataset schema here
			var schema = reader["$schema"];

			if (schema is string)
				{
				TextReader tr = new StringReader((string)schema);
				dt.ReadXmlSchema(tr);
				}
			else
				{
				var ms = (DatasetSchema)this.ParseDictionary((Dictionary<string, object>)schema, typeof(DatasetSchema));
				dt.TableName = ms.Name;
				for (int i = 0; i < ms.Info.Count; i += 2)
					{
					dt.Columns.Add(ms.Info[i], Type.GetType(ms.Info[i + 1]));
					}
				}

			foreach (var pair in reader)
				{
				if (pair.Key == "$type" || pair.Key == "$schema")
					continue;

				var rows = (ArrayList)pair.Value;
				if (rows == null)
					continue;

				if (!dt.TableName.Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase))
					continue;

				ReadDataTable(rows, dt);
				}

			return dt;
			}
#endif
		}
	}