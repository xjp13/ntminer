﻿using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class NTJsonSerializer : INTSerializer {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() {
            MissingMemberHandling = MissingMemberHandling.Ignore,// 默认值也是Ignore
            NullValueHandling = NullValueHandling.Ignore
        };

        public NTJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        public virtual TObject Deserialize<TObject>(string json) {
            return JsonConvert.DeserializeObject<TObject>(json, jsonSerializerSettings);
        }
    }
}
