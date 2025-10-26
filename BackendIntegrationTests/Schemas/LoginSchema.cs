namespace BackendIntegrationTests.Schemas;
public static class LoginSchema
{
    public static readonly string Schema = @"{
        'type': 'object',
        'properties': {
            'token': {
                'type': 'string',
                'minLength': 1,
                'pattern': '^Bearer\\s+[A-Za-z0-9\\-_]+\\.[A-Za-z0-9\\-_]+\\.[A-Za-z0-9\\-_]+$',
                'description': 'JWT token with Bearer prefix'
            },
            'expiresAt': {
                'type': 'string',
                'format': 'date-time',
                'description': 'Token expiration date in ISO 8601 format'
            },
            'user': {
                'type': 'object',
                'properties': {
                    'name': {
                        'type': 'string',
                        'minLength': 1,
                        'description': 'User\'s first name'
                    },
                    'surname': {
                        'type': 'string',
                        'minLength': 1,
                        'description': 'User\'s last name'
                    }
                },
                'required': ['name', 'surname']
            },
            'message': {
                'type': 'string',
                'minLength': 1,
                'description': 'Response message'
            },
            'success': {
                'type': 'boolean',
                'description': 'Indicates if the login was successful'
            }
        },
        'required': ['token', 'expiresAt', 'user', 'message', 'success']
    }";
}
