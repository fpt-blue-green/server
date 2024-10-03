using BusinessObjects;

namespace Service
{
    public interface IOfferService
    {
        Task CreateOffer(UserDTO userDTO, OfferCreateRequestDTO offerCreateRequestDTO);
    }
}
