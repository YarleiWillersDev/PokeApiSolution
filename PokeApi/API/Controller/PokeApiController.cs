using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokeApi.API.DTOs;
using PokeApi.Application.Service;

namespace PokeApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokeApiController : ControllerBase
    {
        private readonly IPokemonService _service;

        public PokeApiController(IPokemonService pokemonService)
        {
            _service = pokemonService;
        }

        [HttpGet("{colorName}/pokemons")]
        public async Task<ActionResult<ColorPokemonResponseDto>> GetByColor(string colorName)
        {
            var result = await _service.GetByColorAsync(colorName);
            return Ok(result);
        }
    }
}