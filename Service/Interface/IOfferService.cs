using BusinessObjects;

namespace Service
{
    public interface IOfferService
    {
        Task CreateOffer(UserDTO userDTO, OfferCreateRequestDTO offerCreateRequestDTO);
        Task ReOffer(Guid id,UserDTO userDTO, ReOfferDTO reOfferDTO);
    }
}
