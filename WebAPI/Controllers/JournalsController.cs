using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public JournalsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;            
            this.mapper = mapper;
        }
        [HttpPost("getRange")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRange([FromBody] JournalFilter filter, [FromQuery] int skip = 0, [FromQuery] int take = 0)
        {
            try
            {
                var ev = new Event()
                {
                    CreatedAt = DateTime.UtcNow,
                    Controller = ControllerContext.ActionDescriptor.ControllerName,
                    Action = ControllerContext.ActionDescriptor.ActionName
                };
                await context.Events.AddAsync(ev);
                await context.SaveChangesAsync();
                var journals = context.Journals
                    .Where(x => x.CreatedAt >= filter.From && x.CreatedAt <= filter.To);

                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    journals = journals.Where(x => x.Message.Contains(filter.Search));
                }
                if (skip > 0)
                {
                    journals = journals.Skip(skip);
                }
                if (take > 0)
                {
                    journals = journals.Take(take);
                }
                var result = await journals.Select(x => x).ToArrayAsync();
                if (journals == null)
                {
                    throw new ControllerException("Journals in this range have not been found", ev.Id);
                }
                return Ok(new JournalInfo
                { Count = journals.Count(), Skip = skip, Items = mapper.Map<IEnumerable<JournalDto>>(journals) });
            }
            catch (ControllerException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Exception",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }

        [HttpGet("getSingle")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSingle([FromQuery][Required] int id)
        {
            try
            {
                var ev = new Event()
                {
                    CreatedAt = DateTime.UtcNow,
                    Controller = ControllerContext.ActionDescriptor.ControllerName,
                    Action = ControllerContext.ActionDescriptor.ActionName
                };
                await context.Events.AddAsync(ev);
                await context.SaveChangesAsync();

                var journal = await context.Journals.FirstOrDefaultAsync(x => x.Id == id);
                if (journal == null)
                {                    
                    throw new ControllerException($"Journal with Id={id} has not been found", ev.Id);
                }
                return Ok(mapper.Map<JournalDto>(journal));
            }
            catch (ControllerException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Exception",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }
    }
}
