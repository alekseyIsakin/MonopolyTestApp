using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal interface Cargo
    {
        public Guid id { get; }
        public int width { get; }
        public int length { get; }
        public int heigth { get; }
        public int weight { get; }
        public int volume { get; }
    }

    internal class Box : Cargo
    {
        public Box(
            Guid id,
            int width, int length, int heigth,
            int weight,
            DateOnly? manufactDate,
            DateOnly? expirationDate)
        {
            this.id = id;
            this.width = width;
            this.length = length;
            this.heigth = heigth;
            this.weight = weight;
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

        public Guid id { get; }
        public int width { get; }
        public int length { get; }
        public int heigth { get; }
        public int weight { get; }
        public int volume { get => width * length * heigth; }
    }

    internal class Pallet : Cargo
    {
        public Pallet(Guid id, int width, int length, int heigth)
        {
            this.id = id;
            this.width = width;
            this.length = length;
            this.P_heigth = heigth;

            _boxes = new List<Box>();
        }

        public Pallet(int width, int length, int heigth)
            : this(Guid.NewGuid(), width, length, heigth) { }

        private List<Box> _boxes { get; set; }
        public int P_heigth { get; }
        public int P_volume
        {
            get => width * length * P_heigth;
        }
        public int Boxes_height
        {
            get => _boxes.Aggregate(0, (heigth, box) => heigth + box.heigth);
        }
        public int Boxes_weight
        {
            get => _boxes.Aggregate(0, (weight, box) => weight + box.weight);
        }
        public int Boxes_volume
        {
            get => _boxes.Aggregate(0, (volume, box) => volume + box.volume);
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

        public Guid id { get; }
        public int width { get; }
        public int length { get; }
        public int heigth
        {
            get => P_heigth + Boxes_height;
        }
        public int weight
        {
            get => P_weight + Boxes_weight;
        }
        public int volume
        {
            get => P_volume + Boxes_volume;
        }

        public List<Box> GetBoxes()
        {
            return _boxes;
        }
        public bool AddBox(Box box)
        {
            var can_hold = (box.width <= width && box.length <= length) ||
                           (box.width <= length && box.width <= length);

            if (can_hold)
            {
                _boxes.Add(box);
            }

            return can_hold;
        }
    }
    public class App
    {
        public static void Main()
        {
            var pallets = new List<Pallet>();
            var rnd = new Random(0);
            int n = 10;
            Box b = new Box();
            for (var i = 0; i < n; i++)
            {
                var p = new Pallet(
                    width: 10 + rnd.Next(3) * 5,
                    length: 10 + rnd.Next(3) * 5,
                    heigth: 10
                    );



                List<Box> boxes = new List<Box>();
                for (var j = 0; j < 2 + rnd.Next(8); j++)
                {
                    DateOnly? manuf = null;
                    DateOnly? expir = null;
                    double mnf_chance = rnd.NextDouble();

                    if (mnf_chance <= .3 || mnf_chance >= .7)
                    {
                        manuf = new DateOnly(2020, 1, 1);
                        manuf = manuf?.AddDays(rnd.Next(5) * 20);
                    }
                    if (mnf_chance > .3)
                    {
                        expir = new DateOnly(2020, 6, 1);
                        expir = expir?.AddDays(rnd.Next(5) * 20);

                    }
                    b = new Box(
                        width: 10,
                        length: 2 + rnd.Next(20),
                        heigth: 1 + rnd.Next(40),
                        weight: 2 + rnd.Next(20),
                        manufactDate: manuf,
                        expirationDate: expir
                        );
                    p.AddBox(b);
                }
                pallets.Add(p);
                //if (i % (n / 50) == 0)
                //    Console.WriteLine($"{i/(n/100)}%     ");
                Console.SetCursorPosition(0, 0);
            }

            var all = pallets.OrderBy(p => p.ExpirationDate)
                .ThenBy(p => p.weight)
                .GroupBy(p => p.ExpirationDate);

            var sb = new StringBuilder();
            foreach (var date in all)
            {
                sb.Append($"\nGroup {date.Key}\n");
                foreach (var pallet in date.OrderBy(p => p.weight))
                {
                    sb.Append($"pal | [{pallet.id}] {date.Key}, weight: {pallet.weight}, boxes: {pallet.GetBoxes().Count}\n");
                    foreach (var dt in pallet.ExpirationDates)
                    {
                        sb.Append($"\t{dt}\n");
                    }
                }
            }
            sb.Append($"\n\n\n");

            var long_stored_p = pallets
                .OrderByDescending(p => p.ExpirationDates.Max())
                .Take(3)
                .OrderBy(p => p.volume);
            foreach (var pallet in long_stored_p)
            {
                sb.Append($"pal | [{pallet.id}] {pallet.ExpirationDates.Max()}, weight: {pallet.weight}, boxes: {pallet.volume}\n");
                foreach (var dt in pallet.ExpirationDates)
                {
                    sb.Append($"\t{dt}\n");
                }
            }
            Console.WriteLine(sb);
        }
    }
}
