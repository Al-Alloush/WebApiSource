using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Core.SpecificationsQueries.Identity;
using Infrastructure.DataApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class UserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserAddressService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Current User' Address
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>return UserAddress if exist, else return null</returns>
        public async Task<UserAddress> FindAddressAsync(string userId)
        {
            var address = await _unitOfWork.Repository<UserAddress>().EntityAsync(new SpeciUser_UserAddress(userId));
            return address;
        }

        public async Task<UserAddress> UpdateUserAddress(UserAddress address)
        {
            if (_unitOfWork.Repository<UserAddress>().Update(address))
                if (await _unitOfWork.Repository<UserAddress>().SaveChangesAsync())
                    return address;

            return null;
        }

        public async Task<UserAddress> CreateUserAddress(UserAddressDto addressDto, string userId)
        {
            UserAddress address = _mapper.Map<UserAddressDto, UserAddress>(addressDto);
            address.UserId = userId;

            if (await _unitOfWork.Repository<UserAddress>().AddAsync(address))
                if (await _unitOfWork.Repository<UserAddress>().SaveChangesAsync())
                    return address;

            return null;
        }
    }
}
