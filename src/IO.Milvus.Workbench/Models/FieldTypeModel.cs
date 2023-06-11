using System;

namespace IO.Milvus.Workbench.Models;

public class MilvusFieldTypeModel
{
    public MilvusFieldTypeModel(FieldType fieldType)
    {
        IsPrimaryKey = fieldType.IsPrimaryKey;
        Name = fieldType.Name;
        FieldID = fieldType.FieldId;
        DataType = fieldType.DataType;
        Description = fieldType.Description;

        if (fieldType.TypeParams.ContainsKey(Constants.VECTOR_DIM))
        {
            Dimension = int.Parse(fieldType.TypeParams[Constants.VECTOR_DIM]);
        }
        if (fieldType.TypeParams.ContainsKey(Constants.VARCHAR_MAX_LENGTH))
        {
            MaxLength = int.Parse(fieldType.TypeParams[Constants.VARCHAR_MAX_LENGTH]);
        }
    }

    public MilvusFieldTypeModel(
        string name, 
        MilvusDataType dataType, 
        bool isPrimaryKey, 
        long fieldID,
        string description)
    {
        IsPrimaryKey = isPrimaryKey;
        Name = name;
        FieldID = fieldID;
        DataType = dataType;
        Description = description;
    }
    
    public bool IsPrimaryKey { get; set; }

    public string Name { get; set; }

    public long FieldID { get; set; }

    public bool AutoId { get; set; } = true;

    public MilvusDataType DataType { get; set; }

    public string Description { get; set; }

    public int Dimension { get; set; }

    public long MaxLength { get; set; }

    public string IndexType { get; set; }

    public string IndexParameters { get; set; }

    public override string ToString()
    {
        return $"{nameof(IsPrimaryKey)}:{IsPrimaryKey}\n{nameof(FieldID)}:{FieldID}\n{nameof(DataType)}:{DataType}\n{nameof(Dimension)}:{Dimension}"; 
    }

    internal FieldType ToFieldType()
    {
        FieldType fieldType = default;
        switch (DataType)
        {
            case MilvusDataType.Bool:
                fieldType = FieldType.Create<bool>(Name);
                break;
            case MilvusDataType.Int8:
                fieldType = FieldType.Create<sbyte>(Name);
                break;
            case MilvusDataType.Int16:
                fieldType = FieldType.Create<Int16>(Name);
                break;
            case MilvusDataType.Int32:
                fieldType = FieldType.Create<Int32>(Name);
                break;
            case MilvusDataType.Int64:
                fieldType = FieldType.Create<Int64>(Name,IsPrimaryKey);
                break;
            case MilvusDataType.Float:
                fieldType = FieldType.Create<float>(Name);
                break;
            case MilvusDataType.Double:
                fieldType = FieldType.Create<double>(Name);
                break;
            case MilvusDataType.VarChar:
                fieldType = FieldType.CreateVarchar(Name,MaxLength,IsPrimaryKey);
                break;
            case MilvusDataType.FloatVector:
                fieldType = FieldType.CreateFloatVector(Name, Dimension);
                break;
            case MilvusDataType.BinaryVector:
            case MilvusDataType.None:
            case MilvusDataType.String:
            default:
                throw new System.NotSupportedException();
        }
        return fieldType;
    }
}
