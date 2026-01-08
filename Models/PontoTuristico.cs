using System;
using System.ComponentModel.DataAnnotations;

namespace TurismoApp.Models
{
    public class PontoTuristico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome do ponto Turístico")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Data de Inclusão")]
        public DateTime DataInclusao { get; set; } = DateTime.Now;
    }
}

