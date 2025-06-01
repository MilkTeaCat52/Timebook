using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Timebook.Controls;

using ClassID = System.Guid;
using CellID = System.Guid;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timebook.Helper
{
    public class Database
    {
        public Version VersionNumber { get; set; }
        public DateTime TimeStamp { get; set; }

        public Dictionary<ClassID, ClassData> Classes { get; set; }
        public List<ClassID> ClassOrder { get; set; }

        public Dictionary<CellID, CellData> Cells { get; set; }
        public TableData Table { get; set; }


        public Database()
        {
            VersionNumber = App.Version;
            TimeStamp = DateTime.Now;
            Classes = new Dictionary<ClassID, ClassData>();
            ClassOrder = new List<ClassID>();
            Cells = new Dictionary<CellID, CellData>();
            Table = new TableData();
        }

        [JsonConstructor]
        public Database(Version versionNumber,
                        DateTime timeStamp,
                        Dictionary<ClassID, ClassData> classes,
                        List<ClassID> classOrder,
                        Dictionary<CellID, CellData> cells,
                        TableData table)
        {
            VersionNumber = versionNumber;
            TimeStamp = timeStamp;
            Classes = classes;
            ClassOrder = classOrder;
            Cells = cells;
            Table = table;

        }
    }

    public static class DataHelper
    {
        public static string StoragePath
        {
            get
            {
                return SettingHelper.StoragePathGet();
            }
        }

        public static Database Database = new Database();

        public static ClassID CreateClassData()
        {
            ClassData classData = new ClassData();
            var id = GetNewClassID();

            Database.Classes.Add(id, classData);
            Database.ClassOrder.Add(id);

            return id;
        }

        public static ClassData GetClassData(ClassID key)
        {
            return Database.Classes[key];
        }

        public static void SetClassData(ClassID key, ClassData classData)
        {
            Database.Classes[key] = classData;
        }

        public static void RemoveClassData(ClassID key)
        {
            Database.Classes.Remove(key);
            Database.ClassOrder.Remove(key);
        }

        public static List<ClassID> GetClassOrder()
        {
            return Database.ClassOrder;
        }

        public static void SetClassOrder(List<ClassID> newOrder)
        {
            Database.ClassOrder = new List<ClassID>(newOrder);
        }

        static public ClassID GetNewClassID()
        {
            return ClassID.NewGuid();
        }

        public static CellID CreateCellData()
        {
            CellData cellData = new CellData();
            var id = GetNewCellID();

            Database.Cells.Add(id, cellData);

            //set in table

            return id;
        }

        public static CellData GetCellData(CellID key)
        {
            return Database.Cells[key];
        }

        public static void SetCellData(CellID key, CellData cellData)
        {
            Database.Cells[key] = cellData;
        }

        /*public static void RemoveCellData(CellID key)
        {
            Database.Cells.Remove(key);
            //remove from table
        }*/

        public static void ResetCellData(CellID key)
        {
            Database.Cells[key] = new CellData();
        }

        static public CellID GetNewCellID()
        {
            return CellID.NewGuid();
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Database, options);

            var file = Path.Combine(StoragePath, "table.timebook");

            if (!File.Exists(file))
            {
                using (File.Create(file)) { }
            }
            File.WriteAllText(file, json);
        }
        public static void Load()
        {
            var file = Path.Combine(StoragePath, "table.timebook");

            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);

                Database = JsonSerializer.Deserialize<Database>(json);
            }
        }
    }
}
