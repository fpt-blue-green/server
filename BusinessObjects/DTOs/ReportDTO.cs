﻿namespace BusinessObjects
{
    public class ReportDTO
    {
        public Guid Id { get; set; }

        public Guid ReporterId { get; set; }

        public string ReporterName { get; set; }

        public Guid InfluencerId { get; set; }

        public string InfluencerName { get; set; }

        public EReportStatus? Reason { get; set; }

        public string Description { get; set; } = null!;

        public EReportStatus? ReportStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }

    public class ReportResponseDTO
    {
        public int TotalCount { get; set; }
        public IEnumerable<ReportDTO> Reports { get; set; }
    }
}
