using Microsoft.AspNetCore.Mvc;
using TaxiNT.Libraries.Entities;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        //Get API Server
        private readonly IBankService context;
        private readonly ILogger<BankController> logger;
        public BankController(IBankService _context, ILogger<BankController> _logger)
        {
            this.context = _context;
            this.logger = _logger;
        }


        [HttpGet("{bankId}")]
        public async Task<IActionResult> Get(string bankId)
        {
            try
            {
                var rs = await context.Get(bankId);
                if (rs == null) return NotFound();

                return Ok(rs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                                                "Error retrieving data from the database");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                var rs = await context.Gets();
                if (rs == null) return NotFound();

                return Ok(rs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                                                "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BankPostDto model)
        {
            try
            {
                var rs = await context.Post(model);
                if (rs == null) return NoContent();

                return Ok(rs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{bankId}")]
        public async Task<IActionResult> Patch([FromRoute] string bankId, [FromBody] BankPatchDto model)
        {
            try
            {
                var rs = await context.Patch(bankId, model);
                if (rs == null) return NoContent();

                return Ok(rs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{bankId}")]
        public async Task<IActionResult> Delete(string bankId)
        {
            try
            {
                var rs = await this.context.Delete(bankId);
                if (rs)
                    return Content($"{bankId}: has been deleted");
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("list")]
        public async Task<IActionResult> Deletes([FromBody] List<string> Ids)
        {
            try
            {
                var rs = await this.context.Deletes(Ids);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("upsert")]
        public async Task<IActionResult> Upserts([FromBody] List<BankUpsertDto> models)
        {
            try
            {
                var rs = await this.context.Upserts(models);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

