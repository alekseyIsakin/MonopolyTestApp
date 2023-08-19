using System;
using System.Text;
using System.Linq;

namespace ConsoleApp1
{
    public struct PalletDateGroup
    {
        public DateOnly Date;
        public List<Pallet> Pallets;
    }
    public class Selector
    {
        public static IEnumerable<Pallet> GetLongestStored(IEnumerable<Pallet> pallets, int count = 3)
        {
            return pallets
                .OrderByDescending(p => p.ExpirationDates.Max())
                .Take(count)
                .OrderBy(p => p.Volume);
        }
        public static IEnumerable<PalletDateGroup> GetOrganaizedPallets(IEnumerable<Pallet> pallets)
        {
            return pallets
                .OrderBy(p => p.ExpirationDate)
                .ThenBy(p => p.Weight)
                .GroupBy(p => p.ExpirationDate)
                .Select(group => new PalletDateGroup { Date = group.Key, Pallets = group.ToList() });
        }
    }
    public class Box
    {
        public Box(
            Guid id,
            int width, int length, int heigth,
            int weight,
            DateOnly? manufactDate,
            DateOnly? expirationDate)
        {
            this.Id = id;
            this.Width = width;
            this.Length = length;
            this.Heigth = heigth;
            this.Weight = weight;
            this._manufactDate = manufactDate;
            this._expirationDate = expirationDate;
        }

        public Box(
            int width, int length, int heigth,
            int weight,
            DateOnly? manufactDate,
            DateOnly? expirationDate)
            : this(Guid.NewGuid(), width, length, heigth, weight, manufactDate, expirationDate)
        { }
        public Box() { }

        private DateOnly? _manufactDate;
        private DateOnly? _expirationDate;

        public void SetExpirationDate(int year, int month, int day)
            => _expirationDate = new DateOnly(year, month, day);

        public void SetManufactionDate(int year, int month, int day)
            => _manufactDate = new DateOnly(year, month, day);


        public DateOnly ExpirationDate
        {
            get => _expirationDate ??
                   _manufactDate?.AddDays(100) ??
                   DateOnly.MinValue;
        }

        public Guid Id { get; }
        public int Width { get; }
        public int Length { get; }
        public int Heigth { get; }
        public int Weight { get; }
        public int Volume { get => Width * Length * Heigth; }
    }

    public class Pallet
    {
        public Pallet(Guid id, int width, int length, int heigth)
        {
            this.Id = id;
            this.Width = width;
            this.Length = length;
            this.P_heigth = heigth;

            _boxes = new List<Box>();
        }

        public Pallet(int width, int length, int heigth)
            : this(Guid.NewGuid(), width, length, heigth) { }

        private List<Box> _boxes { get; set; }
        public int P_heigth { get; }
        public int P_volume
        {
            get => Width * Length * P_heigth;
        }
        public int Boxes_height
        {
            get => _boxes.Aggregate(0, (heigth, box) => heigth + box.Heigth);
        }
        public int Boxes_weight
        {
            get => _boxes.Aggregate(0, (weight, box) => weight + box.Weight);
        }
        public int Boxes_volume
        {
            get => _boxes.Aggregate(0, (volume, box) => volume + box.Volume);
        }
        const int P_weight = 30;

        public DateOnly ExpirationDate
        {
            get => _boxes.Count > 0 ?
                _boxes.Min(box => box.ExpirationDate) :
                DateOnly.MinValue;
        }
        public List<DateOnly> ExpirationDates
        {
            get => _boxes
                .Select(b => b.ExpirationDate)
                .ToList();
        }

        public Guid Id { get; }
        public int Width { get; }
        public int Length { get; }
        public int Heigth
        {
            get => P_heigth + Boxes_height;
        }
        public int Weight
        {
            get => P_weight + Boxes_weight;
        }
        public int Volume
        {
            get => P_volume + Boxes_volume;
        }

        public List<Box> GetBoxes()
        {
            return _boxes;
        }
        public bool AddBox(Box box)
        {
            var can_hold = (box.Width <= Width && box.Length <= Length) ||
                           (box.Width <= Length && box.Width <= Length);

            if (can_hold)
            {
                _boxes.Add(box);
            }

            return can_hold;
        }
    }
}
