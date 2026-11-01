using People.Domain.Entities;

namespace People.Api.Configuration
{
    public static class ApiEndpoints
    {
        public static class Person
        {
            public const string Tag = "People";

            public static class V1
            {
                public const string Base = "api/people";

                public const string ById = "{id:int}";

                public const string GetByIdName = "GetPersonByIdV1";
            }
        }
    }
}
