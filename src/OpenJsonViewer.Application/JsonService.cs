using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace OpenJsonViewer.Application {
    public sealed class JsonService {
        #region PUBLIC_METHODS
        /// <summary>
        /// this methods read the json file
        /// from path and return it as json element
        /// </summary>
        /// <param name="path">json file path</param>
        /// <param name="root">readed converted data from json file</param>
        /// <param name="exception">return occured exception or null</param>
        /// <returns>return true if method success otherwise false</returns>
        public bool TryReadJsonFile(string path, out JsonElement root, out Exception exception) {
            bool result = false;
            exception = null;
            root = new JsonElement();
            try {
                bool isbadPath = string.IsNullOrWhiteSpace(path);
                if (isbadPath) {
                    exception = new ArgumentNullException("path is bad");
                    return false;
                }
                string jsonString = File.ReadAllText(path);
                JsonDocument doc = JsonDocument.Parse(jsonString);
                root = doc.RootElement;
                result = true;
            }
            catch (Exception ex) {
                exception = ex;
                exception.Source = this.GetType().Name;
                result = false;
            }
            return result;
        } 
        #endregion
    }
}
