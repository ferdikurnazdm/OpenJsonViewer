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
        #region PRIVATE_FIELDS
        private readonly JsonService _jsonService;
        private JsonElement _jsonElement;
        private string _fileFullPath;
        private string _fileSaltName;
        private string _fileBaseName;
        private bool _isCollapsed = true;
        #endregion

        #region CONSTRUCTOR
        public MainForm() {
            InitializeComponent();
            _jsonService = new JsonService();
        }
        #endregion

        #region PRIVATE_METHODS
        /// <summary>
        /// this method runs when form loading
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameter</param>
        private void MainForm_Load(object sender, EventArgs e) {
            imgLst_json.Images.Add("object", Resources.brackets);
            imgLst_json.Images.Add("array", Resources.bracket);
            imgLst_json.Images.Add("property", Resources.square);
        }
        /// <summary>
        /// this method load the json files
        /// to tree view from drag and drop
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameters</param>
        private void treeView_DragDrop(object sender, DragEventArgs e) {
            bool isBadDrag = e.Data == null ||
                !e.Data.GetDataPresent(DataFormats.FileDrop);
            if (isBadDrag) { return;}
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> jsonFiles = fileNames.Where(f => f.EndsWith(".json")).ToList();
            if (!jsonFiles.Any()) {
                MessageBox.Show("Please drop only JSON files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int jsonFilesCount = jsonFiles.Count;
            for (int i = 0; i < jsonFilesCount; i++){
                PrepareJsonData(jsonFiles[i]);
            }
        }
        /// <summary>
        /// this method runs when drag enter
        /// the tree view component
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameters</param>
        private void treeView_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.All;
            //treeView.BackColor = Color.LightGray;
        }
        /// <summary>
        /// this method runs when
        /// cursor leved from tree view
        /// for drag and drop feature
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameter</param>
        private void treeView_DragLeave(object sender, EventArgs e) {
            //treeView.BackColor = Color.White;
        }
        /// <summary>
        /// this method load the readed json object
        /// to tree view show message box occured any
        /// error state
        /// </summary>
        /// <param name="filePath">json file path</param>
        private void LoadJsonToTreeView(JsonElement jsonElement) {
            TreeNode titleNode = new TreeNode(_fileSaltName);
            treeView.Nodes.Add(titleNode);
            AddNode(jsonElement, titleNode);
        }
        /// <summary>
        /// this method loads json element to
        /// rich text box
        /// </summary>
        /// <param name="jsonElement">readed json element</param>
        private void LoadJsonToEditor(JsonElement jsonElement) {
            rchTxtBx_editor.AppendText(jsonElement.ToString()); ;
        }
        /// <summary>
        /// this method adds the parsed json 
        /// elements from readed json file
        /// the tree view as recursinly and
        /// also adds the element images
        /// </summary>
        /// <param name="element">parsed json element</param>
        /// <param name="parentNode">will added treeview</param>
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
        /// <summary>
        /// this method runs when open file button
        /// clicked then show the dialog.
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameters</param>
        private void button_open_file_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    _fileFullPath = openFileDialog.FileName;
                    //LoadJsonToTreeView(_fileFullPath);
                    PrepareJsonData(_fileFullPath);
                }
            }
        }
        /// <summary>
        /// this method firstly read json file
        /// and after convert the json element
        /// secondly set the global fields
        /// </summary>
        /// <param name="fileFullPath">will readed json file path</param>
        private void PrepareJsonData(string fileFullPath) {
            Exception exception;
            if (!_jsonService.TryReadJsonFile(fileFullPath, out _jsonElement, out exception)) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _fileSaltName = Path.GetFileName(fileFullPath);
            _fileBaseName = Path.GetDirectoryName(fileFullPath);
            LoadJsonToTreeView(_jsonElement);
            LoadJsonToEditor(_jsonElement);
        }
        /// <summary>
        /// this method collapse or expand
        /// all node on tree view
        /// </summary>
        /// <param name="sender">event trigger object</param>
        /// <param name="e">event parameters</param>
        private void btn_toggle_expand_Click(object sender, EventArgs e) {
            if (_isCollapsed) {
                treeView.ExpandAll();
                _isCollapsed = false;
                return;
            }
            treeView.CollapseAll();
            _isCollapsed = true;
        }

        #endregion


    }

}



