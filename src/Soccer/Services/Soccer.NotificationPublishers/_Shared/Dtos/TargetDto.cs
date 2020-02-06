using System.Collections;

namespace Soccer.NotificationPublishers._Shared.Dtos
{
    public class TargetDto
    {      
        public string type { get; set; }

        public IEnumerable devices { get; set; }
    }
}
