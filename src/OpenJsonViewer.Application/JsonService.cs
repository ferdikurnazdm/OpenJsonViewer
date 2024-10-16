using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace OpenJsonViewer.Application {
    public sealed class JsonService {

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

    }
}
