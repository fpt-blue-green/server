using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class SystemSetting
{
    public Guid Id { get; set; }

    public string KeyName { get; set; }

    public string KeyValue { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
