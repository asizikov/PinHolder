using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.ViewModel
{
    public class CardViewModel : UnsafeBaseViewModel
    {
        private int _pinDigits;

        private sealed class EmptyCardViewModel : CardViewModel
        {
            public EmptyCardViewModel()
            {
                Id = -1;
                Name = string.Empty;
                Description = string.Empty;
                PinItems = new List<PinItemViewModel>(0);
            }

            public override Card GetModel()
            {
                return null;
            }
        }

        public event Action ReadyToSave;

        [NotNull] private static readonly CardViewModel _empty = new EmptyCardViewModel();

        private readonly Random _random = new Random();
        private string _name;
        private string _description;
        private const int CELLS_NUM = 20;
        private const int PIN_SIZE = 4;

        public event Action NameOfDescriptionUpdated;

        public CardViewModel()
        {
            PinItems = new List<PinItemViewModel>();
            PinItems.AddRange(Enumerable.Range(0, CELLS_NUM)
                .Select(_ => new PinItemViewModel(OnUpdate)));
            _pinDigits = PIN_SIZE;
        }

        public CardViewModel(Card model)
        {
            Name = model.Name;
            Description = model.Description;
            Id = model.Id;
            PinItems = model.Pins.Select(p => new PinItemViewModel(p)).ToList();
        }

        public int Id { get; set; }

        [UsedImplicitly]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
                RaiseNameOrDescriptionUpdated();
            }
        }

        [UsedImplicitly]
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged("Description");
                RaiseNameOrDescriptionUpdated();
            }
        }

        [UsedImplicitly]
        public List<PinItemViewModel> PinItems { get; set; }

        public void SetPinCodeSize(int pinCodeSize)
        {
            _pinDigits = pinCodeSize;
            OnUpdate();
        }

        public static CardViewModel Empty
        {
            get { return _empty; }
        }

        private void OnUpdate()
        {
            if (PinItems.Count(pi => !string.IsNullOrEmpty(pi.Pin)) < _pinDigits) return;
            foreach (var pinItemViewModel in PinItems.Where(pi => string.IsNullOrEmpty(pi.Pin)))
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

        [CanBeNull]
        public virtual Card GetModel()
        {
            return new Card
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Pins = PinItems.Select(pi => pi.Pin).ToList()
            };
        }

        private void RaiseNameOrDescriptionUpdated()
        {
            if (NameOfDescriptionUpdated != null)
            {
                NameOfDescriptionUpdated();
            }
        }
    }
}