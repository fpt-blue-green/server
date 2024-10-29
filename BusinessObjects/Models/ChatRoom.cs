using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ChatRoom
{
    public long Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public string? Message { get; set; }

    public DateTime? DateSent { get; set; }

    public string? SenderName { get; set; }

    public string? ReceiverName { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
