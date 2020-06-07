using System;
using System.Collections.Generic;

namespace Cw3.ModelsGenerated
{
    public partial class Pokoj
    {
        public Pokoj()
        {
            Rezerwacja = new HashSet<Rezerwacja>();
        }

        public int NrPokoju { get; set; }
        public int IdKategoria { get; set; }
        public int LiczbaMiejsc { get; set; }

        public virtual Kategoria IdKategoriaNavigation { get; set; }
        public virtual ICollection<Rezerwacja> Rezerwacja { get; set; }
    }
}
