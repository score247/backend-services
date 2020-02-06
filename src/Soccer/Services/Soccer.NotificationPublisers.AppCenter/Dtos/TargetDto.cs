using System.Collections;

namespace Soccer.NotificationPublisers.AppCenter.Dtos
{
    public class TargetDto
    {      
        public string type { get; set; }

        public IEnumerable devices { get; set; }
    }
}
