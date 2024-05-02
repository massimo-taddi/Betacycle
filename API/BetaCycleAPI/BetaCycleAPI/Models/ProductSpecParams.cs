namespace BetaCycleAPI.Models
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;

        public int PageIndex { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Possible values: "priceAsc", "priceDesc"
        /// </summary>
        public string Sort { get; set; }

        private string search;

        public string Search
        {
            get => search;
            set => search = value.ToLowerInvariant();
        }

        public string Culture {  get; set; }
    }
}