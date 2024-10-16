using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using OpenJsonViewer.Application;
using OpenJsonViewer.Presentation.Properties;

namespace OpenJsonViewer.Presentation {
    public partial class MainForm : Form {
        private readonly JsonService _jsonService;
        private string _fileFullPath;
        public MainForm() {
            InitializeComponent();
            _jsonService = new JsonService();
        }

        private void treeView_DragDrop(object sender, DragEventArgs e) {
            //Array fileNames;
            //bool isBadData = e.Data == null;
            //if (isBadData) { return; }
            //fileNames = (Array)e.Data.GetData(DataFormats.FileDrop);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e) {
            
        }

        private void MainForm_Load(object sender, EventArgs e) {
            imgLst_json.Images.Add("object", Resources.brackets);
            imgLst_json.Images.Add("array", Resources.bracket);
            imgLst_json.Images.Add("property", Resources.square);
        }


        private void LoadJsonToTreeView(string filePath) {
            Exception exception;
            JsonElement jsonElement;
            if (!_jsonService.TryReadJsonFile(filePath, out jsonElement, out exception)) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string fileName = Path.GetFileName(filePath);
            TreeNode titleNode = new TreeNode(fileName);
            treeView.Nodes.Add(titleNode);
            AddNode(jsonElement, titleNode);
        }


        private void AddNode(JsonElement element, TreeNode parentNode) {
            switch (element.ValueKind) {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject()) {
                        TreeNode objectNode = new TreeNode(property.Name) {
                            ImageKey = "object",
                            SelectedImageKey = "object"
                        };
                        parentNode.Nodes.Add(objectNode);
                        AddNode(property.Value, objectNode);
                    }
                    break;

                case JsonValueKind.Array:
                    TreeNode arrayNode = new TreeNode("Array") {
                        ImageKey = "array",
                        SelectedImageKey = "array"
                    };
                    parentNode.Nodes.Add(arrayNode);
                    foreach (JsonElement arrayElement in element.EnumerateArray()) {
                        AddNode(arrayElement, arrayNode);
                    }
                    break;

                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    TreeNode valueNode = new TreeNode(element.ToString()) {
                        ImageKey = "property",
                        SelectedImageKey = "property"
                    };
                    parentNode.Nodes.Add(valueNode);
                    break;

                default:
                    TreeNode unknownNode = new TreeNode("Unknown") {
                        ImageKey = "property",
                        SelectedImageKey = "property"
                    };
                    parentNode.Nodes.Add(unknownNode);
                    break;
            }
        }

        private void button_open_file_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "*.json|";
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    _fileFullPath = openFileDialog.FileName;
                    LoadJsonToTreeView(_fileFullPath);
                }
            }
        }
    }

}





//public class JsonTreeViewLoader {
//    public static void LoadJsonToTreeView(string filePath, TreeView treeView) {
//        // JSON dosyasını oku
//        string jsonString = File.ReadAllText(filePath);

//        // JSON'u JsonDocument'e parse et
//        using (JsonDocument doc = JsonDocument.Parse(jsonString)) {
//            // Kök elementi al ve TreeView'a ekle
//            JsonElement root = doc.RootElement;
//            TreeNode rootNode = new TreeNode("JSON Root");
//            treeView.Nodes.Add(rootNode);

//            // JSON'u TreeView'a ekleyen fonksiyon
//            AddNode(root, rootNode);
//        }
//    }

//    private static void AddNode(JsonElement element, TreeNode parentNode) {
//        switch (element.ValueKind) {
//            case JsonValueKind.Object:
//                foreach (JsonProperty property in element.EnumerateObject()) {
//                    TreeNode childNode = new TreeNode(property.Name);
//                    parentNode.Nodes.Add(childNode);
//                    AddNode(property.Value, childNode);  // Rekursif olarak alt düğümleri ekler
//                }
//                break;

//            case JsonValueKind.Array:
//                int index = 0;
//                foreach (JsonElement arrayItem in element.EnumerateArray()) {
//                    TreeNode arrayNode = new TreeNode($"Array Item {index}");
//                    parentNode.Nodes.Add(arrayNode);
//                    AddNode(arrayItem, arrayNode);
//                    index++;
//                }
//                break;

//            default:
//                parentNode.Nodes.Add(new TreeNode(element.ToString()));
//                break;
//        }
//    }
//}

