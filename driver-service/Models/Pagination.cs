namespace driver_service.Models
{
    public class Pagination
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }

        public Pagination(int offset = 1, int limit = 10, int total = 0)
        {
            Offset = offset;
            Limit = limit;
            Total = total;
        }
    }
}
