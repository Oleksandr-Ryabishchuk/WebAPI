using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
            // I think standart try cathch scheme is a little verbose so I created GlobalExceptionMiddleware
            // for handling Exceptions in more simple manner 
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
                var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == treeName);
                if (tree == null)
                {
                    throw new ControllerException($"Tree with name={treeName} has not been found", ev.Id);
                }
                return Ok(mapper.Map<TreeDto>(tree));
            }
            catch (SecureException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Secure",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> Create([FromQuery][Required] string treeName, 
                                                [FromQuery][Required] string nodeName, 
                                                [FromQuery] int? parentNodeId) 
        // in this case Query used just for example purpose
        {
            try
            {
                var ev = new Event()
                {
                    CreatedAt = DateTime.UtcNow,
                    Controller = ControllerContext.ActionDescriptor.ControllerName,
                    Action = ControllerContext.ActionDescriptor.ActionName
                };
                await context.AddAsync(ev);
                await context.SaveChangesAsync();
                var dto = new NodeCreateDto()
                {
                    TreeName = treeName,
                    Name = nodeName,
                    ParentId = parentNodeId
                };
                var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == dto.TreeName);

                // We assume that if a tree doesn't exist then we create it

                if (tree == null)
                {
                    tree = new Entities.Tree()
                    {
                        Name = dto.TreeName
                    };
                    context.Add(tree);
                    await context.SaveChangesAsync();
                }
                var validity = service.CheckIfValid(tree, dto);
                if (!validity.Valid)
                {                    
                    throw new ControllerException(validity.Reason, ev.Id);
                }

                var model = mapper.Map<Node>(dto, opts =>
                {
                    opts.Items[nameof(Node.TreeId)] = tree.Id;
                    opts.Items[nameof(Node.Tree)] = tree;
                });
                await context.AddAsync(model);
                
                var countResult = await context.SaveChangesAsync();
                if (countResult < 1)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return Ok();
            }
            catch (SecureException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Secure",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
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

        [HttpPatch]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(NodeCreateDto dto)
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
                var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == dto.TreeName);

                // We assume that tree should exist on this stage
                if (tree == null)
                {                    
                    throw new ControllerException($"Tree with name={dto.TreeName} has not been found", ev.Id);
                }

                var node = tree.Children.FirstOrDefault(x => x.Id == dto.Id);

                // as well as node 
                if (node == null)
                {                    
                    throw new ControllerException($"Node with Id={dto.Id} has not been found", ev.Id);
                }
                var validity = service.CheckIfValid(tree, dto);
                if (!validity.Valid && validity.Reason != $"Node with Id={dto.ParentId} already has a child")
                {                    
                    throw new ControllerException(validity.Reason, ev.Id);
                }

                mapper.Map(dto, node, opts =>
                {
                    opts.Items[nameof(Node.TreeId)] = tree.Id;
                    opts.Items[nameof(Node.Tree)] = tree;
                });

                var countResult = await context.SaveChangesAsync();
                if (countResult < 1)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return Ok();
            }
            catch (SecureException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Secure",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
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

        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromQuery] string treeName, [FromQuery] int id)
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
                var tree = context.Trees.Include(a => a.Children).FirstOrDefault(a => a.Name == treeName);

                // We assume that tree should exist on this stage
                if (tree == null)
                {
                    throw new ControllerException($"Tree with name={treeName} has not been found", ev.Id);
                }

                var node = tree.Children.FirstOrDefault(x => x.Id == id);

                // as well as node 
                if (node == null)
                {
                    throw new ControllerException($"Node with Id={id} has not been found", ev.Id);
                }

                var child = tree.Children.FirstOrDefault(x => x.ParentId == node.Id);
                if (child != null)
                {                    
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
            catch (SecureException ex)
            {
                await context.Journals.AddAsync(new Journal()
                {
                    Type = "Secure",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
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
