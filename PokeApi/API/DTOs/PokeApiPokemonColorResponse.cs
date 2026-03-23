using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokeApi.API.DTOs
{
    public sealed class PokeApiPokemonColorResponse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        [JsonPropertyName("pokemon_species")]
        public List<PokeApiColors> PokemonSpecies { get; set; } = new List<PokeApiColors>();
    }
}