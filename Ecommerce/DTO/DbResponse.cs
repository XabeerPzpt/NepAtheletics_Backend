namespace Ecommerce.DTO
{
    public class DbResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
    public class DbResponseWithToken: DbResponse
    {
        public string Token { get; set; }
    }

    public static class StatusMapper
    {
        public const string PassWordRepeat= "409";
        public const string TechinicalErrror= "400";
    }
}
