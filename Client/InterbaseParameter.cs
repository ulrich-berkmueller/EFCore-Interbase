/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *
 *    All Rights Reserved.
 */

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

using System.Text;

namespace SK.InterbaseLibraryAdapter;

[ParenthesizePropertyName(true)]
public sealed class InterbaseParameter : DbParameter, ICloneable
{
	#region Fields

	private InterbaseParameterCollection _parent;
	private InterbaseDbType _interbaseDbType;
	private ParameterDirection _direction;
	private DataRowVersion _sourceVersion;
	private InterbaseCharset _charset;
	private bool _isNullable;
	private bool _sourceColumnNullMapping;
	private byte _precision;
	private byte _scale;
	private int _size;
	private object _value;
	private string _parameterName;
	private string _sourceColumn;
	private string _internalParameterName;
	private bool _isUnicodeParameterName;

	#endregion

	#region DbParameter properties

	[DefaultValue("")]
	public override string ParameterName
	{
		get { return _parameterName; }
		set
		{
			_parameterName = value;
			_internalParameterName = NormalizeParameterName(_parameterName);
			_isUnicodeParameterName = IsNonAsciiParameterName(_parameterName);
			_parent?.ParameterNameChanged();
		}
	}

	[Category("Data")]
	[DefaultValue(0)]
	public override int Size
	{
		get
		{
			return (HasSize ? _size : RealValueSize ?? 0);
		}
		set
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException();

			_size = value;

			// Hack for Clob parameters
			if (value == 2147483647 &&
				(InterbaseDbType == InterbaseDbType.VarChar || InterbaseDbType == InterbaseDbType.Char))
			{
				InterbaseDbType = InterbaseDbType.Text;
			}
		}
	}

	[Category("Data")]
	[DefaultValue(ParameterDirection.Input)]
	public override ParameterDirection Direction
	{
		get { return _direction; }
		set { _direction = value; }
	}

	[Browsable(false)]
	[DesignOnly(true)]
	[DefaultValue(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public override bool IsNullable
	{
		get { return _isNullable; }
		set { _isNullable = value; }
	}

	[Category("Data")]
	[DefaultValue("")]
	public override string SourceColumn
	{
		get { return _sourceColumn; }
		set { _sourceColumn = value; }
	}

	[Category("Data")]
	[DefaultValue(DataRowVersion.Current)]
	public override DataRowVersion SourceVersion
	{
		get { return _sourceVersion; }
		set { _sourceVersion = value; }
	}

	[Browsable(false)]
	[Category("Data")]
	[RefreshProperties(RefreshProperties.All)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override DbType DbType
	{
		get { return TypeHelper.GetDbTypeFromDbDataType((DbDataType)_interbaseDbType); }
		set { InterbaseDbType = (InterbaseDbType)TypeHelper.GetDbDataTypeFromDbType(value); }
	}

	[RefreshProperties(RefreshProperties.All)]
	[Category("Data")]
	[DefaultValue(InterbaseDbType.VarChar)]
	public InterbaseDbType InterbaseDbType
	{
		get { return _interbaseDbType; }
		set
		{
			_interbaseDbType = value;
			IsTypeSet = true;
		}
	}

	[Category("Data")]
	[TypeConverter(typeof(StringConverter)), DefaultValue(null)]
	public override object Value
	{
		get { return _value; }
		set
		{
			if (value == null)
			{
				value = DBNull.Value;
			}

			if (InterbaseDbType == InterbaseDbType.Guid && value != null &&
				value != DBNull.Value && !(value is Guid) && !(value is byte[]))
			{
				throw new InvalidOperationException("Incorrect Guid value.");
			}

			_value = value;

			if (!IsTypeSet)
			{
				SetInterbaseDbType(value);
			}
		}
	}

	[Category("Data")]
	[DefaultValue(InterbaseCharset.Default)]
	public InterbaseCharset Charset
	{
		get { return _charset; }
		set { _charset = value; }
	}

	public override bool SourceColumnNullMapping
	{
		get { return _sourceColumnNullMapping; }
		set { _sourceColumnNullMapping = value; }
	}

	#endregion

	#region Properties

	[Category("Data")]
	[DefaultValue((byte)0)]
	public override byte Precision
	{
		get { return _precision; }
		set { _precision = value; }
	}

	[Category("Data")]
	[DefaultValue((byte)0)]
	public override byte Scale
	{
		get { return _scale; }
		set { _scale = value; }
	}

	#endregion

	#region Internal Properties

	internal InterbaseParameterCollection Parent
	{
		get { return _parent; }
		set
		{
			_parent?.ParameterNameChanged();
			_parent = value;
			_parent?.ParameterNameChanged();
		}
	}

	internal string InternalParameterName
	{
		get
		{
			return _internalParameterName;
		}
	}

	internal bool IsTypeSet { get; private set; }

	internal object InternalValue
	{
		get
		{
			switch (_value)
			{
				case string svalue:
					return svalue.Substring(0, Math.Min(Size, svalue.Length));
				case byte[] bvalue:
					var result = new byte[Math.Min(Size, bvalue.Length)];
					Array.Copy(bvalue, result, result.Length);
					return result;
				default:
					return _value;
			}
		}
	}

	internal bool HasSize
	{
		get { return _size != default; }
	}

	#endregion

	#region Constructors

	public InterbaseParameter()
	{
		_interbaseDbType = InterbaseDbType.VarChar;
		_direction = ParameterDirection.Input;
		_sourceVersion = DataRowVersion.Current;
		_sourceColumn = string.Empty;
		_parameterName = string.Empty;
		_charset = InterbaseCharset.Default;
		_internalParameterName = string.Empty;
	}

	public InterbaseParameter(string parameterName, object value)
		: this()
	{
		ParameterName = parameterName;
		Value = value;
	}

	public InterbaseParameter(string parameterName, InterbaseDbType interbaseType)
		: this()
	{
		ParameterName = parameterName;
		InterbaseDbType = interbaseType;
	}

	public InterbaseParameter(string parameterName, InterbaseDbType interbaseType, int size)
		: this()
	{
		ParameterName = parameterName;
		InterbaseDbType = interbaseType;
		Size = size;
	}

	public InterbaseParameter(string parameterName, InterbaseDbType interbaseType, int size, string sourceColumn)
		: this()
	{
		ParameterName = parameterName;
		InterbaseDbType = interbaseType;
		Size = size;
		_sourceColumn = sourceColumn;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public InterbaseParameter(
		string parameterName,
		InterbaseDbType dbType,
		int size,
		ParameterDirection direction,
		bool isNullable,
		byte precision,
		byte scale,
		string sourceColumn,
		DataRowVersion sourceVersion,
		object value)
	{
		ParameterName = parameterName;
		InterbaseDbType = dbType;
		Size = size;
		_direction = direction;
		_isNullable = isNullable;
		_precision = precision;
		_scale = scale;
		_sourceColumn = sourceColumn;
		_sourceVersion = sourceVersion;
		Value = value;
		_charset = InterbaseCharset.Default;
	}

	#endregion

	#region ICloneable Methods
	object ICloneable.Clone()
	{
		return new InterbaseParameter(
			_parameterName,
			_interbaseDbType,
			_size,
			_direction,
			_isNullable,
			_precision,
			_scale,
			_sourceColumn,
			_sourceVersion,
			_value)
		{
			Charset = _charset
		};
	}

	#endregion

	#region DbParameter methods

	public override string ToString()
	{
		return _parameterName;
	}

	public override void ResetDbType()
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Private Methods

	private void SetInterbaseDbType(object value)
	{
		if (value == null)
		{
			value = DBNull.Value;
		}
		_interbaseDbType = TypeHelper.GetInterbaseDataTypeFromType(value.GetType());
	}

	#endregion

	#region Private Properties

	private int? RealValueSize
	{
		get
		{
			var svalue = (_value as string);
			if (svalue != null)
			{
				return svalue.Length;
			}
			var bvalue = (_value as byte[]);
			if (bvalue != null)
			{
				return bvalue.Length;
			}
			return null;
		}
	}

	internal bool IsUnicodeParameterName
	{
		get
		{
			return _isUnicodeParameterName;
		}
	}

	#endregion

	#region Static Methods

	internal static string NormalizeParameterName(string parameterName)
	{
		return string.IsNullOrEmpty(parameterName) || parameterName[0] == '@'
			? parameterName
			: "@" + parameterName;
	}

	internal static bool IsNonAsciiParameterName(string parameterName)
	{
		var isAscii = string.IsNullOrWhiteSpace(parameterName)
			|| Encoding.UTF8.GetByteCount(parameterName) == parameterName.Length;
		return !isAscii;
	}

	#endregion


	#region customizations

	public string ValueAsInterbaseString
	{
		get
		{
			if (_value == null || _value == System.DBNull.Value)
				return "NULL";

			System.Data.DbType dbType = TypeHelper.GetDbTypeFromInterbaseDataType(_interbaseDbType);

			string val;
			bool needsQuoting = true;
			switch (dbType)
			{
				case System.Data.DbType.String:
					val = Convert.ToString(_value);
					break;
				case System.Data.DbType.DateTime:
					val = Convert.ToDateTime(_value).ToString("yyyy-MM-dd HH:mm:ss.ffff");
					break;
				case System.Data.DbType.Int16:
					val = Convert.ToString(Convert.ToInt16(_value));
					needsQuoting = false;
					break;
				case System.Data.DbType.Int32:
					val = Convert.ToString(Convert.ToInt32(_value));
					needsQuoting = false;
					break;
				case System.Data.DbType.Int64:
					val = Convert.ToString(Convert.ToInt64(_value));
					needsQuoting = false;
					break;
				case System.Data.DbType.Double:
					var valAsDouble = Convert.ToDouble(_value);
					val = valAsDouble.ToString(System.Globalization.CultureInfo.InvariantCulture);
					break;
				default:
					throw new NotSupportedException("Value cannot be converted to dbtype " + dbType.ToString());
			}

			if (needsQuoting)
			{
				val = "\'" + val + "\'";
			}

			return val;
		}
	}

	#endregion
}
