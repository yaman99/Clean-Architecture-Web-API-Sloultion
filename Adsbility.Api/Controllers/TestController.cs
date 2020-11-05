using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adsbility.Appilication.Test.Commands;
using Adsbility.Appilication.Test.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adsbility.Api.Controllers
{
    public class TestController : ApiController
    {

        [HttpPost("Create")]
        public async Task<ActionResult<TestVm>> CreateTest([FromBody]CreateTest command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("GetById/{testId}")]
        public async Task<ActionResult<TestVm>> Get(int testId)
        {
            var result = await Mediator.Send(new TestQuery { TestId = testId });
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
