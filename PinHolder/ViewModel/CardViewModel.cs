using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PinHolder.Model;

namespace PinHolder.ViewModel
{
    public class CardViewModel : BaseViewModel
    {
        public event Action ReadyToSave;

        private readonly Random _random = new Random();
        private const int CELLS_NUM = 20;
        private const int PIN_SIZE = 4;

        public CardViewModel()
        {
            PinItems = new List<PinItemViewModel>();
#pragma warning disable 168
            foreach (var i in Enumerable.Range(0, CELLS_NUM))
#pragma warning restore 168
            {
                PinItems.Add(new PinItemViewModel(OnUpdate));
            }
        }

        public CardViewModel(Card model)
        {
            Name = model.Name;
            Description = model.Description;
            Id = model.Id;
            PinItems = model.Pins.Select(p => new PinItemViewModel(p)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PinItemViewModel> PinItems { get; set; }

        private void OnUpdate(PinItemViewModel item)
        {
            if (PinItems.Count(pi => !string.IsNullOrWhiteSpace(pi.Pin)) != PIN_SIZE) return;
            foreach (var pinItemViewModel in PinItems.Where(pi => string.IsNullOrWhiteSpace(pi.Pin)))
            {
                pinItemViewModel.SetSilently(GetRandomPinAsString());
                pinItemViewModel.RaiseChanged();
            }
            if (ReadyToSave != null)
            {
                ReadyToSave();
            }
            
        }

        private string GetRandomPinAsString()
        {
            return _random.Next(10).ToString(CultureInfo.InvariantCulture);
        }

        public Card GetModel()
        {
            return new Card
                {
                    Id = this.Id,
                    Name = this.Name,
                    Description = this.Description,
                    Pins = PinItems.Select(pi => pi.Pin).ToList()
                };
        }
    }
}