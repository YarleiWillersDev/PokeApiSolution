using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.Model
{
    [Table("PokemonColor")]
    public class PokemonColor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public ICollection<PokemonSpecies> PokemonSpecies { get; set; } = new List<PokemonSpecies>();
    }
}