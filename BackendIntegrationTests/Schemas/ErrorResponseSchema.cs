namespace BackendIntegrationTests.Schemas
{
    public class ErrorResponseSchema
    {
        public static string Schema => @"{
            'type': 'object',
            'properties': {
                'error': {
                    'type': 'string',
                    'minLength': 1
                },
                'message': {
                    'type': 'string',
                    'minLength': 1
                },
                'statusCode': {
                    'type': 'integer'
                }
            },
            'required': ['error', 'message'],
            'additionalProperties': false
        }";
    }
}
