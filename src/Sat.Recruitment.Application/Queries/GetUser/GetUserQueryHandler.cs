﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Application.Exceptions;
using Sat.Recruitment.Domain.Entities.UserAggregate;
using Sat.Recruitment.Domain.Interfaces.Data.Repositories;

namespace Sat.Recruitment.Application.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }        

        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var response = await _userRepository.GetUserByIdAsync(request.Id);

            if (response == null)
            {
                _logger.LogDebug("User not found");
                throw new NotFoundException("User not found");
            }
            
            response.SetEmail(NormalizeEmailAddress(response.Email));
            _logger.LogDebug("Email normalized");

            _logger.LogDebug("Response OK");
            return response;
        }

        private static string NormalizeEmailAddress(string email)
        {
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }
    }
}
