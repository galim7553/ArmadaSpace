using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System;

public class ExcelConverter
{
    const string EXCEL_PATH = "Assets/Editor Resources/Info.xlsx";
    const string INFO_PATH = "Assets/Resources/info.json";
    const string LANG_PATH = "Assets/Resources/lang.json";
    const string BACKUP_INFO_PATH = "Assets/Editor Resources/Backup/info.json";
    const string BACKUP_LANG_PATH = "Assets/Editor Resources/Backup/lang.json";

    static Dictionary<string, Type> enumMappings = new Dictionary<string, Type>
    {
        { "cardType", typeof(CardType) },
        { "speciesType", typeof(SpeciesType)},
        { "abilityType", typeof(Ability.AbilityType)},
        { "chanceType", typeof(Ability.ChanceType)},
        { "targetType", typeof(Ability.TargetType)},
        { "rangeType", typeof(Ability.RangeType) },
        { "planetType", typeof(PlanetType) },
    };

    [MenuItem("Tools/Convert Excel to JSON")]
    public static void ConvertExcelToJson()
    {
        // ���� JSON ������ ��� ��η� �̵�
        BackupExistingJsonFiles();

        // Excel ������ �о DataSet���� ��ȯ
        DataSet excelData = ReadExcelFile(EXCEL_PATH);
        if (excelData != null)
        {
            // �� ��Ʈ���� JSON ���� ����
            CreateJsonFiles(excelData);

            Debug.Log($"���� ���� Json���� ��ȯ �Ϸ� infoPath: ({INFO_PATH}), langPath({LANG_PATH})");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("���� ���� ��ȯ ����");
        }
    }

    private static void BackupExistingJsonFiles()
    {
        if (File.Exists(INFO_PATH))
        {
            // ������ �̸��� ��� ������ �����ϴ� ��� ����
            if (File.Exists(BACKUP_INFO_PATH))
            {
                File.Delete(BACKUP_INFO_PATH);
            }
            File.Move(INFO_PATH, BACKUP_INFO_PATH);
        }

        if (File.Exists(LANG_PATH))
        {
            // ������ �̸��� ��� ������ �����ϴ� ��� ����
            if (File.Exists(BACKUP_LANG_PATH))
            {
                File.Delete(BACKUP_LANG_PATH);
            }
            File.Move(LANG_PATH, BACKUP_LANG_PATH);
        }
    }

    private static void CreateJsonFiles(DataSet dataSet)
    {
        var infoData = new Dictionary<string, List<Dictionary<string, object>>>();
        var langData = new Dictionary<string, List<Dictionary<string, object>>>();

        foreach (DataTable table in dataSet.Tables)
        {
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var rowData = new Dictionary<string, object>();

                foreach (DataColumn column in table.Columns)
                {
                    object value = row[column];

                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                    else if (value is double doubleValue)
                    {
                        if (doubleValue % 1 == 0)
                        {
                            value = Convert.ToInt32(doubleValue);
                        }
                        else
                        {
                            value = doubleValue;
                        }
                    }
                    else if (value is string strValue)
                    {
                        strValue = strValue.Trim();
                        strValue = strValue.Replace("\\n", "\n");

                        // �ش� ���� enum���� ���εǴ��� Ȯ��
                        if (enumMappings.TryGetValue(column.ColumnName, out var enumType))
                        {
                            if (Enum.TryParse(enumType, strValue, out var enumValue))
                            {
                                value = Convert.ToInt32(enumValue);
                            }
                            else
                            {
                                Debug.LogWarning($"Unknown value for {column.ColumnName}: {strValue}");
                                value = null; // �� �� ���� �� ó��
                            }
                        }
                        else if (IsJsonArray(strValue))
                        {
                             try
                             {
                                value = JsonConvert.DeserializeObject<List<int>>(strValue);
                             }
                            catch
                             {
                                try
                                {
                                    value = JsonConvert.DeserializeObject<List<string>>(strValue);
                                }
                                catch
                                {
                                    value = strValue;
                                }
                             }
                        }
                        else
                        {
                            value = strValue;
                        }
                    }
                    rowData[column.ColumnName] = value;
                }

                rows.Add(rowData);
            }

            // "Lang" ��Ʈ�� lang.json�� ����, �������� info.json�� ����
            if (table.TableName == "Lang")
            {
                langData["codeStrings"] = rows;
            }
            else
            {
                infoData[table.TableName] = rows;
            }
        }

        // JSON���� ��ȯ�� �� ���Ϸ� ����
        SaveJsonToFile(JsonConvert.SerializeObject(infoData, Formatting.Indented), INFO_PATH);
        SaveJsonToFile(JsonConvert.SerializeObject(langData, Formatting.Indented), LANG_PATH);
    }

    private static bool IsJsonArray(string input)
    {
        return input.StartsWith("[") && input.EndsWith("]");
    }

    private static void SaveJsonToFile(string json, string outputPath)
    {
        File.WriteAllText(outputPath, json);
    }

    private static DataSet ReadExcelFile(string filePath)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = a => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var result = reader.AsDataSet(conf);
                return result;
            }
        }
    }
}
