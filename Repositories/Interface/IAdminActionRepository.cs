﻿using BusinessObjects.Models;

namespace Repositories
{
    public interface IAdminActionRepository
    {
        Task Create(AdminAction detail);
    }
}
