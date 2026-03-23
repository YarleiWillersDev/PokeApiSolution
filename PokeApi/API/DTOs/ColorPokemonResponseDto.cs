using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApi.API.DTOs
{
    public sealed class ColorPokemonResponseDto
    {
        [Required]
        public string Color { get; set; } = default!;

        [Required]
        public List<string> PokemonsNames { get; set; } = new();
    }
}