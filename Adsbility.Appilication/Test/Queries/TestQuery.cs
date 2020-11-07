using Adsbility.Appilication.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adsbility.Appilication.Test.Queries
{
    public class TestQuery : IRequest<TestVm>
    {
        public int TestId { get; set; }
    }

    public class TestQueryHandler : IRequestHandler<TestQuery, TestVm>
    {
        private readonly IAppilicatioDbContext _context;
        private readonly IMapper _mapper;

        public TestQueryHandler(IAppilicatioDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TestVm> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Test.Where(r => r.Id == request.TestId).FirstOrDefaultAsync();
            var result = _mapper.Map<TestVm>(data);
            return result;
        }
    }
}
