using System.Text;

namespace ConsoleApp1
{
    public class App
    {
        public static void Main()
        {
            var rnd = new Random(0);

            // Create [n] pallets with random properties
            var pallets = new List<Pallet>();
            int n = 500;


            for (var i = 0; i < n; i++)
            {
                var p = new Pallet(
                    width: 10 + rnd.Next(3) * 5,
                    length: 10 + rnd.Next(3) * 5,
                    heigth: 10
                    );

                var boxes = new List<Box>();
                for (var j = 0; j < 2 + rnd.Next(8); j++)
                {
                    // Generate the date of manufacture of the box
                    // or/and generate expiration date of the box

                    DateOnly? manuf = null;
                    DateOnly? expir = null;
                    double mnf_chance = rnd.NextDouble();

                    if (mnf_chance <= .3 || mnf_chance >= .7)
                    {
                        manuf = new DateOnly(2020, 1, 1);
                        manuf = manuf?.AddDays(rnd.Next(20) * 5);
                    }
                    if (mnf_chance > .3)
                    {
                        expir = new DateOnly(2020, 6, 1);
                        expir = expir?.AddDays(rnd.Next(20) * 5);

                    }

                    Box b = new Box(
                        width: 10,
                        length: 2 + rnd.Next(20),
                        heigth: 1 + rnd.Next(40),
                        weight: 2 + rnd.Next(20),
                        manufactDate: manuf,
                        expirationDate: expir
                        );

                    // Add a box to the pallete
                    p.AddBox(b);
                }
                pallets.Add(p);
            }

            // Group pallets by expiration date
            // sort by ascending expiration date
            // then by weight
            var groups = Selector.GetOrganaizedPallets(pallets);

            // Display groups 
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Append($"\nGroup {group.Date}\n");
                foreach (var pallet in group.Pallets.OrderBy(p => p.Weight))
                {
                    sb.Append($"pal | [{pallet.Id}] {group.Date}, \nweight: {pallet.Weight}, volume: {pallet.Volume}, boxes: {pallet.GetBoxes().Count}\n");
                    foreach (var box in pallet.GetBoxes())
                    {
                        sb.Append($"\t\t[{box.ExpirationDate}] volume: {box.Volume},\tweight: {box.Weight}\n");
                    }
                }
            }

            // Select 3 pallets with the box having the longest shelf life
            var longest_shelf_life_pallets = Selector.GetPalletsWithLongestShelfLife(pallets, count: 3);

            sb.AppendLine();
            foreach (var pallet in longest_shelf_life_pallets)
            {
                sb.AppendLine($"[{pallet.Id}] {pallet.ExpirationDates.Max()}, volume: {pallet.Volume}");
            }

            Console.WriteLine(sb);
        }
    }
}
