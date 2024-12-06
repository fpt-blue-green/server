using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using static BusinessObjects.AuthEnumContainer;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsers()
        {
            using (var context = new PostgresContext())
            {
                var users = await context.Users.Include(b => b.Brand).ToListAsync();
                return users;
            }
        }

        public async Task<IEnumerable<User>> GetUsersIgnoreFilter()
        {
            using (var context = new PostgresContext())
            {
                var users = await context.Users.Include(b => b.Brand).IgnoreQueryFilters().ToListAsync();
                return users;
            }
        }

        public async Task<IEnumerable<UserPaymentDTO>> GetAllUserPayments(Guid userID)
        {
            using (var context = new PostgresContext())
            {
                // First query: Get payment histories associated with the user
                var userPaymentHistories = await context.Users
                    .Where(u => u.Id == userID)
                    .SelectMany(user => user.PaymentHistories
                        .Where(u => u.Status != (int)EPaymentStatus.Error)
                        .Select(ph => new UserPaymentDTO
                        {
                            Created = ph.CreatedAt,
                            Amount = ph.Amount,
                            Status = ph.Status.HasValue ? (EPaymentStatus)ph.Status : EPaymentStatus.Pending,
                            Type = (EPaymentType)ph.Type
                        })
                    )
                    .ToListAsync();

                // Second query: Get payment bookings associated with the user's brand 
                var brandPaymentBookings = await context.Users
                    .Where(u => u.Id == userID && u.Brand != null)
                    .SelectMany(user => user.Brand!.Campaigns
                        .SelectMany(campaign => campaign.Jobs)
                        .SelectMany(job => job.PaymentBookings
                            .Where(pb => pb.Type != (int)EPaymentType.InfluencerPayment)
                            .Select(pb => new UserPaymentDTO
                            {
                                Created = pb.PaymentDate ?? DateTime.MinValue,
                                Amount = pb.Amount ?? 0,
                                Status = EPaymentStatus.Done,
                                Type = (EPaymentType)pb.Type!
                            })
                        )
                    )
                    .ToListAsync();

                // Third query: Get payment bookings associated with the user's influencer
                var influencerPaymentBookings = await context.Users
                    .Where(u => u.Id == userID && u.Influencer != null)
                    .SelectMany(user => user.Influencer!.Jobs
                        .SelectMany(job => job.PaymentBookings
                            .Where(pb => pb.Type == (int)EPaymentType.InfluencerPayment)
                            .Select(pb => new UserPaymentDTO
                            {
                                Created = pb.PaymentDate ?? DateTime.MinValue,
                                Amount = pb.Amount ?? 0,
                                Status = EPaymentStatus.Done,
                                Type = (EPaymentType)pb.Type!
                            })
                        )
                    )
                    .ToListAsync();


                // Concatenate the three results
                var combinedUserPayments = userPaymentHistories.Concat(brandPaymentBookings).Concat(influencerPaymentBookings);

                return combinedUserPayments;
            }
        }

        public async Task<IEnumerable<UserPaymentDTO>> GetUserPayments(Guid userID)
        {
            using (var context = new PostgresContext())
            {
                // First query: Get payment histories associated with the user
                var userPaymentHistories = await context.Users
                    .Where(u => u.Id == userID)
                    .SelectMany(user => user.PaymentHistories
                        .Where(u => u.Status == (int)EPaymentStatus.Done)
                        .Select(ph => new UserPaymentDTO
                        {
                            Created = ph.CreatedAt,
                            Amount = ph.Amount,
                            Status = ph.Status.HasValue ? (EPaymentStatus)ph.Status : EPaymentStatus.Pending,
                            Type = (EPaymentType)ph.Type
                        })
                    )
                    .ToListAsync();

                // Second query: Get payment bookings associated with the user's brand 
                var brandPaymentBookings = await context.Users
                    .Where(u => u.Id == userID && u.Brand != null)
                    .SelectMany(user => user.Brand!.Campaigns
                        .SelectMany(campaign => campaign.Jobs)
                        .SelectMany(job => job.PaymentBookings
                            .Where(pb => pb.Type != (int)EPaymentType.InfluencerPayment)
                            .Select(pb => new UserPaymentDTO
                            {
                                Created = pb.PaymentDate ?? DateTime.MinValue,
                                Amount = pb.Amount ?? 0,
                                Status = EPaymentStatus.Done,
                                Type = (EPaymentType)pb.Type!
                            })
                        )
                    )
                    .ToListAsync();

                // Third query: Get payment bookings associated with the user's influencer
                var influencerPaymentBookings = await context.Users
                    .Where(u => u.Id == userID && u.Influencer != null)
                    .SelectMany(user => user.Influencer!.Jobs
                        .SelectMany(job => job.PaymentBookings
                            .Where(pb => pb.Type == (int)EPaymentType.InfluencerPayment)
                            .Select(pb => new UserPaymentDTO
                            {
                                Created = pb.PaymentDate ?? DateTime.MinValue,
                                Amount = pb.Amount ?? 0,
                                Status = EPaymentStatus.Done,
                                Type = (EPaymentType)pb.Type!
                            })
                        )
                    )
                    .ToListAsync();


                // Concatenate the three results
                var combinedUserPayments = userPaymentHistories.Concat(brandPaymentBookings).Concat(influencerPaymentBookings);

                return combinedUserPayments;
            }
        }

        public async Task<IEnumerable<User>> GetInfluencerUsersWithPaymentHistory()
        {
            using (var context = new PostgresContext())
            {
                var users = await context.Users
                    .Include(b => b.Brand)
                    .Include(u => u.PaymentHistories)
                    .Where(u => u.Role == (int)ERole.Influencer).IgnoreQueryFilters()
                    .ToListAsync();
                return users;
            }
        }

        public async Task<IEnumerable<User>> GetBrandUsersWithPaymentHistory()
        {
            using (var context = new PostgresContext())
            {
                var users = await context.Users
                    .Include(b => b.Brand)
                    .Include(u => u.PaymentHistories)
                    .Where(u => u.Role == (int)ERole.Brand).IgnoreQueryFilters()
                    .ToListAsync();
                return users;
            }
        }

        public async Task<User> GetUserById(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.Influencer)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                return user!;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.Influencer)
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.UserDevices)
                    .FirstOrDefaultAsync(u => u.Email == email);
                return user!;
            }
        }

        public async Task<User> GetUserByLoginDTO(LoginDTO loginDTO)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.Influencer)
                    .Include(u => u.UserDevices)
                    .Where(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password)
                    .FirstOrDefaultAsync();
                return user!;
            }
        }

        public async Task CreateUser(User user)
        {
            using (var context = new PostgresContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<User>(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
        public async Task<User> GetUserByCampaignId(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Campaigns
                                         .Where(c => c.Id == campaignId)
                                         .Select(c => c.Brand.User)
                                         .FirstOrDefaultAsync();
                return user;
            }
        }

        public async Task<User> GetUserByInfluencerId(Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Influencers
                                         .Where(c => c.Id == influencerId)
                                         .Select(c => c.User)
                                         .FirstOrDefaultAsync();
                return user;
            }
        }

        public async Task DeleteUser(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                                         .Where(c => c.Id == userId)
                                         .FirstOrDefaultAsync();
                if (user != null)
                {
                    user.IsDeleted = true;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<User>> GetUserByNameOrEmail(string nameOrEmail)
        {
            using(var context = new PostgresContext())
            {
                return await context.Users.Where(s =>s.Role !=(int) ERole.Admin && (s.DisplayName.ToLower().Contains(nameOrEmail.ToLower()) )|| (s.Email.ToLower().Contains(nameOrEmail.ToLower()))).ToListAsync();
            }
        }
    }
}
