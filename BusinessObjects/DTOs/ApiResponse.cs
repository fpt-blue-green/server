﻿using BusinessObjects.Enum;

namespace BusinessObjects.DTOs
{
    public class ApiResponse<T>
    {
        public EHttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}