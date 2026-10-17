namespace BusinessEntity.Api.Configuration
{
    public static class ApiEndpoints
    {
        public static class BusinessEntity
        {
            public const string Tag = nameof(BusinessEntity);

            public static class V1
            {
                public const string Base = "api/business-entities";

                public const string ById = "{id:int}";

                public const string GetByIdName = "GetBusinessEntityByIdV1";
            }
        }
    }
}
