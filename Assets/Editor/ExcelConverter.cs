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
        // 기존 JSON 파일을 백업 경로로 이동
        BackupExistingJsonFiles();

        // Excel 파일을 읽어서 DataSet으로 변환
        DataSet excelData = ReadExcelFile(EXCEL_PATH);
        if (excelData != null)
        {
            // 각 시트별로 JSON 파일 생성
            CreateJsonFiles(excelData);

            Debug.Log($"엑셀 파일 Json으로 변환 완료 infoPath: ({INFO_PATH}), langPath({LANG_PATH})");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("엑셀 파일 변환 실패");
        }
    }

    private static void BackupExistingJsonFiles()
    {
        if (File.Exists(INFO_PATH))
        {
            // 동일한 이름의 백업 파일이 존재하는 경우 삭제
            if (File.Exists(BACKUP_INFO_PATH))
            {
                File.Delete(BACKUP_INFO_PATH);
            }
            File.Move(INFO_PATH, BACKUP_INFO_PATH);
        }

        if (File.Exists(LANG_PATH))
        {
            // 동일한 이름의 백업 파일이 존재하는 경우 삭제
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

                        // 해당 열이 enum으로 매핑되는지 확인
                        if (enumMappings.TryGetValue(column.ColumnName, out var enumType))
                        {
                            if (Enum.TryParse(enumType, strValue, out var enumValue))
                            {
                                value = Convert.ToInt32(enumValue);
                            }
                            else
                            {
                                Debug.LogWarning($"Unknown value for {column.ColumnName}: {strValue}");
                                value = null; // 알 수 없는 값 처리
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

            // "Lang" 시트는 lang.json에 저장, 나머지는 info.json에 저장
            if (table.TableName == "Lang")
            {
                langData["codeStrings"] = rows;
            }
            else
            {
                infoData[table.TableName] = rows;
            }
        }

        // JSON으로 변환한 후 파일로 저장
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
