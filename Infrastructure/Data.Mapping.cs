// Seusing DataImport.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Data.Mapping
{
    public class Column
    {
        public string? SourceColumn { get; private set; }
        public string TargetColumn { get; private set; }
        public Type DataType { get; private set; }
        public List<dynamic> Data { get; private set; }
        public dynamic this[int index]
        {
            get => Data[index];
            private set => Data[index] = value;
        }
        public Column(string targetColumn, Type dataType)
        {
            TargetColumn = targetColumn;
            DataType = dataType ?? typeof(object);
            Data = new List<dynamic>();
        }
        public Column(string targetColumn, Type dataType, List<dynamic> data)
        {
            TargetColumn = targetColumn;
            DataType = dataType ?? typeof(object);
            Data = data ?? new List<dynamic>();
        }
        public Column(string sourceColumn, string targetColumn, Type dataType)
        {
            SourceColumn = sourceColumn;
            TargetColumn = targetColumn;
            DataType = dataType ?? typeof(object);
            Data = new List<dynamic>();
        }
        public Column(string sourceColumn, string targetColumn, Type dataType, List<dynamic> data)
        {
            SourceColumn = sourceColumn;
            TargetColumn = targetColumn;
            DataType = dataType ?? typeof(object);
            Data = data ?? new List<dynamic>();
        }
    }

    public class Table
    {
        public string? SourceTable { get; private set; }
        public string TargetTable { get; private set; }
        public List<Column> Columns { get; private set; }
        public Column this[int index]
        {
            get => Columns[index];
            private set => Columns[index] = value;
        }
        public void Add(Column column)
        {
            Columns.Add(column);
        }
        public Table(string targetTable)
        {
            TargetTable = targetTable;
            Columns = new List<Column>();
        }
        public Table(string sourceTable, string targetTable)
        {
            SourceTable = sourceTable;
            TargetTable = targetTable;
            Columns = new List<Column>();
        }
        public Table(string sourceTable, string targetTable, List<Column> columns)
        {
            SourceTable = sourceTable;
            TargetTable = targetTable;
            Columns = columns;
        }
        public bool ContainsSourceColumn(string sourceColumn)
        {
            return Columns.Any(column => column.SourceColumn == sourceColumn);
        }
        public bool ContainsTargetColumn(string targetHeader)
        {
            return Columns.Any(column => column.TargetColumn == targetHeader);
        }
    }
}