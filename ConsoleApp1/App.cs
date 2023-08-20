using System.Text;

namespace ConsoleApp1
{
    public class App
    {
        public static void Main()
        {
            var pallets = new List<Pallet>();
            var rnd = new Random(0);
            int n = 10;
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
                    Box b = new Box(
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
                Console.SetCursorPosition(0, 0);

            }

            var groups = Selector.GetOrganaizedPallets(pallets);

            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Append($"\nGroup {group.Date}\n");
                foreach (var pallet in group.Pallets.OrderBy(p => p.Weight))
                {
                    sb.Append($"pal | [{pallet.Id}] {group.Date}, weight: {pallet.Weight}, boxes: {pallet.GetBoxes().Count}\n");
                    foreach (var dt in pallet.ExpirationDates)
                    {
                        sb.Append($"\t{dt}\n");
                    }
                }
            }
            var most_longest_pallets = Selector.GetLongestStored(pallets);

            foreach (var pallet in most_longest_pallets)
            {
                sb.AppendLine($"[{pallet.Id}] {pallet.ExpirationDates.Max()}, volume: {pallet.Volume}");
            }

            Console.WriteLine(sb);
        }
    }
}
