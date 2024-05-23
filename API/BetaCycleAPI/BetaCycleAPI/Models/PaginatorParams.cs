namespace BetaCycleAPI.Models
{
    public class PaginatorParams
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
    }
}
