using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Exceptions;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly INodeService service;
        private readonly IMapper mapper;
        public NodesController(ApplicationDbContext context,
            INodeService service,
            IMapper mapper)
        {
            this.context = context;
            this.service = service;
            this.mapper = mapper;
        }


        [HttpGet("treeName")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string treeName)
        {
            var ev = new Event()
            {
                CreatedAt = DateTime.UtcNow,
                Controller = ControllerContext.ActionDescriptor.ControllerName,
                Action = ControllerContext.ActionDescriptor.ActionName
            };
            await context.Events.AddAsync(ev);
            await context.SaveChangesAsync();
            var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == treeName);
            if(tree == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException($"Tree with name={treeName} has not been found", ev.Id);
            }
            return Ok(mapper.Map<TreeDto>(tree));
        }
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> Create(NodeCreateDto dto) 
        {
            var ev = new Event()
            {
                CreatedAt = DateTime.UtcNow,
                Controller = ControllerContext.ActionDescriptor.ControllerName,
                Action = ControllerContext.ActionDescriptor.ActionName
            };
            await context.Events.AddAsync(ev);
            await context.SaveChangesAsync();
            var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == dto.TreeName);

            // We assume that if a tree doesn't exist then we create it

            if (tree == null)
            {
                tree = new Entities.Tree() 
                {
                    Name = dto.Name
                };
                context.Trees.Add(tree);
                await context.SaveChangesAsync();
            }
            var validity = service.CheckIfValid(tree, dto);
            if (!validity.Valid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException(validity.Reason, ev.Id);                
            }
            
            var model = mapper.Map<Node>(dto, opts =>
            {
                opts.Items[nameof(Node.TreeId)] = tree.Id;
            });
            await context.Nodes.AddAsync(model);
            tree.Children.Add(model);
            var countResult = await context.SaveChangesAsync();
            if(countResult < 2)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpPatch]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(NodeCreateDto dto)
        {
            var ev = new Event()
            {
                CreatedAt = DateTime.UtcNow,
                Controller = ControllerContext.ActionDescriptor.ControllerName,
                Action = ControllerContext.ActionDescriptor.ActionName
            };
            await context.Events.AddAsync(ev);
            await context.SaveChangesAsync();
            var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == dto.TreeName);
            
            // We assume that tree should exist on this stage
            if (tree == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException($"Tree with name={dto.TreeName} has not been found", ev.Id);
            }

            var node = tree.Children.FirstOrDefault(x => x.Id == dto.Id);

            // as well as node 
            if (node == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException($"Node with Id={dto.Id} has not been found", ev.Id);
            }
            var validity = service.CheckIfValid(tree, dto);
            if (!validity.Valid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new ControllerException(validity.Reason, ev.Id);
            }

            mapper.Map(dto, node, opts =>
            {
                opts.Items[nameof(Node.TreeId)] = tree.Id;
            });
            
            var countResult = await context.SaveChangesAsync();
            if (countResult < 1)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(NodeCreateDto dto)
        {
            var ev = new Event()
            {
                CreatedAt = DateTime.UtcNow,
                Controller = ControllerContext.ActionDescriptor.ControllerName,
                Action = ControllerContext.ActionDescriptor.ActionName
            };
            await context.Events.AddAsync(ev);
            await context.SaveChangesAsync();
            var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == dto.TreeName);

            // We assume that tree should exist on this stage
            if (tree == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var node = tree.Children.FirstOrDefault(x => x.Id == dto.Id);

            // as well as node 
            if (node == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
            var child = tree.Children.FirstOrDefault(x => x.ParentId == node.Id);
            if (child != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new SecureException("You have to delete all children nodes first", ev.Id);
            }

            // We can implement soft delete instead of phisical removing of the node from database
            // by adding nullable field DateTime DeletedAt to entity and filtering then by this field
            context.Nodes.Remove(node);
            var countResult = await context.SaveChangesAsync();
            if (countResult < 1)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
