using System.Collections.Generic;
using MediatR;
using Soccer.Core.Leagues.Models;

namespace Soccer.API.Leagues.Requests
{
    public class MajorLeaguesRequest : IRequest<IEnumerable<League>>
    {
    }
}