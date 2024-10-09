using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Timebook.Controls;

namespace Timebook.Helper
{
    public class Database
    {
        public Version VersionNumber { get; set; }
        public DateTime TimeStamp { get; set; }

        public Dictionary<Guid, ClassData> Classes { get; set; }
        public List<Guid> ClassOrder { get; set; }


        public Database()
        {
            VersionNumber = App.Version;
            TimeStamp = DateTime.Now;
            Classes = new Dictionary<Guid, ClassData>();
            ClassOrder = new List<Guid>();
        }

        [JsonConstructor]
        public Database(Version versionNumber,
                        DateTime timeStamp,
                        Dictionary<Guid, ClassData> classes,
                        List<Guid> classOrder)
        {
            VersionNumber = versionNumber;
            TimeStamp = timeStamp;
            Classes = classes;
            ClassOrder = classOrder;
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

        public static Guid CreateClassData()
        {
            ClassData classData = new ClassData();
            var id = GetNewID();

            Database.Classes.Add(id, classData);
            Database.ClassOrder.Add(id);

            return id;
        }

        public static ClassData GetClassData(Guid key)
        {
            return Database.Classes[key];
        }

        public static void SetClassData(Guid key, ClassData classData)
        {
            Database.Classes[key] = classData;
        }

        public static void RemoveClassData(Guid key)
        {
            Database.Classes.Remove(key);
            Database.ClassOrder.Remove(key);
        }

        public static List<Guid> GetClassOrder()
        {
            return Database.ClassOrder;
        }

        static public Guid GetNewID()
        {
            return Guid.NewGuid();
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
