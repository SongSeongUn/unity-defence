using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using ExcelDataReader;
using UnityEngine;


namespace Editor
{
    public class TableParserToolWindow:EditorWindow
    {
        [MenuItem("Tools/데이터 변환 툴 (Excel To Json And Class)")]
        public static void ShowWindow()
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var window = GetWindow<TableParserToolWindow>("Table Converter");
            window.Show();
        }
        
        public string tableFolderPath = string.Empty; 
        public string jsonFolderPath = "Assets/GameData/JSON";
        
        private void OnEnable()
        {
            if (PlayerPrefs.HasKey("tableFolderPath"))
            {
                tableFolderPath = PlayerPrefs.GetString("tableFolderPath");
            }
        }
        private void OnGUI()
        {
            // Title & InfoBox 디자인 재현
            GUILayout.Label("폴더 일괄 변환 (xlsx -> JSON & Generate Class)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("지정한 폴더 내의 모든 xlsx 파일을 읽어 타입(숫자/문자/bool)에 맞게 Json과 Class로 적용 합니다.", MessageType.Info);
            
            EditorGUILayout.Space(10);

            // xlsx 폴더 선택 영역 (FolderPath 어트리뷰트 기능 구현)
            EditorGUILayout.BeginHorizontal();
            tableFolderPath = EditorGUILayout.TextField("xlsx 경로", tableFolderPath);
            if (GUILayout.Button("📂 선택", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("xlsx 폴더 선택", tableFolderPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // 유니티 프로젝트 내부 경로라면 상대 경로로 변환 (기존 호환성 유지)
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        selectedPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    tableFolderPath = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();

            // JSON 저장 경로 표시
            jsonFolderPath = EditorGUILayout.TextField("JSON 저장 경로", jsonFolderPath);

            EditorGUILayout.Space(15);

            // 🚀 모든 xlsx 변환 버튼 (GUIColor 녹색 계열 및 Large 사이즈 재현)
            GUI.backgroundColor = new Color(0f, 1f, 0.5f);
            if (GUILayout.Button("🚀 모든 xlsx JSON & Class으로 적용", GUILayout.Height(40)))
            {
                GUI.backgroundColor = Color.white; // 색상 리셋
                ConvertAllCSVs();
            }
            GUI.backgroundColor = Color.white; // 색상 리셋
        }
        
        
        public void ConvertAllCSVs()
        {
            if (!tableFolderPath.Contains("Table") || string.IsNullOrEmpty(tableFolderPath))
            {
                EditorUtility.DisplayDialog("에러", "xlsx 폴더 경로가 올바르지 않습니다.", "확인");
                return;
            }

            if (!Directory.Exists(jsonFolderPath)) Directory.CreateDirectory(jsonFolderPath);

            string[] files = Directory.GetFiles(tableFolderPath, "*.xlsx");
            int successCount = 0;

            foreach (string filePath in files)
            {
                if (filePath.Contains("Example") || filePath.Contains("~")) continue;
                if (ConvertSingleFile(filePath)) successCount++;
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("변환 완료", $"{successCount}개의 파일이 JSON & Class로 적용 되었습니다.", "확인");
            PlayerPrefs.SetString("tableFolderPath", tableFolderPath);
        }

        // ========================================================
        // 1. JSON 변환
        // ========================================================
        private bool ConvertSingleFile(string fullPath)
        {
            try
            {
                string tableName = "";
                List<Dictionary<string, object>> tableData = new List<Dictionary<string, object>>();
                using (var fs = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = ExcelReaderFactory.CreateReader(fs))
                    {
                        if (sr == null || sr.RowCount < 2) return false;

                        DataSet result = sr.AsDataSet(); // 전체 시트를 데이터셋으로 변환
                        DataTable table = result.Tables[0];    // 첫 번째 시트 선택
                        tableName = table.TableName;
                        
                        var columns =  table.Rows[0];
                        var types =  table.Rows[1];

                        for (var i = 2; i < table.Rows.Count; i++)
                        {
                            var rowData = table.Rows[i];
                            Dictionary<string, object> values =  new Dictionary<string, object>();
                            for (var j = 0; j < types.ItemArray.Length; j++)
                            {
                                if (types.ItemArray[j] == System.DBNull.Value)
                                    break;
                                
                                var value = ParseValue(types[j].ToString(),rowData.ItemArray[j].ToString());
                                values.Add(columns.ItemArray[j].ToString(), value);
                            }
                            
                            tableData.Add(values);
                        }

                        GenerateClassFile(table, tableName);
                    }
                }

                // JsonConvert가 object에 담긴 진짜 타입(int, float, string)을 보고 알아서 포맷팅해줍니다.
                string jsonResult = JsonConvert.SerializeObject(tableData, Formatting.Indented);

                string fileName = tableName +"Row" + ".json";
                string savePath = Path.Combine(jsonFolderPath, fileName);
                File.WriteAllText(savePath, jsonResult, Encoding.UTF8);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[생성 실패] {fullPath} : {e.Message}");
                return false;
            }
        }
        
        private object ParseValue(string type, string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            
            
            // 1. Bool 체크 (true / false)
            if (bool.TryParse(input, out bool bValue)) return bValue;
            
            // 2. Int 체크 (정수)
            if (int.TryParse(input, out int iValue)) return iValue;
            
            // 3. Float 체크 (소수점) - 다국어 설정(,) 문제 방지를 위해 InvariantCulture 사용
            if (float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float fValue)) 
                return fValue;
            
            // 4. 배열 체크
            if (type.Contains("List<int>")) 
                return JsonConvert.DeserializeObject<List<int>>(input);
            
            if(type.Contains("List<List<int>>"))
                return JsonConvert.DeserializeObject<List<List<int>>>(input);
            
            return input;
        }

        private void GenerateClassFile(DataTable table, string className)
        {
            StringBuilder sb = new StringBuilder();

            // 1. 네임스페이스 및 선언부 작성
            sb.Append("using System.Collections.Generic;");
            sb.AppendLine("\n");
            sb.AppendLine("namespace Table.Models");
            sb.AppendLine("{");
            sb.AppendLine("    [System.Serializable]");
            sb.AppendLine($"    public class {className}Row : ITableRow");
            sb.AppendLine("    {");
            
            // 2. 컬럼명(0행)과 타입(1행)을 읽어서 필드 생성
            // 엑셀 시트 기준: 0번째 가로줄은 컬럼명(No, Name...), 1번째 가로줄은 타입(int, string...)
            DataRow headerRow = table.Rows[0];
            DataRow typeRow = table.Rows[1];
            string key = headerRow[0].ToString();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                string columnName = headerRow[i].ToString();
                string typeName = typeRow[i].ToString();

                if (string.IsNullOrEmpty(columnName)) continue;

                // 예: public int No; 형태의 문자열 생성
                sb.AppendLine($"        public {typeName} {columnName};");
            }
            
            sb.AppendLine($"        public int Key => {key};");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            // 3. 파일로 저장 (유니티 프로젝트의 Assets 폴더 내부)
            string savePath = $"Assets/Scripts/Table/Models/{className}Row.cs";
            File.WriteAllText(savePath, sb.ToString());

            Debug.Log($"{className}.cs 파일이 생성되었습니다!\n 경로 : {savePath}");
        }
    }
}