﻿using System;
using Wiz.Biblioteca.API.ViewModels.Address;

namespace Wiz.Biblioteca.API.ViewModels.Customer
{
    public class CustomerAddressViewModel
    {
        public CustomerAddressViewModel()
        {
            Address = new AddressViewModel();
        }

        public CustomerAddressViewModel(int id, int addressId, string name, DateTime dateCreated, string cep)
        {
            Id = id;
            AddressId = addressId;
            Name = name;
            DateCreated = dateCreated;
            CEP = cep;
            Address = new AddressViewModel();
        }

        public int Id { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string CEP { get; set; }
        public AddressViewModel Address { get; set; }
    }
}
