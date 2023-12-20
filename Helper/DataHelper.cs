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

        public Dictionary<Guid, GroupData> Groups { get; set; }
        public List<Guid> GroupOrder { get; set; }


        public Database()
        {
            VersionNumber = App.Version;
            TimeStamp = DateTime.Now;
            Groups = new Dictionary<Guid, GroupData>();
            GroupOrder = new List<Guid>();
        }

        [JsonConstructor]
        public Database(Version versionNumber,
                        DateTime timeStamp,
                        Dictionary<Guid, GroupData> groups,
                        List<Guid> groupOrder)
        {
            VersionNumber = versionNumber;
            TimeStamp = timeStamp;
            Groups = groups;
            GroupOrder = groupOrder;
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

        public static GroupData CreateGroupData()
        {
            GroupData groupData = new GroupData();
            var id = GetNewID();

            Database.Groups.Add(id, groupData);
            Database.GroupOrder.Add(id);

            return groupData;
        }

        public static GroupData GetGroupData(Guid key)
        {
            return Database.Groups[key];
        }

        public static List<Guid> GetGroupOrder()
        {
            return Database.GroupOrder;
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
