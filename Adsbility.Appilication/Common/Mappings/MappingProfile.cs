using Adsbility.Appilication.Test.Commands;
using Adsbility.Appilication.Test.Queries;
using Adsbility.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adsbility.Appilication.Mappings
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TestEnt,TestVm>();
            CreateMap<CreateTest, TestEnt>();
        }
    }
}
