using Newtonsoft.Json;

namespace UpdateMetainfo
{
    public class Metadata
    {
        [JsonProperty("photoTakenTime")]
        private TimeClass? _photoTakenTime { get; init; }

        [JsonIgnore]
        public DateTime PhotoTime => DateTimeOffset.FromUnixTimeSeconds(_photoTakenTime?.Timestamp ?? 0).DateTime;

        public Metadata() { }
        public Metadata(TimeClass timeClass) : this()
        {
            _photoTakenTime = timeClass;
        }

        public class TimeClass
        {
            [JsonProperty("timestamp")]
            private string? _timestamp;

            [JsonIgnore]
            public long Timestamp => long.Parse(_timestamp ?? "0");

            public TimeClass() { }
            public TimeClass(long timestamp) : this()
            {
                _timestamp = timestamp.ToString();
            }
        }
    }
}
