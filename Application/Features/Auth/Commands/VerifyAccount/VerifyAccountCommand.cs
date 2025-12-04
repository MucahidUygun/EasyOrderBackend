using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.VerifyAccount;

public class VerifyAccountCommand : IRequest
{
    public class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand>
    {


        public Task Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
