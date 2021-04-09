namespace JwtAndCookie.Libs
{
    public class MockClientRequest
    {
        public string Token { get; set; }

        public static MockClientRequest Instance = new MockClientRequest();
    }
}
