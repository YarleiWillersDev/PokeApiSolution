using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.Model
{
    [Table("PokemonSpecies")]
    public class PokemonSpecies
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int ColorId { get; set; }

        public PokemonColor? Color { get; set; }

        public PokemonSpecies(string name)
        {
            Name = name;
        }

        private PokemonSpecies() {}
    }
}