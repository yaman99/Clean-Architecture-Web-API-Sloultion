using Adsbility.Appilication.Interfaces;
using Adsbility.Appilication.Test.Queries;
using Adsbility.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adsbility.Appilication.Test.Commands
{
    public class CreateTest : IRequest<TestVm>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateTestHandler : IRequestHandler<CreateTest , TestVm>
    {
        private readonly IAppilicatioDbContext _context;
        private readonly IMapper _mapper;
        public CreateTestHandler(IAppilicatioDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<TestVm> Handle(CreateTest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TestEnt>(request);
            _context.Test.Add(entity);
            var returnData = _mapper.Map<TestVm>(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return returnData;
        }
    }
}
