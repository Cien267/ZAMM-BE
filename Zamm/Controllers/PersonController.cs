using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Person;
using Zamm.Application.Payloads.ResultModels.Person;
using Zamm.Shared.Models;

namespace Zamm.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/people")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PersonResult>>> GetListPeopleAsync([FromQuery] PersonQuery query)
        {
            var result = await _personService.GetListPeopleAsync(query);
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonResult>> GetPersonByIdAsync(Guid id)
        {
            var result = await _personService.GetPersonByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PersonResult>> CreatePersonAsync([FromBody] CreatePersonInput request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _personService.CreatePersonAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PersonResult>> UpdatePersonAsync(Guid id, [FromBody] UpdatePersonInput request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _personService.UpdatePersonAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonAsync(Guid id)
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
    }
}
