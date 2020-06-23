using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Wiz.Biblioteca.API.ViewModels.Customer
{
    public class CustomerIdViewModel
    {
        public CustomerIdViewModel() { }

        public CustomerIdViewModel(int id)
        {
            Id = id;
        }

        [FromRoute(Name = "id")]
        [Required(ErrorMessage = "Id é obrigatório")]
        public int Id { get; set; }
    }
}
