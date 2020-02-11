using System.Collections;

namespace Soccer.NotificationServices.AppCenter.Dtos
{
    public class TargetDto
    {      
        public string type { get; set; }

        public IEnumerable devices { get; set; }
    }
}
