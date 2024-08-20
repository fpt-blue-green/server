﻿namespace BusinessObjects.DTOs.UserDTOs
{
    public class ImagesDTO
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = null!;

        public string? Description { get; set; }

        public int? Type { get; set; } // Create an Enum
    }
}
