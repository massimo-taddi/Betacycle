namespace BetaCycleAPI.Models
{
    public class ProductSpecParams:PaginatorParams
    {
        private string search;

        public string? Search
        {
            get => search;
            set => search = value == null ? string.Empty : value.ToLowerInvariant();
        }

    }
}