using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("getRange")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRange([FromQuery] int? skip, [FromQuery] int? take, [FromBody] JournalFilter filter)
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
                
            if(!string.IsNullOrWhiteSpace(filter.Search))
            {
                journals = journals.Where(x => x.Message.Contains(filter.Search));
            }
            if(skip.HasValue)
            {
                journals = journals.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                journals = journals.Take(take.Value);
            }
            var result = await journals.Select(x => x).ToArrayAsync();
            if(journals == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException("Journals in this range have not been found", ev.Id);
            }
            return Ok(new JournalInfo 
            { Count = journals.Count(), Skip = skip ?? 0, Items = mapper.Map<IEnumerable<JournalDto>>(journals)});
        }

        [HttpGet("getSingle")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSingle([FromQuery] int id)
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
            if(journal == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException($"Journal with Id={id} has not been found", ev.Id);
            }
            return Ok(mapper.Map<JournalDto>(journal));
        }
    }
}
